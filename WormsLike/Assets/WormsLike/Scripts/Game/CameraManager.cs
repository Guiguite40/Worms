using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    float zoomSpeed = 80;
    float moveSpeed = 35;

    private void Awake()
	{
        if (instance == null)
            instance = this;
	}

	void Start()
    {
        
    }

    void Update()
    {
        UpdateFreeLook();
    }

    void UpdateFreeLook()
	{
        if (Camera.main.orthographicSize <= 19 && Camera.main.orthographicSize >= 3.5)
            Camera.main.orthographicSize += -Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        else
        {
            if(Camera.main.orthographicSize > 19)
                Camera.main.orthographicSize = 19;
            else if(Camera.main.orthographicSize < 3.5)
                Camera.main.orthographicSize = 3.5f;
        }

        if (Input.GetKey(KeyCode.Q))
            Camera.main.transform.position += new Vector3(-moveSpeed, 0, 0) * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            Camera.main.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
        if (Input.GetKey(KeyCode.Z))
            Camera.main.transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            Camera.main.transform.position += new Vector3(0, -moveSpeed, 0) * Time.deltaTime;
    }
}
