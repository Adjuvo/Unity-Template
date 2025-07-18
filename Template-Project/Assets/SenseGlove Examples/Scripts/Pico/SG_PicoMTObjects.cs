using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.XR.ARFoundation;

/*
 * Using reference code from Pico XR, this script updates (up to) two Pico Trackers for a left and right hand tracking, for use with SenseGlove Hand Tracking.
 * https://developer.picoxr.com/document/unity/object-tracking/
 *
 * author: max@senseglove.com
 */

/// <summary> A script that animates two GameObjects based on Pico's Motion Tracker API. For use in Wrist Tracking for SenseGlove. </summary>
public class SG_PicoMTObjects : MonoBehaviour
{

    //---------------------------------------------------------------------------------------------
    // Variables

    /// <summary> If this variable is assigned, we will move the TrackingDevice's relative to this one. Otherwise, we set the LocalPostion instead. </summary>
    public Transform xrOrigin;

    [Header("Left Wrist Tracking")]
    public Transform leftHandTrackingDevice;
    public MotionTrackerNum trackerIndexLeftHand = MotionTrackerNum.ONE;

    [Header("Right Wrist Tracking")]
    public Transform rightHandTrackingDevice;
    public MotionTrackerNum trackerIndexRightHand = MotionTrackerNum.TWO;

    //---------------------------------------------------------------------------------------------
    // Functions

    public void UpdateTrackerLocations()
    {
        MotionTrackerMode trackingMode = PXR_MotionTracking.GetMotionTrackerMode();
        if (trackingMode == MotionTrackerMode.MotionTracking)
        {
            MotionTrackerConnectState mtConnectStates = new MotionTrackerConnectState();
            PXR_MotionTracking.GetMotionTrackerConnectStateWithSN(ref mtConnectStates);
            if (mtConnectStates.trackerSum > 0) //there is at least 1 tracker
            {
                UpdateTransform(leftHandTrackingDevice, trackerIndexLeftHand, xrOrigin, ref mtConnectStates);
                UpdateTransform(rightHandTrackingDevice, trackerIndexRightHand, xrOrigin, ref mtConnectStates);
            }
        }
    }

    public static void UpdateTransform(Transform obj, MotionTrackerNum trackerNum, Transform xrOrigin, ref MotionTrackerConnectState mtConnectState)
    {
        if (trackerNum == MotionTrackerNum.NONE)
            return;
        if (obj == null)
            return;
        int trackingIndex = ToTrackingIndex(trackerNum);
        if (trackingIndex < 0 || trackingIndex >= mtConnectState.trackerSum)
            return; //In case there's less trackers or ToTrackingIndex somewhow still returns an invalid index

        string sn = mtConnectState.trackersSN[trackingIndex].value.ToString().Trim();
        if (string.IsNullOrEmpty(sn))
            return;

        MotionTrackerLocations locations = new MotionTrackerLocations();
        MotionTrackerConfidence confidence = new MotionTrackerConfidence();
        // Retrieve the location of PICO Motion Tracker
        if (PXR_MotionTracking.GetMotionTrackerLocations(mtConnectState.trackersSN[trackingIndex], ref locations, ref confidence) == 1)
            return; //if this function returns 1, it was a failure. (A bit intuitive because it's usualy 0 = false, 1 = true)

        MotionTrackerLocation location = locations.localLocation; //This is relative to the Headset, but not to the player...
        Vector3 localPosition = location.pose.Position.ToVector3();
        Quaternion localRotation = location.pose.Orientation.ToQuat();
        if (xrOrigin == null)
        {
            obj.localRotation = localRotation;
            obj.localPosition = localPosition;
        }
        else
        {
            obj.localRotation = xrOrigin.rotation * localRotation;
            obj.localPosition = xrOrigin.position + (xrOrigin.rotation * localPosition);
        }
    }


    public static int ToTrackingIndex(MotionTrackerNum num)
    {
        return ((int)num) - 1;
    }


    //---------------------------------------------------------------------------------------------
    // Monobehaviour


    private void Start()
    {
        // commended out because it is not possible to press virtual buttons with gloves on
        MotionTrackerMode trackingMode = PXR_MotionTracking.GetMotionTrackerMode();
        if (trackerIndexLeftHand != MotionTrackerNum.NONE && SceneManager.GetActiveScene().buildIndex == 0 && trackingMode != MotionTrackerMode.MotionTracking)
        {
            PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.MotionTracking, trackerIndexLeftHand);
        }
        if (trackerIndexRightHand != MotionTrackerNum.NONE && SceneManager.GetActiveScene().buildIndex == 0 && trackingMode != MotionTrackerMode.MotionTracking)
        {
            PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.MotionTracking, trackerIndexRightHand);
        }
    }

    private void Update()
    {
        UpdateTrackerLocations();
    }
}
