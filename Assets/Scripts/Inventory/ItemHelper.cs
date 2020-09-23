using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHelper
{
    static GameObject getPrefabFromItem(Item item)
    {
        GameObject prefab = null;
        prefab = Resources.Load<GameObject>(Constants.prefabsPath + item.Type.ToString() + "/" + item.name + '$');
        return prefab;
    }
}
