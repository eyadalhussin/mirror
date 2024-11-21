using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class CameraComponent : NetworkBehaviour
{
    private CinemachineVirtualCamera vrCamera;
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            vrCamera = CinemachineVirtualCamera.FindObjectOfType<CinemachineVirtualCamera>();
            vrCamera.LookAt = gameObject.transform;
            vrCamera.Follow = gameObject.transform;
        }
    }
}
