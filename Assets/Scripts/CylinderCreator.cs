using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CylinderCreator : MonoBehaviour
{
    public int number = 9;
    public TextMeshPro textMesh;
    public float rotTime = 0.5f;

    private Quaternion rotation;
    private bool isRotating = false;
    private float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        //rotation = Quaternion.Euler(0, 0, 90);
        CreateCylinder();
    }

    // Update is called once per frame
    void Update()
    {
        //elapsedTime = Time.deltaTime;
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, elapsedTime / rotTime);
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

    public void MoveCylinder()
    {
        if (!isRotating)
        {
            rotation = Quaternion.Euler(transform.rotation.x + 360 / number, 0, transform.rotation.z);
            isRotating = true;
            elapsedTime = 0;
        }
    }
}
