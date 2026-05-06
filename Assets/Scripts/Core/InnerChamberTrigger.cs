using UnityEngine;

public class InnerChamberTrigger : MonoBehaviour
{
    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        Debug.Log("[InnerChamberTrigger] Level complete!");
        GameManager.Instance?.EndRun(true);
    }
}
