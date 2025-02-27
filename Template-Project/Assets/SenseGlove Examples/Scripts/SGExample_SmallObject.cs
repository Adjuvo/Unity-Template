using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGExample
{
    /// <summary>
    ///  If the tracker defines this object the closest then this script is used to make this object grabable and changes the material of the object.
    /// </summary>


    public class SGExample_SmallObject : MonoBehaviour
    {
        private SG.SG_Grabable grabable;

        [Header("materials")]
        public Material white;
        public Material red;

        public SGExample_SmallObjectPickup hover;

        private void Start()
        {
            grabable = GetComponent<SG.SG_Grabable>();
        }

        public void MakeGrabable()
        {
            this.gameObject.GetComponent<MeshRenderer>().material = red;
            grabable.enabled = true;
        }

        public void MakeNonGrabable()
        {
            this.gameObject.GetComponent<MeshRenderer>().material = white;
            grabable.enabled = false;
        }

        public void RemoveFromPickingUp()
        {
            MakeNonGrabable();
            hover.RemoveFromList(this.gameObject);
            this.tag = "Untagged";
            this.GetComponent<BoxCollider>().enabled = false;
            Destroy(this.GetComponent<SGExample_SmallObject>());
        }
    }
}
