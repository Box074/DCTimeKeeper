using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogRot : MonoBehaviour
{
    public float rotPerSec;
    public Transform big;
    void Update()
    {
        var mrot = -rotPerSec * Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, mrot));
        big.RotateAround(transform.position, new Vector3(0, 0, 1), mrot * 0.5f);
    }
}
