using System;
using System.Collections.Generic;
using Complete;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles; // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio; // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f; // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f; // The amount of force added to a tank at the centre of the explosion.

    public float
        explosionRadius = 5f; // The maximum distance away from the explosion tanks can be and are still affected.

    public event Action<TankHitInfo[]> OnHitTargets = info => { };

    public float maxLifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var hitInfos = new List<TankHitInfo>();

        var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, m_TankMask);

        foreach (var hitCollider in hitColliders)
        {
            var targetRigidbody = hitCollider.GetComponent<Rigidbody>();
            if (targetRigidbody == null)
            {
                continue;
            }

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, explosionRadius);

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

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        var mainModule = m_ExplosionParticles.main;
        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        var explosionToTarget = targetPosition - transform.position;

        var explosionDistance = explosionToTarget.magnitude;

        var relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        var damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}