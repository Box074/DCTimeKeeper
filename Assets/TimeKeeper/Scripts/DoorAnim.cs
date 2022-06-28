using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    private void OnEnable() {
        StartCoroutine(Loop());
    }
    private IEnumerator Loop()
    {
        var startTime = Time.time;
        while(true)
        {
            var span = Time.time - startTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, span * 0.5f);
            if(span >= 2) break;
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
