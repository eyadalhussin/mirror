using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;


public class GameManagerScript : NetworkBehaviour
{
    public static GameManagerScript Instance;
    //Managers
    private SpawnManager _spawnManager;
    private PlayerAIManager _playerAIManager;
    private DeathManager _deathManager;
    private ListManager _listManager;

    //Players
    public GameObject playerPrefab;
    public GameObject player;
    public GameObject playerAIPrefab;

    //Minions
    public GameObject playerMinionPrefab;

    //Projectiles
    public GameObject projectile_1;

    //Colors
    List<Color> _playerColors = new List<Color>();

    //Materials
    /*
    private List<Material> materials = new List<Material>();
    public Material mat1;
    public Material mat2;
    public Material mat4;
    public Material mat6;
    public Material mat7;
    public Material mat8;
    */

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            //InitMaterials();
            InitManagers();
            InitColors();
        }
        else
        {
            // Ensure there's only one instance of the GameManager
            Destroy(gameObject);
        }
    }

    private void Update()
    {

    }

    /*
     This Method will replace Start(), since Start will be called at the very beginning in the "Offline" scene,
    only when changing the scene to the "Game", this method should be called*/
    public void InitGameManager()
    {
        _spawnManager.InitMinions();
        _spawnManager.SetGameStarted();
    }


    //Initialize Managers because these are private at the start of the method
    private void InitManagers()
    {
        Debug.Log("Initializing Managers");
        //Spawn Manager
        _spawnManager = GetComponent<SpawnManager>();
        _spawnManager.InitGameManager(this);
        //Death Manager
        _deathManager = GetComponent<DeathManager>();
        _deathManager.InitGameManager(this);
        //List Manager
        _listManager = GetComponent<ListManager>();
        _listManager.InitGameManager(this);
    }

    //AI Manager
    public void InitAI()
    {
        _playerAIManager.InitAI();
    }
    //AI Manager

    //Spawn Manager
    public void SpawnMinion(Vector3 spawnPosition)
    {
        if(_spawnManager != null)
        {
            _spawnManager.SpawnEnemyMinion(spawnPosition);
        }
    }
    public void SpawnExplosion(Vector3 explosionPosition)
    {
        if (_spawnManager != null)
        {
            _spawnManager.SpawnExplosion(explosionPosition);
        }
    }

    public void SpawnPlayerExplosion(Vector3 explosionPosition)
    {
        if (_spawnManager != null)
        {
            _spawnManager.SpawnPlayerExpolision(explosionPosition);
        }
    }

    public void SpawnMinionAttack(Vector3 attackPosition)
    {
        if (_spawnManager != null)
        {
            _spawnManager.SpawnMinionAttack(attackPosition);
        }
    }

    /*
    public void InitMinions()
    {
       if(_spawnManager != null)
        {
            _spawnManager.InitMinions();
        }
    }
    */



    public void SpawnNewPlayerMinion(int killerID)
    {
        GameObject killer = _listManager.GetPlayerWithID(killerID);

        if(_spawnManager != null)
        {
            _spawnManager.SpawnNewPlayerMinion(killer);
        }

    }
    //Spawn Manager

    //Death Manager
    public void OnMinionEnemyDeath(GameObject lastAttacker, GameObject minion)
    {
        _deathManager.OnMinionEnemyDeath(lastAttacker, minion);
    }

    public void OnPlayerDeath(GameObject lastAttacker, GameObject deadplayer)
    {
        _deathManager.OnPlayerDeath(lastAttacker, deadplayer);
    }

    public void OnPlayerMinionDeath(GameObject minion)
    {
        _deathManager.OnPlayerMinionDeath(minion);
    }
    //Death Manager

    //List Manager


    public void RemoveMinionFromList(GameObject minion)
    {
        _listManager.RemoveMinionFromList(minion);
    }

    public void RemovePlayerFromList(GameObject player)
    {
        _listManager.RemovePlayerFromList(player);
    }

    public int GetMinionsCount()
    {
        return _listManager.GetMinionsCount();
    }

    //List Manager

    //TODO : Refactoring
    /*
    public void InitMaterials()
    {
        materials.Add(mat1);
        materials.Add(mat2);
        materials.Add(mat4);
        materials.Add(mat6);
        materials.Add(mat7);
        materials.Add(mat8);
    }
    */

    /*
    public Material SetPlayerMaterial()
    {
        int random = Random.Range(0, materials.Count - 1);
        Material mat = materials[random];
        materials.RemoveAt(random);
        return mat;
    }
    */

    /*
    public Color GetColor()
    {
        int random = Random.Range(0, materials.Count - 1);
        Color color = materials[random].color;
        materials.RemoveAt(random);
        return color;
    }
    */
    public void AddPlayer(GameObject createdPlayer)
    {
        if (!isServer) return;
        if(createdPlayer != null)
        {
            int randomID = Random.Range(100, 999);
            createdPlayer.GetComponent<PlayerCore>().SetID(randomID);
            _listManager.AddPlayerToList(createdPlayer);
            Debug.Log("Successfully created a player");
        }
    }

    public void AddMinionToList(GameObject minion)
    {
        _listManager.AddMinionToList(minion);
    }

    public Transform GetPlayerTransform()
    {
        if (player != null)
        {
            return player.transform;
        }
        return null;

    }

    public GameObject GetProjectile1()
    {
        return projectile_1;
    }

    private GameObject InstatitatePlayer(GameObject playerPrefab, Vector3 position)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        int id = Random.Range(100, 999);
        player.GetComponent<PlayerCore>().SetID(id);
        Debug.Log("Successfully instatitated player with the ID:" + id);
        return player;
    }

    public GameObject GetNextPlayerToPosition(Vector3 minionPosition)
    {
        return _listManager.GetNextPlayerToPosition(minionPosition);
    }

    //Returns the closest normal minion relative to own position
    public GameObject GetNextMinion(Vector3 pos)
    {
        return _listManager.GetNextMinionToPosition(pos);
    }

    public GameObject GetNextPlayer(Vector3 pos, int controllingPlayerID)
    {
        IEnumerable<GameObject> filteredPlayers = _listManager.GetPlayersList().Where(player => player.GetComponent<PlayerCore>().GetID() != controllingPlayerID);
        if(filteredPlayers.Count() >= 1)
        {
            return filteredPlayers.OrderBy(player => Vector3.Distance(pos, player.transform.position)).First();
        }
        return null;
    }

    //Returns the next Target player Minion
    public GameObject GetNextPlayerMinion(Vector3 pos, GameObject playerToAttack)
    {
        List<GameObject> playerMinions = playerToAttack.GetComponent<PlayerCore>().GetMinions();

        if(playerMinions.Count > 0)
        {
            return playerMinions.OrderBy(minion => Vector3.Distance(pos, minion.transform.position)).First();
        }
        return null;
    }

    private void RespawnPlayer(GameObject deadPlayer)
    {
        if (!isServer || deadPlayer == null) return;

        float randomX = Random.Range(50f, 100f);
        float randomZ = Random.Range(50f, 100f);
        
        deadPlayer.SetActive(true);
        deadPlayer.transform.position = new Vector3(randomX, 3.5f, randomZ);
        deadPlayer.GetComponent<LifeComponent>().SetHealth(100f);
        deadPlayer.GetComponent<PlayerCore>().SetAlive(true);
    }

    public void DestroyAllPlayerMinions(GameObject player)
    {
        if (_listManager)
        {
            _listManager.DestroyAllPlayerMinions(player);
        }
    }

    private void CheckGameEnd()
    {
        /*
        //TODO If u set A timer for the players u have to wait also here to check the players!!!
        bool allPlayerDead = true;
        this.players.ForEach(player => {
            if (player.GetComponent<PlayerCore>().IsAlive()) allPlayerDead = false;
        });

        if (allPlayerDead)
        {
            Debug.Log("All Players are, Dead returning to Lobby");
            RoomManagerMR.Instance.ServerChangeScene("Room");
        }
        */
    }


    public Color GetRandomColor()
    {
        /*
        Color[] playerColors = new Color[]
        {
        new Color(0f, 0f, 1f), // Blue
        new Color(1f, 1f, 0f), // Yellow
        new Color(0.5f, 0f, 0.5f), // Purple
        new Color(0f, 1f, 1f), // Cyan
        new Color(1f, 0.5f, 0f), // Orange
        new Color(0f, 0.5f, 0f), // Dark Green
        };
        */
        
        if(_playerColors.Count <= 0)
        {
            InitColors();
        }

        int random = Random.Range(0, _playerColors.Count);
        Color color = _playerColors[random];
        _playerColors.RemoveAt(random);
        return color;
    }

    private void InitColors()
    {
        Color color1 = new Color(0f, 0f, 1f); // Blue
        Color color2 = new Color(1f, 1f, 0f); // Yellow
        Color color3 = new Color(0.5f, 0f, 0.5f); // Purple
        Color color4 = new Color(0f, 1f, 1f); // Cyan
        Color color5 = new Color(1f, 0.5f, 0f); // Orange
        Color color6 = new Color(0f, 0.5f, 0f); // Dark Green
        _playerColors.Add(color1);
        _playerColors.Add(color2);
        _playerColors.Add(color3);
        _playerColors.Add(color4);
        _playerColors.Add(color5);
        _playerColors.Add(color6);
    }


}