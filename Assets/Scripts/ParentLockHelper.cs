using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentLockHelper : MonoBehaviour
{
    public ButtonLockController buttonLeft;
    public ButtonLockController buttonRight;
    public RotaryLockController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void Pressed(float newRotation, bool isLeft)
    {
        controller.SynchRotation(newRotation, isLeft);
    }

    [PunRPC]
    public void Stopped(bool isLeft)
    {
        if (isLeft)
        {
            buttonLeft.Stopped();
        }
        else
        {
            buttonRight.Stopped();
        }
    }
}
