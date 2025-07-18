using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;

public class SG_CustomDrawer : SG_Grabable
{
    public enum moveAngleEnum { x, y, z }

    [Header("Translate through which local angle")]
    public moveAngleEnum localPositionAngle = moveAngleEnum.x;

    [Header("Max distance the drawer can be pulled out")]
    public float maxDistance = 1f;

    // private vars
    private GameObject drawer;

    private Vector3 startPosition = new Vector3(0, 0, 0);
    private Vector3 startRotation = new Vector3(0, 0, 0);

    protected override void Start()
    {
        base.Start();

        // set the drawer object (the parent of the handle)
        drawer = this.transform.parent.gameObject;

        // set the start position and rotation of the drawer
        startPosition = drawer.transform.localPosition;
        startRotation = drawer.transform.localEulerAngles;
    }

    protected override void MoveToTargetLocation(Vector3 targetPosition, Quaternion targetRotation, float dT)
    {
        if (this.IsGrabbed())
        {
            Transform baseHandleTransform = this.MyTransform;
            Vector3 localPos = baseTransform.localPosition;

            baseTransform.position = targetPosition;

            switch (localPositionAngle)
            {
                case moveAngleEnum.x:
                    localPos = new Vector3(baseTransform.localPosition.x, startPosition.y, startPosition.z);
                    break;
                case moveAngleEnum.y:
                    localPos = new Vector3(startPosition.x, baseTransform.localPosition.y, startPosition.z);
                    break;
                case moveAngleEnum.z:
                    localPos = new Vector3(startPosition.x, startPosition.y, baseTransform.localPosition.z);
                    break;
            }

            baseTransform.localPosition = localPos;
        }
    }

    private void Update()
    {
        switch (localPositionAngle)
        {
            case moveAngleEnum.x:
                // don't let the drawer move any other direction that the one indicated by localPositionAngle
                if (drawer.transform.localPosition.y != 0 || drawer.transform.localPosition.z != 0)
                    drawer.transform.localPosition = new Vector3(drawer.transform.localPosition.x, 0, 0);
                // if the drawer is drawn deeper than the startposition place the drawer at the start position
                if (drawer.transform.localPosition.x < startPosition.x)
                    drawer.transform.localPosition = new Vector3(startPosition.x, 0, 0);
                // if the drawer is drawn further than the maxDistance place the drawer at the maxDistance
                if (drawer.transform.localPosition.x > maxDistance)
                    drawer.transform.localPosition = new Vector3(maxDistance, 0, 0);
                break;
            case moveAngleEnum.y:
                // don't let the drawer move any other direction that the one indicated by localPositionAngle
                if (drawer.transform.localPosition.x != 0 || drawer.transform.localPosition.z != 0)
                    drawer.transform.localPosition = new Vector3(0, drawer.transform.localPosition.y, 0);
                // if the drawer is drawn deeper than the startposition place the drawer at the start position
                if (drawer.transform.localPosition.y < startPosition.y)
                    drawer.transform.localPosition = new Vector3(0, startPosition.y, 0);
                // if the drawer is drawn further than the maxDistance place the drawer at the maxDistance
                if (drawer.transform.localPosition.y > maxDistance)
                    drawer.transform.localPosition = new Vector3(0, maxDistance, 0);
                break;
            case moveAngleEnum.z:
                // don't let the drawer move any other direction that the one indicated by localPositionAngle
                if (drawer.transform.localPosition.x != 0 || drawer.transform.localPosition.y != 0)
                    drawer.transform.localPosition = new Vector3(0, 0, drawer.transform.localPosition.z);
                // if the drawer is drawn deeper than the startposition place the drawer at the start position
                if (drawer.transform.localPosition.z < startPosition.z)
                    drawer.transform.localPosition = new Vector3(0, 0, startPosition.z);
                // if the drawer is drawn further than the maxDistance place the drawer at the maxDistance
                if (drawer.transform.localPosition.y > maxDistance)
                    drawer.transform.localPosition = new Vector3(0, 0, maxDistance);
                break;
        }
    }
}
