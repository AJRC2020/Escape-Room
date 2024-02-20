using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssociationPuzzleSolver : MonoBehaviour
{
    private bool solved = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckChildren() && !solved)
        {
            Debug.Log("Puzzle Solved");
            solved = true;
        }
    }

    private bool CheckChildren()
    {
        foreach (AssociationController platform in GetComponentsInChildren<AssociationController>())
        {
            if (!platform.HeldingItem())
            {
                return false;
            }
        }

        return true;
    }
}
