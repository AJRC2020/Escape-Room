using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI result;
    public TextMeshProUGUI time;

    // Start is called before the first frame update
    void Start()
    {
        if (DataTransfer.Instance.success)
        {
            result.text = "Sucesso";

            string timeTaken = DataTransfer.Instance.time;
            string timePhrase = "Completou a escape room em " + timeTaken + "!";
            time.text = timePhrase;
        }
        else
        {
            result.text = "Falhanço";
            time.text = "Melhor sorte para a próxima!";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
