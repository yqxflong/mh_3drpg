//
// CShakeEditor.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2016 Thinksquirrel Software, LLC
//
using Thinksquirrel.CShake;
using UnityEditor;

//!\cond PRIVATE
namespace Thinksquirrel.CShakeEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraShake))]
    class CameraShakeInspector : CameraShakeInspectorBase
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
//!\endcond
