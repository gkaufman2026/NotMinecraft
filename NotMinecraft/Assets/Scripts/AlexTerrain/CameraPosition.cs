using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    // Basic free cam controlling script

    public GameObject player;
    public Camera Camera;
    Rigidbody rb;

    void Start()
    {
        Camera.transform.position = player.transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Camera.transform.position = player.transform.position;

        if (Input.GetKey(KeyCode.A)) rb.AddForce(Vector3.left);
        if (Input.GetKey(KeyCode.D)) rb.AddForce(Vector3.right);
        if (Input.GetKey(KeyCode.W)) rb.AddForce(Vector3.forward);
        if (Input.GetKey(KeyCode.S)) rb.AddForce(Vector3.back);
        if (Input.GetKey(KeyCode.Space)) rb.AddForce(Vector3.up);
        if (Input.GetKey(KeyCode.LeftControl)) rb.AddForce(Vector3.down);
    }
}
