using SG;
using SG.Util;
using UnityEngine;

namespace SGExample
{

    public class SGExample_DoorFollower : MonoBehaviour
    {
        public SGExample_Door door;

        public float rotation;

        private SG_TrackedHand hand;
        private SG_HandPose handPose;
        private float startRotation = 0;

        private void LateUpdate()
        {
            if (door.IsGrabbed())
            {
                hand = door.hand;
                handPose = hand.GetHandPose(SG_TrackedHand.TrackingLevel.RealHandPose);
                FollowWrist(false, handPose.wristPosition);
            }
        }

        public void FollowWrist(bool firstFrame, Vector3 position)
        {
            transform.LookAt(position);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

            if (firstFrame)
                startRotation = Mathf.Abs(Mathf.Abs(door.rotation) - Mathf.Abs(SG_Util.NormalizeAngle(transform.localEulerAngles.y)));

            rotation = Mathf.Abs(SG_Util.NormalizeAngle(transform.localEulerAngles.y)) - startRotation;
        }
    }
}
