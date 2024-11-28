using SG;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

// event that fires an int that shows the new position (0 = left, 1 = middle, 2 = right)
[System.Serializable]
public class SB_ToggleChangedEvent : UnityEngine.Events.UnityEvent<int> { }

public class SB_SwitchToggle : SB_Rotator
{
    [Header("--- Toggle ---")]
    [Header("Amount of toggle positions")]
    [Range(2, 3)] public int amountPositions = 2;

    [Header("Position the Toggle is in (1 = left, 2 = middle , 3 = right)")]
    [Range(1, 3)] public int positionToggle = 1;

    [Header("The speed the button returns to original state")]
    public float returnSpeed = 4f;

    [Header("Haptic effect to play when pressed")]
    public SG_CustomWaveform hapticEffect;

    [Header("--- Events --- (1 = left, 2 = middle , 3 = right)")]
    public UnityEvent firstPosEvent;
    public UnityEvent secondPosEvent;
    public UnityEvent thirdPosEvent;

    // private vars
    private SG_TrackedHand interactingGlove;

    // create a haptic effect on the glove when you toggle to the next state
    private void SendHapticCommand()
    {
        if (interactingGlove != null)
            interactingGlove.SendCustomWaveform(hapticEffect, hapticEffect.intendedMotor);
    }

    // detect which glove is interacting with the toggle
    public void HandDetected(SG_TrackedHand glove)
    {
        interactingGlove = glove;
    }

    public override void TransformUpdate()
    {
        base.TransformUpdate();

        float angle = PosToReturnTo();

        // return object to start rotation
        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                rotatingObject.transform.localEulerAngles = Vector3.Lerp(SG.Util.SG_Util.NormalizeAngles(rotatingObject.transform.localEulerAngles), new Vector3(angle, 0, 0), Time.deltaTime * returnSpeed);
                break;
            case rotateAngleEnum.y:
                rotatingObject.transform.localEulerAngles = Vector3.Lerp(SG.Util.SG_Util.NormalizeAngles(rotatingObject.transform.localEulerAngles), new Vector3(0, angle, 0), Time.deltaTime * returnSpeed);
                break;
            case rotateAngleEnum.z:
                rotatingObject.transform.localEulerAngles = Vector3.Lerp(SG.Util.SG_Util.NormalizeAngles(rotatingObject.transform.localEulerAngles), new Vector3(0, 0, angle), Time.deltaTime * returnSpeed);
                break;
        }
    }

    public override void BoundariesUpdate()
    {
        base.BoundariesUpdate();

        float tenPercent = (minusTotalRotation + posTotalRotation) / 10;
        float middlePos = (startAngle - minusTotalRotation) + ((minusTotalRotation + posTotalRotation) / 2);

        switch (localRotateAngle)
        {
            case rotateAngleEnum.x:
                if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) < startAngle - (minusTotalRotation - tenPercent))
                {
                    FirstPosReached();
                }
                else if (amountPositions == 3 && SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) > middlePos - tenPercent && SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) < middlePos + tenPercent)
                {
                    SecondPosReached();
                }
                else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) > startAngle + (posTotalRotation - tenPercent))
                {
                    ThirdPosReached();
                }
                break;
            case rotateAngleEnum.y:
                if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) < startAngle - (minusTotalRotation - tenPercent))
                {
                    FirstPosReached();
                }
                else if (amountPositions == 3 && SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) > middlePos - tenPercent && SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) < middlePos + tenPercent)
                {
                    SecondPosReached();
                }
                else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) > startAngle + (posTotalRotation - tenPercent))
                {
                    ThirdPosReached();
                }
                break;
            case rotateAngleEnum.z:
                if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) < startAngle - (minusTotalRotation - tenPercent))
                {
                    FirstPosReached();
                }
                else if (amountPositions == 3 && SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) > middlePos - tenPercent && SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) < middlePos + tenPercent)
                {
                    SecondPosReached();
                }
                else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) > startAngle + (posTotalRotation - tenPercent))
                {
                    ThirdPosReached();
                }
                break;
        }
    }

    private float PosToReturnTo()
    {
        int posNumber = positionToggle;
        float angle = 0f;

        switch(amountPositions)
        {
            case 2:
                float middlePos = ((minusTotalRotation + posTotalRotation) / 2) + startAngle;

                switch (localRotateAngle)
                {
                    case rotateAngleEnum.x:
                        angle = SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) < middlePos ? startAngle - minusTotalRotation : startAngle + posTotalRotation;
                        break;
                    case rotateAngleEnum.y:
                        angle = SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) < middlePos ? startAngle - minusTotalRotation : startAngle + posTotalRotation;
                        break;
                    case rotateAngleEnum.z:
                        angle = SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) < middlePos ? startAngle - minusTotalRotation : startAngle + posTotalRotation;
                        break;
                }
                break; 
            case 3:
                float firstQuarter = (startAngle - minusTotalRotation) + ((minusTotalRotation + posTotalRotation) / 4);
                float secondQuarter = (startAngle - minusTotalRotation) + ((minusTotalRotation + posTotalRotation) / 2);
                float thirdQuarter = (startAngle - minusTotalRotation) + (((minusTotalRotation + posTotalRotation) / 4) * 3);

                switch (localRotateAngle)
                {
                    case rotateAngleEnum.x:
                        if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) < firstQuarter)
                        {
                            angle = startAngle - minusTotalRotation;
                        }
                        else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.x) > thirdQuarter)
                        {
                            angle = startAngle + posTotalRotation;
                        }
                        else
                        {
                            angle = secondQuarter;
                        }
                        break;
                    case rotateAngleEnum.y:
                        if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) < firstQuarter)
                        {
                            angle = startAngle - minusTotalRotation;
                        }
                        else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.y) > thirdQuarter)
                        {
                            angle = startAngle + posTotalRotation;
                        }
                        else
                        {
                            angle = secondQuarter;
                        }
                        break;
                    case rotateAngleEnum.z:
                        if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) < firstQuarter)
                        {
                            angle = startAngle - minusTotalRotation;
                        }
                        else if (SG.Util.SG_Util.NormalizeAngle(rotatingObject.transform.localEulerAngles.z) > thirdQuarter)
                        {
                            angle = startAngle + posTotalRotation;
                        }
                        else
                        {
                            angle = secondQuarter;
                        }
                        break;
                }
                break;
        }
        return angle;
    }

    private void FirstPosReached()
    {
        if (positionToggle != 1)
        {
            firstPosEvent.Invoke();
            SendHapticCommand();
        }
            
        positionToggle = 1;
    }

    private void SecondPosReached()
    {
        if (positionToggle != 2)
        {
            secondPosEvent.Invoke();
            SendHapticCommand();
        }
            
        positionToggle = 2;
    }

    private void ThirdPosReached()
    {
        if (positionToggle != 3)
        {
            thirdPosEvent.Invoke();
            SendHapticCommand();
        }

        positionToggle = 3;
    }
}
