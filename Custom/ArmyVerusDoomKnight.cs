/*
name: Army Verus DoomKnight (up to 4 accounts)
description: Checked accounts take VDK quests in progress order; unchecked accounts only assist in combat.
tags: verus, doomknight, vdk, army, support
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Custom/CoreArmyVerusDoomKnight.cs

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

public class ArmyVerusDoomKnight
{
    public enum SupportClassChoice
    {
        LordOfOrder,
        CoreBotsFarm,
        CoreBotsSolo,
        CoreBotsDodge,
        CoreBotsBoss,
        CurrentClass,
        // Skua reads the saved label "Lord Of Order" as "Lord_Of_Order".
        // Keep both names at the same value so existing settings remain valid.
        Lord_Of_Order = LordOfOrder,
    }

    private sealed class Member
    {
        public string Name = "";
        public bool TakesQuests;
        public string Path = "";
    }

    private sealed class MemberStatus
    {
        public string State = "";
        public int QuestIndex;
        public string Map = "";
        public string Cell = "";
        public bool Fighting;
        public int StepIndex;
        public bool StepDone;
        public string ItemName = "";
    }

    private static readonly int[] QuestIds = { 9411, 9412, 9413, 9417, 9416, 9418, 9414, 9419 };
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv => CoreAdvanced.Instance;
    private volatile string _state = "READY";
    private int _questIndex;
    private int _stepIndex;
    private bool _stepDone;
    private string _currentItem = "";
    private readonly object _statusWriteLock = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyVerusDoomKnight";
    public List<IOption> Options = new()
    {
        new Option<string>("Account1", "Account 1 username", "Run this script on this account.", ""),
        new Option<bool>("Quest1", "Account 1 takes VDK quests", "Tick to farm and finish quests on this account.", true),
        new Option<string>("Account2", "Account 2 username", "Run this script on this account.", ""),
        new Option<bool>("Quest2", "Account 2 takes VDK quests", "Untick for combat support only.", false),
        new Option<string>("Account3", "Account 3 username", "Leave blank if unused.", ""),
        new Option<bool>("Quest3", "Account 3 takes VDK quests", "Untick for combat support only.", false),
        new Option<string>("Account4", "Account 4 username", "Leave blank if unused.", ""),
        new Option<bool>("Quest4", "Account 4 takes VDK quests", "Untick for combat support only.", false),
        new Option<SupportClassChoice>("SupportClass", "Support class", "Choose Lord of Order, current class, or an owned class from a CoreBots class dropdown.", SupportClassChoice.LordOfOrder),
        new Option<int>("SupportActionDelay", "Support action cooldown (ms)", "Minimum gap between follow, jump, attack, and cancel actions.", 1500),
        new Option<bool>("EnhanceGear", "Smart enhance gear", "Enhance each account's equipped class and gear. Requires CoreBots Auto Enhance.", true),
        new Option<int>("PrivateRoom", "Private room number", "Use the same room on all clients (1000 to 999999).", 123456),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        try
        {
            // Each client can retain an older OptionContainer in memory. Read the
            // shared saved roster again before deciding which accounts to wait for.
            Bot.Config!.Load();
            _state = "READY";
            var members = new List<Member>();
            for (int slot = 1; slot <= 4; slot++)
            {
                string name = Bot.Config!.Get<string>($"Account{slot}")?.Trim() ?? "";
                if (name.Length > 0)
                    members.Add(new Member { Name = name, TakesQuests = Bot.Config.Get<bool>($"Quest{slot}") });
            }
            int room = Bot.Config!.Get<int>("PrivateRoom");
            int actionDelay = Bot.Config.Get<int>("SupportActionDelay");
            if (members.Count < 2 || members.Select(m => m.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != members.Count ||
                !members.Any(m => m.TakesQuests) || room < 1000 || room > 999999 || actionDelay < 1000 || actionDelay > 10000)
            {
                Core.Logger("Configure 2 to 4 unique accounts, tick at least one quest taker, set a room from 1000 to 999999, and a support cooldown from 1000 to 10000 ms.");
                return;
            }

            Member? me = members.FirstOrDefault(m => m.Name.Equals(Core.Username(), StringComparison.OrdinalIgnoreCase));
            if (me == null)
            {
                Core.Logger("This client is not one of the configured army accounts.");
                return;
            }

            string folder = ClientFileSources.SkuaOptionsDIR;
            Directory.CreateDirectory(folder);
            AssignSyncPaths(members, room, folder);
            Core.Logger($"Army roster ({members.Count} accounts; blank slots ignored): " +
                string.Join(", ", members.Select(m => $"{m.Name} ({(m.TakesQuests ? "quests" : "support")})")));

            Core.PrivateRooms = true;
            Core.PrivateRoomNumber = room;
            _questIndex = me.TakesQuests ? FirstIncompleteQuest() : -1;
            if (me.TakesQuests && IsFinished(_questIndex))
                _state = "DONE";
            if (me.TakesQuests)
                Core.Logger(_questIndex < QuestIds.Length
                    ? $"Starting VDK progress: next quest {QuestIds[_questIndex]}."
                    : $"Starting VDK progress: all quests complete; class {(_state == "DONE" ? "rank 10" : "purchase/rank-up pending")}.");
            using CancellationTokenSource cancel = new();
            Publish(me.Path);
            Task publisher = Task.Run(async () =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    Publish(me.Path);
                    try { await Task.Delay(750, cancel.Token); }
                    catch (OperationCanceledException) { break; }
                }
            });

            try
            {
                RunArmy(members, me, Bot.Config.Get<SupportClassChoice>("SupportClass"),
                    actionDelay, Bot.Config.Get<bool>("EnhanceGear"));
            }
            finally
            {
                if (_state != "DONE")
                    _state = "STOPPED";
                cancel.Cancel();
                publisher.GetAwaiter().GetResult();
                Publish(me.Path);
                Bot.Skills.Stop();
                Bot.Options.AggroMonsters = false;
            }
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void RunArmy(List<Member> members, Member me, SupportClassChoice classChoice, int actionDelay, bool enhanceGear)
    {
        DateTime lastWaitLog = DateTime.MinValue;
        while (!Bot.ShouldExit)
        {
            string[] missing = members.Where(m =>
            {
                MemberStatus? status = Read(m.Path);
                return status == null || (status.State != "READY" && status.State != "FARMING" && status.State != "DONE");
            }).Select(m => m.Name).ToArray();
            if (missing.Length == 0) break;
            if (DateTime.UtcNow - lastWaitLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Waiting for: {string.Join(", ", missing)}. Start this script on those accounts using the saved roster and the same server.");
                lastWaitLog = DateTime.UtcNow;
            }
            Bot.Sleep(1000);
        }
        if (Bot.ShouldExit) return;

        bool helping = false;
        string following = "";
        DateTime lastAction = DateTime.MinValue;
        DateTime lastError = DateTime.MinValue;
        var vdk = new CoreArmyVerusDoomKnight();

        vdk.ReportStep = (step, done, item) =>
        {
            _stepIndex = step;
            _stepDone = done;
            _currentItem = item;
            Publish(me.Path);
        };
        vdk.WaitStepBarrier = (questId, step, item) => WaitStepBarrier(members, me, questId, step, item);

        while (!Bot.ShouldExit)
        {
            var statuses = members.ToDictionary(m => m, m => Read(m.Path));
            Member? stopped = members.Where(m => m.TakesQuests)
                .FirstOrDefault(m => statuses[m]?.State == "STOPPED");
            if (stopped != null)
            {
                Core.Logger($"Quest account {stopped.Name} stopped before finishing VDK; ending this army run.");
                return;
            }
            if (members.Where(m => m.TakesQuests).All(m => statuses[m]?.State == "DONE"))
            {
                Core.Logger("Every checked account finished all VDK quests and reached class rank 10.");
                break;
            }

            if (members.Any(m => m.TakesQuests && statuses[m] == null))
            {
                Bot.Sleep(500);
                continue;
            }

            var activeTakers = members.Where(m => m.TakesQuests && statuses[m]!.State != "DONE").ToList();
            if (activeTakers.Count == 0) break;

            int minQuestIndex = activeTakers.Min(m => statuses[m]!.QuestIndex);

            // Accounts that take quests and share the lowest incomplete quest index farm together
            if (me.TakesQuests && _questIndex == minQuestIndex && _state != "DONE")
            {
                _state = "FARMING";
                Publish(me.Path);
                try
                {
                    if (helping)
                    {
                        Bot.Skills.Stop();
                        Bot.Combat.CancelTarget();
                        helping = false;
                        following = "";
                    }
                    int before = Volatile.Read(ref _questIndex);
                    Core.Logger(before < QuestIds.Length
                        ? $"Active army farm for VDK quest {QuestIds[before]}; accounts on this quest will farm together."
                        : "Active army turn to buy and rank Verus DoomKnight.");
                    if (enhanceGear) EnhanceCurrentGear();
                    vdk.GetClass(rankup: true, stopAfterQuestId: before < QuestIds.Length ? QuestIds[before] : 0);
                    int after = FirstIncompleteQuest();
                    Volatile.Write(ref _questIndex, after);
                    _stepIndex = 0;
                    _stepDone = false;
                    _currentItem = "";
                    _state = IsFinished(after) ? "DONE" : after <= before || Bot.ShouldExit ? "STOPPED" : "READY";
                }
                finally
                {
                    if (_state == "FARMING") _state = "STOPPED";
                    Publish(me.Path);
                }
                if (_state == "STOPPED")
                {
                    Core.Logger("This account made no quest or class progress; stopping it to avoid repeated farming.");
                    return;
                }
            }
            else
            {
                // Support mode: Follow the accounts actively farming minQuestIndex
                var leaderToFollow = activeTakers.FirstOrDefault(m => statuses[m]?.QuestIndex == minQuestIndex && statuses[m]?.State == "FARMING")
                // Support mode: Follow the accounts actively farming minQuestIndex.
                // Prioritize following the account that is still farming the item (!StepDone) and actively fighting:
                var leaderToFollow = activeTakers.FirstOrDefault(m => statuses[m]?.QuestIndex == minQuestIndex && statuses[m]?.State == "FARMING" && !statuses[m]!.StepDone && statuses[m]!.Fighting)
                                     ?? activeTakers.FirstOrDefault(m => statuses[m]?.QuestIndex == minQuestIndex && statuses[m]?.State == "FARMING" && !statuses[m]!.StepDone)
                                     ?? activeTakers.FirstOrDefault(m => statuses[m]?.QuestIndex == minQuestIndex && statuses[m]?.State == "FARMING")
                                     ?? activeTakers.FirstOrDefault(m => statuses[m]?.QuestIndex == minQuestIndex);
                if (leaderToFollow != null)
                {
                    if (!helping)
                    {
                        if (!PrepareSupport(classChoice, enhanceGear)) return;
                        helping = true;
                    }
                    if (following != leaderToFollow.Name)
                    {
                        following = leaderToFollow.Name;
                        Core.Logger($"Supporting {leaderToFollow.Name} on quest {(minQuestIndex < QuestIds.Length ? QuestIds[minQuestIndex] : 0)} until caught up.");
                    }
                    MemberStatus? target = statuses[leaderToFollow];
                    if (target != null)
                    {
                        try { FollowAndAssist(leaderToFollow.Name, target, actionDelay, ref lastAction); }
                        catch (Exception ex)
                        {
                            if (DateTime.UtcNow - lastError >= TimeSpan.FromSeconds(30))
                            {
                                Core.Logger($"Support follow failed temporarily: {ex.Message}");
                                lastError = DateTime.UtcNow;
                            }
                        }
                    }
                }
                Bot.Sleep(250);
            }
        }
    }

    private bool WaitStepBarrier(List<Member> members, Member me, int questId, int stepIndex, string itemName)
    {
        var otherQuestTakers = members.Where(m => m != me && m.TakesQuests).ToList();
        if (otherQuestTakers.Count == 0) return true;

        DateTime lastLog = DateTime.MinValue;

        while (!Bot.ShouldExit)
        {
            var statuses = members.ToDictionary(m => m, m => Read(m.Path));

            Member? stopped = otherQuestTakers.FirstOrDefault(p => statuses[p]?.State == "STOPPED");
            if (stopped != null)
            {
                Core.Logger($"Quest partner {stopped.Name} stopped; stopping army barrier.");
                return false;
            }

            bool allDone = true;
            string waitingOn = "";
            Member? partnerToAssist = null;

            foreach (var p in otherQuestTakers)
            {
                var s = statuses[p];
                if (s == null)
                {
                    allDone = false;
                    waitingOn += $"{p.Name} (syncing), ";
                    continue;
                }

                if (s.State == "DONE" || s.QuestIndex > _questIndex)
                    continue;

                if (s.QuestIndex == _questIndex)
                {
                    if (s.StepIndex > stepIndex || (s.StepIndex == stepIndex && s.StepDone))
                        continue;

                    partnerToAssist ??= p;
                }

                allDone = false;
                waitingOn += $"{p.Name}, ";
            }

            if (allDone)
            {
                Core.Logger($"Army check: All accounts have completed {itemName}; moving together.");
                break;
            }

            if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Have {itemName}. Assisting while waiting for: {waitingOn.TrimEnd(',', ' ')}");
                lastLog = DateTime.UtcNow;
            }

            // Travel to partner's map and cell to help them fight
            if (partnerToAssist != null && statuses[partnerToAssist] != null)
            {
                var targetStatus = statuses[partnerToAssist]!;
                if (targetStatus.Cell.Length > 0 && !targetStatus.Cell.Equals(Bot.Player.Cell, StringComparison.OrdinalIgnoreCase))
                if (targetStatus.Map.Length > 0 && !targetStatus.Map.Equals("whitemap", StringComparison.OrdinalIgnoreCase))
                {
                    Bot.Map.Jump(targetStatus.Cell, "Left", false);
                    if (!Bot.Map.Name.Equals(targetStatus.Map, StringComparison.OrdinalIgnoreCase))
                    {
                        Core.Join(targetStatus.Map);
                        Bot.Wait.ForMapLoad(targetStatus.Map);
                    }
                    if (targetStatus.Cell.Length > 0 && !targetStatus.Cell.Equals(Bot.Player.Cell, StringComparison.OrdinalIgnoreCase))
                    {
                        Bot.Map.Jump(targetStatus.Cell, "Left", false);
                    }
                }
            }

            if (Bot.Player.Alive && (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0))
            if (Bot.Player.Alive)
            {
                Bot.Combat.Attack("*");
                if (!Bot.Skills.TimerRunning)
                    Bot.Skills.StartAdvanced(Bot.Player.CurrentClass?.Name ?? "generic", false);

                if (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0)
                    Bot.Combat.Attack("*");
            }

            Publish(me.Path);
            Bot.Sleep(300);
        }

        return !Bot.ShouldExit;
    }

    private int FirstIncompleteQuest()
    {
        for (int index = 0; index < QuestIds.Length; index++)
            if (!Core.isCompletedBefore(QuestIds[index], log: false))
                return index;
        return QuestIds.Length;
    }

    private bool IsFinished(int questIndex) =>
        questIndex == QuestIds.Length &&
        Core.CheckInventory("Verus DoomKnight") &&
        Core.CheckClassRank(ClassName: "Verus DoomKnight") >= 10;

    private bool PrepareSupport(SupportClassChoice choice, bool enhanceGear)
    {
        string className = choice switch
        {
            SupportClassChoice.LordOfOrder => "Lord of Order",
            SupportClassChoice.CoreBotsFarm => Core.FarmClass,
            SupportClassChoice.CoreBotsSolo => Core.SoloClass,
            SupportClassChoice.CoreBotsDodge => Core.DodgeClass,
            SupportClassChoice.CoreBotsBoss => Core.BossClass,
            SupportClassChoice.CurrentClass => Bot.Player.CurrentClass?.Name ?? "",
            _ => "",
        };
        if (string.IsNullOrWhiteSpace(className) || className == "Generic" || className == "(Current)" ||
            !Core.CheckInventory(className))
        {
            Core.Logger($"Support class '{className}' is unavailable. Select an owned class in CoreBots or use CurrentClass.");
            return false;
        }
        if (!string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase))
            Core.Equip(className);
        Bot.Wait.ForTrue(() => string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase), 20);
        if (!string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase))
        {
            Core.Logger($"Could not equip support class {className}.");
            return false;
        }
        if (enhanceGear) EnhanceCurrentGear();
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Skills.StartAdvanced(className, false);
        return true;
    }

    private void EnhanceCurrentGear()
    {
        string className = Bot.Player.CurrentClass?.Name ?? "";
        if (className.Length == 0 || Core.CBOBool("DisableAutoEnhance", out bool disabled) && disabled)
            return;
        try { Adv.SmartEnhance(className); }
        catch (Exception ex) { Core.Logger($"SmartEnhance failed for {className}: {ex.Message}"); }
    }

    private void FollowAndAssist(string leader, MemberStatus status, int actionDelay, ref DateTime lastAction)
    {
        if (status.Map.Length == 0 || status.Map.Equals("whitemap", StringComparison.OrdinalIgnoreCase) ||
            DateTime.UtcNow - lastAction < TimeSpan.FromMilliseconds(actionDelay))
            return;

        if (!Bot.Map.Name.Equals(status.Map, StringComparison.OrdinalIgnoreCase))
        {
            lastAction = DateTime.UtcNow;
            Core.Join(status.Map);
            Bot.Wait.ForMapLoad(status.Map);
            return;
        }

        if (!Bot.Map.TryGetPlayer(leader, out PlayerInfo? player) || player == null)
        {
            lastAction = DateTime.UtcNow;
            Bot.Player.Goto(leader);
            return;
        }
        string cell = player.Cell ?? status.Cell;
        if (cell.Length > 0 && !cell.Equals(Bot.Player.Cell, StringComparison.OrdinalIgnoreCase))
        {
            lastAction = DateTime.UtcNow;
            Bot.Map.Jump(cell, player.Pad ?? "Left", false);
            return;
        }
        if (status.Fighting && Bot.Player.Alive && cell.Equals(Bot.Player.Cell, StringComparison.OrdinalIgnoreCase) &&
            (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0))
        {
            lastAction = DateTime.UtcNow;
            Bot.Combat.Attack("*");
        }
        else if (!status.Fighting && Bot.Player.HasTarget)
        else if (!status.Fighting && Bot.Player.HasTarget && Bot.Player.Target?.HP <= 0)
        {
            lastAction = DateTime.UtcNow;
            Bot.Combat.CancelTarget();
        }
    }

    private static void AssignSyncPaths(List<Member> members, int room, string folder)
    {
        // A username keeps the same file even if its slot changes on another client.
        Member[] ordered = members.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        string roster = room + "|" + string.Join("|", ordered.Select(m =>
            m.Name.ToLowerInvariant() + (m.TakesQuests ? ":quests" : ":support")));
        using SHA256 sha = SHA256.Create();
        string key = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(roster))).Replace("-", "").Substring(0, 16);
        for (int index = 0; index < ordered.Length; index++)
            ordered[index].Path = Path.Combine(folder, $"ArmyVDK_{key}_{index + 1}.sync");
    }

    private void Publish(string path)
    {
        lock (_statusWriteLock)
        {
            try
            {
                string temporary = path + ".tmp";
                File.WriteAllText(temporary, $"{_state}|{Volatile.Read(ref _questIndex)}|{Bot.Map.Name}|{Bot.Player.Cell}|{(Bot.Player.HasTarget || Bot.Player.InCombat ? 1 : 0)}|{_stepIndex}|{(_stepDone ? 1 : 0)}|{_currentItem}");
                // Readers see the complete old or new status, never an empty file
                // while another client is publishing its heartbeat.
                File.Move(temporary, path, true);
            }
            catch (IOException) { }
        }
    }

    private MemberStatus? Read(string path)
    {
        try
        {
            if (!File.Exists(path) || DateTime.UtcNow - File.GetLastWriteTimeUtc(path) > TimeSpan.FromSeconds(10))
                return null;
            string[] fields = File.ReadAllText(path).Split('|');
            if (fields.Length < 5 || !int.TryParse(fields[1], out int index))
                return null;
            int stepIndex = 0;
            bool stepDone = false;
            string itemName = "";
            if (fields.Length >= 8)
            {
                int.TryParse(fields[5], out stepIndex);
                stepDone = fields[6] == "1";
                itemName = fields[7];
            }
            return new MemberStatus
            {
                State = fields[0],
                QuestIndex = index,
                Map = fields[2],
                Cell = fields[3],
                Fighting = fields[4] == "1",
                StepIndex = stepIndex,
                StepDone = stepDone,
                ItemName = itemName,
            };
        }
        catch (IOException) { return null; }
    }
}
