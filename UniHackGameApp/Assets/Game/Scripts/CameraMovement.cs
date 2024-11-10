using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Movement speed of the object
    public float moveSpeed = 5f;

    void Update()
    {
        // Get the current position
        Vector3 newPosition = transform.position;

        // Move only on the X axis when pressing W or S
        if (Input.GetKey(KeyCode.W))
            newPosition.x += moveSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            newPosition.x -= moveSpeed * Time.deltaTime;

        // Move only on the Z axis when pressing A or D
        else if (Input.GetKey(KeyCode.A))
            newPosition.z += moveSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            newPosition.z -= moveSpeed * Time.deltaTime;

        // Update the position of the object
        transform.position = newPosition;
    }
}
