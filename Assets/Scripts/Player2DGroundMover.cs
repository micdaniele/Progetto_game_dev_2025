using UnityEngine;
using UnityEngine.InputSystem;

public class Player2DGroundMover : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] private float _speed = 5f;

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
        _moveAction = InputSystem.actions.FindAction("Move");

        // Configurazione Rigidbody2D per movimento libero 2D
        _rb.gravityScale = 0f; // Nessuna gravità per movimento libero
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Blocca la rotazione
    }

    void Update()
    {
        // Leggi input in entrambe le direzioni (WASD o Frecce)
        _inputMovement = _moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Check se è a terra
        Collider2D groundCollider = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _whatIsGround);
        _isGrounded = groundCollider != null;

        // Movimento in tutte le direzioni (WASD)
        // W/S = su/giù, A/D = sinistra/destra
        Vector2 velocity = _inputMovement * _speed;
        _rb.linearVelocity = velocity;
    }

    // Visualizza il ground checker nell'editor
    private void OnDrawGizmos()
    {
        if (_groundChecker != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckRadius);
        }
    }
}