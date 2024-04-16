using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialManager : MonoBehaviour
{
    public List<Material> materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {        
        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView.isMine)
        {
            int index = Random.Range(0, materials.Count);

            Renderer renderer = GetComponent<Renderer>();

            renderer.material = materials[index];

            photonView.RPC("AlterMaterial", PhotonTargets.OthersBuffered, index);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void AlterMaterial(int index)
    {
        Renderer renderer = GetComponent<Renderer>();

        renderer.material = materials[index];
    }
}
