using UnityEngine;
using System.Collections.Generic;

#if APP_UPDATED

namespace Pathfinding.RVO.Sampled
{
    public interface IAgentExtend
    {
        /* this is the velocity which has been computed, but won't be applied until next frame (could be though of as the on-deck velocity) */
        Vector3 NewVelocity { get; }

        // added AgentsToCollideWith, AgentCollisionType, radiusAgainstObstacles
        /** the types of other agents which can be collided with */
        LayerMask AgentsToCollideWith { get; set; }

        /** the type of this agent for collision purposes */
        LayerMask AgentCollisionType { get; set; }

        /** the radius of the agent when looking for obstacle avoidance, seperate from radius as the agents may 
         * like to stay a different distance to obstacle than other agents */
        float RadiusAgainstObstacles { get; set; }

        void Teleport(Vector3 pos, bool doResetVelocity);
    }

    public partial class Agent : IAgentExtend
    {
        public float radiusAgainstObstacles;
        public LayerMask agentsToCollideWith, agentCollisionType;

        public LayerMask AgentCollisionType
        {
            get; set;
        }

        public LayerMask AgentsToCollideWith
        {
            get; set;
        }

        public Vector3 NewVelocity
        {
            get; set;
        }

        public float RadiusAgainstObstacles
        {
            get; set;
        }

        public List<ObstacleVertex> NeighbourObstacles
        {
            get { return obstaclesBuffered; }
        }

        public Agent(Vector3 pos)
        {
            MaxSpeed = 2;
            NeighbourDist = 15;
            AgentTimeHorizon = 2;
            ObstacleTimeHorizon = 2;
            Height = 5;
            Radius = 5;
            MaxNeighbours = 10;
            Locked = false;

            position = pos;
            Position = position;
            prevSmoothPos = position;
            smoothPos = position;

            Layer = RVOLayer.DefaultAgent;
            CollidesWith = (RVOLayer)(-1);

#if BNICKSON_UPDATED
            // added AgentCollisionType, AgentsToCollideWith
            AgentCollisionType = AgentsToCollideWith = -1;

            // added radiusAgainstObstacles
            RadiusAgainstObstacles = Radius = 5;
#endif
        }

        public void BufferSwitch()
        {
            // <==
            radius = Radius;
            height = Height;
            maxSpeed = MaxSpeed;
            neighbourDist = NeighbourDist;
            agentTimeHorizon = AgentTimeHorizon;
            obstacleTimeHorizon = ObstacleTimeHorizon;
            maxNeighbours = MaxNeighbours;
            desiredVelocity = DesiredVelocity;
            locked = Locked;
            collidesWith = CollidesWith;
            layer = Layer;

            //position = Position;

            // ==>
            Velocity = velocity;
            List<ObstacleVertex> tmp = obstaclesBuffered;
            obstaclesBuffered = obstacles;
            obstacles = tmp;

#if BNICKSON_UPDATED
            agentsToCollideWith = AgentsToCollideWith;
            agentCollisionType = AgentCollisionType;
            radiusAgainstObstacles = RadiusAgainstObstacles;
#endif
        }

        public void Teleport(Vector3 pos)
        {
#if BNICKSON_UPDATED
            Teleport(pos, true);
#else
            Position = pos;
            smoothPos = pos;
            prevSmoothPos = pos;
#endif
        }

        public void Teleport(Vector3 pos, bool doResetVelocity)
        {
            Position = pos;
            smoothPos = pos;
            prevSmoothPos = pos;

#if BNICKSON_UPDATED
            // after a teleport we don't want any velocity or positions still hanging around from before the teleport
            position = pos;
            if (doResetVelocity)
            {
                DesiredVelocity = desiredVelocity = newVelocity = new Vector3(0, 0, 0);
            }
#endif
        }

        internal void CalculateVelocity(Pathfinding.RVO.Simulator.WorkerContext context)
        {
            if (locked)
            {
                newVelocity = Vector2.zero;
                return;
            }

            if (context.vos.Length < neighbours.Count + simulator.obstacles.Count)
            {
                context.vos = new VO[Mathf.Max(context.vos.Length * 2, neighbours.Count + simulator.obstacles.Count)];
            }

            Vector2 position2D = new Vector2(position.x,position.z);

            var vos = context.vos;
            var voCount = 0;

            Vector2 optimalVelocity = new Vector2(velocity.x, velocity.z);

            float inverseAgentTimeHorizon = 1.0f/agentTimeHorizon;

            //#if !BNICKSON_UPDATED
            float wallThickness = simulator.WallThickness;

            float wallWeight = simulator.algorithm == Simulator.SamplingAlgorithm.GradientDecent ? 1 : WallWeight;

            for (int i = 0; i < simulator.obstacles.Count; i++)
            {
                var obstacle = simulator.obstacles[i];
                var vertex = obstacle;

                do
                {
                    if (vertex.ignore || position.y > vertex.position.y + vertex.height || position.y + height < vertex.position.y || (vertex.layer & collidesWith) == 0)
                    {
                        vertex = vertex.next;
                        continue;
                    }

                    float cross = VO.Det(new Vector2(vertex.position.x, vertex.position.z), vertex.dir, position2D);// vertex.dir.x * ( vertex.position.z - position.z ) - vertex.dir.y * ( vertex.position.x - position.x );
                    // Signed distance from the line (not segment), lines are infinite
                    // Usually divided by vertex.dir.magnitude, but that is known to be 1
                    float signedDist = cross;

                    if (Mathf.Abs(signedDist) < neighbourDist)
                    {
#if BNICKSON_UPDATED
                        //from up move to there  less call  NearestPoint  ---lzt 
                        Vector3 lineToAgent = position - VectorMath.ClosestPointOnLine(obstacle.position, vertex.position, position);
                        lineToAgent.y = 0f;
                        if (Vector3.Dot(lineToAgent, obstacle.pushAwayDir) < 0f) // if the agent is behind the obstacle
                        {
                            continue; // we don't want this obstacle to stop the agent from getting back onto the nav mesh, alternately
                                      // we could consider the agent to be in front of the obstacle for the orca calculations, the agent will then be pushed away from the obstacle
                                      // which is probably desirable
                        }
#endif
                        //from up move to there  less call  ---lzt 
                        float dotFactor = Vector2.Dot(vertex.dir, position2D - new Vector2(vertex.position.x, vertex.position.z));

                        // It is closest to the segment
                        // if the dotFactor is <= 0 or >= length of the segment
                        // WallThickness*0.1 is added as a margin to avoid false positives when moving along the edges of square obstacles
                        bool closestIsEndpoints = dotFactor <= wallThickness * 0.05f || dotFactor >= (new Vector2(vertex.position.x, vertex.position.z) - new Vector2(vertex.next.position.x, vertex.next.position.z)).magnitude - wallThickness * 0.05f;

                        if (signedDist <= 0 && !closestIsEndpoints && signedDist > -wallThickness)
                        {
                            // Inside the wall on the "wrong" side
                            vos[voCount] = new VO(position2D, new Vector2(vertex.position.x, vertex.position.z) - position2D, vertex.dir, wallWeight * 2);
                            voCount++;
                        }
                        else if (signedDist > 0)
                        {
                            //Debug.DrawLine (position, (vertex.position+vertex.next.position)*0.5f, Color.yellow);
                            Vector2 p1 = new Vector2(vertex.position.x, vertex.position.z) - position2D;
                            Vector2 p2 = new Vector2(vertex.next.position.x, vertex.next.position.z) - position2D;
                            Vector2 tang1 = (p1).normalized;
                            Vector2 tang2 = (p2).normalized;
                            vos[voCount] = new VO(position2D, p1, p2, tang1, tang2, wallWeight);
                            voCount++;

                        }
                    }
                    vertex = vertex.next;
                } while (vertex != obstacle);
            }
            //#endif
            for (int o = 0; o < neighbours.Count; o++)
            {
                Agent other = neighbours[o];

                if (other == this) continue;

                float maxY = System.Math.Min (position.y+height,other.position.y+other.height);
                float minY = System.Math.Max (position.y,other.position.y);

                //The agents cannot collide since they
                //are on different y-levels
                if (maxY - minY < 0)
                {
                    continue;
                }

                Vector2 otherOptimalVelocity = new Vector2(other.Velocity.x, other.velocity.z);


                float totalRadius = radius + other.radius;

                // Describes a circle on the border of the VO
                //float boundingRadius = totalRadius * inverseAgentTimeHorizon;
                Vector2 voBoundingOrigin = new Vector2(other.position.x,other.position.z) - position2D;

                //float boundingDist = voBoundingOrigin.magnitude;

                Vector2 relativeVelocity = optimalVelocity - otherOptimalVelocity;

                {
                    //voBoundingOrigin *= inverseAgentTimeHorizon;
                    //boundingDist *= inverseAgentTimeHorizon;

                    // Common case, no collision

                    Vector2 voCenter;
                    if (other.locked)
                    {
                        voCenter = otherOptimalVelocity;
                    }
                    else
                    {
                        voCenter = (optimalVelocity + otherOptimalVelocity) * 0.5f;
                    }

                    vos[voCount] = new VO(voBoundingOrigin, voCenter, totalRadius, relativeVelocity, inverseAgentTimeHorizon, 1);
                    voCount++;
                    if (DebugDraw) DrawVO(position2D + voBoundingOrigin * inverseAgentTimeHorizon + voCenter, totalRadius * inverseAgentTimeHorizon, position2D + voCenter);
                }


            }


            Vector2 result = Vector2.zero;

            if (simulator.algorithm == Simulator.SamplingAlgorithm.GradientDecent)
            {
                if (DebugDraw)
                {
                    const int PlotWidth = 40;
                    const float WorldPlotWidth = 15;

                    for (int x = 0; x < PlotWidth; x++)
                    {
                        for (int y = 0; y < PlotWidth; y++)
                        {
                            Vector2 p = new Vector2 (x*WorldPlotWidth / PlotWidth, y*WorldPlotWidth / PlotWidth);

                            Vector2 dir = Vector2.zero;
                            float weight = 0;
                            for (int i = 0; i < voCount; i++)
                            {
                                float w;
                                dir += vos[i].Sample(p - position2D, out w);
                                if (w > weight) weight = w;
                            }
                            Vector2 d2 = (new Vector2(desiredVelocity.x,desiredVelocity.z) - (p-position2D));
                            dir += d2 * DesiredVelocityScale;

                            if (d2.magnitude * DesiredVelocityWeight > weight) weight = d2.magnitude * DesiredVelocityWeight;

                            if (weight > 0) dir /= weight;

                            //Vector2 d3 = simulator.SampleDensity (p+position2D);
                            Debug.DrawRay(To3D(p), To3D(d2 * 0.00f), Color.blue);
                            //simulator.Plot (p, Rainbow(weight*simulator.colorScale));

                            float sc = 0;
                            Vector2 p0 = p - Vector2.one*WorldPlotWidth*0.5f;
                            Vector2 p1 = Trace (vos, voCount, p0, 0.01f, out sc);
                            if ((p0 - p1).sqrMagnitude < Sqr(WorldPlotWidth / PlotWidth) * 2.6f)
                            {
                                Debug.DrawRay(To3D(p1 + position2D), Vector3.up * 1, Color.red);
                            }
                        }
                    }
                }

                //if ( debug ) {
                float best = float.PositiveInfinity;

                float cutoff = new Vector2(velocity.x,velocity.z).magnitude*simulator.qualityCutoff;

                //for ( int i = 0; i < 10; i++ ) {
                {
                    result = Trace(vos, voCount, new Vector2(desiredVelocity.x, desiredVelocity.z), cutoff, out best);
                    if (DebugDraw) DrawCross(result + position2D, Color.yellow, 0.5f);
                }

                // Can be uncommented for higher quality local avoidance
                /*for ( int i = 0; i < 3; i++ ) {
                    Vector2 p = desiredVelocity + new Vector2(Mathf.Cos(Mathf.PI*2*(i/3.0f)), Mathf.Sin(Mathf.PI*2*(i/3.0f)));
                    float score;
                    Vector2 res = Trace ( vos, voCount, p, velocity.magnitude*simulator.qualityCutoff, out score );
                    
                    if ( score < best ) {
                        //if ( score < best*0.9f )EB.Debug.Log ("Better " + score + " < " + best);
                        result = res;
                        best = score;
                    }
                }*/

                {
                    Vector2 p = Velocity;
                    float score;
                    Vector2 res = Trace ( vos, voCount, p, cutoff, out score );

                    if (score < best)
                    {
                        //if ( score < best*0.9f )EB.Debug.Log ("Better " + score + " < " + best);
                        result = res;
                        best = score;
                    }
                    if (DebugDraw) DrawCross(res + position2D, Color.magenta, 0.5f);
                }
            }
            else
            {
                // Adaptive sampling

                Vector2[] samplePos = context.samplePos;
                float[] sampleSize = context.sampleSize;
                int samplePosCount = 0;


                Vector2 desired2D = new Vector2(desiredVelocity.x,desiredVelocity.z);
                float sampleScale = Mathf.Max (radius, Mathf.Max (desired2D.magnitude, Velocity.magnitude));
                samplePos[samplePosCount] = desired2D;
                sampleSize[samplePosCount] = sampleScale * 0.3f;
                samplePosCount++;

                const float GridScale = 0.3f;

                // Initial 9 samples
                samplePos[samplePosCount] = optimalVelocity;
                sampleSize[samplePosCount] = sampleScale * GridScale;
                samplePosCount++;

                {
                    Vector2 fw = optimalVelocity * 0.5f;
                    Vector2 rw = new Vector2(fw.y, -fw.x);

                    const int Steps = 8;
                    for (int i = 0; i < Steps; i++)
                    {
                        samplePos[samplePosCount] = rw * Mathf.Sin(i * Mathf.PI * 2 / Steps) + fw * (1 + Mathf.Cos(i * Mathf.PI * 2 / Steps));
                        sampleSize[samplePosCount] = (1.0f - (Mathf.Abs(i - Steps * 0.5f) / Steps)) * sampleScale * 0.5f;
                        samplePosCount++;
                    }

                    const float InnerScale = 0.6f;
                    fw *= InnerScale;
                    rw *= InnerScale;

                    const int Steps2 = 6;
                    for (int i = 0; i < Steps2; i++)
                    {
                        samplePos[samplePosCount] = rw * Mathf.Cos((i + 0.5f) * Mathf.PI * 2 / Steps2) + fw * ((1.0f / InnerScale) + Mathf.Sin((i + 0.5f) * Mathf.PI * 2 / Steps2));
                        sampleSize[samplePosCount] = sampleScale * 0.3f;
                        samplePosCount++;
                    }

                    const float TargetScale = 0.2f;

                    const int Steps3 = 6;
                    for (int i = 0; i < Steps3; i++)
                    {
                        samplePos[samplePosCount] = optimalVelocity + new Vector2(sampleScale * TargetScale * Mathf.Cos((i + 0.5f) * Mathf.PI * 2 / Steps3), sampleScale * TargetScale * Mathf.Sin((i + 0.5f) * Mathf.PI * 2 / Steps3));
                        sampleSize[samplePosCount] = sampleScale * TargetScale * 2;
                        samplePosCount++;
                    }
                }

                samplePos[samplePosCount] = optimalVelocity * 0.5f;
                sampleSize[samplePosCount] = sampleScale * 0.4f;
                samplePosCount++;

                const int KeepCount = Simulator.WorkerContext.KeepCount;
                Vector2[] bestPos = context.bestPos;
                float[] bestSizes = context.bestSizes;
                float[] bestScores = context.bestScores;

                for (int i = 0; i < KeepCount; i++)
                {
                    bestScores[i] = float.PositiveInfinity;
                }
                bestScores[KeepCount] = float.NegativeInfinity;

                Vector2 bestEver = optimalVelocity;
                float bestEverScore = float.PositiveInfinity;

                for (int sub = 0; sub < 3; sub++)
                {

                    for (int i = 0; i < samplePosCount; i++)
                    {

                        float score = 0;
                        for (int vo = 0; vo < voCount; vo++)
                        {
                            score = System.Math.Max(score, vos[vo].ScalarSample(samplePos[i]));
                        }
                        // Note that velocity is a vector and speed is a scalar, not the same thing
                        float bonusForDesiredVelocity = (samplePos[i] - desired2D).magnitude;

                        // This didn't work out as well as I though
                        // Code left here because I might reenable it later
                        //float bonusForDesiredSpeed = Mathf.Abs (samplePos[i].magnitude - desired2D.magnitude);

                        float biasedScore = score + bonusForDesiredVelocity*DesiredVelocityWeight;// + bonusForDesiredSpeed*0;
                        score += bonusForDesiredVelocity * 0.001f;

                        if (DebugDraw)
                        {
                            DrawCross(position2D + samplePos[i], Rainbow(Mathf.Log(score + 1) * 5), sampleSize[i] * 0.5f);
                        }

                        if (biasedScore < bestScores[0])
                        {
                            for (int j = 0; j < KeepCount; j++)
                            {
                                if (biasedScore >= bestScores[j + 1])
                                {
                                    bestScores[j] = biasedScore;
                                    bestSizes[j] = sampleSize[i];
                                    bestPos[j] = samplePos[i];
                                    break;
                                }
                            }
                        }

                        if (score < bestEverScore)
                        {
                            bestEver = samplePos[i];
                            bestEverScore = score;

                            if (score == 0)
                            {
                                sub = 100;
                                break;
                            }
                        }
                    }

                    samplePosCount = 0;

                    for (int i = 0; i < KeepCount; i++)
                    {
                        Vector2 p = bestPos[i];
                        float s = bestSizes[i];
                        bestScores[i] = float.PositiveInfinity;

                        const float Half = 0.6f;

                        float offset = s * Half * 0.5f;

                        samplePos[samplePosCount + 0] = (p + new Vector2(+offset, +offset));
                        samplePos[samplePosCount + 1] = (p + new Vector2(-offset, +offset));
                        samplePos[samplePosCount + 2] = (p + new Vector2(-offset, -offset));
                        samplePos[samplePosCount + 3] = (p + new Vector2(+offset, -offset));

                        s *= s * Half;
                        sampleSize[samplePosCount + 0] = (s);
                        sampleSize[samplePosCount + 1] = (s);
                        sampleSize[samplePosCount + 2] = (s);
                        sampleSize[samplePosCount + 3] = (s);
                        samplePosCount += 4;
                    }
                }

                result = bestEver;
            }


            if (DebugDraw) DrawCross(result + position2D);


            newVelocity = To3D(Vector2.ClampMagnitude(result, maxSpeed));
        }

        public void CalculateNeighbours()
        {
            neighbours.Clear();
            neighbourDists.Clear();

            float rangeSq;

            if (locked) return;

            //watch1.Start ();
            if (MaxNeighbours > 0)
            {
                rangeSq = neighbourDist * neighbourDist;

                //simulator.KDTree.GetAgentNeighbours (this, rangeSq);
                simulator.Quadtree.Query(new Vector2(position.x, position.z), neighbourDist, this);

#if BNICKSON_UPDATED
                // removing agents from the list to collide with if they are not of the type which this agent collides with
#if DEBUG
                if (neighbours.Count != neighbourDists.Count)
                {
                    EB.Debug.LogError("The lengths of the neighbours and neighbourDists lists should be equal");
                }
#endif

                for (int otherAgent = 0; otherAgent < neighbours.Count;) // go over the list of all the returned agents
                {
                    if (0 == (agentsToCollideWith & neighbours[otherAgent].agentCollisionType)) // if they are not of the type which we collide with
                    {
                        neighbours.RemoveAt(otherAgent); // remove the agents from both lists
                        neighbourDists.RemoveAt(otherAgent);
                    }
                    else
                    {
                        ++otherAgent;
                    }
                }
#endif
            }
            //watch1.Stop ();

            obstacles.Clear();
            obstacleDists.Clear();

#if BNICKSON_UPDATED
            rangeSq = (obstacleTimeHorizon * maxSpeed + radius + radiusAgainstObstacles);
#else
            rangeSq = (obstacleTimeHorizon * maxSpeed + radius);
#endif
            rangeSq *= rangeSq;
            // Obstacles disabled at the moment
            //simulator.KDTree.GetObstacleNeighbours (this, rangeSq);

#if BNICKSON_UPDATED
            simulator.GetStaticAndDynamicObstacleNeighbours(this, rangeSq);
#endif
        }
    }
}

#endif
