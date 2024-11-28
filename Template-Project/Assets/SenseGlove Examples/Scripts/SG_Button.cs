using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;

// event that fires a boolean if triggered
[System.Serializable]
public class ButtonPressedEvent : UnityEngine.Events.UnityEvent<bool> { }

public class SG_Button : MonoBehaviour
{
    [Header("Distance to activate button")]
    public float maxButtonMovement = 0.2F;

    [Header("Haptic effect to play when pressed")]
    public SG_CustomWaveform hapticEffect;

    [Header("The speed the button returns to original state")]
    public float buttonReturnSpeed = 8f;

    [Header("Event that triggers when the switch is flipped")]
    public ButtonPressedEvent buttonPressedUnityEvent = new ButtonPressedEvent();

    // private vars
    private float timer = 5F;
    private bool switched = false;
    private bool onOff = false;
    private SG_TrackedHand interactingGlove;
    private Vector3 localStartPos;


    private void Awake()
    {
        localStartPos = this.transform.localPosition;
    }

    void Update()
    {
        // timer to make sure the button doesn't fire multiple time in quick succession
        if (timer < 1)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            switched = false;
        }

        // if the max range is reached then fire of the event and a haptic command
        if (this.transform.localPosition.y < -maxButtonMovement && !switched)
        {
            SendHapticCommand();
            buttonPressedUnityEvent.Invoke(onOff);
            onOff = !onOff;
            timer = 0;
            switched = true;
        }

        // update the button position to the start position
        UpdateButtonPosition();
    }

    // create a haptic effect on the glove when you pressed the button deep enough to trigger the event
    private void SendHapticCommand()
    {
        if(interactingGlove != null)
            interactingGlove.SendCustomWaveform(hapticEffect, VibrationLocation.Index_Tip);
    }


    // detect which glove is interacting with the button to give the haptic effect to the right glove.
    public void HandDetected(SG_TrackedHand glove)
    {
        interactingGlove = glove;
    }

    // Update the button position to move slowly back to its starting position
    private void UpdateButtonPosition()
    {
        // Return button to startPosition
        transform.localPosition = Vector3.Lerp(transform.localPosition, localStartPos, Time.deltaTime * buttonReturnSpeed);
    }
}
