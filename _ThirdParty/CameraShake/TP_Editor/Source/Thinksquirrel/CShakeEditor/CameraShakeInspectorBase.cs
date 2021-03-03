//
// CameraShakeInspectorBase.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2016 Thinksquirrel Software, LLC
//
using UnityEditor;
using UnityEngine;

//! \cond PRIVATE
namespace Thinksquirrel.CShakeEditor
{
    abstract class CameraShakeInspectorBase : Editor
    {
        public new bool DrawDefaultInspector()
        {
            return DrawDefaultInspector(true);
        }

        public bool DrawDefaultInspector(bool drawEvents)
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible (enterChildren))
            {
                if (iterator.name == "m_Script" || iterator.name == "m_Target" || iterator.name == "m_TargetGameObject" || iterator.name == "m_MethodName" || iterator.name == "m_SerializedInvocationList")
                    continue;

                /*
                if (iterator.type == "CameraShakeEvent")
                {
                    enterChildren = false;

                    if (!drawEvents)
                        continue;

                    CShakeEditorHelpers.DrawEventInternal(iterator);

                    continue;
                }
                */

                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }
            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        [MenuItem("CONTEXT/CameraShakeBase/Help")]
        static void OpenHelp(MenuCommand command)
        {
            Application.OpenURL(CameraShakePreferences.ComponentUrl(command.context.GetType()));
        }

        [MenuItem("CONTEXT/CameraShakeBase/API Reference")]
        static void OpenAPIReference(MenuCommand command)
        {
            Application.OpenURL(CameraShakePreferences.ScriptingUrl(command.context.GetType()));
        }
    }
}
//! \endcond

