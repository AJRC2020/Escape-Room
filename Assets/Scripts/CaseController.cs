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
                PhotonView photonView  = locker.GetComponent<PhotonView>();
                if (photonView.isMine)
                {
                    PhotonNetwork.Destroy(locker);
                }
            }
        }

        if (startRotation)
        {
            childTrans.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
            float angle = childTrans.localEulerAngles.x * Mathf.Deg2Rad;
            float y = 0.125f * Mathf.Cos(angle) + 0.375f * Mathf.Sin(angle) + 0.125f;
            float z = 0.125f * Mathf.Sin(angle) - 0.375f * Mathf.Cos(angle) + 0.375f;
            childTrans.localPosition = new Vector3(0, y, z);

            Check();
        }
    }

    private void Check()
    {
        startRotation = childTrans.localEulerAngles.x < 90f;
    }
}
