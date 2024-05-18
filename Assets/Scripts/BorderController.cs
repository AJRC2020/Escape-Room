using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderController : MonoBehaviour
{
    public int type = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ResetPosition(other.gameObject);
    }

    private void ResetPosition(GameObject obj)
    {
        switch (type)
        {
            case 0:
                obj.transform.position += Vector3.up * 2;
                break;

            case 1:
                obj.transform.position += Vector3.right;
                break;

            case 2:
                obj.transform.position += Vector3.left;
                break;

            case 3:
                obj.transform.position += Vector3.forward;
                break;

            case 4:
                obj.transform.position += Vector3.back;
                break;
        }

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
