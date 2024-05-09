using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMoveObjectController : MoveObjectController
{
    public List<Transform> InsideDrawer = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        MoveOtherObjects();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3 && collision.gameObject.GetComponent<GrabbableObjectController>() != null && !InsideDrawer.Contains(collision.gameObject.transform))
        {
            InsideDrawer.Add(collision.gameObject.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (InsideDrawer.Contains(collision.gameObject.transform) && CheckStop()) 
        {
            InsideDrawer.Remove(collision.gameObject.transform);
        }
    }

    private void MoveOtherObjects()
    {
        if (CheckStop())
        {
            return;
        }

        foreach (Transform trans in InsideDrawer)
        {
            float movement = moveSpeed * Time.deltaTime;

            switch (axis)
            {
                case 0:
                    float newX = 0.0f;

                    if (moveTo)
                    {
                        newX = trans.position.x - movement;
                    }
                    else
                    {
                        newX = trans.position.x + movement;
                    }

                    Vector3 newPosX = new Vector3(newX, trans.position.y, trans.position.z);

                    trans.position = newPosX;
                    break;

                case 1:
                    float newY = 0.0f;

                    if (moveTo)
                    {
                        newY = trans.position.y - movement;
                    }
                    else
                    {
                        newY = trans.position.y + movement;
                    }

                    Vector3 newPosY = new Vector3(trans.position.x, newY, trans.position.z);

                    trans.position = newPosY;
                    break;

                case 2:
                    float newZ = 0.0f;

                    if (moveTo)
                    {
                        newZ = trans.position.z - movement;
                    }
                    else
                    {
                        newZ = trans.position.z + movement;
                    }

                    Vector3 newPosZ = new Vector3(trans.position.x, trans.position.y, newZ);

                    trans.position = newPosZ;
                    break;
            }
        }
    }
}
