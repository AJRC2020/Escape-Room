using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    private int duration = 3600;
    private float timeDelta = 0.0f;

    private List<List<TextMeshPro>> digitsList = new List<List<TextMeshPro>>();
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        CreateDigitsList();

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

    public PhotonView GetPhotonView()
    {
        return photonView;
    }

    [PunRPC]
    public void Decrease()
    {
        timeDelta = 0.0f;
        duration--;
        UpdateAllTimers();
    }

    [PunRPC]
    public void Penalty(int minutes)
    {
        duration -= minutes * 60;
        UpdateAllTimers();
    }

    private void CreateDigitsList()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i).GetChild(0);
            List<TextMeshPro> digits = new List<TextMeshPro>();

            for (int j = 0; j < child.childCount; j++)
            {
                if (j != 2)
                {
                    digits.Add(child.GetChild(j).GetComponent<TextMeshPro>());
                }
            }

            digitsList.Add(digits);
        }
    }

    private void UpdateAllTimers()
    {
        foreach (List<TextMeshPro> digits in digitsList)
        {
            digits[0].text = (duration / 600).ToString();
            digits[1].text = (duration / 60 % 10).ToString();
            digits[2].text = (duration % 60 / 10).ToString();
            digits[3].text = (duration % 60 % 10).ToString();
        }
    }
}
