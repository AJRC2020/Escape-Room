using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorHelper : MonoBehaviour
{
    private bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void Pressing()
    {
        pressed = true;
    }

    public bool CheckPress()
    {
        return pressed;
    }

    public void Unpress()
    {
        pressed = false;
    }
}
