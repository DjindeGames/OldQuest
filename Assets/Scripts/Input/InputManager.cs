using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    //THIS IS BAD!!!
    private Fader introFader;

    private bool introCompleted = false;

    private void Start()
    {
        if (introFader)
        {
            introFader.fadeHasCompleted += onIntroCompleted;
        }
        else
        {
            introCompleted = true;
        }
    }

    void Update()
    {
        ScreenType activeScreen = ScreenManager.Instance.ActiveScreen;
        switch(activeScreen)
        {
            case (ScreenType.Main):
                checkInputMain();
                break;
            case (ScreenType.Menu):
                checkInputMainMenu();
                break;
            case (ScreenType.Inventory):
                checkInputInventory();
                break;
            case (ScreenType.Archives):
                checkInputArchives();
                break;
            case (ScreenType.DiceBoard):
                checkInputDiceBoard();
                break;
            case (ScreenType.Puppet):
                checkInputPuppet();
                break;
        }
    }

    void checkInputMain()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && introCompleted && !PlayerHelper.IsGrabbing)
        {
            ScreenManager.Instance.switchScreen(ScreenType.Inventory);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && introCompleted && !PlayerHelper.IsGrabbing)
        {
             ScreenManager.Instance.switchScreen(ScreenType.Menu);
        }
    }

    void checkInputMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuUI.Instance.closeMenu();
        }
    }

    void checkInputArchives()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ScreenManager.Instance.switchToPreviousScreen();
        }
    }

    void checkInputInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            ScreenManager.Instance.switchScreen(ScreenType.Main);
        }
    }

    void checkInputPuppet()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            ScreenManager.Instance.switchScreen(ScreenType.Inventory);
        }
    }

    void checkInputDiceBoard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DiceBoardManager.Instance.throwAll();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && DiceBoardUI.Instance.IsWaitingForAcknowledgement)
        {
            DiceBoardUI.Instance.acknowledge();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DiceBoardUI.Instance.closeTabs();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DiceBoardUI.Instance.displayConsumables();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DiceBoardUI.Instance.displaySpells();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DiceBoardUI.Instance.displayStats();
        }
    }

    void onIntroCompleted()
    {
        StartCoroutine(setIntroCompleted());
    }

    private IEnumerator setIntroCompleted()
    {
        yield return new WaitForEndOfFrame();
        introCompleted = true;
    }

    private void OnDestroy()
    {
        if (introFader)
        {
            introFader.fadeHasCompleted -= onIntroCompleted;
        }
    }
}
