using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// When using this script place it on a object that has its local rotation set to 0!!!
/// </summary>
public class SB_Rotator : MonoBehaviour
{
    public enum rotateAngleEnum { x, y, z }

    [Header("The rotating gameobject")]
    public GameObject rotatingObject;

    [Header("Rotate local angle")]
    public rotateAngleEnum localRotateAngle = rotateAngleEnum.y;

    [Header("Negative Rotation of the object")]
    [Range(0, 180)]public int minusTotalRotation = 0;

    [Header("Positive Rotation of the object")]
    [Range(0, 180)] public int posTotalRotation = 70;

    [Header("Event that triggers if the rotating object reaches the minimal position")]
    public UnityEvent minPosEvent;

    [Header("Event that triggers if the rotating object reaches the maximum position")]
    public UnityEvent maxPosEvent;

    // private vars
    [HideInInspector]
    public float startAngleEuler;
    [HideInInspector]
    public float startAngle;

    private void Awake()
    {
        // set the start angle of the object
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                startAngle = SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x);
                startAngleEuler = rotatingObject.transform.localEulerAngles.x;
                break;
            case rotateAngleEnum.y:
                startAngle = SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y);
                startAngleEuler = rotatingObject.transform.localEulerAngles.y;
                break;
            case rotateAngleEnum.z:
                startAngle = SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z);
                startAngleEuler = rotatingObject.transform.localEulerAngles.z;
                break;
        }
    }

    public virtual void Update()
    {
        // make sure the object doesn't rotate further than the maximum and minimum positions
        BoundariesUpdate();

        // make sure the object doesn't move away or use a different rotation
        TransformUpdate();
    }

    // don't let the object rotate further that the boundaries set
    public virtual void BoundariesUpdate()
    {
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) < startAngle - minusTotalRotation)
                {
                    rotatingObject.transform.localEulerAngles = new Vector3(startAngleEuler - minusTotalRotation, 0, 0);
                    MinPosReached();
                }
                else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) > startAngle + posTotalRotation)
                {
                    rotatingObject.transform.localEulerAngles = new Vector3(startAngleEuler + posTotalRotation, 0, 0);
                    MaxPosReached();
                }
                break;
            case rotateAngleEnum.y:
                if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) < startAngle - minusTotalRotation)
                {
                    rotatingObject.transform.localEulerAngles = new Vector3(0, startAngleEuler - minusTotalRotation, 0);
                    MinPosReached();
                }
                else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) > startAngle + posTotalRotation)
                {
                    rotatingObject.transform.localEulerAngles = new Vector3(0, startAngleEuler + posTotalRotation, 0);
                    MaxPosReached();
                }
                break;
            case rotateAngleEnum.z:
                if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) < startAngle - minusTotalRotation)
                {
                    rotatingObject.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler - minusTotalRotation);
                    MinPosReached();
                }
                else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) > startAngle + posTotalRotation)
                {
                    rotatingObject.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler + posTotalRotation);
                    MaxPosReached();
                }
                break;
        }
    }

    // set the position and rotation of the door on zero, except for the angle chosen
    public virtual void TransformUpdate()
    {
        // set the rotation on all axes except the chosen one to zero
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                if (rotatingObject.transform.localEulerAngles.y != 0 || rotatingObject.transform.localEulerAngles.z != 0)
                    rotatingObject.transform.localEulerAngles = new Vector3(rotatingObject.transform.localEulerAngles.x, 0, 0);
                break;
            case rotateAngleEnum.y:
                if (rotatingObject.transform.localEulerAngles.x != 0 || rotatingObject.transform.localEulerAngles.z != 0)
                    rotatingObject.transform.localEulerAngles = new Vector3(0, rotatingObject.transform.localEulerAngles.y, 0);
                break;
            case rotateAngleEnum.z:
                if (rotatingObject.transform.localEulerAngles.x != 0 || rotatingObject.transform.localEulerAngles.y != 0)
                    rotatingObject.transform.localEulerAngles = new Vector3(0, 0, rotatingObject.transform.localEulerAngles.z);
                break;
        }
        // set the position on all axes to zero as it can change sometimes, because of the Unity physics engine
        if (rotatingObject.transform.localPosition.x != 0 || rotatingObject.transform.localPosition.y != 0 || rotatingObject.transform.localPosition.z != 0)
            rotatingObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public virtual void MinPosReached()
    {
        minPosEvent.Invoke();
    }

    public virtual void MaxPosReached() 
    {
        maxPosEvent.Invoke();
    }
}
