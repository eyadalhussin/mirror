using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

enum PlayerType
{
    player,
    ai
}

public class PlayerCore : NetworkBehaviour
{
    private int ID;
    private List<GameObject> minionsList = new List<GameObject>();
    PlayerType playerType;

    //Life
    [SyncVar]
    private int _lifes = 0;
    [SyncVar]
    private bool _isAlive = true;

    //Movement
    public float _moveSpeed = 15f;

    //Attack
    public Transform _projectileSpawnPos;
    private float _visionRange = 10f;
    private float _attackInterval = 0.2f;
    private float _lastAttackTime;

    //private GameObject nextTarget = null;

    [SyncVar(hook = nameof(UpdateRendererOnPlayercolorChange))]
    public Color _playerColor;

    private Renderer _objectRenderer;

    void Start()
    {
        if (isLocalPlayer)
        {
            _lastAttackTime -= _attackInterval;
            CmdChangePlayerColor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerType == PlayerType.ai)
        {
            AIBehaviour();
        }
    }

    private void UpdateRendererOnPlayercolorChange(Color oldColor, Color newColor)
    {
        //UpdatePlayerColorOnServer(newColor);
        //UpdatePlayerColorOnClient(newColor);
       Renderer rend = transform.Find("Body").GetComponent<Renderer>();
       if (rend)
       {
         rend.material.color = newColor;
       }
    }

    [Server]
    private void UpdatePlayerColorOnServer(Color newColor)
    {
        if(SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
        {
            Renderer rend = transform.Find("Body").GetComponent<Renderer>();
            if (rend)
            {
                rend.material.color = newColor;
            }
        }
    }

    [Client]
    private void UpdatePlayerColorOnClient(Color newColor)
    {
        Renderer rend = transform.Find("Body").GetComponent<Renderer>();
        if (rend)
        {
            rend.material.color = newColor;
        }
    } 

    [Command]
    public void CmdChangePlayerColor()
    {
        Color newColor = GameManagerScript.Instance.GetRandomColor();
        Renderer rend = transform.Find("Body").GetComponent<Renderer>();
        if (rend)
        {
            rend.material.color = newColor;
            _playerColor = newColor; // SyncVar update triggers OnColorChange on all clients
        }
    }

    public Color GetPlayerColor()
    {
        return _playerColor;
    }

    private void AIBehaviour()
    {
        GameObject nextMinion = GameManagerScript.Instance.GetNextMinion(transform.position);
        GameObject nextPlayer = GameManagerScript.Instance.GetNextPlayer(transform.position, ID);

        float distanceToMinion = 1000f;
        float distanceToPlayer = 1000f;

        if (nextMinion != null) distanceToMinion = Vector3.Distance(nextMinion.transform.position, transform.position);
        if (nextPlayer != null) distanceToPlayer = Vector3.Distance(nextPlayer.transform.position, transform.position);

        GameObject target = null;

        if (distanceToMinion < distanceToPlayer) target = nextMinion;
        if (distanceToMinion > distanceToPlayer) target = nextPlayer;

        Debug.DrawLine(transform.position, target.transform.position, Color.green);

        float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
        if (distanceToTarget > _visionRange)
        {
            MoveToTarget(target);
        }
        if (distanceToTarget <= _visionRange)
        {
            Attack(target);
        }
    }

    private void MoveToTarget(GameObject target)
    {
        transform.LookAt(target.transform);
        // Calculate the direction vector from the minion to the target.
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        // Calculate the target position.
        Vector3 targetPosition = transform.position + directionToTarget * _moveSpeed * Time.deltaTime;

        // Move the player towards the target position.
        transform.position = targetPosition;
    }


    private void Attack(GameObject nextTarget)
    {
        if (nextTarget != null)
        {
            if (Time.time - _lastAttackTime >= _attackInterval)
            {
                transform.LookAt(nextTarget.transform);
                GameObject projectilePrefab = GameManagerScript.Instance.GetProjectile1();
                //Instantitate the projectile
                GameObject bullet = Instantiate(projectilePrefab, _projectileSpawnPos.position, Quaternion.identity);
                //Change the color of the projectile to match the shooter

                Material shooterMat = transform.Find("Body").GetComponent<Renderer>().material;
                bullet.GetComponent<Renderer>().material = shooterMat;
                //Adjust the forward position
                ProjectileCore projectileCoreComponent = bullet.GetComponent<ProjectileCore>();
                projectileCoreComponent.SetDamageDealer(gameObject);
                projectileCoreComponent.SetForward(transform);
                _lastAttackTime = Time.time; // Set the last attack time to the current time
            }
        }
    }

    public void EmptyMinionsList()
    {
        this.minionsList = new List<GameObject>();
    }

    public void SetID(int idNumber)
    {
        ID = idNumber;
    }

    public int GetID()
    {
        return ID;
    }

    public List<GameObject> GetMinions()
    {
        return minionsList;
    }

    public void SetTypeAI()
    {
        playerType = PlayerType.ai;
    }

    public int GetPlayerLifes()
    {
        return this._lifes;
    }

    public void SetPlayerLifes(int newLifes)
    {
        this._lifes = newLifes;
    }

    public bool IsAlive()
    {
        return this._isAlive;
    }

    public void SetAlive(bool value)
    {
        this._isAlive = value;
    }
    

}
