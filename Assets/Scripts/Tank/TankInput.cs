using UnityEngine;

public class TankInput : MonoBehaviour
{
    public float HorizontalInput { get; set; }
    public float VerticalInput { get; set; }
    public bool FireInput { get; set; }

    public void ResetAllInputs()
    {
        HorizontalInput = 0f;
        VerticalInput = 0f;
        FireInput = false;
    }
}