using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public float duration = 1.0f;

    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject Camera;

    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private TMP_InputField RoomInput;
    [SerializeField] private TMP_InputField PlayersInput;

    [SerializeField] private GameObject CreateBtn;
    [SerializeField] private GameObject JoinBtn;
    [SerializeField] private GameObject LeftBtn;
    [SerializeField] private GameObject RightBtn;

    private bool firstOption = true;
    private bool moving = false;
    private Vector3 pos1 = new Vector3(0, 1, -10);
    private Vector3 pos2 = new Vector3(9, 1, -10);
    private float timeLapsed = 0f;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
    }

    private void Update()
    {
        if (moving)
        {
            timeLapsed += Time.deltaTime;

            if (firstOption)
            {
                Camera.transform.position = Vector3.Lerp(pos2, pos1, timeLapsed / duration);

                if (timeLapsed > duration)
                {
                    RightBtn.SetActive(true);
                    moving = false;
                    timeLapsed = 0;
                }
            }
            else
            {
                Camera.transform.position = Vector3.Lerp(pos1, pos2, timeLapsed / duration);

                if (timeLapsed > duration)
                {
                    LeftBtn.SetActive(true);
                    moving = false;
                    timeLapsed = 0;
                }
            }
        }

        EnableButtons();
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    private void EnableButtons()
    {
        if (UsernameInput.text.Length >= 1 && RoomInput.text.Length >= 4 && !moving && int.TryParse(PlayersInput.text, out DataTransfer.Instance.players) && PlayersInput.text.Length >= 1) 
        {
            CreateBtn.SetActive(true);
            JoinBtn.SetActive(true);
        }
        else
        {
            CreateBtn.SetActive(false);
            JoinBtn.SetActive(false);
        }
    }

    public void LeftArrow()
    {
        firstOption = true;
        LeftBtn.SetActive(false);
        moving = true;
    }

    public void RightArrow()
    {
        firstOption = false;
        RightBtn.SetActive(false);
        moving = true;
    }

    public void CreateGame()
    {
        PhotonNetwork.playerName = UsernameInput.text;
        PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions() { MaxPlayers = 9}, null);
    }

    public void JoinGame()
    {
        PhotonNetwork.playerName = UsernameInput.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 9;
        PhotonNetwork.JoinOrCreateRoom(RoomInput.text, roomOptions, TypedLobby.Default);
    }

    private void OnJoinedRoom()
    {
        DataTransfer.Instance.option = firstOption;
        PhotonNetwork.LoadLevel("EscapeRoom");
    }

}
