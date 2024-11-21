using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float _moveSpeed = 30.0f;


    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Get input from the player.
        float _horizontalInput = Input.GetAxis("Horizontal");
        float _verticalInput = Input.GetAxis("Vertical");

        //Beschleunigen
        _moveSpeed = Input.GetKey("space") ? 90f : 30f;


        // Calculate the movement direction based on input.
        Vector3 _moveDirection = new Vector3(_horizontalInput, 0, _verticalInput).normalized;


        if (_moveDirection != Vector3.zero)
        {
            _rigidbody.velocity = _moveDirection * _moveSpeed;
        }
    }

}
