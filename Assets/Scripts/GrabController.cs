using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform HoldArea;
    public CameraPlayerController cameraController;
    public float forceIntensity = 5.0f;

    private PhotonView photonView;
    private GameObject heldObj;
    private Rigidbody heldObjRB;
    private GameObject heldDownObj;

    private float pickupRange = 5.0f;
    private float pickupForce = 150.0f;
    private float senX = 800.0f;
    private float senY = 800.0f;
    private bool mouseDown = false;

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
                    HandleObject(gameObject);
                }
            }
            else
            {
                DropObject(false);
            }
        }

        if (mouseDown) 
        { 
            if (Input.GetMouseButtonUp(0))
            {
                UnrotateLock();
            }
            else
            {
                //photonView.RPC("Pressed", PhotonTargets.AllBuffered, heldDownObj.GetComponent<ButtonLockController>().isLeft);
                heldDownObj.GetComponent<ButtonLockController>().Pressed(photonView);
            }
        }

        if (heldObj != null)
        {
            MoveObject();

            if (Input.GetMouseButton(1))
            {
                RotateObject();
            }
            else if (Input.GetMouseButtonDown(2))
            {
                DropObject(true);
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

    private void HandleObject(GameObject gameObject)
    {
        if (gameObject.GetComponent<Rigidbody>() != null && gameObject.layer == 3)
        {
            photonView = gameObject.GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            PickupObject(gameObject);
        }
        if (gameObject.GetComponent<CylinderController>() != null) 
        {
            photonView = gameObject.GetComponentInParent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            TurnObject(gameObject);
        }
        if (gameObject.GetComponent<ButtonLockController>() != null)
        {
            photonView = gameObject.GetComponentInParent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            RotateLock(gameObject);
        }
    }

    private void PickupObject(GameObject pickObj)
    {
        heldObjRB = pickObj.GetComponent<Rigidbody>();
        heldObjRB.useGravity = false;
        heldObjRB.drag = 10;
        pickObj.layer = 0;
        heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

        heldObjRB.transform.parent = HoldArea;
        heldObj = pickObj;
        heldObj.GetComponent<PhotonView>().enabled = false;

        photonView.RPC("SetUpObject", PhotonTargets.OthersBuffered);
    }

    private void TurnObject(GameObject turnObj)
    {
        //turnObj.GetComponent<CylinderController>().MoveCylinder();
        int index = turnObj.transform.GetSiblingIndex() - 1;
        photonView.RPC("MoveCylinder", PhotonTargets.AllBuffered, index);
    }
    
    private void RotateLock(GameObject buttonObj)
    {
        mouseDown = true;
        heldDownObj = buttonObj;
    }

    private void UnrotateLock()
    {
        mouseDown = false;
        photonView.RPC("Stopped", PhotonTargets.AllBuffered, heldDownObj.GetComponent<ButtonLockController>().isLeft);
        //heldDownObj.GetComponent<ButtonLockController>().Stopped();
        heldDownObj = null;
    }

    private void DropObject(bool isThrow)
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObj.layer = 3;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObj.GetComponent<PhotonView>().enabled = true;
        heldObjRB.transform.parent = null;

        if (isThrow)
        {
            Vector3 force = heldObj.transform.position - transform.position;
            heldObjRB.AddForce(force * forceIntensity, ForceMode.Impulse);
        }

        heldObj = null;

        photonView.RPC("DropDownObject", PhotonTargets.OthersBuffered);
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
