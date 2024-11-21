using Mirror;
using Mirror.SimpleWeb;
using System.Collections.Generic;
using UnityEngine;
using static HTTPHandler;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    private LobbyUI _lobbyUI;
    private string masterServerUrl;
    private string gameServerUrl;
    public NetworkManager _networkManager;

    public List<Server> availableServers = new List<Server>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        _lobbyUI = GetComponent<LobbyUI>();
        _networkManager = GetComponent<NetworkManager>();
        masterServerUrl = EnvironmentSetting.masterServerUrl;
        gameServerUrl = EnvironmentSetting.gameServerUrl;
        FetchServers();
    }

    public void CreateServer()
    {
        if (instance == null) DebugManager.Log("LobbyManager is null");
        if (HTTPHandler.instance == null) DebugManager.Log("HTTPHandler is null");
        StartCoroutine(HTTPHandler.instance.HTTPCreateServer(
            onSuccess: (string msg) =>
            {
                DebugManager.Log(msg);
                FetchServers();
            },
            onFailure: (string err) =>
            {
                DebugManager.Log(err);
            }
        ));
    }

    public void FetchServers()
    {
        StartCoroutine(HTTPHandler.instance.HTTPGetAvailableServersList(
            onSuccess: (List<Server> servers) =>
            {
                availableServers = servers;
                _lobbyUI.SetUIStatus("PreConnection");
            },
            onFailure: (string err) =>
            {
                availableServers.Clear();
                _lobbyUI.SetUIStatus("PreConnection");
            }
        ));
    }

    internal void ConnectToGameServer(int port)
    {

        if(EnvironmentSetting.prod && _networkManager != null)
        {
            string networkaddress = $"{port}.{gameServerUrl}";
            _networkManager.networkAddress = networkaddress;
            _networkManager.GetComponent<SimpleWebTransport>().port = (ushort)443;
            _networkManager.GetComponent<SimpleWebTransport>().clientUseWss = true;
            DebugManager.Log($"Connecting as Client to production server {networkaddress} , WSS Enabled");
            _networkManager.StartClient();
        } else if(!EnvironmentSetting.prod && _networkManager != null)
        {
            string networkaddress = "127.0.0.1";
            _networkManager.networkAddress = networkaddress;
            _networkManager.GetComponent<SimpleWebTransport>().port = (ushort)port;
            _networkManager.GetComponent<SimpleWebTransport>().clientUseWss = false;
            DebugManager.Log($"Connecting as Client to development server {networkaddress}:{port} , WSS Disabled");
            _networkManager.StartClient();
        } else
        {
            DebugManager.Log("Cannot connect to the server, NetworkManager might be null");
        }
    }

    internal void ConnectToGameServerWithIP(string testIp, string port)
    {
        int parsedPort;
        int.TryParse(port, out parsedPort);

        if (_networkManager != null)
        {
            DebugManager.Log("Connecting as Client to " + testIp + ":" + parsedPort);
            _networkManager.networkAddress = testIp;
            _networkManager.GetComponent<SimpleWebTransport>().port = (ushort)parsedPort;
            _networkManager.StartClient();
        }
         else
        {
            DebugManager.Log("Cannot connect to the server, NetworkManager might be null");
        }
    }

}


