/*
name: Army Blood Gems (1 Low-Level Farmer + up to 3 High-Level Carries)
description: Farms "Blood Gem of the Archfiend" on 1 low-level account while up to 3 high-level accounts follow, protect, and carry in combat using their currently equipped classes.
tags: army, blood gem, bloodgem, carry, support, nation, nulgath, kiss the void, bloody chaos
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Players;
using Skua.Core.Options;
using Skua.Core.Utils;

public class ArmyBloodGems
{
    public enum FarmingMethod
    {
        BestAvailable,
        KissTheVoid,
        BloodyChaos,
    }

    public enum HydraHeadLevel
    {
        Head_85 = 85,
        Head_90 = 90,
    }

    private sealed class Status
    {
        public string State = "";
        public string Map = "";
        public string Cell = "";
        public bool Fighting;
        public int BloodGems;
        public DateTime PublishedUtc;
    }

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv => CoreAdvanced.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    private volatile string _state = "STARTING";
    private readonly object _statusWriteLock = new();
    private string _supportClass = "generic";
    private DateTime _farmerMissingSince = DateTime.MinValue;
    private DateTime _lastRoomJoin = DateTime.MinValue;
    private DateTime _startedUtc;
    private DateTime _lastPublishError = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyBloodGems";
    public List<IOption> Options = new()
    {
        new Option<string>("FarmerAccount", "Farmer account (Low-level)", "Account that runs quests and receives all Blood Gems.", ""),
        new Option<string>("SupportAccount1", "Support account 1 (High-level)", "First high-level combat supporter.", ""),
        new Option<string>("SupportAccount2", "Support account 2 (High-level)", "Second supporter (leave blank if unused).", ""),
        new Option<string>("SupportAccount3", "Support account 3 (High-level)", "Third supporter (leave blank if unused).", ""),
        new Option<int>("BloodGemQuantity", "Target Blood Gems", "Target quantity of Blood Gem of the Archfiend (1 to 100).", 100),
        new Option<FarmingMethod>("Method", "Farming Method", "BestAvailable auto-detects based on level/pets; KissTheVoid works at any level; BloodyChaos requires level 80+.", FarmingMethod.BestAvailable),
        new Option<HydraHeadLevel>("HydraLevel", "Hydra Level (Bloody Chaos)", "Level of Hydra Head to fight during Bloody Chaos.", HydraHeadLevel.Head_85),
        new Option<bool>("RelicSupplies", "Bloody Chaos Supplies", "Complete Supplies to Spin The Wheel during Bloody Chaos for extra Nation mats.", true),
        new Option<bool>("FarmerKeepCurrentClass", "Farmer keep current class", "Keep the farmer's currently equipped class instead of swapping between Solo and Farm classes.", true),
        new Option<int>("PrivateRoom", "Private room number", "Use the same private room on all clients (1000 to 999999).", 123456),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        try
        {
            Bot.Config?.Load();
            _state = "STARTING";
            _startedUtc = DateTime.UtcNow;

            string farmer = Bot.Config?.Get<string>("FarmerAccount")?.Trim() ?? "";
            var supporters = new List<string>();
            for (int slot = 1; slot <= 3; slot++)
            {
                string sup = Bot.Config?.Get<string>($"SupportAccount{slot}")?.Trim() ?? "";
                if (sup.Length > 0 && !supporters.Contains(sup, StringComparer.OrdinalIgnoreCase))
                    supporters.Add(sup);
            }

            int room = Bot.Config?.Get<int>("PrivateRoom") ?? 123456;
            int targetQuant = Bot.Config?.Get<int>("BloodGemQuantity") ?? 100;
            if (targetQuant < 1) targetQuant = 1;
            if (targetQuant > 100) targetQuant = 100;

            FarmingMethod method = Bot.Config?.Get<FarmingMethod>("Method") ?? FarmingMethod.BestAvailable;
            HydraHeadLevel hydraLevel = Bot.Config?.Get<HydraHeadLevel>("HydraLevel") ?? HydraHeadLevel.Head_85;
            bool relicSupplies = Bot.Config?.Get<bool>("RelicSupplies") ?? true;
            bool keepClass = Bot.Config?.Get<bool>("FarmerKeepCurrentClass") ?? true;

            if (string.IsNullOrWhiteSpace(farmer) || supporters.Count == 0 ||
                supporters.Any(s => s.Equals(farmer, StringComparison.OrdinalIgnoreCase)) ||
                room < 1000 || room > 999999)
            {
                Core.Logger("Configuration error: Set 1 Farmer account, at least 1 different Support account, and room 1000-999999.");
                return;
            }

            bool isFarmer = Core.Username().Equals(farmer, StringComparison.OrdinalIgnoreCase);
            bool isSupporter = supporters.Any(s => s.Equals(Core.Username(), StringComparison.OrdinalIgnoreCase));

            if (!isFarmer && !isSupporter)
            {
                Core.Logger($"This client ({Core.Username()}) is neither the configured Farmer nor one of the Supporters.");
                return;
            }

            string folder = ClientFileSources.SkuaOptionsDIR;
            Directory.CreateDirectory(folder);

            // Deterministic sync paths based on room and roster
            var allMembers = new List<string> { farmer };
            allMembers.AddRange(supporters.OrderBy(s => s, StringComparer.OrdinalIgnoreCase));
            using SHA256 sha = SHA256.Create();
            string rosterKey = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(room + "|" + string.Join("|", allMembers.Select(m => m.ToLowerInvariant())))))
                .Replace("-", "").Substring(0, 16);

            string farmerSyncPath = Path.Combine(folder, $"ArmyBG_{rosterKey}_farmer.sync");
            string mySyncPath = isFarmer
                ? farmerSyncPath
                : Path.Combine(folder, $"ArmyBG_{rosterKey}_{Core.Username().ToLowerInvariant()}.sync");

            Core.Logger($"Army Blood Gems: Running as {(isFarmer ? "FARMER (Recipient)" : "SUPPORTER (Carry)")}. Roster: Farmer [{farmer}], Supporters [{string.Join(", ", supporters)}]. Target: {targetQuant} Blood Gems.");

            Core.PrivateRooms = true;
            Core.PrivateRoomNumber = room;
            Core.PublicDifficult = false;

            using CancellationTokenSource cancel = new();
            Publish(mySyncPath);
            Task publisher = Task.Run(async () =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    Publish(mySyncPath);
                    try { await Task.Delay(750, cancel.Token); }
                    catch (OperationCanceledException) { break; }
                }
            });

            try
            {
                if (isFarmer)
                {
                    RunFarmer(farmerSyncPath, supporters, targetQuant, method, hydraLevel, relicSupplies, keepClass);
                }
                else
                {
                    RunSupporter(farmer, farmerSyncPath);
                }
            }
            finally
            {
                if (_state != "DONE") _state = "STOPPED";
                cancel.Cancel();
                publisher.GetAwaiter().GetResult();
                Publish(mySyncPath);
                Bot.Skills.Stop();
                Bot.Options.AggroMonsters = false;
                Bot.Options.AggroAllMonsters = false;
            }
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void RunFarmer(string myPath, List<string> supporters, int targetQuantity, FarmingMethod method, HydraHeadLevel hydraLevel, bool relicSupplies, bool keepClass)
    {
        if (Core.CheckInventory("Blood Gem of the Archfiend", targetQuantity))
        {
            Core.Logger($"Farmer: Already owns {targetQuantity}x Blood Gem of the Archfiend!");
            _state = "DONE";
            Publish(myPath);
            return;
        }

        if (keepClass)
        {
            Core.SoloClass = "(Current)";
            Core.FarmClass = "(Current)";
        }

        // Keep an assembled group together. PlayerNames is the room roster;
        // PlayerInfo objects may not yet be available for every player.
        if (!supporters.Any(IsPlayerInRoom))
        {
            Bot.Map.Join($"battleon-{Core.PrivateRoomNumber}");
            Bot.Wait.ForMapLoad("battleon");
        }
        _state = "READY";
        Publish(myPath);
        Core.Logger("Farmer: Waiting for at least 1 supporter in the private room...");
        DateTime lastLog = DateTime.MinValue;

        while (!Bot.ShouldExit)
        {
            // Presence satisfies the startup handshake. Do not also require a
            // supporter heartbeat: it can lag behind the in-game room roster.
            if (supporters.Any(IsPlayerInRoom)) break;

            if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Farmer: Waiting in [{Bot.Map.Name}] for [{string.Join(", ", supporters)}]. " +
                    $"Players detected: [{string.Join(", ", Bot.Map.PlayerNames?.ToArray() ?? Array.Empty<string>())}].");
                lastLog = DateTime.UtcNow;
            }
            Bot.Sleep(200);
        }

        if (Bot.ShouldExit) return;

        Core.Logger($"Supporter detected! Starting Blood Gems farming (Target: {targetQuantity}, Method: {method}).");
        _state = "FARMING";
        Publish(myPath);

        try
        {
            Core.AddDrop("Blood Gem of the Archfiend");

            switch (method)
            {
                case FarmingMethod.BestAvailable:
                    Core.Logger("Farming method: BestAvailable (auto-detecting level & pets)");
                    Nation.FarmBloodGem(targetQuantity, (int)hydraLevel);
                    break;

                case FarmingMethod.KissTheVoid:
                    Core.Logger("Farming method: Kiss the Void (Quest 3743 - Tendurrr / Blindingsnow / Evilwarnul)");
                    Nation.KisstheVoid(targetQuantity);
                    break;

                case FarmingMethod.BloodyChaos:
                    if (Bot.Player.Level < 80)
                    {
                        Core.Logger($"Farmer level {Bot.Player.Level} is below 80! Bloody Chaos requires level 80+. Falling back to Kiss the Void.");
                        Nation.KisstheVoid(targetQuantity);
                    }
                    else
                    {
                        Core.Logger($"Farming method: Bloody Chaos (Quest 7816 - Hydra Head {(int)hydraLevel} / Escherion / Vath)");
                        Nation.BloodyChaos(targetQuantity, relicSupplies, (int)hydraLevel);
                    }
                    break;
            }

            int currentCount = Bot.Inventory.GetQuantity("Blood Gem of the Archfiend");
            if (currentCount >= targetQuantity)
            {
                _state = "DONE";
                Core.Logger($"Blood Gems farming completed successfully! Owned: {currentCount}/{targetQuantity}.");
            }
            else
            {
                _state = "STOPPED";
                Core.Logger($"Farming ended. Current Blood Gems: {currentCount}/{targetQuantity}.");
            }
        }
        catch (Exception ex)
        {
            Core.Logger($"Farmer encountered an error: {ex.Message}");
            _state = "STOPPED";
        }
        finally
        {
            if (_state != "DONE") _state = "STOPPED";
            Publish(myPath);
        }
    }

    private void RunSupporter(string farmerName, string farmerSyncPath)
    {
        if (!PrepareSupport())
            return;

        _state = "READY";
        Core.Logger($"Supporter ready. Joining Farmer [{farmerName}] before farming begins...");
        DateTime lastWaitLog = DateTime.MinValue;

        while (!Bot.ShouldExit)
        {
            Status? farmerStatus = Read(farmerSyncPath);
            if (farmerStatus?.State == "DONE" || farmerStatus?.State == "STOPPED")
            {
                // A terminal file from the previous run must not stop a
                // supporter that is being started before the farmer.
                if (farmerStatus.PublishedUtc >= _startedUtc)
                {
                    _state = farmerStatus.State;
                    return;
                }
            }
            if (farmerStatus != null && (farmerStatus.State == "READY" || farmerStatus.State == "FARMING"))
            {
                try
                {
                    FollowAndAssist(farmerName, farmerStatus, false);
                    if (IsPlayerInRoom(farmerName))
                        break;
                }
                catch (Exception ex)
                {
                    if (DateTime.UtcNow - lastWaitLog >= TimeSpan.FromSeconds(10))
                        Core.Logger($"Supporter rendezvous retry: {ex.Message}");
                }
            }

            if (DateTime.UtcNow - lastWaitLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Waiting for Farmer [{farmerName}] to start ArmyBloodGems...");
                lastWaitLog = DateTime.UtcNow;
            }
            Bot.Sleep(200);
        }

        if (Bot.ShouldExit) return;

        _state = "SUPPORTING";
        Core.Logger($"Farmer [{farmerName}] active. Following and carrying in combat!");

        DateTime lastError = DateTime.MinValue;

        while (!Bot.ShouldExit)
        {
            Status? farmerStatus = Read(farmerSyncPath);

            if (farmerStatus == null)
            {
                StopCombat();
                Bot.Sleep(250);
                continue;
            }

            if (farmerStatus.State == "DONE")
            {
                Core.Logger($"Farmer [{farmerName}] finished farming Blood Gems! (Collected: {farmerStatus.BloodGems}). Supporter halting.");
                _state = "DONE";
                break;
            }

            if (farmerStatus.State == "STOPPED")
            {
                Core.Logger($"Farmer [{farmerName}] stopped; supporter halting.");
                _state = "STOPPED";
                break;
            }

            try
            {
                FollowAndAssist(farmerName, farmerStatus, farmerStatus.State == "FARMING");
            }
            catch (Exception ex)
            {
                if (DateTime.UtcNow - lastError >= TimeSpan.FromSeconds(30))
                {
                    Core.Logger($"Support assist note: {ex.Message}");
                    lastError = DateTime.UtcNow;
                }
            }

            Bot.Sleep(200);
        }
    }

    private bool IsPlayerInRoom(string name) =>
        Bot.Map.PlayerNames?.Any(playerName =>
            string.Equals(playerName?.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)) == true;

    private static string CleanMap(string? map)
    {
        if (string.IsNullOrWhiteSpace(map)) return "";
        string m = map.Trim();
        int dash = m.LastIndexOf('-');
        if (dash >= 0 && int.TryParse(m.Substring(dash + 1), out _))
            m = m.Substring(0, dash);
        return m.Trim().ToLowerInvariant();
    }

    private void StopCombat()
    {
        if (Bot.Skills.TimerRunning)
            Bot.Skills.Stop();
        Bot.Combat.CancelTarget();
    }

    private void FollowAndAssist(string leader, Status status, bool allowCombat)
    {
        string targetMap = CleanMap(status.Map);
        if (targetMap.Length == 0 || targetMap == "whitemap")
        {
            StopCombat();
            return;
        }

        string myMap = CleanMap(Bot.Map?.Name);

        // 1. If in a different map, disengage combat and join farmer's map
        if (!string.Equals(myMap, targetMap, StringComparison.OrdinalIgnoreCase))
        {
            StopCombat();
            for (int i = 0; i < 10 && Bot.Player.InCombat && !Bot.ShouldExit; i++)
            {
                Bot.Combat.CancelTarget();
                Bot.Sleep(150);
            }

            Core.Logger($"Supporter: Moving to [{targetMap}] to follow [{leader}]...");
            Core.Join(targetMap, ignoreCheck: true);
            _lastRoomJoin = DateTime.UtcNow;
            Bot.Wait.ForMapLoad(targetMap);
            Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);
            Bot.Sleep(300);
            return;
        }

        // 2. In the same map, locate farmer's cell
        string targetCell = status.Cell?.Trim() ?? "";
        PlayerInfo? player = Bot.Map.Players?.FirstOrDefault(p => string.Equals(p.Name, leader, StringComparison.OrdinalIgnoreCase));
        if (!IsPlayerInRoom(leader))
        {
            StopCombat();
            if (_farmerMissingSince == DateTime.MinValue)
                _farmerMissingSince = DateTime.UtcNow;
            if (DateTime.UtcNow - _farmerMissingSince > TimeSpan.FromSeconds(2) &&
                DateTime.UtcNow - _lastRoomJoin > TimeSpan.FromSeconds(5))
            {
                Bot.Map.Join($"{targetMap}-{Core.PrivateRoomNumber}");
                _lastRoomJoin = DateTime.UtcNow;
                Bot.Wait.ForMapLoad(targetMap);
            }
            return;
        }
        _farmerMissingSince = DateTime.MinValue;
        if (!string.IsNullOrWhiteSpace(player?.Cell))
            targetCell = player.Cell.Trim();

        string myCell = Bot.Player?.Cell?.Trim() ?? "";

        // If in a different cell, cancel combat lock and jump to farmer's cell
        if (targetCell.Length > 0 && !string.Equals(myCell, targetCell, StringComparison.OrdinalIgnoreCase))
        {
            StopCombat();
            for (int i = 0; i < 10 && Bot.Player.InCombat && !Bot.ShouldExit; i++)
            {
                Bot.Combat.CancelTarget();
                Bot.Sleep(150);
            }

            string pad = player?.Pad ?? "Left";
            Bot.Map.Jump(targetCell, pad, false);
            Bot.Wait.ForCellChange(targetCell);
            Bot.Sleep(200);
            return;
        }

        // 3. In the exact same map and cell: fight alongside the farmer
        if (allowCombat && targetCell.Length > 0 && string.Equals(myCell, targetCell, StringComparison.OrdinalIgnoreCase))
        {
            if (Bot.Player.Alive)
            {
                if (!Bot.Skills.TimerRunning)
                    Bot.Skills.StartAdvanced(_supportClass, false);

                if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
                {
                    if (status.Fighting || Bot.Player.InCombat || (Bot.Monsters.CurrentMonsters?.Any(m => m.Alive) == true))
                        Bot.Combat.Attack("*");
                }
            }
        }
        else
            StopCombat();
    }

    private bool PrepareSupport()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";
        if (string.IsNullOrWhiteSpace(className) || className == "Generic")
        {
            Core.Logger($"Supporter [{Core.Username()}] has no class equipped; please equip a class before starting.");
            return false;
        }
        Core.Logger($"Supporter [{Core.Username()}] using currently equipped class: {className}. Forge enhancements preserved.");

        _supportClass = className;
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;
        return true;
    }

    private void Publish(string path)
    {
        lock (_statusWriteLock)
        {
            try
            {
                string map = CleanMap(Bot.Map?.Name);
                string cell = Bot.Player?.Cell?.Trim() ?? "";
                bool fighting = (Bot.Player?.HasTarget == true) || (Bot.Player?.InCombat == true);
                int bloodGems = Bot.Inventory?.GetQuantity("Blood Gem of the Archfiend") ?? 0;
                string temporary = path + ".tmp";
                File.WriteAllText(temporary, $"{_state}|{map}|{cell}|{(fighting ? 1 : 0)}|{bloodGems}");
                File.Move(temporary, path, true);
            }
            catch (Exception ex)
            {
                if (DateTime.UtcNow - _lastPublishError >= TimeSpan.FromSeconds(10))
                {
                    _lastPublishError = DateTime.UtcNow;
                    Core.Logger($"Cannot publish army heartbeat [{path}]: {ex.Message}");
                }
            }
        }
    }

    private Status? Read(string path)
    {
        try
        {
            DateTime publishedUtc = File.GetLastWriteTimeUtc(path);
            if (!File.Exists(path) || DateTime.UtcNow - publishedUtc > TimeSpan.FromSeconds(30))
                return null;
            string[] fields = File.ReadAllText(path).Split('|');
            if (fields.Length < 4)
                return null;
            return new Status
            {
                State = fields[0],
                Map = CleanMap(fields[1]),
                Cell = fields[2].Trim(),
                Fighting = fields[3] == "1",
                BloodGems = fields.Length >= 5 && int.TryParse(fields[4], out int bg) ? bg : 0,
                PublishedUtc = publishedUtc,
            };
        }
        catch (Exception) { return null; }
    }
}

