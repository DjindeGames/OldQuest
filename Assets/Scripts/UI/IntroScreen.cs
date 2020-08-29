using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class IntroScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private AudioClip intro1;
    [SerializeField]
    private AudioClip intro2;
    [SerializeField]
    private Image djindeGames;
    [SerializeField]
    private Image adrianVonZiegler;
    [SerializeField]
    private Text djindeGamesText;
    [SerializeField]
    private Text adrianVonZieglerText;
    [SerializeField]
    private string nextScene;

    private AudioSource soundPlayer;

    void Awake()
    {
        soundPlayer = GetComponent<AudioSource>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        djindeGames.CrossFadeAlpha(0f, 0, false);
        djindeGamesText.CrossFadeAlpha(0f, 0, false);
        adrianVonZiegler.CrossFadeAlpha(0f, 0, false);
        adrianVonZieglerText.CrossFadeAlpha(0f, 0, false);
        StartCoroutine(fadeDjinde());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            djindeGames.CrossFadeAlpha(0, 1, false);
            djindeGamesText.CrossFadeAlpha(0, 1, false);
            adrianVonZiegler.CrossFadeAlpha(0, 1, false);
            adrianVonZieglerText.CrossFadeAlpha(0, 1, false);
            StartCoroutine(loadNextScene());
        }
    }

    private IEnumerator fadeDjinde()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        soundPlayer.clip = intro1;
        soundPlayer.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        djindeGames.CrossFadeAlpha(1, 2, false);
        djindeGamesText.CrossFadeAlpha(1, 2, false);
        yield return new WaitForSecondsRealtime(3);
        djindeGames.CrossFadeAlpha(0, 1, false);
        djindeGamesText.CrossFadeAlpha(0, 1, false);
        yield return new WaitForSecondsRealtime(2);
        StartCoroutine(fadeAdrian());
    }

    private IEnumerator fadeAdrian()
    {
        soundPlayer.clip = intro2;
        soundPlayer.Play();
        adrianVonZiegler.CrossFadeAlpha(1, 2, false);
        adrianVonZieglerText.CrossFadeAlpha(1, 2, false);
        yield return new WaitForSecondsRealtime(3);
        adrianVonZiegler.CrossFadeAlpha(0, 1, false);
        adrianVonZieglerText.CrossFadeAlpha(0, 1, false);
        StartCoroutine(loadNextScene());
    }

    private IEnumerator loadNextScene()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(nextScene);
    }
}
