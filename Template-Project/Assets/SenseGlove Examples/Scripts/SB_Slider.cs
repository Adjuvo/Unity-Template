using SG;
using SG.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SB_Slider : MonoBehaviour
{

    [Header("Slider Object")]
    public GameObject sliderObject;

    [Header("Translate along which local angle")]
    public MoveAxis localPositionAngle = MoveAxis.X;

    [Header("Minimal position of the object compared startposition")]
    [Range(0, 180)] public float minPos = 30;

    [Header("maximum position of the object compared startposition")]
    [Range(0, 180)] public float maxPos = 30;

    [Header("How many haptic steps over the total lenght")]
    public int step = 10;

    [Header("Haptic waveform")]
    public SG_CustomWaveform waveform;

    [Header("The collider the slider might collide with the underground and ignores that collision")]
    public GameObject table;

    [Header("The colliders of the slider")]
    public Collider[] colliders;

    [Header("--- Events ---")]
    public UnityEvent stepEvent;

    // private vars
    private Vector3 startPos;
    private float totalMovement;
    private float minimalPosition;
    public float sliderPosition;
    private int oldPosition;
    private SG_TrackedHand hand;

    private void Awake()
    {
        startPos = sliderObject.transform.localPosition;

        // ignore the collider that the button is placed on
        if (table != null && table.GetComponent<Collider>() != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Physics.IgnoreCollision(table.GetComponent<Collider>(), colliders[i]);
            }
        }

        totalMovement = minPos + maxPos;

        switch (localPositionAngle)
        {
            case MoveAxis.X:
            case MoveAxis.NegativeX:
                minimalPosition = sliderObject.transform.localPosition.x - minPos;
                break;
            case MoveAxis.Y:
            case MoveAxis.NegativeY:
                minimalPosition = sliderObject.transform.localPosition.y - minPos;
                break;
            case MoveAxis.Z:
            case MoveAxis.NegativeZ:
                minimalPosition = sliderObject.transform.localPosition.z - minPos;
                break;
        }

        SliderPos();
        
    }

    private void Update()
    {
        UpdatePosition(); 
    }

    private void UpdatePosition()
    {
        switch (localPositionAngle)
        {
            case MoveAxis.X:
            case MoveAxis.NegativeX:
                // don't let the slider move any other direction that the one indicated by localPositionAngle
                if (sliderObject.transform.localPosition.y != 0 || sliderObject.transform.localPosition.z != 0)
                    sliderObject.transform.localPosition = new Vector3(sliderObject.transform.localPosition.x, 0, 0);
                // if the slider is drawn deeper than the startposition place the drawer at the start position
                if (sliderObject.transform.localPosition.x < (startPos.x - minPos))
                    sliderObject.transform.localPosition = new Vector3((startPos.x - minPos), 0, 0);
                // if the slider is drawn further than the maxDistance place the drawer at the maxDistance
                if (sliderObject.transform.localPosition.x > (startPos.x + maxPos))
                    sliderObject.transform.localPosition = new Vector3((startPos.x + maxPos), 0, 0);
                break;
            case MoveAxis.Y:
            case MoveAxis.NegativeY:
                // don't let the slider move any other direction that the one indicated by localPositionAngle
                if (sliderObject.transform.localPosition.x != 0 || sliderObject.transform.localPosition.z != 0)
                    sliderObject.transform.localPosition = new Vector3(0, sliderObject.transform.localPosition.y, 0);
                // if the slider is drawn deeper than the startposition place the drawer at the start position
                if (sliderObject.transform.localPosition.y < (startPos.y - minPos))
                    sliderObject.transform.localPosition = new Vector3(0, (startPos.y - minPos), 0);
                // if the slider is drawn further than the maxDistance place the drawer at the maxDistance
                if (sliderObject.transform.localPosition.y > (startPos.y + maxPos))
                    sliderObject.transform.localPosition = new Vector3(0, (startPos.y + maxPos), 0);
                break;
            case MoveAxis.Z:
            case MoveAxis.NegativeZ:
                // don't let the slider move any other direction that the one indicated by localPositionAngle
                if (sliderObject.transform.localPosition.x != 0 || sliderObject.transform.localPosition.y != 0)
                    sliderObject.transform.localPosition = new Vector3(0, 0, sliderObject.transform.localPosition.z);
                // if the slider is drawn deeper than the startposition place the drawer at the start position
                if (sliderObject.transform.localPosition.z < (startPos.z - minPos))
                    sliderObject.transform.localPosition = new Vector3(0, 0, (startPos.z - minPos));
                // if the slider is drawn further than the maxDistance place the drawer at the maxDistance
                if (sliderObject.transform.localPosition.y > (startPos.z + maxPos))
                    sliderObject.transform.localPosition = new Vector3(0, 0, (startPos.z + maxPos));
                break;
        }

        SliderPos();
    }

    private void SliderPos()
    {
        switch (localPositionAngle)
        {
            case MoveAxis.X:
            case MoveAxis.NegativeX:
                sliderPosition = (sliderObject.transform.localPosition.x - minimalPosition) / totalMovement;
                break;
            case MoveAxis.Y:
            case MoveAxis.NegativeY:
                sliderPosition = (sliderObject.transform.localPosition.y - minimalPosition) / totalMovement;
                break;
            case MoveAxis.Z:
            case MoveAxis.NegativeZ:
                sliderPosition = (sliderObject.transform.localPosition.z - minimalPosition) / totalMovement;
                break;
        }

        if((int)(sliderPosition * step) != oldPosition)
        {
            PlayHaptics();
            stepEvent.Invoke();
        }

        oldPosition = (int)(sliderPosition * step);
    }

    private void PlayHaptics()
    {
        if(hand != null)
            hand.SendCustomWaveform(waveform, waveform.intendedMotor);
    }

    // detect which glove is interacting with the button to give the haptic effect to the right glove.
    public void HandDetected(SG_TrackedHand glove)
    {
        hand = glove;
    }
}
