using Cinemachine;
using Unity.MLAgents;
using UnityEngine;

public class SwitchCameraTarget : MonoBehaviour
{
    private Agent[] _agents;

    public CinemachineVirtualCamera virtualCamera;

    private float timeBetChange = 5f;

    private float lastChangeTime;
    private void Start()
    {
        _agents = FindObjectsOfType<Agent>();

        virtualCamera.LookAt = _agents[0].transform;

        if (Academy.Instance.IsCommunicatorOn)
        {
            // 전체 보기 카메라로 세팅
        }
    }

    private void Update()
    {
        if (Time.unscaledTime >= lastChangeTime + timeBetChange)
        {
            lastChangeTime = Time.unscaledTime + timeBetChange;
            
            virtualCamera.LookAt = _agents[Random.Range(0, _agents.Length)].transform;
        }
    }
}
