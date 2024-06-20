using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetMenuController : MonoBehaviour
{
    public GameObject menu;
    public GameObject controls;

    private CameraPlayerController cam;
    private bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMenu();

        if (!done)
        {
            SetUpSlider();
        }
    }

    private void ChangeMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.GetActive() || controls.GetActive())
            {
                menu.SetActive(false);
                controls.SetActive(false);
                cam.enabled = true;
            }
            else
            {
                menu.SetActive(true);
                cam.enabled = false;
            }

            if (menu.GetActive())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void SetUpSlider()
    {
        cam = FindObjectOfType<CameraPlayerController>();

        if (cam != null)
        {
            Slider slider = menu.GetComponentInChildren<Slider>();

            slider.onValueChanged.AddListener(cam.ChangeSen);
        }
    }

    public void ChangeControlMenu()
    {
        menu.SetActive(false);
        controls.SetActive(true);
    }

    public void ReturnSettingsMenu()
    {
        controls.SetActive(false);
        menu.SetActive(true);
    }

    public void ExitGame()
    {
        DataTransfer.Instance.success = false;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("GameOver");
    }
}
