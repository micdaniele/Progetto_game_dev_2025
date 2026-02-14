using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player2DGroundMover : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] private float _speed = 5f;//velocità

    private Animator _animator;//variabile per l'animator

    [Header("Audio Config")]
    [SerializeField] private AudioClip[] _footstepSounds;//array con i suoni dei passi
    [SerializeField] private float _footstepInterval = 0.5f;//intervallo di tempo in cui far partire il suono tra un passo e l'altro
    [SerializeField] private float _audioVolume = 1f;//volume audio dei passi

    // Input
    private InputAction _moveAction;
    private Vector2 _inputMovement;

    // Componenti
    private Rigidbody2D _rb;
    private AudioSource _audioSource;

    // Footstep timer
    private float _footstepTimer;
    //player ingelton
    private static Player2DGroundMover instance;

    void Awake()
    {
        // Gestione singleton
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Il player rimane sempre persistente tra le scene
        DontDestroyOnLoad(gameObject);

        // Registra il listener per il cambio scena e aggiunge OnSceneLoaded all'inizio della scena
        // Permettendo così di controllare la visibilità ogni volta che si cambia scena
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Controlla subito la visibilità nella scena corrente
        UpdatePlayerVisibility(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        // rimuove il listener quando il player viene distrutto
        // per evitare errori di memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Questo metodo viene chiamato automaticamente ogni volta che una nuova scena viene caricata
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdatePlayerVisibility(scene.name);
    }

    // Controlla se il player deve essere visibile o nascosto
    private void UpdatePlayerVisibility(string sceneName)
    {
        // Il player è visibile SOLO nella scena Kitchen2
        bool shouldBeVisible = (sceneName == "Kitchen2");
        //disabilita il gameobject
        gameObject.SetActive(shouldBeVisible);

        //Debug.Log($"[Player] Scena: {sceneName} - Player visibile: {shouldBeVisible}");
    }

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
        _audioSource.spatialBlend = 0f;

        // Azione di movimento
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //Debug.Log("Player instance ID: " + GetInstanceID());

        // Sistema di ripristino della posizione
        if (GameManager.Instance != null && GameManager.Instance.ShouldRestorePlayerPosition())
        {
            Vector2 savedPos = GameManager.Instance.GetPlayerPosition();
            _rb.position = savedPos;
            _rb.linearVelocity = Vector2.zero;
            GameManager.Instance.ClearPositionRestore();

            //Debug.Log("Restore flag: " + GameManager.Instance.ShouldRestorePlayerPosition());
            //Debug.Log("Player instance ID: " + GetInstanceID());
        }
    }

    void Update()
    {
        // Legge l'input
        _inputMovement = _moveAction.ReadValue<Vector2>();

        // Aggiorna l'animator
        if (_animator != null)
        {
            //imposta l'intensità del movimento
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

        // Gestione del suono dei passi
        HandleFootsteps();
    }

    //applica il movimento al rigid body
    void FixedUpdate()
    {

        _rb.linearVelocity = _inputMovement * _speed;
    }

    private void HandleFootsteps()
    {
        bool isMoving = _inputMovement.magnitude > 0.01f;

        // fa partire il suono dei passi solo se il personaggio si sta muovendo
        if (isMoving && _footstepSounds != null && _footstepSounds.Length > 0)
        {
            //timer per i passi per far in modo che non suonano ad ogni frame ma a intervalli regolari
            _footstepTimer -= Time.deltaTime;

            //Simula il ritmo naturale dei passi.
            if (_footstepTimer <= 0f)
            {
                PlayFootstepSound();
                _footstepTimer = _footstepInterval;
            }
        }
        else
        {
            //il timer si resetta il prossimo passo parte subito quando riprendi a muoverti
            _footstepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        if (_audioSource == null) return;
        if (_footstepSounds == null || _footstepSounds.Length == 0) return;

        // Scegli un suono casuale dall'array
        AudioClip clip = _footstepSounds[Random.Range(0, _footstepSounds.Length)];
        if (clip == null) return;

        //piccola variazione nel suono dei passi per suono più realistico 
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(clip, _audioVolume);
    }

    //Salva la posizione del player quando cambi scena e quando entri in un minigioco
    public void SavePosition()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerPosition(transform.position);
        }
    }
}