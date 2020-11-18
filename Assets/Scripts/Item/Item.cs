using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    private float _rotateSpeed = 180f;
    public abstract void Use(GameObject target);
    
    public UnityEvent<Item> onItemDestroyed;

    private void OnDestroy()
    {
        onItemDestroyed.Invoke(this);
    }

    private void FixedUpdate()
    {
        transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f);
    }
}
