using SG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace SGExample
{
    /// <summary>
    ///  This object can be grabbed independently from the other part.
    ///  By moving this or the other hand you can move the objects seperatly
    /// </summary>

    public class SGExamples_SecondHandOnObject : SG_Grabable
    {
        [Header("two handed manipulation vars")]
        public GameObject mainObject;
        public GameObject fakeHandle;
        public GameObject realHandPos;

        public float movingSpeed = 1f;

        public float objectOffsetStart = 1.7f;
        public float objectOffsetEnd = 2.5f;

        private SG_Grabable mainGrabable;


        protected override void Start()
        {
            base.Start();

            mainGrabable = mainObject.GetComponent<SG_Grabable>();
        }

        private void Update()
        {
            // you can only pick this object if the first hand has picked it up first
            this.allowedHands = mainGrabable.IsGrabbed() ? GrabMethod.OneHandOnly : GrabMethod.None;
        }

        protected override void UpdateLocation(float dT)
        {
            if (!this.IsGrabbed())
            {
                transform.position = fakeHandle.transform.position;
                transform.rotation = fakeHandle.transform.rotation;
            }
            else
            {
                float yValue = GetYValue();
                transform.rotation = fakeHandle.transform.rotation;
                transform.position = fakeHandle.transform.position;
                transform.localPosition = new Vector3(transform.localPosition.x, yValue, transform.localPosition.z);
            }
        }

        private float GetYValue()
        {
            float yValue = this.transform.localPosition.y;

            SG_TrackedHand hand = ScriptsGrabbingMe()[0].TrackedHand;
            Vector3 wristPos = hand.GetHandPose(SG.SG_TrackedHand.TrackingLevel.RealHandPose).wristPosition;

            if (yValue < realHandPos.transform.localPosition.y)
                yValue = yValue + (movingSpeed * Time.deltaTime);

            if (yValue > realHandPos.transform.localPosition.y)
                yValue = yValue - (movingSpeed * Time.deltaTime);

            if (yValue < objectOffsetStart)
                yValue = objectOffsetStart;

            if (yValue > objectOffsetEnd)
                yValue = objectOffsetEnd;

            return yValue;
        }

        protected override void OnReleaseComplete(GrabArguments beReleasedBy)
        {
            fakeHandle.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);

            base.OnReleaseComplete(beReleasedBy);
        }

    }
}
