using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallController : MonoBehaviour
{
    public float breakVelocity = 10f;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.contacts[i];
            Vector3 contactPoint = contact.point;
            string tag = collision.gameObject.tag;

            if (contactPoint.y <= 2.05f && contactPoint.y >= 1.45f && contactPoint.z <= 11.5f && contactPoint.z >= 10.9f && tag == "Brick")
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                float velocity = rb.velocity.magnitude;

                Debug.Log("Velocity = " + velocity);

                if (velocity > breakVelocity && photonView.isMine)
                {
                    PhotonView photonView = DialogueManager.Instance.GetPhotonView();

                    if (photonView.isMine)
                    {
                        photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "break1");
                    }

                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }
}
