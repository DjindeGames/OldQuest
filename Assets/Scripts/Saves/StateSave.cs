using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class StateSave : MonoBehaviour
{
    public int id = -1;

#if UNITY_EDITOR
    public void Awake()
    {
        if (EditorApplication.isPlaying || !gameObject.scene.isLoaded) return;
        gameObject.tag = Constants.SaveTag;
        generateIdIfNeeded();
    }

    private bool idExists()
    {
        bool exists = false;
        foreach (GameObject o in GameObject.FindGameObjectsWithTag(Constants.SaveTag))
        {
            StateSave stateSave = o.GetComponent<StateSave>();
            if (stateSave)
            {
                if (stateSave.id == id && o != gameObject)
                {
                    exists = true;
                    break;
                }
            }
        }
        return exists;
    }

    public void generateIdIfNeeded()
    {
        if (EditorApplication.isPlaying) return;
        if (id == -1 || idExists())
        {
            id = StateSaveIdsManager.Instance.getNewId();
            Debug.Log("[StateSave] " + gameObject.name + " receives id #" + id);
        }
    }
#endif
}
