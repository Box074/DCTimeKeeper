using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCatch : MonoBehaviour
{
    public Rigidbody2D rig;
    public HookController ctrl;
    public GameObject catchGO;
    public Vector3 offset;
    public DamageHero dh;
    private void OnEnable() {
        catchGO = null;
        dh.damageDealt = 1;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.root.CompareTag("Player") && catchGO == null && !ctrl.isReturning)
        {
            rig.velocity = Vector2.zero;
            catchGO = other.transform.root.gameObject;
            offset = catchGO.transform.position - rig.transform.position;
        }
    }
    private void Update() {
        if(catchGO != null)
        {
            dh.damageDealt = 0;
            catchGO.transform.position = rig.transform.position + offset;
        }
    }
}
