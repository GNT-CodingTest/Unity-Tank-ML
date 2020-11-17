using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    public abstract void Use(GameObject target);
    
    public UnityEvent<Item> onItemDestroyed;

    private void OnDestroy()
    {
        onItemDestroyed.Invoke(this);
    }
}
