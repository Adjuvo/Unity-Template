using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using SG.Util;

/// <summary> Maps finger flexion to a value between 0...1, and uses this for Haptics </summary>
public class SGExamples_TriggerLogic : MonoBehaviour 
{
	[Header("The Trigger")]
	public GameObject triggerObject;
    [Header("How far can the trigger be pressed")]
    public float maxDistancePressed = 0.02f;
    [Header("The axis the trigger will be pressed")]
    public MoveAxis axisTriggerMovement = MoveAxis.X;
    [Header("The haptic effect")]
    public SG_CustomWaveform hapticEffect;

    [Header("The grabable script on the object that has this trigger")]
    public SG_Grabable grabable;

    /// <summary> The trigger logic will respond to the flexion of this finger, which is given as value between 0..1. </summary>
    [Header("Which finger will be used to use the trigger")]
    public SGCore.Finger respondsTo = SGCore.Finger.Index;

    [Header("Start and end of the flexion of the finger that will move the trigger")]
    [Range(0, 1)] public float startFlexion = 0.2f; // when finger flexion is above this value, trigger pressure will ramp up from 0% 
	[Range(0, 1)] public float endFlexion = 0.8f; // when finger flexion is at this value, the trigger pressure it at 100%

	/// <summary> The last calculated pressure </summary>
	private float latestPressure = 0.0f;

	/// <summary> The Trigger Pressure as calculated by this script.. </summary>
	public float TriggerPressure
    {
		get { return grabable.IsGrabbed() ? latestPressure : 0.0f; } // For outside scripts; When you're not being grabbed, return 0.
    }

    // private vars
    private Vector3 startPosTrigger = Vector3.zero;

    private void Start()
    {
        // set the start position of the trigger
        startPosTrigger = triggerObject.transform.localPosition;
    }

    void Update () 
	{
		if (grabable.IsGrabbed()) // indicated there is at least one script grabbing onto grabable
        {
			SG_TrackedHand firstHand = grabable.ScriptsGrabbingMe()[0].TrackedHand; // grab the first hand

			// -- Update the latest pressure
			float[] flexions;
			if (firstHand.GetNormalizedFlexion(out flexions)) // attempt to get the latest normalized finger flexions: values between 0...1
            {
				float currFlex = flexions[(int)this.respondsTo]; // we're using an enumerator to index the array (0..4, where 0 = thumb and 4 = pinky).
				if (startFlexion == endFlexion)  // if these are equal, mapping would result into a div/0.
				{ 
					latestPressure = currFlex >= startFlexion ? 1.0f : 0.0f;
				}
				else
                {
					latestPressure = SG_Util.Map(currFlex, startFlexion, endFlexion, 0.0f, 1.0f, true); // free function that comes with unity plugin; map a value from one range to another.
                }
            }

            // -- Update the trigger position
            float triggerDis = maxDistancePressed * latestPressure;
            Vector3 triggerPos = Vector3.zero;
            switch (axisTriggerMovement)
            {
                case MoveAxis.X:
                    triggerPos = new Vector3(startPosTrigger.x + triggerDis, startPosTrigger.y, startPosTrigger.z);
                    break;
                case MoveAxis.NegativeX:
                    triggerPos = new Vector3(startPosTrigger.x - triggerDis, startPosTrigger.y, startPosTrigger.z);
                    break;
                case MoveAxis.Y:
                    triggerPos = new Vector3(startPosTrigger.x, startPosTrigger.y + triggerDis, startPosTrigger.z);
                    break;
                case MoveAxis.NegativeY:
                    triggerPos = new Vector3(startPosTrigger.x, startPosTrigger.y - triggerDis, startPosTrigger.z);
                    break;
                case MoveAxis.Z:
                    triggerPos = new Vector3(startPosTrigger.x, startPosTrigger.y, startPosTrigger.z + triggerDis);
                    break;
                case MoveAxis.NegativeZ:
                    triggerPos = new Vector3(startPosTrigger.x, startPosTrigger.y, startPosTrigger.z - triggerDis);
                    break;
            }
            // set the position of the trigger
            triggerObject.transform.localPosition = triggerPos;

            // -- Update Haptics
            int amplitude = Mathf.RoundToInt(100.0f * latestPressure); // calculate intensity
			if (amplitude > 0)
			{
                /* If you want to do it through code (This example is for the index finger)
                // create the haptics effect 
				SGCore.Haptics.SG_TimedBuzzCmd vibration = new SGCore.Haptics.SG_TimedBuzzCmd(this.respondsTo, amplitude, 0.02f); 
                // send the command (haptics effect)
				grabable.SendCmd(vibration);
                */

                // set the strenght of the effect a bit harder for a power tool, so amplitude times two
                hapticEffect.amplitude = amplitude * 2;
                // send the command (haptics effect)
                grabable.SendCustomWaveform(hapticEffect, VibrationLocation.WholeHand);
                grabable.SendCustomWaveform(hapticEffect, VibrationLocation.Palm_IndexSide);
                grabable.SendCustomWaveform(hapticEffect, VibrationLocation.Index_Tip);
            }

        }
	}
}
