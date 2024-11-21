using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionPlayerAI : MonoBehaviour
{
    //Range
    public float _playerRange = 12f;
    public float _visionRange = 24f;
    public float _attackRange = 3f;

    //Attack
    private float _lastAttackTime = 0f;
    private float _attackCooldown = 1f;
    public float _damage = 10f;


    //Movement
    public float _moveSpeed = 30f;
    private Rigidbody _rb;

    private GameObject controllingPlayer;

    private void Start()
    {

        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        StartAI();
    }


    private void StartAI()
    {
        int controllingPlayerID = controllingPlayer.GetComponent<PlayerCore>().GetID();
        float distanceToControllingPlayer = Vector3.Distance(transform.position, controllingPlayer.transform.position);

        if(distanceToControllingPlayer > 20f)
        {
            MoveToPlayer();
            return;
        }

        GameObject nextPlayer = GameManagerScript.Instance.GetNextPlayer(controllingPlayer.transform.position, controllingPlayerID);
        GameObject nextMinion = GameManagerScript.Instance.GetNextMinion(controllingPlayer.transform.position);

        GameObject nextTarget = null;
        bool nextTargetIsPlayer = false;

        float distanceToTargetPlayer = nextPlayer != null ? Vector3.Distance(controllingPlayer.transform.position, nextPlayer.transform.position) : 1000f;
        float distanceToTargetMinion = nextMinion != null ? Vector3.Distance(controllingPlayer.transform.position, nextMinion.transform.position) : 1000f;

        // Decide on the next target based on which is closer
        if (distanceToTargetPlayer < distanceToTargetMinion)
        {
            nextTarget = nextPlayer;
            nextTargetIsPlayer = true;
        }
        else if (nextMinion != null)
        {
            nextTarget = nextMinion;
        }

        // If there's a target selected and within 20 units, attack it
        if (nextTarget != null && Vector3.Distance(controllingPlayer.transform.position, nextTarget.transform.position) <= 20f)
        {
            if (nextTargetIsPlayer)
            {
                List<GameObject> playerMinions = nextTarget.GetComponent<PlayerCore>().GetMinions();
                // If the target player has minions
                if (playerMinions.Count > 0)
                {
                    // Get a random minion to attack
                    System.Random random = new System.Random();
                    int randomIndex = random.Next(playerMinions.Count);
                    GameObject minionToAttack = playerMinions[randomIndex];
                    AttackTarget(minionToAttack);
                }
                // If the target player has no minions
                else
                {
                    AttackTarget(nextTarget);
                }
            }
            else
            {
                AttackTarget(nextTarget);
            }
        }
        // If there's no target within 20 units, stay within 5 units of the controlling player
        else if (distanceToControllingPlayer > 5f)
        {
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {

        float randomOffsetX = Random.Range(-10f, 10f);
        float randomOffsetZ = Random.Range(-10f, 10f);

        Vector3 randomOffset = new Vector3(randomOffsetX, 0f, randomOffsetZ);

        Vector3 targetPosition = controllingPlayer.transform.position + randomOffset;

        // Calculate the velocity needed to reach the target position.
        Vector3 velocity = (controllingPlayer.transform.position - transform.position).normalized * _moveSpeed;

        // Apply the velocity to the rigidbody.
        Rigidbody rb = GetComponent<Rigidbody>();
        // Check if the minion has reached the target position.
        if (Vector3.Distance(transform.position, controllingPlayer.transform.position) < 0.1f)
        {
            // If the distance is small, stop the minion.
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = velocity;
        }
    }

    void AttackTarget(GameObject target)
    {
        if (target != null)
        {
            Debug.DrawLine(transform.position, target.transform.position, Color.red);
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            float _modifiedAttackCooldown = _attackCooldown * Random.Range(0.6f, 1f);
            if (distanceToTarget <= _visionRange)
            {
                if (distanceToTarget <= _attackRange)
                {
                    //Debug.DrawLine(transform.position, target.transform.position, Color.green);
                    if (Time.time - _lastAttackTime >= _modifiedAttackCooldown)
                    {
                        Vector3 midPosition = (transform.position + target.transform.position) / 2;
                        GameManagerScript.Instance.SpawnMinionAttack(midPosition);
                        //LifeComponent playerLife = GameManagerScript.Instance.GetPlayer().GetComponent<LifeComponent>();
                        //playerLife.TakeDamage(1, gameObject);
                        LifeComponent targetLife = target.GetComponent<LifeComponent>();
                        if (targetLife != null) targetLife.TakeDamage(10, gameObject);
                        _lastAttackTime = Time.time;
                    }
                }
                else
                {
                    MoveToTarget(target);
                }
            }
        }
    }

    void MoveToTarget(GameObject target)
    {
        // Calculate the direction vector from the minion to the target.
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        // Calculate a random offset within the specified range.
        float randomOffset = 0f;

        // Calculate the target position with the random offset.
        Vector3 targetPosition = target.transform.position - directionToTarget * randomOffset;

        // Calculate the velocity needed to reach the target position.
        Vector3 velocity = (targetPosition - transform.position).normalized * _moveSpeed;

        // Apply the velocity to the rigidbody.
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity;
        // Check if the minion has reached the target position.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // If the distance is small, stop the minion.
            rb.velocity = Vector3.zero;
        }

    }

    public GameObject GetControllingPlayer()
    {
            return controllingPlayer;
    }

    /*Set the controlling Player and change the color of the minion*/
    public void SetControllingPlayer(GameObject player)
    {
        if (player)
        {
            controllingPlayer = player;
            Color playerColor = player.transform.Find("Body").GetComponent<Renderer>().material.color;
            //gameObject.GetComponent<MinionCore>().SetMinionColor(playerColor);
        }
    }

    public Color GetControllingPlayerColor()
    {
        return controllingPlayer.GetComponent<PlayerCore>()._playerColor;
    }
}
