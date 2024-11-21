using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class OfflineManager : MonoBehaviour
{
    private const string TestUrl = "https://google.com";

    void Start()
    {
        DebugManager.Log("Offline Manager: Checking internet connection");
        DebugManager.Log("Current Scene is: " + SceneManager.GetActiveScene().name);
        StartCoroutine(CheckInternetConnection());
    }

    private IEnumerator CheckInternetConnection()
    {
        UnityWebRequest request = UnityWebRequest.Head(TestUrl);
        request.timeout = 5; // Set timeout in seconds
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && SceneManager.GetActiveScene().name.Equals("Offline"))
        {
            DebugManager.Log("Internet connection available. Transitioning to Room Scene.");
            SceneManager.LoadScene("Room");
        }
        else
        {
            DebugManager.Log("No internet connection available!");
        }
    }
}
