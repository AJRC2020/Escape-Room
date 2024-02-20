using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerController : MonoBehaviour
{
    public PhotonView photonView;
    public Transform playerBody;

    public float senX = 400.0f;
    public float senY = 400.0f;

    private float rotX = 0.0f;

    private void Awake()
    {
        if (photonView.isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (photonView.isMine)
        {
            CheckCamera();
        }
    }

    private void CheckCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * senX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * senY * Time.deltaTime;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
