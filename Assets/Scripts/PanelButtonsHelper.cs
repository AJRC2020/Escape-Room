using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtonsHelper : MonoBehaviour
{
    public int type = 0;
    public PanelController panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void ButtonPressed()
    {
        if (panel.isActive())
        {
            switch (type)
            {
                case 0:
                    panel.Decrease();
                    break;

                case 1:
                    panel.CheckFinal(); 
                    break;

                case 2:
                    panel.Increase();
                    break;
            }
        }
    }
}
