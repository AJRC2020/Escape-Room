using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterDoorController : MonoBehaviour
{
    public float rotSpeed = 40.0f;

    private bool open = false;
    private Transform childTrans;
    private static bool notMoved = true;

    // Start is called before the first frame update
    void Start()
    {
        childTrans = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        RotateDoor();

        if (childTrans.localPosition.z > -0.45)
        {
            childTrans.localPosition = new Vector3(0, 0, -0.45f);
        }
    }

    [PunRPC]
    public void ChangeDoor()
    {
        open = !open;

        if (notMoved)
        {
            notMoved = false;
            PhotonView photonView = DialogueManager.Instance.GetPhotonView();

            if (photonView.isMine)
            {
                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "counter");
            }
        }
    }

    private void RotateDoor()
    {
        if (open)
        {
            if (childTrans.localEulerAngles.y >= 90)
            {
                childTrans.localEulerAngles = new Vector3(childTrans.localEulerAngles.x, 90, childTrans.localEulerAngles.z);
                return;
            }
            childTrans.Rotate(Vector3.up, rotSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            if (childTrans.localEulerAngles.y == 0 || childTrans.localEulerAngles.y > 270)
            {
                childTrans.localEulerAngles = new Vector3(childTrans.localEulerAngles.x, 0, childTrans.localEulerAngles.z);
                return;
            }
            childTrans.Rotate(Vector3.up, -rotSpeed * Time.deltaTime, Space.Self);
        }

        float angle = childTrans.localEulerAngles.y * Mathf.Deg2Rad;
        float x = 0.5f * Mathf.Cos(-angle) - 0.05f * Mathf.Sin(-angle) - 0.5f;
        float z = 0.5f * Mathf.Sin(-angle) + 0.05f * Mathf.Cos(-angle) - 0.5f;
        childTrans.localPosition = new Vector3(x, 0, z);
    }
}
