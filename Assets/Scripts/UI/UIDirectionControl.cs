using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool useRelativeRotation = true;
    private Quaternion _relativeRotation;

    private void Start()
    {
        _relativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (useRelativeRotation)
        {
            transform.rotation = _relativeRotation;
        }
    }
}