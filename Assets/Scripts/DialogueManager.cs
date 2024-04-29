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

    private Dictionary<string, string> DialogueLines = new Dictionary<string, string>();
    private PhotonView photonView;
    private GameObject childObj;

    private float duration = 0.0f;
    private bool showing = false;
    private string text;
    private int startingMessages = 1;
    private int numberOfPlayers = 0;
    private int numberOfPlayersToStart = 2;

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

                photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, dialogueChoice, 12f);
            }
        }

        if (showing)
        {
            duration -= Time.deltaTime;

            if (duration < 0.0f)
            {
                Debug.Log("Wtf");
                showing = false;
                childObj.SetActive(false);
                if (startingMessages < 4)
                {
                    startingMessages++;
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

    [PunRPC]
    public void PlayDialogue(string dialogue, float duration)
    {
        text = DialogueLines[dialogue];
        this.duration = duration;
        showing = true;
        childObj.SetActive(true);
        StartCoroutine(TypeDialogue());
    }

    private IEnumerator TypeDialogue()
    {
        for (int i = 0; i <= text.Length; i++)
        {
            string displayText = text.Substring(0, i);

            dialogue.text = displayText;

            yield return new WaitForSeconds(typeSpeed);

        }
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
