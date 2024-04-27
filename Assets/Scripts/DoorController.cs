using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float rotationSpeed = 100f;

    private bool start = false;
    private float currentRotation = 0.0f;
    private bool over = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime, Space.Self);
            currentRotation += rotationSpeed * Time.deltaTime;

            if (currentRotation > 90f)
            {
                over = true;
                start = false;
            }
        }
    }

    [PunRPC]
    public void OpenDoor()
    {
        if (!over)
        {
            start = true;

            PhotonView photonView = DialogueManager.Instance.GetPhotonView();

            if (photonView.isMine)
            {
                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "door1", 8f);
            }
        }
    }
}
