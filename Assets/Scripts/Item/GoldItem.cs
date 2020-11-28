using Unity.MLAgents;
using UnityEngine;

public class GoldItem : Item
{
    public float reward = 0.2f;
    public override void Use(GameObject target)
    {
        var agent = target.GetComponent<Agent>();

        if (agent != null)
        {
            agent.AddReward(reward);
            Destroy(gameObject);
        }
    }
}
