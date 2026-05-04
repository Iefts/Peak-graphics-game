using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Session")]
    public int playerCount = 1; // 1–4
    public List<GameObject> activePlayers = new List<GameObject>();

    [Header("Run")]
    public RunData currentRun;
    public bool isRunActive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartNewRun();
    }

    public void StartNewRun()
    {
        currentRun = new RunData();
        isRunActive = true;
        Debug.Log($"[GameManager] Run #{currentRun.runNumber} started. Players: {playerCount}");
    }

    public void EndRun(bool success)
    {
        currentRun.completed = success;
        currentRun.endTime = Time.time;
        isRunActive = false;
        Debug.Log($"[GameManager] Run ended — Success: {success}, Duration: {currentRun.GetRunDuration():F1}s");
    }

    public void RegisterPlayer(GameObject player)
    {
        if (!activePlayers.Contains(player))
            activePlayers.Add(player);
    }

    public void UnregisterPlayer(GameObject player)
    {
        activePlayers.Remove(player);
        if (activePlayers.Count == 0 && isRunActive)
            EndRun(false);
    }
}
