using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssociationPanelController : MonoBehaviour
{
    private TextMeshPro textMesh;
    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = transform.parent.GetChild(1).GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Card" && other.gameObject.layer == 3 && textMesh.text == "")
        {
            SetWord(other.gameObject);
        }
    }

    private void SetWord(GameObject obj)
    {
        if (!stop)
        {
            TextMeshPro tmp = obj.transform.GetChild(0).GetComponent<TextMeshPro>();

            textMesh.text = tmp.text;
            textMesh.fontSize = tmp.fontSize;
        }
    }

    public void DropWord()
    {
        textMesh.text = "";
    }

    public string GetWord()
    {
        return textMesh.text;
    }

    public void StopPanel()
    {
        stop = true;
    }
}
