using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderController : MonoBehaviour
{
    public int type = 0;
    public bool deletable = false;
    public GameObject key;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (key == null && deletable)
        {
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ResetPosition(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        ResetPosition(other.gameObject);
    }

    private void ResetPosition(GameObject obj)
    {
        Vector3 newPos = obj.transform.position;

        switch (type)
        {
            case 0:
                newPos.y = 1;
                break;

            case 1:
                newPos.x = -21;
                break;

            case 2:
                newPos.x = 28;
                break;

            case 3:
                newPos.z = -2;
                break;

            case 4:
                newPos.z = 28;
                break;

            case 5:
                newPos.x = 35;
                break;

            case 6:
                float diffZ = Mathf.Abs(newPos.z - 14);
                float diffX = Mathf.Abs(newPos.x - 1);

                if (diffZ > diffX)
                {
                    newPos.x = 1;
                }
                else
                {
                    newPos.z = 14;
                }

                break;

            case 7:
                float diffZ2 = Mathf.Abs(newPos.z - 14);
                float diffX2 = Mathf.Abs(newPos.x - 7);

                if (diffZ2 > diffX2)
                {
                    newPos.x = 7;
                }
                else
                {
                    newPos.z = 14;
                }

                break;
        }

        obj.transform.position = newPos;

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
