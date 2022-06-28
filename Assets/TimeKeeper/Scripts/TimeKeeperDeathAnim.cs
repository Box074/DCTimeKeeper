using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeperDeathAnim : MonoBehaviour
{
    public TimeKeeperAnim anim;
    public GameObject door;
    public SpriteRenderer fake;
    public GameObject frag;
    public bool deathEx;
    // Start is called before the first frame update
    void OnEnable()
    {
        door.SetActive(false);
        StartCoroutine(Death());
    }
    private IEnumerator Death()
    {
        fake.enabled = false;
        anim.gameObject.SetActive(true);
        anim.PlayLoop("idle");
        var ots = Time.timeScale;
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(2);
        
        anim.ftScale = 0.25f;
        anim.PlayR("stunIdle", 5);
        yield return anim.Wait();
        anim.ftScale = 1;
        anim.PlayLoop("stun");
        yield return new WaitForSeconds(1.25f);
        Time.timeScale = ots;
        if(deathEx)
        {
            anim.gameObject.SetActive(false);
            frag.SetActive(true);
            foreach(var v in frag.GetComponentsInChildren<Rigidbody2D>())
            {
                v.velocity = new Vector2(Random.Range(-4, 4),10 * Random.value);
            }
            yield break;
        }
        yield return anim.PlayWait("stunIdle");
        yield return anim.PlayWait("idleCast");
        anim.PlayLoop("cast");
        door.transform.localScale = Vector3.zero;
        door.SetActive(true);
        var startTime = Time.time;
        while(true)
        {
            var span = Time.time - startTime;
            door.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, span * 0.5f);
            if(span >= 2) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.35f);
        startTime = Time.time;
        fake.enabled = true;
        anim.gameObject.SetActive(false);
        while(true)
        {
            var span = Time.time - startTime;
            fake.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), span);
            if(span >= 1f) break;
            yield return null;
        }
        fake.enabled = false;
        yield return new WaitForSeconds(0.55f);
        startTime = Time.time;
        while(true)
        {
            var span = Time.time - startTime;
            door.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, span);
            if(span >= 1) break;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
