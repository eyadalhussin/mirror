using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHealthbar : MonoBehaviour
{
    private Transform _camera;


    private void Start()
    {
        GameObject virtualCamera = GameObject.FindWithTag("Virtual Camera");
        if (virtualCamera)
        {
            _camera = virtualCamera.transform;
        }
    }

    private void LateUpdate()
    {
        if (_camera)
        {
            transform.LookAt(transform.position + _camera.forward);
            //transform.rotation = Quaternion.identity;
        }
    }
}
