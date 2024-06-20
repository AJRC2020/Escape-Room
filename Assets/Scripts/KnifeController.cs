using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    private Rigidbody rb;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "butter" && collision.gameObject.layer == 3)
        {
            PhotonNetwork.Destroy(collision.gameObject);

            photonView.RPC("UnlockKnife", PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    public void UnlockKnife()
    {
        rb.constraints = RigidbodyConstraints.None;
        gameObject.AddComponent<GrabbableObjectController>();
        gameObject.layer = 3;
    }
}
