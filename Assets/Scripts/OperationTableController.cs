using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationTableController : MonoBehaviour
{
    public GameObject pincers;
    public GameObject knife;
    public GameObject stethoscope;
    public GameObject ailment;
    public GameObject controlPanel;
    public GameObject cross;
    public Image soundLevel;
    public TimerController timer;
    public float distanceMultiplier = 2.5f;
    public float distanceDivider = 2f;
    public float biggerRadius = 0.3f;
    public float minDist = 0.1f;
    public float radius = 0.1f;

    private PhotonView photonView;
    private bool isPlaying = false;
    private int state = 0;
    private Vector3 contactPoint;
    private float scaleFactor = 0.0f;
    private bool stop = true;
    private Vector3 originalPos;
    private LineRenderer lineRenderer;
    private Vector3 prePos;
    private bool dialoguePlayed = false;
    private bool cutDialogue = false;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        originalPos = ailment.transform.position;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            switch (state)
            {
                case 0:
                    UpdateUI();
                    if (CheckFoundAilment())
                    {
                        photonView.RPC("ChangeState", PhotonTargets.AllBuffered, 1);
                        stop = false;
                        controlPanel.SetActive(false);
                        photonView.RPC("RevealObjects", PhotonTargets.AllBuffered, true);
                    }
                    break;

                case 1:
                    if (stop)
                    {
                        photonView.RPC("ChangeState", PhotonTargets.AllBuffered, 2);
                        photonView.RPC("RevealObjects", PhotonTargets.AllBuffered, false);
                    }
                    break;

                case 2:
                    if (CheckAilment())
                    {
                        photonView.RPC("ChangeState", PhotonTargets.AllBuffered, 3);
                    }
                    break;

                case 3:
                    if (!dialoguePlayed)
                    {
                        PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();

                        if (photonViewDialogue.isMine)
                        {
                            photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "operation");
                        }

                        dialoguePlayed = true;
                    }
                    break;
            }
        }
    }

    [PunRPC]
    public void ChangeMinigameState()
    {
        if (stethoscope.GetActive() && knife.GetActive() && pincers.GetActive())
        {
            isPlaying = !isPlaying;
            if (state == 0)
            {
                controlPanel.SetActive(true);
            }
        }
    }

    [PunRPC]
    public void RevealObjects(bool first)
    {
        if (first)
        {
            cross.SetActive(true);
        }
        else
        {
            cross.SetActive(false);
            ailment.SetActive(true);
        }
    }

    [PunRPC]
    public void ChangeState(int state)
    {
        this.state = state;
    }

    [PunRPC]
    public void Draw(Vector3 currentPos)
    {
        if (!stop)
        {
            if (Vector3.Distance(currentPos, prePos) > minDist || lineRenderer.positionCount == 0)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
                prePos = currentPos;
            }
        }
    }

    [PunRPC]
    public void StopDrawing()
    {
        CheckCut();
        lineRenderer.positionCount = 0;
        prePos = transform.position;
    }

    public void SetContactPoint(Vector3 point)
    {
        contactPoint = point;
    }

    public int GetState()
    {
        return state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Stethoscope" && pincers.GetActive() && collision.gameObject.layer == 3)
        {
            stethoscope.SetActive(true);

            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.name == "Knife" && collision.gameObject.layer == 3)
        {
            knife.SetActive(true);

            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.name == "Pincers" && collision.gameObject.layer == 3)
        {
            pincers.SetActive(true);

            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(collision.gameObject);
            }
        }
    }

    private void UpdateUI()
    {
        if (contactPoint != null)
        {
            float distance = Vector3.Distance(contactPoint, ailment.transform.position);

            scaleFactor = 0.6f / (distance * distanceMultiplier);

            if (scaleFactor > 0.6f)
            {
                scaleFactor = 0.6f;
            }

            Color imageColor = Color.Lerp(Color.red, Color.blue, distance / distanceDivider);
            soundLevel.color = imageColor;

            soundLevel.transform.localScale = new Vector3(soundLevel.transform.localScale.x, scaleFactor, soundLevel.transform.localScale.z);
        }
    }

    private bool CheckFoundAilment()
    {
        return scaleFactor == 0.6f && Input.GetMouseButtonUp(0);
    }

    private bool CheckAilment()
    {
        if (ailment.transform.parent != transform)
        {
            float distance = Vector3.Distance(ailment.transform.position, originalPos);

            if (distance > biggerRadius)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private void CheckCut()
    {
        Vector3[] points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);

        float totalLength = 0f;
        bool stop = false;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(points[i], ailment.transform.position);

            if (i != 0)
            {
                totalLength += Vector3.Distance(points[i], points[i - 1]);
            }

            if (distance <= radius)
            {
                stop = true;
            }
        }

        if (totalLength >= 0.2f && totalLength <= 0.5f)
        {
            if (stop)
            {
                this.stop = true;
            }
            else
            {
                PhotonView photonViewTimer = timer.GetPhotonView();
                photonViewTimer.RPC("Penalty", PhotonTargets.AllBuffered, 1);

                if (!cutDialogue)
                {
                    PhotonView photonViewDialogue = DialogueManager.Instance.GetPhotonView();

                    if (photonViewDialogue.isMine)
                    {
                        photonViewDialogue.RPC("PlayDialogue", PhotonTargets.AllBuffered, "cut");
                    }

                    cutDialogue = true;
                }
            }
        }
    }
}
