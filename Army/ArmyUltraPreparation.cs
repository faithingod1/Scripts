/*
name: Four Account Ultra Preparation
description: Run on each of four accounts. Checks Ultras-v3 prerequisites, farms missing requirements in priority order, then checks again.
tags: army, ultras, preparation, classes, forge
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/PrerequisitesChecker.cs
//cs_include Scripts/Ultrasv3/DoAllUltrasStorys.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Other/Classes/REP-based/StoneCrusher.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Other/Classes/VerusDoomKnight.cs
//cs_include Scripts/Other/Classes/KingsEcho.cs
//cs_include Scripts/Other/Classes/REP-based/Shaman.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs

using System;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ArmyUltraPreparation
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots C => CoreBots.Instance;
    private CoreFarms Farm = new();
    private CoreAdvanced Adv = new();
    private UnlockForgeEnhancements Forge = new();
    private static readonly ItemCategory[] WeaponCategories =
    {
        ItemCategory.Sword, ItemCategory.Axe, ItemCategory.Dagger,
        ItemCategory.Gun, ItemCategory.HandGun, ItemCategory.Rifle,
        ItemCategory.Bow, ItemCategory.Mace, ItemCategory.Gauntlet,
        ItemCategory.Polearm, ItemCategory.Staff, ItemCategory.Wand, ItemCategory.Whip
    };

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyUltraPreparation";
    public System.Collections.Generic.List<IOption> Options = new()
    {
        new Option<bool>("PrepareStories", "Prepare ultra stories", "Complete the story unlocks used by the Ultras-v3 fights.", true),
        new Option<bool>("FarmClasses", "Farm missing classes", "Obtain and rank the classes in the full prerequisite checklist, plus Lord of Order.", true),
        new Option<bool>("FarmForge", "Unlock required Forge", "Unlock base Forge weapon, Lacerate, Praxis, base Forge helm and cape.", true),
        new Option<bool>("FarmDamageGear", "Farm boost gear", "Attempt NSoD and Awescended if no qualifying boost gear is owned. These are long grinds.", true),
        new Option<bool>("UseGold", "Use gold for Forge reputation", "Spend gold to speed up Blacksmithing reputation.", false),
        new Option<bool>("PreFarm", "NSoD pre-farm", "Use the NSoD script's pre-farm path.", false),
        new Option<bool>("getSDKA", "Get SDKA for Void Auras", "If a member, obtain SDKA before farming Void Auras.", false),
        new Option<bool>("MaxStack", "Max Void Aura essence stack", "Farm the larger essence stack during NSoD preparation.", false),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(true);
        try
        {
            C.Logger($"[UltraPrep] Initial prerequisite check for {Bot.Player.Username}.");
            new PrerequisitesChecker().Check();

            RunStage("Level 100", () => Bot.Player.Level >= 100, () => Farm.Experience(100));

            if (Bot.Config!.Get<bool>("PrepareStories"))
                RunTask("Ultra story unlocks", () => new DoAllUltrasStory().DoAllUltrasPrereqs());

            if (Bot.Config.Get<bool>("FarmClasses"))
                PrepareClasses();

            RunStage("Good reputation rank 10", () => Bot.Reputation.GetRank("Good") >= 10, () => Farm.GoodREP(10));
            RunStage("Alchemy reputation rank 8", () => Bot.Reputation.GetRank("Alchemy") >= 8, () => Farm.AlchemyREP(8));

            if (Bot.Config.Get<bool>("FarmForge"))
                PrepareForge();

            RunStage("10 million gold", () => Bot.Player.Gold >= 10_000_000, () => Farm.Gold(10_000_000));

            if (Bot.Config.Get<bool>("FarmDamageGear"))
                PrepareBoostGear();

            if (!Bot.ShouldExit)
            {
                C.Logger($"[UltraPrep] Final prerequisite check for {Bot.Player.Username}.");
                bool ready = new PrerequisitesChecker().Check();
                C.Logger(ready
                    ? $"[UltraPrep] {Bot.Player.Username} passed the full prerequisite checker."
                    : $"[UltraPrep] {Bot.Player.Username} still has missing requirements. Review the checker report.",
                    ready ? "Info" : "Warning");
            }
        }
        finally
        {
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    private void PrepareClasses()
    {
        // Support roles are used across the widest range of Ultras-v3 fights.
        RunClass("Lord of Order", () => new LordOfOrder().GetLoO());
        RunClass("StoneCrusher", () => new StoneCrusher().GetSC());
        RunClass("ArchPaladin", () => new ArchPaladin().GetAP());
        RunClass("Verus DoomKnight", () => new VerusDoomKnightClass().GetClass());
        RunClass("King's Echo", () => new KingsEcho().GetKE());

        // These complete the remaining class entries of PrerequisitesChecker.
        RunClass("Shaman", () => new Shaman().GetShaman());
        RunClass("ArchFiend", () => new ArchFiend().GetArchfiend());
        RunClass("Dragon of Time", () => new DragonOfTime().GetDoT());
        RunStage("Warrior class", () => C.CheckInventory("Warrior"),
            () => C.BuyItem("trainers", 170, "Warrior"));
    }

    private void RunClass(string name, Action acquire)
    {
        if (Bot.ShouldExit)
            return;

        if (!C.CheckInventory(name))
            RunStage($"Get {name}", () => C.CheckInventory(name), acquire);

        if (C.CheckInventory(name) && C.CheckClassRank(ClassName: name) < 10)
            RunStage($"Rank {name} to 10", () => C.CheckClassRank(ClassName: name) >= 10,
                () => Adv.RankUpClass(name));
    }

    private void PrepareForge()
    {
        RunStage("Forge weapon", () => Adv.uForgeWeapon(), () => Forge.ForgeWeaponEnhancement());
        RunStage("Lacerate", () => Adv.uLacerate(), () => Forge.Lacerate());
        RunStage("Praxis", () => Adv.uPraxis(), () => Forge.Praxis());
        RunStage("Forge helm", () => Adv.uForgeHelm(), () => Forge.ForgeHelmEnhancement());
        RunStage("Forge cape", () => Adv.uForgeCape(), () => Forge.ForgeCapeEnhancement());
    }

    private void PrepareBoostGear()
    {
        EquipOwnedBoostGear();

        if (!HasBoostWeapon())
        {
            RunStage("Necrotic Sword of Doom", () => C.CheckInventory("Necrotic Sword of Doom"),
                () => new CoreNSOD().GetNSOD());
            EquipOwnedBoostGear();
        }

        if (!HasRaceBoostArmorOrPet())
        {
            RunStage("Awescended gear", () => C.CheckInventory("Awescended"),
                () => new Awescended().GetAwe());
            EquipOwnedBoostGear();
        }
    }

    private void EquipOwnedBoostGear()
    {
        var items = Bot.Inventory.Items.Concat(Bot.Bank.Items).ToList();
        var weapon = items.Where(i => WeaponCategories.Contains(i.Category))
            .OrderByDescending(i => C.GetBoostFloat(i, "dmgAll"))
            .FirstOrDefault(i => C.GetBoostFloat(i, "dmgAll") >= 1.4f);
        if (weapon != null)
            EquipIfNeeded(weapon.Name);

        string[] races = { "Human", "Undead", "Dragonkin", "Chaos", "Elemental" };
        var armorOrPet = items.FirstOrDefault(i =>
            (i.Category == ItemCategory.Armor ||
             i.CategoryString?.Equals("Pet", StringComparison.OrdinalIgnoreCase) == true) &&
            races.All(r => C.GetBoostFloat(i, r) >= 1.3f));
        if (armorOrPet != null)
            EquipIfNeeded(armorOrPet.Name);
    }

    private void EquipIfNeeded(string name)
    {
        if (Bot.Inventory.Items.Any(i => i.Name == name && i.Equipped))
            return;
        C.Unbank(name);
        C.Equip(name);
    }

    private bool HasBoostWeapon()
    {
        var equipped = Bot.Inventory.Items.FirstOrDefault(i => i.Equipped &&
            WeaponCategories.Contains(i.Category));
        return equipped != null && C.GetBoostFloat(equipped, "dmgAll") >= 1.4f;
    }

    private bool HasRaceBoostArmorOrPet()
    {
        string[] races = { "Human", "Undead", "Dragonkin", "Chaos", "Elemental" };
        return Bot.Inventory.Items.Any(i => i.Equipped &&
            (i.Category == ItemCategory.Armor ||
             i.CategoryString?.Equals("Pet", StringComparison.OrdinalIgnoreCase) == true) &&
            races.All(r => C.GetBoostFloat(i, r) >= 1.3f));
    }

    private void RunStage(string label, Func<bool> complete, Action work)
    {
        if (Bot.ShouldExit || complete())
            return;

        C.Logger($"[UltraPrep] Starting: {label}");
        try
        {
            work();
            if (!Bot.ShouldExit)
            {
                bool done = complete();
                C.Logger($"[UltraPrep] {label}: {(done ? "complete" : "still missing")}",
                    done ? "Info" : "Warning");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[UltraPrep] {label} failed: {ex.Message}", "Error");
        }
    }

    private void RunTask(string label, Action work)
    {
        if (Bot.ShouldExit)
            return;
        C.Logger($"[UltraPrep] Starting: {label}");
        try
        {
            work();
            C.Logger($"[UltraPrep] Finished: {label}. Boss access is checked during each fight.");
        }
        catch (Exception ex)
        {
            C.Logger($"[UltraPrep] {label} failed: {ex.Message}", "Error");
        }
    }
}
