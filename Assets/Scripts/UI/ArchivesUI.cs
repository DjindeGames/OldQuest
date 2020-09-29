using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchivesUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject filePrefab;
    [SerializeField]
    private Transform filesContainer;
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text content;
    [SerializeField]
    private TMP_Text currentPage;
    [SerializeField]
    private TMP_Text totalPages;
    [SerializeField]
    private Button buttonPrevious;
    [SerializeField]
    private Button buttonNext;
    [Header("Parameters")]
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color selectedColor;

    private string cachedContent;

    private ArchiveFile currentlySelected;

    private int charsPerPage;
    private int currentPageIndex;
    private int currentlyDisplayedChars;
    private int numberOfPages;
    private List<int> startIndexByPage = new List<int>();

    public static ArchivesUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        buttonNext.interactable = false;
        buttonPrevious.interactable = false;
        title.text = "";
        content.text = "";
        currentPage.text = "";
        totalPages.text = "";
    }

    public void close()
    {
        SoundManager.Instance.playSFX(ESFXType.CloseReadable);
        ScreenManager.Instance.switchToPreviousScreen();
    }

    public void addFile(ReadableKey key, bool withSelection = true)
    {
        GameObject newFile = Instantiate(filePrefab, filesContainer);
        newFile.transform.SetAsFirstSibling();
        ArchiveFile file = newFile.GetComponent<ArchiveFile>();
        file.key = key;
        file.label.text = ReadableTextsDB.getTitleByKey(key);
        newFile.GetComponent<Button>().onClick.AddListener(delegate () { this.select(file); });
        if (withSelection)
        {
            this.select(file);
        }
    }

    public void select(ArchiveFile file)
    {
        if (currentlySelected)
        {
            if (currentlySelected == file)
            {
                return;
            }
            currentlySelected.GetComponent<Image>().color = normalColor;
        }
        
        currentlySelected = file;
        currentlySelected.GetComponent<Image>().color = selectedColor;

        ReadableKey key = file.key;
        currentPageIndex = 0;
        startIndexByPage.Clear();
        cachedContent = ReadableTextsDB.getTextByKey(key);

        title.text = ReadableTextsDB.getTitleByKey(key);
        content.text = cachedContent;
        startIndexByPage.Add(0);

        SoundManager.Instance.playSFX(ESFXType.OpenReadable);

        StartCoroutine(computeNumberOfPages());
    }

    private IEnumerator computeNumberOfPages()
    {
        yield return new WaitForEndOfFrame();
        charsPerPage = content.textInfo.characterCount;
        numberOfPages = Mathf.CeilToInt(cachedContent.Length / (float)charsPerPage);
        currentlyDisplayedChars = charsPerPage;
        refreshPage();
    }

    public void nextPage()
    {
        currentPageIndex++;
        if (currentPageIndex + 1 > startIndexByPage.Count)
        {
            startIndexByPage.Add(startIndexByPage[currentPageIndex - 1] + currentlyDisplayedChars);
        }
        SoundManager.Instance.playSFX(ESFXType.PageChanged);
        refreshPage();
    }

    public void previousPage()
    {
        currentPageIndex--;
        SoundManager.Instance.playSFX(ESFXType.PageChanged);
        refreshPage();
    }

    private void refreshPage()
    {
        currentPage.text = (currentPageIndex + 1).ToString();
        totalPages.text = numberOfPages.ToString();
        buttonPrevious.interactable = currentPageIndex > 0;
        buttonNext.interactable = currentPageIndex + 1 < numberOfPages;
        content.text = cachedContent.Substring(startIndexByPage[currentPageIndex]);
        StartCoroutine(adjustPage());
    }

    //If the final word of the page is truncated, pushes it to the next page
    private IEnumerator adjustPage()
    {
        yield return new WaitForEndOfFrame();
        bool hasNextPage = (startIndexByPage[currentPageIndex] + charsPerPage) < cachedContent.Length;
        if (hasNextPage)
        {
            int splitIndex = startIndexByPage[currentPageIndex] + charsPerPage;
            while (cachedContent[splitIndex] != ' ')
            {
                splitIndex--;
            }
            currentlyDisplayedChars = splitIndex - startIndexByPage[currentPageIndex];
            content.text = cachedContent.Substring(startIndexByPage[currentPageIndex], currentlyDisplayedChars);
        }
    }
}