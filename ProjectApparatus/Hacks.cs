﻿using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Windows.Forms;

namespace ProjectApparatus
{

    internal class Hacks : MonoBehaviour
    {
        private static GUIStyle Style = null;


        bool IsPlayerValid(PlayerControllerB plyer)
        {
            return (plyer != null &&
                    !plyer.disconnectedMidGame &&
                    !plyer.playerUsername.Contains("Player #"));
        }

        public void OnGUI()
        {
            if (!Settings.Instance.b_isMenuOpen && Event.current.type != EventType.Repaint)
                return;

            UI.Reset();

            Style = new GUIStyle(GUI.skin.label);
            Style.normal.textColor = Color.white;
            Style.fontStyle = FontStyle.Bold;

            this.menuButton.Enable();
            this.unloadMenu.Enable();

            if (Settings.Instance.settingsData.b_EnableESP)
            {
                this.DisplayLoot();
                this.DisplayPlayers();
                this.DisplayDoors();
                this.DisplayLandmines();
                this.DisplayTurrets();
                this.DisplaySteamHazard();
                this.DisplayEnemyAI();
                this.DisplayShip();
                this.DisplayDeadPlayers();
            }

            Vector2 centeredPos = new Vector2(UnityEngine.Screen.width / 2f, UnityEngine.Screen.height / 2f);

            GUI.color = Settings.Instance.settingsData.c_Theme;

            if (Settings.Instance.settingsData.b_CenteredIndicators)
            {
                float iY = Settings.TEXT_HEIGHT;
                if (Settings.Instance.settingsData.b_DisplayGroupCredits && GameObjectManager.Instance.shipTerminal != null) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Group Credits: " + GameObjectManager.Instance.shipTerminal.groupCredits, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (Settings.Instance.settingsData.b_DisplayQuota && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Profit Quota: " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (Settings.Instance.settingsData.b_DisplayDaysLeft && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Days Left: " + TimeOfDay.Instance.daysUntilDeadline, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
            }

            string Watermark = "Project Apparatus";
            if (!Settings.Instance.settingsData.b_CenteredIndicators)
            {
                if (Settings.Instance.settingsData.b_DisplayGroupCredits && GameObjectManager.Instance.shipTerminal != null)
                    Watermark += " | Group Credits: " + GameObjectManager.Instance.shipTerminal.groupCredits;
                if (Settings.Instance.settingsData.b_DisplayQuota && TimeOfDay.Instance)
                    Watermark += " | Profit Quota: " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota;
                if (Settings.Instance.settingsData.b_DisplayDaysLeft && TimeOfDay.Instance)
                    Watermark += " | Days Left: " + TimeOfDay.Instance.daysUntilDeadline;
            }
            Render.String(Style, 10f, 5f, 150f, Settings.TEXT_HEIGHT, Watermark, GUI.color);

            if (Settings.Instance.b_isMenuOpen)
            {
                Settings.Instance.windowRect = GUILayout.Window(0, Settings.Instance.windowRect, new GUI.WindowFunction(this.MenuContent), "Project Apparatus", Array.Empty<GUILayoutOption>());
            }

            if (Settings.Instance.settingsData.b_Crosshair)
            {
                Render.FilledCircle(centeredPos, 5, Color.black);
                Render.FilledCircle(centeredPos, 3, Settings.Instance.settingsData.c_Theme);
            }
        }

        private PlayerControllerB selectedPlayer = null;

        private void MenuContent(int windowID)
        {
            GUILayout.BeginHorizontal();
            UI.Tab("Self", ref UI.nTab, UI.Tabs.Self);
            UI.Tab("Misc", ref UI.nTab, UI.Tabs.Misc);
            UI.Tab("ESP", ref UI.nTab, UI.Tabs.ESP);
            UI.Tab("Players", ref UI.nTab, UI.Tabs.Players);
            UI.Tab("Graphics", ref UI.nTab, UI.Tabs.Graphics);
            UI.Tab("Cheat", ref UI.nTab, UI.Tabs.Cheat);
            GUILayout.EndHorizontal();

            SettingsData settingsData = Settings.Instance.settingsData;

            UI.TabContents("Self", UI.Tabs.Self, () =>
            {
                UI.Checkbox(ref settingsData.b_GodMode, "God Mode", "Prevents you from taking any damage.");
                UI.Checkbox(ref settingsData.b_InfiniteStam, "Infinite Stamina", "Prevents you from losing any stamina.");
                UI.Checkbox(ref settingsData.b_InfiniteCharge, "Infinite Charge", "Prevents your items from losing any charge.");
                UI.Checkbox(ref settingsData.b_InfiniteZapGun, "Infinite Zap Gun", "Infinitely stuns enemies with the zap-gun.");
                UI.Checkbox(ref settingsData.b_InfiniteShotgunAmmo, "Infinite Shotgun Ammo", "Prevents you from out of ammo.");
                UI.Checkbox(ref settingsData.b_NightVision, "Night Vision", "Allows you to see in the dark.");
                UI.Checkbox(ref settingsData.b_InteractThroughWalls, "Interact Through Walls", "Allows you to interact with anything through walls.");
                UI.Checkbox(ref settingsData.b_UnlimitedGrabDistance, "No Grab Distance Limit", "Allows you to interact with anything no matter the distance.");
                UI.Checkbox(ref settingsData.b_OneHandAllObjects, "One Hand All Objects", "Allows you to one-hand any two-handed objects.");
                UI.Checkbox(ref settingsData.b_DisableInteractCooldowns, "Disable Interact Cooldowns", "Disables all interact cooldowns (e.g., noisemakers, toilets, etc).");
                UI.Checkbox(ref settingsData.b_InstantInteractions, "Instant Interactions", "Makes all hold interactions instantaneous.");
                UI.Checkbox(ref settingsData.b_PlaceAnywhere, "Place Anywhere", "Place objects from the ship anywhere you want.");
                UI.Checkbox(ref settingsData.b_TauntSlide, "Taunt Slide", "Allows you to emote and move at the same time.");
                UI.Checkbox(ref settingsData.b_FastLadderClimbing, "Fast Ladder Climbing", "Instantly climbs up ladders.");
                UI.Checkbox(ref settingsData.b_HearEveryone, "Hear Everyone", "Allows you to hear everyone no matter the distance.");
                UI.Checkbox(ref settingsData.b_ChargeAnyItem, "Charge Any Item", "Allows you to put any grabbable item in the charger.");
                UI.Checkbox(ref settingsData.b_WalkSpeed, "Adjust Walk Speed (" + settingsData.i_WalkSpeed + ")", "Allows you to modify your walk speed.");
                settingsData.i_WalkSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_WalkSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_SprintSpeed, "Adjust Sprint Speed (" + settingsData.i_SprintSpeed + ")", "Allows you to modify your sprint speed.");
                settingsData.i_SprintSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_SprintSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_JumpHeight, "Jump Height (" + settingsData.i_JumpHeight + ")", "Allows you to modify your jump height.");
                settingsData.i_JumpHeight = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_JumpHeight, 1, 100));
                if (GUILayout.Button("Respawn"))
                    ReviveLocalPlayer();

                GUILayout.BeginHorizontal();
                UI.Checkbox(ref settingsData.b_Noclip, "Noclip (" + settingsData.fl_NoclipSpeed + ")", "Allows you to fly and clip through walls.");
                settingsData.keyNoclip.Menu();
                GUILayout.EndHorizontal();
                settingsData.fl_NoclipSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.fl_NoclipSpeed, 1, 100));
            });

            UI.TabContents("Misc", UI.Tabs.Misc, () =>
            {
                UI.Checkbox(ref settingsData.b_NoMoreCredits, "No More Credits", "Prevents your group from receiving any credits. (Doesn't apply to quota)");
                UI.Checkbox(ref settingsData.b_SensitiveLandmines, "Sensitive Landmines", "Automatically detonates landmines when a player is in kill range.");
                UI.Checkbox(ref settingsData.b_AllJetpacksExplode, "All Jetpacks Explode", "When a player tries to equip a jetpack they will be greeted with an explosion.");
                UI.Checkbox(ref settingsData.b_LightShow, "Light Show", "Rapidly turns on/off the light switch and TV.");
                if (!settingsData.b_NoMoreCredits)
                {
                    settingsData.str_MoneyToGive = GUILayout.TextField(settingsData.str_MoneyToGive, Array.Empty<GUILayoutOption>());
                    UI.Button("Give Credits", "Give your group however many credits you want. (Doesn't apply to quota)",  () =>
                    {
                        if (GameObjectManager.Instance.shipTerminal)
                            GameObjectManager.Instance.shipTerminal.groupCredits += int.Parse(settingsData.str_MoneyToGive);
                    });

                    GUILayout.BeginHorizontal();
                    settingsData.str_QuotaFulfilled = GUILayout.TextField(settingsData.str_QuotaFulfilled, GUILayout.Width(42));
                    GUILayout.Label("/", GUILayout.Width(4));
                    settingsData.str_Quota = GUILayout.TextField(settingsData.str_Quota, GUILayout.Width(42));
                    GUILayout.EndHorizontal();

                    UI.Button("Set Quota", "Allows you to set the quota. (Host only)", () => {
                        if (TimeOfDay.Instance)
                        {
                            TimeOfDay.Instance.profitQuota = int.Parse(settingsData.str_Quota);
                            TimeOfDay.Instance.quotaFulfilled = int.Parse(settingsData.str_QuotaFulfilled);
                            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                        }
                    });
                }

                UI.Button("Start Server", "Lands the ship.", () => StartOfRound.Instance.StartGameServerRpc());
                UI.Button("Stop Server", "Ship will leave the planet it's currently on.", () => StartOfRound.Instance.EndGameServerRpc(0));
                UI.Button("Unlock All Doors", "Unlocks all locked doors.", () => 
                {
                    foreach (DoorLock obj in GameObjectManager.Instance.door_locks)
                        if (obj != null)
                            obj.UnlockDoorServerRpc();
                });
                UI.Button("Explode All Mines", "Explodes every single mine on the level.", () =>
                {
                    foreach (Landmine obj in GameObjectManager.Instance.landmines)
                        if (obj != null)
                            obj.ExplodeMineServerRpc();
                });
                UI.Button("Kill All Enemies", "Kills all enemies.", () => {
                    foreach (EnemyAI obj in GameObjectManager.Instance.enemies)
                        if (obj != null)
                            obj.KillEnemyServerRpc(false);
                });
                UI.Button("Delete All Enemies", "Deletes all enemies.", () => {
                    foreach (EnemyAI obj in GameObjectManager.Instance.enemies)
                        if (obj != null)
                            obj.KillEnemyServerRpc(true);
                });
                UI.Button("Attack Players at Deposit Desk", "Forces the tentacle monster to attack, killing a nearby player.", () => {
                    if (GameObjectManager.Instance.itemsDesk)
                        GameObjectManager.Instance.itemsDesk.AttackPlayersServerRpc();
                });
            });


            UI.TabContents("ESP", UI.Tabs.ESP, () => {
                UI.Checkbox(ref settingsData.b_EnableESP, "Enabled");
                UI.Checkbox(ref settingsData.b_ItemESP, "Items");
                UI.Checkbox(ref settingsData.b_EnemyESP, "Enemies");
                UI.Checkbox(ref settingsData.b_PlayerESP, "Players");
                UI.Checkbox(ref settingsData.b_ShipESP, "Ships");
                UI.Checkbox(ref settingsData.b_DoorESP, "Doors");
                UI.Checkbox(ref settingsData.b_SteamHazard, "Steam Hazards");
                UI.Checkbox(ref settingsData.b_LandmineESP, "Landmines");
                UI.Checkbox(ref settingsData.b_TurretESP, "Turrets");
                UI.Checkbox(ref settingsData.b_DisplayHP, "Show Health", "Shows players' health.");
                UI.Checkbox(ref settingsData.b_DisplayWorth, "Show Value", "Shows items' value.");
                UI.Checkbox(ref settingsData.b_DisplayDistance, "Show Distance", "Shows the distance between you and the entity.");
                UI.Checkbox(ref settingsData.b_DisplaySpeaking, "Show Is Speaking", "Shows if the player is speaking.");

                UI.Checkbox(ref settingsData.b_ItemDistanceLimit, "Item Distance Limit (" + Mathf.RoundToInt(settingsData.fl_ItemDistanceLimit) + ")", "Toggle to set the item distance limit.");
                settingsData.fl_ItemDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_ItemDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_EnemyDistanceLimit, "Enemy Distance Limit (" + Mathf.RoundToInt(settingsData.fl_EnemyDistanceLimit) + ")", "Toggle to set the enemy distance limit.");
                settingsData.fl_EnemyDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_EnemyDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_MineDistanceLimit, "Landmine Distance Limit (" + Mathf.RoundToInt(settingsData.fl_MineDistanceLimit) + ")", "Toggle to set the landmine distance limit.");
                settingsData.fl_MineDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_MineDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_TurretDistanceLimit, "Turret Distance Limit (" + Mathf.RoundToInt(settingsData.fl_TurretDistanceLimit) + ")", "Toggle to set the turret distance limit.");
                settingsData.fl_TurretDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_TurretDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());
            });

            UI.TabContents(null, UI.Tabs.Players, () =>
            {
                GUILayout.BeginHorizontal();
                foreach (PlayerControllerB player in GameObjectManager.Instance.players)
                {
                    if (!IsPlayerValid(player)) continue;
                    UI.Tab(PAUtils.TruncateString(player.playerUsername, 12), ref selectedPlayer, player, true);
                }
                GUILayout.EndHorizontal();

                if (!IsPlayerValid(selectedPlayer))
                    selectedPlayer = null;

                if (selectedPlayer)
                {
                    UI.Header("Selected Player: " + selectedPlayer.playerUsername);
                    Settings.Instance.InitializeDictionaries(selectedPlayer);

                    Settings.Instance.b_DemiGod[selectedPlayer] = GUILayout.Toggle(Settings.Instance.b_DemiGod[selectedPlayer], "Demigod", "Automatically refills the selected player's health if below zero.");

                    UI.Button("Kill", "Kills the currently selected player.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(selectedPlayer.health + 1, new Vector3(900, 900, 900), 0); });
                    UI.Button("Teleport To", "Teleports you to the currently selected player.", () => { GameObjectManager.Instance.localPlayer.TeleportPlayer(selectedPlayer.playerGlobalHead.position); });

                    Settings.Instance.str_DamageToGive = GUILayout.TextField(Settings.Instance.str_DamageToGive, Array.Empty<GUILayoutOption>());
                    UI.Button("Damage", "Damages the player for a given amount.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(int.Parse(Settings.Instance.str_DamageToGive), new Vector3(900, 900, 900), 0); });

                    Settings.Instance.str_HealthToHeal = GUILayout.TextField(Settings.Instance.str_HealthToHeal, Array.Empty<GUILayoutOption>());
                    UI.Button("Heal", "Heals the player for a given amount.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(-int.Parse(Settings.Instance.str_HealthToHeal), new Vector3(900, 900, 900), 0); });

                    UI.Button("Steam Profile", "Opens the selected player's steam profile in your overlay.", () => { SteamFriends.OpenUserOverlay(selectedPlayer.playerSteamId, "steamid");  });
                }
            });

            UI.TabContents("Graphics", UI.Tabs.Graphics, () =>
            {
                UI.Checkbox(ref settingsData.b_DisableFog, "Disable Fog", "Disables the fog effect.");
                UI.Checkbox(ref settingsData.b_DisableDepthOfField, "Disable Depth of Field", "Disables the depth of field effect.");
            });

            UI.TabContents("Cheat", UI.Tabs.Cheat, () =>
            {
                UI.Checkbox(ref settingsData.b_Crosshair, "Crosshair", "Displays a crosshair on the screen.");
                UI.Checkbox(ref settingsData.b_DisplayGroupCredits, "Display Group Credits", "Shows how many credits you have.");
                UI.Checkbox(ref settingsData.b_DisplayQuota, "Display Quota", "Shows the current quota.");
                UI.Checkbox(ref settingsData.b_DisplayDaysLeft, "Display Days Left", "Shows the time you have left to meet quota.");
                UI.Checkbox(ref settingsData.b_CenteredIndicators, "Centered Indicators", "Displays the above indicators at the center of the screen.");
                UI.Checkbox(ref settingsData.b_DeadPlayers, "Dead Player List", "Shows a list of currently dead players.");
                UI.Checkbox(ref settingsData.b_Tooltips, "Tooltips", "Shows information about the currently hovered menu item.");

                UI.Header("Colors");
                UI.ColorPicker("Theme", ref settingsData.c_Theme);
                UI.ColorPicker("Valve", ref settingsData.c_Valve);
                UI.ColorPicker("Enemy", ref settingsData.c_Enemy);
                UI.ColorPicker("Turret", ref settingsData.c_Turret);
                UI.ColorPicker("Landmine", ref settingsData.c_Landmine);
                UI.ColorPicker("Player", ref settingsData.c_Player);
                UI.ColorPicker("Door", ref settingsData.c_Door);
                UI.ColorPicker("Loot", ref settingsData.c_Loot);
                UI.ColorPicker("Small Loot", ref settingsData.c_smallLoot);
                UI.ColorPicker("Medium Loot", ref settingsData.c_medLoot);
                UI.ColorPicker("Big Loot", ref settingsData.c_bigLoot);
            });

            Settings.Instance.settingsData = settingsData;

            UI.RenderTooltip();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }


        private void DisplayObjects<T>(IEnumerable<T> objects, bool shouldDisplay, Func<T, string> labelSelector, Func<T, Color> colorSelector) where T : Component
        {
            if (!shouldDisplay) return;

            foreach (T obj in objects)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    float distanceToPlayer = PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        obj.transform.position);
                    Vector3 pos;
                    if (PAUtils.WorldToScreen(GameObjectManager.Instance.localPlayer.gameplayCamera, obj.transform.position, out pos))
                    {
                        string ObjName = PAUtils.ConvertFirstLetterToUpperCase(labelSelector(obj));
                        if (Settings.Instance.settingsData.b_DisplayDistance)
                            ObjName += " [" + distanceToPlayer.ToString().ToUpper() + "M]";
                        Render.String(Style, pos.x, pos.y, 150f, 50f, ObjName, colorSelector(obj));
                    }
                }
            }
        }

        public void DisplayDeadPlayers()
        {
            if (!Settings.Instance.settingsData.b_DeadPlayers) return;

            float yOffset = 30f;

            foreach (PlayerControllerB playerControllerB in GameObjectManager.Instance.players)
            {
                if (playerControllerB != null && playerControllerB.isPlayerDead)
                {
                    string playerUsername = playerControllerB.playerUsername;

                    Render.String(Style, 10f, yOffset, 200f, Settings.TEXT_HEIGHT, playerUsername, GUI.color);
                    yOffset += (Settings.TEXT_HEIGHT - 10f);
                }
            }
        }

        private void DisplayShip()
        {
            DisplayObjects(
                new[] { GameObjectManager.Instance.shipDoor },
                Settings.Instance.settingsData.b_ShipESP,
                _ => "Ship",
                _ => Settings.Instance.settingsData.c_Door
            );
        }

        private void DisplayDoors()
        {
            DisplayObjects(
                GameObjectManager.Instance.entrance_doors,
                Settings.Instance.settingsData.b_DoorESP,
                entranceTeleport => entranceTeleport.isEntranceToBuilding ? "Entrance" : "Exit",
                _ => Settings.Instance.settingsData.c_Door
            );
        }

        private void DisplayLandmines()
        {
            DisplayObjects(
                GameObjectManager.Instance.landmines.Where(landmine => landmine != null && landmine.IsSpawned && !landmine.hasExploded &&
                    ((Settings.Instance.settingsData.b_MineDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        landmine.transform.position) < Settings.Instance.settingsData.fl_MineDistanceLimit) ||
                        !Settings.Instance.settingsData.b_MineDistanceLimit)),
                Settings.Instance.settingsData.b_LandmineESP,
                _ => "Landmine",
                _ => Settings.Instance.settingsData.c_Landmine
            );
        }

        private void DisplayTurrets()
        {
            DisplayObjects(
                GameObjectManager.Instance.turrets.Where(turret => turret != null && turret.IsSpawned &&
                    ((Settings.Instance.settingsData.b_TurretDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        turret.transform.position) < Settings.Instance.settingsData.fl_TurretDistanceLimit) ||
                        !Settings.Instance.settingsData.b_TurretDistanceLimit)),
                Settings.Instance.settingsData.b_TurretESP,
                _ => "Turret",
                _ => Settings.Instance.settingsData.c_Turret
            );
        }

        private void DisplaySteamHazard()
        {
            DisplayObjects(
                GameObjectManager.Instance.steamValves.Where(steamValveHazard => steamValveHazard != null && steamValveHazard.triggerScript.interactable),
                Settings.Instance.settingsData.b_SteamHazard,
                _ => "Steam Valve",
                _ => Settings.Instance.settingsData.c_Valve
            );
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                GameObjectManager.Instance.players.Where(playerControllerB =>
                    IsPlayerValid(playerControllerB) &&
                    !playerControllerB.IsLocalPlayer &&
                     playerControllerB.playerUsername != GameObjectManager.Instance.localPlayer.playerUsername &&
                    !playerControllerB.isPlayerDead
                ),
                Settings.Instance.settingsData.b_PlayerESP,
                playerControllerB =>
                {
                    string str = playerControllerB.playerUsername;
                    if (Settings.Instance.settingsData.b_DisplaySpeaking && playerControllerB.voicePlayerState.IsSpeaking)
                        str += " [VC]";
                    if (Settings.Instance.settingsData.b_DisplayHP)
                        str += " [" + playerControllerB.health + "HP]";
                    return str;
                },
                _ => Settings.Instance.settingsData.c_Player
            );
        }

        private void DisplayEnemyAI()
        {
            DisplayObjects(
                GameObjectManager.Instance.enemies.Where(enemyAI =>
                    enemyAI != null &&
                    enemyAI.eye != null &&
                    enemyAI.enemyType != null &&
                    !enemyAI.isEnemyDead &&
                    ((Settings.Instance.settingsData.b_EnemyDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        enemyAI.transform.position) < Settings.Instance.settingsData.fl_EnemyDistanceLimit) ||
                        !Settings.Instance.settingsData.b_EnemyDistanceLimit)
                ),
                Settings.Instance.settingsData.b_EnemyESP,
                enemyAI =>
                {
                    string name = enemyAI.enemyType.enemyName;
                    return string.IsNullOrWhiteSpace(name) ? "Enemy" : name;
                },
                _ => Settings.Instance.settingsData.c_Enemy
            );
        }

        private Color GetLootColor(int value)
        {
            int[] thresholds = { 15, 35 };
            Color[] colors = { Settings.Instance.settingsData.c_smallLoot, Settings.Instance.settingsData.c_medLoot, Settings.Instance.settingsData.c_bigLoot };

            for (int i = 0; i < thresholds.Length; i++)
                if (value <= thresholds[i])
                    return colors[i];

            return Settings.Instance.settingsData.c_Loot;
        }

        private void DisplayLoot()
        {
            DisplayObjects(
                GameObjectManager.Instance.items.Where(grabbableObject =>
                    grabbableObject != null &&
                    !grabbableObject.isHeld &&
                    !grabbableObject.isPocketed &&
                    grabbableObject.itemProperties != null &&
                    ((Settings.Instance.settingsData.b_ItemDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        grabbableObject.transform.position) < Settings.Instance.settingsData.fl_ItemDistanceLimit) ||
                        !Settings.Instance.settingsData.b_ItemDistanceLimit)
                ),
                Settings.Instance.settingsData.b_ItemESP,
                grabbableObject =>
                {
                    string text = "Object";
                    Item itemProperties = grabbableObject.itemProperties;
                    if (itemProperties.itemName != null)
                        text = itemProperties.itemName;
                    int scrapValue = grabbableObject.scrapValue;
                    if (Settings.Instance.settingsData.b_DisplayWorth && scrapValue > 0)
                        text += " [" + scrapValue.ToString() + "C]";
                    return text;
                },
                grabbableObject => GetLootColor(grabbableObject.scrapValue)
            );
        }

        public void Start()
        {
            Harmony harmony = new Harmony("com.waxxyTF2.ProjectApparatus");
            harmony.PatchAll();

            StartCoroutine(GameObjectManager.Instance.CollectObjects());
            Settings.Instance.ResetBindStates();
        }

        public void Update()
        {
            if (this.menuButton.WasPerformedThisFrame())
            {
                Settings.Instance.SaveSettings();
                Settings.Instance.b_isMenuOpen = !Settings.Instance.b_isMenuOpen;
            }
            if (this.unloadMenu.WasPressedThisFrame())
            {
                Loader.Unload();
                base.StopCoroutine(GameObjectManager.Instance.CollectObjects());
            }

            if (Settings.Instance.settingsData.b_LightShow)
            {
                if (GameObjectManager.Instance.shipLights)
                    GameObjectManager.Instance.shipLights.SetShipLightsServerRpc(!GameObjectManager.Instance.shipLights.areLightsOn);

                if (GameObjectManager.Instance.tvScript)
                {
                    if (GameObjectManager.Instance.tvScript.tvOn)
                        GameObjectManager.Instance.tvScript.TurnOffTVServerRpc();
                    else
                        GameObjectManager.Instance.tvScript.TurnOnTVServerRpc();
                }
            }

            if (Settings.Instance.settingsData.b_NoMoreCredits && GameObjectManager.Instance.shipTerminal)
                GameObjectManager.Instance.shipTerminal.groupCredits = 0;

            Noclip();

            Settings.Instance.settingsData.keyNoclip.Update();
        }

        private void Noclip()
        {
            PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
            if (!localPlayer) return;

            Collider localCollider = localPlayer.GetComponent<CharacterController>();
            if (!localCollider) return;

            Transform localTransform = localPlayer.transform;
            localCollider.enabled = !(localTransform 
                && Settings.Instance.settingsData.b_Noclip
                && PAUtils.GetAsyncKeyState(Settings.Instance.settingsData.keyNoclip.inKey) != 0); 

            if (!localCollider.enabled)
            {
                bool WKey = PAUtils.GetAsyncKeyState((int)Keys.W) != 0,
                    AKey = PAUtils.GetAsyncKeyState((int)Keys.A) != 0,
                    SKey = PAUtils.GetAsyncKeyState((int)Keys.S) != 0,
                    DKey = PAUtils.GetAsyncKeyState((int)Keys.D) != 0,
                    SpaceKey = PAUtils.GetAsyncKeyState((int)Keys.Space) != 0,
                    CtrlKey = PAUtils.GetAsyncKeyState((int)Keys.LControlKey) != 0;

                Vector3 inVec = new Vector3(0, 0, 0);

                if (WKey)
                    inVec += localTransform.forward;
                if (SKey)
                    inVec -= localTransform.forward;
                if (AKey)
                    inVec -= localTransform.right;
                if (DKey)
                    inVec += localTransform.right;
                if (SpaceKey)
                    inVec.y += localTransform.up.y;
                if (CtrlKey)
                    inVec.y -= localTransform.up.y;

                localPlayer.transform.position += inVec * (Settings.Instance.settingsData.fl_NoclipSpeed * Time.deltaTime);
            }
        }

        private void ReviveLocalPlayer() // This is a modified version of StartOfRound.ReviveDeadPlayers
        {
            PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
            StartOfRound.Instance.allPlayersDead = false;
            localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
            if (localPlayer.isPlayerDead || localPlayer.isPlayerControlled)
            {
                localPlayer.isClimbingLadder = false;
                localPlayer.ResetZAndXRotation();
                localPlayer.thisController.enabled = true;
                localPlayer.health = 100;
                localPlayer.disableLookInput = false;
                if (localPlayer.isPlayerDead)
                {
                    localPlayer.isPlayerDead = false;
                    localPlayer.isPlayerControlled = true;
                    localPlayer.isInElevator = true;
                    localPlayer.isInHangarShipRoom = true;
                    localPlayer.isInsideFactory = false;
                    localPlayer.wasInElevatorLastFrame = false;
                    StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
                    localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].position, false, 0f, false, true);
                    localPlayer.setPositionOfDeadPlayer = false;
                    localPlayer.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[localPlayer.playerClientId], true, true);
                    localPlayer.helmetLight.enabled = false;
                    localPlayer.Crouch(false);
                    localPlayer.criticallyInjured = false;
                    if (localPlayer.playerBodyAnimator != null)
                        localPlayer.playerBodyAnimator.SetBool("Limp", false);
                    localPlayer.bleedingHeavily = false;
                    localPlayer.activatingItem = false;
                    localPlayer.twoHanded = false;
                    localPlayer.inSpecialInteractAnimation = false;
                    localPlayer.disableSyncInAnimation = false;
                    localPlayer.inAnimationWithEnemy = null;
                    localPlayer.holdingWalkieTalkie = false;
                    localPlayer.speakingToWalkieTalkie = false;
                    localPlayer.isSinking = false;
                    localPlayer.isUnderwater = false;
                    localPlayer.sinkingValue = 0f;
                    localPlayer.statusEffectAudio.Stop();
                    localPlayer.DisableJetpackControlsLocally();
                    localPlayer.health = 100;
                    localPlayer.mapRadarDotAnimator.SetBool("dead", false);
                    if (localPlayer.IsOwner)
                    {
                        HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                        localPlayer.hasBegunSpectating = false;
                        HUDManager.Instance.RemoveSpectateUI();
                        HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                        localPlayer.hinderedMultiplier = 1f;
                        localPlayer.isMovementHindered = 0;
                        localPlayer.sourcesCausingSinking = 0;
                        localPlayer.reverbPreset = StartOfRound.Instance.shipReverb;
                    }
                }
                SoundManager.Instance.earsRingingTimer = 0f;
                localPlayer.voiceMuffledByEnemy = false;
                SoundManager.Instance.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
                SoundManager.Instance.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
                if (localPlayer.currentVoiceChatIngameSettings == null)
                {
                    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
                }
                if (localPlayer.currentVoiceChatIngameSettings != null)
                {
                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        localPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        return;

                    localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                }
            }
            PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
            playerControllerB.bleedingHeavily = false;
            playerControllerB.criticallyInjured = false;
            playerControllerB.playerBodyAnimator.SetBool("Limp", false);
            playerControllerB.health = 100;
            HUDManager.Instance.UpdateHealthUI(100, false);
            playerControllerB.spectatedPlayerScript = null;
            HUDManager.Instance.audioListenerLowPass.enabled = false;
            StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, playerControllerB);
            RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
            for (int j = 0; j < array.Length; j++)
            {
                if (!array[j].isHeld)
                {
                    if (StartOfRound.Instance.IsServer)
                    {
                        if (array[j].NetworkObject.IsSpawned)
                            array[j].NetworkObject.Despawn(true);
                        else
                            UnityEngine.Object.Destroy(array[j].gameObject);
                    }
                }
                else if (array[j].isHeld && array[j].playerHeldBy != null)
                {
                    array[j].playerHeldBy.DropAllHeldItems(true, false);
                }
            }
            DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
            for (int k = 0; k < array2.Length; k++)
            {
                UnityEngine.Object.Destroy(array2[k].gameObject);
            }
            StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
            StartOfRound.Instance.allPlayersDead = false;
            StartOfRound.Instance.UpdatePlayerVoiceEffects();
            StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
        }

        public LayerMask s_layerMask = LayerMask.GetMask(new string[]
        {
            "Room"
        });

        private readonly InputAction menuButton = new InputAction(null, InputActionType.Button, "<Keyboard>/insert", null, null, null);
        private readonly InputAction unloadMenu = new InputAction(null, InputActionType.Button, "<Keyboard>/pause", null, null, null);
    }
}
