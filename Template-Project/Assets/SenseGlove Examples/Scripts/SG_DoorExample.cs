using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using SG.Util;

public class SG_DoorExample : SG_Grabable
{
    [Header("Game objects")]
    public GameObject door;
    public SG_Grabable doorHingeGrab;
    public GameObject handle;

    [Header("Angles of rotation")]
    public MoveAxis rotateAroundHandle = MoveAxis.Y;
    public MoveAxis rotateAroundDoor = MoveAxis.Y;

    [Header("The rotation location of the handle when the door can be opened")]
    public float doorUnlock = 25f;

    [Header("The maximum rotation location of the handle")]
    public float maxHandle = 35f;

    [Header("The maximum and minimum rotation location of the door")]
    public float doorUnlockedAngle = 1;
    public float minDoor = 0f;
    public float maxDoor = 90f;

    [Header("Speed of opening door")]
    public int openDoorSpeed = 90;

    // public vars hidden to ease the amount of data in the editor
    [HideInInspector]
    public bool unlocked = false;
    [HideInInspector]
    public int axIndexHinge;
    [HideInInspector]
    public float timer = 0f;
    [HideInInspector]
    public Vector3 doorRot = new Vector3(0, 0, 0);

    // private vars
    private Vector3 startAnglesDoor;
    private Vector3 startPositionHandle;
    private bool released = true;
    private Vector3 startAnglesHandle;
    public Vector3 handpose;
    private bool firstGrab = false;
    private float startDragging = 0f;

    protected override void Start()
    {
        base.Start();
        axIndexHinge = SG_Util.AxisIndex(rotateAroundDoor);
        startAnglesDoor = door.transform.localEulerAngles;
        startAnglesHandle = handle.transform.localEulerAngles;
        startPositionHandle = handle.transform.localPosition;
    }

    private void Update()
    {
        // check the first frame while grabbed
        if (this.IsGrabbed() && released)
        {
            startAnglesDoor = door.transform.localEulerAngles;

            SG_TrackedHand hand = grabbedBy[0].TrackedHand;
            SG_HandPose handPose = hand.GetHandPose(SG_TrackedHand.TrackingLevel.RealHandPose);

            handpose = handPose.wristPosition;

            firstGrab = true;
        }

        // check the first frame while released
        if (!this.IsGrabbed() && !released)
        {
            // is the door unlocked from the start position
            Vector3 localEulerDoor = door.transform.localEulerAngles;
            unlocked = (localEulerDoor[axIndexHinge] > doorUnlockedAngle) ? true : false;
            timer = 0;
            doorRot = door.transform.localEulerAngles;
        }

        // reset the handle to the base (start) position
        if (!this.IsGrabbed())
            ReturnHandleToStart();

        released = this.IsGrabbed() ? false : true;

        if (this.IsGrabbed())
        {
            SG_TrackedHand hand = grabbedBy[0].TrackedHand;
            SG_HandPose handPose = hand.GetHandPose(SG_TrackedHand.TrackingLevel.RealHandPose);

            handpose = handPose.wristPosition;
        }

        // run the timer at least till 5 so it won't overload
        timer = timer < 5 ? timer + Time.deltaTime : 5;

        //print(unlocked);
    }


    protected override void MoveToTargetLocation(Vector3 targetPosition, Quaternion targetRotation, float dT)
    {
        if (this.IsGrabbed())
        {
            Transform baseHandleTransform = this.MyTransform;

            baseHandleTransform.rotation = targetRotation;

            Vector3 localEulerHandle = baseHandleTransform.localEulerAngles;
            Vector3 localEulerDoor = door.transform.localEulerAngles;

            int axIndexHandle = SG_Util.AxisIndex(rotateAroundHandle);

            for (int i = 0; i < 3; i++)
            {
                if (i != axIndexHandle)
                {
                    localEulerHandle[i] = 0;
                }
                else if (i == axIndexHandle && (SG_Util.NormalizeAngle(localEulerHandle[i]) <= 0 || SG_Util.NormalizeAngle(localEulerHandle[i]) >= maxHandle))
                {
                    localEulerHandle[i] = SG_Util.NormalizeAngle(localEulerHandle[i]) <= 0 ? 0 : maxHandle;
                }

                if (i == axIndexHandle)
                {
                    if (SG_Util.NormalizeAngle(localEulerHandle[i]) >= doorUnlock || localEulerDoor[axIndexHinge] > doorUnlockedAngle)
                    {
                        if (!unlocked || localEulerDoor[axIndexHinge] > doorUnlockedAngle && firstGrab)
                        {
                            StartMovingDoor();
                            unlocked = true;
                            firstGrab = false;
                        }
                    }
                    else
                    {
                        unlocked = false;
                    }
                }

            }

            baseHandleTransform.localEulerAngles = localEulerHandle;
            baseHandleTransform.localPosition = startPositionHandle;
        }
    }

    private void ReturnHandleToStart()
    {
        handle.transform.localEulerAngles = startAnglesHandle;
    }

    private void StartMovingDoor()
    {

    }

    private void RotateDoor()
    {
        Vector3 localEulerAngles = new Vector3(0, 0, 0);
        localEulerAngles = door.transform.localEulerAngles;

        // calculate how far to open the door
        float angle = 0;
        angle = (this.transform.localPosition.z - startDragging) * openDoorSpeed;

        //print(angle + " - " + this.transform.localPosition.z + " - " + startDragging);

        // check minimum and max range
        angle = angle < minDoor ? minDoor : angle;
        angle = angle > maxDoor ? maxDoor : angle;

        // set door angle
        localEulerAngles[axIndexHinge] = angle;
        door.transform.localEulerAngles = localEulerAngles;
    }

    protected override void SetupScript()
    {
        base.SetupScript();
        if (this.physicsBody != null)
        {
            this.physicsBody.isKinematic = true;
        }
    }

    // make sure the handle and door can't been moved
    protected virtual void OnValidate()
    {
        this.moveSpeed = 0.0f;
        doorHingeGrab.moveSpeed = 0f;
    }


}
