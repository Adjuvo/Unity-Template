using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;


namespace SGExample
{
    public class SGExample_SolidSqueeze : SG_Grabable
    {
        [Header("------ SolidSqueeze values -----")]
        public SG_CustomWaveform hapticEffect;
        [Range(1,3)]public float effectStrenght = 1;

        [Header("Which fingers are used for the average flexion")]
        public SGCore.Finger[] respondsTo;

        private float averageFingerFlexion = 0;
        private Vector3 startSize = Vector3.zero;

        protected override void Start()
        {
            base.Start();

            startSize = this.transform.localScale;
        }

        private void Update()
        {
            if (this.IsGrabbed())
            {
                SG_TrackedHand firstHand = this.ScriptsGrabbingMe()[0].TrackedHand;

                averageFingerFlexion = AverageFingerFlexion(firstHand);

                VibroCommand(averageFingerFlexion);

                ScalingObject(averageFingerFlexion);
            }
        }

        private float AverageFingerFlexion(SG_TrackedHand hand)
        {
            float aFF = 0;
            float[] flexions;
            hand.GetNormalizedFlexion(out flexions);

            for (int i = 0; i < respondsTo.Length; i++)
            {
                aFF += flexions[(int)respondsTo[i]];
            }

            aFF = aFF / respondsTo.Length;

            return Mathf.Lerp(1, 0, aFF);
        }

        private void VibroCommand(float amplitude)
        { 
            hapticEffect.amplitude = Mathf.Lerp(1, 0, amplitude * effectStrenght);
            this.SendCustomWaveform(hapticEffect, VibrationLocation.Palm_IndexSide);
            this.SendCustomWaveform(hapticEffect, VibrationLocation.Palm_PinkySide);
        }

        private void ScalingObject(float scale)
        {
            scale = scale * 1.2f;
            scale = scale < 0.5 ? 0.5f : scale;
            Vector3 size = this.transform.localScale;
            size.x = scale * startSize.x;
            size.z = scale * startSize.z;
            this.transform.localScale = size;
        }


        // utility
        protected override void MoveToTargetLocation(Vector3 targetPosition, Quaternion targetRotation, float dT)
        {
            // empty so the object won't move from its position when grabbed
        }
    }
}
