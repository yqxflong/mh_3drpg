//
// CameraShakeBase.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

//! This is the root namespace for all Thinksquirrel products
namespace Thinksquirrel {}

//! The main Camera Shake namespace.
namespace Thinksquirrel.CShake
{
    /// <summary>
    /// The base class for all Camera Shake components.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CameraShakeBase : MonoBehaviour
    {
        /// <summary>
        /// Logs a prefixed message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        public static void Log(object message, string prefix, string type)
        {
           EB.Debug.Log(string.Format("[{0}] ({1}): {2}", prefix, type, message));
        }
        /// <summary>
        /// Logs a prefixed warning.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        public static void LogWarning(object message, string prefix, string type)
        {
            Debug.LogWarning(string.Format("[{0}] ({1}): {2}", prefix, type, message));
        }
        /// <summary>
        /// Logs a prefixed erorr.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        public static void LogError(object message, string prefix, string type)
        {
            Debug.LogError(string.Format("[{0}] ({1}): {2}", prefix, type, message));
        }
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void LogException(System.Exception ex)
        {
            Debug.LogException(ex);
        }
        /// <summary>
        /// Logs a prefixed message, with context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log(object message, string prefix, string type, Object context)
        {
           EB.Debug.Log(string.Format("[{0}] ({1}): {2}", prefix, type, message), context);
        }
        /// <summary>
        /// Logs a prefixed warning, with context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning(object message, string prefix, string type, Object context)
        {
            Debug.LogWarning(string.Format("[{0}] ({1}): {2}", prefix, type, message), context);
        }
        /// <summary>
        /// Logs a prefixed exception, with context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError(object message, string prefix, string type, Object context)
        {
            Debug.LogError(string.Format("[{0}] ({1}): {2}", prefix, type, message), context);
        }
        /// <summary>
        /// Logs an exception, with context.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogException(System.Exception ex, Object context)
        {
            Debug.LogException(ex, context);
        }
    }
}