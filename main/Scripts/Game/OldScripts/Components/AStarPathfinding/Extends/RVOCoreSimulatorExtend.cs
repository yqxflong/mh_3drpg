using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.RVO.Sampled;

#if APP_UPDATED

namespace Pathfinding.RVO
{
	public partial class Simulator
	{
		/* instruct the APP system to use the same time deltas as unity */
		protected bool _useUnityTimeDelta = false;

		/* Dynamic obstacles in this simulation */
		protected List<ObstacleVertex> _dynamicObstacles = new List<ObstacleVertex>();

		/* are we matching the unity time delta */
		public bool UseUnityTimeDelta
		{
			get { return _useUnityTimeDelta; }
			set { _useUnityTimeDelta = value; }
		}

		public void AddDynamicObstacle(ObstacleVertex obstacle)
		{
			_dynamicObstacles.Add(obstacle);
#if DEBUG
			const int MaxDynamicObstacles = 8; // this allows for two four sided obstacles, there really shouldn't be many moving obstacles
			if (_dynamicObstacles.Count > MaxDynamicObstacles)
			{
				EB.Debug.LogWarning("RVOCoreSimulator.AddDynamicObstacle() : There are a lot of moving obstacles in the scene - this has performance implications");
			}
#endif
		}

		public void RemoveDynamicObstacle(ObstacleVertex obstacle)
		{
			_dynamicObstacles.Remove(obstacle);
		}

		/**
		 * Adds a line obstacle with a specified height.
		 * 
		 * \see RemoveObstacle
		 */
		public ObstacleVertex AddObstacle(Vector3 a, Vector3 b, Vector3 pushAwayDir, float height, bool isActive)
		{
			ObstacleVertex first = CreateObstacle(a, b, pushAwayDir, height, isActive);

			//Don't interfere with ongoing calculations
			if (Multithreading && doubleBuffering) for (int j = 0; j < workers.Length; j++) workers[j].WaitOne();

			obstacles.Add(first);

			UpdateObstacles();
			return first;
		}

		/**
		 * Creates a line obstacle with a specified height.
		 * 
		 * \see RemoveObstacle
		 */
		public ObstacleVertex CreateObstacle(Vector3 a, Vector3 b, Vector3 pushAwayDir, float height, bool isActive)
		{
			ObstacleVertex first = new ObstacleVertex();
			ObstacleVertex second = new ObstacleVertex();

			first.pushAwayDir = pushAwayDir;
			second.pushAwayDir = pushAwayDir;

			first.isActive = second.isActive = isActive;

			first.prev = second;
			second.prev = first;
			first.next = second;
			second.next = first;

			first.position = a;
			second.position = b;
			first.height = height;
			second.height = height;

			first.convex = true;
			second.convex = true;

			first.dir = new Vector2(b.x - a.x, b.z - a.z).normalized;
			second.dir = -first.dir;

			return first;
		}

		// turn obstacles off/on		
		public void SetObstacleActive(ObstacleVertex obstacle, bool isActive)
		{
			ObstacleVertex c = obstacle;
			do
			{
				c.isActive = isActive;
				c = c.next;
			} while (c != obstacle);
		}

		// StaticAndDynamicObstacle query functions
		public void GetStaticAndDynamicObstacles(List<ObstacleVertex> obstacles, Vector3 position, float rangeSq)
		{
			for (int i = 0; i < this.obstacles.Count; ++i)
			{
				if (this.obstacles[i].isActive)
				{
					// this section is a copy of RVOKDTree.QueryObstacleTreeRecursive() ---------------------------------
					ObstacleVertex ob1 = this.obstacles[i];

					float agentLeftOfLine = VectorMath.SignedTriangleAreaTimes2XZ(ob1.position, ob1.next.position, position);

					Vector3 dir2D = ob1.position - ob1.next.position;
					dir2D.y = 0;

					float distSqLine = (agentLeftOfLine * agentLeftOfLine) / dir2D.sqrMagnitude; //Isn't this 4 times too large since TriangleArea is actually 2*triangle area
					if (distSqLine < rangeSq)
					{
						float distSqr = VectorMath.SqrDistancePointSegment(ob1.position, ob1.next.position, position);
						if (distSqr < rangeSq)
						{
							obstacles.Add(this.obstacles[i]);
						}
					}
				}
			}

			for (int dynamic = 0; dynamic < _dynamicObstacles.Count; ++dynamic)
			{
				if (_dynamicObstacles[dynamic].isActive)
				{
					// this section is a copy of RVOKDTree.QueryObstacleTreeRecursive() ---------------------------------
					ObstacleVertex ob1 = _dynamicObstacles[dynamic];

					float agentLeftOfLine = VectorMath.SignedTriangleAreaTimes2XZ(ob1.position, ob1.next.position, position);

					Vector3 dir2D = ob1.position - ob1.next.position;
					dir2D.y = 0;

					float distSqLine = (agentLeftOfLine * agentLeftOfLine) / dir2D.sqrMagnitude; //Isn't this 4 times too large since TriangleArea is actually 2*triangle area
					if (distSqLine < rangeSq)
					{
						float distSqr = VectorMath.SqrDistancePointSegment(ob1.position, ob1.next.position, position);
						if (distSqr < rangeSq)
						{
							obstacles.Add(_dynamicObstacles[dynamic]);
						}
					}
				}
			}
		}

		public void GetStaticAndDynamicObstacleNeighbours(Agent agent, float rangeSq)
		{
			for (int i = 0; i < obstacles.Count; ++i)
			{
				if (this.obstacles[i].isActive)
				{
					// this section is a copy of RVOKDTree.QueryObstacleTreeRecursive() ---------------------------------
					ObstacleVertex ob1 = obstacles[i];

					float agentLeftOfLine = VectorMath.SignedTriangleAreaTimes2XZ(ob1.position, ob1.next.position, agent.position);

					Vector3 dir2D = ob1.position - ob1.next.position;
					dir2D.y = 0;

					float distSqLine = (agentLeftOfLine * agentLeftOfLine) / dir2D.sqrMagnitude; //Isn't this 4 times too large since TriangleArea is actually 2*triangle area
					if (distSqLine < rangeSq)
					{
						float distSqr = VectorMath.SqrDistancePointSegment(ob1.position, ob1.next.position, agent.position);
						if (distSqr < rangeSq)
						{
							agent.InsertObstacleNeighbour(obstacles[i], rangeSq);
						}
					}
				}
			}

			for (int dynamic = 0; dynamic < _dynamicObstacles.Count; ++dynamic)
			{
				if (_dynamicObstacles[dynamic].isActive)
				{
					// this section is a copy of RVOKDTree.QueryObstacleTreeRecursive() ---------------------------------
					ObstacleVertex ob1 = _dynamicObstacles[dynamic];

					float agentLeftOfLine = VectorMath.SignedTriangleAreaTimes2XZ(ob1.position, ob1.next.position, agent.position);

					Vector3 dir2D = ob1.position - ob1.next.position;
					dir2D.y = 0;

					float distSqLine = (agentLeftOfLine * agentLeftOfLine) / dir2D.sqrMagnitude; // Isn't this 4 times too large since TriangleArea is actually 2*triangle area
					if (distSqLine < rangeSq)
					{
						agent.InsertObstacleNeighbour(_dynamicObstacles[dynamic], rangeSq);
					}
				}
			}
		}

		#region override functions

		/** Should be called once per frame */
		public void Update()
		{
			//Initialize last step
			if (lastStep < 0)
			{
				lastStep = Time.time;
				deltaTime = DesiredDeltaTime;
				prevDeltaTime = deltaTime;
				lastStepInterpolationReference = lastStep;
			}

#if BNICKSON_UPDATED
			if (_useUnityTimeDelta || Time.time - lastStep > DesiredDeltaTime)
#else
			if (Time.time - lastStep >= DesiredDeltaTime)
#endif
			{
				for (int i = 0; i < agents.Count; i++)
				{
					agents[i].Interpolate((Time.time - lastStepInterpolationReference) / DeltaTime);
				}

				lastStepInterpolationReference = Time.time;

				prevDeltaTime = DeltaTime;
				deltaTime = Time.time - lastStep;
				lastStep = Time.time;

				// Implements averaging of delta times
				// Disabled for now because it seems to have caused more issues than it solved
				// Might re-enable later
				/*frameTimeBufferIndex++;
				 * frameTimeBufferIndex %= frameTimeBuffer.Length;
				 * frameTimeBuffer[frameTimeBufferIndex] = deltaTime;
				 *
				 * float sum = 0;
				 * float mn = float.PositiveInfinity;
				 * float mx = float.NegativeInfinity;
				 * for (int i=0;i<frameTimeBuffer.Length;i++) {
				 *  sum += frameTimeBuffer[i];
				 *  mn = Mathf.Min (mn, frameTimeBuffer[i]);
				 *  mx = Mathf.Max (mx, frameTimeBuffer[i]);
				 * }
				 * sum -= mn;
				 * sum -= mx;
				 * sum /= (frameTimeBuffer.Length-2);
				 * sum = frame
				 * deltaTime = sum;*/

				//Calculate smooth delta time
				//Disabled because it seemed to cause more problems than it solved
				//deltaTime = (Time.time - frameTimeBuffer[(frameTimeBufferIndex-1+frameTimeBuffer.Length)%frameTimeBuffer.Length]) / frameTimeBuffer.Length;

				//Prevent a zero delta time
				deltaTime = System.Math.Max(deltaTime, 1.0f / 2000f);

#if BNICKSON_UPDATED
				if (_useUnityTimeDelta)
				{
					deltaTime = Time.deltaTime;
					interpolation = false;
				}
#endif

				// Time reference for the interpolation
				// If delta time would not be subtracted, the character would have a zero velocity
				// during all frames when the velocity was recalculated

				if (Multithreading)
				{
					// Make sure the threads have completed their tasks
					// Otherwise block until they have
					if (doubleBuffering)
					{
						for (int i = 0; i < workers.Length; i++) workers[i].WaitOne();
						if (!Interpolation) for (int i = 0; i < agents.Count; i++) agents[i].Interpolate(1.0f);
					}

					if (doCleanObstacles)
					{
						CleanObstacles();
						doCleanObstacles = false;
						doUpdateObstacles = true;
					}

					if (doUpdateObstacles)
					{
						doUpdateObstacles = false;
					}


					BuildQuadtree();

					for (int i = 0; i < workers.Length; i++)
					{
						workers[i].start = i * agents.Count / workers.Length;
						workers[i].end = (i + 1) * agents.Count / workers.Length;
					}

					//Update
					//BufferSwitch
					for (int i = 0; i < workers.Length; i++) workers[i].Execute(1);
					for (int i = 0; i < workers.Length; i++) workers[i].WaitOne();

					//Calculate New Velocity
					for (int i = 0; i < workers.Length; i++) workers[i].Execute(0);

					// Make sure the threads have completed their tasks
					// Otherwise block until they have
					if (!doubleBuffering)
					{
						for (int i = 0; i < workers.Length; i++) workers[i].WaitOne();
						if (!Interpolation) for (int i = 0; i < agents.Count; i++) agents[i].Interpolate(1.0f);
					}
				}
				else {
					if (doCleanObstacles)
					{
						CleanObstacles();
						doCleanObstacles = false;
						doUpdateObstacles = true;
					}

					if (doUpdateObstacles)
					{
						doUpdateObstacles = false;
					}

					BuildQuadtree();

					for (int i = 0; i < agents.Count; i++)
					{
						agents[i].Update();
						agents[i].BufferSwitch();
					}


					for (int i = 0; i < agents.Count; i++)
					{
						agents[i].CalculateNeighbours();
						agents[i].CalculateVelocity(coroutineWorkerContext);
					}

					if (oversampling)
					{
						for (int i = 0; i < agents.Count; i++)
						{
							agents[i].Velocity = agents[i].newVelocity;
						}

						for (int i = 0; i < agents.Count; i++)
						{
							Vector3 vel = agents[i].newVelocity;
							agents[i].CalculateVelocity(coroutineWorkerContext);
							agents[i].newVelocity = (vel + agents[i].newVelocity) * 0.5f;
						}
					}

					if (!Interpolation) for (int i = 0; i < agents.Count; i++) agents[i].Interpolate(1.0f);
				}
			}

			if (Interpolation)
			{
				for (int i = 0; i < agents.Count; i++)
				{
					agents[i].Interpolate((Time.time - lastStepInterpolationReference) / DeltaTime);
				}
			}
		}

#endregion
	}
}

#endif