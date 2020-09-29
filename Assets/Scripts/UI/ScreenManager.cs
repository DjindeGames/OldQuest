using UnityEngine;


public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    public EScreenType ActiveScreen { get; private set; } = EScreenType.Main;

    public delegate void screenChange(EScreenType current);
    public event screenChange screenHasChanged;

    private EScreenType previousScreen;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ActiveScreen = EScreenType.Main;
        previousScreen = ActiveScreen;
        //switchScreen(ScreenType.Main);
    }
    
    public void switchScreen (EScreenType which)
    {
        if (which == ActiveScreen)
        {
            return;
        }
        previousScreen = ActiveScreen;
        ActiveScreen = which;
        //Debug.Log("Switched Screen to " + ActiveScreen.ToString());
        switch (which)
        {
            case EScreenType.Main:
                UIManager.Instance.switchUI(EUIType.Main);
                CameraManager.Instance.switchCamera(ECameraType.Player);
                TimeManager.Instance.resumeTime();
                TimeManager.Instance.resumeEllapsedTime();
                break;
            case EScreenType.Menu:
                UIManager.Instance.switchUI(EUIType.Menu);
                CameraManager.Instance.toggleFPCursor(false);
                TimeManager.Instance.pauseTime();
                TimeManager.Instance.pauseEllapsedTime();
                break;
            case EScreenType.Inventory:
                UIManager.Instance.switchUI(EUIType.Inventory);
                CameraManager.Instance.switchCamera(ECameraType.Inventory);
                CameraManager.Instance.toggleFPCursor(false);
                TimeManager.Instance.resumeTime();
                break;
            case EScreenType.Archives:
                UIManager.Instance.switchUI(EUIType.Archives);
                CameraManager.Instance.toggleFPCursor(false);
                TimeManager.Instance.pauseTime();
                break;
            case EScreenType.Puppet:
                UIManager.Instance.switchUI(EUIType.Puppet);
                CameraManager.Instance.switchCamera(ECameraType.Puppet);
                break;
            case EScreenType.DiceBoard:
                UIManager.Instance.switchUI(EUIType.DiceBoard);
                CameraManager.Instance.switchCamera(ECameraType.DiceBoard);
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
