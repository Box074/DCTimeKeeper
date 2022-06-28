using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeperController : MonoBehaviour
{
    public enum Actions
    {
        None, Dash, Shuriken, Atk, Hook, ShurikenPlus
    }
    public bool disableAI = false;
    public GameObject door;
    public Actions nextAction = 0;
    public Actions prevAction = 0;
    public TimeKeeperAnim anim;
    public CommonAnimControl[] effects;
    private Dictionary<string, CommonAnimControl> effectMap = new Dictionary<string, CommonAnimControl>();
    public HealthManager hm;
    public Rigidbody2D rig;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    private Dictionary<string, AudioClip> clipMap;
    public bool Phase2 = false;
    public GameObject target;
    public float runSpeed;
    public GameObject Hook;
    public Transform HookShot;
    [Header("Prefab")]
    public GameObject HookPrefab;
    public GameObject DeathPrefab;
    public GameObject HugeSwordPrefab;
    public GameObject ShurikenPrefab;
    [Header("Skill")]
    public GameObject DashBox;
    public GameObject ShurikenSpawnPoint;
    public GameObject AtkA0Box;
    public GameObject AtkA1Box;
    public GameObject AtkB0Box;
    public GameObject AtkB1Box;
    public System.Action cleanInv;


    private void Awake()
    {
        clipMap = new Dictionary<string, AudioClip>();
        foreach (var v in audioClips)
        {
            clipMap[v.name] = v;
        }
        foreach (var v in effects)
        {
            effectMap[v.name] = v;
        }
    }
    private void PlayOneShot(string name)
    {
        audioSource.Stop();
        if (clipMap.TryGetValue(name, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
    }
    private void PlayEffect(string name)
    {
        var effect = effectMap[name];
        effect.gameObject.SetActive(true);
        effect.currentAnimFrame = 0;
        effect.Play();
    }
    public bool IsFacingRight => transform.localScale.x > 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(CommandChoice());
        StartCoroutine(CommandExecuter());
        StartCoroutine(DieCheck());
        StartCoroutine(P2Check());
        StartCoroutine(HitCheck());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator CommandChoice()
    {
        while (door?.activeInHierarchy ?? false) yield return null;
        while (true)
        {
            while (nextAction != 0 || disableAI) yield return null;
            nextAction = (Actions)Random.Range(0, 5);
            var dis = Mathf.Abs(transform.position.x - target.transform.position.x);
            if (nextAction == Actions.Atk)
            {
                if (dis > 5) nextAction = (Actions)Random.Range(0, 5);
                if (dis > 6) continue;
            }
            if (nextAction == Actions.Shuriken || nextAction == Actions.Hook)
            {
                if (dis < 5) nextAction = Random.value < 0.5f ? Actions.Dash : Actions.Atk;
            }
            if (prevAction == Actions.Atk && nextAction == Actions.Atk)
            {
                nextAction = Actions.Dash;
            }
            if (prevAction == Actions.Dash && nextAction == Actions.Dash)
            {
                nextAction = Actions.Atk;
            }
            if (nextAction == prevAction) continue;
            yield return new WaitForSeconds(0.125f);
        }
    }
    private IEnumerator DieCheck()
    {
        while (true)
        {
            if (hm.isDead)
            {
                Instantiate(DeathPrefab, transform.position, Quaternion.identity).transform.localScale = transform.localScale;
                Destroy(gameObject);
            }
            yield return null;
        }
    }
    private IEnumerator P2Check()
    {
        while (true)
        {
            while (!Phase2) yield return null;
            yield return new WaitForSeconds(Random.Range(1.15f, 2));
            Instantiate(HugeSwordPrefab, new Vector3(target.transform.position.x + Random.Range(-2, 2), transform.position.y + 12, 0), Quaternion.identity);
        }
    }
    private IEnumerator HitCheck()
    {
        int lastHp = 0;
        while (true)
        {
            if (hm.hp < lastHp)
            {
                anim.rendererMat.SetColor("_Color", Color.red);
                yield return new WaitForSeconds(0.125f);
                anim.rendererMat.SetColor("_Color", Color.white);
            }
            if(hm.hp <= 1250)
            {
                Phase2 = true;
            }
            lastHp = hm.hp;
            yield return null;
        }
    }
    private IEnumerator CommandExecuter()
    {
        while (true)
        {
            if (nextAction != 0)
            {
                switch (nextAction)
                {
                    case Actions.Dash:
                        yield return Dash();
                        break;
                    case Actions.Shuriken:
                        yield return ThrowShuriken();
                        break;
                    case Actions.Atk:
                        yield return Atk();
                        break;
                    case Actions.Hook:
                        yield return ThrowHook();
                        break;
                    default:
                        break;
                }
                prevAction = nextAction;
                nextAction = 0;
            }

            anim.PlayLoop("idle");
            yield return null;
        }
    }
    private void FreezeY(bool enabled)
    {
        rig.constraints = enabled ? rig.constraints | RigidbodyConstraints2D.FreezePositionY : rig.constraints ^ RigidbodyConstraints2D.FreezePositionY;
    }
    private void Face(bool right)
    {
        var ls = transform.localScale;
        ls.x = Mathf.Abs(ls.x) * (right ? 1 : -1);
        transform.localScale = ls;
    }
    private void FaceTarget()
    {
        Face(target.transform.position.x >= transform.position.x);
    }
    class ObjHolder<T>
    {
        public T value;
    }
    private IEnumerator RunToTarget(float maxTime, ObjHolder<bool> isTimeout)
    {
        FaceTarget();
        isTimeout.value = false;
        if (Mathf.Abs(target.transform.position.x - transform.position.x) < 1.5f) yield break;
        anim.ftScale = 0.5f;
        yield return anim.PlayWait("idleRun");
        var bt = Time.time;
        anim.PlayLoop("run");
        var vel = new Vector2(IsFacingRight ? runSpeed : -runSpeed, 0);
        FreezeY(true);
        while (Mathf.Abs(target.transform.position.x - transform.position.x) > 1.5f)
        {
            if(Time.time - bt >= maxTime)
            {
                isTimeout.value = true;
                break;
            }
            if(IsFacingRight && target.transform.position.x <= transform.position.x) break;
            if(!IsFacingRight && target.transform.position.x >= transform.position.x) break;
            rig.velocity = vel;
            yield return null;
        }
        rig.velocity = Vector2.zero;
        anim.ftScale = 1;
    }
    private IEnumerator AtkA(bool fast = false)
    {
        FaceTarget();
        cleanInv?.Invoke();
        if (!fast) yield return anim.PlayWait("atkLoadA");
        anim.Play("atkA");
        yield return anim.WaitToFrame(6);
        PlayOneShot("enm_berserk_release1");
        AtkA0Box.SetActive(true);
        yield return anim.WaitToFrame(7);
        AtkA0Box.SetActive(false);
        AtkA1Box.SetActive(true);
        yield return anim.WaitToFrame(8);
        AtkA1Box.SetActive(false);
    }
    private IEnumerator Atk()
    {
        FaceTarget();
        ObjHolder<bool> isTimeout = new ObjHolder<bool>();
        yield return RunToTarget(5, isTimeout);
        if(isTimeout.value) yield break;
        if (Mathf.Abs(target.transform.position.x - transform.position.x) > 1.5f) yield break;
        anim.ftScale = 0.75f;
        PlayOneShot("enm_berserk_rootcharge");
        yield return anim.PlayWait("idleAtkLoadA");

        yield return AtkA();

        cleanInv?.Invoke();
        yield return anim.PlayWait("atkLoadB");
        anim.Play("atkB");
        yield return anim.WaitToFrame(7);
        PlayOneShot("weapon_broadsword_release3");
        AtkB0Box.SetActive(true);
        yield return anim.WaitToFrame(8);
        AtkB0Box.SetActive(false);
        AtkB1Box.SetActive(true);
        yield return anim.WaitToFrame(9);
        AtkB1Box.SetActive(false);
        yield return anim.WaitToFrame(12);

        if (Phase2 && Mathf.Abs(target.transform.position.x - transform.position.x) < 2.5f)
        {
            yield return AtkA();
        }
        yield return anim.Wait();
        anim.ftScale = 1;
    }
    private IEnumerator ThrowHook()
    {
        FaceTarget();
        var hookRig = Hook.GetComponent<Rigidbody2D>();
        anim.PlayLoop("throwLoad");
        var lockPos = target.GetComponent<Collider2D>().bounds.center;
        PlayOneShot("enm_berserk_hookcharge");
        Hook.transform.localPosition = Vector3.zero;
        var vel = new Vector2(60 * transform.localScale.x, lockPos.y - Hook.transform.position.y);
        yield return new WaitForSeconds(1);

        anim.Play("throw");
        yield return anim.WaitToFrame(3);


        Hook.SetActive(true);
        hookRig.velocity = vel;
        PlayOneShot("enm_berserk_hookshot");
        while (Mathf.Abs(hookRig.velocity.x) >= 0.1f) yield return null;
        yield return new WaitForSeconds(0.5f);
        hookRig.velocity = -vel * 2;
        if (IsFacingRight)
        {
            while (Hook.transform.position.x > HookShot.position.x) yield return null;
        }
        else
        {
            while (Hook.transform.position.x < HookShot.position.x) yield return null;
        }
        Hook.SetActive(false);
        anim.Play("catch");
        yield return anim.WaitToFrame(4);
        cleanInv?.Invoke();
        yield return AtkA(true);
    }
    private IEnumerator ThrowShuriken()
    {
        FaceTarget();
        int count = Phase2 ? Random.Range(5, 7) : Random.Range(3, 5);
        yield return anim.PlayWait("throwShurikenLoad");
        for (; count > 0; count--)
        {
            anim.ftScale = 0.35f;
            FaceTarget();
            anim.Play("throwShuriken");
            yield return anim.WaitToFrame(4);
            PlayEffect("fxThrowShuriken");
            PlayOneShot("weapon_kunai_release");
            var s = Instantiate(ShurikenPrefab, ShurikenSpawnPoint.transform.position, Quaternion.identity);
            var srig = s.GetComponent<Rigidbody2D>();
            var yz = target.transform.position.y - s.transform.position.y;
            srig.velocity = new Vector2(45 * transform.localScale.x, Random.Range(yz - 5, yz + 5));
            yield return anim.Wait();
            if (count > 1)
            {
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            }
            anim.ftScale = 1;
            if(Mathf.Abs(transform.position.x - target.transform.position.x) < 5) break;
        }
    }

    private IEnumerator Dash(bool isDouble = false)
    {
        FreezeY(true);
        PlayOneShot("enm_berserk_charge3");
        if (!isDouble)
        {
            FaceTarget();
            yield return anim.PlayWait("idleDashLoad");
        }
        else
        {
            var scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
        }

        yield return anim.PlayWait("dashLoad");
        PlayOneShot("enm_berserk_release2");
        cleanInv?.Invoke();
        anim.PlayLoop("dash");
        DashBox.SetActive(true);
        var speed = new Vector2(IsFacingRight ? 50 : -50, 0);
        rig.velocity = speed;
        yield return null;
        while (Mathf.Abs(rig.velocity.x) >= 0.5)
        {
            rig.velocity = speed;
            yield return null;
        }
        rig.velocity = Vector2.zero;
        DashBox.SetActive(false);
        FreezeY(false);
        if (!isDouble && Phase2 && Random.value < 0.65f)
        {
            yield return Dash(true);
        }
        else
        {
            yield return anim.PlayWait("dashIdle");
            anim.PlayLoop("idle");
        }
    }
}
