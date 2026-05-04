using UnityEngine;

/// Placed on the castle's inner chamber trigger zone.
/// Fires when the player reaches the level objective.
public class InnerChamberTrigger : MonoBehaviour
{
    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        triggered = true;
        Debug.Log("[InnerChamberTrigger] Player reached the inner chamber — level complete!");
        GameManager.Instance?.EndRun(true);
    }
}
