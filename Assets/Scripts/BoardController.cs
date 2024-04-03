using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        prePos = transform.position;
        CreateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        Debug.Log("First Point = " + points[0]);

        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            boundsDrawned.Encapsulate(points[i]);
        }

        Bounds diagonalBound = new Bounds(boundsDrawned.center, Vector3.zero);

        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            diagonalBound.Encapsulate(RotatePoint(points[i], boundsDrawned.center));
        }

        int index = GetMatchIndex(boundsDrawned, diagonalBound);

        Debug.Log("Bound = " + boundsDrawned);
        Debug.Log("Diagonal Bound = " + diagonalBound);

        if (index == -1)
        {
            Debug.Log("Got No Word");
        }
        else
        {
            Debug.Log("Got Word " + index);
        }
    }

    private bool CompareBounds(Bounds bound1, Bounds bound2)
    {
        float dist = Vector3.Distance(bound1.center, bound2.center);
        float diffX = Mathf.Abs(bound1.extents.x - bound2.extents.x) / bound1.extents.x;
        float diffY = Mathf.Abs(bound1.extents.y - bound2.extents.y) / bound1.extents.y;

        return dist <= offset && diffX <= offset && diffY <= offset;
    }

    private Vector3 RotatePoint(Vector3 point, Vector3 center)
    {
        float sinAngle = Mathf.Sin(35f * Mathf.Deg2Rad);
        float cosAngle = Mathf.Cos(35f * Mathf.Deg2Rad);

        Vector3 transPoint = point - center;

        float rotatedX = transPoint.x * cosAngle - transPoint.y * sinAngle;
        float rotatedY = transPoint.x * sinAngle + transPoint.y * cosAngle;

        Vector3 rotatedPoint = new Vector3(rotatedX, rotatedY, 0) + center;
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
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Bounds newBound = new Bounds();

            newBound.center = new Vector3(child.position.x, child.position.y, -0.01f);
            newBound.extents = new Vector3(child.localScale.x * 2, child.localScale.z * 1.25f, 0);

            if (i >= transform.childCount - 3)
            {
                newBound.extents = new Vector3(newBound.extents.x * Mathf.Cos(35f * Mathf.Deg2Rad), newBound.extents.y * 1.6f, 0);
            }

            bounds.Add(newBound);
        }
    }
}
