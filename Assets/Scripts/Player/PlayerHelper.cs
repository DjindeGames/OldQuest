public class PlayerHelper
{
    private static bool isGrabbing = false;

    public static bool IsGrabbing
    {
        get
        {
            return isGrabbing;
        }
        set
        {
            isGrabbing = value;
        }
    }
}
