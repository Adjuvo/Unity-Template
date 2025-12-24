using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class SG_PicoPassthrough : MonoBehaviour
{
    public bool startWithSeethrough = true;

    private void Start()
    {
        PXR_Manager.EnableVideoSeeThrough = startWithSeethrough ? true : false;
    }

    public void StartSeeThrough()
    {
        PXR_Manager.EnableVideoSeeThrough = false;
    }

    public void StopSeeThrough()
    {
        PXR_Manager.EnableVideoSeeThrough = false;
    }
}
