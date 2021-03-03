using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIServerRequest), true)]

public class UIServerRequestEditor : Editor
{

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        UIServerRequest request = target as UIServerRequest;
        NGUIEditorTools.DrawEvents("On Send", request, request.onSendRequest, false);
        NGUIEditorTools.DrawEvents("On Response", request, request.onResponse, false);
    }

}
