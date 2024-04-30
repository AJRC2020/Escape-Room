using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class MonitorController : MonoBehaviour
{
    public GameObject key;
    public int answer;
    public TextMeshPro showingSolution;

    private int currentSolution = 0;
    private bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        ChangeActivationChilds(false);
        showingSolution.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        CheckKey();
        CheckPressed();
    }

    private void ChangeActivationChilds(bool activationState)
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(activationState);
        }
    }

    private void CheckPressed()
    {
        for (int i = 3; i < transform.childCount; i++)
        {
            MonitorHelper helper = transform.GetChild(i).GetComponent<MonitorHelper>();

            if (helper.CheckPress())
            {
                helper.Unpress();
                if (i == 3)
                {
                    if (CheckSolution())
                    {
                        ChangeActivationChilds(false);
                        transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else
                    {
                        ResetSolution();
                    }
                }
                else if (i == 4)
                {
                    ResetSolution();
                }
                else
                {
                    if (showingSolution.text.Length < 6)
                    {
                        AddToSolution(i - 5);
                    }
                }
            }
        }
    }

    private void CheckKey()
    {
        if (key == null && firstTime)
        {
            ChangeActivationChilds(true);
            firstTime = false;
        }
    }

    private bool CheckSolution()
    {
        return currentSolution == answer;
    }

    private void ResetSolution()
    {
        showingSolution.text = "";
        currentSolution = 0;
    }

    private void AddToSolution(int digit)
    {
        string digitChar = digit.ToString();

        showingSolution.text += digitChar;

        currentSolution *= 10;
        currentSolution += digit;
    }
}
