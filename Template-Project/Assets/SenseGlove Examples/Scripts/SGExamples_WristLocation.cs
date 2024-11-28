using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;

public class SGExamples_WristLocation : MonoBehaviour
{
    [Header("Place the SG_Trackedhand script from the SGHandRight or left")]
    public SG_TrackedHand handLeft;
    public SG_TrackedHand handRight;

    private GameObject wristLeft;
    private GameObject wristRight;

    private void Start()
    {
        // find the wrist
        if(handLeft != null)
            wristLeft = handLeft.GetTransform(SG_TrackedHand.TrackingLevel.RenderPose, HandJoint.Wrist).gameObject;
        if (handRight != null)
            wristRight = handRight.GetTransform(SG_TrackedHand.TrackingLevel.RenderPose, HandJoint.Wrist).gameObject;
    }

    private void Update()
    {
        if (handRight != null && handLeft != null)
            Debug.Log("Left hand: " + wristLeft.transform.position + " || Right hand: " + wristRight.transform.position);
    }
}
