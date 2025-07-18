using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using SG.Util;

// event that fires a boolean if triggered
[System.Serializable]
public class SwitchFlippedEvent : UnityEngine.Events.UnityEvent<bool> { }

public class SG_Switch : MonoBehaviour
{
    [Header("Max Rotation of the switch")]
    public int maxTotalRotation = 70;

    [Header("Haptic effect to play when switched")]
    public SG_CustomWaveform hapticEffect;

    [Header("Event that triggers when the switch is flipped")]
    public SwitchFlippedEvent switchFlippedUnityEvent = new SwitchFlippedEvent();

    // private vars
    private float timer = 10F;
    private bool switched = false;
    private SG_TrackedHand interactingGlove;
    private float startAngle;
    private float startAngleEuler;

    private void Awake()
    {
        // angle of the start position from where the maximum rotation is measured.
        startAngle = SG_Util.NormalizeAngle(this.transform.localEulerAngles.x);
        startAngleEuler = this.transform.localEulerAngles.x;
    }

    void Update()
    {
        // the timer for a short freeze period after firing the event, so that the switch is not switched back because of the collider getting to the otherside of the switchpole
        if (timer < 1)
        {
            timer = timer + Time.deltaTime;
        }
        else if (timer > 1 & timer < 5)
        {
            UnFreezeToggle();
            timer = 10;
        }

        // check if the switched reached the end and then firing the event
        if (SG_Util.NormalizeAngle(this.transform.localEulerAngles.x) >= startAngle + maxTotalRotation && !switched)
        {
            SendHapticCommand();
            switchFlippedUnityEvent.Invoke(false);
            FreezeToggle();
            timer = 0;
            switched = true;
        }

        if (SG_Util.NormalizeAngle(this.transform.localEulerAngles.x) <= startAngle && switched)
        {
            SendHapticCommand();
            switchFlippedUnityEvent.Invoke(true);
            FreezeToggle();
            timer = 0;
            switched = false;
        }

        // make sure the switch doesn't move further than the max and minimum positions
        if (SG_Util.NormalizeAngle(this.transform.localEulerAngles.x) < startAngle)
        {
            this.transform.localEulerAngles = new Vector3(startAngleEuler, 0, 0);
        }
        else if(SG_Util.NormalizeAngle(this.transform.localEulerAngles.x) > startAngle + maxTotalRotation)
        {
            this.transform.localEulerAngles = new Vector3(startAngleEuler + maxTotalRotation, 0, 0);
        }    
    }

    // create a haptic effect on the Senseglove when you switched the switch far enough to trigger the event
    private void SendHapticCommand()
    {
        if (interactingGlove != null)
            interactingGlove.SendCustomWaveform(hapticEffect, VibrationLocation.Index_Tip);
    }

    // check which Senseglove is interacting with the switch
    public void HandDetected(SG_TrackedHand glove)
    {
        interactingGlove = glove;
    }

    // freeze the switch for a short amount of time to not switch back because of colliders on the finger pushing it back
    private void FreezeToggle()
    {
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    // unfreeze when the timer reached its point
    private void UnFreezeToggle()
    {
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}
