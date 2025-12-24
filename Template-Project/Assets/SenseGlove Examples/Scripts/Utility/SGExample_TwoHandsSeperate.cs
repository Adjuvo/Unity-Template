using SG;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


namespace SGExample
{
    /// <summary>
    ///  The Main script to use an object with both hands seperatly
    /// </summary>

    public enum whichObject { firstObject, secondObject }

    public enum whichObjectGrabbed { none, firstObject, secondObject, both}

    public enum ObjectMovementphases { none, freeMovement, onlyYAxisCollision, onlyYAxisGrabbed, follow }

    public class SGExample_TwoHandsSeperate : MonoBehaviour
    {
        public whichObjectGrabbed partsGrabbed = whichObjectGrabbed.none;

        public GameObject firstObject;
        public GameObject secondObject;

        [Header("The minimum and maximum the second object is allowed to move")]
        public float minY = 0f;
        public float maxY = 1f;

        [Header("The difference of the first and second object in the y direction")]
        public float yDif = 0f;

        [Header("The speed the second object follows the second object")]
        public float followSpeed = 10f;

        // private vars
        private SGExample_THS_Object firstObjectGrab;
        private SGExample_THS_Object secondObjectGrab;

        private int handsCounter = 0;

        private bool[] colliding = { false, false };

        private void Start()
        {
            firstObjectGrab = firstObject.GetComponent<SGExample_THS_Object>();
            secondObjectGrab = secondObject.GetComponent<SGExample_THS_Object>();

            firstObjectGrab.main = this;
            secondObjectGrab.main = this;

            UpdateObjects();
        }

        public void CountHands(bool addHand, whichObject which)
        {
            switch(partsGrabbed)
            {
                case whichObjectGrabbed.none:
                    partsGrabbed = which == whichObject.firstObject ? whichObjectGrabbed.firstObject : whichObjectGrabbed.secondObject;
                    break;
                case whichObjectGrabbed.firstObject:
                    partsGrabbed = addHand ? whichObjectGrabbed.both : whichObjectGrabbed.none;
                    break;
                case whichObjectGrabbed.secondObject:
                    partsGrabbed = addHand ? whichObjectGrabbed.both : whichObjectGrabbed.none;
                    break;  
                case whichObjectGrabbed.both:
                    partsGrabbed = which == whichObject.firstObject ? whichObjectGrabbed.secondObject : whichObjectGrabbed.firstObject;
                    break;
            }

            handsCounter = addHand ? handsCounter + 1 : handsCounter - 1;

            firstObjectGrab.handCounter = handsCounter;
            secondObjectGrab.handCounter = handsCounter;

            UpdateObjects();
        }

        /// <summary> Setting the movements limits of the two objects </summary>
        private void UpdateObjects()
        {
            ObjectMovementphases firstMov = ObjectMovementphases.freeMovement;
            ObjectMovementphases secondMov = ObjectMovementphases.freeMovement;

            Rigidbody firstRigid = firstObject.GetComponent<Rigidbody>();

            switch (partsGrabbed)
            {
                case whichObjectGrabbed.none:
                    firstMov = ObjectMovementphases.none;
                    secondMov = colliding[1] ? ObjectMovementphases.onlyYAxisCollision : ObjectMovementphases.follow;
                    firstRigid.useGravity = true;
                    break;
                case whichObjectGrabbed.firstObject:
                    firstMov = ObjectMovementphases.freeMovement;
                    secondMov = colliding[1] ? ObjectMovementphases.onlyYAxisCollision : ObjectMovementphases.follow;
                    break;
                case whichObjectGrabbed.secondObject:
                    firstMov = colliding[0] ? ObjectMovementphases.onlyYAxisCollision : ObjectMovementphases.follow;
                    secondMov = ObjectMovementphases.freeMovement;
                    firstRigid.useGravity = false;
                    break;
                case whichObjectGrabbed.both:
                    firstMov = ObjectMovementphases.freeMovement;
                    secondMov = ObjectMovementphases.onlyYAxisGrabbed;
                    break;
            }

            firstObjectGrab.objectMov = firstMov;
            secondObjectGrab.objectMov = secondMov;
        }

        private void SetObject()
        {

        }

        /// <summary> which object is colliding other objects </summary>
        public void ObjectsColliding(whichObject objectNumber, bool enterCol)
        {
            switch (objectNumber)
            {
                case whichObject.firstObject:
                    colliding[0] = enterCol ? true : false;
                    break; 
                case whichObject.secondObject:
                    colliding[1] = enterCol ? true : false;
                    break;
            }
        }

    }
}
