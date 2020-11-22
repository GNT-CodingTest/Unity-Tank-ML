using UnityEngine;
using UnityEngine.UI;

namespace Tank
{
    public class TankMovement : MonoBehaviour
    {
        public float Fuel { get; private set; }
        public float speed = 12f;                 // How fast the tank moves forward and back.
        public float turnSpeed = 180f;            // How fast the tank turns in degrees per second.

        private Rigidbody _rigidbody;              // Reference used to move the tank.
        private ParticleSystem[] _particleSystems; // References to all the particles systems used by the Tanks
        private TankInput _tankInput;

        public Slider fuelSlider;

        private Vector3 _velocityChange;
        private float _angleChange;
        private const float SmoothTime = 0.05f;

        private void Awake ()
        {
            _tankInput = GetComponent<TankInput>();
            _rigidbody = GetComponent<Rigidbody> ();
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        public void AddFuel(float amount)
        {
            Fuel = Mathf.Min(Fuel + amount, 100f);
        }

        private void OnEnable ()
        {
            fuelSlider.gameObject.SetActive(true);
            Fuel = 100f;
            _rigidbody.isKinematic = false;
            _velocityChange = Vector3.zero;
            _angleChange = 0f;

            foreach (var particle in _particleSystems)
            {
                particle.Play();
            }
        }

        private void OnDisable ()
        {
            fuelSlider.gameObject.SetActive(false);
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;

            _velocityChange = Vector3.zero;
            foreach (var particle in _particleSystems)
            {
                particle.Stop();
            }
        }

        private void Update()
        {
            fuelSlider.value = Fuel;
        }
        
        private void FixedUpdate ()
        {
            if (Fuel <= 0f)
            {
                return;
            }

            Fuel -= Time.deltaTime;

            Move ();
            Turn ();

            Fuel = Mathf.Max(0f, Fuel);
        }
        
        private void Move ()
        {
            Fuel -= _tankInput.VerticalInput * Time.deltaTime;
            _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, transform.forward * _tankInput.VerticalInput * speed, ref _velocityChange, SmoothTime);
        }

        private void Turn ()
        {
            Fuel -= _tankInput.HorizontalInput * 0.5f * Time.deltaTime;
            
            var turn = _tankInput.HorizontalInput * turnSpeed * Time.deltaTime;

            turn = Mathf.SmoothDampAngle(_rigidbody.rotation.y, turn, ref _angleChange, SmoothTime);
            
            var turnRotation = Quaternion.Euler (0f, turn, 0f);
            _rigidbody.MoveRotation (_rigidbody.rotation * turnRotation);
        }
    }
}