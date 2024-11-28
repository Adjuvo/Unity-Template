using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SG;
using UnityEngine.UI;

public class SG_TeleportScript : MonoBehaviour
{
    public enum WhichHand {left, right }

    [Header("For which hand is this teleport script")]
    public WhichHand whichHand = WhichHand.left;

    [Header("Place the sg_trackedhand script from the SGHandRight or left")]
    public SG_TrackedHand handScript;

    [Header("How long till teleport")]
    public float gestureTime = 1;

    [Header("XRRig")]
    public GameObject xrRig;

    [Header("Layer of the ground plane")]
    public LayerMask layerMask;

    [Header("Colour of beam")]
    public Material greenBeam;
    public Material redBeam;

    // If only the event is used do not attach the camera rig to this script
    [Header("This event fires when the teleport timer runs out")]
    public UnityEvent<Vector3> TeleportEvent = new UnityEvent<Vector3>();

    // -- private vars --
    // objects
    private SG_BasicGesture pointGesture;
    private GameObject teleportTimerObject;
    private Slider timer;
    private GameObject beamMain;
    private GameObject beam;
    private SG_GestureLayer gestureLayer;
    private GameObject wrist;
    public Transform cameraPosition;

    // vars
    private float teleportTimer = 0f;
    private float startTimer = 0f;
    private bool validHit = false;
    private Vector3 teleportDestination = new Vector3(0,0,0);

    // pre stashed values
    private Vector3 rightBeam = new Vector3(0.0992f, -0.0161f, 0.0317f);
    private Vector3 leftBeam = new Vector3(0.1033f, -0.0157f, -0.0309f);
    private Vector3 beamScale = new Vector3(0.005f, 0.005f, 0.005f);
    private Vector3 beamRot = new Vector3(0, 0, 0);

    private Vector3 rightCanvasPos = new Vector3(0.0988f, -0.0114f, 0.053f);
    private Vector3 rightCanvasRot = new Vector3(0, -35, -270);
    private Vector3 leftCanvasPos = new Vector3(0.1038f, -0.0115f, -0.0529f);
    private Vector3 leftCanvasRot = new Vector3(0, 40, 90);
    private Vector3 canvasScale = new Vector3(1, 1, 1);

    private void Awake()
    {
        // get the vars needed for this script to work
        GetVars();

        // place the teleport parts on the right spots on the Senseglove hand
        PlaceSubParts();

        beamMain.SetActive(false);
        teleportTimerObject.SetActive(false);

        if (Camera.main != null)
        {
            cameraPosition = Camera.main.transform;
        }
    }

    private void Update()
    {
        bool teleportDesired = pointGesture.IsGesturing || handScript.OverrideUse() > 0.5f;

        // check if the gesture is made
        if (teleportDesired && !handScript.grabScript.IsGrabbing)
        {
            if(startTimer > 0.75)
            {
                beamMain.SetActive(true);
                teleportTimerObject.SetActive(true);
                Beam();
                teleportTimer = validHit ? teleportTimer + Time.deltaTime : 0;
                TimerSize();


                // if the gesture is made for a certain amount of time then teleport
                if (teleportTimer > gestureTime)
                {
                    teleportTimer = 0f;
                    TimerSize();
                    Teleport();
                }
            }
            startTimer = startTimer + Time.deltaTime;
        }
        else
        {
            beamMain.SetActive(false);
            teleportTimerObject.SetActive(false);
            teleportTimer = 0f;
            startTimer = 0f;
            TimerSize();
        }
    }

    private void TimerSize()
    {
        float percent = teleportTimer / gestureTime;


        if (teleportTimer > gestureTime)
            percent = 100;

        timer.value = percent;
    }

    private void Beam()
    {

        Vector3 newPos = Vector3.zero;

        Ray raycast = new Ray(this.beamMain.transform.position, this.beamMain.transform.right);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit, Mathf.Infinity, layerMask);

        float d = 20f;

        if (bHit)
        {
            teleportDestination = hit.point;
            newPos = teleportDestination;
            d = (newPos - beamMain.transform.position).magnitude; //size (m between points)
            validHit = true;

            beamMain.transform.localScale = new Vector3(d, beamMain.transform.localScale.y, beamMain.transform.localScale.z);
            beam.GetComponent<Renderer>().material = greenBeam;
        }
        else
        {
            beamMain.transform.localScale = new Vector3(10, beamMain.transform.localScale.y, beamMain.transform.localScale.z);
            beam.GetComponent<Renderer>().material = redBeam;
            validHit = false;
        }
    }

    private void Teleport()
    {
        if(xrRig != null)
        {
            //xrRig.transform.position = teleportDestination;

            Vector3 dPos = cameraPosition.position - xrRig.transform.position;
            Vector3 newpos = new Vector3 (teleportDestination.x - dPos.x, teleportDestination.y, teleportDestination.z - dPos.z);

            xrRig.transform.position = newpos;
        }

        TeleportEvent.Invoke(teleportDestination);
    }

    // -- Initialize this script --
    private void GetVars()
    {
        // find gesture layer
        gestureLayer = handScript.gestureLayer;

        // find the wrist where the objects(beamMain and timer) are placed
        wrist = handScript.GetTransform(SG_TrackedHand.TrackingLevel.RenderPose, HandJoint.Wrist).gameObject;

        // get and set point gesture
        pointGesture = this.transform.Find("TeleportGesture").gameObject.GetComponent<SG_BasicGesture>();
        SG_BasicGesture[] gestures = gestureLayer.gestures;
        SG_BasicGesture[] gesturesPlus = new SG_BasicGesture[gestures.Length + 1];

        for (int i = 0; i < gestures.Length; i++ ) {
            gesturesPlus[i] = gestures[i];
        }
        gesturesPlus[gesturesPlus.Length - 1] = pointGesture;
        gestureLayer.gestures = gesturesPlus;

        // get the Teleport timer object
        teleportTimerObject = this.transform.Find("TeleportTimer").gameObject;
        // get the slider of the timer
        timer = teleportTimerObject.transform.GetChild(0).gameObject.GetComponent<Slider>();

        // get the Beam main object
        beamMain = this.transform.Find("BeamMain").gameObject;
        // get the Beam
        beam = beamMain.transform.GetChild(0).gameObject;
    }

    private void PlaceSubParts()
    {
        // set the teleport canvas at the right location
        teleportTimerObject.transform.parent = wrist.transform;
        teleportTimerObject.transform.localPosition = whichHand == WhichHand.left ? leftCanvasPos : rightCanvasPos;
        teleportTimerObject.transform.localEulerAngles = whichHand == WhichHand.left ? leftCanvasRot : rightCanvasRot;
        teleportTimerObject.transform.localScale = canvasScale;

        // set the teleport beam at the right location
        beamMain.transform.parent = wrist.transform;
        beamMain.transform.localPosition = whichHand == WhichHand.left ? leftBeam : rightBeam;
        beamMain.transform.localEulerAngles = beamRot;
        beamMain.transform.localScale = beamScale;
    }
}
