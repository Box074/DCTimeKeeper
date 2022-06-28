using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public GameObject shotPoint;
    public SpriteRenderer chain;
    public float leftTime = 0;
    public bool isReturning = false;
    private void OnEnable() {
        transform.localEulerAngles = new Vector3(0, 0, 180);
        leftTime = 1.35f;
        isReturning = false;
    }
    void Update()
    {
        
        if(leftTime < 0 && !isReturning)
        {
            isReturning = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        leftTime -= Time.deltaTime;
        var distance = Vector2.Distance(shotPoint.transform.position, transform.position);
        if(distance > 100)
        {
            distance = 0;
        }
        chain.size = new Vector2(distance, 0.4090909f);
        var selfP = transform.position;
        var masterP = shotPoint.transform.position;
        var rot = Mathf.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, rot);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == 8)
        {
            isReturning = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
