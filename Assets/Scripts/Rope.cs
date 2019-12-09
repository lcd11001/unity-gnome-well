using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject ropeSegmentPrefab;
    List<GameObject> ropeSegments = new List<GameObject>();

    public bool isIncreasing { get; set; }
    public bool isDecreasing { get; set; }

    [Tooltip("the rigidbody object that the end of the rope should be attached to")]
    public Rigidbody2D connectedObject;

    public float maxRopeSegmentLength = 1.0f;
    public float ropeSpeed = 4.0f;
    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ResetLength();
    }

    public void ResetLength()
    {
        foreach(GameObject segment in ropeSegments)
        {
            Destroy(segment);
        }

        ropeSegments = new List<GameObject>();

        isDecreasing = false;
        isIncreasing = false;

        CreateRopeSegment();
    }

    void CreateRopeSegment()
    {
        GameObject segment = (GameObject)Instantiate(ropeSegmentPrefab, this.transform.position, Quaternion.identity);

        segment.transform.SetParent(this.transform, true);

        Rigidbody2D segmentBody = segment.GetComponent<Rigidbody2D>();
        SpringJoint2D segmentJoint = segment.GetComponent<SpringJoint2D>();

        if (segmentBody == null || segmentJoint == null)
        {
            Debug.LogError("Rope segment body prefab has no Rigidbody2D and/or SpringJoint2D");
            return;
        }

        ropeSegments.Insert(0, segment);

        if (ropeSegments.Count == 1)
        {
            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();
            connectedObjectJoint.connectedBody = segmentBody;
            connectedObjectJoint.distance = 0.1f;

            segmentJoint.distance = maxRopeSegmentLength;
        }
        else
        {
            GameObject nextSegment = ropeSegments[1];
            SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();
            nextSegmentJoint.connectedBody = segmentBody;

            segmentJoint.distance = 0.0f;
        }

        segmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();
    }

    void RemoveRopeSegment()
    {
        if (ropeSegments.Count < 2)
        {
            return;
        }

        GameObject topSegment = ropeSegments[0];
        GameObject nextSegment = ropeSegments[1];

        SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();
        nextSegmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();

        ropeSegments.RemoveAt(0);
        Destroy(topSegment);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
