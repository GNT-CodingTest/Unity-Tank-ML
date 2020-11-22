using UnityEngine;

public class FuelItem : Item
{
    public float amount = 30f;
    
    public override void Use(GameObject target)
    {
        var tankMovement = target.GetComponent<TankMovement>();

        if (tankMovement != null)
        {
            tankMovement.AddFuel(amount);
            Destroy(gameObject);
        }
    }
}