using Complete;
using UnityEngine;

public class HealthItem : Item
{
    public float amount = 30f;
    
    public override void Use(GameObject target)
    {
        var tankHealth = target.GetComponent<TankHealth>();

        if (tankHealth != null)
        {
            tankHealth.RestoreHealth(amount);
            Destroy(gameObject);
        }
    }
}