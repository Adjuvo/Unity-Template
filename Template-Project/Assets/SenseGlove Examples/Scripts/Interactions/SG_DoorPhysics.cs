using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SG_DoorPhysics : MonoBehaviour
{
    public enum rotateAngleEnum { x, y, z }

    [Header("Rotate local angle")]
    public rotateAngleEnum localRotateAngle = rotateAngleEnum.y;

    [Header("Max Rotation of the door")]
    public int maxTotalRotation = 70;

    // private vars
    private float startAngleEuler;
    private float startAngle;

    private void Awake()
    {
        // set the start angle of the door
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                startAngle = SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.x);
                startAngleEuler = this.transform.localEulerAngles.x;
                break;
            case rotateAngleEnum.y:
                startAngle = SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.y);
                startAngleEuler = this.transform.localEulerAngles.y;
                break;
            case rotateAngleEnum.z:
                startAngle = SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.z);
                startAngleEuler = this.transform.localEulerAngles.z;
                break;
        }
    }

    private void Update()
    {
        // make sure the door doesn't move further than the maximum and minimum positions
        BoundariesUpdate();
        
        // make sure the door doesn't move away or use a different rotation
        TransformUpdate();
    }

    // don't let the door open further that the boundaries set
    private void BoundariesUpdate()
    {
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                if (SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.x) < startAngle)
                {
                    this.transform.localEulerAngles = new Vector3(startAngleEuler, 0, 0);
                }
                else if (SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.x) > startAngle + maxTotalRotation)
                {
                    this.transform.localEulerAngles = new Vector3(startAngleEuler + maxTotalRotation, 0, 0);
                }
                break;
            case rotateAngleEnum.y:
                if (SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.y) < startAngle)
                {
                    this.transform.localEulerAngles = new Vector3(0, startAngleEuler, 0);
                }
                else if (SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.y) > startAngle + maxTotalRotation)
                {
                    this.transform.localEulerAngles = new Vector3(0, startAngleEuler + maxTotalRotation, 0);
                }
                break;
            case rotateAngleEnum.z:
                if (SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.z) < startAngle)
                {
                    this.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler);
                }
                else if (SG.Util.SG_Util.NormalizeAngle(this.transform.localEulerAngles.z) > startAngle + maxTotalRotation)
                {
                    this.transform.localEulerAngles = new Vector3(0, 0, startAngleEuler + maxTotalRotation);
                }
                break;
        }
    }

    // set the position and rotation of the door on zero, except for the angle chosen
    private void TransformUpdate()
    {
        // set the rotation on all axes except the chosen one to zero
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                if (this.transform.localEulerAngles.y != 0 || this.transform.localEulerAngles.z != 0)
                    this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, 0, 0);
                break;
            case rotateAngleEnum.y:
                if (this.transform.localEulerAngles.x != 0 || this.transform.localEulerAngles.z != 0)
                    this.transform.localEulerAngles = new Vector3(0, this.transform.localEulerAngles.y, 0);
                break;
            case rotateAngleEnum.z:
                if (this.transform.localEulerAngles.x != 0 || this.transform.localEulerAngles.y != 0)
                    this.transform.localEulerAngles = new Vector3(0, 0, this.transform.localEulerAngles.z);
                break;
        }
        // set the position on all axes to zero as it can change sometimes, because of the Unity physics engine
        if (this.transform.localPosition.x != 0 || this.transform.localPosition.y != 0 || this.transform.localPosition.z != 0)
            this.transform.localPosition = new Vector3(0, 0, 0);
    }
}
