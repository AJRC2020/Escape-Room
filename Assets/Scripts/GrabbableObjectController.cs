using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObjectController : MonoBehaviour
{
    private Transform follow;

    private void Update()
    {
        if (follow != null)
        {
            transform.position = follow.position;
            transform.rotation = follow.rotation;
        }   
    }

    public void SetFollow(Transform Follower)
    {
        follow = Follower;
    }

    [PunRPC]
    public void UpdateGrabbedObject(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    [PunRPC]
    public void SetUpObject()
    {
        //GetComponent<PhotonTransformView>().enabled = false;
        Rigidbody heldObjRB = GetComponent<Rigidbody>();
        heldObjRB.useGravity = false;
        heldObjRB.drag = 10;
        heldObjRB.isKinematic = true;
        gameObject.layer = 0;
    }

    [PunRPC]
    public void SetUpObject(Vector3 pos, float scaleFactor)
    {
        GetComponent<PhotonTransformView>().enabled = false;
        Rigidbody heldObjRB = GetComponent<Rigidbody>();
        heldObjRB.useGravity = false;
        heldObjRB.drag = 10;
        gameObject.layer = 0;

        transform.rotation = Quaternion.identity;
        transform.position = pos;
        //transform.localScale = Vector3.one * scaleFactor;
        heldObjRB.constraints = RigidbodyConstraints.FreezePosition;
        heldObjRB.transform.parent = GetComponent<AssociationController>().HoldArea;
    }

    [PunRPC]
    public void DropDownObject()
    {
        //GetComponent<PhotonTransformView>().enabled = true;
        Rigidbody heldObjRB = GetComponent<Rigidbody>();
        heldObjRB.useGravity = true;
        heldObjRB.isKinematic = false;
        heldObjRB.drag = 1;
        gameObject.layer = 3;
    }
}
