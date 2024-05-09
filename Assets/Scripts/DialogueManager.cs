using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public TextMeshProUGUI dialogue;
    public float typeSpeed = 0.05f;
    public float extraDuration = 0.5f;
    public GameObject greenKey;

    private Dictionary<string, string> DialogueLines = new Dictionary<string, string>();
    private PhotonView photonView;
    private GameObject childObj;

    private float duration = 0.0f;
    private bool showing = false;
    private string text;
    private int startingMessages = 1;
    private int numberOfPlayers = 0;
    private int numberOfPlayersToStart = 1;

    private bool startSpeech = false;
    private int speechLine = 1;
    private bool inExtraTime = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        photonView = GetComponent<PhotonView>();
        childObj = transform.GetChild(0).gameObject;
        numberOfPlayersToStart = DataTransfer.Instance.players;
        GetDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfPlayersToStart == numberOfPlayers && !showing && startingMessages < 4)
        {
            if (photonView.isMine)
            {
                string dialogueChoice = "start" + startingMessages.ToString();

                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, dialogueChoice);
            }
        }

        if (startSpeech && !showing && speechLine < 4)
        {
            if (photonView.isMine)
            {
                string dialogueLine = "speech" + speechLine.ToString();

                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, dialogueLine);
            }
        }

        if (showing && inExtraTime)
        {
            duration -= Time.deltaTime;

            if (duration < 0.0f)
            {
                showing = false;
                inExtraTime = false;
                childObj.SetActive(false);
                if (startingMessages < 4)
                {
                    if (photonView.isMine)
                    {
                        photonView.RPC("IncreaseMessage", PhotonTargets.AllBuffered);
                    }
                }

                if (speechLine < 4 && startSpeech)
                {
                    if (photonView.isMine)
                    {
                        photonView.RPC("IncreaseSpeech", PhotonTargets.AllBuffered);
                    }
                }
            }
        }
    }

    public PhotonView GetPhotonView()
    {
        return photonView;
    }

    [PunRPC]
    public void AddPlayer()
    {
        numberOfPlayers++;
    }

    public int GetNumberOfMessage()
    {
        return startingMessages;
    }

    public int GetNumberOfLines()
    {
        return speechLine;
    }

    public void StartSpeech()
    {
        startSpeech = true;
    }

    public bool hasSpeechStarted()
    {
        return startSpeech;
    }

    [PunRPC]
    public void PlayDialogue(string dialogue)
    {
        text = DialogueLines[dialogue];
        duration = extraDuration;
        showing = true;
        childObj.SetActive(true);
        StartCoroutine(TypeDialogue());
    }

    [PunRPC]
    public void IncreaseMessage()
    {
        startingMessages++;
    }

    [PunRPC]
    public void IncreaseSpeech()
    {
        speechLine++;
    }

    private IEnumerator TypeDialogue()
    {
        for (int i = 0; i <= text.Length; i++)
        {
            string displayText = text.Substring(0, i);

            dialogue.text = displayText;

            yield return new WaitForSeconds(typeSpeed);
        }

        inExtraTime = true;
    }

    private void GetDialogue()
    {
        string filepath = "Assets/Texts/dialogue.txt";

        foreach (string line in File.ReadAllLines(filepath))
        {
            string[] lineParts = line.Split(" ", 2);

            DialogueLines[lineParts[0]] = lineParts[1];
        }
    }
}
