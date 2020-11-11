using System;
using UnityEngine;

namespace Djinde.Utils
{
    public class Tools
    {
        #region Public Methods

        public static bool TryCast<T>(object obj, out T result)
        {
            result = default(T);
            try
            {
                result = (T)obj;
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public static void LogError(object obj, string desc)
        {
            Debug.LogError(obj.GetType().ToString() + " [" + ((MonoBehaviour)obj).name + "]" + ": " + desc);
        }

        public static void LogWarning(object obj, string desc)
        {
            Debug.LogWarning(obj.GetType().ToString() + ": " + desc);
        }

        public static void Log(object obj, string desc)
        {
            Debug.Log(obj.GetType().ToString() + ": " + desc);
        }

        #endregion
    }
}