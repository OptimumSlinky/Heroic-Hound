using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;

    public Vector3 cameraOffset;

    // Zoom variables
    public float currentZoom = 1.5f;
    public float zoomSpeed = 0.5f;
    public float minZoom = 0.5f;
    public float maxZoom = 3f;

    // Camera rotation
    public float turnSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mainCamera.transform.position = player.position + cameraOffset * currentZoom;
        mainCamera.transform.LookAt(player.position);
    }
}
