using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPHandler : MonoBehaviour
{
    [System.Serializable]
    public class Server
    {
        public int port;
        public string status;
    }

    [System.Serializable]
    public class ServerList
    {
        public Server[] servers;
    }

    public static HTTPHandler instance;
    private LobbyUI LobbyUI;
    private string masterServerUrl;
    private int masterServerPort;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        masterServerUrl = EnvironmentSetting.masterServerUrl;
        masterServerPort = EnvironmentSetting.masterServerPort;
    }

    public IEnumerator HTTPGetAvailableServersList(Action<List<Server>> onSuccess, Action<string> onFailure)
    {
        string url = $"{masterServerUrl}:{masterServerPort}/servers?serverType=Mirror";
        DebugManager.Log($"Sending request to {url}");

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ServerList serverList = JsonUtility.FromJson<ServerList>(request.downloadHandler.text);
            if (serverList != null && serverList.servers != null && serverList.servers.Length > 0)
            {
                onSuccess?.Invoke(new List<Server>(serverList.servers));
            }
            else
            {
                onFailure?.Invoke("No servers are currently available.");
            }
        }
        else
        {
            Debug.Log("Failed to get server list: " + request.error);
        }
    }

    public IEnumerator HTTPCreateServer(Action<string> onSuccess, Action<string> onFailure)
    {
        string url = $"{masterServerUrl}:{masterServerPort}/start-server";
        string serverType = "{\"serverType\" : \"Mirror\" }";
        DebugManager.Log($"Sending request to {url}");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] data = new System.Text.UTF8Encoding().GetBytes(serverType);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke("Server created Successfully");
        }
        else
        {
            onFailure?.Invoke("Could not create a Server");
        }
    }

    public void ShutdownServer(ushort _port)
    {
        StartCoroutine(HTTPShutdownServer(_port));
    }

    [System.Obsolete]
    private IEnumerator HTTPShutdownServer(ushort _port)
    {
        string url = $"{masterServerUrl}:{masterServerPort}/shutdown-server";
        string body = "{\"port\":" + _port + ", \"serverType\":\"Mirror\"}";
        DebugManager.Log($"Sending shutdown request to {url}");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] data = new System.Text.UTF8Encoding().GetBytes(body);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully shutdown the Server with port: " + _port);
        }
        else
        {
            DebugManager.Log($"Could not shutdown the Server at ${masterServerUrl}:{masterServerPort}\n Error:{request.error}");
        }
    }
}
