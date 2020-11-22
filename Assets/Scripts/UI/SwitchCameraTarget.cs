using System.Linq;
using Cinemachine;
using Unity.MLAgents;
using UnityEngine;

public class SwitchCameraTarget : MonoBehaviour
{
    private Agent[] _agents;
    public CinemachineVirtualCamera virtualCamera;

    private const float TimeBetChange = 3f;
    private float _lastChangeTime;

    private void Start()
    {
        if (Academy.Instance.IsCommunicatorOn)
        {
            return;
        }
        
        _agents = FindObjectsOfType<Agent>();
        virtualCamera.LookAt = _agents[0].transform;
    }

    private void Update()
    {
        if (Time.time >= _lastChangeTime + TimeBetChange)
        {
            _lastChangeTime = Time.time + TimeBetChange;
            
            var agentWithMaxReward = _agents.Aggregate((agent1, agent2) => agent1.GetCumulativeReward() > agent2.GetCumulativeReward() ? agent1 : agent2);
            virtualCamera.LookAt = agentWithMaxReward.transform;
        }
    }
}