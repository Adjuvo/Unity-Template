using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;

public class SG_Dial : SG_Grabable
{
    public SG.Util.MoveAxis rotateAround = SG.Util.MoveAxis.Y;

    protected override void MoveToTargetLocation(Vector3 targetPosition, Quaternion targetRotation, float dT)
    {
        //base.MoveToTargetLocation(targetPosition, targetRotation, dT);

        Transform baseTransform = this.MyTransform;
        baseTransform.rotation = targetRotation;

        Vector3 localEuler = baseTransform.localEulerAngles;

        int axIndex = SG.Util.SG_Util.AxisIndex(rotateAround);
        for (int i=0; i<3; i++)
        {
            if (i != axIndex) { localEuler[i] = 0; }
        }
        baseTransform.localEulerAngles = localEuler;
    }


    protected override void SetupScript()
    {
        base.SetupScript();
        if (this.physicsBody != null)
        {
            this.physicsBody.isKinematic = true;
        }
    }

    // make sure the object can't been moved
    protected virtual void OnValidate()
    {
        this.moveSpeed = 0.0f;
    }
}
