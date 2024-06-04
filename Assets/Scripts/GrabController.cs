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
    //public Animator animator;

    private PhotonView photonView;
    private GameObject heldObj;
    private Rigidbody heldObjRB;
    private GameObject heldDownObj;

    private float pickupRange = 5.0f;
    private float pickupForce = 150.0f;
    private float senX = 800.0f;
    private float senY = 800.0f;
    private bool mouseDown = false;

    private bool smallDialoguePlayed = false;
    private bool bigDialoguePlayed = false;

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
                    heldDownObj.GetComponent<ButtonLockController>().Pressed(photonView);
                }
            }

            if (heldDownObj.GetComponent<BoardController>() != null || heldDownObj.GetComponent<OperationTableController>() != null)
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
                            point.x = gameObject.transform.position.x - 0.01f;
                            photonView.RPC("Draw", PhotonTargets.AllBuffered, point);
                        }

                        if (gameObject.GetComponent<OperationTableController>() != null)
                        {
                            Vector3 point = hit.point;
                            point.y = gameObject.transform.position.y + 0.02f;
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

            if (heldObj.GetComponent<StethoscopeController>() != null)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    heldObj.GetComponent<StethoscopeController>().ChangeState();
                }
            }
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

            //photonView.RPC("UpdateGrabbedObject", PhotonTargets.OthersBuffered, heldObj.transform.position, heldObj.transform.rotation);
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
            //animator.SetBool("isGrabbing", true);
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

        if (gameObject.GetComponent<BoardController>() != null || gameObject.GetComponent<OperationTableController>() != null)
        {
            photonView = gameObject.GetComponent<PhotonView>();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            if (gameObject.GetComponent<BoardController>() != null)
            {
                DrawObject(gameObject, true);
            }
            else
            {
                DrawObject(gameObject, false);
            }
        }

        if (gameObject.GetComponent<HintCardSpawner>() != null)
        {
            gameObject.GetComponent<HintCardSpawner>().Spawn();
        }

        if (gameObject.GetComponent<BookShelfButtonController>() != null)
        {
            photonView = gameObject.GetComponent<BookShelfButtonController>().GetPhotonView();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            MoveStaticObject();
            if (!bigDialoguePlayed || !smallDialoguePlayed)
            {
                PlayDialogue(gameObject.name);
            }
        }

        if (gameObject.GetComponent<OperationRoomButtonController>() != null)
        {
            photonView = gameObject.GetComponent<OperationRoomButtonController>().GetPhotonView();
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            OperationMinigame();
        }

        if (gameObject.tag == "Key")
        {
            photonView = gameObject.GetComponent<PhotonView>();
            
            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }

            PhotonNetwork.Destroy(gameObject);
        }

        if (gameObject.GetComponent<PanelButtonsHelper>() != null)
        {
            photonView = gameObject.GetComponent<PhotonView>();

            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }

            PanelButton();
        }

        if (gameObject.GetComponent<DoorController>() != null)
        {
            photonView = gameObject.GetComponent<PhotonView>();

            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }

            OpenDoor();
        }

        if (gameObject.GetComponent<MonitorHelper>() != null)
        {
            photonView = gameObject.GetComponent<PhotonView>();

            if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }

            PressMonitor();
        }
    }

    private void PickupObject(GameObject pickObj)
    {
        heldObjRB = pickObj.GetComponent<Rigidbody>();
        heldObjRB.useGravity = false;
        heldObjRB.drag = 10;
        pickObj.layer = 0;
        heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

        GrabbableObjectController goc = pickObj.GetComponent<GrabbableObjectController>();

        goc.SetFollow(HoldArea);

        //heldObjRB.transform.parent = HoldArea;
        heldObj = pickObj;
        //heldObj.GetComponent<PhotonView>().enabled = false;

        //animator.SetBool("isGrabbing", true);

        photonView.RPC("SetUpObject", PhotonTargets.OthersBuffered);
    }

    private void TurnObject(GameObject turnObj)
    {
        int index = turnObj.transform.GetSiblingIndex() - 1;
        photonView.RPC("MoveCylinder", PhotonTargets.AllBuffered, index);
    }
    
    private void RotateLock(GameObject buttonObj)
    {
        mouseDown = true;
        heldDownObj = buttonObj;
        //animator.SetBool("isGrabbing", true);
    }

    private void UnrotateLock()
    {
        mouseDown = false;
        photonView.RPC("Stopped", PhotonTargets.AllBuffered, heldDownObj.GetComponent<ButtonLockController>().isLeft);
        heldDownObj = null;
        //animator.SetBool("isGrabbing", false);
    }

    private void DrawObject(GameObject drawObj, bool isBoard)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
        {
            mouseDown = true;
            heldDownObj = drawObj;
            Vector3 point = hit.point;
            if (isBoard)
            {
                point.x = drawObj.transform.position.x - 0.01f;
            }
            else
            {
                point.y = drawObj.transform.position.y + 0.02f;
            }
            photonView.RPC("Draw", PhotonTargets.AllBuffered, point);
        }

    }

    private void StopDrawingObject()
    {
        mouseDown = false;
        photonView.RPC("StopDrawing", PhotonTargets.AllBuffered);
        heldDownObj = null;
        //animator.SetBool("isGrabbing", false);
    }

    private void OpenCounterDoor()
    {
        photonView.RPC("ChangeDoor", PhotonTargets.AllBuffered);
    }

    private void MoveStaticObject()
    {
        photonView.RPC("ChangeMove", PhotonTargets.AllBuffered);
    }

    private void PanelButton()
    {
        photonView.RPC("ButtonPressed", PhotonTargets.AllBuffered);
    }

    private void OperationMinigame()
    {
        photonView.RPC("ChangeMinigameState", PhotonTargets.AllBuffered);
    }
    private void OpenDoor()
    {
        photonView.RPC("OpenDoor", PhotonTargets.AllBuffered);
    }

    private void PressMonitor()
    {
        photonView.RPC("Pressing", PhotonTargets.AllBuffered);
    }

    private void PlayDialogue(string objName)
    {
        PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();

        if (photonViewDialogue.isMine)
        {
            switch (objName)
            {
                case "Button Small":
                    if (!smallDialoguePlayed)
                    {
                        photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "button1");
                        smallDialoguePlayed = true;
                    }
                    break;

                case "Button Big":
                    if (!bigDialoguePlayed)
                    {
                        photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "button2");
                        bigDialoguePlayed = true;
                    }
                    break;
            }
        }
    }

    private void DropObject(bool isThrow)
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObj.layer = 3;
        heldObjRB.constraints = RigidbodyConstraints.None;

        /*heldObj.GetComponent<PhotonView>().enabled = true;
        heldObjRB.transform.parent = null;*/

        if (heldObj.GetComponent<StethoscopeController>() != null)
        {
            StethoscopeController controler = heldObj.GetComponent<StethoscopeController>();
            if (controler.GetState())
            {
                controler.ChangeState();
            }
        }

        if (isThrow)
        {
            Vector3 force = heldObj.transform.position - transform.position;
            heldObjRB.AddForce(force * forceIntensity, ForceMode.Impulse);
        }

        GrabbableObjectController goc = heldObj.GetComponent<GrabbableObjectController>();
        goc.SetFollow(null);
        heldObj = null;

        //animator.SetBool("isGrabbing", false);

        HoldArea.localPosition = new Vector3(0, 0.5f, 3.5f);
        HoldArea.localEulerAngles = Vector3.zero;

        photonView.RPC("DropDownObject", PhotonTargets.OthersBuffered);
    }

    private void RotateObject()
    {
        cameraController.enabled = false;
        heldObjRB.constraints = RigidbodyConstraints.None;

        float mouseX = Input.GetAxisRaw("Mouse X") * senX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * senY * Time.deltaTime;

        HoldArea.transform.Rotate(Vector3.down, mouseX);
        HoldArea.transform.Rotate(Vector3.right, mouseY);
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
