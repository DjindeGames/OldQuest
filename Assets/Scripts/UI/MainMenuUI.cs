using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private string nextScene;
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private GameObject savesScreen;

    private void Start()
    {
        savesScreen.SetActive(false);
        fadeImage.CrossFadeAlpha(0, 5, false);
    }

    public void exit()
    {
        Application.Quit();
    }

    public void start()
    {
        Cursor.visible = false;
        SceneManager.LoadScene(nextScene);
    }

    public void load()
    {
        savesScreen.SetActive(true);
        /*
        Cursor.visible = false;
        SaveManager.Instance.load();
        */

    }

    public void closeSavesScreen()
    {
        savesScreen.SetActive(false);
    }
}
