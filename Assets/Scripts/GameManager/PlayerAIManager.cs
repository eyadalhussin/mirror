using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAIManager : NetworkBehaviour
{
    private GameManagerScript _gameManager;

    public PlayerAIManager(GameManagerScript gameManager)
    {
        _gameManager = gameManager;
    }

    //Instantitating
    public void InitAI()
    {
        /*
        if (_gameManager.playerAIPrefab != null)
        {
            _gameManager.players.Add(Instantiate(playerAIPrefab, new Vector3(20f, 3.25f, 20f), Quaternion.identity));
            _gameManager.players.Add(Instantiate(playerAIPrefab, new Vector3(1f, 3.25f, 1f), Quaternion.identity));
            _gameManager.players.Add(Instantiate(playerAIPrefab, new Vector3(30f, 3.25f, 30f), Quaternion.identity));
        }
        _gameManager.players.ForEach(player => {
            int id = Random.Range(1000, 9999);
            PlayerObj playerObj = player.GetComponent<PlayerObj>();

            playerObj.SetID(id);
            playerObj.SetTypeAI();

        });
        */
    }
}
