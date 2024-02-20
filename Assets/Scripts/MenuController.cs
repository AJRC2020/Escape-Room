using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject StartMenu;

    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private TMP_InputField RoomInput;

    [SerializeField] private GameObject CreateBtn;
    [SerializeField] private GameObject JoinBtn;


    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void EnableButtons()
    {
        if (UsernameInput.text.Length >= 1 && RoomInput.text.Length >= 4) 
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

    public void CreateGame()
    {
        PhotonNetwork.playerName = UsernameInput.text;
        PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions() { maxPlayers = 4}, null);
    }

    public void JoinGame()
    {
        PhotonNetwork.playerName = UsernameInput.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(RoomInput.text, roomOptions, TypedLobby.Default);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("EscapeRoom");
    }

}
