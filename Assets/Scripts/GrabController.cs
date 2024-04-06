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
    public float zoomSpeed = 1.0f;

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
            if (heldDownObj.GetComponent<ButtonLockController>() != null)
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

            if (heldDownObj.GetComponent<BoardController>() != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    StopDrawingObject();
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
                    {
                        GameObject gameObject = hit.transform.gameObject;

                        if (gameObject.GetComponent<BoardController>() != null)
                        {
                            Vector3 point = hit.point;
                            point.z = -0.01f;
                            photonView.RPC("Draw", PhotonTargets.AllBuffered, point);
                        }
                    }
                    else
                    {
                        StopDrawingObject();
                    }
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject();
            ZoomObject();
            ChangePageBook();

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

            photonView.RPC("UpdateGrabbedObject", PhotonTargets.OthersBuffered, heldObj.transform.position, heldObj.transform.rotation);
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
        if (gameObject.GetComponent<ScaleController>() != null)
        {
            photonView = gameObject.GetComponentInParent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            TakeOutObject(gameObject);
        }
        if (gameObject.GetComponentInParent<CounterDoorController>() != null)
        {
            photonView = gameObject.GetComponentInParent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            OpenCounterDoor();
        }

        if (gameObject.GetComponent<MoveObjectController>() != null)
        {
            photonView = gameObject.GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            MoveStaticObject();
        }

        if (gameObject.GetComponent<BoardController>() != null)
        {
            photonView = gameObject.GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            DrawObject(gameObject);
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

    private void DrawObject(GameObject drawObj)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
        {
            mouseDown = true;
            heldDownObj = drawObj;
            Vector3 point = hit.point;
            point.z = -0.01f;
            photonView.RPC("Draw", PhotonTargets.AllBuffered, point);
        }

    }

    private void StopDrawingObject()
    {
        mouseDown = false;
        photonView.RPC("StopDrawing", PhotonTargets.AllBuffered);
        heldDownObj = null;
    }

    private void TakeOutObject(GameObject scaleObj)
    {
        GameObject obj = scaleObj.GetComponent<ScaleController>().GetHeldObject();

        if (obj != null)
        {
            photonView.RPC("GetObjectOut", PhotonTargets.OthersBuffered);

            photonView = obj.GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }

            PickupObject(obj);
        }
    }

    private void OpenCounterDoor()
    {
        photonView.RPC("ChangeDoor", PhotonTargets.AllBuffered);
    }

    private void MoveStaticObject()
    {
        photonView.RPC("ChangeMove", PhotonTargets.AllBuffered);
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

        HoldArea.localPosition = new Vector3(0, 0.5f, 3.5f);

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

    private void ZoomObject()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");

        if (wheel > 0)
        {
            float z = HoldArea.localPosition.z + zoomSpeed * Time.deltaTime;
            float y = z / 7f;

            HoldArea.localPosition = new Vector3(0, y, z);
        }
        else if (wheel < 0)
        {
            float z = HoldArea.localPosition.z - zoomSpeed * Time.deltaTime;
            float y = z / 7f;

            HoldArea.localPosition = new Vector3(0, y, z);
        }
    }

    private void ChangePageBook()
    {
        if (heldObj.GetComponent<BookController>() != null)
        {
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                photonView.RPC("CanMove", PhotonTargets.AllBuffered, true);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("CanMove", PhotonTargets.AllBuffered, false);
            }
        }
    }
}
