using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NaughtyAttributes;

public class MainUI : MonoBehaviour
{
    [BoxGroup("References")]
    [Header("Texts")]
    [SerializeField]
    private TMP_Text descriptionText;
    [BoxGroup("References")]
    [SerializeField]
    private TMP_Text logText;

    [Header("Parameters")]
    [BoxGroup("References")]
    [SerializeField]
    [Range(1, 10)]
    private int logDuration;

    public static MainUI Instance { get; private set; }

    private Coroutine lastLog;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        descriptionText.text = "";
        logText.text = "";
    }

    public void displayMenu()
    {
        ScreenManager.Instance.switchScreen(EScreenType.Menu);
    }

    public void displayInventory()
    {
        PlayerController.Instance.cancelMove();
        ScreenManager.Instance.switchScreen(EScreenType.Inventory);
    }

    public void displayDescription(string description)
    {
        descriptionText.text = description;
    }

    public void hideDescription()
    {
        descriptionText.text = "";
    }

    public void writeLog(string log)
    {
        if (lastLog != null)
        {
            StopCoroutine(lastLog);
        }
        lastLog = StartCoroutine(startLog(log));
    }

    private IEnumerator startLog(string log)
    {
        logText.text = log;
        yield return new WaitForSecondsRealtime(logDuration);
        logText.text = "";
    }
}
