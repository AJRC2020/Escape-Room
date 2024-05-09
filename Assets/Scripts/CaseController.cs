using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseController : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public GameObject locker;
    
    private bool startRotation = false;
    private Transform childTrans;
    private float totalRotation = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        childTrans = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (locker != null)
        {
            if (locker.GetComponent<LockController>().VerifySolution()) 
            {
                startRotation = true;
                PhotonView photonViewLocker  = locker.GetComponent<PhotonView>();
                if (photonViewLocker.isMine)
                {
                    PhotonNetwork.Destroy(locker);
                }

                PlayDialogue();
            }
        }

        if (startRotation)
        {
            childTrans.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
            totalRotation += rotationSpeed * Time.deltaTime;
            float angle = childTrans.localEulerAngles.x * Mathf.Deg2Rad;
            float y = 0.125f * Mathf.Cos(angle) + 0.375f * Mathf.Sin(angle) + 0.125f;
            float z = 0.125f * Mathf.Sin(angle) - 0.375f * Mathf.Cos(angle) + 0.375f;
            childTrans.localPosition = new Vector3(0, y, z);

            Check();
        }
    }

    private void PlayDialogue()
    {
        PhotonView photonView = DialogueManager.Instance.GetPhotonView();

        if (photonView.isMine)
        {
            switch (gameObject.name)
            {
                case "Case":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "case1");
                    break;

                case "Case (1)":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "case2");
                    break;

                case "Case (2)":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "case3");
                    break;

                case "Case (3)":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "case4");
                    break;
            }
        }
    }

    private void Check()
    {
        startRotation = totalRotation < 90f;
    }
}
