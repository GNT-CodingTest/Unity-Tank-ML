using UnityEngine;
using UnityEngine.UI;

public class TankMovement : MonoBehaviour
{
    public float Fuel { get; private set; }
    public float speed = 12f;
    public float turnSpeed = 180f;

    private Rigidbody _rigidbody;              // Reference used to move the tank.
    private ParticleSystem[] _particleSystems; // References to all the particles systems used by the Tanks
    private TankInput _tankInput;

    public Slider fuelSlider;

    public AudioSource movementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip engineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip engineDriving;           // Audio to play when the tank is moving.
    public float pitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    private float originalPitch;
        
        
    private void Awake ()
    {
        _tankInput = GetComponent<TankInput>();
        _rigidbody = GetComponent<Rigidbody> ();
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Start()
    {
        originalPitch = movementAudio.pitch;
    }

    public void AddFuel(float amount)
    {
        Fuel = Mathf.Min(Fuel + amount, 100f);
    }
    private void EngineAudio ()
    {
        if (Mathf.Abs (_tankInput.VerticalInput) < 0.1f && Mathf.Abs (_tankInput.HorizontalInput) < 0.1f)
        {
            if (movementAudio.clip == engineDriving)
            {
                movementAudio.clip = engineIdling;
                movementAudio.pitch = Random.Range (originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudio.Play ();
            }
        }
        else
        {
            if (movementAudio.clip == engineIdling)
            {
                movementAudio.clip = engineDriving;
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }

        
    private void OnEnable ()
    {
        fuelSlider.gameObject.SetActive(true);
        Fuel = 100f;
        _rigidbody.isKinematic = false;

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

        foreach (var particle in _particleSystems)
        {
            particle.Stop();
        }
    }

    private void Update()
    {
        fuelSlider.value = Fuel;
        EngineAudio();
    }
        
    private void FixedUpdate ()
    {
        if (Fuel <= 0f)
        {
            return;
        }

        Fuel -= Time.deltaTime;
        Fuel -= _tankInput.VerticalInput * Time.deltaTime;
        Fuel -= _tankInput.HorizontalInput * 0.5f * Time.deltaTime;
            
        Move ();
        Turn ();

        Fuel = Mathf.Max(0f, Fuel);
    }
        
    private void Move ()
    {
        _rigidbody.velocity = transform.forward * _tankInput.VerticalInput * speed;
    }

    private void Turn ()
    {
        var turn = _tankInput.HorizontalInput * turnSpeed * Time.deltaTime;
            
        var turnRotation = Quaternion.Euler (0f, turn, 0f);
        _rigidbody.MoveRotation (_rigidbody.rotation * turnRotation);
    }
}