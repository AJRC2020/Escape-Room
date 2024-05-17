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
            result.text = "Success";

            string timeTaken = DataTransfer.Instance.time;
            string timePhrase = "Completed the escape room in " + timeTaken + "!";
            time.text = timePhrase;
        }
        else
        {
            result.text = "Failure";
            time.text = "Better luck next time!";
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
