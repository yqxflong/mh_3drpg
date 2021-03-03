//
// CameraShakeDocumentationName.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2016 Thinksquirrel Software, LLC
//
namespace Thinksquirrel.CShake.Internal
{
    //! \cond PRIVATE
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public sealed class CameraShakeDocumentationName : System.Attribute
    {
        string m_Name;

        public string name { get { return m_Name; } }

        public CameraShakeDocumentationName(string name)
        {
            m_Name = name;
        }
    }
    //! \cond PRIVATE
}
