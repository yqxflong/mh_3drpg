//
// CameraShakeMenuItems.cs
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
    static class CameraShakeMenuItems
    {
        // This variable controls the location of Camera Shake menu commands.
        // Do not include the "Camera Shake" folder in the name.
        // No trailing slashes allowed!
        public const string menuToolsLocation = "Custom/Thinksquirrel";

        [MenuItem(menuToolsLocation + "/Product Support", false, 10000)]
        internal static void ProductSupport()
        {
            Application.OpenURL("https://support.thinksquirrel.com");
        }

        [MenuItem(menuToolsLocation + "/Get Update Notifications", false, 10001)]
        internal static void GetUpdateNotifications()
        {
            Application.OpenURL("https://www.thinksquirrel.com/#subscribe");
        }

        [MenuItem(menuToolsLocation + "/Camera Shake/Reference Manual")]
        internal static void OpenReferenceManual()
        {
            Application.OpenURL(CameraShakePreferences.ReferenceManualUrl());
        }
    }
}
//! \endcond

