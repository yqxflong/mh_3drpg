using UnityEngine;
using System.Collections;

#if APP_UPDATED

namespace Pathfinding.RVO
{
    public partial class ObstacleVertex
    {
        // points into the nav mesh (not normalized)
        public Vector3 pushAwayDir;
        // allows nav mesh cuts to add/remove obstacles without re-evaluating the bsp tree
        public bool isActive = true; 
    }
}

#endif