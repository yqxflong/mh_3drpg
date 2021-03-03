using UnityEngine;
using UnityEditor;
using System.Collections;
using Pathfinding.RVO;
[CustomEditor(typeof(ObstacleHelper))]
public class ObstacleHelperInspector : Editor {

    private string m_group="";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(8);
        if (GUILayout.Button("Add Obstacle"))
        {
            AddObstacle();
        }
        m_group = GUILayout.TextField(m_group, 30, GUILayout.Width(500));

        if (GUILayout.Button("Show Obstacle"))
        {
            ShowObstacle();
        }

        if (GUILayout.Button("Disappear Obstacle"))
        {
            DispearObstacle();
        }
    }

    void AddObstacle()
    {
        ObstacleHelper group = target as ObstacleHelper;
        if (group == null)
        {
            return;
        }
        int count = group.transform.childCount;
        GameObject go = new GameObject(string.Format("{0}", count + 1));
        //go.AddComponent<BoxCollider>();
        //Rigidbody rigidbody=go.AddComponent<Rigidbody>();
        //rigidbody.useGravity = false;
        //rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        //go.layer = LayerMask.NameToLayer("Interactive");
        RVOSquareObstacle rvo =go.AddComponent<RVOSquareObstacle>();
        rvo.obstacleMode = RVOSquareObstacle.ObstacleVertexWinding.KeepOut;
        rvo.center = Vector2.zero;
        GameObject herostart =GameObject.Find("HeroStart");
        go.transform.parent = group.transform;
        go.transform.localPosition = herostart.transform.localPosition;
        go.AddComponent<SceneObstacleFxLength>();

        GameObject obstacle_anchor = new GameObject("anchor");
        obstacle_anchor.transform.parent = go.transform;
        obstacle_anchor.transform.localPosition = Vector3.zero;

    }

    void ShowObstacle()
    {
        ObstacleHelper group = target as ObstacleHelper;

        if (group == null)
        {
            return;
        }
        group.ShowObstacle(m_group);
    }

    void DispearObstacle()
    {
        ObstacleHelper group = target as ObstacleHelper;

        if (group == null)
        {
            return;
        }
        group.DispearObstacle();
    }
}
