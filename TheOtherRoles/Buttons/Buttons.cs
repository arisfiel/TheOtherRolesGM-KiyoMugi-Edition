using HarmonyLib;
using Hazel;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TheOtherRolesGM;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using System.Linq;
using TheOtherRoles.Utilities;

namespace TheOtherRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    static class HudManagerStartPatch
    {
        private static CustomButton engineerRepairButton;
        private static CustomButton janitorCleanButton;
        private static CustomButton timeMasterShieldButton;
        private static CustomButton medicShieldButton;
        private static CustomButton shifterShiftButton;
        private static CustomButton morphlingButton;
        private static CustomButton camouflagerButton;
        private static CustomButton evilHackerButton;
        private static CustomButton evilHackerCreatesMadmateButton;
        private static CustomButton hackerButton;
        private static CustomButton hackerVitalsButton;
        private static CustomButton hackerAdminTableButton;
        private static CustomButton trackerTrackPlayerButton;
        private static CustomButton trackerTrackCorpsesButton;
        private static CustomButton vampireKillButton;
        private static CustomButton garlicButton;
        private static CustomButton jackalKillButton;
        private static CustomButton sidekickKillButton;
        private static CustomButton jackalSidekickButton;
        private static CustomButton eraserButton;
        private static CustomButton placeJackInTheBoxButton;
        private static CustomButton lightsOutButton;
        //public static CustomButton showInfoOverlay;
        public static CustomButton cleanerCleanButton;
        public static CustomButton warlockCurseButton;
        public static CustomButton securityGuardButton;
        public static CustomButton securityGuardCamButton;
        public static CustomButton arsonistButton;
        public static CustomButton arsonistIgniteButton;
        public static CustomButton vultureEatButton;
        public static CustomButton mediumButton;
        public static CustomButton pursuerButton;
        public static CustomButton witchSpellButton;
        private static CustomButton portalmakerPlacePortalButton;
        private static CustomButton usePortalButton;
        public static CustomButton assassinButton;
        public static CustomButton underTakerButton;

        public static TMPro.TMP_Text vultureNumCorpsesText;
        public static TMPro.TMP_Text securityGuardButtonScrewsText;
        public static TMPro.TMP_Text securityGuardChargesText;
        public static TMPro.TMP_Text pursuerButtonBlanksText;
        public static TMPro.TMP_Text hackerAdminTableChargesText;
        public static TMPro.TMP_Text hackerVitalsChargesText;

        public static PoolablePlayer morphTargetDisplay;

        public static void setCustomButtonCooldowns()
        {
            engineerRepairButton.MaxTimer = 0f;
            janitorCleanButton.MaxTimer = Janitor.cooldown;
            timeMasterShieldButton.MaxTimer = TimeMaster.cooldown;
            medicShieldButton.MaxTimer = 0f;
            shifterShiftButton.MaxTimer = 0f;
            morphlingButton.MaxTimer = Morphling.cooldown;
            camouflagerButton.MaxTimer = Camouflager.cooldown;
            evilHackerButton.MaxTimer = 0f;
            evilHackerCreatesMadmateButton.MaxTimer = 0f;
            hackerButton.MaxTimer = Hacker.cooldown;
            hackerVitalsButton.MaxTimer = Hacker.cooldown;
            hackerAdminTableButton.MaxTimer = Hacker.cooldown;
            vampireKillButton.MaxTimer = Vampire.cooldown;
            trackerTrackPlayerButton.MaxTimer = 0f;
            garlicButton.MaxTimer = 0f;
            jackalKillButton.MaxTimer = Jackal.cooldown;
            sidekickKillButton.MaxTimer = Sidekick.cooldown;
            jackalSidekickButton.MaxTimer = Jackal.createSidekickCooldown;
            eraserButton.MaxTimer = Eraser.cooldown;
            placeJackInTheBoxButton.MaxTimer = Trickster.placeBoxCooldown;
            lightsOutButton.MaxTimer = Trickster.lightsOutCooldown;
            cleanerCleanButton.MaxTimer = Cleaner.cooldown;
            warlockCurseButton.MaxTimer = Warlock.cooldown;
            securityGuardButton.MaxTimer = SecurityGuard.cooldown;
            securityGuardCamButton.MaxTimer = SecurityGuard.cooldown;
            arsonistButton.MaxTimer = Arsonist.cooldown;
            vultureEatButton.MaxTimer = Vulture.cooldown;
            mediumButton.MaxTimer = Medium.cooldown;
            pursuerButton.MaxTimer = Pursuer.cooldown;
            trackerTrackCorpsesButton.MaxTimer = Tracker.corpsesTrackingCooldown;
            witchSpellButton.MaxTimer = Witch.cooldown;
            portalmakerPlacePortalButton.MaxTimer = Portalmaker.cooldown;
            usePortalButton.MaxTimer = Portalmaker.usePortalCooldown;
            assassinButton.MaxTimer = Assassin.cooldown;

            timeMasterShieldButton.EffectDuration = TimeMaster.shieldDuration;
            hackerButton.EffectDuration = Hacker.duration;
            hackerVitalsButton.EffectDuration = Hacker.duration;
            hackerAdminTableButton.EffectDuration = Hacker.duration;
            vampireKillButton.EffectDuration = Vampire.delay;
            camouflagerButton.EffectDuration = Camouflager.duration;
            morphlingButton.EffectDuration = Morphling.duration;
            lightsOutButton.EffectDuration = Trickster.lightsOutDuration;
            arsonistButton.EffectDuration = Arsonist.duration;
            mediumButton.EffectDuration = Medium.duration;
            trackerTrackCorpsesButton.EffectDuration = Tracker.corpsesTrackingDuration;
            witchSpellButton.EffectDuration = Witch.spellCastingDuration;
            securityGuardCamButton.EffectDuration = SecurityGuard.duration;
            // Already set the timer to the max, as the button is enabled during the game and not available at the start
            lightsOutButton.Timer = lightsOutButton.MaxTimer;

            arsonistIgniteButton.MaxTimer = 0f;
            arsonistIgniteButton.Timer = 0f;

            ButtonsGM.setCustomButtonCooldowns();
        }

        public static void resetTimeMasterButton()
        {
            timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
            timeMasterShieldButton.isEffectActive = false;
            timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        }

        public static void Postfix(HudManager __instance)
        {
            // Engineer Repair
            engineerRepairButton = new CustomButton(
                () =>
                {
                    engineerRepairButton.Timer = 0f;

                    MessageWriter usedRepairWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EngineerUsedRepair, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(usedRepairWriter);
                    RPCProcedure.engineerUsedRepair();

                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    {
                        if (task.TaskType == TaskTypes.FixLights)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EngineerFixLights, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.engineerFixLights();
                        }
                        else if (task.TaskType == TaskTypes.RestoreOxy)
                        {
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                        }
                        else if (task.TaskType == TaskTypes.ResetReactor)
                        {
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 16);
                        }
                        else if (task.TaskType == TaskTypes.ResetSeismic)
                        {
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Laboratory, 16);
                        }
                        else if (task.TaskType == TaskTypes.FixComms)
                        {
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                        }
                        else if (task.TaskType == TaskTypes.StopCharles)
                        {
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                            MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
                        }
                        else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EngineerFixSubmergedOxygen, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.engineerFixSubmergedOxygen();
                        }
                    }
                },
                () => { return Engineer.engineer != null && Engineer.engineer == PlayerControl.LocalPlayer && Engineer.remainingFixes > 0 && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    bool sabotageActive = false;
                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                        if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles
                            || SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                            sabotageActive = true;
                    return sabotageActive && Engineer.remainingFixes > 0 && PlayerControl.LocalPlayer.CanMove;
                },
                () => { },
                Engineer.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            engineerRepairButton.buttonText = ModTranslation.getString("RepairText");

            // Janitor Clean
            janitorCleanButton = new CustomButton(
                () =>
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.cleanBody(playerInfo.PlayerId);
                                    janitorCleanButton.Timer = janitorCleanButton.MaxTimer;

                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; },
                () => { janitorCleanButton.Timer = janitorCleanButton.MaxTimer; },
                Janitor.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            janitorCleanButton.buttonText = ModTranslation.getString("CleanText");


            // Time Master Rewind Time
            timeMasterShieldButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TimeMasterShield, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.timeMasterShield();
                },
                () => { return TimeMaster.timeMaster != null && TimeMaster.timeMaster == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                    timeMasterShieldButton.isEffectActive = false;
                    timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                TimeMaster.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F,
                true,
                TimeMaster.shieldDuration,
                () => { timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer; }
            );
            timeMasterShieldButton.buttonText = ModTranslation.getString("TimeShieldText");

            // Medic Shield
            medicShieldButton = new CustomButton(
                () =>
                {
                    medicShieldButton.Timer = 0f;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, Medic.setShieldAfterMeeting ? (byte)CustomRPC.SetFutureShielded : (byte)CustomRPC.MedicSetShielded, Hazel.SendOption.Reliable, -1);
                    writer.Write(Medic.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    if (Medic.setShieldAfterMeeting)
                        RPCProcedure.setFutureShielded(Medic.currentTarget.PlayerId);
                    else
                        RPCProcedure.medicSetShielded(Medic.currentTarget.PlayerId);
                },
                () => { return Medic.medic != null && Medic.medic == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return !Medic.usedShield && Medic.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { },
                Medic.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            medicShieldButton.buttonText = ModTranslation.getString("ShieldText");


            // Shifter shift
            shifterShiftButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFutureShifted, Hazel.SendOption.Reliable, -1);
                    writer.Write(Shifter.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
                },
                () => { return Shifter.shifter != null && Shifter.shifter == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return Shifter.currentTarget && Shifter.futureShift == null && PlayerControl.LocalPlayer.CanMove; },
                () => { },
                Shifter.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            shifterShiftButton.buttonText = ModTranslation.getString("ShiftText");

            // Morphling morph
            morphlingButton = new CustomButton(
                () =>
                {
                    if (Morphling.sampledTarget != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MorphlingMorph, Hazel.SendOption.Reliable, -1);
                        writer.Write(Morphling.sampledTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.morphlingMorph(Morphling.sampledTarget.PlayerId);
                        Morphling.sampledTarget = null;
                        morphlingButton.EffectDuration = Morphling.duration;
                    }
                    else if (Morphling.currentTarget != null)
                    {
                        Morphling.sampledTarget = Morphling.currentTarget;
                        morphlingButton.Sprite = Morphling.getMorphSprite();
                        morphlingButton.buttonText = ModTranslation.getString("MorphText");
                        morphlingButton.EffectDuration = 1f;

                        // Add poolable player to the button so that the target outfit is shown
                        morphlingButton.actionButton.cooldownTimerText.transform.localPosition = new Vector3(0, 0, -1f);  // Before the poolable player
                        morphTargetDisplay = UnityEngine.Object.Instantiate<PoolablePlayer>(Patches.IntroCutsceneOnDestroyPatch.playerPrefab, morphlingButton.actionButton.transform);
                        GameData.PlayerInfo data = Morphling.sampledTarget.Data;
                        Morphling.sampledTarget.SetPlayerMaterialColors(morphTargetDisplay.cosmetics.currentBodySprite.BodySprite);
                        morphTargetDisplay.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                        morphTargetDisplay.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                         //PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, morphTargetDisplay.PetSlot);
                        morphTargetDisplay.cosmetics.nameText.text = "";  // Hide the name!
                        morphTargetDisplay.transform.localPosition = new Vector3(0f, 0.22f, -0.01f);
                        morphTargetDisplay.transform.localScale = Vector3.one * 0.33f;
                        morphTargetDisplay.setSemiTransparent(false);
                        morphTargetDisplay.gameObject.SetActive(true);
                    }
                },
                () => { return Morphling.morphling != null && Morphling.morphling == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return (Morphling.currentTarget || Morphling.sampledTarget) && PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    morphlingButton.Timer = morphlingButton.MaxTimer;
                    morphlingButton.Sprite = Morphling.getSampleSprite();
                    morphlingButton.buttonText = ModTranslation.getString("SampleText");
                    morphlingButton.isEffectActive = false;
                    morphlingButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                    Morphling.sampledTarget = null;
                    if (morphTargetDisplay != null)
                    {  // Reset the poolable player
                        morphTargetDisplay.gameObject.SetActive(false);
                        GameObject.Destroy(morphTargetDisplay.gameObject);
                        morphTargetDisplay = null;
                    }
                },
                Morphling.getSampleSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F,
                true,
                Morphling.duration,
                () =>
                {
                    if (Morphling.sampledTarget == null)
                    {
                        morphlingButton.Timer = morphlingButton.MaxTimer;
                        morphlingButton.Sprite = Morphling.getSampleSprite();
                        morphlingButton.buttonText = ModTranslation.getString("SampleText");

                        // Reset the poolable player
                        morphTargetDisplay.gameObject.SetActive(false);
                        GameObject.Destroy(morphTargetDisplay.gameObject);
                        morphTargetDisplay = null;
                    }
                }
            );
            morphlingButton.buttonText = ModTranslation.getString("SampleText");

            // Camouflager camouflage
            camouflagerButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CamouflagerCamouflage, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.camouflagerCamouflage();
                },
                () => { return Camouflager.camouflager != null && Camouflager.camouflager == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    camouflagerButton.Timer = camouflagerButton.MaxTimer;
                    camouflagerButton.isEffectActive = false;
                    camouflagerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Camouflager.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F,
                true,
                Camouflager.duration,
                () => { camouflagerButton.Timer = camouflagerButton.MaxTimer; }
            );
            camouflagerButton.buttonText = ModTranslation.getString("CamoText");

            // EvilHacker button
            evilHackerButton = new CustomButton(
                () =>
                {
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                    Action<MapBehaviour> tmpAction = (MapBehaviour m) => { m.ShowCountOverlay(); };
                    FastDestroyableSingleton<HudManager>.Instance.ShowMap(tmpAction);
                },
                () =>
                {
                    return EvilHacker.evilHacker != null &&
                    EvilHacker.evilHacker == PlayerControl.LocalPlayer &&
                    PlayerControl.LocalPlayer.isAlive();
                },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () => { },
                EvilHacker.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F,
                false,
                0f,
                () => { },
                PlayerControl.GameOptions.MapId == 3,
                FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
            );

            // EvilHacker creates madmate button
            evilHackerCreatesMadmateButton = new CustomButton(
                () =>
                {
                    /*
                     * creates madmate
                     */
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EvilHackerCreatesMadmate, Hazel.SendOption.Reliable, -1);
                    writer.Write(EvilHacker.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.evilHackerCreatesMadmate(EvilHacker.currentTarget.PlayerId);
                },
                () =>
                {
                    return EvilHacker.evilHacker != null &&
                    EvilHacker.evilHacker == PlayerControl.LocalPlayer &&
                    EvilHacker.canCreateMadmate &&
                    PlayerControl.LocalPlayer.isAlive();
                },
                () => { return EvilHacker.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { },
                EvilHacker.getMadmateButtonSprite(),
                new Vector3(-2.7f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                null
            );
            evilHackerCreatesMadmateButton.buttonText = ModTranslation.getString("MadmateText");

            // Hacker button
            hackerButton = new CustomButton(
                () =>
                {
                    Hacker.hackerTimer = Hacker.duration;
                },
                () => { return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return true; },
                () =>
                {
                    hackerButton.Timer = hackerButton.MaxTimer;
                    hackerButton.isEffectActive = false;
                    hackerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Hacker.getButtonSprite(),
                new Vector3(0f, 1f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F,
                true,
                0f,
                () => { hackerButton.Timer = hackerButton.MaxTimer; }
            );
            hackerButton.buttonText = ModTranslation.getString("HackerText");

            hackerAdminTableButton = new CustomButton(
                () =>
                {
                    if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                        FastDestroyableSingleton<HudManager>.Instance.ShowMap((System.Action<MapBehaviour>)(m => m.ShowCountOverlay()));

                    if (Hacker.cantMove) PlayerControl.LocalPlayer.moveable = false;
                    PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                    Hacker.chargesAdminTable--;
                },
                () => { return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer && MapOptions.couldUseAdmin && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    if (hackerAdminTableChargesText != null)
                        hackerAdminTableChargesText.text = hackerVitalsChargesText.text = String.Format(ModTranslation.getString("hackerChargesText"), Hacker.chargesAdminTable, Hacker.toolsNumber);
                    return Hacker.chargesAdminTable > 0 && MapOptions.canUseAdmin; ;
                },
                () =>
                {
                    hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                    hackerAdminTableButton.isEffectActive = false;
                    hackerAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Hacker.getAdminSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.Q,
                true,
                0f,
                () =>
                {
                    hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                    if (!hackerVitalsButton.isEffectActive) PlayerControl.LocalPlayer.moveable = true;
                    if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
                },
                PlayerControl.GameOptions.MapId == 3,
                FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
            );

            // Hacker Admin Table Charges
            hackerAdminTableChargesText = GameObject.Instantiate(hackerAdminTableButton.actionButton.cooldownTimerText, hackerAdminTableButton.actionButton.cooldownTimerText.transform.parent);
            hackerAdminTableChargesText.text = "";
            hackerAdminTableChargesText.enableWordWrapping = false;
            hackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
            hackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            hackerVitalsButton = new CustomButton(
                () =>
                {
                    if (PlayerControl.GameOptions.MapId != 1)
                    {
                        if (Hacker.vitals == null)
                        {
                            var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("panel_vitals"));
                            if (e == null || Camera.main == null) return;
                            Hacker.vitals = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                        }
                        Hacker.vitals.transform.SetParent(Camera.main.transform, false);
                        Hacker.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                        Hacker.vitals.Begin(null);
                    }
                    else
                    {
                        if (Hacker.doorLog == null)
                        {
                            var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                            if (e == null || Camera.main == null) return;
                            Hacker.doorLog = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                        }
                        Hacker.doorLog.transform.SetParent(Camera.main.transform, false);
                        Hacker.doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                        Hacker.doorLog.Begin(null);
                    }

                    if (Hacker.cantMove) PlayerControl.LocalPlayer.moveable = false;
                    PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement

                    Hacker.chargesVitals--;
                },
                () => { return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer && MapOptions.couldUseVitals && PlayerControl.LocalPlayer.isAlive() && PlayerControl.GameOptions.MapId != 0 && PlayerControl.GameOptions.MapId != 3; },
                () =>
                {
                    if (hackerVitalsChargesText != null)
                        hackerVitalsChargesText.text = String.Format(ModTranslation.getString("hackerChargesText"), Hacker.chargesVitals, Hacker.toolsNumber);
                    hackerVitalsButton.actionButton.graphic.sprite = PlayerControl.GameOptions.MapId == 1 ? Hacker.getLogSprite() : Hacker.getVitalsSprite();
                    hackerVitalsButton.actionButton.OverrideText(PlayerControl.GameOptions.MapId == 1 ?
                        TranslationController.Instance.GetString(StringNames.DoorlogLabel) :
                        TranslationController.Instance.GetString(StringNames.VitalsLabel));
                    return Hacker.chargesVitals > 0 && MapOptions.canUseVitals;
                },
                () =>
                {
                    hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                    hackerVitalsButton.isEffectActive = false;
                    hackerVitalsButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Hacker.getVitalsSprite(),
                new Vector3(-2.7f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.Q,
                true,
                0f,
                () =>
                {
                    hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                    if (!hackerAdminTableButton.isEffectActive) PlayerControl.LocalPlayer.moveable = true;
                    if (Minigame.Instance)
                    {
                        if (PlayerControl.GameOptions.MapId == 1) Hacker.doorLog.ForceClose();
                        else Hacker.vitals.ForceClose();
                    }
                },
                false,
                PlayerControl.GameOptions.MapId == 1 ?
                    TranslationController.Instance.GetString(StringNames.DoorlogLabel) :
                    TranslationController.Instance.GetString(StringNames.VitalsLabel)
            );

            // Hacker Vitals Charges
            hackerVitalsChargesText = GameObject.Instantiate(hackerVitalsButton.actionButton.cooldownTimerText, hackerVitalsButton.actionButton.cooldownTimerText.transform.parent);
            hackerVitalsChargesText.text = "";
            hackerVitalsChargesText.enableWordWrapping = false;
            hackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
            hackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Tracker button
            trackerTrackPlayerButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TrackerUsedTracker, Hazel.SendOption.Reliable, -1);
                    writer.Write(Tracker.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.trackerUsedTracker(Tracker.currentTarget.PlayerId);
                },
                () => { return Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return PlayerControl.LocalPlayer.CanMove && Tracker.currentTarget != null && !Tracker.usedTracker; },
                () => { if (Tracker.resetTargetAfterMeeting) Tracker.resetTracked(); },
                Tracker.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            trackerTrackPlayerButton.buttonText = ModTranslation.getString("TrackerText");

            trackerTrackCorpsesButton = new CustomButton(
                () => { Tracker.corpsesTrackingTimer = Tracker.corpsesTrackingDuration; },
                () => { return Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive() && Tracker.canTrackCorpses; },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                    trackerTrackCorpsesButton.isEffectActive = false;
                    trackerTrackCorpsesButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Tracker.getTrackCorpsesButtonSprite(),
                new Vector3(-2.7f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.Q,
                true,
                Tracker.corpsesTrackingDuration,
                () =>
                {
                    trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                }
            );
            trackerTrackCorpsesButton.buttonText = ModTranslation.getString("PathfindText");

            vampireKillButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murder = Helpers.checkMuderAttempt(Vampire.vampire, Vampire.currentTarget);
                    if (murder == MurderAttemptResult.PerformKill)
                    {
                        if (Vampire.targetNearGarlic)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                            writer.Write(Vampire.vampire.PlayerId);
                            writer.Write(Vampire.currentTarget.PlayerId);
                            writer.Write(Byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.uncheckedMurderPlayer(Vampire.vampire.PlayerId, Vampire.currentTarget.PlayerId, Byte.MaxValue);

                            vampireKillButton.HasEffect = false; // Block effect on this click
                            vampireKillButton.Timer = vampireKillButton.MaxTimer;
                        }
                        else
                        {
                            Vampire.bitten = Vampire.currentTarget;
                            // Notify players about bitten
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                            writer.Write(Vampire.bitten.PlayerId);
                            writer.Write((byte)0);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.vampireSetBitten(Vampire.bitten.PlayerId, 0);

                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.delay, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.checkMuderAttemptAndKill(Vampire.vampire, Vampire.bitten, showAnimation: false);
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                                    writer.Write(byte.MaxValue);
                                    writer.Write(byte.MaxValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                                }
                            })));

                            vampireKillButton.HasEffect = true; // Trigger effect on this click
                        }
                    }
                    else if (murder == MurderAttemptResult.BlankKill)
                    {
                        vampireKillButton.Timer = vampireKillButton.MaxTimer;
                        vampireKillButton.HasEffect = false;
                    }
                },
                () => { return Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    if (Vampire.targetNearGarlic && Vampire.canKillNearGarlics)
                    {
                        vampireKillButton.Sprite = __instance.KillButton.graphic.sprite;
                        vampireKillButton.buttonText = TranslationController.Instance.GetString(StringNames.KillLabel);
                    }
                    else
                    {
                        vampireKillButton.Sprite = Vampire.getButtonSprite();
                        vampireKillButton.buttonText = ModTranslation.getString("VampireText");
                    }
                    return Vampire.currentTarget != null && PlayerControl.LocalPlayer.CanMove && (!Vampire.targetNearGarlic || Vampire.canKillNearGarlics);
                },
                () =>
                {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    vampireKillButton.isEffectActive = false;
                    vampireKillButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Vampire.getButtonSprite(),
                new Vector3(0, 1f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.Q,
                false,
                0f,
                () =>
                {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                }
            );
            vampireKillButton.buttonText = ModTranslation.getString("VampireText");

            garlicButton = new CustomButton(
                () =>
                {
                    Vampire.localPlacedGarlic = true;
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceGarlic, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeGarlic(buff);
                },
                () => { return !Vampire.localPlacedGarlic && PlayerControl.LocalPlayer.isAlive() && Vampire.garlicsActive && !PlayerControl.LocalPlayer.isGM(); },
                () => { return PlayerControl.LocalPlayer.CanMove && !Vampire.localPlacedGarlic; },
                () => { },
                Vampire.getGarlicButtonSprite(),
                new Vector3(0, -0.06f, 0),
                __instance,
                __instance.UseButton,
                null,
                true
            );
            garlicButton.buttonText = ModTranslation.getString("GarlicText");


            // Jackal Sidekick Button
            jackalSidekickButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.JackalCreatesSidekick, Hazel.SendOption.Reliable, -1);
                    writer.Write(Jackal.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.jackalCreatesSidekick(Jackal.currentTarget.PlayerId);
                },
                () => { return Jackal.canCreateSidekick && Jackal.jackal != null && Jackal.jackal == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return Jackal.canCreateSidekick && Jackal.currentTarget != null && PlayerControl.LocalPlayer.CanMove; },
                () => { jackalSidekickButton.Timer = jackalSidekickButton.MaxTimer; },
                Jackal.getSidekickButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            jackalSidekickButton.buttonText = ModTranslation.getString("SidekickText");

            // Jackal Kill
            jackalKillButton = new CustomButton(
                () =>
                {
                    if (Helpers.checkMuderAttemptAndKill(Jackal.jackal, Jackal.currentTarget) == MurderAttemptResult.SuppressKill) return;

                    jackalKillButton.Timer = jackalKillButton.MaxTimer;
                    Jackal.currentTarget = null;
                },
                () => { return PlayerControl.LocalPlayer.isRole(RoleType.Jackal) && PlayerControl.LocalPlayer.isAlive(); },
                () => { return Jackal.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { jackalKillButton.Timer = jackalKillButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                new Vector3(0, 1f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.Q
            );

            // Sidekick Kill
            sidekickKillButton = new CustomButton(
                () =>
                {
                    if (Helpers.checkMuderAttemptAndKill(Sidekick.sidekick, Sidekick.currentTarget) == MurderAttemptResult.SuppressKill) return;
                    sidekickKillButton.Timer = sidekickKillButton.MaxTimer;
                    Sidekick.currentTarget = null;
                },
                () => { return Sidekick.canKill && Sidekick.sidekick != null && Sidekick.sidekick == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return Sidekick.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { sidekickKillButton.Timer = sidekickKillButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                new Vector3(0, 1f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.Q
            );


            // Eraser erase button
            eraserButton = new CustomButton(
                () =>
                {
                    eraserButton.MaxTimer += Eraser.cooldownIncrease;
                    eraserButton.Timer = eraserButton.MaxTimer;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFutureErased, Hazel.SendOption.Reliable, -1);
                    writer.Write(Eraser.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureErased(Eraser.currentTarget.PlayerId);
                },
                () => { return Eraser.eraser != null && Eraser.eraser == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return PlayerControl.LocalPlayer.CanMove && Eraser.currentTarget != null; },
                () => { eraserButton.Timer = eraserButton.MaxTimer; },
                Eraser.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            eraserButton.buttonText = ModTranslation.getString("EraserText");

            placeJackInTheBoxButton = new CustomButton(
                () =>
                {
                    placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                    var pos = PlayerControl.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceJackInTheBox, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeJackInTheBox(buff);
                },
                () => { return Trickster.trickster != null && Trickster.trickster == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive() && !JackInTheBox.hasJackInTheBoxLimitReached(); },
                () => { return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.hasJackInTheBoxLimitReached(); },
                () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer; },
                Trickster.getPlaceBoxButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            placeJackInTheBoxButton.buttonText = ModTranslation.getString("PlaceJackInTheBoxText");

            lightsOutButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LightsOut, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lightsOut();
                },
                () => { return Trickster.trickster != null && Trickster.trickster == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive() && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents; },
                () => { return PlayerControl.LocalPlayer.CanMove && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents; },
                () =>
                {
                    lightsOutButton.Timer = lightsOutButton.MaxTimer;
                    lightsOutButton.isEffectActive = false;
                    lightsOutButton.actionButton.graphic.color = Palette.EnabledColor;
                },
                Trickster.getLightsOutButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F,
                true,
                Trickster.lightsOutDuration,
                () => { lightsOutButton.Timer = lightsOutButton.MaxTimer; }
            );
            lightsOutButton.buttonText = ModTranslation.getString("LightsOutText");

            // Cleaner Clean
            cleanerCleanButton = new CustomButton(
                () =>
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.cleanBody(playerInfo.PlayerId);

                                    Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return Cleaner.cleaner != null && Cleaner.cleaner == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; },
                () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
                Cleaner.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            cleanerCleanButton.buttonText = ModTranslation.getString("CleanText");

            // Warlock curse
            warlockCurseButton = new CustomButton(
                () =>
                {
                    if (Warlock.curseVictim == null)
                    {
                        // Apply Curse
                        Warlock.curseVictim = Warlock.currentTarget;
                        warlockCurseButton.Sprite = Warlock.getCurseKillButtonSprite();
                        warlockCurseButton.Timer = 1f;
                        warlockCurseButton.buttonText = ModTranslation.getString("CurseKillText");
                    }
                    else if (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)
                    {
                        MurderAttemptResult murder = Helpers.checkMuderAttemptAndKill(Warlock.warlock, Warlock.curseVictimTarget, showAnimation: false);
                        if (murder == MurderAttemptResult.SuppressKill) return;

                        // If blanked or killed
                        warlockCurseButton.buttonText = ModTranslation.getString("CurseText");
                        if (Warlock.rootTime > 0)
                        {
                            PlayerControl.LocalPlayer.moveable = false;
                            PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.rootTime, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                {
                                    PlayerControl.LocalPlayer.moveable = true;
                                }
                            })));
                        }

                        Warlock.curseVictim = null;
                        Warlock.curseVictimTarget = null;
                        warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                        Warlock.warlock.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;

                    }
                },
                () => { return Warlock.warlock != null && Warlock.warlock == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () => { return ((Warlock.curseVictim == null && Warlock.currentTarget != null) || (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)) && PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                    warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                    warlockCurseButton.buttonText = ModTranslation.getString("CurseText");
                    Warlock.curseVictim = null;
                    Warlock.curseVictimTarget = null;
                },
                Warlock.getCurseButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            warlockCurseButton.buttonText = ModTranslation.getString("CurseText");

            // Security Guard button
            securityGuardButton = new CustomButton(
                () =>
                {
                    if (SecurityGuard.ventTarget != null)
                    { // Seal vent
                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SealVent, Hazel.SendOption.Reliable);
                        writer.WritePacked(SecurityGuard.ventTarget.Id);
                        writer.EndMessage();
                        RPCProcedure.sealVent(SecurityGuard.ventTarget.Id);
                        SecurityGuard.ventTarget = null;

                    }
                    else if (PlayerControl.GameOptions.MapId != 1 && !SubmergedCompatibility.IsSubmerged)
                    { // Place camera if there's no vent and it's not MiraHQ or Submerged
                        var pos = PlayerControl.LocalPlayer.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        byte roomId;
                        try
                        {
                            roomId = (byte)FastDestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom.RoomId;
                        }
                        catch
                        {
                            roomId = 255;
                        }

                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceCamera, Hazel.SendOption.Reliable);
                        writer.WriteBytesAndSize(buff);
                        writer.Write(roomId);
                        writer.EndMessage();
                        RPCProcedure.placeCamera(buff, roomId);
                    }
                    securityGuardButton.Timer = securityGuardButton.MaxTimer;
                },
                () => { return SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive() && SecurityGuard.remainingScrews >= Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice); },
                () =>
                {
                    securityGuardButton.actionButton.graphic.sprite = (SecurityGuard.ventTarget == null && PlayerControl.GameOptions.MapId != 1 && !SubmergedCompatibility.IsSubmerged) ? SecurityGuard.getPlaceCameraButtonSprite() : SecurityGuard.getCloseVentButtonSprite();
                    if (SecurityGuard.ventTarget == null && PlayerControl.GameOptions.MapId != 1)
                    {
                        securityGuardButton.buttonText = ModTranslation.getString("PlaceCameraText");
                        securityGuardButton.Sprite = SecurityGuard.getPlaceCameraButtonSprite();
                    }
                    else
                    {
                        securityGuardButton.buttonText = ModTranslation.getString("CloseVentText");
                        securityGuardButton.Sprite = SecurityGuard.getCloseVentButtonSprite();
                    }
                    if (securityGuardButtonScrewsText != null) securityGuardButtonScrewsText.text = String.Format(ModTranslation.getString("securityGuardScrews"), SecurityGuard.remainingScrews);

                    if (SecurityGuard.ventTarget != null)
                    {
                        return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice && PlayerControl.LocalPlayer.CanMove;
                    }

                    return PlayerControl.GameOptions.MapId != 1 && !SubmergedCompatibility.IsSubmerged && SecurityGuard.remainingScrews >= SecurityGuard.camPrice && PlayerControl.LocalPlayer.CanMove;
                },
                () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
                SecurityGuard.getPlaceCameraButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            securityGuardButton.buttonText = ModTranslation.getString("PlaceCameraText");

            // Security Guard button screws counter
            securityGuardButtonScrewsText = GameObject.Instantiate(securityGuardButton.actionButton.cooldownTimerText, securityGuardButton.actionButton.cooldownTimerText.transform.parent);
            securityGuardButtonScrewsText.text = "";
            securityGuardButtonScrewsText.enableWordWrapping = false;
            securityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
            securityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            securityGuardCamButton = new CustomButton(
                () =>
                {
                    if (PlayerControl.GameOptions.MapId != 1)
                    {
                        if (SecurityGuard.minigame == null)
                        {
                            byte mapId = PlayerControl.GameOptions.MapId;
                            var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("Surv_Panel"));
                            if (mapId == 0 || mapId == 3) e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                            else if (mapId == 4) e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                            if (e == null || Camera.main == null) return;
                            SecurityGuard.minigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                        }
                        SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                        SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                        SecurityGuard.minigame.Begin(null);
                    }
                    else
                    {
                        if (SecurityGuard.minigame == null)
                        {
                            var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                            if (e == null || Camera.main == null) return;
                            SecurityGuard.minigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                        }
                        SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                        SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                        SecurityGuard.minigame.Begin(null);
                    }
                    SecurityGuard.charges--;

                    if (SecurityGuard.cantMove) PlayerControl.LocalPlayer.moveable = false;
                    PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                },
                () =>
                {
                    return SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews < Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice)
                            && !SubmergedCompatibility.IsSubmerged;
                },
                () =>
                {
                    if (securityGuardChargesText != null)
                        securityGuardChargesText.text = securityGuardChargesText.text = String.Format(ModTranslation.getString("hackerChargesText"), SecurityGuard.charges, SecurityGuard.maxCharges);
                    securityGuardCamButton.actionButton.graphic.sprite = PlayerControl.GameOptions.MapId == 1 ? SecurityGuard.getLogSprite() : SecurityGuard.getCamSprite();
                    securityGuardCamButton.actionButton.OverrideText(PlayerControl.GameOptions.MapId == 1 ?
                        TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) :
                        TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));
                    return PlayerControl.LocalPlayer.CanMove && SecurityGuard.charges > 0;
                },
                () =>
                {
                    securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                    securityGuardCamButton.isEffectActive = false;
                    securityGuardCamButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                SecurityGuard.getCamSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.Q,
                true,
                0f,
                () =>
                {
                    securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                    if (Minigame.Instance)
                    {
                        SecurityGuard.minigame.ForceClose();
                    }
                    PlayerControl.LocalPlayer.moveable = true;
                },
                false,
                PlayerControl.GameOptions.MapId == 1 ?
                    TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) :
                    TranslationController.Instance.GetString(StringNames.SecurityCamsSystem)
            );

            // Security Guard cam button charges
            securityGuardChargesText = GameObject.Instantiate(securityGuardCamButton.actionButton.cooldownTimerText, securityGuardCamButton.actionButton.cooldownTimerText.transform.parent);
            securityGuardChargesText.text = "";
            securityGuardChargesText.enableWordWrapping = false;
            securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
            securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Arsonist button
            arsonistButton = new CustomButton(
                () =>
                {
                    if (Arsonist.currentTarget != null)
                    {
                        Arsonist.douseTarget = Arsonist.currentTarget;
                    }
                },
                () => { return Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer && !Arsonist.dousedEveryone && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    if (arsonistButton.isEffectActive && Arsonist.douseTarget != Arsonist.currentTarget)
                    {
                        Arsonist.douseTarget = null;
                        arsonistButton.Timer = 0f;
                        arsonistButton.isEffectActive = false;
                    }

                    return PlayerControl.LocalPlayer.CanMove && Arsonist.currentTarget != null;
                },
                () =>
                {
                    arsonistButton.Timer = arsonistButton.MaxTimer;
                    arsonistButton.isEffectActive = false;
                    Arsonist.douseTarget = null;
                    Arsonist.updateStatus();
                },
                Arsonist.getDouseSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F,
                true,
                Arsonist.duration,
                () =>
                {
                    if (Arsonist.douseTarget != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ArsonistDouse, Hazel.SendOption.Reliable, -1);
                        writer.Write(Arsonist.douseTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.arsonistDouse(Arsonist.douseTarget.PlayerId);
                    }

                    Arsonist.douseTarget = null;
                    Arsonist.updateStatus();
                    arsonistButton.Timer = Arsonist.dousedEveryone ? 0 : arsonistButton.MaxTimer;

                    foreach (PlayerControl p in Arsonist.dousedPlayers)
                    {
                        if (MapOptions.playerIcons.ContainsKey(p.PlayerId))
                        {
                            MapOptions.playerIcons[p.PlayerId].setSemiTransparent(false);
                        }
                    }
                }
            );
            arsonistButton.buttonText = ModTranslation.getString("DouseText");

            arsonistIgniteButton = new CustomButton(
                () =>
                {
                    if (Arsonist.dousedEveryone)
                    {
                        MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ArsonistWin, Hazel.SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.arsonistWin();
                    }
                },
                () => { return Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer && Arsonist.dousedEveryone && PlayerControl.LocalPlayer.isAlive(); },
                () => { return PlayerControl.LocalPlayer.CanMove && Arsonist.dousedEveryone; },
                () => { },
                Arsonist.getIgniteSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.Q
            );
            arsonistIgniteButton.buttonText = ModTranslation.getString("IgniteText");

            // Vulture Eat
            vultureEatButton = new CustomButton(
                () =>
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VultureEat, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.vultureEat(playerInfo.PlayerId);

                                    Vulture.cooldown = vultureEatButton.Timer = vultureEatButton.MaxTimer;
                                    break;
                                }
                            }
                        }
                    }
                    if (Vulture.eatenBodies >= Vulture.vultureNumberToWin)
                    {
                        MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VultureWin, Hazel.SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.vultureWin();
                        return;
                    }
                },
                () => { return Vulture.vulture != null && Vulture.vulture == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    if (vultureNumCorpsesText != null)
                        vultureNumCorpsesText.text = String.Format(ModTranslation.getString("vultureCorpses"), Vulture.vultureNumberToWin - Vulture.eatenBodies);
                    return __instance.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove;
                },
                () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
                Vulture.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            vultureEatButton.buttonText = ModTranslation.getString("VultureText");

            vultureNumCorpsesText = GameObject.Instantiate(vultureEatButton.actionButton.cooldownTimerText, vultureEatButton.actionButton.cooldownTimerText.transform.parent);
            vultureNumCorpsesText.text = "";
            vultureNumCorpsesText.enableWordWrapping = false;
            vultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
            vultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);

            // Medium button
            mediumButton = new CustomButton(
                () =>
                {
                    if (Medium.target != null)
                    {
                        Medium.soulTarget = Medium.target;
                        mediumButton.HasEffect = true;
                    }
                },
                () => { return Medium.medium != null && Medium.medium == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    if (mediumButton.isEffectActive && Medium.target != Medium.soulTarget)
                    {
                        Medium.soulTarget = null;
                        mediumButton.Timer = 0f;
                        mediumButton.isEffectActive = false;
                    }
                    return Medium.target != null && PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {
                    mediumButton.Timer = mediumButton.MaxTimer;
                    mediumButton.isEffectActive = false;
                    Medium.soulTarget = null;
                },
                Medium.getQuestionSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F,
                true,
                Medium.duration,
                () =>
                {
                    mediumButton.Timer = mediumButton.MaxTimer;
                    if (Medium.target == null || Medium.target.player == null) return;
                    string msg = "";

                    int randomNumber = TheOtherRoles.rnd.Next(4);
                    if (Medium.target.killerIfExisting != null)
                    {
                        if (Helpers.playerById(Medium.target.killerIfExisting.PlayerId).hasModifier(ModifierType.Mini))
                        {
                            randomNumber = TheOtherRoles.rnd.Next(3);
                        }
                    }
                    string typeOfColor = Helpers.isLighterColor(Medium.target.killerIfExisting.Data.DefaultOutfit.ColorId) ?
                        ModTranslation.getString("detectiveColorLight") :
                        ModTranslation.getString("detectiveColorDark");
                    float timeSinceDeath = ((float)(Medium.meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds);
                    string name = " (" + Medium.target.player.Data.PlayerName + ")";

                    if (randomNumber == 0) msg = string.Format(ModTranslation.getString("mediumQuestion1"), RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true)) + name;
                    else if (randomNumber == 1) msg = string.Format(ModTranslation.getString("mediumQuestion2"), typeOfColor) + name;
                    else if (randomNumber == 2) msg = string.Format(ModTranslation.getString("mediumQuestion3"), Math.Round(timeSinceDeath / 1000)) + name;
                    else msg = string.Format(ModTranslation.getString("mediumQuestion4"), RoleInfo.GetRolesString(Medium.target.killerIfExisting, false, includeHidden: true)) + name; ; // Excludes mini

                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");

                    // Remove soul
                    if (Medium.oneTimeUse)
                    {
                        float closestDistance = float.MaxValue;
                        SpriteRenderer target = null;

                        foreach ((DeadPlayer db, Vector3 ps) in Medium.deadBodies)
                        {
                            if (db == Medium.target)
                            {
                                Tuple<DeadPlayer, Vector3> deadBody = Tuple.Create(db, ps);
                                Medium.deadBodies.Remove(deadBody);
                                break;
                            }

                        }
                        foreach (SpriteRenderer rend in Medium.souls)
                        {
                            float distance = Vector2.Distance(rend.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                target = rend;
                            }
                        }

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>((p) =>
                        {
                            if (target != null)
                            {
                                var tmp = target.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                target.color = tmp;
                            }
                            if (p == 1f && target != null && target.gameObject != null) UnityEngine.Object.Destroy(target.gameObject);
                        })));

                        Medium.souls.Remove(target);
                    }
                }
            );
            mediumButton.buttonText = ModTranslation.getString("MediumText");

            // Pursuer button
            pursuerButton = new CustomButton(
                () =>
                {
                    if (Pursuer.target != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBlanked, Hazel.SendOption.Reliable, -1);
                        writer.Write(Pursuer.target.PlayerId);
                        writer.Write(Byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.setBlanked(Pursuer.target.PlayerId, Byte.MaxValue);

                        Pursuer.target = null;

                        Pursuer.blanks++;
                        pursuerButton.Timer = pursuerButton.MaxTimer;
                    }

                },
                () => { return Pursuer.pursuer != null && Pursuer.pursuer == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive() && Pursuer.blanks < Pursuer.blanksNumber; },
                () =>
                {
                    if (pursuerButtonBlanksText != null) pursuerButtonBlanksText.text = String.Format(ModTranslation.getString("pursuerBlanks"), Pursuer.blanksNumber - Pursuer.blanks);

                    return Pursuer.blanksNumber > Pursuer.blanks && PlayerControl.LocalPlayer.CanMove && Pursuer.target != null;
                },
                () => { pursuerButton.Timer = pursuerButton.MaxTimer; },
                Pursuer.getTargetSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            pursuerButton.buttonText = ModTranslation.getString("PursuerText");

            // Pursuer button blanks left
            pursuerButtonBlanksText = GameObject.Instantiate(pursuerButton.actionButton.cooldownTimerText, pursuerButton.actionButton.cooldownTimerText.transform.parent);
            pursuerButtonBlanksText.text = "";
            pursuerButtonBlanksText.enableWordWrapping = false;
            pursuerButtonBlanksText.transform.localScale = Vector3.one * 0.5f;
            pursuerButtonBlanksText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Witch Spell button
            witchSpellButton = new CustomButton(
                () =>
                {
                    if (Witch.currentTarget != null)
                    {
                        Witch.spellCastingTarget = Witch.currentTarget;
                    }
                },
                () => { return Witch.witch != null && Witch.witch == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.isAlive(); },
                () =>
                {
                    if (witchSpellButton.isEffectActive && Witch.spellCastingTarget != Witch.currentTarget)
                    {
                        Witch.spellCastingTarget = null;
                        witchSpellButton.Timer = 0f;
                        witchSpellButton.isEffectActive = false;
                    }
                    return PlayerControl.LocalPlayer.CanMove && Witch.currentTarget != null;
                },
                () =>
                {
                    witchSpellButton.Timer = witchSpellButton.MaxTimer;
                    witchSpellButton.isEffectActive = false;
                    Witch.spellCastingTarget = null;
                },
                Witch.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F,
                true,
                Witch.spellCastingDuration,
                () =>
                {
                    if (Witch.spellCastingTarget == null) return;
                    MurderAttemptResult attempt = Helpers.checkMuderAttempt(Witch.witch, Witch.spellCastingTarget);
                    if (attempt == MurderAttemptResult.PerformKill)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFutureSpelled, Hazel.SendOption.Reliable, -1);
                        writer.Write(Witch.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.setFutureSpelled(Witch.currentTarget.PlayerId);
                    }
                    if (attempt == MurderAttemptResult.BlankKill || attempt == MurderAttemptResult.PerformKill)
                    {
                        witchSpellButton.MaxTimer += Witch.cooldownAddition;
                        witchSpellButton.Timer = witchSpellButton.MaxTimer;
                        if (Witch.triggerBothCooldowns)
                            Witch.witch.killTimer = PlayerControl.GameOptions.KillCooldown;
                    }
                    else
                    {
                        witchSpellButton.Timer = 0f;
                    }
                    Witch.spellCastingTarget = null;
                }
            );
            witchSpellButton.buttonText = ModTranslation.getString("WitchText");

            portalmakerPlacePortalButton = new CustomButton(
                () =>
                {
                    portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer;

                    var pos = PlayerControl.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlacePortal, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placePortal(buff);
                },
                () => { return Portalmaker.portalmaker != null && Portalmaker.portalmaker == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Portal.secondPortal == null; },
                () => { return PlayerControl.LocalPlayer.CanMove && Portal.secondPortal == null; },
                () => { portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer; },
                Portalmaker.getPlacePortalButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            portalmakerPlacePortalButton.buttonText = ModTranslation.getString("PlacePortalText");

            usePortalButton = new CustomButton(
                () =>
                {
                    bool didTeleport = false;
                    Vector3 exit = Portal.findExit(PlayerControl.LocalPlayer.transform.position);
                    Vector3 entry = Portal.findEntry(PlayerControl.LocalPlayer.transform.position);
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(entry);

                    if (!PlayerControl.LocalPlayer.Data.IsDead)
                    {  // Ghosts can portal too, but non-blocking and only with a local animation
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UsePortal, Hazel.SendOption.Reliable, -1);
                        writer.Write((byte)PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    RPCProcedure.usePortal(PlayerControl.LocalPlayer.PlayerId);
                    usePortalButton.Timer = usePortalButton.MaxTimer;
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration, new Action<float>((p) =>
                    { // Delayed action
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                        {
                            if (SubmergedCompatibility.IsSubmerged)
                            {
                                SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            }
                            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }
                        if (p == 1f)
                        {
                            PlayerControl.LocalPlayer.moveable = true;
                        }
                    })));
                },
                () => { return Portal.bothPlacedAndEnabled; },
                () => { return PlayerControl.LocalPlayer.CanMove && Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) && !Portal.isTeleporting; },
                () => { usePortalButton.Timer = usePortalButton.MaxTimer; },
                Portalmaker.getUsePortalButtonSprite(),
                new Vector3(0.9f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.G,
                mirror: true
            );
            usePortalButton.buttonText = ModTranslation.getString("UsePortalText");

            // Assassin mark and assassinate button
            assassinButton = new CustomButton(
                () =>
                {
                    if (Assassin.assassinMarked != null)
                    {
                        // Murder attempt with teleport
                        MurderAttemptResult attempt = Helpers.checkMuderAttempt(Assassin.assassin, Assassin.assassinMarked);
                        if (attempt == MurderAttemptResult.PerformKill)
                        {
                            // Create first trace before killing
                            var pos = PlayerControl.LocalPlayer.transform.position;
                            byte[] buff = new byte[sizeof(float) * 2];
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceAssassinTrace, Hazel.SendOption.Reliable);
                            writer.WriteBytesAndSize(buff);
                            writer.EndMessage();
                            RPCProcedure.placeAssassinTrace(buff);

                            // Perform Kill
                            MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer2.Write(Assassin.assassinMarked.PlayerId);
                            writer2.Write(byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);
                            if (SubmergedCompatibility.IsSubmerged)
                            {
                                SubmergedCompatibility.ChangeFloor(Assassin.assassinMarked.transform.localPosition.y > -7);
                            }
                            RPCProcedure.uncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, Assassin.assassinMarked.PlayerId, byte.MaxValue);

                            // Create Second trace after killing
                            pos = Assassin.assassinMarked.transform.position;
                            buff = new byte[sizeof(float) * 2];
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                            MessageWriter writer3 = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceAssassinTrace, Hazel.SendOption.Reliable);
                            writer3.WriteBytesAndSize(buff);
                            writer3.EndMessage();
                            RPCProcedure.placeAssassinTrace(buff);
                        }

                        if (attempt == MurderAttemptResult.BlankKill || attempt == MurderAttemptResult.PerformKill)
                        {
                            assassinButton.Timer = assassinButton.MaxTimer;
                            Assassin.assassin.killTimer = PlayerControl.GameOptions.KillCooldown;
                        }
                        else if (attempt == MurderAttemptResult.SuppressKill)
                        {
                            assassinButton.Timer = 0f;
                        }
                        Assassin.assassinMarked = null;

                        assassinButton.buttonText = ModTranslation.getString("assassinText");
                        return;
                    }
                    if (Assassin.currentTarget != null)
                    {
                        Assassin.assassinMarked = Assassin.currentTarget;
                        assassinButton.Timer = Assassin.start;
                        assassinButton.buttonText = ModTranslation.getString("assassinKillText");
                    }
                },
                () => { return Assassin.assassin != null && Assassin.assassin == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {  // CouldUse
                    assassinButton.Sprite = Assassin.assassinMarked != null ? Assassin.getKillButtonSprite() : Assassin.getMarkButtonSprite();
                    return (Assassin.currentTarget != null || Assassin.assassinMarked != null) && PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {  // on meeting ends
                    assassinButton.Timer = assassinButton.MaxTimer;
                    Assassin.assassinMarked = null;
                },
                Assassin.getMarkButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.UseButton,
                KeyCode.F
            );
            assassinButton.buttonText = ModTranslation.getString("assassinText");

            // UnderTaker Button
            underTakerButton = new CustomButton(
                () =>
                {
                    if (UnderTaker.dragginBody)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DragPlaceBody, Hazel.SendOption.Reliable, -1);
                        writer.Write(UnderTaker.bodyId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.dragPlaceBody(UnderTaker.bodyId);
                    }
                    else
                    {
                        foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), 1f, Constants.PlayersOnlyMask))
                        {
                            if (collider2D.tag == "DeadBody")
                            {
                                DeadBody component = collider2D.GetComponent<DeadBody>();
                                if (component && !component.Reported)
                                {
                                    Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                    Vector2 truePosition2 = component.TruePosition;
                                    if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                    {
                                        GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);
                                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DragPlaceBody, Hazel.SendOption.Reliable, -1);
                                        writer.Write(playerInfo.PlayerId);
                                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                                        RPCProcedure.dragPlaceBody(playerInfo.PlayerId);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                },
                () => { return UnderTaker.underTaker != null && UnderTaker.underTaker == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    if (UnderTaker.dragginBody)
                        underTakerButton.buttonText = ModTranslation.getString("dropText");
                    else
                        underTakerButton.buttonText = ModTranslation.getString("dragText");
                    bool canDrag = false;
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), 1f, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                            canDrag = true;
                    return canDrag && PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {
                    underTakerButton.Timer = 0f;
                    UnderTaker.underTakerResetValuesAtDead();
                },
                UnderTaker.getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                __instance,
                __instance.KillButton,
                KeyCode.F
            );
            underTakerButton.buttonText = ModTranslation.getString("dragText");

            ButtonsGM.makeButtons(__instance);

            // Set the default (or settings from the previous game) timers/durations when spawning the buttons
            setCustomButtonCooldowns();
        }
    }
}