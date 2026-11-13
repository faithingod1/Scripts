/*
name: Avarice Cape Enhancement (Bypass The Beast)
description: Farms only the missing requirements for Avarice (quest 8745), without fighting The Beast.
tags: forge, avarice, cape, enhancement, sevencircleswar, bypass
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
using System;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class AvariceBypassBeast
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private readonly CoreFarms Farm = new();
    private readonly CoreStory Story = new();
    private readonly SevenCircles Circles = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(new[]
        {
            "Indulgence", "Penance", "Essence of Wrath", "Essence of Violence",
            "Essence of Treachery", "Souls of Heresy"
        });

        Core.SetOptions();
        try { UnlockAvariceOnly(); }
        finally { Core.SetOptions(false); }
    }

    public void UnlockAvariceOnly()
    {
        const int questID = 8745;
        if (Core.isCompletedBefore(questID))
        {
            Core.Logger("Avarice Cape Enhancement is already unlocked.");
            return;
        }

        if (!Core.isCompletedBefore(8744))
        {
            Core.Logger("Complete Vainglory (quest 8744) before farming Avarice.");
            return;
        }

        if (Bot.Player.Level < 90)
            Farm.Experience(90);
        Farm.BlacksmithingREP(9);
        if (Bot.ShouldExit)
            return;

        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(questID));
        ItemBase? indulgence = quest?.Requirements.FirstOrDefault(r => r.Name == "Indulgence");
        ItemBase? penance = quest?.Requirements.FirstOrDefault(r => r.Name == "Penance");
        if (quest == null || indulgence == null || penance == null)
        {
            Core.Logger("Could not read the Indulgence and Penance requirements for quest 8745.");
            return;
        }

        Core.Unbank("Indulgence", "Penance", "Essence of Wrath", "Essence of Violence",
            "Essence of Treachery", "Souls of Heresy");
        Core.AddDrop("Indulgence", "Penance", "Essence of Wrath", "Essence of Violence",
            "Essence of Treachery", "Souls of Heresy");
        LogRequirement(indulgence);
        LogRequirement(penance);

        if (!HasRequirement(indulgence))
        {
            Circles.Circles();
            if (!Core.isCompletedBefore(7977))
            {
                Core.Logger("Seven Circles story is incomplete; Indulgence farm was not started.");
                return;
            }
            if (Bot.ShouldExit || !FarmIndulgence(indulgence.Quantity))
                return;
        }

        if (!HasRequirement(penance))
        {
            CirclesWarWithoutBeast();
            if (!Core.isCompletedBefore(7988))
            {
                Core.Logger("Seven Circles War is incomplete through quest 7988; Penance farm was not started.");
                return;
            }
            if (Bot.ShouldExit || !FarmPenance(penance.Quantity))
                return;
        }

        if (Bot.ShouldExit || !HasRequirement(indulgence) || !HasRequirement(penance))
        {
            Core.Logger("Avarice requirements are still missing; quest 8745 was not submitted.");
            return;
        }

        Core.EnsureAccept(questID);
        Core.EnsureComplete(questID);
        if (Core.isCompletedBefore(questID))
            Core.Logger("Avarice Cape Enhancement unlocked.");
        else
            Core.Logger("Quest 8745 did not complete; check its current requirements.");
    }

    private bool HasRequirement(ItemBase requirement) =>
        requirement.Temp
            ? Bot.TempInv.Contains(requirement.ID, requirement.Quantity)
            : Core.CheckInventory(requirement.ID, requirement.Quantity);

    private void LogRequirement(ItemBase requirement)
    {
        int have = requirement.Temp
            ? Bot.TempInv.GetQuantity(requirement.Name)
            : Bot.Inventory.GetQuantity(requirement.Name);
        Core.Logger($"Avarice needs {requirement.Quantity} {requirement.Name}; inventory has {have}; missing {Math.Max(0, requirement.Quantity - have)}.");
    }

    private void SafeEquip(ClassType type)
    {
        if (type == ClassType.Farm && !string.IsNullOrEmpty(Core.FarmClass))
            Core.EquipClass(ClassType.Farm);
        else if (type == ClassType.Solo && !string.IsNullOrEmpty(Core.SoloClass))
            Core.EquipClass(ClassType.Solo);
    }

    // The existing SevenCircles story is used for quests 7968-7977. Stop the war
    // after 7988 so neither Treachery nor The Beast is required by this script.
    public void CirclesWarWithoutBeast()
    {
        if (Core.isCompletedBefore(7988))
            return;

        Circles.Circles();
        Story.PreLoad(this);
        SafeEquip(ClassType.Farm);
        Story.KillQuest(7979, "sevencircleswar", "Wrath Guard");
        Story.KillQuest(7980, "sevencircleswar", "Wrath Guard");
        Story.KillQuest(7981, "sevencircleswar", "Wrath Guard");
        SafeEquip(ClassType.Solo);
        Story.KillQuest(7982, "sevencircleswar", "Wrath");
        SafeEquip(ClassType.Farm);
        Story.KillQuest(7983, "sevencircleswar", "Heresy Guard");
        SafeEquip(ClassType.Solo);
        Story.KillQuest(7984, "sevencircleswar", "Violence's Gatekeeper");
        SafeEquip(ClassType.Farm);
        Story.KillQuest(7985, "sevencircleswar", "Violence Guard");
        SafeEquip(ClassType.Solo);
        Story.KillQuest(7986, "sevencircleswar", "Geryon");
        Story.KillQuest(7987, "sevencircleswar", "Violence");
        SafeEquip(ClassType.Farm);
        Story.KillQuest(7988, "sevencircleswar", "Treachery Guard");
    }

    public bool FarmIndulgence(int target)
    {
        if (Core.CheckInventory("Indulgence", target))
            return true;

        Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(7978));
        if (quest == null || quest.Requirements.Count == 0)
        {
            Core.Logger("Could not load Circles of Fate (quest 7978).");
            return false;
        }

        while (!Bot.ShouldExit && !Core.CheckInventory("Indulgence", target))
        {
            int before = Bot.Inventory.GetQuantity("Indulgence");
            int batch = Math.Min(3, target - before);
            Core.Logger($"Indulgence: {before}/{target}; collecting materials for {batch} turn-in(s).");
            Core.EnsureAccept(7978);

            foreach (ItemBase req in quest.Requirements)
            {
                if (Bot.ShouldExit) return false;
                string monster = req.Name switch
                {
                    "Souls of Limbo" => "Limbo Guard",
                    "Essence of Luxuria" => "Luxuria",
                    "Essence of Gluttony" => "Gluttony",
                    "Essence of Avarice" => "Avarice",
                    _ => ""
                };
                if (monster.Length == 0)
                {
                    Core.Logger($"No farm route for quest 7978 requirement: {req.Name}.");
                    return false;
                }

                SafeEquip(monster == "Limbo Guard" ? ClassType.Farm : ClassType.Solo);
                Core.HuntMonster("sevencircles", monster, req.Name, req.Quantity * batch,
                    isTemp: req.Temp, log: false);
            }

            Core.EnsureCompleteMulti(7978, batch);
            Bot.Wait.ForPickup("Indulgence");
            if (Bot.Inventory.GetQuantity("Indulgence") <= before)
            {
                Core.Logger("Circles of Fate did not award Indulgence; stopping to avoid repeating the farm.");
                return false;
            }
        }
        return !Bot.ShouldExit && Core.CheckInventory("Indulgence", target);
    }

    public bool FarmPenance(int target)
    {
        if (Core.CheckInventory("Penance", target))
            return true;

        int missingPenance = target - Bot.Inventory.GetQuantity("Penance");
        Core.Logger($"Penance missing: {missingPenance}. Checking existing merge materials first.");

        // Each Penance uses one of each essence. Farm each missing material once,
        // in order, after accounting for what is already in inventory.
        if (!FarmWarMaterial(7979, "Wrath Guard", "Essence of Wrath", missingPenance)) return false;
        if (!FarmWarMaterial(7985, "Violence Guard", "Essence of Violence", missingPenance)) return false;
        if (!FarmWarMaterial(7988, "Treachery Guard", "Essence of Treachery", missingPenance)) return false;

        // Souls of Heresy stack to 300, enough for 20 Penance. Use any Souls
        // already held, then farm only the next amount that fits in inventory.
        while (!Bot.ShouldExit && !Core.CheckInventory("Penance", target))
        {
            int before = Bot.Inventory.GetQuantity("Penance");
            int remaining = target - before;
            int souls = Bot.Inventory.GetQuantity("Souls of Heresy");
            int soulsTarget = Math.Min(300, remaining * 15);
            if (souls < 15)
            {
                Core.Logger($"Souls of Heresy: {souls}/{soulsTarget}; farming the missing amount for the next merge batch.");
                Core.RegisterQuests(7980, 7981, 7983);
                try
                {
                    SafeEquip(ClassType.Farm);
                    Core.HuntMonster("sevencircleswar", "Heresy Guard", "Souls of Heresy",
                        soulsTarget, isTemp: false, log: false);
                }
                finally { Core.CancelRegisteredQuests(); }
                souls = Bot.Inventory.GetQuantity("Souls of Heresy");
            }

            int batch = Math.Min(remaining, souls / 15);
            if (batch <= 0)
            {
                Core.Logger("Could not collect enough Souls of Heresy for one Penance.");
                return false;
            }

            if (!Core.CheckInventory("Essence of Wrath", batch) ||
                !Core.CheckInventory("Essence of Violence", batch) ||
                !Core.CheckInventory("Essence of Treachery", batch) ||
                !Core.CheckInventory("Souls of Heresy", batch * 15))
            {
                Core.Logger("Penance merge materials are still missing; stopping.");
                return false;
            }

            Core.Logger($"Merging {batch} Penance using materials already collected.");
            Core.BuyItem("sevencircleswar", 1984, "Penance", before + batch);
            Bot.Wait.ForPickup("Penance");
            if (Bot.Inventory.GetQuantity("Penance") <= before)
            {
                Core.Logger("Penance merge did not increase the item count; stopping.");
                return false;
            }
        }
        return !Bot.ShouldExit && Core.CheckInventory("Penance", target);
    }

    private bool FarmWarMaterial(int questID, string monster, string item, int target)
    {
        int have = Bot.Inventory.GetQuantity(item);
        Core.Logger($"{item}: have {have}/{target}; missing {Math.Max(0, target - have)}.");
        if (Core.CheckInventory(item, target))
            return true;

        Core.RegisterQuests(questID);
        try
        {
            SafeEquip(ClassType.Farm);
            Core.HuntMonster("sevencircleswar", monster, item, target, isTemp: false, log: false);
        }
        finally { Core.CancelRegisteredQuests(); }
        return !Bot.ShouldExit && Core.CheckInventory(item, target);
    }
}
