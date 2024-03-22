using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RotaryLockController : MonoBehaviour
{
    public List<int> solutions = new List<int>();
    public float rotationSpeed = 10.0f;
    public bool isMoving = false;
    public bool isCounting = true;
    public List<TextMeshPro> digitsList = new List<TextMeshPro>();

    private float currentRotation = 0;
    private int currentIndex = 0;
    private bool currentLeft = true;
    private bool objectiveLeft = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateScreen();
        CheckSolution();

        if (currentIndex == 5)
        {
            Debug.Log("Lock Unlocked");
        }
    }

    public void IncreaseRotation(PhotonView photonView)
    {
        currentRotation += rotationSpeed * Time.deltaTime;

        currentRotation = (currentRotation + 360) % 360;

        currentLeft = true;

        photonView.RPC("Pressed", PhotonTargets.OthersBuffered, currentRotation, currentLeft);
    }

    public void DecreaseRotation(PhotonView photonView)
    {
        currentRotation -= rotationSpeed * Time.deltaTime;

        currentRotation = (currentRotation + 360) % 360;

        currentLeft = false;

        photonView.RPC("Pressed", PhotonTargets.OthersBuffered, currentRotation, currentLeft);
    }

    public void SynchRotation(float newRotation, bool isLeft)
    {
        currentRotation = newRotation;
        currentLeft = isLeft;
        isMoving = true;
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    private void UpdateScreen()
    {
        digitsList[0].text = ((int)currentRotation / 100).ToString();
        digitsList[1].text = ((int)currentRotation / 10 % 10).ToString();
        
        int unitsNumber = (int)currentRotation % 10;
        if (unitsNumber < 5)
        {
            digitsList[2].text = "0";
        }
        else
        {
            digitsList[2].text = "5";
        }

        transform.localEulerAngles = new Vector3(0, currentRotation, 0);
    }

    private void CheckSolution()
    {
        string numberString = digitsList[0].text + digitsList[1].text + digitsList[2].text;
        int rotationNumber = int.Parse(numberString);

        if (!isMoving)
        {
            if ((currentIndex % 2 == 0 && currentLeft || currentIndex % 2 == 1 && !currentLeft) && currentLeft == objectiveLeft && rotationNumber == solutions[currentIndex])
            {
                currentIndex++;
                objectiveLeft = !objectiveLeft;
                isCounting = false;
            }
            else if (isCounting)
            {
                currentIndex = 0;
                objectiveLeft = true;
            }
        }
    }
}
