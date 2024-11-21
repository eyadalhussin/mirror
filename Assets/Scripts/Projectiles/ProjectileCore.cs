using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCore : NetworkBehaviour
{
    public float moveSpeed = 20.0f;
    public float life = 2.0f;
    public float _spawnTime;

    public float _damage = 100f;

    private Vector3 targetPosition;
    private Transform parent;

    private bool forceApplied = false;

    private GameObject damageDealer;

    [SyncVar(hook = nameof(OnChangecolor))]
    Color projectileColor = Color.black;

    private void Awake()
    {
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.velocity = transform.forward * moveSpeed;
        Move();
    }

    void Start()
    {
        ChangeColor();
        _spawnTime = Time.deltaTime;
    }

    void Update()
    {
        Move();
    }

    private void DestroyIfLifeExceeded()
    {
        if(_spawnTime - Time.deltaTime > life)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void Move()
    {
        if (parent != null && !forceApplied)
        {
            Vector3 direction = parent.transform.forward;
            direction.y = 0;
            transform.forward = direction.normalized;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * moveSpeed;
            forceApplied = true;
        }
    }

    public void SetDamageDealer(GameObject obj)
    {
        damageDealer = obj;
    }

    public void SetForward(Transform shooter)
    {
        parent = shooter;
    }

    [Server]
    private void ChangeColor()
    {
        if(damageDealer != null)
        {
            PlayerCore PlayerCore = damageDealer.GetComponent<PlayerCore>();
            if(PlayerCore)
            {
                Color newColor = PlayerCore.GetPlayerColor();
                projectileColor = newColor;
            }
        }
    }

    private void OnChangecolor(Color oldColor, Color newColor)
    {
        UpdateProjectileColorOnClients(newColor);
    }

    [Client]
    private void UpdateProjectileColorOnClients(Color newColor)
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend)
        {
            rend.material.color = newColor;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")){
            GameManagerScript.Instance.SpawnMinionAttack(transform.position);
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            if(other.gameObject != damageDealer)
            {
                dealDamage(other.gameObject, (_damage/3f));
                GameManagerScript.Instance.SpawnMinionAttack(transform.position);
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Minion"))
        {
            LifeComponent minionLife = other.GetComponent<LifeComponent>();
            MinionCore minionAi = other.GetComponent<MinionCore>();
            //Normal Minion
            if(minionAi.GetMinionType() == MinionType.Minion)
            {
                minionLife.TakeDamage(_damage, damageDealer);
                GameManagerScript.Instance.SpawnMinionAttack(transform.position);
                Destroy(gameObject);
            }

            //PlayerMinion
            if (minionAi.GetMinionType() == MinionType.PlayerMinion)
            {
                MinionPlayerAI playerMinionAI = other.GetComponent<MinionPlayerAI>();
                //Get the ID who shooted the projectile
                int damageDealerID = damageDealer.GetComponent<PlayerCore>().GetID();

                //Get the ID of the controlling player
                int controllingPlayerID = playerMinionAI.GetControllingPlayer().GetComponent<PlayerCore>().GetID();

                //if the ID's does not match, another player should have attacked the minion
                if (damageDealerID != controllingPlayerID)
                {
                    minionLife.TakeDamage(_damage, damageDealer);
                    GameManagerScript.Instance.SpawnMinionAttack(transform.position);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void dealDamage(GameObject other, float damage)
    {
        LifeComponent otherLife = other.GetComponent<LifeComponent>();
        if (otherLife)
        {
            otherLife.TakeDamage(damage, gameObject);
        }
    }
}
