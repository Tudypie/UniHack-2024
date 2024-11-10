using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Movement speed of the object
    public float moveSpeed = 5f;

    void Update()
    {
        // Variable to store the movement direction
        Vector3 movement = Vector3.zero;

        // Move only on the X axis when pressing W or S
        if (Input.GetKey(KeyCode.D))
            movement = new Vector3(1f, 0f, 0f);
        else if (Input.GetKey(KeyCode.A))
            movement = new Vector3(-1f, 0f, 0f);

        // Move only on the Z axis when pressing A or D
        else if (Input.GetKey(KeyCode.W))
            movement = new Vector3(0f, 1f, 0f);
        else if (Input.GetKey(KeyCode.S))
            movement = new Vector3(0f, -1f, 0f);

        // Apply movement if any input is detected
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
