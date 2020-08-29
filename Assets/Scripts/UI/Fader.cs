using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum FadeMode { In, Out, InOut}

public class Fader : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private FadeMode mode = FadeMode.Out;
    [SerializeField]
    private float delayCursorRelease = 0f;
    [SerializeField]
    private float delayIn = 0f;
    [SerializeField]
    private float delayOut = 0f;
    [SerializeField]
    private float fadeDurationIn = 1f;
    [SerializeField]
    private float fadeDurationOut = 1f;  

    private TMP_Text text;
    private Image image;

    public delegate void fadeComplete();
    public event fadeComplete fadeHasCompleted;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponent<TMP_Text>();
        if (image)
        {
            image.enabled = true;
        }
        if (text)
        {
            text.enabled = true;
        }
        if (mode == FadeMode.Out)
        {
            if (text)
            {
                //text.CrossFadeAlpha(1f, 0, false);
            }
            if (image)
            {
                //image.CrossFadeAlpha(1f, 0, false);
            }
        } else if (mode == FadeMode.In)
        {
            if (text)
            {
                text.CrossFadeAlpha(0f, 0, false);
            }
            if (image)
            {
                image.CrossFadeAlpha(0f, 0, false);
            }
        }
        else
        {
            if (text)
            {
                text.CrossFadeAlpha(0f, 0, false);
            }
            if (image)
            {
                image.CrossFadeAlpha(0f, 0, false);
            }
        }
        StartCoroutine(fade());
    }

    private IEnumerator fade()
    {
        StartCoroutine(waitForEndOfFade());
        StartCoroutine(releaseCursor());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (mode == FadeMode.Out)
        {
            yield return new WaitForSecondsRealtime(delayOut);
            if (text)
            {
                text.CrossFadeAlpha(0f, fadeDurationOut, false);
            }
            if (image)
            {
                image.CrossFadeAlpha(0f, fadeDurationOut, false);
            }
        }
        else if (mode == FadeMode.In)
        {
            yield return new WaitForSecondsRealtime(delayIn);
            if (text)
            {
                text.CrossFadeAlpha(1f, fadeDurationIn, false);
            }
            if (image)
            {
                image.CrossFadeAlpha(1f, fadeDurationIn, false);
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(delayIn);
            if (text)
            {
                text.CrossFadeAlpha(1f, fadeDurationIn, false);
            }
            if (image)
            {
                image.CrossFadeAlpha(1f, fadeDurationIn, false);
            }
            yield return new WaitForSecondsRealtime(delayOut);
            if (text)
            {
                text.CrossFadeAlpha(0f, fadeDurationOut, false);
            }
            if (image)
            {
                image.CrossFadeAlpha(0f, fadeDurationOut, false);
            }
        }
    }

    private IEnumerator waitForEndOfFade()
    {
        float countDown = 0;
        switch (mode)
        {
            case FadeMode.In:
                countDown = delayIn + fadeDurationIn;
                break;
            case FadeMode.Out:
                countDown = delayOut + fadeDurationOut;
                break;
            case FadeMode.InOut:
                countDown = delayIn + fadeDurationIn + delayOut + fadeDurationOut;
                break;
        }
        yield return new WaitForSecondsRealtime(countDown);
        if (fadeHasCompleted != null)
        {
            fadeHasCompleted();
        }
        //Destroy(gameObject);
    }

    private IEnumerator releaseCursor()
    {
        yield return new WaitForSecondsRealtime(delayCursorRelease);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void skip()
    {
        StopAllCoroutines();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (mode == FadeMode.Out || mode == FadeMode.InOut)
        {
            if (text)
            {
                text.CrossFadeAlpha(0f, 0.01f, true);
            }
            if (image)
            {
                image.CrossFadeAlpha(0f, 0.01f, true);
            }
        }
        else 
        {
            if (text)
            {
                text.CrossFadeAlpha(1f, 0f, true);
            }
            if (image)
            {
                image.CrossFadeAlpha(1f, 0f, true);
            }
        }
        if (fadeHasCompleted != null)
        {
            fadeHasCompleted();
        }
        //Destroy(gameObject);
    }
}
