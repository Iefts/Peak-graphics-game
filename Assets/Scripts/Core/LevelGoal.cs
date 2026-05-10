using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CircleCollider2D))]
public class LevelGoal : MonoBehaviour
{
    public string nextScene = "MainMenu";
    public float  showDuration = 1.6f;

    bool triggered;

    void Reset()
    {
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.attachedRigidbody) return;
        if (!other.CompareTag("Player") && !other.attachedRigidbody.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(Clear());
    }

    IEnumerator Clear()
    {
        var ui = FindFirstObjectByType<LevelClearUI>();
        if (ui) ui.Show("LEVEL CLEAR");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(showDuration);
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
    }
}
