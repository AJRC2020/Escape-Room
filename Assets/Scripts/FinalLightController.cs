using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalLightController : MonoBehaviour
{
    public GameObject key;
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (key == null)
        {
            light.intensity = 100;
        }
    }
}
