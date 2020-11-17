using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    private Agent[] _agents;

    public Text text;
    // Start is called before the first frame update
    private void Start()
    {
        _agents = FindObjectsOfType<Agent>();
    }

    // Update is called once per frame
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
