using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    public float rotateSpeed = 180f;
    public abstract void Use(GameObject target);
    
    public UnityEvent<Item> onItemDestroyed;

    private void OnDestroy()
    {
        onItemDestroyed.Invoke(this);
    }

    private void FixedUpdate()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }
}
