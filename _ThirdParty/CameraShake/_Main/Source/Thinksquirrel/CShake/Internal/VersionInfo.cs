//
// VersionInfo.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2016 Thinksquirrel Software, LLC
//
using System.Text;

namespace Thinksquirrel.CShake.Internal
{
    // !\cond PRIVATE
    public static class VersionInfo
    {
        const string _version = "1.6.0f1";                

        /// <summary>
        /// Gets the version of Camera Shake.
        /// </summary>
        public static string version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Gets the current Camera Shake license, in human-readable form.
        /// </summary>
        public static string license
        {
            get
            {
                return "Camera Shake";
            }
        }

        /// <summary>
        /// Gets the copyright message.
        /// </summary>
        public static string copyright
        {
            get
            {
                return "(c) 2012-2016 Thinksquirrel Software, LLC";
            }
        }
    }
    // !\endcond
}
