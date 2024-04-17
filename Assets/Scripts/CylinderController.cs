using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CylinderController: MonoBehaviour
{
    public TextMeshPro textMesh;
    public float rotTime = 0.5f;
    public int answer = 0;
    public int currentAnswer = 0;

    private Quaternion rotation;
    private float elapsedTime = 0;
    private float currentRotation = 0.0f;
    private bool isRotating = false;
    private int number = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            elapsedTime = Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, elapsedTime / rotTime);
            if (elapsedTime >= rotTime)
            {
                isRotating = false;
            }
        }
    }

    public void CreateCylinder(int digits)
    {
        number = digits;
        for (int j = 1; j < number; j++)
        {
            TextMeshPro text = Instantiate(textMesh);
            text.transform.SetParent(transform, false);
            text.text = j.ToString();
            float z = -0.5f * Mathf.Cos(j * 2 * Mathf.PI / number);
            float x = -0.5f * Mathf.Sin(j * 2 * Mathf.PI / number);
            float y = j * 360 / number;
            text.rectTransform.localPosition = new Vector3(x, 0, z);
            text.rectTransform.rotation = textMesh.rectTransform.rotation;
            text.rectTransform.localRotation = Quaternion.Euler(0, y, -90);
            text.rectTransform.localScale = textMesh.rectTransform.localScale;       
        }
    }

    public void MoveCylinder()
    {
        currentRotation += 360 / number;
        currentRotation %= 360;
        rotation = Quaternion.Euler(currentRotation, 0, 90);
        elapsedTime = 0;
        currentAnswer++;
        currentAnswer %= number;
        isRotating = true;
    }

    public void SetAnswer(int newAnswer)
    {
        answer = newAnswer;
    }

    public bool Solution()
    {
        return answer == currentAnswer;
    }
}
