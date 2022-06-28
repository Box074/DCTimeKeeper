using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeSword : MonoBehaviour
{
    public float saveTime = 0;
    public Rigidbody2D rig;
    public Renderer r;
    private void Awake() {
        saveTime = Time.time;
    }
    private void Update() {
        if(Time.time - saveTime >= 0.75f)
        {
            rig.isKinematic = false;
        }
        if(!r.isVisible && (Time.time - saveTime >= 4.5f))
        {
            Destroy(gameObject);
        }
    }
}
