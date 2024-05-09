using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MixerController : MonoBehaviour
{
    public GameObject key;
    public GameObject cup;
    public TextMeshPro showingSolution;
    public TextMeshPro showingRecipe;

    private List<string> answerSubstances = new List<string>();
    private List<int> answerAmounts = new List<int>();
    private List<string> currentSubstances = new List<string>();
    private List<int> currentAmounts = new List<int>();
    
    private bool firstTime = true;
    private bool stop = false;
    private string currentSubstance = "";
    private int currentAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        answerSubstances.Add("Oxygen");
        answerSubstances.Add("Hydrogen");
        answerSubstances.Add("Nitrogen");
        answerSubstances.Add("Carbon");

        answerAmounts.Add(2);
        answerAmounts.Add(10);
        answerAmounts.Add(4);
        answerAmounts.Add(8);

        showingSolution.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        CheckKey();
        CheckPressed();
        MakeRecipe();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Flask" && currentSubstance == "")
        {
            currentSubstance = collision.gameObject.name;
            ChangeActivationChilds(true);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Flask" && currentSubstance == collision.gameObject.name)
        {
            currentSubstance = "";
            ChangeActivationChilds(false);
            transform.GetChild(0).gameObject.SetActive(true);
            ResetSolution();
            stop = false;
        }
    }

    private void CheckKey()
    {
        if (key == null && firstTime)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            firstTime = false;
        }
    }

    private void ChangeActivationChilds(bool activationState)
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(activationState);
        }
    }

    private void CheckPressed()
    {
        for (int i = 3; i < transform.childCount; i++)
        {
            MonitorHelper helper = transform.GetChild(i).GetComponent<MonitorHelper>();

            if (helper.CheckPress())
            {
                helper.Unpress();
                if (i == 3)
                {
                    if (CheckSolution())
                    {
                        currentSubstances.Add(currentSubstance);
                        currentAmounts.Add(currentAmount);
                        ResetSolution();
                        stop = true;
                    }
                    else
                    {
                        ResetSolution();
                    }
                }
                else if (i == 4)
                {
                    ResetSolution();
                }
                else if (i == 5)
                {
                    if (CheckRecipe())
                    {
                        ChangeActivationChilds(false);

                        PhotonView photonView = transform.GetChild(i).GetComponent<PhotonView>();
                        if (photonView.isMine)
                        {
                            PhotonNetwork.Instantiate(cup.name, transform.position, Quaternion.identity, 0);
                        }

                        PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();

                        if (photonView.isMine)
                        {
                            photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "mixer");
                        }
                    }
                    else
                    {
                        ResetRecipe();
                    }
                }
                else if (i == 6)
                {
                    ResetRecipe();
                }
                else
                {
                    if (showingSolution.text.Length < 4 && !stop)
                    {
                        AddToAmount(i - 7);
                    }
                }
            }
        }
    }

    private void AddToAmount(int digit)
    {
        string digitChar = digit.ToString();

        showingSolution.text += digitChar;

        currentAmount *= 10;
        currentAmount += digit;
    }

    private bool CheckSolution()
    {
        return currentAmount != 0 && !currentSubstances.Contains(currentSubstance);
    }

    private bool CheckRecipe()
    {
        if (currentSubstances.Count == 0 && currentAmounts.Count == 0)
        {
            return false;
        }

        if (currentSubstances.Count != answerSubstances.Count)
        {
            return false;
        }

        for (int k = 0; k < currentSubstances.Count; k++)
        {
            int indexSubstance = answerSubstances.IndexOf(currentSubstances[k]);
            int indexAmount = answerAmounts.IndexOf(currentAmounts[k]);

            if (indexSubstance != indexAmount || indexAmount == -1 || indexSubstance == -1)
            {
                return false;
            }
        }

        return true;
    }

    private void ResetSolution()
    {
        showingSolution.text = "";
        currentAmount = 0;
    }

    private void ResetRecipe()
    {
        currentSubstances.Clear();
        currentAmounts.Clear();
    }

    private void MakeRecipe()
    {
        showingRecipe.text = "";

        for (int m = 0; m < currentSubstances.Count; m++)
        {
            string subs = currentSubstances[m];
            int amount = currentAmounts[m];

            showingRecipe.text += subs + " " + amount.ToString();

            if (m + 1 != currentSubstances.Count)
            {
                showingRecipe.text += "\n";
            }
        }
    }
}
