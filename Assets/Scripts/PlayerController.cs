using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _turnSpeed = 360;

    private Vector3 _input;

    public bool IsMoving = false;

    private void Update()
    {
        GatherInput();
        LookAt();        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void LookAt()
    {
        if (_input != Vector3.zero)
        {
            var direction = (transform.position + _input) - transform.position;
            var rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _turnSpeed * Time.deltaTime);

            IsMoving = true;            
        }
        else
        {
            IsMoving = false;            
        }
    }
    private void Move()
    {
        _rb.MovePosition(transform.position + transform.forward *_input.magnitude * _speed * Time.deltaTime);
    }
}
