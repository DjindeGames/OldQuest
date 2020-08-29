#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEditor;

[ExecuteInEditMode]
public class StateSaveIdsManager : MonoBehaviour
{
    public static StateSaveIdsManager Instance { get; private set; }

    private void OnEnable() 
    {
        Instance = this;
        forceGenerateIds();
    }

    [Button("Force Ids Generation")]
    public void forceGenerateIds()
    {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag(Constants.SaveTag))
        {
            StateSave stateSave = o.GetComponent<StateSave>();
            if (stateSave)
            {
                stateSave.generateIdIfNeeded();
            }
        }
    }

    [Button("Force Ids Reset")]
    public void forceIdsReset()
    {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag(Constants.SaveTag))
        {
            StateSave stateSave = o.GetComponent<StateSave>();
            if (stateSave)
            {
                stateSave.id = -1;
            }
        }
    }

    private int getLastGivenId()
    {
        int maxId = -1;
        foreach (GameObject o in GameObject.FindGameObjectsWithTag(Constants.SaveTag))
        {
            StateSave stateSave = o.GetComponent<StateSave>();
            if (stateSave)
            {
                if (stateSave.id > maxId)
                {
                    maxId = stateSave.id;
                }
            }
        }
        return maxId;
    }

    public int getNewId()
    {
        return getLastGivenId() + 1;
    }
}
#endif