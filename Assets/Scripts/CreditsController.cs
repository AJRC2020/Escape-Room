using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    public Transform credits;
    public float duration = 30;

    private float speed = 1.0f;
    private float limit = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        speed = credits.localPosition.y * -2 / duration;
        limit = Mathf.Abs(credits.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        credits.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);

        if (Input.GetKeyDown(KeyCode.Escape) || limit < credits.localPosition.y)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
