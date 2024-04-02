using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private float minDist = 0.1f;
    [SerializeField]
    private List<Bounds> bounds = new List<Bounds>();
    [SerializeField]
    private float offset = 0.25f;

    private LineRenderer lineRenderer;
    private Vector3 prePos;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        prePos = transform.position;
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

        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            boundsDrawned.Encapsulate(points[i]);
        }

        int index = CompareBounds(boundsDrawned);

        Debug.Log("Bound = " + boundsDrawned);

        if (index == -1)
        {
            Debug.Log("Got No Word");
        }
        else
        {
            Debug.Log("Got Word " + index);
        }
    }

    private int CompareBounds(Bounds boundsDrawned)
    {
        for (int i = 0; i < bounds.Count; i++)
        {
            float dist = Vector3.Distance(bounds[i].center, boundsDrawned.center);
            float diffX = Mathf.Abs(bounds[i].extents.x - boundsDrawned.extents.x) / bounds[i].extents.x;
            float diffY = Mathf.Abs(bounds[i].extents.y - boundsDrawned.extents.y) / bounds[i].extents.y;

            if (dist <= offset && diffX <= offset && diffY <= offset)
            {
                return i;
            }
        }

        return -1;
    }
}
