
namespace DCTimeKeeperMod;

class DreamReturn : CSFsm<DreamReturn>
{
    [FsmVar("HUD Blanker White")]
    private FsmGameObject HUDBlankerWhite = new();
    [FsmVar("HUD Canvas")]
    private FsmGameObject HUDCanvas = new();
    [FsmState]
    private IEnumerator Init()
    {
        DefineEvent("DREAM RETURN", nameof(DreamReturnS));
        DefineEvent("DREAM EXIT", nameof(DreamExit));
        yield return StartActionContent;
    }
    [FsmState]
    private IEnumerator DreamExit()
    {
        yield return StartActionContent;
        
    }
    [FsmState]
    private IEnumerator DreamReturnS()
    {
        yield return StartActionContent;
        HeroController.instance.gameObject.LocateMyFSM("Dream Return").FsmVariables.FindFsmBool("Dream Returning").Value = true;
        HUDBlankerWhite.Value.LocateMyFSM("Blanker Control").FsmVariables.FindFsmFloat("Fade Time").Value = 0;
        FSMUtility.SendEventToGameObject(HUDBlankerWhite.Value, "FADE IN");
        HeroController.instance.MaxHealth();
        Camera.main.gameObject.LocateMyFSM("CameraFade").FsmVariables.FindFsmBool("No Fade").Value = true;
        StaticVariableList.SetValue("finishedBossReturning", true);
        FSMUtility.SendEventToGameObject(HUDCanvas.Value, "OUT");
        HeroController.instance.RelinquishControl();
        HeroController.instance.StopAnimationControl();
        HeroController.instance.EnterWithoutInput(true);
        GameManager.instance.BeginSceneTransition(new()
        {
            SceneName = PlayerData.instance.dreamReturnScene,
            EntryGateName = PlayerData.instance.bossReturnEntryGate,
            EntryDelay = 0,
            Visualization = GameManager.SceneLoadVisualizations.GodsAndGlory,
            PreventCameraFadeOut = true
        });
        yield return null;
    }
}
