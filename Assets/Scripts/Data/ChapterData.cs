using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterData : MonoBehaviour
{
    [Header("Chapter's Data")]
    public int chapterNumber;
    public string chapterName;

    public static ChapterData Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
