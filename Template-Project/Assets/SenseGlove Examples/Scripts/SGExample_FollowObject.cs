using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SGExample_FollowObject : MonoBehaviour
{
    public GameObject followObject;

    public float speed = 1.0f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, followObject.transform.position, speed * Time.deltaTime);
        transform.rotation = followObject.transform.rotation;
    }
}
