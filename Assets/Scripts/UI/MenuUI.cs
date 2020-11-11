using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NaughtyAttributes;

namespace Djinde.Quest
{
    public class MenuUI : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField]
        private TMP_InputField saveName;

        [Header("Windows")]
        [BoxGroup("References")]
        [SerializeField]
        private GameObject confirmWindow;
        [BoxGroup("References")]
        [SerializeField]
        private GameObject menu;
        [BoxGroup("References")]
        [SerializeField]
        private GameObject options;
        [BoxGroup("References")]
        [SerializeField]
        private GameObject saveInProgress;
        [BoxGroup("References")]
        [SerializeField]
        private GameObject saveInput;
        [BoxGroup("References")]
        [SerializeField]
        private GameObject fileNameError;
        [BoxGroup("References")]
        [SerializeField]
        private GameObject loadScreen;

        [Header("Sliders")]
        [BoxGroup("References")]
        [SerializeField]
        private Slider effectsVolume;
        [BoxGroup("References")]
        [SerializeField]
        private Slider physicsVolume;
        [BoxGroup("References")]
        [SerializeField]
        private Slider musicsVolume;
        [BoxGroup("References")]
        [SerializeField]
        private Slider firstPersonMouseSensitivity;

        public static MenuUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            menu.SetActive(true);
            fileNameError.SetActive(false);
            confirmWindow.SetActive(false);
            saveInProgress.SetActive(false);
            saveInput.SetActive(false);
            options.SetActive(false);
            loadScreen.SetActive(false);
            refreshSettings();
            SaveManager.Instance.saveIsComplete += onSaveCompleted;
        }

        private void onSaveCompleted()
        {
            saveInProgress.SetActive(false);
        }

        public void refreshSettings()
        {
            musicsVolume.value = SettingsManager.Instance.MusicVolume;
            effectsVolume.value = SettingsManager.Instance.EffectsVolume;
            physicsVolume.value = SettingsManager.Instance.PhysicsVolume;
            firstPersonMouseSensitivity.value = SettingsManager.Instance.FirstPersonMouseSensitivity;
        }

        public void closeMenu()
        {
            closeOptions();
            closeSaveInput();
            discardConfirmExit();
            closeLoadManager();
            ScreenManager.Instance.switchScreen(EScreenType.Main);
        }

        public void displayOptions()
        {
            options.SetActive(true);
        }

        public void closeOptions()
        {
            SaveManager.Instance.SaveSettings();
            options.SetActive(false);
        }

        public void closeFileNameError()
        {
            fileNameError.SetActive(false);
        }

        public void displaySaveInput()
        {
            saveInput.SetActive(true);
            saveName.Select();
        }

        public void closeSaveInput()
        {
            closeFileNameError();
            saveInput.SetActive(false);
        }

        public void displayConfirmExit()
        {
            PlayerController.Instance.cancelMove();
            confirmWindow.SetActive(true);
        }

        public void discardConfirmExit()
        {
            PlayerController.Instance.cancelMove();
            confirmWindow.SetActive(false);
        }

        public void displayLoadManager()
        {
            loadScreen.SetActive(true);
        }

        public void closeLoadManager()
        {
            loadScreen.SetActive(false);
        }

        public void save()
        {
            if (saveName.text.Length >= 1
                && !saveName.text.Contains("/")
                && !saveName.text.Contains("\\")
                && !saveName.text.Contains(":")
                && !saveName.text.Contains("*")
                && !saveName.text.Contains("?")
                && !saveName.text.Contains("|")
                && !saveName.text.Contains("\"")
                && !saveName.text.Contains("<")
                & !saveName.text.Contains(">"))
            {
                saveInProgress.SetActive(true);
                closeSaveInput();
                SaveManager.Instance.save(saveName.text);
            }
            else
            {
                fileNameError.SetActive(true);
            }
        }

        public void exit()
        {
            TimeManager.Instance.resumeTime();
            SceneManager.LoadScene("MainMenu");
        }

        public void updatePhysicsVolume(Slider slider)
        {
            SettingsManager.Instance.setVolume(EVolumeType.Physics, slider.value);
        }

        public void updateEffectsVolume(Slider slider)
        {
            SettingsManager.Instance.setVolume(EVolumeType.Effects, slider.value);
        }

        public void updateMusicsVolume(Slider slider)
        {
            SettingsManager.Instance.setVolume(EVolumeType.Music, slider.value);
        }

        public void updateFirstPersonMouseSensitivity(Slider slider)
        {
            SettingsManager.Instance.setFirstPersonMouseSensitivity(slider.value);
        }

        public void setFileName(string name)
        {
            saveName.text = name;
        }

        private void OnDestroy()
        {
            SaveManager.Instance.saveIsComplete -= onSaveCompleted;
        }
    }
}