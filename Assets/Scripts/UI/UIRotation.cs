using UnityEngine;

public class UIRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // Rotation speed in degrees per second
    [SerializeField] private bool isRotateY;

    private bool isRotating = true; // Flag to control whether the object is rotating

    void Update()
    {
        // Only rotate if isRotating is true
        if (isRotating)
        {
            // Calculate the rotation for this frame
            float rotationStep = rotationSpeed * Time.deltaTime;

            // Apply the rotation to the Z axis
            if(!isRotateY)
                transform.Rotate(0, 0, rotationStep);
            else
                transform.Rotate(0, rotationStep, 0);
        }
    }

    // Method to start the rotation
    public void StartRotation()
    {
        isRotating = true;
    }

    // Method to stop the rotation
    public void StopRotation()
    {
        isRotating = false;
    }

    // Optional: Method to toggle the rotation
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }
}
