using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public UIInstance[] uis;

    private Canvas activeUI;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        activeUI = getCanvas(UIType.Main);
        toggleUI(UIType.Main);
    }

    public void switchUI(UIType which)
    {
        activeUI.enabled = false;
        activeUI = getCanvas(which);
        activeUI.enabled = true;
        //Debug.Log("Switched UI to " + activatedUI.name + " " + activatedUI.enabled);
    }

    public void toggleCurrentUI(bool on)
    {
        activeUI.enabled = on;
    }

    private Canvas getCanvas(UIType which)
    {
        Canvas canvas = null;
        for (int i = 0; i < uis.Length; i++)
        {
            if (uis[i].type == which)
            {
                canvas = uis[i].canvas;
                break;
            }
        }
        return canvas;
    }

    private void toggleUI(UIType which)
    {
        for (int i = 0; i < uis.Length; i++)
        {
            if (uis[i].type == which)
            {
                uis[i].canvas.enabled = true;
            }
            else
            {
                uis[i].canvas.enabled = false;
            }
        }
    }
}

[System.Serializable]
public class UIInstance
{
    public UIType type;
    public Canvas canvas;
}