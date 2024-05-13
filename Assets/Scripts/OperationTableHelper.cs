using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class OperationTableHelper : MonoBehaviour
{
    private float pickupRange = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckForTable();
    }

    private void CheckForTable()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
        {
            GameObject gameObject = hit.transform.gameObject;
            
            if (gameObject.GetComponent<OperationTableController>() != null)
            {
                Vector3 point = hit.point;

                gameObject.GetComponent<OperationTableController>().SetContactPoint(point);
            }
        }
    }
}
