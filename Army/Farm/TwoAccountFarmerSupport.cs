/*
name: Two Account Farmer and Support
description: One account farms a monster drop while a second account helps in the same room.
tags: army, farm, support, two accounts
*/
//cs_include Scripts/CoreBots.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TwoAccountFarmerSupport
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "TwoAccountFarmerSupport";
    public List<IOption> Options = new()
    {
        new Option<string>("FarmerAccount", "Farmer account", "Account that receives the item and runs the quest.", ""),
        new Option<string>("SupportAccount", "Support account", "Account that helps fight but does not run the farm.", ""),
        new Option<string>("FarmMap", "Map", "Map name without a room number.", ""),
        new Option<string>("FarmMonster", "Monster", "Exact monster name.", ""),
        new Option<string>("FarmItem", "Item to farm", "Exact item name.", ""),
        new Option<int>("FarmQuantity", "Quantity", "Quantity needed on the farmer account.", 1),
        new Option<bool>("TemporaryItem", "Temporary item", "Enable for a temporary quest drop; disable for a regular inventory item.", false),
        new Option<int>("FarmQuest", "Quest ID", "Quest to register on the farmer only; use 0 if none.", 0),
        new Option<int>("FarmRoom", "Private room number", "Use the same options on both clients; number must be at least 1000.", 123456),
        new Option<string>("SupportClass", "Support class", "Class to equip on the support account; blank keeps its current class.", "StoneCrusher"),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        try
        {
            Run();
        }
        finally
        {
            Bot.Options.AggroMonsters = false;
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
        string farmer = Bot.Config!.Get<string>("FarmerAccount")?.Trim() ?? "";
        string support = Bot.Config.Get<string>("SupportAccount")?.Trim() ?? "";
        string map = Bot.Config.Get<string>("FarmMap")?.Trim() ?? "";
        string monster = Bot.Config.Get<string>("FarmMonster")?.Trim() ?? "";
        string item = Bot.Config.Get<string>("FarmItem")?.Trim() ?? "";
        int quantity = Bot.Config.Get<int>("FarmQuantity");
        int quest = Bot.Config.Get<int>("FarmQuest");
        int room = Bot.Config.Get<int>("FarmRoom");
        bool temporary = Bot.Config.Get<bool>("TemporaryItem");
        string supportClass = Bot.Config.Get<string>("SupportClass")?.Trim() ?? "";

        if (farmer.Length == 0 || support.Length == 0 || map.Length == 0 || monster.Length == 0 ||
            item.Length == 0 || quantity < 1 || quest < 0 || room < 1000 || room > 999999 ||
            farmer.Equals(support, StringComparison.OrdinalIgnoreCase) || map.Contains('-'))
        {
            Core.Logger("Fill in two different account names, a map, monster, item, positive quantity, and a room number from 1000 to 999999.");
            return;
        }

        bool isFarmer = Core.Username().Equals(farmer, StringComparison.OrdinalIgnoreCase);
        bool isSupport = Core.Username().Equals(support, StringComparison.OrdinalIgnoreCase);
        if (!isFarmer && !isSupport)
        {
            Core.Logger("This account is neither the configured farmer nor the configured support account.");
            return;
        }

        if (isFarmer && HasItem(item, quantity, temporary))
        {
            Core.Logger($"Farmer already has {quantity}x {item}.");
            return;
        }

        if (isSupport && supportClass.Length > 0)
        {
            if (!Core.CheckInventory(supportClass))
            {
                Core.Logger($"Support account does not have {supportClass}.");
                return;
            }
            Core.Equip(supportClass);
        }

        // Core.Join skips a join when already on the same base map, so leave it first.
        Core.Join("whitemap");
        Core.Join($"{map}-{room}");
        Bot.Wait.ForMapLoad(map);

        string partner = isFarmer ? support : farmer;
        Core.Logger($"Waiting for {partner} in /{map}-{room}.");
        while (!Bot.ShouldExit && !PartnerPresent(partner))
            Bot.Sleep(500);
        if (Bot.ShouldExit)
            return;

        if (isFarmer)
        {
            if (quest > 0)
                Core.RegisterQuests(quest);
            try
            {
                Core.Logger($"Farmer collecting {quantity}x {item} from {monster}.");
                Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
                Core.HuntMonster(map, monster, item, quantity, isTemp: temporary);
            }
            finally
            {
                Bot.Skills.Stop();
                if (quest > 0)
                    Core.CancelRegisteredQuests();
                // Leaving the room lets the support account detect completion.
                Core.Join("whitemap");
            }
        }
        else
        {
            string activeClass = Bot.Player.CurrentClass?.Name ?? "generic";
            Core.Logger($"Supporting {farmer} with {activeClass}; no quest or target drop is configured here.");
            Bot.Skills.StartAdvanced(activeClass, false);
            int absentChecks = 0;
            try
            {
                while (!Bot.ShouldExit && absentChecks < 20)
                {
                    if (PartnerPresent(farmer))
                    {
                        absentChecks = 0;
                        if (Bot.Player.Alive)
                        {
                            var target = Core.FindMonster(map, monster);
                            if (target != null)
                            {
                                if (Bot.Player.Cell != target.Cell)
                                    Core.Jump(target.Cell ?? "Enter", "Left");
                                if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
                                    Bot.Combat.Attack(monster);
                            }
                        }
                    }
                    else
                        absentChecks++;
                    Bot.Sleep(500);
                }
            }
            finally
            {
                Bot.Skills.Stop();
            }
            Core.Logger("Farmer left the room; support stopped.");
        }
    }

    private bool PartnerPresent(string name) =>
        Bot.Map.PlayerNames?.Any(player => player.Equals(name, StringComparison.OrdinalIgnoreCase)) == true;

    private bool HasItem(string item, int quantity, bool temporary) =>
        temporary ? Bot.TempInv.Contains(item, quantity) : Core.CheckInventory(item, quantity);
}
