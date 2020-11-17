using System;
using System.Collections.Generic;
using Complete;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tank
{
    public class TankShooting : MonoBehaviour
    {
        [FormerlySerializedAs("m_Shell")] public Rigidbody shell; // Prefab of the shell.

        [FormerlySerializedAs("m_FireTransform")]
        public Transform fireTransform; // A child of the tank where the shells are spawned.

        [FormerlySerializedAs("m_ShootingAudio")]
        public AudioSource
            shootingAudio; // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.

        [FormerlySerializedAs("m_FireClip")] public AudioClip fireClip; // Audio that plays when each shot is fired.

        [FormerlySerializedAs("_launchForce")]
        public float launchForce = 20f; // The force given to the shell if the fire button is not held.
        
        private TankInput _tankInput;
        public event Action<TankHitInfo[]> OnHitTargets;
        public float LastFireTime { get; private set; }
        public readonly float TimeBetFire = 2f;
        
        public Rigidbody LastFiredShell { get; private set; }


        private void OnDisable()
        {
            if (LastFiredShell == null)
            {
                return;
            }

            LastFiredShell.GetComponent<ShellExplosion>().OnHitTargets -= OnHitTargets;
            LastFiredShell = null;
        }

        private void Start()
        {
            _tankInput = GetComponent<TankInput>();
        }

        private void FixedUpdate()
        {
            if (_tankInput.FireInput && Time.time >= LastFireTime + TimeBetFire)
            {
                LastFireTime = Time.time;
                Fire();
            }
        }

        private void Fire()
        {
            var shellInstance = Instantiate(shell, fireTransform.position, fireTransform.rotation);
            LastFiredShell = shellInstance;

            var shellExplosion = shellInstance.GetComponent<ShellExplosion>();
            
            shellExplosion.OnHitTargets += OnHitTargets;
            shellInstance.velocity = launchForce * fireTransform.forward;

            shootingAudio.clip = fireClip;
            shootingAudio.Play();
        }
    }
}