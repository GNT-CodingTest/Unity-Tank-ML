using System;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask tankMask;
    public ParticleSystem explosionParticles;
    public AudioSource explosionAudio;
    public float maxDamage = 100f;
    public float explosionForce = 1000f;
    public float explosionRadius = 5f;
    public event Action<TankHitInfo[]> OnHitTargets = info => { };

    public float maxLifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var hitInfos = new List<TankHitInfo>();

        var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);

        foreach (var hitCollider in hitColliders)
        {
            var targetRigidbody = hitCollider.GetComponent<Rigidbody>();
            if (targetRigidbody == null)
            {
                continue;
            }

            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            var tankHealth = hitCollider.GetComponent<TankHealth>();

            if (tankHealth == null || tankHealth.IsDead)
            {
                continue;
            }

            var damage = CalculateDamage(targetRigidbody.position);
            var damageApplied = tankHealth.TakeDamage(damage);

            var hitInfo = new TankHitInfo
            {
                Target = tankHealth.gameObject,
                AppliedDamage = damageApplied,
                TargetKilled = tankHealth.IsDead
            };

            hitInfos.Add(hitInfo);
        }

        OnHitTargets(hitInfos.ToArray());

        explosionParticles.transform.parent = null;
        explosionParticles.Play();
        explosionAudio.Play();

        var mainModule = explosionParticles.main;
        Destroy(explosionParticles.gameObject, mainModule.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        var explosionToTarget = targetPosition - transform.position;

        var explosionDistance = explosionToTarget.magnitude;

        var relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        var damage = relativeDistance * maxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}