using System;
using UnityEngine;

public class Utils
{
    #region Public Methods

    public static bool IsA(object obj, Type type)
    {
        return obj.GetType() == type;
    }

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

    public static bool TryGetLootablePrefabFromItem(Item item, out GameObject prefab)
    {
        prefab = Resources.Load<GameObject>(Constants.PrefabsPath + item.Type.ToString() + "/" + item.name + "$");
        return (prefab != null);
    }
    #endregion
}
