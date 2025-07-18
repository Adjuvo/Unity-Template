using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGExample
{
    /// <summary>
    ///  Puts this object this script is on to the same location of the wrist that grabs an object but only in the designated axis
    /// </summary>

    public class SGExample_FollowHand : MonoBehaviour
    {
        private SG.SG_TrackedHand hand;
        public SGExamples_SecondHandOnObject grabbedObject;

        private void Update()
        {
            if (this.grabbedObject.IsGrabbed())
            {
                hand = grabbedObject.ScriptsGrabbingMe()[0].TrackedHand;

                Vector3 wristPos = hand.GetHandPose(SG.SG_TrackedHand.TrackingLevel.RealHandPose).wristPosition;

                this.transform.position = wristPos;

                this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);
            }
        }
    }
}
