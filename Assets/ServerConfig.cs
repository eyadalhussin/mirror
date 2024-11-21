using Mirror;
using Mirror.SimpleWeb;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerConfig : MonoBehaviour
{
    bool isServer = false;
    int port = 8000;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            // Handling -port Argument
            if (args[i].ToLower() == "-port" && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out int parsedPort))
                {
                    port = parsedPort;
                    Debug.Log($"Setting port to {port}");
                }
                else
                {
                    Debug.LogError("Invalid port argument");
                }
            }

            // Handling -server Argument
            if (args[i].ToLower() == "-server")
            {
                isServer = true;
                DebugManager.Log("Starting as Server");
                NetworkManager manager = GetComponent<RoomManagerMR>();
                manager.GetComponent<SimpleWebTransport>().Port = (ushort)port;
                manager.StartServer();
            }
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DebugManager.Log("Loaded Scene :" + scene.name);
        if (scene.name == "Room")
        {
            NetworkManager manager = GetComponent<RoomManagerMR>();
            manager.GetComponent<SimpleWebTransport>().Port = (ushort)port;
            if(isServer)
            {
                DebugManager.Log("Starting as Server");
                manager.StartServer();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
