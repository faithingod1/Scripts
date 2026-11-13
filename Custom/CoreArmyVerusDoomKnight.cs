/*
name: null
description: VDK quest worker used by Custom/ArmyVerusDoomKnight.cs.
tags: versus, verus, doomknight, vdk, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs

//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs

//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Other/Classes/Necromancer.cs

//cs_include Scripts/Evil/NSoD/CoreNSOD.cs

//cs_include Scripts/Other/MergeShops/TerminaTempleMerge.cs

//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DoomPirateStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/MergeShops/DoomPirateHaulMerge.cs
//cs_include Scripts/Prototypes/InfernalArenaLW.cs

using System.Dynamic;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Skills;
using System.Linq;
using System;

public class CoreArmyVerusDoomKnight
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private readonly CoreAOR AOR = new();
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static SepulchuresOriginalHelm SOH
    {
        get => _SOH ??= new SepulchuresOriginalHelm();
        set => _SOH = value;
    }
    private static SepulchuresOriginalHelm _SOH;
    private static ArchDoomKnight ADK
    {
        get => _ADK ??= new ArchDoomKnight();
        set => _ADK = value;
    }
    private static ArchDoomKnight _ADK;
    private static SRoD SRoD
    {
        get => _SRoD ??= new SRoD();
        set => _SRoD = value;
    }
    private static SRoD _SRoD;
    private static TerminaTempleMerge TTMerge
    {
        get => _TTMerge ??= new TerminaTempleMerge();
        set => _TTMerge = value;
    }
    private static TerminaTempleMerge _TTMerge;
    private static DoomPirateHaulMerge DPHM
    {
        get => _DPHM ??= new DoomPirateHaulMerge();
        set => _DPHM = value;
    }
    private static DoomPirateHaulMerge _DPHM;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static InfernalArenaLW Infernal { get => _Infernal ??= new(); set => _Infernal = value; }
    private static InfernalArenaLW _Infernal;

    public Action<int, bool, string>? ReportStep;
    public Func<int, int, string, bool>? WaitStepBarrier;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    private bool FarmQuestRequirement(int questID, int stepIndex, string itemName, Action<int, string> farm)
    {
        if (Bot.ShouldExit) return false;
        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(questID));
        ItemBase? requirement = quest?.Requirements.FirstOrDefault(item =>
            item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (requirement == null)
        {
            Core.Logger($"VDK quest {questID} has no requirement named {itemName}; stopping.");
            return false;
        }

        if (!requirement.Temp) Core.Unbank(requirement.Name);
        int have = requirement.Temp
            ? Bot.TempInv.GetQuantity(requirement.Name)
            : Bot.Inventory.GetQuantity(requirement.Name);
        Core.Logger($"VDK [{questID}] {requirement.Name}: have {have}/{requirement.Quantity}, missing {Math.Max(0, requirement.Quantity - have)}.");

        ReportStep?.Invoke(stepIndex, false, requirement.Name);

        // Core hunt methods take the desired total, not the missing quantity.
        if (have < requirement.Quantity)
            farm(requirement.Quantity, requirement.Name);
        if (Bot.ShouldExit) return false;

        bool ready = requirement.Temp
            ? Bot.TempInv.Contains(requirement.ID, requirement.Quantity)
            : Core.CheckInventory(requirement.ID, requirement.Quantity);
        if (!ready)
        {
            Core.Logger($"VDK [{questID}] {requirement.Name} is still missing; stopping.");
            return false;
        }

        ReportStep?.Invoke(stepIndex, true, requirement.Name);

        if (WaitStepBarrier != null && !WaitStepBarrier(questID, stepIndex, requirement.Name))
            return false;

        return true;
    }

    private bool FinishQuest(int questID)
    {
        if (Bot.ShouldExit) return false;
        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(questID));
        if (quest == null) return false;
        foreach (ItemBase requirement in quest.Requirements)
        {
            bool ready = requirement.Temp
                ? Bot.TempInv.Contains(requirement.ID, requirement.Quantity)
                : Core.CheckInventory(requirement.ID, requirement.Quantity);
            if (!ready)
            {
                Core.Logger($"VDK [{questID}] still needs {requirement.Name} x{requirement.Quantity}; not submitting.");
                return false;
            }
        }
        if (!Core.EnsureComplete(questID)) return false;
        Bot.Wait.ForQuestComplete(questID);
        return Core.isCompletedBefore(questID);
    }

    private void BuyVoidTooth(string item, int target)
    {
        int missing = Math.Max(0, target - Bot.Inventory.GetQuantity(item));
        int energyTarget = missing * 50;
        if (energyTarget > 0 && !Core.CheckInventory("Void Energy", energyTarget))
            Core.HuntMonster("thevoid", "Ninja", "Void Energy", energyTarget, isTemp: false);
        Adv.BuyItem("thevoid", 1406, item, target);
    }

    // Return after the requested quest so the army can choose the next account.
    // The final quest also purchases the class and finishes its rank-up.
    public void GetClass(bool rankup = true, int stopAfterQuestId = 0)
    {
        Story.PreLoad(this);

        Farm.Experience(80);

        Core.EquipClass(ClassType.Solo);
        // Body, Soul and, Domination (9411)
        if (!Story.QuestProgression(9411))
        {
            Core.EnsureAccept(9411);
            if (!FarmQuestRequirement(9411, 0, "Fear's Bones", (q, n) => Core.HuntMonster("underrealm", "Fear", n, q, false))) return;
            if (!FarmQuestRequirement(9411, 1, "Gray Matter", (q, n) => Core.HuntMonster("brainmeat", "Brain Matter", n, q, false))) return;
            Bot.Quests.UpdateQuest(8777);
            if (!FarmQuestRequirement(9411, 2, "Titanic Spine", (q, n) => Core.HuntMonster("titanattack", "Titanic DoomKnight", n, q, false))) return;
            if (!FarmQuestRequirement(9411, 3, "Doom Knight Plating", (q, n) => Core.HuntMonsterMapID("valleyofdoom", 25, n, q, false))) return;
            if (!FinishQuest(9411)) return;
        }
        if (stopAfterQuestId == 9411) return;

        // Of the Same Cloak (9412)
        if (!Story.QuestProgression(9412))
        {
            Farm.Experience(50);
            Core.EnsureAccept(9412);
            if (!FarmQuestRequirement(9412, 0, "The Mask of the Skulls", (q, n) => Core.HuntMonsterMapID("necrodungeon", 47, n, q, false))) return;
            if (!FarmQuestRequirement(9412, 1, "Doom Worshipper's Blade Of Doom", (q, n) => Core.HuntMonster("lumafortress", "Corrupted Luma", n, q, false))) return;
            Core.Logger(
                " \"Empress' ShadowCloak\" 's Droprate is fairly low... so dont complain if it takes \"hours\" according to reddit... "
            );
            if (!FarmQuestRequirement(9412, 2, "Empress' ShadowCloak", (q, n) => Core.HuntMonster("innershadows", "Krahen", n, q, false))) return;
            Bot.Quests.UpdateQuest(7646);
            if (!FarmQuestRequirement(9412, 3, "Cybernetic Doom Blade", (q, n) => Core.HuntMonster("techfortress", "MechaVortrix", n, q, false))) return;
            Core.GhostItem(55823, "Kyger", 1, false, ItemCategory.Pet, "Time for training!", 1);
            Bot.Quests.UpdateQuest(7650);
            if (!FarmQuestRequirement(9412, 4, "Asherion Armor", (q, n) => Core.HuntMonsterMapID("stonewooddeep", 16, n, q, false))) return;
            if (!FinishQuest(9412)) return;
        }
        if (stopAfterQuestId == 9412) return;

        // Refracted Light (9413)
        if (!Story.QuestProgression(9413))
        {
            Farm.Experience(60);
            Core.EnsureAccept(9413);
            Core.EquipClass(ClassType.Farm);
            if (!FarmQuestRequirement(9413, 0, "Shadowflame Spike", (q, n) => Core.HuntMonster("brightshadow", "Shadowflame Paladin", n, q, false))) return;
            if (!FarmQuestRequirement(9413, 1, "Light Fiend Horn", (q, n) => Core.HuntMonster("fiendshard", "Paladin Fiend", n, q, false))) return;
            if (!FarmQuestRequirement(9413, 2, "Underworld Soul Glow", (q, n) => Core.HuntMonster("legionarena", "Dark Legion Paladin", n, q, false))) return;
            Core.EquipClass(ClassType.Solo);
            if (!FarmQuestRequirement(9413, 3, "Gold Hammer Chip", (q, n) => Core.HuntMonster("noxustower", "General Goldhammer", n, q, false))) return;
            if (!FarmQuestRequirement(9413, 4, "Shimmering Tentacle", (q, n) => Core.HuntMonster("chaoslab", "Chaos Artix", n, q, false))) return;
            if (!FinishQuest(9413)) return;
        }
        if (stopAfterQuestId == 9413) return;

        // Life Carve (9417)
        if (!Story.QuestProgression(9417))
        {
            Farm.Experience(70);
            Farm.EvilREP();
            Bot.Quests.UpdateQuest(3773);
            Core.EnsureAccept(9417);
            Core.Logger(
                "The map \"Wanders\", is a bit broke,\n"
                    + "it will take a minute to hunt the mosnter"
            );
            if (!FarmQuestRequirement(9417, 0, "Trace of Light", (q, n) => Core.HuntMonsterMapID("wanders", 46, n, q, false))) return;
            if (!FarmQuestRequirement(9417, 1, "Trace of Dark", (q, n) => Core.HuntMonster("lightguardwar", "Extreme Noxus", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 2, "Trace of Wind", (q, n) => Core.HuntMonster("eternalchaos", "Bandit Drakath", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 3, "Trace of Earth", (q, n) => Core.HuntMonster("quibblehunt", "Entropy Dragon", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 4, "Trace of Energy", (q, n) => Core.HuntMonster("crashsite", "ProtoSartorium", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 5, "Trace of Ice", (q, n) => Core.HuntMonster("northlands", "Aisha's Drake", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 6, "Trace of Water", (q, n) => Core.HuntMonster("deepchaos", "Kathool", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 7, "Trace of Fire", (q, n) => Core.HuntMonster("drakonnan", "Ultra Drakonnan", n, q, false))) return;
            if (!FarmQuestRequirement(9417, 8, "Trace of Bacon", (q, n) => Core.HuntMonster("battlefowl", "Zeuster Projection", n, q, false))) return;
            if (!FinishQuest(9417)) return;
        }
        if (stopAfterQuestId == 9417) return;

        // Soul Fracture (9416)
        if (!Story.QuestProgression(9416))
        {
            Core.EnsureAccept(9416);

            if (!FarmQuestRequirement(9416, 0, "Soul of Alteon", (q, n) => Core.HuntMonster("ultraalteon", "Ultra Chaos Alteon", n, q, false))) return;
            if (!FarmQuestRequirement(9416, 1, "Soul of Dethrix", (q, n) => Core.HuntMonster("ebondungeon", "Dethrix", n, q, false))) return;
            if (!FarmQuestRequirement(9416, 2, "Soul of Sepulchuroth", (q, n) => Core.HuntMonster("shadowstrike", "Sepulchuroth", n, q, false))) return;
            if (!FarmQuestRequirement(9416, 3, "Soul of Drakath", (q, n) => Core.HuntMonster("ultradrakath", "Champion of Chaos", n, q, false))) return;
            if (!FarmQuestRequirement(9416, 4, "Soul of Gravelyn", (q, n) => Core.HuntMonster("ebilcorphq", "Gravelyn", n, q, false))) return;
            if (!FarmQuestRequirement(9416, 5, "Soul of Doom", (q, n) => Core.HuntMonster("shadowvoid", "Fragment of Doom", n, q, false))) return;

            if (!FinishQuest(9416)) return;
        }
        if (stopAfterQuestId == 9416) return;

        // Doom Spikes (9418)
        if (!Story.QuestProgression(9418))
        {
            Farm.Experience(80);
            Core.EnsureAccept(9418);
            if (!FarmQuestRequirement(9418, 0, "Doomkitten's Molar", (q, n) =>
            {
                Adv.GearStore(EnhAfter: true);
                try { Core.KillDoomKitten(n, q, false); }
                finally { Adv.GearStore(true, true); }
            })) return;
            if (!FarmQuestRequirement(9418, 1, "Deadly Duo's Decayed Denture", (q, n) => Infernal.FarmDeadlyDuo(n, q))) return;
            if (!FarmQuestRequirement(9418, 2, "Maw of the Sea", (q, n) => AOR.FightVoiceInTheSea(n, q))) return;
            if (!FarmQuestRequirement(9418, 3, "Xyfrag's Slimy Tooth", (q, n) => BuyVoidTooth(n, q))) return;
            if (!FarmQuestRequirement(9418, 4, "Nerfkitten's Fang", (q, n) => BuyVoidTooth(n, q))) return;

            if (!FinishQuest(9418)) return;
        }
        if (stopAfterQuestId == 9418) return;

        // Necrotic Blade (9414)
        // FIX: previously checked 9419 which caused this to rerun whenever Unleash Doom wasn't done.
        if (!Story.QuestProgression(9414))
        {
            Core.EnsureAccept(9414);
            if (!FarmQuestRequirement(9414, 0, "ShadowReaper Of Doom", (q, n) => SRoD.ShadowReaperOfDoom())) return;
            if (!FarmQuestRequirement(9414, 1, "Sepulchure's Original Helm", (q, n) => SOH.DoAll())) return;
            if (!FarmQuestRequirement(9414, 2, "Arch DoomKnight Helm", (q, n) => ADK.AMeansToAnEnd(HelmOnly: true))) return;
            if (!FarmQuestRequirement(9414, 3, "Dragonlord of Evil", (q, n) => TTMerge.BuyAllMerge(n))) return;
            if (!FarmQuestRequirement(9414, 4, "DoomTech DoomKnight", (q, n) => DPHM.BuyAllMerge(n))) return;

            //Ensure Everything is unbanked.
            Core.Unbank(
                "Dragonlord of Evil",
                "DoomTech DoomKnight",
                "Arch DoomKnight Helm",
                "Sepulchure's Original Helm",
                "ShadowReaper Of Doom"
            );
            if (!FinishQuest(9414)) return;
        }
        if (stopAfterQuestId == 9414) return;

        // Unleash Doom (9419)
        if (!Story.QuestProgression(9419))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(9419);
            if (!FarmQuestRequirement(9419, 0, "Inquisitor Bones", (q, n) => Core.HuntMonster("citadelruins", "Inquisitor Hobo", n, q, false))) return;

            // FIX: deltavlab -> deltavcore (Refined Metal)
            if (!FarmQuestRequirement(9419, 1, "Refined Metal", (q, n) => Core.HuntMonster("deltavcore", "Pistol Guard", n, q, false))) return;

            if (!FarmQuestRequirement(9419, 2, "Dragon Skin", (q, n) => Core.HuntMonster("etherwardes", "Earth Dragon Warrior", n, q, false))) return;
            Core.EquipClass(ClassType.Solo);
            if (!FarmQuestRequirement(9419, 3, "PaladinSlayer's Skull", (q, n) => Core.HuntMonster("necrocavern", "Chaos Vordred", n, q, false))) return;
            if (!FinishQuest(9419)) return;
        }

        foreach (int questID in new[] { 9411, 9412, 9413, 9417, 9416, 9418, 9414, 9419 })
        {
            if (!Core.isCompletedBefore(questID))
            {
                Core.Logger($"VDK quest {questID} is incomplete; stopping before class purchase.");
                return;
            }
        }

        if (!Core.CheckInventory("Verus DoomKnight"))
            Core.BuyItem("terminatemple", 2343, "Verus DoomKnight");
        if (!Core.CheckInventory("Verus DoomKnight"))
        {
            Core.Logger("Verus DoomKnight was not purchased; stopping before rank-up.");
            return;
        }

        if (rankup)
        {
            int rank = Core.CheckClassRank(ClassName: "Verus DoomKnight");
            for (int attempt = 1; attempt <= 3 && rank < 10 && !Bot.ShouldExit; attempt++)
            {
                Adv.RankUpClass("Verus DoomKnight");
                int nextRank = Core.CheckClassRank(ClassName: "Verus DoomKnight");
                if (nextRank <= rank) break;
                rank = nextRank;
            }
            Core.Logger($"Verus DoomKnight class rank: {Core.CheckClassRank(ClassName: "Verus DoomKnight")}/10.");
        }
    }


}
