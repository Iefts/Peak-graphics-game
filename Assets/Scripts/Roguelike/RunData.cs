using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunData
{
    public int runNumber;
    public float startTime;
    public float endTime;
    public bool completed;
    public int enemiesDefeated;
    public List<string> itemsCollected = new List<string>();

    public RunData()
    {
        runNumber = PlayerPrefs.GetInt("TotalRuns", 0) + 1;
        PlayerPrefs.SetInt("TotalRuns", runNumber);
        startTime = Time.time;
    }

    public float GetRunDuration() => endTime - startTime;
}
