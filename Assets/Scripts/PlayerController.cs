using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public GameObject PlayerCamera;
    public TMP_Text PlayerNameText;
    public CharacterController controller;

    public float MoveSpeed = 3.0f;
    public float GroundDrag = 3.0f;

    private float x;
    private float z;
    private float gravity = -9.81f;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.playerName;
        }
        else
        {
            PlayerNameText.text = photonView.owner.NickName;
        }
    }

    private void Update()
    {
        if (photonView.isMine)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        velocity.y += gravity * Time.deltaTime;

        Vector3 moveDir = transform.right * x + transform.forward * z + velocity;

        controller.Move(moveDir * MoveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
}
