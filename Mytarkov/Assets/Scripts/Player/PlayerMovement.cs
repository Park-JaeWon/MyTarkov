using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float gravity = -9.8f;
    public float jumpForce = 10f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool IsGrounded;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        IsGrounded = controller.isGrounded;

        if(IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}