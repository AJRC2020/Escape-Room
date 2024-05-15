using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenDoorController : MonoBehaviour
{
    public GameObject key;
    public float rotationSpeed = 100f;
    public bool playDialogue = true;

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
        if (!start)
        {
            CheckKey();
        }
        else
        {
            if (!over)
            {
                transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime, Space.Self);
                currentRotation += rotationSpeed * Time.deltaTime;

                if (currentRotation > rotationSpeed)
                {
                    over = true;
                }
            }
        }
    }

    private void CheckKey()
    {
        if (key == null)
        {
            start = true;
            
            if (playDialogue)
            {
                PhotonView photonView = DialogueManager.Instance.GetPhotonView();

                if (photonView.isMine)
                {
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "door2");
                }
            }
        }
    }
}
