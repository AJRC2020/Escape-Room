using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeController : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public GameObject locker;
    public List<Transform> panels = new List<Transform>();

    private bool startRotation = false;
    private Transform childTrans;
    private bool kill = false;

    // Start is called before the first frame update
    void Start()
    {
        childTrans = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (kill && locker != null)
        {
            startRotation = true;
            PhotonView photonView = locker.GetComponent<PhotonView>();
            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(locker);
            }

            PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();
            if (photonViewDialogue.isMine)
            {
                photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "fridge", 6f);
            }
        }

        if (locker != null)
        {
            int progress = locker.GetComponentInChildren<RotaryLockController>().GetCurrentIndex();

            switch(progress)
            {
                case 0:
                    DeactivateEverything(); 
                    break;
                case 1:
                    ActivatePanel(0);
                    break;
                case 2:
                    ActivatePanel(1);
                    break;
                case 3:
                    ActivatePanel(2);
                    break;
                case 4:
                    ActivatePanel(3);
                    break;
                case 5:
                    ActivatePanel(4);
                    kill = true;
                    break;
            }
        }

        if (startRotation)
        {
            childTrans.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
            float angle = childTrans.localEulerAngles.y * Mathf.Deg2Rad;
            float x = 0.835f * Mathf.Cos(-angle) - 0.715f * Mathf.Sin(-angle) - 0.835f;
            float z = 0.835f * Mathf.Sin(-angle) + 0.715f * Mathf.Cos(-angle) - 0.715f;
            childTrans.localPosition = new Vector3(x, 0, z);

            Check();
        }
    }

    private void Check()
    {
        startRotation = childTrans.localEulerAngles.y < 90f;
    }

    private void DeactivateEverything()
    {
        foreach (Transform panel in panels)
        {
            if (panel.GetChild(0).gameObject.GetActive())
            {
                panel.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void ActivatePanel(int index)
    {
        panels[index].GetChild(0).gameObject.SetActive(true);
    }
}
