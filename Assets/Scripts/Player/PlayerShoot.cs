using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    public GameObject _projectilePrefab;
    public Transform _shootingPos;

    private float _lastAttack = 0f;
    public float _attackInterval = 0.2f;
    void Update()
    {
        if (!isLocalPlayer) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Calculate the direction from the player to the mouse position
            Vector3 targetPosition = hit.point;
            Vector3 playerPositionOnPlane = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 shootDirection = (targetPosition - playerPositionOnPlane).normalized;

            // Set the y-component of the direction to zero
            shootDirection.y = 0f;

            // Calculate the rotation that faces the shooting direction
            Quaternion targetRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

            // Limit the rotation to the y-axis
            targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

            transform.rotation = targetRotation;

            if (Input.GetButton("Fire1"))
            {
                CmdShoot();
            }

        }
    }

    [Command]
    void CmdShoot()
    {
        if (Time.time - _lastAttack >= _attackInterval)
        {
            if (_projectilePrefab != null)
            {
                GameObject bullet = Instantiate(_projectilePrefab, _shootingPos.position, Quaternion.identity);
                NetworkServer.Spawn(bullet);
                ProjectileCore projectileCoreComponent = bullet.GetComponent<ProjectileCore>();
                projectileCoreComponent.SetDamageDealer(gameObject);
                projectileCoreComponent.SetForward(transform);
                _lastAttack = Time.time;
            }
        }
    }
}