
namespace DCTimeKeeperMod;

class DeadCellsTimeKeeper : ModBaseWithSettings<DeadCellsTimeKeeper, object, BossStatue.Completion>, Modding.ILocalSettings<BossStatue.Completion>
{
    public static Assembly ScriptAssembly = Assembly.Load(ModRes.SCRIPT_ASSEMBLY_BYTE);
    public static AssetBundle Assets = Application.platform switch
    {
        RuntimePlatform.WindowsPlayer => ModRes.AB_WIN,
        RuntimePlatform.LinuxPlayer => ModRes.AB_LINUX,
        RuntimePlatform.OSXPlayer => ModRes.AB_MAC,
        _ => throw new NotSupportedException()
    };
    public static AssetBundle Scene = Application.platform switch
    {
        RuntimePlatform.WindowsPlayer => ModRes.SCENE_WIN,
        RuntimePlatform.LinuxPlayer => ModRes.SCENE_LINUX,
        RuntimePlatform.OSXPlayer => ModRes.SCENE_MAC,
        _ => throw new NotSupportedException()
    };
    public static GameObject TimeKeeperPrefab = Assets.LoadAsset<GameObject>("Assets/TimeKeeper/Prefab/TimeKeeper.prefab");
    public static MusicCue TimeKeeperMusicCue = Assets.LoadAsset<MusicCue>("Assets/TimeKeeper/Music/TimeKeeperMusic.asset");
    public static Sprite TimeKeeperIcon = Assets.LoadAsset<Sprite>("Assets/TimeKeeper/Icon.png");
    public static BossScene TimeKeeperBossScene = Assets.LoadAsset<BossScene>("Assets/TimeKeeper/BossScene.asset");
    public static readonly string sceneName = "ClockRoom";
    [Preload("Crossroads_01", "_SceneManager")]
    public static GameObject SceneManager = null!;
    [Preload("GG_Vengefly", "Boss Scene Controller")]
    public static GameObject BossSceneController = null!;
    public override void Initialize()
    {
        I18n.AddLanguage(GlobalEnums.SupportedLanguages.ZH, ModRes.LANG_ZH);
        I18n.DefaultLanguage = Language.LanguageCode.ZH;
        I18n.UseGameLanguage(GlobalEnums.SupportedLanguages.ZH);
        I18n.UseLanguageHook = true;
        Modding.ModHooks.GetPlayerVariableHook += (type, name, orig) =>
        {
            if (name != "pdTimeKeeper") return orig;
            return localSettings;
        };
        Modding.ModHooks.SetPlayerVariableHook += (type, name, orig) =>
        {
            if (name == "pdTimeKeeper") localSettings = (BossStatue.Completion)orig;
            return orig;
        };
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (prev, next) =>
        {
            if (next.name == "GG_Workshop")
            {
                PrepareStatue();
            }
        };
    }
    private static void CopyFSMTo(Fsm src, PlayMakerFSM dest)
    {
        var template = new FsmTemplate()
        {
            fsm = src
        };
        dest.SetFsmTemplate(template);
        dest.Fsm.Name = src.Name;
    }
    class TempComponent : MonoBehaviour
    {
        private void Update() {
            GameCameras.instance.colorCorrectionCurves.enabled = false;
        }
        private void OnDestroy() {
            GameCameras.instance.colorCorrectionCurves.enabled = true;
        }
        private void OnDisable() {
            GameCameras.instance.colorCorrectionCurves.enabled = true;
        }
    }
    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneName)
        {
            var ctrl = scene.FindGameObject("TimeKeeper")!.GetComponent<TimeKeeperController>();
            ctrl.target = HeroController.instance.gameObject;
            ctrl.cleanInv = () =>
            {
                PlayerData.instance.isInvincible = false;
                HeroController.instance.cState.invulnerable = false;
                HeroController.instance.parryInvulnTimer = 0;
            };
            var sceneEnd = scene.FindGameObject("_BossSceneController")!.GetComponent<BossSceneController>();
            sceneEnd.gameObject.AddComponent<TempComponent>();
            CopyFSMTo(BossSceneController.LocateMyFSM("Dream Return").Fsm, sceneEnd.gameObject.AddComponent<PlayMakerFSM>());
            CopyFSMTo(BossSceneController.LocateMyFSM("Dream Enter Next Scene").Fsm, sceneEnd.gameObject.AddComponent<PlayMakerFSM>());

            IEnumerator Wait()
            {

                yield return new WaitForSeconds(0.15f);
                HeroController.instance.RegainControl();
                HeroController.instance.StartAnimationControl();
                HeroController.instance.AcceptInput();
                PlayMakerFSM.BroadcastEvent("FADE OUT");
            }
            Wait().StartCoroutine();
        }
    }
    private static void PrepareStatue()
    {
        var template = GameObject.Find("GG_Statue");
        var statue = UnityEngine.Object.Instantiate(template, new(23.6765f, 6.4081f), Quaternion.identity);
        var statueC = statue.GetComponent<BossStatue>();
        statueC.bossScene = TimeKeeperBossScene;
        statueC.bossDetails = new()
        {
            nameKey = "TIME_KEEPER_NAME",
            nameSheet = "UI",
            descriptionKey = "TIME_KEEPER_DESC",
            descriptionSheet = "UI"
        };
        statueC.isAlwaysUnlocked = true;
        statueC.statueStatePD = "pdTimeKeeper";
        var icon = new GameObject("Statue");
        icon.transform.parent = statue.FindChildWithPath("Base", "Statue")!.transform;
        icon.transform.localPosition = new(0, 1.88f);
        var iconS = icon.AddComponent<SpriteRenderer>();
        iconS.sprite = TimeKeeperIcon;
        iconS.drawMode = SpriteDrawMode.Sliced;
        iconS.size = new(2, 3);
        statueC.Start();
        statueC.UpdateDetails();
    }
    public static void Test()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}
