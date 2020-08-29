using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public float ElapsedTime { get; private set; } = 0;

    private bool timePaused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(!timePaused)
        {
            ElapsedTime += Time.deltaTime;
        }
    }

    public void pauseTime()
    {
        Time.timeScale = 0;
    }

    public void resumeTime()
    {
        Time.timeScale = 1;
    }

    public void pauseEllapsedTime()
    {
        timePaused = true;
    }

    public void resumeEllapsedTime()
    {
        timePaused = false;
    }
}
