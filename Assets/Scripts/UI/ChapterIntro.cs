using UnityEngine;
using TMPro;

public class ChapterIntro : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Fader backgroundFader;
    [SerializeField]
    private Fader textFader;
    [SerializeField]
    private TMP_Text title;

    private bool fadeCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        title.text = "Chapter " + ChapterData.Instance.chapterNumber + ": " + ChapterData.Instance.chapterName;
        backgroundFader.fadeHasCompleted += onFadeCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !fadeCompleted)
        {
            backgroundFader.skip();
            textFader.skip();
            Destroy(gameObject);
        }
    }

    void onFadeCompleted()
    {
        fadeCompleted = true;
    }

    void OnDestroy()
    {
        backgroundFader.fadeHasCompleted -= onFadeCompleted;
    }
}
