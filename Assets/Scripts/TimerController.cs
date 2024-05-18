using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerController : MonoBehaviour
{
    public float stageChange = 1.15f;
    public GameObject poisonCloud;

    private int duration = 3600;
    private float timeDelta = 0.0f;

    private List<List<TextMeshPro>> digitsList = new List<List<TextMeshPro>>();
    private PhotonView photonView;
    private bool frozen = true;
    private float timeModifier = 1.0f;

    private bool activateFog = false;
    private float fogTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        CreateDigitsList();

        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen)
        {
            CheckDialogueManager();
        }
        else {
            if (timeDelta >= 1.0f)
            {
                if (photonView.isMine)
                {
                    photonView.RPC("Decrease", PhotonTargets.AllBuffered);
                }

                PlayDialogue();
            }
            else
            {
                timeDelta += Time.deltaTime * timeModifier;
            }

            if (duration == 0)
            {
                DataTransfer.Instance.success = false;
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene("GameOver");
                Debug.Log("Game Over");
            }
        }

        if (activateFog)
        {
            IncreaseFog();
        }
    }

    public PhotonView GetPhotonView()
    {
        return photonView;
    }

    public void IncreaseStage()
    {
        timeModifier *= stageChange;
    }

    public void DecreaseStage()
    {
        timeModifier /= stageChange;

        if (timeModifier < 1.0f)
        {
            timeModifier = 1.0f;
        }
    }

    public string GetTimeTaken()
    {
        int timeTaken = 3600 - duration;

        string time = "";

        time += (timeTaken / 600).ToString();
        time += (timeTaken / 60 % 10).ToString();
        time += ":";
        time += (timeTaken % 60 / 10).ToString();
        time += (timeTaken % 60 % 10).ToString();

        return time;
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

    private void CheckDialogueManager()
    {
        frozen = DialogueManager.Instance.GetNumberOfMessage() < 4;
    }

    private void IncreaseFog()
    {
        fogTime += Time.deltaTime;
        ParticleSystem fog = poisonCloud.GetComponent<ParticleSystem>();
        var emission = fog.emission;

        Vector3 newScale = Vector3.one * fogTime / 10;
        float newEmission = fogTime * 10;

        poisonCloud.transform.localScale = newScale;
        emission.rateOverTime = newEmission;
    }

    private void PlayDialogue()
    {
        PhotonView photonView = DialogueManager.Instance.GetPhotonView();

        if (photonView.isMine)
        {
            switch (duration)
            {
                case 2700:
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "timer1");
                    break;

                case 1800:
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "timer2");
                    break;

                case 900:
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "timer3");
                    break;

                case 600:
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "timer4");
                    break;

                case 300:
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "timer5");
                    break;

                case 60:
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "timer6");
                    activateFog = true;
                    break;
            }
        }
    }
}
