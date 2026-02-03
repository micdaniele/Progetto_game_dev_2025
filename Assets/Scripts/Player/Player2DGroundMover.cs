using UnityEngine;
using UnityEngine.InputSystem;

public class Player2DGroundMover : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] private float _speed = 5f;

    [Header("Animation Config")]
    [SerializeField] private Animator _animator;

    [Header("Audio Config")]
    [SerializeField] private AudioClip[] _footstepSounds;
    [SerializeField] private float _footstepInterval = 0.5f;
    [SerializeField] private float _audioVolume = 1f; // Volume dei passi

    // Input
    private InputAction _moveAction;
    private Vector2 _inputMovement;

    // Componenti
    private Rigidbody2D _rb;
    private AudioSource _audioSource;

    // Footstep timer
    private float _footstepTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_animator == null) _animator = GetComponent<Animator>();

        // Setup AudioSource
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            //Debug.Log("[Footsteps] AudioSource creato automaticamente");
        }

        // Configura l'AudioSource per i passi
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
        _audioSource.volume = _audioVolume;
        _audioSource.spatialBlend = 0f; // 2D audio

        // DEBUG: Verifica configurazione
        //Debug.Log($"[Footsteps] AudioClips configurati: {(_footstepSounds != null ? _footstepSounds.Length : 0)}");
        //if (_footstepSounds != null)
        //{
        //    for (int i = 0; i < _footstepSounds.Length; i++)
        //    {
        //        if (_footstepSounds[i] == null)
        //            Debug.LogWarning($"[Footsteps] AudioClip {i} è NULL!");
        //        else
        //            Debug.Log($"[Footsteps] AudioClip {i}: {_footstepSounds[i].name}");
        //    }
        //}

        _moveAction = InputSystem.actions.FindAction("Move");
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Ripristina la posizione
        if (GameManager.Instance != null && GameManager.Instance.HasSavedPlayerPosition())
        {
            transform.position = GameManager.Instance.GetPlayerPosition();
        }
    }

    void Update()
    {
        // Legge l'input
        _inputMovement = _moveAction.ReadValue<Vector2>();

        if (_animator != null)
        {
            _animator.SetFloat("Speed", _inputMovement.magnitude);

            if (_inputMovement.magnitude > 0.01f)
            {
                _animator.SetFloat("Move x", _inputMovement.x);
                _animator.SetFloat("Move y", _inputMovement.y);

                if (_inputMovement.x < -0.01f)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (_inputMovement.x > 0.01f)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        HandleFootsteps();
    }

    void FixedUpdate()
    {
         _rb.linearVelocity = _inputMovement * _speed;
    }

    private void HandleFootsteps()
    {
        bool isMoving = _inputMovement.magnitude > 0.01f;

        // Suona i passi solo se il personaggio si sta muovendo
        if (isMoving && _footstepSounds != null && _footstepSounds.Length > 0)
        {
            _footstepTimer -= Time.deltaTime;

            if (_footstepTimer <= 0f)
            {
                PlayFootstepSound();
                _footstepTimer = _footstepInterval;
            }
        }
        else
        {
            _footstepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        if (_audioSource == null)
        {
            //Debug.LogError("[Footsteps] AudioSource è NULL!");
            return;
        }

        if (_footstepSounds == null || _footstepSounds.Length == 0)
        {
            //Debug.LogError("[Footsteps] Nessun AudioClip configurato!");
            return;
        }

        // Scegli un suono casuale dall'array
        AudioClip clip = _footstepSounds[Random.Range(0, _footstepSounds.Length)];

        if (clip == null)
        {
            //Debug.LogError("[Footsteps] AudioClip selezionato è NULL!");
            return;
        }

        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(clip, _audioVolume);

        //Debug.Log($"[Footsteps] Suono riprodotto: {clip.name}, Volume: {_audioVolume}");
    }

    public void SavePosition()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerPosition(transform.position);
        }
    }
}