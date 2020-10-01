using System;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenManager : MonoBehaviour
{
    #region Events



    #endregion

    #region Exposed Attributes

    [Header("References")]
    [SerializeField]
    private Image _screenshot;
    [SerializeField]
    private TMP_Text _chapter;
    [SerializeField]
    private Button _loadButton;
    [SerializeField]
    private Button _deleteButton;
    [SerializeField]
    private GameObject _confirmDeletePopup;
    [SerializeField]
    private SelectableList _saveList;

    #endregion

    #region Attributes

    public static LoadScreenManager Instance { get; private set; }

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        Instance = this;
        _confirmDeletePopup.SetActive(false);
        _saveList.OnItemSelectedEvent += OnSaveFileSelected;
    }

    private void Start()
    {
        RefreshFilesList();
    }

    private void OnDestroy()
    {
        _saveList.OnItemSelectedEvent -= OnSaveFileSelected;
    }

    #endregion

    #region Private Methods

    private void OnSaveFileSelected(SelectableListItem listItem)
    {
        SelectableListItem_SaveData data = (SelectableListItem_SaveData)listItem._Data;

        // Enabling right part
        _screenshot.color = Color.white;
        _loadButton.interactable = true;
        _deleteButton.interactable = true;
        _screenshot.sprite = data._screenshot;
        _chapter.text = data._details;
    }

    private void ResetSelection()
    {
        _saveList.ClearList();
        _screenshot.color = Color.black;
        _loadButton.interactable = false;
        _deleteButton.interactable = false;
        _chapter.text = "No available save file!";
    }

    private void RemoveFileFromDisk(SelectableListItem item)
    {
        SelectableListItem_SaveData data = (SelectableListItem_SaveData)item._Data;
        System.IO.File.Delete(data._filePath);
        _saveList.RemoveItem(item);
    }

    private void ClearTempFolder()
    {
        try
        {
            Directory.Delete(Constants.TemporaryExtractedSavesPath, true);
        }
        catch (DirectoryNotFoundException) { }
    }

    private string formatPlayedTime(int timeInSeconds)
    {
        int hours = Mathf.FloorToInt(((float)timeInSeconds) / 3600);
        int minutes = Mathf.FloorToInt(((float)(timeInSeconds % 3600)) / 60);
        int seconds = timeInSeconds % 60;
        string hoursString = (hours >= 10) ? hours.ToString() : "0" + hours;
        string minutesString = (minutes >= 10) ? minutes.ToString() : "0" + minutes;
        string secondsString = (seconds >= 10) ? seconds.ToString() : "0" + seconds;
        return (hoursString + ":" + minutesString + ":" + secondsString);
    }

    #endregion

    #region Public Methods

    public void RefreshFilesList()
    {
        ResetSelection();
        try
        {
            string[] saveFilesNames = Directory.GetFiles(Constants.SaveFilesPath, "*" + Constants.ZippedSavesExtension);
            Array.Sort(saveFilesNames);

            ClearTempFolder();

            for (int i = saveFilesNames.Length - 1; i >= 0; i--)
            {
                ZipFile.ExtractToDirectory(saveFilesNames[i], Constants.TemporaryExtractedSavesPath);
                JSONObject saveFile = new JSONObject(System.IO.File.ReadAllText(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.SaveFilesExtension)[0]));

                // Generating screenshot
                byte[] byteArray = System.IO.File.ReadAllBytes(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.ScreenshotsExtension)[0]);
                Texture2D screenshot = new Texture2D(2, 2);
                screenshot.LoadImage(byteArray);

                // Generating list item
                SelectableListItem_SaveData data = new SelectableListItem_SaveData();
                data._label = saveFile.GetField(Constants.SFSerializedNameField).str + "\n" + saveFile.GetField(Constants.SFSerializedDateField).str;
                data._filePath = saveFilesNames[i];
                data._details = saveFile.GetField(Constants.SFSerializedChapterField).str + " (Played Time: " + formatPlayedTime(Int32.Parse(saveFile.GetField(Constants.SFSerializedPlayTimeField).ToString())) + ")";
                data._screenshot = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0, 0), 100);
                _saveList.AddItem(data).OnSelect();

                ClearTempFolder();
            }
        }
        catch (DirectoryNotFoundException)
        {
            Utils.LogWarning(this, "Save files directory not found, creating a new one now.");
            Directory.CreateDirectory(Constants.SaveFilesPath);
        }
    }

    public void DisplayConfirmDeletePopup()
    {
        _confirmDeletePopup.SetActive(true);
    }

    public void CloseConfirmDeletePopup()
    {
        _confirmDeletePopup.SetActive(false);
    }

    public void OnLoadRequest()
    {
        SelectableListItem currentSelectedFile = _saveList.GetSelectedItem();
        if (currentSelectedFile != null)
        {
            SelectableListItem_SaveData data = (SelectableListItem_SaveData)currentSelectedFile._Data;
            SaveManager.Instance.load(data._filePath);
        }
        else
        {
            Utils.LogError(this, "Requested load on a null file, this should not happen !");
        }
    }

    public void OnDeleteRequest()
    {
        CloseConfirmDeletePopup();
        SelectableListItem currentSelectedFile = _saveList.GetSelectedItem();
        SelectableListItem nextSelectedItem = null;
        if (currentSelectedFile != null)
        {
            nextSelectedItem = _saveList.GetItemAfter(currentSelectedFile);
            if (nextSelectedItem == null)
            {
                nextSelectedItem = _saveList.GetItemBefore(currentSelectedFile);
            }
            if (nextSelectedItem != null)
            {
                _saveList.SelectItem(nextSelectedItem);
            }
            else
            {
                ResetSelection();
            }
            RemoveFileFromDisk(currentSelectedFile);
        }
        else
        {
            Utils.LogError(this, "Requested delete on a null file, this should not happen !");
        }
    }

    #endregion
    
}
