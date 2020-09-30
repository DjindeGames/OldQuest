using System;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image screenShot;
    [SerializeField]
    private TMP_Text chapter;
    [SerializeField]
    private Button loadButton;
    [SerializeField]
    private Button deleteButton;
    [SerializeField]
    private GameObject confirmDelete;
    [SerializeField]
    private SelectableList _saveList;

    public static LoadScreenManager Instance { get; private set; }

    private SelectableListItem_SaveData _selectedSaveData;

    private void Awake()
    {
        Instance = this;
        confirmDelete.SetActive(false);
        _saveList.OnItemSelectedEvent += OnSaveFileSelected;
        refreshList();
    }

    private void OnDestroy()
    {
        _saveList.OnItemSelectedEvent -= OnSaveFileSelected;
    }

    public void refreshList()
    {
        try
        {
            string[] saveFilesNames = Directory.GetFiles(Constants.SaveFilesPath, "*" + Constants.ZippedSavesExtension);

            Array.Sort(saveFilesNames);

            clearTemp();

            for (int i = saveFilesNames.Length - 1; i >= 0; i--)
            {

                ZipFile.ExtractToDirectory(saveFilesNames[i], Constants.TemporaryExtractedSavesPath);
                JSONObject saveFile = new JSONObject(System.IO.File.ReadAllText(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.SaveFilesExtension)[0]));

                // Generating screenshot
                byte[] byteArray = System.IO.File.ReadAllBytes(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.ScreenshotsExtension)[0]);
                Texture2D screenshot = new Texture2D(2, 2);
                screenshot.LoadImage(byteArray);

                // Generating file prefabs
                SelectableListItem_SaveData data = new SelectableListItem_SaveData();
                data._label = saveFile.GetField(Constants.SFSerializedNameField).str + "\n" + saveFile.GetField(Constants.SFSerializedDateField).str;
                data._filePath = saveFilesNames[i];
                data._details = saveFile.GetField(Constants.SFSerializedChapterField).str + " (Played Time: " + formatPlayedTime(Int32.Parse(saveFile.GetField(Constants.SFSerializedPlayTimeField).ToString())) + ")";
                data._screenshot = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0, 0), 100);
                _saveList.AddItem(data).OnSelect();
                clearTemp();
            }
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(Constants.SaveFilesPath);
        }
    }

    public void displayConfirm()
    {
        confirmDelete.SetActive(true);
    }

    public void closeConfirm()
    {
        confirmDelete.SetActive(false);
    }

    public void OnSaveFileSelected(SelectableListItem listItem)
    {
        SelectableListItem_SaveData data = (SelectableListItem_SaveData)listItem._Data;
        _selectedSaveData = data;

        // Enabling right part
        screenShot.color = Color.white;
        loadButton.interactable = true;
        deleteButton.interactable = true;
        screenShot.sprite = data._screenshot;
        chapter.text = data._details;
    }

    private void ResetSelection()
    {
        screenShot.color = Color.black;
        loadButton.interactable = false;
        deleteButton.interactable = false;
        chapter.text = "No available save file!";
    }

    public void load()
    {
        SaveManager.Instance.load(_selectedSaveData._filePath);
    }

    public void delete()
    {
        closeConfirm();
        SelectableListItem currentSelectedFile = _saveList.GetSelectedItem();
        SelectableListItem nextSelectedItem = null;
        if (currentSelectedFile != null)
        {
            nextSelectedItem = _saveList.GetItemAfter(currentSelectedFile);
            if (nextSelectedItem == null)
            {
                nextSelectedItem = _saveList.GetItemBefore(currentSelectedFile);
            }
        }
        if (nextSelectedItem != null)
        {
            OnSaveFileSelected(nextSelectedItem);
        }
        _saveList.RemoveItem(currentSelectedFile);        
    }

    private void clearTemp()
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
        int minutes = Mathf.FloorToInt(((float)(timeInSeconds%3600)) / 60);
        int seconds = timeInSeconds % 60;
        string hoursString = (hours >= 10) ? hours.ToString() : "0" + hours;
        string minutesString = (minutes >= 10) ? minutes.ToString() : "0" + minutes;
        string secondsString = (seconds >= 10) ? seconds.ToString() : "0" + seconds;
        return (hoursString + ":" + minutesString + ":" + secondsString);
    }
}
