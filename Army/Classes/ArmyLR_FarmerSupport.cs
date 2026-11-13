/*
name: ArmyLR Farmer & Support
description: The exact ArmyLR workflow where only 1 account farms and receives Legion Revenant, while up to 3 non-Legion accounts act as combat helpers/supports.
tags: legion, legion rev, legion revenant, revenant, army, support, helper
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyLR_FarmerSupport
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreLegion CLR = new();

    public string OptionsStorage = "ArmyLR_FarmerSupport";
    public bool DontPreconfigure = true;

    public string[] LRMaterials =
    {
        "Exalted Crown",
        "Revenant's Spellscroll",
        "Conquest Wreath",
        "Legion Revenant",
    };
    public string[] LF1 =
    {
        "Aeacus Empowered",
        "Tethered Soul",
        "Darkened Essence",
        "Dracolich Contract",
    };
    public string[] LF2 =
    {
        "Grim Cohort Conquered",
        "Ancient Cohort Conquered",
        "Pirate Cohort Conquered",
        "Battleon Cohort Conquered",
        "Mirror Cohort Conquered",
        "Darkblood Cohort Conquered",
        "Vampire Cohort Conquered",
        "Spirit Cohort Conquered",
        "Dragon Cohort Conquered",
        "Doomwood Cohort Conquered",
    };
    public string[] LF3 =
    {
        "Hooded Legion Cowl",
        "Legion Token",
        "Dage's Favor",
        "Emblem of Dage",
        "Diamond Token of Dage",
        "Dark Token",
    };

    public List<IOption> Options = new()
    {
        new Option<string>("FarmerAccount", "Farmer Account (Username)", "Account with Undead Legion that accepts quests and gets LR.", ""),
        new Option<string>("Helper1", "Helper Account 1", "Combat support account 1 (does NOT need Legion).", ""),
        new Option<string>("Helper2", "Helper Account 2", "Combat support account 2 (does NOT need Legion).", ""),
        new Option<string>("Helper3", "Helper Account 3", "Combat support account 3 (does NOT need Legion).", ""),
        new Option<int>("PrivateRoomNumber", "Private Room Number", "Shared private room number (e.g. 100000 - 999999).", 123456),
        new Option<bool>(
            "UseFirstClassEntertainment",
            "Use First Class Entertainment?",
            "Farm Legion Tokens using First Class Entertainment (/legionarena) instead of Dreadrock.",
            false
        ),
        CoreBots.Instance.SkipOptions,
    };

    private static readonly string StateFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Skua",
        "ArmyLR_FarmerSupport_State.txt"
    );

    private int _stateSeq = 0;

    public void ScriptMain(IScriptInterface Bot)
    {
        string farmer = Bot.Config?.Get<string>("FarmerAccount")?.Trim() ?? "";
        string helper1 = Bot.Config?.Get<string>("Helper1")?.Trim() ?? "";
        string helper2 = Bot.Config?.Get<string>("Helper2")?.Trim() ?? "";
        string helper3 = Bot.Config?.Get<string>("Helper3")?.Trim() ?? "";
        int room = Bot.Config?.Get<int>("PrivateRoomNumber") ?? 123456;

        if (string.IsNullOrEmpty(farmer))
        {
            C.Logger("ERROR: 'FarmerAccount' username must be configured in script options!");
            return;
        }

        string currentUsername = C.Username();
        bool isFarmer = currentUsername.Equals(farmer, StringComparison.OrdinalIgnoreCase);
        bool isH1 = !string.IsNullOrEmpty(helper1) && currentUsername.Equals(helper1, StringComparison.OrdinalIgnoreCase);
        bool isH2 = !string.IsNullOrEmpty(helper2) && currentUsername.Equals(helper2, StringComparison.OrdinalIgnoreCase);
        bool isH3 = !string.IsNullOrEmpty(helper3) && currentUsername.Equals(helper3, StringComparison.OrdinalIgnoreCase);

        if (!isFarmer && !isH1 && !isH2 && !isH3)
        {
            C.Logger($"ERROR: '{currentUsername}' does not match FarmerAccount or any Helper accounts in options.");
            return;
        }

        C.SetOptions(disableClassSwap: true);
        C.PrivateRooms = true;
        C.PrivateRoomNumber = room;

        // Start skill rotations to prevent basic-attack AFK timeouts
        string activeClass = Bot.Player.CurrentClass?.Name ?? "generic";
        Bot.Skills.StartAdvanced(activeClass, false);
        C.Logger($"Advanced combat skills started for class: {activeClass}");

        try
        {
            if (isFarmer)
            {
                C.Logger($"=== [ROLE: FARMER] '{currentUsername}' farming Legion Revenant ===");
                C.BankingBlackList.AddRange(new[] { "add", "any", "drops", "needed" });
                C.BankingBlackList.AddRange(LRMaterials);
                C.BankingBlackList.AddRange(LF1);
                C.BankingBlackList.AddRange(LF2);
                C.BankingBlackList.AddRange(LF3);

                // Reset state file on start
                UpdateState(0, "whitemap", "Enter", "Spawn", "*", false);

                CLR.JoinLegion();
                GetLR();
            }
            else
            {
                C.Logger($"=== [ROLE: HELPER] '{currentUsername}' supporting farmer '{farmer}' ===");
                RunHelper(farmer, isH3);
            }
        }
        finally
        {
            Bot.Skills.Stop();
            Bot.Options.AggroMonsters = false;
            C.SetOptions(false);
        }
    }

    #region Farmer Logic

    void GetLR()
    {
        C.AddDrop("Legion Revenant");
        C.EnsureAccept(6900);
        LF1Items(20);
        LF2Items(6);
        LF3Items(10);
        C.EnsureComplete(6900);
        Bot.Wait.ForPickup("Legion Revenant");

        // Broadcast completion to all helpers
        UpdateState(++_stateSeq, "whitemap", "Enter", "Spawn", "*", true);
        C.Logger("🎉 LEGION REVENANT ACQUIRED SUCCESSFULLY! ALL HELPERS NOTIFIED.");
    }

    void LF1Items(int quant = 20)
    {
        while (!Bot.ShouldExit)
        {
            if (C.CheckInventory("Revenant's Spellscroll", quant))
            {
                Bot.Wait.ForPickup("Revenant's Spellscroll");
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger($"Farmer finished {quant}x Revenant's Spellscroll.");
                break;
            }

            C.EnsureAccept(6897);
            Bot.Quests.UpdateQuest(2060);

            FarmerHandler("judgement", "r10a", "Left", "*", "Aeacus Empowered", 50);
            FarmerHandler("revenant", "r2", "Left", "*", "Tethered Soul", 300);
            FarmerHandler("shadowrealmpast", "Enter", "Spawn", "*", "Darkened Essence", 500);
            FarmerHandler("necrodungeon", "r22", "Left", "*", "Dracolich Contract", 1000);

            C.EnsureComplete(6897);
            Bot.Wait.ForPickup("Revenant's Spellscroll");
        }
    }

    void LF2Items(int quant = 6)
    {
        while (!Bot.ShouldExit)
        {
            if (C.CheckInventory("Conquest Wreath", quant))
            {
                Bot.Wait.ForPickup("Conquest Wreath");
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger($"Farmer finished {quant}x Conquest Wreath.");
                break;
            }

            C.EnsureAccept(6898);

            FarmerHandler("doomvault", "r1", "Left", "*", "Grim Cohort Conquered", 400);
            FarmerHandler("mummies", "Enter", "Spawn", "*", "Ancient Cohort Conquered", 400);
            FarmerHandler("wrath", "r2", "Left", "*", "Pirate Cohort Conquered", 400);
            FarmerHandler("doomwar", "r6", "Left", "*", "Battleon Cohort Conquered", 400);
            FarmerHandler("overworld", "Enter", "Spawn", "*", "Mirror Cohort Conquered", 400);
            FarmerHandler("deathpits", "r1", "Left", "*", "Darkblood Cohort Conquered", 400);
            FarmerHandler("maxius", "r4", "Left", "*", "Vampire Cohort Conquered", 400);
            FarmerHandler("curseshore", "Enter", "Spawn", "*", "Spirit Cohort Conquered", 400);
            FarmerHandler("dragonbone", "r2", "Left", "*", "Dragon Cohort Conquered", 400);
            FarmerHandler("doomwood", "r3", "Left", "*", "Doomwood Cohort Conquered", 400);

            C.EnsureComplete(6898);
            Bot.Wait.ForPickup("Conquest Wreath");
        }
    }

    void LF3Items(int quant = 10)
    {
        C.FarmingLogger("Exalted Crown", quant);
        C.AddDrop(LF3);

        while (!Bot.ShouldExit)
        {
            if (C.CheckInventory("Exalted Crown", quant))
            {
                Bot.Wait.ForPickup("Exalted Crown");
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger($"Farmer finished {quant}x Exalted Crown.");
                break;
            }

            C.EnsureAccept(6899);

            // Purchase Cowl
            UpdateState(++_stateSeq, "whitemap", "Enter", "Spawn", "*", false);
            Adv.BuyItem("underworld", 216, "Hooded Legion Cowl");

            ArmyDarkToken(100);

            if (Bot.Config!.Get<bool>("UseFirstClassEntertainment"))
                ArmyFirstClassEntertainment(4000);
            else
                ArmyLegionTokens(4000);

            DiamondTokenofDage(30);
            ArmyEmblemOfDage(1);

            if (Bot.Quests.CanComplete(6899))
            {
                Bot.Quests.Complete(6899);
                Bot.Wait.ForPickup("Exalted Crown");
            }
        }
    }

    void DiamondTokenofDage(int quant = 30)
    {
        if (C.CheckInventory("Diamond Token of Dage", quant))
            return;

        if (!C.CheckInventory("Legion Round 4 Medal"))
            CLR.LegionRound4Medal();
        if (!C.CheckInventory("Legion Token", 50))
            CLR.FarmLegionToken(50);

        C.FarmingLogger("Diamond Token of Dage", quant);
        C.AddDrop("Diamond Token of Dage", "Legion Token");
        C.RegisterQuests(4743);

        while (!Bot.ShouldExit && !C.CheckInventory("Diamond Token of Dage", quant))
        {
            FarmerHandler("tercessuinotlim", "m2", "Left", "*", "Defeated Makai", 25);
            FarmerHandler("aqlesson", "Frame9", "Right", "Carnax", "Carnax Eye", 1);
            FarmerHandler("deepchaos", "Frame4", "Left", "Kathool", "Kathool Tentacle", 1);
            FarmerHandler("lair", "End", "Left", "Red Dragon", "Red Dragon's Fang", 1);
            FarmerHandler("bloodtitan", "Enter", "Spawn", "Blood Titan", "Blood Titan's Blade", 1);
            FarmerHandler("dflesson", "r12", "Right", "*", null, 1, itemId: 33257);
        }
        C.CancelRegisteredQuests();
    }

    void ArmyEmblemOfDage(int quant)
    {
        FarmerHandler(
            map: "shadowblast",
            aggroCell: "r12",
            pad: "Left",
            monster: "*",
            itemName: "Emblem of Dage",
            quant: quant,
            questIds: new int[] { 4742 }
        );
    }

    void ArmyLegionTokens(int quant)
    {
        FarmerHandler(
            map: "dreadrock",
            aggroCell: "r3",
            pad: "Left",
            monster: "*",
            itemName: "Legion Token",
            quant: quant,
            questIds: new int[] { 4849 }
        );
    }

    void ArmyFirstClassEntertainment(int quant)
    {
        FarmerHandler(
            map: "legionarena",
            aggroCell: "Boss",
            pad: "Left",
            monster: "*",
            itemName: "Legion Token",
            quant: quant,
            questIds: new int[] { 6743 }
        );
    }

    void ArmyDarkToken(int quant)
    {
        FarmerHandler(
            map: "seraphicwardage",
            aggroCell: "r3",
            pad: "Left",
            monster: "*",
            itemName: "Dark Token",
            quant: quant,
            questIds: new int[] { 6248, 6249, 6251 }
        );
    }

    private void FarmerHandler(
        string map,
        string aggroCell,
        string pad,
        string monster,
        string? itemName = null,
        int quant = 0,
        int? itemId = null,
        int[]? questIds = null
    )
    {
        Func<bool> isComplete = () =>
        {
            if (itemName != null)
                return C.CheckInventory(itemName, quant);
            if (itemId != null)
                return C.CheckInventory(itemId.Value, quant);
            return true;
        };

        if (isComplete())
            return;

        if (questIds != null && questIds.Length > 0)
            C.RegisterQuests(questIds);

        if (itemName != null)
            C.AddDrop(itemName);
        else if (itemId != null)
            C.AddDrop(itemId.Value);

        if (map == "revenant")
            C.GhostItem(47465, "Revenant Map Bypass", 1, false, category: Skua.Core.Models.Items.ItemCategory.Class, "Used to bypass the dark caster class requirement for the map \"Revenant\"");
        if (map == "mummies")
            Bot.Quests.UpdateQuest(4614);
        if (map == "judgement")
            Bot.Quests.UpdateQuest(2060);

        // Notify helpers of target map and cell
        UpdateState(++_stateSeq, map, aggroCell, pad, monster, false);

        C.Join(map);
        C.Jump(aggroCell, pad);
        Bot.Player.SetSpawnPoint();

        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;

        Stopwatch antiAfkWatch = Stopwatch.StartNew();

        while (!Bot.ShouldExit && !isComplete())
        {
            // 1. Anti-AFK Packet guard
            if (Bot.Player.AFK || antiAfkWatch.ElapsedMilliseconds > 45000)
            {
                Bot.Send.Packet("%xt%zm%afk%1%false%");
                antiAfkWatch.Restart();
            }

            // 2. Death & Respawn check: When revived, FORCE jump back to the farming cell
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Bot.Sleep(1000);
                C.Jump(aggroCell, pad);
                Bot.Player.SetSpawnPoint();
                continue;
            }

            // 3. Cell Drift check: ensure character hasn't drifted to 'Enter' or wrong cell
            if (Bot.Player.Cell != aggroCell)
            {
                C.Jump(aggroCell, pad);
                Bot.Sleep(500);
            }

            // 4. Ensure Aggro remains enabled
            if (!Bot.Options.AggroMonsters)
                Bot.Options.AggroMonsters = true;

            // 5. Active Combat targeting
            var availableMon = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m != null && m.Alive);
            if (availableMon != null)
            {
                Bot.Combat.Attack(availableMon.MapID);
            }
            else
            {
                Bot.Combat.Attack(monster);
            }

            Bot.Sleep(500);
        }

        Bot.Options.AggroMonsters = false;
        C.Jump("Enter", "Spawn");

        if (questIds != null && questIds.Length > 0)
            C.CancelRegisteredQuests();
    }

    #endregion

    #region Helper Logic

    private void RunHelper(string farmer, bool isHelper3)
    {
        C.Logger($"Helper active! Following farmer '{farmer}'.");

        int lastSequence = -1;
        Stopwatch helperAfkWatch = Stopwatch.StartNew();

        try
        {
            while (!Bot.ShouldExit)
            {
                var state = ReadState();
                if (state == null)
                {
                    Bot.Sleep(1000);
                    continue;
                }

                if (state.Value.isDone)
                {
                    C.Logger($"Farmer '{farmer}' has finished Legion Revenant! Helper shutting down.");
                    Bot.Options.AggroMonsters = false;
                    C.Join("whitemap");
                    break;
                }

                // Farmer transitioned to a new step
                if (state.Value.seq != lastSequence)
                {
                    lastSequence = state.Value.seq;
                    Bot.Options.AggroMonsters = false;

                    if (state.Value.map.Equals("whitemap", StringComparison.OrdinalIgnoreCase))
                    {
                        C.Join("whitemap");
                        Bot.Sleep(1000);
                        continue;
                    }

                    // /revenant map has a 3-player room cap (Farmer + Helper 1 + Helper 2)
                    // Helper 3 safely waits in whitemap so no room-full error occurs
                    if (state.Value.map.Equals("revenant", StringComparison.OrdinalIgnoreCase) && isHelper3)
                    {
                        C.Logger("/revenant cap is 3 players. Helper 3 resting in /whitemap...");
                        C.Join("whitemap");
                        continue;
                    }

                    if (state.Value.map.Equals("revenant", StringComparison.OrdinalIgnoreCase))
                    {
                        C.GhostItem(47465, "Revenant Map Bypass", 1, false, category: Skua.Core.Models.Items.ItemCategory.Class, "Used to bypass the dark caster class requirement for the map \"Revenant\"");
                    }
                    else if (state.Value.map.Equals("mummies", StringComparison.OrdinalIgnoreCase))
                    {
                        Bot.Quests.UpdateQuest(4614);
                    }
                    else if (state.Value.map.Equals("judgement", StringComparison.OrdinalIgnoreCase))
                    {
                        Bot.Quests.UpdateQuest(2060);
                    }

                    C.Join(state.Value.map);
                    C.Jump(state.Value.cell, state.Value.pad);
                    Bot.Player.SetSpawnPoint();
                    Bot.Sleep(1000);
                    Bot.Options.AggroMonsters = true;
                }

                // If Helper 3 is resting during /revenant, wait for next map
                if (state.Value.map.Equals("revenant", StringComparison.OrdinalIgnoreCase) && isHelper3)
                {
                    Bot.Sleep(1000);
                    continue;
                }

                // 1. Anti-AFK guard
                if (Bot.Player.AFK || helperAfkWatch.ElapsedMilliseconds > 45000)
                {
                    Bot.Send.Packet("%xt%zm%afk%1%false%");
                    helperAfkWatch.Restart();
                }

                // 2. Combat & Position loop
                if (Bot.Map.Name.Equals(state.Value.map, StringComparison.OrdinalIgnoreCase))
                {
                    // Respawn check
                    if (!Bot.Player.Alive)
                    {
                        Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                        Bot.Sleep(1000);
                        C.Jump(state.Value.cell, state.Value.pad);
                        Bot.Player.SetSpawnPoint();
                        continue;
                    }

                    // Cell drift check
                    if (Bot.Player.Cell != state.Value.cell)
                    {
                        C.Jump(state.Value.cell, state.Value.pad);
                        Bot.Sleep(500);
                    }

                    if (!Bot.Options.AggroMonsters)
                        Bot.Options.AggroMonsters = true;

                    var availableMon = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m != null && m.Alive);
                    if (availableMon != null)
                    {
                        Bot.Combat.Attack(availableMon.MapID);
                    }
                    else
                    {
                        Bot.Combat.Attack(state.Value.monster);
                    }
                }
                else
                {
                    C.Join(state.Value.map);
                    C.Jump(state.Value.cell, state.Value.pad);
                    Bot.Player.SetSpawnPoint();
                }

                Bot.Sleep(500);
            }
        }
        finally
        {
            Bot.Skills.Stop();
            Bot.Options.AggroMonsters = false;
        }
    }

    #endregion

    #region State Synchronization Helpers

    private void UpdateState(int seq, string map, string cell, string pad, string monster, bool isDone)
    {
        string content = $"{seq}|{map}|{cell}|{pad}|{monster}|{(isDone ? "1" : "0")}";
        for (int i = 0; i < 15; i++)
        {
            try
            {
                File.WriteAllText(StateFilePath, content);
                return;
            }
            catch (IOException)
            {
                Bot.Sleep(100);
            }
            catch (Exception ex)
            {
                C.Logger($"State write error: {ex.Message}");
                return;
            }
        }
    }

    private (int seq, string map, string cell, string pad, string monster, bool isDone)? ReadState()
    {
        if (!File.Exists(StateFilePath))
            return null;

        for (int i = 0; i < 15; i++)
        {
            try
            {
                string text = File.ReadAllText(StateFilePath).Trim();
                if (string.IsNullOrEmpty(text))
                    return null;

                string[] parts = text.Split('|');
                if (parts.Length < 6)
                    return null;

                if (!int.TryParse(parts[0], out int seq))
                    return null;

                string map = parts[1];
                string cell = parts[2];
                string pad = parts[3];
                string monster = parts[4];
                bool isDone = parts[5] == "1";

                return (seq, map, cell, pad, monster, isDone);
            }
            catch (IOException)
            {
                Bot.Sleep(100);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    #endregion
}