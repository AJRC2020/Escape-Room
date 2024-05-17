using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLineController : MonoBehaviour
{
    public TimerController timer;

    private int playerCount = 0;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCount == DataTransfer.Instance.players)
        {
            playerCount++;
            DataTransfer.Instance.success = true;
            DataTransfer.Instance.time = timer.GetTimeTaken();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("GameOver");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            if (photonView.isMine)
            {
                photonView.RPC("AddPlayer", PhotonTargets.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void AddPlayer()
    {
        playerCount++;
    }
}
