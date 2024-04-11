using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssociationPanelController : MonoBehaviour
{
    private Rigidbody heldObjRB = null;
    private TextMeshPro textMesh;

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
        if (other.gameObject.tag == "Card" && other.gameObject.layer == 3 && heldObjRB == null)
        {
            SetWord(other.gameObject);
        }
    }

    private void SetWord(GameObject obj)
    {
        TextMeshPro tmp = obj.transform.GetChild(0).GetComponent<TextMeshPro>();

        textMesh.text = tmp.text;
        textMesh.fontSize = tmp.fontSize;
    }

    public void DropWord()
    {
        textMesh.text = "";
    }

    public string GetWord()
    {
        return textMesh.text;
    }
}
