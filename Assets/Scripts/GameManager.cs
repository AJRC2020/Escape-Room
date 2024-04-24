using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject FirstOptionPrefab;
    public GameObject SecondOptionPrefab;
    public GameObject SceneCamera;

    public void Awake()
    {
        if (DataTransfer.Instance.option)
        {
            PhotonNetwork.Instantiate(FirstOptionPrefab.name, this.transform.position, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate(SecondOptionPrefab.name, this.transform.position, Quaternion.identity, 0);
        }

        SceneCamera.SetActive(false);    
    }
}
