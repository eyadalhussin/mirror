using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionEnemyAI : MonoBehaviour
{
    //Range and Speed
    public float _visionRange = 10f;
    public float _attackRange = 2f;
    public float _moveSpeed = 10f;
    public float _damage = 5f;
    public Vector3 _spawnPosition;

    //Attack
    private float _lastAttackTime = 0f;
    private float _attackCooldown = 1f;

    //Rigidobdy
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _spawnPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        StartAI();
    }

    private GameObject GetNextTarget()
    {
        return GameManagerScript.Instance.GetNextPlayerToPosition(transform.position);
    }



void MoveToPosition(Vector3 pos)
    {
        //Vector3 _targetPosition = Vector3.MoveTowards(transform.position, pos, _moveSpeed);

        // Calculate the direction vector from the minion to the target.
        Vector3 directionToTarget = (pos - transform.position).normalized;

        // Calculate the velocity needed to reach the target position.
        Vector3 velocity = (pos - transform.position).normalized * _moveSpeed;

        _rigidbody.velocity = velocity;


        //_rigidbody.MovePosition(_targetPosition);
    }

    void StartAI()
    {
        /*At Start, Always check if the minion is too far from its spawn position so it wont follow the player everywhere*/
        float _distanceToSpawnPosition = Vector3.Distance(transform.position, _spawnPosition);
        if (_distanceToSpawnPosition > _visionRange)
        {
            MoveToPosition(_spawnPosition);
            return;
        }

        GameObject _nextTarget = GetNextTarget();

        if (_nextTarget != null)
        {
            float _distanceToTarget = Vector3.Distance(transform.position, _nextTarget.transform.position);

            bool targetInVisionRange = _distanceToTarget <= _visionRange;
            bool targetInAttackRange = _distanceToTarget <= _attackRange;
            
            //Target in Attack- and Visionrange -> Attack Target
            if(targetInAttackRange && targetInVisionRange)
            {
                AttackTarget(_nextTarget);
            }
            //Target NOT in Attackrange but in Visionrange -> Move towards the target
            else if(!targetInAttackRange && targetInVisionRange)
            {
                MoveToPosition(_nextTarget.transform.position);
            }
            //Target NOT in Attackrange and NOT in Visionrange -> Go back to Spawn Position
            else if (!targetInAttackRange && !targetInVisionRange)
            {
                MoveToPosition(_spawnPosition);
            }
        }
    }

    void AttackTarget(GameObject target)
    {
        if(target != null)
        {
            Debug.DrawLine(transform.position, target.transform.position, Color.red);

            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                //Calculate the Middleposition to spawn the Attack animation
                Vector3 midPosition = (transform.position + target.transform.position) / 2;
                GameManagerScript.Instance.SpawnMinionAttack(midPosition);

                //Attack the target
                LifeComponent targetLife = target.GetComponent<LifeComponent>();
                if (targetLife != null) targetLife.TakeDamage(5, gameObject);

                //Reset Attack time
                _lastAttackTime = Time.time;
            }
        }
    }
}
