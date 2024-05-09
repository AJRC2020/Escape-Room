using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using TMPro;

public class ScaleController : MonoBehaviour
{
    public List<TextMeshPro> digits = new List<TextMeshPro>();
    public GameObject enableDigits;

    private Dictionary<string, int> map = new Dictionary<string, int>();
    private bool notTouched = true;
    private string foodName = "";

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Food" && collision.gameObject.layer == 3 && foodName == "")
        {
            if (notTouched)
            {
                notTouched = false;

                PhotonView photonView = DialogueManager.Instance.GetPhotonView();
                if (photonView.isMine)
                {
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "counter");
                }
            }
            UpdatePanel(collision.gameObject.name);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Food" && foodName == collision.gameObject.name)
        {
            foodName = "";
            enableDigits.SetActive(false);
        }
    }

    private void UpdatePanel(string name)
    {
        foodName = name;

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
