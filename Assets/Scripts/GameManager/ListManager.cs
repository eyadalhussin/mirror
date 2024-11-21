using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Mirror;

public class ListManager : NetworkBehaviour
{
    private GameManagerScript _gameManager;

    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> minions = new List<GameObject>();

    public void InitGameManager(GameManagerScript gameManager)
    {
        _gameManager = gameManager;
    }


    public void AddPlayerToList(GameObject player)
    {
        if(player != null)
        {
            players.Add(player);
        }
    }

    public void AddMinionToList(GameObject minion)
    {
        if(minion != null)
        {
            minions.Add(minion);
        }
    }

    public GameObject GetNextPlayerToPosition(Vector3 minionPosition)
    {
        if(players.Count > 0)
        {
            return players.OrderBy(player => Vector3.Distance(player.transform.position, minionPosition)).First();
        }
        return null;
    }

    public GameObject GetNextMinionToPosition(Vector3 pos)
    {
        GameObject nextMinion = null;
        if(minions.Count > 0)
        {
            //return minions.OrderBy(minion => Vector3.Distance(minion.transform.position, pos)).First();
            float distance = 1000f;
            minions.ForEach(minion => { 
                if(minion != null)
                {
                    float nextDistance = Vector3.Distance(pos, minion.transform.position);
                    if (nextDistance < distance)
                    {
                        distance = nextDistance;
                        nextMinion = minion;
                    }
                }
            });
        }
        return nextMinion;
    }

    public List<GameObject> GetPlayersList()
    {
        return players;
    }
    

    public int GetPlayersCount()
    {
        return players.Count;
    }

    public void AddMinion(GameObject minion)
    {
        minions.Add(minion);
    }

    public void RemoveMinionFromList(GameObject minion)
    {
        if (minion)
        {
            int minionID = minion.GetComponent<MinionCore>().GetID();

            //int minionIndex = minions.FindIndex(minion => minion.GetComponent<MinionCore>().GetID() == minionID);
            minions.Remove(minion);

            //minions.RemoveAt(minionIndex);
            NetworkServer.Destroy(minion);
        }
    }

    // Removing a player from the list will only accures when the player disconnects 
    // When a player is removed, we should also remove all his minions and destroy them
    public void RemovePlayerFromList(GameObject player)
    {
        if (player)
        {
            players.Remove(player);

            PlayerCore playerCore = player.GetComponent<PlayerCore>();
            if (playerCore)
            {
                // Destroy all the minions of the player object
                playerCore.GetMinions().ForEach(minion => { NetworkServer.Destroy(minion); });
            }
            Debug.Log("Removed Player Successfully !");
        }
    }

    public GameObject GetPlayerWithID(int id)
    {
       if(players.Count > 0)
        {
            GameObject player = players.Find(player => player.GetComponent<PlayerCore>().GetID() == id);
            return player;
        }
        return null;
    }

    public void DestroyAllPlayerMinions(GameObject player)
    {
        if (player)
        {
            PlayerCore playerCore = player.GetComponent<PlayerCore>();
            if (playerCore)
            {
                playerCore.GetMinions().ForEach(minion => {
                    _gameManager.SpawnExplosion(minion.transform.position);
                    //Destroy(minion);
                    NetworkServer.Destroy(minion);
                });
                playerCore.EmptyMinionsList();
            }
        }
    }

    public int GetMinionsCount()
    {
        return minions.Count;
    }

}
