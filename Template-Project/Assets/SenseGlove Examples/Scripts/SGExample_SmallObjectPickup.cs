using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;

namespace SGExample
{
    /// <summary>
    ///  Create a hover that checks which objects with a specific Tag, are inside of it and checks which one is the closest to the tracker. 
    ///  The tracker is situated between the thumb and index finger.
    ///  The closest object with a specific Tag will become grabable and can be moved around with the fingers of the hand
    /// </summary>

    public class SGExample_SmallObjectPickup : MonoBehaviour
    {
        [Header("Place the sg_trackedhand script from the SGHandRight or SGHandLeft")]
        public SG_TrackedHand hand;

        [Header("The tracker to find the closest object with the smallObject tag")]
        public GameObject tracker;

        [Header("The tag that is used for the small objects")]
        public string tagObject = "smallObject";

        // The two fingers the tracker needs to hover inbetween
        private GameObject thumb;
        private GameObject index;

        // The hover to not make a distance check with all small objects in scene every frame
        private GameObject hover;

        // The wrist the hover is placed on
        private GameObject wrist;

        private List<GameObject> objectsInHover = new List<GameObject>();

        private GameObject oldRed = null;

        private void Start()
        {
            hover = this.gameObject;

            wrist = hand.GetTransform(SG_TrackedHand.TrackingLevel.RenderPose, HandJoint.Wrist).gameObject;

            // place the hover on the wrist with an small offset so its center is at the fingers
            hover.transform.parent = wrist.transform;
            hover.transform.localPosition = new Vector3(0.138f, 0, 0);

            thumb = hand.GetTransform(SG_TrackedHand.TrackingLevel.RenderPose, HandJoint.Thumb_FingerTip).gameObject;
            index = hand.GetTransform(SG_TrackedHand.TrackingLevel.RenderPose, HandJoint.Index_FingerTip).gameObject;
        }


        private void Update()
        {
            // set the tracker in the position between the thumb and the index
            tracker.transform.position = 0.5f * (thumb.transform.position + index.transform.position);

            if (objectsInHover.Count > 0)
            {
                GameObject obj = CheckDistance();

                if (obj != oldRed)
                {
                    if (oldRed != null)
                        oldRed.GetComponent<SGExample_SmallObject>().MakeNonGrabable();

                    obj.GetComponent<SGExample_SmallObject>().MakeGrabable();
                    oldRed = obj;
                }
            }
        }

        /// <summary> Check the different distances of all small objects inside the hover object. And then find the closest object to the tracker and return that object </summary>
        private GameObject CheckDistance()
        {
            GameObject obj = null;
            float smallestDist = 10f;

            for (int i = 0; i < objectsInHover.Count; i++)
            {
                float dist = Vector3.Distance(objectsInHover[i].transform.position, tracker.transform.position);
                if (dist < smallestDist)
                {
                    smallestDist = dist;
                    obj = objectsInHover[i];
                }
            }
            return obj;
        }

        private void OnTriggerStay(Collider other)
        {

            if (hand == null && other.tag == tagObject || !hand.grabScript.IsGrabbing && other.tag == tagObject)
            {
                for (int i = 0; i < objectsInHover.Count; i++)
                {
                    // check if the object is already inside the hover list
                    if (objectsInHover[i] == other.gameObject)
                    {
                        return;
                    }
                }
                // If not add the object to the list
                objectsInHover.Add(other.gameObject);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == tagObject)
            {
                for (int i = 0; i < objectsInHover.Count; i++)
                {
                    // check if the object is already inside the hover list
                    if (objectsInHover[i] == other.gameObject)
                    {
                        objectsInHover[i].GetComponent<SGExample_SmallObject>().MakeNonGrabable();
                        objectsInHover.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}
