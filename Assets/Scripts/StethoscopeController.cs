using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StethoscopeController : MonoBehaviour
{
    public GameObject finding;
    public GameObject controlPanel;
    public Image soundLevel;

    private bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isShowing)
        {
            UpdateUI();
        }
    }

    public void ChangeState()
    {
        isShowing = !isShowing;
        controlPanel.SetActive(isShowing);
    }

    public bool GetState()
    {
        return isShowing;
    }

    private void UpdateUI()
    {
        Vector3 findPos = finding.transform.position;
        Vector3 pos = transform.position;

        float distance = Vector3.Distance(pos, findPos);
        float scaleFactor = 0.6f / distance;
       
        if (scaleFactor > 0.6f)
        {
            scaleFactor = 0.6f;
        }

        Color imageColor = Color.Lerp(Color.red, Color.blue, distance / 15);
        soundLevel.color = imageColor;

        soundLevel.transform.localScale = new Vector3(soundLevel.transform.localScale.x, scaleFactor, soundLevel.transform.localScale.z);
    }
}
