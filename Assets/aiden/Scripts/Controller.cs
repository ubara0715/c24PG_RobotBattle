using UnityEngine;

public class Controller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // This checks if the player is on the ground
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
