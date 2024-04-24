using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;
    private Animator animator;
    private PhotonView photonView;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            VelocityChange();
        }
    }

    private void VelocityChange()
    {
        velocityX = character.velocity.x;
        velocityZ = character.velocity.z;

        if (velocityX > 0.5f) 
        {
            velocityX = 0.5f;
        }
        if (velocityX < -0.5f)
        {
            velocityX = -0.5f;
        }
        if (velocityZ > 0.5f) 
        {
            velocityZ = 0.5f;
        }
        if (velocityZ < -0.5f)
        {
            velocityZ = -0.5f;
        }

        animator.SetFloat("VelocityZ", velocityZ);
        animator.SetFloat("VelocityX", velocityX);
    }
}
