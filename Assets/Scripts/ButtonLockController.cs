using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLockController : MonoBehaviour
{
    public RotaryLockController controller;
    public bool isLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pressed(PhotonView photonView)
    {
        if (isLeft)
        {
            controller.IncreaseRotation(photonView);
        }
        else
        {
            controller.DecreaseRotation(photonView);
        }
        controller.isMoving = true;
    }

    public void Stopped()
    {
        controller.isMoving = false;
        controller.isCounting = true;
    }
}
