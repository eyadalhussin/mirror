using UnityEngine;
using Mirror;
using Mirror.SimpleWeb;

public class RoomManagerMR : NetworkRoomManager
{
    public static RoomManagerMR Instance;
    public bool ServerAutoStart;
    public GameObject lobbyManagerPrefab;
    
    public override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public override void Start()
    {
        if (ServerAutoStart)
        {
            //StartServer();
        }
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        float randomX = Random.Range(-5f, 40f);
        float randomZ = Random.Range(-20f, 20f);
        GameObject gamePlayer = Instantiate(playerPrefab, new Vector3(randomX, 3.5f, randomZ), Quaternion.identity);
        Debug.Log("Successfully created a player from custom OnRoomServerCreateGamePlayer method");
        Debug.Log("Adding the player to the GameManagerScript");
        GameManagerScript.Instance.AddPlayer(gamePlayer);
        return gamePlayer;
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (sceneName == "Assets/Scenes/Room.unity")
        {
            GameObject lobbyManager = Instantiate(lobbyManagerPrefab);
            NetworkServer.Spawn(lobbyManager);
        }
        if (sceneName == "Assets/Scenes/Game.unity")
        {
            Debug.Log("Network Manager: Scene Changed to Game, Calling the init Method from the GameManagerScript");
            GameManagerScript.Instance.InitGameManager();
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn != null && conn.identity.gameObject != null && GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.RemovePlayerFromList(conn.identity.gameObject);
        }

        base.OnServerDisconnect(conn);
        CheckIfNoPlayersLeft();
    }

    private void CheckIfNoPlayersLeft()
    {
        DebugManager.Log("Player Disconnected, remaining players: " + NetworkServer.connections.Count);

        if (NetworkServer.active && NetworkServer.connections.Count == 0)
        {
            ushort port = GetComponent<SimpleWebTransport>().port;
            NetworkServer.Shutdown();
            HTTPHandler.instance.ShutdownServer(port);
        }
    }
}
