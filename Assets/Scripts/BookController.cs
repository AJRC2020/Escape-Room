using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookController : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public GameObject locker;
    public bool isUsingLocker = true;

    private Transform currentRotating;
    private bool rotPos = true;
    private int currentIndex = 0;
    private bool canMove = false;
    private bool cantMoveRight = false;
    private bool cantMoveLeft = true;
    private bool notLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        currentRotating = transform.GetChild(0);
        ActivatePages();
        notLocked = !isUsingLocker;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            RotateBook();
            Check();
        }

        if (notLocked)
        {
            ControlPagesOnIndex(currentIndex);
        }

        if (!notLocked && isUsingLocker)
        {
            CheckLocker();
        }
    }

    [PunRPC]
    public void CanMove(bool isPos)
    {
        if (notLocked)
        {
            if (!canMove)
            {
                if (isPos && !rotPos && !cantMoveRight)
                {
                    if (!cantMoveLeft)
                    {
                        currentIndex++;
                        currentRotating = transform.GetChild(currentIndex);
                    }
                    rotationSpeed *= -1;
                    rotPos = true;
                }

                if (!isPos && rotPos && !cantMoveLeft)
                {
                    if (!cantMoveRight)
                    {
                        currentIndex--;
                        currentRotating = transform.GetChild(currentIndex);
                    }
                    rotationSpeed *= -1;
                    rotPos = false;
                }

                if (!(cantMoveLeft && !isPos || cantMoveRight && isPos))
                {
                    cantMoveLeft = false;
                    cantMoveRight = false;
                    canMove = true;
                }
            }
        }
    }

    private void RotateBook()
    {
        currentRotating.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        float angle = currentRotating.localEulerAngles.z * Mathf.Deg2Rad;

        float x = 0.0f;
        float y = 0.0f;

        if (currentIndex == 0)
        {
            x = 0.5f * Mathf.Cos(angle) - 0.1f * Mathf.Sin(angle) - 0.5f;
            y = 0.5f * Mathf.Sin(angle) + 0.1f * Mathf.Cos(angle) + 0.1f;
        }
        else if (currentIndex == transform.childCount - 1)
        {
            x = 0.5f * Mathf.Cos(angle) + 0.1f * Mathf.Sin(angle) - 0.5f;
            y = 0.5f * Mathf.Sin(angle) - 0.1f * Mathf.Cos(angle) + 0.1f;
        }
        else
        {
            x = 0.5f * Mathf.Cos(angle) - 0.5f;
            y = 0.5f * Mathf.Sin(angle) + 0.1f;
        }

        currentRotating.localPosition = new Vector3(x, y, 0);
    }

    private void Check()
    {
        if (rotPos)
        {
            if (currentRotating.localEulerAngles.z > 180)
            {
                currentRotating.localEulerAngles = new Vector3(currentRotating.localEulerAngles.x, currentRotating.localEulerAngles.y, 180);
                canMove = false;

                if (currentIndex + 1 == transform.childCount)
                {
                    cantMoveRight = true;
                }
                else
                {
                    currentIndex++;
                    currentRotating = transform.GetChild(currentIndex);
                }
            }
        }
        else
        {
            if (currentRotating.localEulerAngles.z > 340f)
            {
                currentRotating.localEulerAngles = new Vector3(currentRotating.localEulerAngles.x, currentRotating.localEulerAngles.y, 0);
                canMove = false;

                if (currentIndex == 0)
                {
                    cantMoveLeft = true;
                }
                else
                {
                    currentIndex--;
                    currentRotating = transform.GetChild(currentIndex);
                }
            }
        }
    }

    private void ActivatePages()
    {
        switch(currentIndex)
        {
            case 0:
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                transform.GetChild(4).gameObject.SetActive(false);
                break;

            case 1:
                transform.GetChild(1).gameObject.SetActive(true);
                if (canMove && rotPos)
                {
                    transform.GetChild(2).gameObject.SetActive(true);
                }
                transform.GetChild(3).gameObject.SetActive(false);
                transform.GetChild(4).gameObject.SetActive(false);
                break;

            case 2:
                if (canMove && !rotPos)
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                }
                transform.GetChild(2).gameObject.SetActive(true);
                if (canMove && rotPos)
                {
                    transform.GetChild(3).gameObject.SetActive(true);
                }
                transform.GetChild(4).gameObject.SetActive(false);
                break;

            case 3:
                transform.GetChild(1).gameObject.SetActive(false);
                if (canMove && !rotPos)
                {
                    transform.GetChild(2).gameObject.SetActive(true);
                }
                transform.GetChild(3).gameObject.SetActive(true);
                if (canMove && rotPos)
                {
                    transform.GetChild(4).gameObject.SetActive(true);
                }
                break;

            case 4:
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                if (canMove && !rotPos)
                {
                    transform.GetChild(3).gameObject.SetActive(true);
                }
                transform.GetChild(4).gameObject.SetActive(true);
                break;

            case 5:
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                transform.GetChild(4).gameObject.SetActive(true);
                break;
        }
    }

    private void ControlPagesOnIndex(int index)
    {
        for (int k = 1; k < transform.childCount - 1; k++)
        {
            if (k == index - 1)
            {
                if ((canMove && !rotPos) || index == transform.childCount - 1)
                {
                    transform.GetChild(k).gameObject.SetActive(true);
                }
            }
            else if (k == index)
            {
                transform.GetChild(k).gameObject.SetActive(true);
            }
            else if (k == index + 1)
            {
                if ((canMove && rotPos) || index == 0)
                {
                    transform.GetChild(k).gameObject.SetActive(true);
                }
            }
            else
            {
                transform.GetChild(k).gameObject.SetActive(false);
            }
        }
    }

    private void CheckLocker()
    {
        if (locker == null)
        {
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
            notLocked = true;
        }
    }
}
