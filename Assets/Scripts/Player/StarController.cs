using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StarController : MonoBehaviour
{
    [SerializeField] float rollTorque = 30f;
    [SerializeField] float jumpForce  = 11f;
    [SerializeField] float maxAngVel  = 720f;

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void FixedUpdate()
    {
        float dir = 0f;
        if (Input.GetKey(KeyCode.A)) dir -= 1f;
        if (Input.GetKey(KeyCode.D)) dir += 1f;
        if (dir != 0f) rb.AddTorque(-dir * rollTorque);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngVel, maxAngVel);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    bool IsGrounded()
    {
        var contacts = new ContactPoint2D[8];
        int count = rb.GetContacts(contacts);
        for (int i = 0; i < count; i++)
            if (contacts[i].normal.y > 0.25f) return true;
        return false;
    }
}
