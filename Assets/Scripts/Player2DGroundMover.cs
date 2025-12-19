using UnityEngine;
using UnityEngine.InputSystem;

public class Player2DGroundMover : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] private float _speed = 5f;

    [Header("Animation Config")]
    [SerializeField] private Animator _animator; // Questa riga serve per vedere la casella nell'Inspector

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

        // Se non hai trascinato l'animator, lo cerca da solo
        if (_animator == null) _animator = GetComponent<Animator>();

        _moveAction = InputSystem.actions.FindAction("Move");

        // Configurazione Rigidbody2D
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. Legge l'input (WASD o Frecce)
        _inputMovement = _moveAction.ReadValue<Vector2>();

        // 2. Aggiorna l'Animator solo se il cuoco si sta muovendo
        if (_animator != null)
        {
            // Usiamo una piccola soglia (0.1) per capire se stiamo premendo i tasti
            if (_inputMovement.magnitude > 0.1f)
            {
                _animator.SetFloat("Move x", _inputMovement.x);
                _animator.SetFloat("Move y", _inputMovement.y);

                // LOGICA PER GIRARE IL CUOCO (FLIP)
                // Se vai a destra (x positivo), scala normale
                if (_inputMovement.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                // Se vai a sinistra (x negativo), scala invertita
                else if (_inputMovement.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            
        }
    }

    void FixedUpdate()
    {
        // Check se è a terra
        Collider2D groundCollider = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _whatIsGround);
        _isGrounded = groundCollider != null;

        // Movimento
        Vector2 velocity = _inputMovement * _speed;
        _rb.linearVelocity = velocity;
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