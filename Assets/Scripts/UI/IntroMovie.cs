using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroMovie : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private string nextScene;

    VideoPlayer vp;
    bool ready = false;

    // Start is called before the first frame update
    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        StartCoroutine(wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            if (!vp.isPlaying || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    private IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(1);
        ready = true;
    }
}
