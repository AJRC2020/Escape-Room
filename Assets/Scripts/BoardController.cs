using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private float minDist = 0.1f;
    [SerializeField]
    private float offset = 0.25f;
    [SerializeField]
    private List<Bounds> bounds = new List<Bounds>();

    private LineRenderer lineRenderer;
    private Vector3 prePos;
    private List<bool> found = Enumerable.Repeat(false, 15).ToList();
    private Dictionary<int, int> wordToLight = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        prePos = transform.position;
        CreateBounds();
        wordToLight[0] = 10;
        wordToLight[1] = 3;
        wordToLight[2] = 9;
        wordToLight[3] = 5;
        wordToLight[4] = 4;
        wordToLight[5] = 13;
        wordToLight[6] = 14;
        wordToLight[7] = 2;
        wordToLight[8] = 6;
        wordToLight[9] = 0;
        wordToLight[10] = 8;
        wordToLight[11] = 7;
        wordToLight[12] = 12;
        wordToLight[13] = 11;
        wordToLight[14] = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (CountFound() == 15)
        {
            Debug.Log("Finished");
        }
    }

    [PunRPC]
    public void Draw(Vector3 currentPos)
    {
        if (Vector3.Distance(currentPos, prePos) > minDist || lineRenderer.positionCount == 0)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
            prePos = currentPos;
        }
    }

    [PunRPC]
    public void StopDrawing()
    {
        AnalyzeLine();
        lineRenderer.positionCount = 0;
        prePos = transform.position;
    }

    private void AnalyzeLine()
    {
        Vector3[] points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);

        Bounds boundsDrawned = new Bounds(points[0], Vector3.zero);

        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            boundsDrawned.Encapsulate(points[i]);
        }

        Debug.Log("Bounds = " + boundsDrawned);

        Bounds diagonalBound = new Bounds(boundsDrawned.center, Vector3.zero);

        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            diagonalBound.Encapsulate(RotatePoint(points[i], boundsDrawned.center));
        }

        Debug.Log("Diagonal = " +  diagonalBound);

        int index = GetMatchIndex(boundsDrawned, diagonalBound);

        if (index != -1)
        {
            found[index] = true;
            ActivateLight(index);
        }
    }

    private bool CompareBounds(Bounds bound1, Bounds bound2)
    {
        float dist = Vector3.Distance(bound1.center, bound2.center);
        float diffZ = Mathf.Abs(bound1.extents.z - bound2.extents.z) / bound1.extents.z;
        float diffY = Mathf.Abs(bound1.extents.y - bound2.extents.y) / bound1.extents.y;

        return dist <= offset && diffZ <= offset && diffY <= offset;
    }

    private Vector3 RotatePoint(Vector3 point, Vector3 center)
    {
        float sinAngle = Mathf.Sin(35f * Mathf.Deg2Rad);
        float cosAngle = Mathf.Cos(35f * Mathf.Deg2Rad);

        Vector3 transPoint = point - center;

        float rotatedY = transPoint.z * cosAngle - transPoint.y * sinAngle;
        float rotatedZ = transPoint.z * sinAngle + transPoint.y * cosAngle;

        Vector3 rotatedPoint = new Vector3(0, rotatedY, rotatedZ) + center;
        return rotatedPoint;
    }

    private int GetMatchIndex(Bounds boundsDrawned, Bounds diagonalBounds)
    {
        for (int i = 0; i < bounds.Count; i++)
        {
            if (i < bounds.Count - 3)
            {
                if (CompareBounds(boundsDrawned, bounds[i]))
                {
                    return i;
                }
            }
            else
            {
                if (CompareBounds(diagonalBounds, bounds[i]))
                {
                    return i;
                }
            }
        }

        return -1;
    }
    private void CreateBounds()
    {
        Transform childTrans = transform.GetChild(0); 

        for (int i = 0; i < childTrans.childCount; i++)
        {
            Transform child = childTrans.GetChild(i);
            Bounds newBound = new Bounds();

            newBound.center = new Vector3(child.position.x - 0.01f, child.position.y, child.position.z);
            newBound.extents = new Vector3(0, child.localScale.z * 1.25f, child.localScale.x * 2);

            if (i >= childTrans.childCount - 3)
            {
                newBound.extents = new Vector3(0, newBound.extents.y * 1.6f, newBound.extents.z * Mathf.Cos(35f * Mathf.Deg2Rad));
            }

            bounds.Add(newBound);
        }
    }

    private int CountFound()
    {
        int foundCount = 0;

        foreach (bool answer in found)
        {
            if (answer)
            {
                foundCount++;
            }
        }

        return foundCount;
    }

    private void ActivateLight(int index)
    {
        Transform childTrans = transform.GetChild(1);

        GameObject light = childTrans.GetChild(wordToLight[index]).gameObject;

        light.SetActive(true);
    }
}
