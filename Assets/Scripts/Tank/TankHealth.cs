using System;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float maxHealth = 100f;
        public Slider healthSlider;
        public Image fillImage;
        public Color fullHealthColor = Color.green;
        public Color zeroHealthColor = Color.red;

        public GameObject tankRender;
        private Collider _collider;

        
        public GameObject explosionPrefab;

        public bool IsDead { get; private set; }
        
        private AudioSource _explosionAudio;
        private ParticleSystem _explosionParticles;
        public float CurrentHealth { get; private set; }
        public event Action OnTankDead = () => { };
        
        public void RestoreHealth(float health)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + health);
            SetHealthUI();
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            _explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
            _explosionAudio = _explosionParticles.GetComponent<AudioSource>();
            _explosionParticles.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CurrentHealth = maxHealth;
            IsDead = false;
            tankRender.SetActive(true);
            _collider.enabled = true;
            SetHealthUI();
            healthSlider.gameObject.SetActive(true);
        }
        

        public float TakeDamage(float damage)
        {
            if (IsDead)
            {
                return 0f;
            }

            var appliedDamage = damage;
            if (damage >= CurrentHealth)
            {
                appliedDamage = CurrentHealth;
                Death();
            }
            else
            {
                CurrentHealth -= appliedDamage;
            }
            
            SetHealthUI();
            return appliedDamage;
        }


        private void SetHealthUI()
        {
            healthSlider.value = CurrentHealth;
            fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, CurrentHealth / maxHealth);
        }

        public void Death()
        {
            IsDead = true;
            tankRender.SetActive(false);
            _collider.enabled = false;
            
            healthSlider.gameObject.SetActive(false);

            _explosionParticles.transform.position = transform.position;
            _explosionParticles.gameObject.SetActive(true);
            
            _explosionParticles.Play();
            _explosionAudio.Play();
            
            OnTankDead();
        }
    }
}