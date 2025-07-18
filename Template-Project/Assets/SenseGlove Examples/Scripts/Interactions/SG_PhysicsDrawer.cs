using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SG_PhysicsDrawer : MonoBehaviour
{
    public enum moveAngleEnum { x, y, z }

    [Header("Translate through local angle")]
    public moveAngleEnum localPositionAngle = moveAngleEnum.x;

    [Header("Max distance pulled out drawer")]
    public float maxDistance = 1f;

    [Header("Start position of the drawer")]
    public Vector3 startPosition = new Vector3(0, 0, 0);

    private void Update()
    {
        switch (localPositionAngle)
        {
            case moveAngleEnum.x:
                // don't let the drawer move any other direction that the one indicated by localPositionAngle
                if (this.transform.localPosition.y != 0 || this.transform.localPosition.z != 0)
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, 0, 0);
                // if the drawer is drawn deeper than the startposition place the drawer at the start position
                if (this.transform.localPosition.x < startPosition.x)
                    this.transform.localPosition = new Vector3(startPosition.x, 0, 0);
                // if the drawer is drawn further than the maxDistance place the drawer at the maxDistance
                if (this.transform.localPosition.x > maxDistance)
                    this.transform.localPosition = new Vector3(maxDistance, 0, 0);
                break;
            case moveAngleEnum.y:
                // don't let the drawer move any other direction that the one indicated by localPositionAngle
                if (this.transform.localPosition.x != 0 || this.transform.localPosition.z != 0)
                    this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);
                // if the drawer is drawn deeper than the startposition place the drawer at the start position
                if (this.transform.localPosition.y < startPosition.y)
                    this.transform.localPosition = new Vector3(0, startPosition.y, 0);
                // if the drawer is drawn further than the maxDistance place the drawer at the maxDistance
                if (this.transform.localPosition.y > maxDistance)
                    this.transform.localPosition = new Vector3(0, maxDistance, 0);
                break;
            case moveAngleEnum.z:
                // don't let the drawer move any other direction that the one indicated by localPositionAngle
                if (this.transform.localPosition.x != 0 || this.transform.localPosition.y != 0)
                    this.transform.localPosition = new Vector3(0, 0, this.transform.localPosition.z);
                // if the drawer is drawn deeper than the startposition place the drawer at the start position
                if (this.transform.localPosition.z < startPosition.z)
                    this.transform.localPosition = new Vector3(0, 0, startPosition.z);
                // if the drawer is drawn further than the maxDistance place the drawer at the maxDistance
                if (this.transform.localPosition.y > maxDistance)
                    this.transform.localPosition = new Vector3(0, 0, maxDistance);
                break;
        }
    }
}
