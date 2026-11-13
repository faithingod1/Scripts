/*
name: Army Forge (2 to 4 accounts)
description: Farms all Forge enhancements in progress order starting from the lowest progress, and waits for all configured accounts after each stage and requirement.
tags: army, forge, enhancement, weapon, cape, helm, lacerate, smite, herosvaliance, arcanasconcerto, elysium, acheron, absolution, vainglory, avarice, penitence, lament, vim, examen, anima, pneuma, dauntless, praxis, ravenous
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Army/ArmyForgeQuestRunner.cs

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

public class ArmyForge
{
    private sealed class Member
    {
        public string Name = "";
        public string Path = "";
    }

    private sealed class Status
    {
        public string State = "";
        public int Stage;
        public int Step;
        public string Map = "";
        public string Cell = "";
        public bool Fighting;
    }

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private readonly object _writeLock = new();
    private volatile string _state = "READY";
    private int _stage;
    private int _step;
    private DateTime _startedUtc;
    private readonly HashSet<string> _soloMaps = new(StringComparer.OrdinalIgnoreCase)
        { "grimgaol", "gaolcell", "baconcatyou", "chaoslord", "baconcat", "baconcatb" };
    private DateTime _lastFollowError = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyForge";
    public List<IOption> Options = new()
    {
        new Option<string>("Account1", "Account 1 username", "Run this script on this account.", ""),
        new Option<string>("Account2", "Account 2 username", "Run this script on this account.", ""),
        new Option<string>("Account3", "Account 3 username", "Run this script on this account.", ""),
        new Option<string>("Account4", "Account 4 username", "Run this script on this account.", ""),
        new Option<int>("PrivateRoom", "Private room number", "Use the same room on all accounts (1000 to 999999).", 123456),
        new Option<bool>("UseGold", "Use Gold", "Speed the Blacksmithing REP grind up with Gold?", false),
        new Option<bool>("SkipBlockedEnhancements", "Skip Blocked Enhancements", "Skip enhancements that require missing un-farmable items (Dark Box, LoO dailies, ultra insignias)?", true),
        new Option<bool>("SellQuestClass", "Sell quest classes", "Sell classes back after Vim, Examen, Anima, and Pneuma?", false),
        new Option<bool>("UseInsignOnDaunt", "Use Insignia for Dauntless", "Use Malgor/Tyndarius insignias to buy pre-requisites?", false),
        new Option<bool>("UseInsignOnArcanasConcerto", "Use Insignias for Arcana's Concerto", "Use insignias to buy Darkon's Debris 2 and pre-reqs?", false),
        new Option<string>("SoloMaps", "Additional solo maps", "Comma-separated map names helpers must never follow into. Known solo maps are included automatically.", ""),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        try
        {
            Bot.Config!.Load();
            var members = Enumerable.Range(1, 4)
                .Select(i => new Member { Name = Bot.Config.Get<string>($"Account{i}")?.Trim() ?? "" })
                .Where(m => !string.IsNullOrEmpty(m.Name))
                .ToList();

            int room = Bot.Config.Get<int>("PrivateRoom");
            bool useGold = Bot.Config.Get<bool>("UseGold");
            bool skipBlocked = Bot.Config.Get<bool>("SkipBlockedEnhancements");
            bool sellQuestClass = Bot.Config.Get<bool>("SellQuestClass");
            bool useInsignDaunt = Bot.Config.Get<bool>("UseInsignOnDaunt");
            bool useInsignConcerto = Bot.Config.Get<bool>("UseInsignOnArcanasConcerto");

            // Keep equipped class across all farming and assistance
            Core.SoloClass = Core.FarmClass = Core.DodgeClass = Core.BossClass = "Generic";
            string currentClass = Bot.Player.CurrentClass?.Name ?? "generic";
            Core.Logger($"[ArmyForge] Keeping current equipped class: {currentClass}");

            foreach (string map in (Bot.Config.Get<string>("SoloMaps") ?? "").Split(','))
                if (!string.IsNullOrWhiteSpace(map)) _soloMaps.Add(map.Trim());

            if (members.Count < 2 || members.Count > 4 ||
                members.Select(m => m.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != members.Count ||
                room < 1000 || room > 999999)
            {
                Core.Logger("Enter 2 to 4 distinct account usernames and a private room from 1000 to 999999.");
                return;
            }

            Member? me = members.FirstOrDefault(m => m.Name.Equals(Core.Username(), StringComparison.OrdinalIgnoreCase));
            if (me == null)
            {
                Core.Logger("This client is not one of the configured army accounts.");
                return;
            }

            // All clients must choose the same ordering even when roster slots differ.
            members = members.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase).ToList();
            AssignPaths(members, room);
            Core.PrivateRooms = true;
            Core.PrivateRoomNumber = room;
            _stage = ArmyForgeQuestRunner.FirstIncompleteStage(skipBlocked, useInsignDaunt, useInsignConcerto);
            _state = _stage == ArmyForgeQuestRunner.TotalStages ? "DONE" : "READY";
            _startedUtc = DateTime.UtcNow;

            Core.Logger($"Forge army: {string.Join(", ", members.Select(m => m.Name))}. " +
                (_stage == ArmyForgeQuestRunner.TotalStages
                    ? "All available Forge enhancements already owned!"
                    : $"Starting at stage {_stage}: {ArmyForgeQuestRunner.StageNames[_stage]}."));

            using CancellationTokenSource cancel = new();
            Publish(me.Path);
            Task heartbeat = Task.Run(async () =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    Publish(me.Path);
                    try { await Task.Delay(1000, cancel.Token); }
                    catch (OperationCanceledException) { break; }
                }
            });

            try { Run(members, me, useGold, skipBlocked, sellQuestClass, useInsignDaunt, useInsignConcerto); }
            finally
            {
                if (_state != "DONE") _state = "STOPPED";
                cancel.Cancel();
                heartbeat.GetAwaiter().GetResult();
                Publish(me.Path);
                Bot.Skills.Stop();
                Bot.Combat.CancelTarget();
            }
        }
        finally { Core.SetOptions(false); }
    }

    private void Run(List<Member> members, Member me, bool useGold, bool skipBlocked, bool sellQuestClass, bool useInsignDaunt, bool useInsignConcerto)
    {
        DateTime lastLog = DateTime.MinValue;
        DateTime lastAction = DateTime.MinValue;
        string following = "";

        while (!Bot.ShouldExit)
        {
            var statuses = members.ToDictionary(m => m, m => Read(m.Path, _startedUtc));
            Member? stopped = members.FirstOrDefault(m => statuses[m]?.State == "STOPPED");
            if (stopped != null)
            {
                Core.Logger($"{stopped.Name} stopped before all accounts completed Forge enhancements. Ending this run.");
                return;
            }

            Member[] missing = members.Where(m => statuses[m] == null).ToArray();
            if (missing.Length > 0)
            {
                // Missing clients block progression, not travel by an available helper.
                Member? availableFarmer = members
                    .Where(m => m != me && statuses[m] != null &&
                        statuses[m]!.State == "FARMING" && statuses[m]!.Stage < Volatile.Read(ref _stage))
                    .OrderBy(m => statuses[m]!.Stage)
                    .ThenBy(m => statuses[m]!.Step)
                    .FirstOrDefault();

                if (availableFarmer != null && Bot.Player.LoggedIn)
                {
                    if (following != availableFarmer.Name)
                    {
                        following = availableFarmer.Name;
                        Core.Logger($"Regrouping with {following} while waiting for the missing accounts.");
                    }
                    Assist(availableFarmer.Name, statuses[availableFarmer]!, ref lastAction);
                }

                if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
                {
                    Core.Logger($"Waiting for {string.Join(", ", missing.Select(m => m.Name))}. Start ArmyForge on all configured accounts.");
                    lastLog = DateTime.UtcNow;
                }
                Bot.Sleep(1000);
                continue;
            }

            int firstStage = statuses.Values.Min(s => s!.Stage);
            if (firstStage == ArmyForgeQuestRunner.TotalStages)
            {
                Core.Logger("All army accounts have obtained all Forge enhancements!");
                return;
            }

            int ownStage = Volatile.Read(ref _stage);
            if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(30))
            {
                Core.Logger("Army progress: " + string.Join(", ", members.Select(m =>
                    $"{m.Name}: {(statuses[m]!.Stage == ArmyForgeQuestRunner.TotalStages ? "all completed" : $"{ArmyForgeQuestRunner.StageNames[statuses[m]!.Stage]}")} ({statuses[m]!.State})")));
                lastLog = DateTime.UtcNow;
            }

            if (ownStage == firstStage)
            {
                if (following.Length > 0)
                {
                    Bot.Skills.Stop();
                    Bot.Combat.CancelTarget();
                    following = "";
                }

                _state = "FARMING";
                Volatile.Write(ref _step, 0);
                Publish(me.Path);
                Core.Logger($"Farming stage {ownStage}: {ArmyForgeQuestRunner.StageNames[ownStage]} ({ownStage + 1}/{ArmyForgeQuestRunner.TotalStages}); army progresses at this stage.");

                try
                {
                    var runner = new ArmyForgeQuestRunner();
                    runner.SoloRequirement = (map, farm) => RunSoloRequirement(members, me, map, farm);
                    runner.RequirementCompleted = (stage, step, item) => WaitForRequirement(members, me, stage, step, item);
                    runner.RunStage(ownStage, useGold, sellQuestClass, useInsignDaunt, useInsignConcerto);
                }
                catch (Exception ex)
                {
                    _state = "STOPPED";
                    Publish(me.Path);
                    Core.Logger($"Stage {ArmyForgeQuestRunner.StageNames[ownStage]} failed: {ex.Message}");
                    return;
                }

                int after = ArmyForgeQuestRunner.FirstIncompleteStage(skipBlocked, useInsignDaunt, useInsignConcerto);
                Volatile.Write(ref _step, 0);
                Volatile.Write(ref _stage, after);
                _state = after == ArmyForgeQuestRunner.TotalStages ? "DONE" : "READY";
                Publish(me.Path);

                if (!Bot.ShouldExit && after <= ownStage)
                {
                    _state = "STOPPED";
                    Publish(me.Path);
                    Core.Logger($"Stage {ArmyForgeQuestRunner.StageNames[ownStage]} did not advance. Stopping instead of looping indefinitely.");
                    return;
                }
                continue;
            }

            Member? farmer = members.Where(m => statuses[m]!.Stage == firstStage && statuses[m]!.State == "FARMING")
                .OrderBy(m => statuses[m]!.Step).FirstOrDefault();

            if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Waiting at {ArmyForgeQuestRunner.StageNames[firstStage]} for " +
                    string.Join(", ", members.Where(m => statuses[m]!.Stage == firstStage).Select(m => m.Name)) + ".");
                lastLog = DateTime.UtcNow;
            }

            if (farmer != null)
            {
                if (following != farmer.Name)
                {
                    following = farmer.Name;
                    Core.Logger($"Helping {farmer.Name} on {ArmyForgeQuestRunner.StageNames[firstStage]}.");
                }
                Assist(farmer.Name, statuses[farmer]!, ref lastAction);
            }
            Bot.Sleep(500);
        }
    }

    private void WaitForRequirement(List<Member> members, Member me, int stage, int step, string item)
    {
        if (Bot.ShouldExit) throw new OperationCanceledException();

        // Never run a presence barrier inside a solo map.
        if (_soloMaps.Contains(Bot.Map.Name))
            Core.Join("battleon");

        int completedStep = step * 2;
        Volatile.Write(ref _step, completedStep);
        Publish(me.Path);

        DateTime lastLog = DateTime.MinValue;
        DateTime lastAction = DateTime.MinValue;

        while (!Bot.ShouldExit)
        {
            var statuses = members.ToDictionary(m => m, m => Read(m.Path, _startedUtc));
            if (members.Any(m => statuses[m]?.State == "STOPPED"))
                throw new OperationCanceledException("An army account stopped during a requirement.");

            int ownStage = Volatile.Read(ref _stage);
            Member[] waiting = members.Where(m =>
                statuses[m] == null || statuses[m]!.Stage < ownStage ||
                (statuses[m]!.Stage == ownStage && statuses[m]!.Step < completedStep)).ToArray();

            if (waiting.Length == 0)
            {
                // All accounts on this stage (and any lagging accounts) have acquired the requirement.
                Volatile.Write(ref _step, completedStep + 1);
                Publish(me.Path);
                if (members.All(m =>
                    statuses[m] != null && (statuses[m]!.Stage > ownStage ||
                        (statuses[m]!.Stage == ownStage && statuses[m]!.Step >= completedStep + 1))))
                    return;
                Bot.Sleep(300);
                continue;
            }

            if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Have {item} for stage {stage} ({ArmyForgeQuestRunner.StageNames[stage]}); waiting for {string.Join(", ", waiting.Select(m => m.Name))} to collect enough.");
                lastLog = DateTime.UtcNow;
            }

            Member? farmer = waiting.FirstOrDefault(m => statuses[m]?.Stage == ownStage && statuses[m]?.State == "FARMING");
            if (farmer != null)
                Assist(farmer.Name, statuses[farmer]!, ref lastAction);

            Bot.Sleep(500);
        }
        throw new OperationCanceledException();
    }

    private void RunSoloRequirement(List<Member> members, Member me, string map, Action farm)
    {
        int sharedRoom = Core.PrivateRoomNumber;
        bool previousPrivateRooms = Core.PrivateRooms;
        _state = "SOLO";
        Publish(me.Path);
        Core.Logger($"Solo requirement in /{map}: helpers wait outside; this account farms its own instance.");
        try
        {
            Core.PrivateRooms = true;
            Core.PrivateRoomNumber = 1000 + ((sharedRoom - 1000 + 100 + members.IndexOf(me)) % 999000);
            if (Bot.Map.Name.Equals(map, StringComparison.OrdinalIgnoreCase)) Core.Join("battleon");
            Core.Join(map);
            if (!Bot.ShouldExit) farm();
        }
        finally
        {
            Core.PrivateRoomNumber = sharedRoom;
            Core.PrivateRooms = previousPrivateRooms;
            if (!Bot.ShouldExit) Core.Join("battleon");
            _state = "FARMING";
            Publish(me.Path);
        }
    }

    private void Assist(string name, Status status, ref DateTime lastAction)
    {
        // Do not try Goto or join fallback for an instanced solo fight.
        if (status.State == "SOLO" || _soloMaps.Contains(status.Map)) return;
        if (!Bot.Player.LoggedIn || status.Map.Length == 0 || status.Map.Equals("whitemap", StringComparison.OrdinalIgnoreCase) ||
            DateTime.UtcNow - lastAction < TimeSpan.FromMilliseconds(1200)) return;

        try
        {
            if (!Bot.Map.Name.Equals(status.Map, StringComparison.OrdinalIgnoreCase))
            {
                lastAction = DateTime.UtcNow;
                Bot.Player.Goto(name);
                Bot.Sleep(500);
                if (!Bot.Map.Name.Equals(status.Map, StringComparison.OrdinalIgnoreCase))
                {
                    Core.Join(status.Map);
                    Bot.Wait.ForMapLoad(status.Map);
                }
                return;
            }

            if (Bot.Map.TryGetPlayer(name, out PlayerInfo? player) && player != null)
            {
                string cell = player.Cell ?? status.Cell;
                if (cell.Length > 0 && !cell.Equals(Bot.Player.Cell, StringComparison.OrdinalIgnoreCase))
                {
                    lastAction = DateTime.UtcNow;
                    Bot.Map.Jump(cell, player.Pad ?? "Left", false);
                    return;
                }
            }
            else
            {
                lastAction = DateTime.UtcNow;
                Bot.Player.Goto(name);
                return;
            }

            if (status.Fighting && Bot.Player.Alive)
            {
                lastAction = DateTime.UtcNow;
                if (!Bot.Skills.TimerRunning)
                    Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);
                if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
                    Bot.Combat.Attack("*");
            }
        }
        catch (Exception ex)
        {
            if (DateTime.UtcNow - _lastFollowError >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Could not assist {name}: {ex.Message}");
                _lastFollowError = DateTime.UtcNow;
            }
            lastAction = DateTime.UtcNow;
        }
    }

    private static void AssignPaths(List<Member> members, int room)
    {
        Member[] ordered = members.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        string roster = room + "|" + string.Join("|", ordered.Select(m => m.Name.ToLowerInvariant()));
        using SHA256 sha = SHA256.Create();
        string key = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(roster))).Replace("-", "").Substring(0, 16);
        string folder = ClientFileSources.SkuaOptionsDIR;
        Directory.CreateDirectory(folder);
        for (int i = 0; i < ordered.Length; i++)
            ordered[i].Path = Path.Combine(folder, $"ArmyForge_{key}_{i + 1}.sync");
    }

    private void Publish(string path)
    {
        lock (_writeLock)
        {
            try
            {
                string temporary = path + ".tmp";
                string state = Bot.Player.LoggedIn ? _state : "OFFLINE";
                string map = Bot.Map.Name ?? "";
                string cell = Bot.Player.Cell ?? "";
                File.WriteAllText(temporary,
                    $"{state}|{Volatile.Read(ref _stage)}|{Volatile.Read(ref _step)}|{map}|{cell}|" +
                    (Bot.Player.HasTarget || Bot.Player.InCombat ? "1" : "0"));
                File.Move(temporary, path, true);
            }
            catch (Exception) { }
        }
    }

    private static Status? Read(string path, DateTime startedUtc)
    {
        try
        {
            if (!File.Exists(path) || File.GetLastWriteTimeUtc(path) < startedUtc ||
                DateTime.UtcNow - File.GetLastWriteTimeUtc(path) > TimeSpan.FromSeconds(10))
                return null;
            string[] fields = File.ReadAllText(path).Split('|');
            if (fields.Length != 6 || !int.TryParse(fields[1], out int stage) ||
                !int.TryParse(fields[2], out int step) || stage < 0 || stage > ArmyForgeQuestRunner.TotalStages || step < 0 ||
                (fields[0] != "READY" && fields[0] != "FARMING" && fields[0] != "SOLO" && fields[0] != "DONE" && fields[0] != "STOPPED" && fields[0] != "OFFLINE"))
                return null;
            if (fields[0] == "OFFLINE") return null;
            return new Status { State = fields[0], Stage = stage, Step = step, Map = fields[3], Cell = fields[4], Fighting = fields[5] == "1" };
        }
        catch (Exception) { return null; }
    }
}
