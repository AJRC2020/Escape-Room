using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenCaseController : MonoBehaviour
{
    public GameObject key;
    public float rotationSpeed = 50f;

    private bool start = false;
    private float currentRotation = 0.0f;
    private bool over = false;
    private Transform childTrans;

    // Start is called before the first frame update
    void Start()
    {
        childTrans = transform.GetChild(0);
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
                childTrans.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
                currentRotation += rotationSpeed * Time.deltaTime;
                float angle = childTrans.localEulerAngles.y * Mathf.Deg2Rad;
                float x = -0.05f * Mathf.Cos(-angle) - 0.5f * Mathf.Sin(-angle) + 0.5f;
                float z = -0.05f * Mathf.Sin(-angle) + 0.5f * Mathf.Cos(-angle) - 0.5f;
                childTrans.localPosition = new Vector3(x, 0, z);

                if (currentRotation > 90f)
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

            /*PhotonView photonView = DialogueManager.Instance.GetPhotonView();

            if (photonView.isMine)
            {
                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "door2", 10f);
            }*/
        }
    }
}
