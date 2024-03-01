using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    private int duration = 3600;
    private float timeDelta = 0.0f;

    private List<TextMeshPro> digitsList = new List<TextMeshPro>();
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        digitsList.Add(transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>());
        digitsList.Add(transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>());
        digitsList.Add(transform.GetChild(0).GetChild(3).GetComponent<TextMeshPro>());
        digitsList.Add(transform.GetChild(0).GetChild(4).GetComponent<TextMeshPro>());

        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeDelta >= 1.0f) 
        {
            if (photonView.isMine) 
            {
                photonView.RPC("Decrease", PhotonTargets.AllBuffered);
            }
        }
        else
        {
            timeDelta += Time.deltaTime;
        }

        if (duration == 0)
        {
            Debug.Log("Game Over");
        }
    }

    [PunRPC]
    public void Decrease()
    {
        timeDelta = 0.0f;
        duration--;
        digitsList[0].text = (duration / 600).ToString();
        digitsList[1].text = (duration / 60 % 10).ToString();
        digitsList[2].text = (duration % 60 / 10).ToString();
        digitsList[3].text = (duration % 60 % 10).ToString();
        Debug.Log("Duration = " + duration);
    }
}
