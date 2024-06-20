using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Debug.Log("Player = " + playerCount);

        if (playerCount == DataTransfer.Instance.players && photonView.isMine)
        {
            photonView.RPC("EndGame", PhotonTargets.AllBuffered);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            /*if (!photonView.isMine)
            {
                photonView.TransferOwnership(PhotonNetwork.player);
            }
            photonView.RPC("AddPlayer", PhotonTargets.AllBuffered);*/

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

    [PunRPC]
    public void EndGame()
    {
        DataTransfer.Instance.success = true;
        DataTransfer.Instance.time = timer.GetTimeTaken();
        StartCoroutine(EndGameCoroutine());

    }

    private IEnumerator EndGameCoroutine()
    {
        // Give some time for the RPC to be sent and processed
        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("GameOver");
    }
}
