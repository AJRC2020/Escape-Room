using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectController : MonoBehaviour
{
    public float moveDistance = 1;
    public float moveSpeed = 10f;
    public int axis = 0;
    public bool useWorld = false;

    protected bool moveTo = false;
    private float originalPos;
    private float finalPos;
    private bool notMoved = true;

    // Start is called before the first frame update
    protected void Start()
    {
        switch(axis)
        {
            case 0:
                originalPos = transform.localPosition.x;
                break;

            case 1:
                originalPos = transform.localPosition.y;
                break;

            case 2:
                originalPos = transform.localPosition.z;
                break;
        }

        finalPos = originalPos + moveDistance;

        if (moveDistance < 0)
        {
            moveSpeed *= -1;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        MoveObject();
    }

    [PunRPC]
    public void ChangeMove()
    {
        moveTo = !moveTo;
    }

    public bool isRight()
    {
        if (moveTo)
        {
            if (moveDistance > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (moveDistance > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public bool BookHelper()
    {
        return CheckStop() && !isRight();
    }

    private void MoveObject()
    {
        if (CheckStop())
        {
            return;
        }

        if (notMoved)
        {
            PlayDialogue();
            notMoved = false;
        }

        Vector3 movement = Vector3.zero;

        switch(axis)
        {
            case 0:
                movement = Vector3.right * moveSpeed * Time.deltaTime;
                break;

            case 1:
                movement = Vector3.up * moveSpeed * Time.deltaTime; 
                break;

            case 2:
                movement = Vector3.forward * moveSpeed * Time.deltaTime;
                break;
        }

        if (moveTo)
        {
            if (useWorld)
            {
                transform.Translate(movement, Space.World);
            }
            else
            {
                transform.Translate(movement, Space.Self);
            }
        }
        else
        {
            if (useWorld)
            {
                transform.Translate(-movement, Space.World);
            }
            else
            {
                transform.Translate(-movement, Space.Self);
            }
        }
    }

    private void PlayDialogue()
    {
        PhotonView photonView = DialogueManager.Instance.GetPhotonView();

        if (photonView.isMine)
        {
            if (gameObject.name == "Rug2")
            {
                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "rug");
            }
        }
    }

    protected bool CheckStop()
    {
        if (moveTo)
        {
            switch(axis)
            {
                case 0:
                    if (moveDistance > 0)
                    {
                        if (transform.localPosition.x >= finalPos)
                        {
                            transform.localPosition = new Vector3(finalPos, transform.localPosition.y, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (transform.localPosition.x <= finalPos)
                        {
                            transform.localPosition = new Vector3(finalPos, transform.localPosition.y, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                case 1:
                    if (moveDistance > 0)
                    {
                        if (transform.localPosition.y >= finalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, finalPos, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (transform.localPosition.y <= finalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, finalPos, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                case 2:
                    if (moveDistance > 0)
                    {
                        if (transform.localPosition.z >= finalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, finalPos);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (transform.localPosition.z <= finalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, finalPos);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                default:
                    return false;
            }
        }
        else
        {
            switch (axis)
            {
                case 0:
                    if (moveDistance < 0)
                    {
                        if (transform.localPosition.x >= originalPos)
                        {
                            transform.localPosition = new Vector3(originalPos, transform.localPosition.y, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (transform.localPosition.x <= originalPos)
                        {
                            transform.localPosition = new Vector3(originalPos, transform.localPosition.y, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                case 1:
                    if (moveDistance < 0)
                    {
                        if (transform.localPosition.y >= originalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, originalPos, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (transform.localPosition.y <= originalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, originalPos, transform.localPosition.z);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                case 2:
                    if (moveDistance < 0)
                    {
                        if (transform.localPosition.z >= originalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, originalPos);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (transform.localPosition.z <= originalPos)
                        {
                            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, originalPos);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                default:
                    return false;
            }
        }
    }
}
