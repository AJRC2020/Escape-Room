using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CylinderController: MonoBehaviour
{
    public int number = 9;
    public TextMeshPro textMesh;
    public float rotTime = 0.5f;

    private Quaternion rotation;
    private float elapsedTime = 0;
    private float currentRotation = 0.0f;
    private bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {
        CreateCylinder();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            elapsedTime = Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, elapsedTime / rotTime);
            if (elapsedTime >= rotTime)
            {
                isRotating = false;
            }
        }
    }

    private void CreateCylinder()
    {
        for (int i = 1; i < number; i++)
        {
            TextMeshPro text = Instantiate(textMesh);
            text.transform.parent = transform;
            text.text = (i + 1).ToString();
            float z = -0.5f * Mathf.Cos(i * 2 * Mathf.PI / number);
            float x = -0.5f * Mathf.Sin(i * 2 * Mathf.PI / number);
            float y = i * 360 / number;
            text.rectTransform.localPosition = new Vector3(x, 0, z);
            text.rectTransform.rotation = textMesh.rectTransform.rotation;
            text.rectTransform.localRotation = Quaternion.Euler(0, y, -90);
            text.rectTransform.localScale = textMesh.rectTransform.localScale;
        }
    }

    [PunRPC]
    public void MoveCylinder()
    {
        currentRotation += 360 / number;
        currentRotation %= 360;
        rotation = Quaternion.Euler(currentRotation, 0, 90);
        elapsedTime = 0;
        isRotating = true;
    }
}
