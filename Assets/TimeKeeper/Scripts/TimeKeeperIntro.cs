using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeperIntro : MonoBehaviour
{
    public GameObject timeKeeper;
    public TimeKeeperAnim anim;
    public GameObject door;
    private void OnEnable() {
        StartCoroutine(Loop());
    }
    private IEnumerator Loop()
    {
        anim.PlayLoop("idle");
        while(door?.activeInHierarchy ?? false) yield return null;
        anim.PlayLoop("idle");
        yield return new WaitForSeconds(0.75f);
        timeKeeper.transform.position = transform.position;
        timeKeeper.transform.localScale = transform.localScale;
        timeKeeper.SetActive(true);
        gameObject.SetActive(false);
    }
}
