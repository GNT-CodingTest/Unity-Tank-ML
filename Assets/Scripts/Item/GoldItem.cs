using Unity.MLAgents;
using UnityEngine;

public class GoldItem : Item
{
    public override void Use(GameObject target)
    {
        var agent = target.GetComponent<Agent>();

        if (agent != null)
        {
            agent.AddReward(0.2f);
        }
    }
}
