using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform filesParent;
    [SerializeField]
    private GameObject filePrefab;
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

    [Header("Parameters")]
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color selectedColor;

    private List<SaveFile> files;


    private string savesPath;
    private SaveFile currentSelectedFile;
    private int filesNumber = 0;


    public static LoadScreenManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        confirmDelete.SetActive(false);
        refreshList();
    }

    public void refreshList()
    {
        clearFiles();
        files = new List<SaveFile>();
        
        try
        {
            string[] saveFilesNames = Directory.GetFiles(Constants.SaveFilesPath, "*" + Constants.ZippedSavesExtension);

            filesNumber = saveFilesNames.Length;


            Array.Sort(saveFilesNames);

            clearTemp();

            for (int i = filesNumber - 1; i >= 0; i--)
            {

                ZipFile.ExtractToDirectory(saveFilesNames[i], Constants.TemporaryExtractedSavesPath);
                JSONObject saveFile = new JSONObject(System.IO.File.ReadAllText(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.SaveFilesExtension)[0]));
                SaveFile fileInstance;

                // Generating screenshot
                byte[] byteArray = System.IO.File.ReadAllBytes(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.ScreenshotsExtension)[0]);
                Texture2D screenshot = new Texture2D(2, 2);
                screenshot.LoadImage(byteArray);

                // Generating file prefabs
                GameObject file = Instantiate(filePrefab, filesParent);
                fileInstance = file.GetComponent<SaveFile>();
                fileInstance.FilePath = saveFilesNames[i];
                fileInstance.Name = saveFile.GetField(Constants.SFSerializedNameField).str + "\n" + saveFile.GetField(Constants.SFSerializedDateField).str;
                fileInstance.Details = saveFile.GetField(Constants.SFSerializedChapterField).str + " (Played Time: " + formatPlayedTime(Int32.Parse(saveFile.GetField(Constants.SFSerializedPlayTimeField).ToString())) + ")";
                fileInstance.ScreenShot = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0, 0), 100);
                fileInstance.label.text = fileInstance.Name;
                file.GetComponent<Button>().onClick.AddListener(delegate () { select(fileInstance); });

                files.Add(fileInstance);
                clearTemp();
            }
            if (files.Count > 0)
            {
                select(files[0]);
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

    public void select(SaveFile file)
    {
        //Unselecting previous
        if (currentSelectedFile)
        {
            currentSelectedFile.GetComponent<Image>().color = normalColor;
        }

        //Updating current
        currentSelectedFile = file;

        if (currentSelectedFile)
        {
            // Enabling right part
            screenShot.color = Color.white;
            loadButton.interactable = true;
            deleteButton.interactable = true;
            //Selecting new file
            currentSelectedFile.gameObject.GetComponent<Image>().color = selectedColor;
            screenShot.sprite = currentSelectedFile.ScreenShot;
            chapter.text = currentSelectedFile.Details;
        }
        else
        {
            screenShot.color = Color.black;
            loadButton.interactable = false;
            deleteButton.interactable = false;
            chapter.text = "No available save file!";
        }
    }

    private void clearFiles()
    {
        if (files != null)
        {
            foreach (SaveFile file in files)
            {
                Destroy(file.gameObject);
            }
            files.Clear();
        }
    }

    public void load()
    {
        SaveManager.Instance.load(currentSelectedFile.FilePath);
    }

    public void delete()
    {
        SaveFile nextSelectedFile = getNextAvailableFile();
        closeConfirm();
        System.IO.File.Delete(currentSelectedFile.FilePath);
        files.Remove(currentSelectedFile);
        Destroy(currentSelectedFile.gameObject);
        select(nextSelectedFile);
    }

    
    private SaveFile getNextAvailableFile()
    {
        SaveFile nextAvailable = null;
        int currentIndex = files.IndexOf(currentSelectedFile);
        if (currentIndex > 0)
        {
            nextAvailable = files[currentIndex - 1];
        } else if (files.Count > 1) {
            nextAvailable = files[currentIndex + 1];
        }
        return nextAvailable;
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
