using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;

namespace SGExample
{

    /// <summary>
    /// A class that can hook itself up to a SG_Interactable or material, and deform its mesh.
    /// Script also increases the strenght of the palmstrap on top of the force feedback when just using SG_MeshDeform
    /// </summary>

    public class SGExample_MeshDeformPalmstrap : SG_MeshDeform
    {
        [Header("Extra value on top of the mesh deform strenght")]
        [Range(1.0f, 5.0f)] public float extraSqueezeStrenght = 3;

        [SerializeField] protected SG_Interactable linkedInteractible;

        public float squeezeLevel = 0;

        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {
            squeezeLevel = Get01Value() * extraSqueezeStrenght;

            if (squeezeLevel > 1)
                squeezeLevel = 1;

            if (linkedInteractible != null && linkedInteractible.IsGrabbed())
            {
                UpdateGrabbedHaptics();
            }
        }

        /// <summary> Fires when this object is grabbed by a GrabScript. Queues a Wrist Squeeze command that will be sent at the end of this frame. </summary>
        /// <param name="thisObj"></param>
        /// <param name="grabbedByHand"></param>
        private void ObjectGrabbed(SG_Interactable thisObj, SG_GrabScript grabbedByHand)
        {
            linkedInteractible.QueueWristSqueeze(squeezeLevel);
        }


        /// <summary> Fires when this object stops being grabbed by a grabscript. Ends wrist squeeze on the hand </summary>
        /// <param name="thisObj"></param>
        /// <param name="grabbedByHand"></param>
        protected virtual void ObjectReleased(SG_Interactable thisObj, SG_GrabScript grabbedByHand)
        {
            if (grabbedByHand != null && grabbedByHand.TrackedHand != null)
                grabbedByHand.TrackedHand.QueueWristSqueeze(0.0f); //end the wirst squeeze on the hand, since it might have already left the Grabable's scope
        }

        /// <summary> Fires each frame while the object is grabbed. We'll keep queueing QueueWristSqueeze(SqueezeLevel) through the linkedInteractable. </summary>
        protected virtual void UpdateGrabbedHaptics()
        {
            linkedInteractible.QueueWristSqueeze(squeezeLevel);
        }

        protected virtual void OnEnable()
        {
            if (linkedInteractible == null) { this.linkedInteractible = this.GetComponent<SG_Interactable>(); }
            if (linkedInteractible != null)
            {
                linkedInteractible.ObjectGrabbed.AddListener(ObjectGrabbed);
                linkedInteractible.ObjectReleased.AddListener(ObjectReleased);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (linkedInteractible != null)
            {
                linkedInteractible.ObjectGrabbed.RemoveListener(ObjectGrabbed);
                linkedInteractible.ObjectReleased.RemoveListener(ObjectReleased);
            }
        }
    }
}
