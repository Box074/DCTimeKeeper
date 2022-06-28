using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockRot : MonoBehaviour
{
    public Transform min;
    public Transform hour;
    public float rotPerSec;
    public float maxPerSec;
    public HealthManager hm;
    void Update()
    {
        float mrot;
        if (hm != null)
        {
            mrot = -Time.deltaTime * Mathf.Lerp(rotPerSec, maxPerSec, Mathf.Abs(hm.hp - 2500) / 2500);
        }
        else
        {
            mrot = -maxPerSec * Time.deltaTime;
        }
        min.Rotate(new Vector3(0, 0, mrot));
        hour.Rotate(new Vector3(0, 0, mrot / 8));
    }
}
