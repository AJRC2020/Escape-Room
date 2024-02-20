using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform HoldArea;
    public CameraPlayerController cameraController;

    private PhotonView photonView;
    private GameObject heldObj;
    private Rigidbody heldObjRB;

    private float pickupRange = 5.0f;
    private float pickupForce = 150.0f;
    private float senX = 800.0f;
    private float senY = 800.0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
                {
                    GameObject gameObject = hit.transform.gameObject;
                    photonView = gameObject.GetComponent<PhotonView>();

                    if (photonView.isMine) 
                    {
                        PickupObject(hit.transform.gameObject);
                    }
                    else if (hit.transform.gameObject.layer == 3)
                    {
                        photonView.TransferOwnership(PhotonNetwork.player);
                        PickupObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                DropObject();
            }
        }

        if (heldObj != null)
        {
            MoveObject();

            if (Input.GetMouseButton(1))
            {
                RotateObject();
            }
            else
            {
                heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;
                cameraController.enabled = true;
            }

            photonView.RPC("UpdateGrabbedObject", PhotonTargets.Others, heldObj.transform.position, heldObj.transform.rotation);
        }
    }

    private void MoveObject()
    {
        if (Vector3.Distance(heldObj.transform.position, HoldArea.position) > 0.1f)
        {
            Vector3 moveDir = (HoldArea.position - heldObj.transform.position);
            heldObjRB.AddForce(moveDir * pickupForce);
        }
    }

    private void PickupObject(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>() != null) 
        {
            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            pickObj.layer = 0;
            heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = HoldArea;
            heldObj = pickObj;
            heldObj.GetComponent<PhotonView>().enabled = false;

            photonView.RPC("SetUpObject", PhotonTargets.Others);
        }
    }

    private void DropObject()
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObj.layer = 3;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObj.GetComponent<PhotonView>().enabled = true;
        heldObjRB.transform.parent = null;
        heldObj = null;

        photonView.RPC("DropDownObject", PhotonTargets.Others);
    }

    private void RotateObject()
    {
        cameraController.enabled = false;
        heldObjRB.constraints = RigidbodyConstraints.None;

        float mouseX = Input.GetAxisRaw("Mouse X") * senX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * senY * Time.deltaTime;

        heldObj.transform.Rotate(Vector3.down, mouseX);
        heldObj.transform.Rotate(Vector3.right, mouseY);
    }
}
