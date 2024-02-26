using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockController : MonoBehaviour
{
    public int numberCylinders = 5;
    public int numberDigits = 10;
    public List<int> solutions = new List<int>();
    public GameObject cylinder;
    public Transform axisTrans;
    
    private List<CylinderController> cylinderList = new List<CylinderController>();

    // Start is called before the first frame update
    void Start()
    {
        CreateLock();
    }

    // Update is called once per frame
    void Update()
    {
        if (VerifySolution())
        {
            Debug.Log("We did it Joe");
        }
    }

    private void CreateLock()
    {
        axisTrans.localScale = new Vector3(0.5f, 0.35f * numberCylinders, 0.5f);
        axisTrans.localPosition = Vector3.right * 0.3f * (numberCylinders - 1);

        for (int i = 0; i < numberCylinders; i++)
        {
            GameObject newCylinder = Instantiate(cylinder, transform);
            newCylinder.GetComponent<CylinderController>().CreateCylinder(numberDigits);
            newCylinder.transform.localPosition += Vector3.right * 0.6f * i;
            newCylinder.GetComponent<CylinderController>().SetAnswer(solutions[i]);
            cylinderList.Add(newCylinder.GetComponent<CylinderController>());
        }

        Destroy(cylinder);
    }

    [PunRPC]
    public void MoveCylinder(int index)
    {
        cylinderList[index].MoveCylinder();
    }

    private bool VerifySolution()
    {
        foreach (CylinderController cylinder in cylinderList)
        {
            if (!cylinder.Solution())
            {
                return false;
            }
        }
        
        return true;
    }
}
