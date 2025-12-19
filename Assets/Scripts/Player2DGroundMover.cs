using UnityEngine;
using UnityEngine.InputSystem;

public class Player2DGroundMover : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] private float _speed = 5f;

    [Header("Animation Config")]
    [SerializeField] private Animator _animator;

    [Header("Ground Check Config")]
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckRadius = 0.1f;

    // Input
    private InputAction _moveAction;
    private Vector2 _inputMovement;

    // Componenti
    private Rigidbody2D _rb;
    private bool _isGrounded;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_animator == null) _animator = GetComponent<Animator>();

        _moveAction = InputSystem.actions.FindAction("Move");

        // Forza la gravità a 0 per evitare che cada fuori scena
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. Legge l'input
        _inputMovement = _moveAction.ReadValue<Vector2>();

        if (_animator != null)
        {
            // 2. GESTIONE STATO FERMO/CAMMINA
            // Usiamo la magnitudine per dire all'Animator se il cuoco è fermo o si muove
            // RICORDATI: Crea un parametro Float chiamato "Speed" nell'Animator
            _animator.SetFloat("Speed", _inputMovement.magnitude);

            // 3. AGGIORNA DIREZIONE SOLO SE C'E' MOVIMENTO
            if (_inputMovement.magnitude > 0.01f)
            {
                // Aggiorna i valori Move x e Move y nel Blend Tree
                _animator.SetFloat("Move x", _inputMovement.x);
                _animator.SetFloat("Move y", _inputMovement.y);

                // 4. LOGICA DI ROTAZIONE (FLIP)
                // Se vai a sinistra (x negativo), specchia la scala
                if (_inputMovement.x < -0.01f)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                // Se vai a destra (x positivo), rimetti la scala normale
                else if (_inputMovement.x > 0.01f)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
            
        }
    }

    void FixedUpdate()
    {
        // Check se è a terra
        Collider2D groundCollider = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _whatIsGround);
        _isGrounded = groundCollider != null;

        // Applica il movimento fisico
        _rb.linearVelocity = _inputMovement * _speed;
    }

    private void OnDrawGizmos()
    {
        if (_groundChecker != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckRadius);
        }
    }
}