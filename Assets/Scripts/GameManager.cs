using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject SceneCamera;

    public void Awake()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, this.transform.position, Quaternion.identity, 0);

        SceneCamera.SetActive(false);
    }
}
