using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    private Vector3 Target;
    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.LookAt(Target);
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void RotateTo(Vector3 TargetTransform)
    {
        Target = TargetTransform;
    }
}
