using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    private string status = "PreConnection";

    private string testIp = "127.0.0.1";  // Default value for IP
    private string testPort = "9000";      // Default value for Port

    private void OnGUI()
    {
        GUIStyle bigLabel = new GUIStyle(GUI.skin.label);
        bigLabel.fontSize = 18;
        bigLabel.normal.textColor = Color.green;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin.left = 10;

        // Define the main container area
        //GUILayout.BeginArea(new Rect(50, 50, 500, 400), GUI.skin.box);
        //GUILayout.Label("Mirror Networking", bigLabel);

        // Check the connection status and draw the appropriate UI

        if(!NetworkClient.isConnected && !NetworkServer.active)
        {
            GUILayout.BeginArea(new Rect(50, 50, 500, 400), GUI.skin.box);
            GUILayout.Label("Mirror Networking", bigLabel);
            DrawPreConnectionUI();
            GUILayout.EndArea();
        } else
        {
            GUILayout.BeginArea(new Rect(10, 50, 520, 100), GUI.skin.box);
            GUILayout.Label("Mirror Networking", bigLabel);
            DrawPostConnectionUI();
            GUILayout.EndArea();
        }

        /*
        switch (status)
        {
            case "PreConnection":
                DrawPreConnectionUI();
                break;
            case "Connected":
                DrawPostConnectionUI();
                break;
            default:
                // Optionally, handle other statuses or default case
                break;
        }*/

    }

    // Draw the UI for pre-connection (Connect Client, Start Host)
    private void DrawPreConnectionUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("IP Address:");
        testIp = GUILayout.TextField(testIp, GUILayout.Width(150));  // IP input field

        // Label and Input field for Port
        GUILayout.Label("Port:");
        testPort = GUILayout.TextField(testPort, GUILayout.Width(70));  // Port input field

        RoomManagerMR.Instance.networkAddress = testIp;
        RoomManagerMR.Instance.GetComponent<SimpleWebTransport>().Port = ushort.Parse(testPort);


        if (GUILayout.Button("Connect", GUILayout.Width(100)))
        {
            RoomManagerMR.Instance.StartClient();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        // Server list
        if (LobbyManager.instance.availableServers.Count > 0)
        {
            int i = 1;
            foreach (var server in LobbyManager.instance.availableServers)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Server: {i++} - Port: {server.port} - Status: {server.status}", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Connect", GUILayout.Width(100)))
                {
                    LobbyManager.instance.ConnectToGameServer(server.port);
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("No servers are currently Running");
            GUILayout.EndHorizontal();
        }

        // Server creation and reload controls at the top
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Server", GUILayout.Width(150)))
        {
            Debug.Log("Creating Server...");
            //LobbyManager.instance.CreateServer();
            if(LobbyManager.instance != null)
            {
                LobbyManager.instance.CreateServer();
            } else
            {
                DebugManager.Log("Failed to create server, LobbyManager is null");
            }
        }

        if (GUILayout.Button("Reload", GUILayout.Width(150)))
        {
            LobbyManager.instance.FetchServers();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }


    // Draw the UI for post-connection (Stop Client, Ready Button, Player List)
    private void DrawPostConnectionUI()
    {
        /*
        if (GUILayout.Button("Stop Client"))
        {
            status = "PreConnection";
            OnStopClient();
        }

        bool ready = false;
        LobbyManager.instance.GetPlayerObjectForClientId(NetworkManager.Singleton.LocalClientId);


        if (GUILayout.Button(ready ? "Not Ready" : "Ready"))
        {
            OnReadyButtonClicked();
        }

        // Show the list of connected players
        GUILayout.Label("Connected Players:");
        DrawPlayersListGUI();*/

        if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                LobbyManager.instance._networkManager.StopClient();
            }
        }
    }

    // Dynamically update the list of connected players
    public void DrawPlayersListGUI()
    {
        /*foreach (var player in LobbyManager.instance.LobbyPlayerList)
        {
            GUILayout.Label($"Name: {player.PlayerName} Id: {player.ClientId} Status: {(player.IsReady ? "Ready" : "Not Ready")}");
        }*/
    }
    public void SetUIStatus(string _status)
    {
        this.status = _status;
    }
}
