using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class PanelController : MonoBehaviour
{
    public TextMeshPro diseaseText;
    public int solutionIndex = 9;
    public float rotationSpeed = 100f;

    private List<string> diseases = new List<string>();
    private bool activated = false;
    private bool open = false;
    private bool stop = true;
    private int currentIndex = 0;
    private float totalRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateList();
    }

    // Update is called once per frame
    void Update()
    {
        CheckBook();
        
        if (activated)
        {
            UpdatePanelText();
        }

        if (open && !stop)
        {
            OpenDoor();
        }
    }

    private void CheckBook()
    {
        if (GameObject.Find("DiseaseBook(Clone)") != null)
        {
            activated = true;
        }
    }

    private void OpenDoor()
    {
        Transform parentTrans = transform.parent;

        parentTrans.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        totalRotation += rotationSpeed * Time.deltaTime;

        if (totalRotation > 90)
        {
            parentTrans.eulerAngles = new Vector3(parentTrans.eulerAngles.x, 90f, parentTrans.eulerAngles.z);
            stop = true;
        }
    }

    private void CreateList()
    {
        string filepath = "Assets/Texts/diseases.txt";

        foreach (string line in File.ReadLines(filepath))
        {
            diseases.Add(line);
        }
    }

    private void UpdatePanelText()
    {
        diseaseText.text = diseases[currentIndex];
    }

    public bool isActive()
    {
        return activated;
    }

    public void Increase()
    {
        currentIndex++;

        if (currentIndex == diseases.Count)
        {
            currentIndex = 0;
        }
    }

    public void Decrease()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = diseases.Count - 1;
        }
    }

    public void CheckFinal()
    {
        if (currentIndex == solutionIndex)
        {
            open = true;
            stop = false;
        }
    }
}
