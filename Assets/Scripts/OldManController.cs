using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OldManController : MonoBehaviour
{
    public GameObject Pizza;
    public GameObject Cocktail;
    public GameObject Muffin;
    public GameObject Stool;
    public GameObject Glasses;
    public GameObject TV;
    public GameObject blueKey;
    public float cooldownTime = 120f;
    public TimerController timer;
    public float triggerDis = 3f;
    public OperationTableController operation;

    private bool allowFood = false;
    private bool allowStool = false;
    private bool allowGlasses = false;
    private bool allowRemote = false;
    private bool startPuzzle = false;
    private bool proximityChecked = false;
    private bool coffeeTaken = false;
    private bool finaleMessage = false;

    private List<string> dialogues = new List<string>();
    private float currentTimeOut = 0f;

    // Start is called before the first frame update
    void Start()
    {
        dialogues.Add("requestStool");
        dialogues.Add("requestRemote");
        dialogues.Add("requestGlasses");
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckProximity() && !proximityChecked)
        {
            DialogueManager.Instance.StartSpeech();
            proximityChecked = true;
        }

        if (CheckPuzzleStart() && !startPuzzle)
        {
            startPuzzle = true;
            currentTimeOut = cooldownTime;
        }

        if (startPuzzle)
        {
            if (currentTimeOut <= 0.0f)
            {
                RNGDialogue();
            }
            else
            {
                currentTimeOut -= Time.deltaTime;
            }
        }

        if (CheckFinale() && !finaleMessage)
        {
            PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();

            if (photonViewDialogue.isMine)
            {
                photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "finale");
            }

            finaleMessage = true;
            DialogueManager.Instance.StartFinale();
        }

        if (DialogueManager.Instance.SpawnBlueKey() && blueKey != null)
        {
            blueKey.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            bool delete = false;

            switch (collision.gameObject.name)
            {
                case "pizza":
                    if (allowFood)
                    {
                        Pizza.SetActive(true);
                        delete = true;
                    }

                    break;

                case "muffin":
                    if (allowFood)
                    {
                        Muffin.SetActive(true);
                        delete = true;
                    }

                    break;

                case "cocktail":
                    if (allowFood)
                    {
                        Cocktail.SetActive(true);
                        delete = true;
                    }

                    break;

                case "remote":
                    if (allowRemote)
                    {
                        TV.SetActive(true);
                        delete = true;
                    }

                    break;

                case "Stool":
                    if (allowStool)
                    {
                        Stool.SetActive(true);
                        dialogues.Add("requestFood");
                        delete = true;
                    }
                    break;

                case "Glasses":
                    if (allowGlasses)
                    {
                        Glasses.SetActive(true);
                        delete = true;
                    }
                    break;

                case "cup(Clone)":
                    if (operation.GetState() == 3)
                    {
                        coffeeTaken = true;
                        delete = true;
                    }
                    break;

                default:
                    break;
            }

            if (delete)
            {
                timer.DecreaseStage();

                PhotonView photonView = collision.gameObject.GetComponent<PhotonView>();

                if (photonView.isMine)
                {
                    PhotonNetwork.Destroy(collision.gameObject);
                }
            }
        }
    }

    private void RNGDialogue()
    {
        if (dialogues.Count == 0)
        {
            startPuzzle = false;
            return;
        }

        int choice = Random.Range(0, dialogues.Count - 1);

        PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();

        if (photonViewDialogue.isMine)
        {
            photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, dialogues[choice]);
        }

        switch(dialogues[choice])
        {
            case "requestStool":
                allowStool = true;
                break;

            case "requestRemote":
                allowRemote = true;
                break;

            case "requestGlasses":
                allowGlasses = true;
                break;

            case "requestFood":
                allowFood = true;
                break;

        }

        dialogues.RemoveAt(choice);
        timer.IncreaseStage();
        currentTimeOut = cooldownTime;
    }

    private bool CheckProximity()
    {
        PlayerController[] objs = FindObjectsOfType<PlayerController>();

        foreach (PlayerController obj in objs)
        {
            Vector3 playerPos = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
            Vector3 oldManPos = new Vector3(transform.position.x, 0, transform.position.z);

            float distance = Vector3.Distance(oldManPos, playerPos);
            
            if (distance < triggerDis)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckPuzzleStart()
    {
        return DialogueManager.Instance.GetNumberOfLines() == 4;
    }

    private bool CheckFinale()
    {
        return dialogues.Count == 0 && coffeeTaken;
    }
}
