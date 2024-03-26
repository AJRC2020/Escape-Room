using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectController : MonoBehaviour
{
    public float moveDistance = 1;
    public float moveSpeed = 10f;
    public int axis = 0;

    private bool moveTo = false;
    private float originalPos;
    private float finalPos;

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        Debug.Log("Current position: " + transform.position);
        MoveObject();   
    }

    [PunRPC]
    public void ChangeMove()
    {
        moveTo = !moveTo;
    }

    private void MoveObject()
    {
        if (CheckStop())
        {
            return;
        }

        Vector3 movement = Vector3.zero;

        switch(axis)
        {
            case 0:
                Debug.Log("Got here 0");
                movement = Vector3.right * moveSpeed * Time.deltaTime;
                break;

            case 1:
                Debug.Log("Got here 1");
                movement = Vector3.up * moveSpeed * Time.deltaTime; 
                break;

            case 2:
                Debug.Log("Got here 2");
                movement = Vector3.forward * moveSpeed * Time.deltaTime;
                break;
        }

        if (moveTo)
        {
            transform.Translate(movement, Space.World);
        }
        else
        {
            transform.Translate(-movement, Space.World);
        }
    }

    private bool CheckStop()
    {
        if (moveTo)
        {
            switch(axis)
            {
                case 0:
                    if (moveDistance > 0)
                    {
                        return transform.localPosition.x >= finalPos;
                    }
                    else
                    {
                        return transform.localPosition.x <= finalPos;
                    }

                case 1:
                    if (moveDistance > 0)
                    {
                        return transform.localPosition.y >= finalPos;
                    }
                    else
                    {
                        return transform.localPosition.y <= finalPos;
                    }

                case 2:
                    if (moveDistance > 0)
                    {
                        return transform.localPosition.z >= finalPos;
                    }
                    else
                    {
                        return transform.localPosition.z <= finalPos;
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
                        return transform.localPosition.x >= originalPos;
                    }
                    else
                    {
                        return transform.localPosition.x <= originalPos;
                    }

                case 1:
                    if (moveDistance < 0)
                    {
                        return transform.localPosition.y >= originalPos;
                    }
                    else
                    {
                        return transform.localPosition.y <= originalPos;
                    }

                case 2:
                    if (moveDistance < 0)
                    {
                        return transform.localPosition.z >= originalPos;
                    }
                    else
                    {
                        return transform.localPosition.z <= originalPos;
                    }

                default:
                    return false;
            }
        }
    }
}
