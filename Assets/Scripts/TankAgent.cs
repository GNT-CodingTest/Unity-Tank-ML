using Complete;
using Tank;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TankAgent : Agent
{
    private TankInput _tankInput;

    private TankHealth _tankHealth;
    private TankMovement _tankMovement;
    private TankShooting _tankShooting;

    private SpawnPointProvider[] _spawnPointProviders;

    private void Awake()
    {
        _tankInput = GetComponent<TankInput>();
        _tankHealth = GetComponent<TankHealth>();
        _tankShooting = GetComponent<TankShooting>();
        _tankMovement = GetComponent<TankMovement>();

        _spawnPointProviders = FindObjectsOfType<SpawnPointProvider>();

        _tankHealth.OnTankDead += OnTankDead;
        _tankShooting.OnHitTargets += OnHitTargets;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetButton("Fire1") ? 1 : 0;
    }

    public void OnHitTargets(TankHitInfo[] tankHitInfos)
    {
        if (tankHitInfos.Length <= 0 || tankHitInfos.Length == 1 && tankHitInfos[0].Target == gameObject)
        {
            // 총알 낭비
            AddReward(-0.1f);
        }

        foreach (var tankHitInfo in tankHitInfos)
        {
            if (tankHitInfo.Target == gameObject)
            {
                AddReward(-tankHitInfo.AppliedDamage * 0.01f);
            }
            else
            {
                AddReward(tankHitInfo.AppliedDamage * 0.01f);

                if (tankHitInfo.TargetKilled)
                {
                    AddReward(1f);
                }
            }
        }
    }

    private void SetActiveTankComponents(bool active)
    {
        _tankHealth.enabled = active;
        _tankMovement.enabled = active;
        _tankShooting.enabled = active;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (_tankHealth.IsDead)
        {
            _tankInput.ResetAllInputs();
            return;
        }

        _tankInput.HorizontalInput = Mathf.Clamp(vectorAction[0], -1f, 1f);
        _tankInput.VerticalInput = Mathf.Clamp(vectorAction[1], -1f, 1f);
        _tankInput.FireInput = vectorAction[2] >= 1f;
    }

    private void OnTankDead()
    {
        _tankInput.ResetAllInputs();
        SetActiveTankComponents(false);
        EndEpisode();
    }

    private void FixedUpdate()
    {
        if (!_tankHealth.IsDead && _tankMovement.Fuel <= 0f)
        {
            _tankHealth.Death();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_tankHealth.CurrentHealth * 0.01f);
        sensor.AddObservation(_tankMovement.Fuel * 0.01f);

        sensor.AddObservation(Mathf.Clamp01((Time.time - _tankShooting.LastFireTime) / _tankShooting.TimeBetFire));

        if (_tankShooting.LastFiredShell != null)
        {
            sensor.AddObservation(1f);
            sensor.AddObservation(transform.InverseTransformDirection(_tankShooting.LastFiredShell.velocity) / 15f);
            sensor.AddObservation(transform.InverseTransformPoint(_tankShooting.LastFiredShell.position) / 25f);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector3.zero);
        }
    }

    public override void OnEpisodeBegin()
    {
        var spawnPointProvider = _spawnPointProviders[Random.Range(0, _spawnPointProviders.Length)];
        var spawnPosition = spawnPointProvider.GetRandomSpawnPoint(3f);

        transform.position = spawnPosition;
        transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);

        _tankInput.ResetAllInputs();
        SetActiveTankComponents(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_tankHealth.IsDead)
        {
            return;
        }

        var item = other.GetComponent<Item>();
        if (item != null)
        {
            item.Use(gameObject);
        }
    }
}