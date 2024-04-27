using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintCardSpawner : MonoBehaviour
{
    public GameObject HintCard;

    private bool spawned = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        if (!spawned)
        {
            PhotonNetwork.Instantiate(HintCard.name, transform.position, Quaternion.identity, 0);
            Destroy(this);
        }
    }

    private void PlayDialogue()
    {
        PhotonView photonView = DialogueManager.Instance.GetPhotonView();

        if (photonView.isMine)
        {
            switch (HintCard.name)
            {
                case "Hint Card 1":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "hint1", 2f);
                    break;

                case "Hint Card 2":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "hint2", 5f);
                    break;

                case "Hint Card 3":
                    photonView.RPC("PlayDialogue", PhotonTargets.AllBuffered, "hint3", 6f);
                    break;
            }
        }
    }
}
