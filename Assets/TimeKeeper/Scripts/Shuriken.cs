using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public Rigidbody2D rig;

    private void FixedUpdate() {
        if(Mathf.Abs(rig.velocity.x) < 0.3) Destroy(rig.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == 8)
        {
            Destroy(rig.gameObject);
        }
    }
}
