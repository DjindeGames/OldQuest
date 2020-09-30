using System;

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


    #endregion
}
