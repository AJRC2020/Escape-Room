using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    private GrabbableObjectController controller;
    private PhotonView photonView;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<GrabbableObjectController>();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        controller.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "butter" && collision.gameObject.layer == 3)
        {
            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(collision.gameObject);
            }

            rb.constraints = RigidbodyConstraints.None;
            controller.enabled = true;
        }
    }
}
