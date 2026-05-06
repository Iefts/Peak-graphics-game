using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float   smooth = 5f;
    [SerializeField] Vector3 offset = new Vector3(2f, 2f, -10f);

    Transform target;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p) target = p.transform;
    }

    void LateUpdate()
    {
        if (!target) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
    }
}
