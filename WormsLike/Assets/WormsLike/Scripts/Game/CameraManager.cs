using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    float zoomSpeed = 80;
    float moveSpeed = 35;
    float targetCamSpeed = 1f;
    Vector3 basePos;
    float baseSize;

    private void Awake()
	{
        if (instance == null)
            instance = this;
	}

	void Start()
    {
        basePos = Camera.main.transform.position;
        baseSize = Camera.main.orthographicSize;
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

    public void SetCamOnTarget(Vector3 _pos)
	{
        /*Vector3 targetPos = new Vector3(_pos.x, _pos.y, Camera.main.transform.position.z);
        if ((targetPos - Camera.main.transform.position).magnitude > 0.5f)
        {
            Camera.main.transform.position += (targetPos - Camera.main.transform.position) * targetCamSpeed * Time.deltaTime;
        }
        else
            Camera.main.transform.position = targetPos;

        if (Camera.main.orthographicSize <= 19 && Camera.main.orthographicSize >= 6)
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        else
        {
            if (Camera.main.orthographicSize > 19)
                Camera.main.orthographicSize = 19;
            else if (Camera.main.orthographicSize < 6)
                Camera.main.orthographicSize = 6f;
        }*/

        Camera.main.orthographicSize = 7f;
        Camera.main.transform.position = new Vector3(_pos.x, _pos.y, Camera.main.transform.position.z);
    }

    public void ResetCam()
	{
        Camera.main.transform.position = basePos;
        Camera.main.orthographicSize = baseSize;
    }
}
