using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using TMPro;

public class AssociationPuzzleController : MonoBehaviour
{
    public List<AssociationPanelController> panels = new List<AssociationPanelController>();
    public List<GameObject> icons = new List<GameObject>();
    public float waitDuration = 0.1f;
    public float answerDuration = 0.5f;
    public Transform progressBar;

    private List<string> connections = new List<string>();
    private List<bool> found = Enumerable.Repeat(false, 8).ToList();
    private float elapsedTime = 0.0f;
    private int state = 0;
    private int increment = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateList();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgressBar();

        StateMachine();
    }

    private void StateMachine()
    {
        switch (state)
        {
            case 0:
                if (CheckPanels(false))
                {
                    state = 1;
                }
                break;

            case 1:
                elapsedTime += Time.deltaTime;
                if (CheckPanels(true))
                {
                    elapsedTime = 0.0f;
                    state = 2;
                }
                if (elapsedTime > waitDuration)
                {
                    elapsedTime = 0.0f;
                    state = 3;
                }
                break;

            case 2:
                int answer = CheckAnswer();

                switch (answer)
                {
                    case -1:
                        icons[0].SetActive(true);
                        break;

                    case 0:
                        icons[3].SetActive(true);
                        break;

                    case 1:
                        icons[1].SetActive(true);
                        break;
                }

                state = 4;
                break;

            case 3:
                icons[2].SetActive(true);
                state = 4;
                break;

            case 4:
                elapsedTime += Time.deltaTime;

                if (elapsedTime > answerDuration)
                {
                    foreach (GameObject icon in icons)
                    {
                        icon.SetActive(false);
                    }

                    panels[0].DropWord();
                    panels[1].DropWord();

                    elapsedTime = 0.0f;

                    state = 0;
                }

                break;
        }
    }

    private bool CheckPanels(bool both)
    {
        if (both)
        {
            return panels[0].GetWord() != "" && panels[1].GetWord() != "";
        }
        else
        {
            return panels[0].GetWord() != "" || panels[1].GetWord() != "";
        }
    }

    private int CheckAnswer()
    {
        string word1 = panels[0].GetWord();
        string word2 = panels[1].GetWord();

        string option1 = word1 + " " + word2;
        string option2 = word2 + " " + word1;

        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == option1 || connections[i] == option2)
            {
                if (!found[i])
                {
                    found[i] = true;
                    increment++;
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        return -1;
    }

    private void CreateList()
    {
        string filepath = "Assets/Texts/connections.txt";

        foreach (string line in File.ReadLines(filepath))
        {
            connections.Add(line);
        }
    }

    private void UpdateProgressBar() 
    {
        progressBar.localScale = new Vector3(increment / 8f, progressBar.localScale.y, progressBar.localScale.z);

        if (progressBar.localScale.x == 1f) 
        {
            Debug.Log("Finished");
        }
    }
}
