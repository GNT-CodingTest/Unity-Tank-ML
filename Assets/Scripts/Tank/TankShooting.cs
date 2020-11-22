using System;
using UnityEngine;

namespace Tank
{
    public class TankShooting : MonoBehaviour
    {
        public Rigidbody shell;
        public Transform fireTransform;
        
        public AudioSource shootingAudio;
        public AudioClip fireClip;
        
        public float launchForce = 18f;
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