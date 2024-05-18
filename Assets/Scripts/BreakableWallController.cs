using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallController : MonoBehaviour
{
    public float breakVelocity = 10f;
    public float minHeight = 1.45f;
    public float maxHeight = 2.05f;
    public float minWidth = 10.9f;
    public float maxWidth = 11.5f;
    public int dialogueLine = 1;

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

            if (contactPoint.y <= maxHeight && contactPoint.y >= minHeight && contactPoint.z <= maxWidth && contactPoint.z >= minWidth && tag == "Brick")
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                float velocity = rb.velocity.magnitude;

                Debug.Log("Velocity = " + velocity);

                if (velocity > breakVelocity)
                {
                    if (!this.photonView.isMine)
                    {
                        this.photonView.TransferOwnership(PhotonNetwork.player);
                    }

                    PhotonView photonView = DialogueManager.Instance.GetPhotonView();

                    string line = "break" + dialogueLine.ToString();
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, line);

                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }
}
