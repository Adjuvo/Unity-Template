using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using UnityEngine.Events;
using System;

public class SB_Dial : SG_Grabable
{
    public SG.Util.MoveAxis rotateAround = SG.Util.MoveAxis.Y;

    [Header("How many rotations degree for an event to fire")]
    public int step = 10;

    [Header("Haptic waveform")]
    public SG_CustomWaveform waveform;

    [Header("--- Events ---")]
    public UnityEvent stepEvent;

    // private vars
    private Vector3 startrotation = Vector3.zero;
    private float oldRotation = 0f;

    protected override void Start()
    {
        base.Start();

        startrotation = this.MyTransform.localEulerAngles;
    }

    protected override void MoveToTargetLocation(Vector3 targetPosition, Quaternion targetRotation, float dT)
    {
        Transform baseTransform = this.MyTransform;
        baseTransform.rotation = targetRotation;

        Vector3 localEuler = baseTransform.localEulerAngles;

        int axIndex = SG.Util.SG_Util.AxisIndex(rotateAround);
        for (int i=0; i<3; i++)
        {
            if (i != axIndex)
            {
                localEuler[i] = 0;
            }
            else
            {
                // check if a haptic effect needs to be played
                if (CalcStep(oldRotation) != CalcStep(SG.Util.SG_Util.NormalizeAngle(localEuler[i])))
                {
                    PlayHaptics();
                    stepEvent.Invoke();
                }
                // set the oldraotion for the next frame
                oldRotation = SG.Util.SG_Util.NormalizeAngle(localEuler[i]);
            }    
        }
        baseTransform.localEulerAngles = localEuler;
    }

    private void PlayHaptics()
    {
        this.SendCustomWaveform(waveform, waveform.intendedMotor);
    }

    // check in which step of the dial the value sits
    private int CalcStep(float angle)
    {
        angle = Mathf.Abs(angle);

        float stepSize = 360 / step;
        int currentStep = (int)(angle / stepSize);

        return currentStep;
    }

    protected override void SetupScript()
    {
        base.SetupScript();
        if (this.physicsBody != null)
        {
            this.physicsBody.isKinematic = true;
        }
    }

    // make sure the object can't been moved
    protected virtual void OnValidate()
    {
        this.moveSpeed = 0.0f;
    }
}
