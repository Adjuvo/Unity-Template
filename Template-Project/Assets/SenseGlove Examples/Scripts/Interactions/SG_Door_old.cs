using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using SG.Util;

/// <summary>
/// !!!The object (doorMain) should be an empty gameobject with all values in the transform(position and rotation) to 0.
/// !!!The door should start fully closed when the scene starts.
/// </summary>

public class SG_Door_old : SG_Grabable
{
    public enum rotateAngleEnum { x, y, z }

    [Header("The door main object")]
    public GameObject doorMain;

    [Header("Rotate door around local angle")]
    public MoveAxis localRotateAngle = MoveAxis.Y;

    [Header("Max Rotation of the door")]
    public int maxTotalRotation = 70;

    [Header("Local Axis of the door that stands 90 degree towards the door")]
    public MoveAxis doorAxis = MoveAxis.X;

    [Header("Speed of opening the door")]
    public float doorSpeed = 10f;

    // private vars
    private float startAngleEuler;
    private float startAngle;
    private bool grabbed = false;
    private Vector3 startAnglesDoor = new Vector3(0, 0, 0);
    private Vector3 wristPosition = new Vector3(0, 0, 0);
    private Transform transformWrist;
    private SG_TrackedHand hand;
    
    private SG_HandPose handPose;
    private float startDistance = 0f;

    protected override void Awake()
    {
        base.Awake();

        // if the door is forgotten to be assigned
        if (doorMain == null)
            doorMain = this.transform.parent.gameObject;

        // set the start angle of the door
        switch (localRotateAngle)
        {
            case MoveAxis.X:
            case MoveAxis.NegativeX:
                startAngle = SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.x);
                startAngleEuler = doorMain.transform.localEulerAngles.x;
                break;
            case MoveAxis.Y:
            case MoveAxis.NegativeY:
                startAngle = SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.y);
                startAngleEuler = doorMain.transform.localEulerAngles.y;
                break;
            case MoveAxis.Z:
            case MoveAxis.NegativeZ:
                startAngle = SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.z);
                startAngleEuler = doorMain.transform.localEulerAngles.z;
                break;
        }

        // set private vars
        transformWrist = this.transform;
    }

    private void Update()
    {
        // -- The first frame while grabbed
        if (this.IsGrabbed() && !grabbed)
        {
            startAnglesDoor = doorMain.transform.localEulerAngles;

            // set the start distance between the wrist of the real hand position to the door
            hand = grabbedBy[0].TrackedHand;
            handPose = hand.GetHandPose(SG_TrackedHand.TrackingLevel.RealHandPose);
            transformWrist.position = handPose.wristPosition;
            startDistance = DistanceIWantToCalculate(doorMain.transform, transformWrist);
        }

        grabbed = this.IsGrabbed() ? true : false;


        // -- Door scripted rotation
        if (this.IsGrabbed())
        {
            // calculate the rotation position of the door compared to the wrist
            RotateGrabbedDoor();
        }

        // -- Door bounderies

        // make sure the door doesn't move further than the maximum and minimum positions
        BoundariesUpdate();


        // -- Door fixed positions and rotations of all parts of the door

        // make sure the door doesn't move away or use a different rotation
        TransformUpdate();

        // Set this doorknob on zero position so it won't slightly change course
        this.transform.localPosition = new Vector3(0, 0, 0);
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    // set the own transform from grabscript off
    protected override void MoveToTargetLocation(Vector3 targetPosition, Quaternion targetRotation, float dT)
    {
        // Do nothing
    }

    private void RotateGrabbedDoor()
    {
        SG_TrackedHand hand = grabbedBy[0].TrackedHand;
        float distance = 0f;
        float angle = 0f;

        handPose = hand.GetHandPose(SG_TrackedHand.TrackingLevel.RealHandPose);
        transformWrist.position = handPose.wristPosition;

        switch (localRotateAngle)
        {
            case MoveAxis.X:
            case MoveAxis.NegativeX:
                distance = DistanceIWantToCalculate(doorMain.transform, transformWrist);

                angle = distance > startDistance ? doorMain.transform.localEulerAngles.x + (doorSpeed * Time.deltaTime) : doorMain.transform.localEulerAngles.x - (doorSpeed * Time.deltaTime);

                if (distance < startDistance + 0.05 && distance > startDistance - 0.05)
                    angle = doorMain.transform.localEulerAngles.x;

                doorMain.transform.localEulerAngles = new Vector3(angle, 0, 0);
                break;
            case MoveAxis.Y:
            case MoveAxis.NegativeY:
                distance = DistanceIWantToCalculate(doorMain.transform, transformWrist);

                angle = distance > startDistance ? doorMain.transform.localEulerAngles.y + (doorSpeed * Time.deltaTime) : doorMain.transform.localEulerAngles.y - (doorSpeed * Time.deltaTime);

                if (distance < startDistance + 0.05 && distance > startDistance - 0.05)
                    angle = doorMain.transform.localEulerAngles.y;

                doorMain.transform.localEulerAngles = new Vector3(0, angle, 0);
                break;
            case MoveAxis.Z:
            case MoveAxis.NegativeZ:
                distance = DistanceIWantToCalculate(doorMain.transform, transformWrist);

                angle = distance > startDistance ? doorMain.transform.localEulerAngles.z + (doorSpeed * Time.deltaTime) : doorMain.transform.localEulerAngles.z - (doorSpeed * Time.deltaTime);

                if (distance < startDistance + 0.05 && distance > startDistance - 0.05)
                    angle = doorMain.transform.localEulerAngles.z;

                doorMain.transform.localEulerAngles = new Vector3(0, 0, angle);
                break;
        }
    }

    private float DistanceIWantToCalculate(Transform a, Transform b)
    {
        var positionOfBInAsSpace = a.InverseTransformPoint(b.position);

        switch (doorAxis)
        {
            case MoveAxis.X:
                return positionOfBInAsSpace.x;
            case MoveAxis.NegativeX:
                return -positionOfBInAsSpace.x;
            case MoveAxis.Y:
                return positionOfBInAsSpace.y;
            case MoveAxis.NegativeY:
                return -positionOfBInAsSpace.y;
            case MoveAxis.Z:
                return positionOfBInAsSpace.z;
            case MoveAxis.NegativeZ:
                return -positionOfBInAsSpace.z;
        }
        return 0;
    }

    // don't let the door open further that the boundaries set
    private void BoundariesUpdate()
    {
        // don't let the door open further that the boundaries set
        switch (localRotateAngle)
        {
            case MoveAxis.X:
                if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.x) < startAngle)
                {
                    doorMain.transform.localEulerAngles = new Vector3(startAngleEuler, 0, 0);
                }
                else if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.x) > startAngle + maxTotalRotation)
                {
                    doorMain.transform.localEulerAngles = new Vector3(startAngleEuler + maxTotalRotation, 0, 0);
                }
                break;
            case MoveAxis.NegativeX:
                if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.x) > startAngle)
                {
                    doorMain.transform.localEulerAngles = new Vector3(startAngleEuler, 0, 0);
                }
                else if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.x) < startAngle - maxTotalRotation)
                {
                    doorMain.transform.localEulerAngles = new Vector3(startAngleEuler - maxTotalRotation, 0, 0);
                }
                break;
            case MoveAxis.Y:
                if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.y) < startAngle)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, startAngleEuler, 0);
                }
                else if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.y) > startAngle + maxTotalRotation)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, startAngleEuler + maxTotalRotation, 0);
                }
                break;
            case MoveAxis.NegativeY:
                if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.y) > startAngle)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, startAngleEuler, 0);
                }
                else if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.y) < startAngle - maxTotalRotation)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, startAngleEuler - maxTotalRotation, 0);
                }
                break;
            case MoveAxis.Z:
                if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.z) < startAngle)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler);
                }
                else if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.z) > startAngle + maxTotalRotation)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler + maxTotalRotation);
                }
                break;
            case MoveAxis.NegativeZ:
                if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.z) > startAngle)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler);
                }
                else if (SG_Util.NormalizeAngle(doorMain.transform.localEulerAngles.z) < startAngle - maxTotalRotation)
                {
                    doorMain.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler - maxTotalRotation);
                }
                break;
        }
    }

    // set the position and rotation of the doorangles not selected to zero (needs to run if grabbed or not)
    private void TransformUpdate()
    {
        // set the rotation on all axes except the chosen one to zero
        switch (localRotateAngle)
        {
            case MoveAxis.X:
            case MoveAxis.NegativeX:
                if (doorMain.transform.localEulerAngles.y != 0 || doorMain.transform.localEulerAngles.z != 0)
                    doorMain.transform.localEulerAngles = new Vector3(doorMain.transform.localEulerAngles.x, 0, 0);
                break;
            case MoveAxis.Y:
            case MoveAxis.NegativeY:
                if (doorMain.transform.localEulerAngles.x != 0 || doorMain.transform.localEulerAngles.z != 0)
                    doorMain.transform.localEulerAngles = new Vector3(0, doorMain.transform.localEulerAngles.y, 0);
                break;
            case MoveAxis.Z:
            case MoveAxis.NegativeZ:
                if (doorMain.transform.localEulerAngles.x != 0 || doorMain.transform.localEulerAngles.y != 0)
                    doorMain.transform.localEulerAngles = new Vector3(0, 0, doorMain.transform.localEulerAngles.z);
                break;
        }
        // set the position on all axes to zero as it can change sometimes, because of the Unity physics engine
        if (doorMain.transform.localPosition.x != 0 || doorMain.transform.localPosition.y != 0 || doorMain.transform.localPosition.z != 0)
            doorMain.transform.localPosition = new Vector3(0, 0, 0);
    }
}
