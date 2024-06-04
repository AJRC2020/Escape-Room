using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    private bool frozen = true;
    private bool sent = false;

    private void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.playerName;
            PlayerNameText.gameObject.SetActive(false);
            UncheckBody();
        }
        else
        {
            PlayerNameText.text = photonView.owner.NickName;
        }
    }

    private void Update()
    {
        if (!sent && photonView.isMine)
        {
            DialogueManager.Instance.GetPhotonView().RPC("AddPlayer", PhotonTargets.AllBuffered);
            sent = true;
        }
        if (frozen)
        {
            CheckDialogueManager();
        }
        else if (photonView.isMine)
        {
            CheckInput();
            CheckDialogueManager();
        }
    }

    private void CheckInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        velocity.y += gravity * Time.deltaTime;

        Vector3 moveDir = transform.right * x + transform.forward * z + velocity;

        controller.Move(moveDir * MoveSpeed * Time.deltaTime);

        bool isGrounded = Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, 0.2f);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void UncheckBody()
    {
        for (int i = 0; i < transform.childCount - 3; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            child.gameObject.SetActive(false);
        }
    }

    private void CheckDialogueManager()
    {
        if (DialogueManager.Instance.hasSpeechStarted())
        {
            frozen = DialogueManager.Instance.GetNumberOfLines() < 4;
        }
        else
        {
            frozen = DialogueManager.Instance.GetNumberOfMessage() < 4;
        }
    }
}
