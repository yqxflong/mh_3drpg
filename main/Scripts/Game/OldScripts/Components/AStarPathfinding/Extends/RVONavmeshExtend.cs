using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

#if APP_UPDATED

namespace Pathfinding.RVO
{
    public partial class RVONavmesh
    {
        #region override functions

        /** Removes obstacles which were added with AddGraphObstacles */
        public void RemoveObstacles()
        {
#if BNICKSON_UPDATED
            if (lastSim != null)
            {
                // if our usage of APP changes, this will throw an error
                throw new System.Exception("This section of A Star Pathfinding code should never be executed");
            }
#else
            if (lastSim == null) return;

            Pathfinding.RVO.Simulator sim = lastSim;
            lastSim = null;

            for (int i = 0; i < obstacles.Count; i++) sim.RemoveObstacle(obstacles[i]);

            obstacles.Clear();
#endif
        }

        #endregion
    }
}

#endif