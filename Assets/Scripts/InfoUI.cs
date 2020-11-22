using System.Text;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    private Agent[] _agents;

    public Text text;

    private void Start()
    {
        _agents = FindObjectsOfType<Agent>();
    }

    private void Update()
    {
        var stringBuilder = new StringBuilder();
        foreach (var agent in _agents)
        {
            var reward = agent.GetCumulativeReward();
            stringBuilder.AppendLine($"{agent.gameObject.name} : {reward}");
        }

        text.text = stringBuilder.ToString();
    }
}