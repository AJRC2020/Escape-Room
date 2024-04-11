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
}
