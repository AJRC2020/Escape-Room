using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssociationController : MonoBehaviour
{
    public bool isPlatform = false;
    public int associationNumber = 0;
    public Transform HoldArea;
    public float rotSpeed = 10;
    public float scaleFactor = 0.5f;

    private Rigidbody heldObjRB;
    private PhotonView heldObjView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (heldObjRB != null)
        {
            heldObjRB.angularVelocity = new Vector3(0.0f, rotSpeed * Time.deltaTime, 0.0f);
        }
        else
        {
            CheckChild();
        }
    }

    public bool HeldingItem()
    {
        return heldObjRB != null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<AssociationController>() != null && collision.gameObject.layer == 3)
        {
            if (collision.gameObject.GetComponent<AssociationController>().associationNumber == associationNumber && isPlatform) 
            {
                SetObject(collision.gameObject);
            }
        }
    }

    private void SetObject(GameObject obj)
    {
        obj.layer = 0;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = HoldArea.position + Vector3.up * 0.5f;
        obj.transform.localScale = Vector3.one * scaleFactor;

        heldObjView = obj.GetComponent<PhotonView>();

        heldObjRB = obj.GetComponent<Rigidbody>();
        heldObjRB.useGravity = false;
        heldObjRB.drag = 10;
        heldObjRB.constraints = RigidbodyConstraints.FreezePosition;

        heldObjRB.transform.parent = HoldArea;

        heldObjView.RPC("SetUpObject", PhotonTargets.OthersBuffered, HoldArea.position, scaleFactor);
    }

    private void CheckChild()
    {
        if (HoldArea.childCount != 0 && heldObjRB == null)
        {
            heldObjRB = HoldArea.GetChild(0).gameObject.GetComponent<Rigidbody>();
        }
    }
}
