using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover2D : MonoBehaviour
{
    [SerializeField] private float _speed = 4;

    private InputAction _moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = _moveAction.ReadValue<Vector2>(); // <- legge il valore in input di questa Action
        Debug.Log(dir);
        Vector3 dir3D = dir; // <- cast implicito da Vector2 a Vector3
        transform.position = transform.position + dir3D * _speed * Time.deltaTime;
    }
}
