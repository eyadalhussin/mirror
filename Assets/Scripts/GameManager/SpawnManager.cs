using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class SpawnManager : NetworkBehaviour
{
    private GameManagerScript _gameManager;

    //Object Prefabs
    public GameObject minionPrefab;

    //Expolisions
    public GameObject _explosionPrefab;
    public GameObject _minionAttackPrefab;
    public GameObject _playerExplosionPrefab;

    private bool _gameStarted = false;


    // Logic for Spawning the minions
    private const int totalMinions = 100;
    private const int groupSize = 5;
    private const int gridSize = 5; // 5x5 grid for 20 groups
    private const float gameboardSize = 180f; // From -90 to 90

    private void Update()
    {
        int minionsCount = _gameManager.GetMinionsCount();
        if (_gameStarted && minionsCount < 5)
        {
            InitMinions();
        }
    }

    public void InitGameManager(GameManagerScript gameManager)
    {
        _gameManager = gameManager;
    }

    public void SetGameStarted()
    {
        _gameStarted = true;
    }

    public void SpawnEnemyMinion(Vector3 spawnPosition)
    {
        if (!isServer) return;
        if (minionPrefab != null)
        {
            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            // MinionType is important for the collision-handling of the projectiles
            minion.tag = "Minion";
            minion.GetComponent<MinionCore>().SetMinionType(MinionType.Minion);
            minion.AddComponent<MinionEnemyAI>();
            _gameManager.AddMinionToList(minion);
            NetworkServer.Spawn(minion);
        }
    }

    public void SpawnExplosion(Vector3 explosionPosition)
    {
        if (!isServer) return;
        if (_explosionPrefab != null)
        {
            GameObject explosion = Instantiate(_explosionPrefab, explosionPosition, Quaternion.identity);
            NetworkServer.Spawn(explosion);
        }
    }

    public void SpawnMinionAttack(Vector3 pos)
    {
        if (!isServer) return;
        if (_minionAttackPrefab != null)
        {
            GameObject minionAttack = Instantiate(_minionAttackPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(minionAttack);
        }
    }

    public void SpawnPlayerExpolision(Vector3 pos)
    {
        if (!isServer) return;
        if (_playerExplosionPrefab != null)
        {
            GameObject playerExplosion = Instantiate(_playerExplosionPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(playerExplosion);
        }
    }

    public void SpawnNewPlayerMinion(GameObject player)
    {
        if (!isServer) return;
        if (player)
        {
            //Instantitate a new playerMinion
            float randomOffset = Random.Range(4f, 6f);
            Vector3 instancePosition = player.transform.position;
            GameObject newPlayerMinion = Instantiate(minionPrefab, instancePosition, Quaternion.identity);

            //Set the Type of and ID of the Minion
            int randomID = Random.Range(100, 999);
            newPlayerMinion.GetComponent<MinionCore>().SetMinionType(MinionType.PlayerMinion);
            newPlayerMinion.GetComponent<MinionCore>().SetID(randomID);

            //Set The color of the minion to match the player
            newPlayerMinion.AddComponent<MinionPlayerAI>();
            //Setting the controlling player will update the color of the minion to correspond the the color of the player in the component MinionPlayerAI
            newPlayerMinion.GetComponent<MinionPlayerAI>().SetControllingPlayer(player);

            //Add the newPlayerMinion to the minion-list of the player
            player.GetComponent<PlayerCore>().GetMinions().Add(newPlayerMinion);

            NetworkServer.Spawn(newPlayerMinion);
        }
    }

    public void InitMinions()
    {
        SpawnMinions(5, new Vector3(-75f, 2.7f, 75f));
        SpawnMinions(5, new Vector3(0f, 2.7f, 75f));
        SpawnMinions(5, new Vector3(0f, 2.7f, 40f));
        SpawnMinions(5, new Vector3(-55f, 2.7f, 40f));
        SpawnMinions(5, new Vector3(55f, 2.7f, 8f));
        SpawnMinions(5, new Vector3(55f, 2.7f, -23f));
        SpawnMinions(5, new Vector3(-80f, 2.7f, 20f));
        SpawnMinions(5, new Vector3(13f, 2.7f, -23f));
        SpawnMinions(5, new Vector3(66f, 2.7f, 23f));
        SpawnMinions(5, new Vector3(66f, 2.7f, -44f));
        SpawnMinions(5, new Vector3(-15f, 2.7f, -70f));
        SpawnMinions(5, new Vector3(25f, 2.7f, -70f));
        SpawnMinions(5, new Vector3(-75f, 2.7f, -70f));
        SpawnMinions(5, new Vector3(-60f, 2.7f, -87f));
        SpawnMinions(5, new Vector3(-60f, 2.7f, -20f));
    }

    public void SpawnMinions(int count, Vector3 pos)
    {
        float distance = 2f;
        SpawnEnemyMinion(new Vector3(pos.x, pos.y, pos.z));
        SpawnEnemyMinion(new Vector3(pos.x + distance, pos.y, pos.z));
        SpawnEnemyMinion(new Vector3(pos.x - distance, pos.y, pos.z));
        SpawnEnemyMinion(new Vector3(pos.x, pos.y, pos.z + distance));
        SpawnEnemyMinion(new Vector3(pos.x, pos.y, pos.z - distance));
    }


    public void InitMinionsAI()
    {
        if (!isServer) return;

        Debug.Log("Initializing Minions");

        for (int i = 0; i < totalMinions / groupSize; i++)
        {
            // Calculate sector position
            int gridX = i % gridSize;
            int gridZ = i / gridSize;
            float sectorWidth = gameboardSize / gridSize;
            float sectorX = -90 + gridX * sectorWidth;
            float sectorZ = -90 + gridZ * sectorWidth;

            // Random position within sector for the group
            Vector3 groupCenter = new Vector3(
                Random.Range(sectorX, sectorX + sectorWidth),
                2.7f, // Fixed Y value as per your original code
                Random.Range(sectorZ, sectorZ + sectorWidth)
            );

            // Spawn a group of minions around the central point
            for (int j = 0; j < groupSize; j++)
            {
                Vector3 spawnPosition = groupCenter + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)); // Random offset for grouping
                SpawnEnemyMinion(spawnPosition);
            }
        }
    }
}
