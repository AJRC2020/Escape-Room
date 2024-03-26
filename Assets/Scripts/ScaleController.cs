using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using TMPro;

public class ScaleController : MonoBehaviour
{
    public Transform HoldArea;
    public float scaleFactor = 0.5f;
    public List<TextMeshPro> digits = new List<TextMeshPro>();
    public GameObject enableDigits;

    private Rigidbody heldObjRB;
    private float currentTime = 0.0f;
    private Transform childTrans;
    private Vector3 originalPos;
    private Vector3 finalPos;
    private bool allowCollision = true;
    private Dictionary<string, int> map = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        childTrans = transform.GetChild(0);
        originalPos = transform.position;
        finalPos = transform.position - new Vector3(0f, 0.25f, 0f);

        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (heldObjRB != null)
        {
            childTrans.position = Vector3.Lerp(originalPos, finalPos, 4 * currentTime);
            currentTime += Time.deltaTime;
            if (currentTime > 0.25f)
            {
                heldObjRB.gameObject.layer = 3;
            }
        }
        else
        {
            childTrans.position = Vector3.Lerp(finalPos, originalPos, 4 * currentTime);
            currentTime += Time.deltaTime;
            if (currentTime > 0.25f)
            {
                allowCollision = true;
            }
        }
    }

    public GameObject GetHeldObject()
    {
        if (heldObjRB != null)
        {
            GameObject retObj = heldObjRB.gameObject;
            heldObjRB = null;
            enableDigits.SetActive(false);
            return retObj;
        }
        else 
        { 
            return null; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Food" && collision.gameObject.layer == 3 && allowCollision)
        {
            SetObject(collision.gameObject);
        }
    }

    private void SetObject(GameObject obj)
    {
        obj.layer = 0;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = HoldArea.position;

        heldObjRB = obj.GetComponent<Rigidbody>();
        heldObjRB.useGravity = false;
        heldObjRB.drag = 10;
        heldObjRB.constraints = RigidbodyConstraints.FreezeAll;

        heldObjRB.transform.parent = HoldArea;

        UpdatePanel(obj.name);
    }

    private void UpdatePanel(string name)
    {
        int calories = map[name];

        enableDigits.SetActive(true);
        digits[0].text = (calories / 100).ToString();
        digits[1].text = (calories / 10 % 10).ToString();
        digits[2].text = (calories % 10).ToString();   
    }

    private void CreateMap()
    {
        string filePath = "Assets/Texts/food.txt";

        foreach(string line in File.ReadLines(filePath))
        {
            string[] lineSplitted = line.Split(" ");

            map[lineSplitted[0]] = int.Parse(lineSplitted[1]);
        }
    }
}
