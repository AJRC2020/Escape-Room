using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class SpecialBookShelfController : MonoBehaviour
{
    public GameObject book;

    private List<MoveObjectController> books = new List<MoveObjectController>();

    private List<bool> answer = new List<bool> {
        false, true, true, true, false,
        true, false, true, true, false,
        false, false, false, true, false,
        true, true, false, false, false,
        false, true, false, true, true
    };

    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) 
        {
            Transform child = transform.GetChild(i);
            for (int j = 0; j < child.childCount; j++)
            {
                MoveObjectController book = child.GetChild(j).GetComponent<MoveObjectController>();
                if (book != null)
                {
                    books.Add(book);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CompareWithAnswer() && !stop && !GetComponent<MoveObjectController>().isRight())
        {
            stop = true;
            if (GetComponent<PhotonView>().isMine)
            {
                PhotonNetwork.Instantiate(book.name, new Vector3(12.75f, 0.5f, 13.75f), Quaternion.identity, 0);
            }
        }
    }

    private bool CompareWithAnswer()
    {
        for (int i = 0; i < answer.Count; i++)
        {
            if (answer[i] != books[i].isRight())
            {
                return false;
            }
        }

        return true;
    }
}
