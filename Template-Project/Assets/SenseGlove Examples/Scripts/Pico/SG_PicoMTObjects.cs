using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

/*
 * Using reference code from Pico XR, this script updates (up to) two Pico Trackers for a left and right hand tracking, for use with SenseGlove Hand Tracking.
 * https://developer.picoxr.com/document/unity/object-tracking/
 */

/// <summary> A script that animates two GameObjects based on Pico's Motion Tracker API. For use in Wrist Tracking for SenseGlove. </summary>
public class SG_PicoMTObjects : MonoBehaviour
{
    [Header("Amount of motion trackers used")]
    [Range(1, 2)] public int amountTrackers = 2;

    [Header("Left Wrist Tracking")]
    public Transform leftHandTrackingDevice;

    [Header("Right Wrist Tracking")]
    public Transform rightHandTrackingDevice;

    [Header("--- Debug ---")]
    public bool debug = false;
    public TextMesh debugText;
    private string debugString = "";

    // private vars
    private bool updateOT = true;
    private int objectTrackersMaxNum = 1;
    private int DeviceCount = 0;
    private List<long> trackerIds = new List<long>();
    private List<Transform> trackers = new List<Transform>();


    void Start()
    {
        // set the debug text active or not on the setting
        debugText.gameObject.SetActive(debug ? true : false);

        // set the object inside the xrrig so it will move with the rig
        this.transform.parent = Camera.main.transform.parent;
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;

        // place the tracking device gameobject into the list
        trackers.Add(leftHandTrackingDevice);
        trackers.Add(rightHandTrackingDevice);

        PXR_MotionTracking.RequestMotionTrackerCompleteAction += RequestMotionTrackerComplete;

        int res = PXR_MotionTracking.CheckMotionTrackerNumber(amountTrackers == 1 ? MotionTrackerNum.ONE : MotionTrackerNum.TWO);
    }

    private void RequestMotionTrackerComplete(RequestMotionTrackerCompleteEventData obj)
    {
        DeviceCount = (int)obj.trackerCount;
        for (int i = 0; i < DeviceCount; i++)
        {
            trackerIds.Add(obj.trackerIds[i]);
        }

        updateOT = true;

        // -- debug
        debugString = debugString + "\n" + "DeviceCount: " + DeviceCount;
        if(debugText != null) 
            debugText.text = debugString;
        // --
    }

    void Update()
    {
#if UNITY_ANDROID

        for (int i = 0; i < objectTrackersMaxNum; i++)
        {
            var obj = trackers[i];
            if (obj)
            {
                obj.localScale = Vector3.zero;
            }
        }

        // Update motiontrackers pose.
        if (updateOT)
        {
            MotionTrackerLocation location = new MotionTrackerLocation();
            for (int i = 0; i < trackerIds.Count; i++)
            {
                bool isValidPose = false;
                int result = PXR_MotionTracking.GetMotionTrackerLocation(trackerIds[i], ref location, ref isValidPose);

                // if the return is successful
                if (result == 0)
                {
                    var obj = trackers[i];
                    if (obj)
                    {
                        obj.localPosition = location.pose.Position.ToVector3();
                        obj.localRotation = location.pose.Orientation.ToQuat();
                        obj.localScale = Vector3.one * 0.1f;
                    }
                }
            }
        }
#endif
    }
}
