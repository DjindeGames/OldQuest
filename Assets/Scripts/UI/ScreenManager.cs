using UnityEngine;


public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    public ScreenType ActiveScreen { get; private set; } = ScreenType.Main;

    public delegate void screenChange(ScreenType current);
    public event screenChange screenHasChanged;

    private ScreenType previousScreen;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ActiveScreen = ScreenType.Main;
        previousScreen = ActiveScreen;
        //switchScreen(ScreenType.Main);
    }
    
    public void switchScreen (ScreenType which)
    {
        previousScreen = ActiveScreen;
        ActiveScreen = which;
        //Debug.Log("Switched Screen to " + ActiveScreen.ToString());
        switch (which)
        {
            case ScreenType.Main:
                UIManager.Instance.switchUI(UIType.Main);
                CameraManager.Instance.switchCamera(CameraType.Player);
                TimeManager.Instance.resumeTime();
                TimeManager.Instance.resumeEllapsedTime();
                break;
            case ScreenType.Menu:
                UIManager.Instance.switchUI(UIType.Menu);
                CameraManager.Instance.toggleFPCursor(false);
                TimeManager.Instance.pauseTime();
                TimeManager.Instance.pauseEllapsedTime();
                break;
            case ScreenType.Inventory:
                UIManager.Instance.switchUI(UIType.Inventory);
                CameraManager.Instance.switchCamera(CameraType.Inventory);
                CameraManager.Instance.toggleFPCursor(false);
                TimeManager.Instance.resumeTime();
                break;
            case ScreenType.Archives:
                UIManager.Instance.switchUI(UIType.Archives);
                CameraManager.Instance.toggleFPCursor(false);
                TimeManager.Instance.pauseTime();
                break;
            case ScreenType.Puppet:
                UIManager.Instance.switchUI(UIType.Puppet);
                CameraManager.Instance.switchCamera(CameraType.Puppet);
                break;
            case ScreenType.DiceBoard:
                UIManager.Instance.switchUI(UIType.DiceBoard);
                CameraManager.Instance.switchCamera(CameraType.DiceBoard);
                break;
        }
        if (screenHasChanged != null)
        {
            screenHasChanged(ActiveScreen);
        }
    }

    public void switchToPreviousScreen()
    {
        switchScreen(previousScreen);
    }
}
