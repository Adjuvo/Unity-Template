using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using System;
using System.Runtime.InteropServices;

namespace SGExample
{
    /// <summary>
    ///  The script for each of the two seperate objects to make them usable for acting seperate from the other object while holding them with both hands.
    /// </summary>

    public class SGExample_THS_Object : SG_Grabable
    {
        [Header("Which object is this (Make sure both objects are not the same number!)")]
        public whichObject numberObject = whichObject.firstObject;

        [HideInInspector]
        public int handCounter = 0;

        [HideInInspector]
        public SGExample_TwoHandsSeperate main;

        [HideInInspector]
        public ObjectMovementphases objectMov = ObjectMovementphases.freeMovement;


        private void GrabbedThis(SG_Interactable arg0, SG_GrabScript arg1)
        {
            main.CountHands(true, numberObject);
        }

        private void ReleasedThis(SG_Interactable arg0, SG_GrabScript arg1)
        {
            main.CountHands(false, numberObject);
        }

        protected override void UpdateLocation(float dT)
        {
            switch (objectMov)
            {
                case ObjectMovementphases.freeMovement:
                    if (numberObject == whichObject.firstObject)
                    {
                        base.UpdateLocation(dT);
                    }
                    else
                    {
                        base.UpdateLocation(dT);
                    }
                    break;
                case ObjectMovementphases.onlyYAxisCollision:
                    if (numberObject == whichObject.firstObject)
                    {
                        transform.localPosition = main.secondObject.transform.localPosition;
                        transform.rotation = main.secondObject.transform.rotation;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - main.yDif, transform.localPosition.z);
                    }
                    else
                    {
                        transform.localPosition = main.firstObject.transform.localPosition;
                        transform.rotation = main.firstObject.transform.rotation;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + main.yDif, transform.localPosition.z);
                    }
                    break;
                case ObjectMovementphases.onlyYAxisGrabbed:

                    break;
                case ObjectMovementphases.follow:
                    if(numberObject == whichObject.firstObject)
                    {
                        transform.localPosition = main.secondObject.transform.localPosition;
                        transform.rotation = main.secondObject.transform.rotation;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - main.yDif, transform.localPosition.z);
                    }
                    else
                    {
                        transform.localPosition = main.firstObject.transform.localPosition;
                        transform.rotation = main.firstObject.transform.rotation;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + main.yDif, transform.localPosition.z);
                    }
                    break;
            }
        }

        private float GetYValue()
        {
            float yValue = this.transform.localPosition.y;

            SG_TrackedHand hand = ScriptsGrabbingMe()[0].TrackedHand;
            Vector3 wristPos = hand.GetHandPose(SG.SG_TrackedHand.TrackingLevel.RealHandPose).wristPosition;



            return yValue;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.ObjectGrabbed.AddListener(GrabbedThis);
            this.ObjectReleased.AddListener(ReleasedThis);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.ObjectGrabbed.RemoveListener(GrabbedThis);
            this.ObjectReleased.RemoveListener(ReleasedThis);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.IsGrabbed())
                main.ObjectsColliding(numberObject, true);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!this.IsGrabbed())
                main.ObjectsColliding(numberObject, false);
        }
    }
}
