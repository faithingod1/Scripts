/*
name: Four Account Ultra Boss Planner
description: Runs selected ultra bosses as one shared four-account queue.
tags: army, ultras, four accounts, boss planner
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraQueue.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/PrerequisitesChecker.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/1UltraEzrajalv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/2UltraWardenv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/3UltraEngineerv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/4UltraAvatarTyndariusv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/5ChampionDrakathv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/6UltraNulgathv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/7UltraDragov3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/8UltraDarkonv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/9UltraDagev3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/10UltraSpeakerv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/11UltraGramielv3.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyUltraBossPlanner
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _ultra;

    private const string BossSyncFile = "army_ultra_planner_bosses.sync";
    private const string ParticipantSyncFile = "army_ultra_planner_participants.sync";
    private static readonly string[] AllBosses =
    {
        "UltraEzrajal", "UltraWarden", "UltraEngineer", "UltraAvatarTyndarius",
        "ChampionDrakath", "UltraNulgath", "UltraDrago", "UltraDarkon",
        "UltraDage", "UltraSpeaker", "UltraGramiel"
    };

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyUltraBossPlanner";
    public List<IOption> Options = new()
    {
        new Option<bool>("CheckPrerequisites", "Check full prerequisites", "Run the broad Ultras-v3 readiness check on each account. Boss fights assign classes separately.", false),
        new Option<int>("MaxPasses", "Maximum passes", "How many times to retry unfinished bosses before stopping.", 2),
        new Option<bool>("UltraEzrajal", "Ultra Ezrajal", "Include Ultra Ezrajal.", true),
        new Option<bool>("UltraWarden", "Ultra Warden", "Include Ultra Warden.", true),
        new Option<bool>("UltraEngineer", "Ultra Engineer", "Include Ultra Engineer.", true),
        new Option<bool>("UltraAvatarTyndarius", "Ultra Avatar Tyndarius", "Include Ultra Avatar Tyndarius.", true),
        new Option<bool>("ChampionDrakath", "Champion Drakath", "Include Champion Drakath.", true),
        new Option<bool>("UltraNulgath", "Ultra Nulgath", "Include Ultra Nulgath.", true),
        new Option<bool>("UltraDrago", "Ultra Drago", "Include Ultra Drago.", true),
        new Option<bool>("UltraDarkon", "Ultra Darkon", "Include Ultra Darkon.", true),
        new Option<bool>("UltraDage", "Ultra Dage", "Include Ultra Dage.", true),
        new Option<bool>("UltraSpeaker", "Ultra Speaker", "Include Ultra Speaker.", true),
        new Option<bool>("UltraGramiel", "Ultra Gramiel", "Include Ultra Gramiel.", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(true);
        try
        {
            var selected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string boss in AllBosses)
                if (Bot.Config!.Get<bool>(boss))
                    selected.Add(boss);

            if (selected.Count == 0)
            {
                C.Logger("[ArmyUltraBossPlanner] Select at least one boss.", "Warning");
                return;
            }

            int maxPasses = Bot.Config!.Get<int>("MaxPasses");
            if (maxPasses < 1 || maxPasses > 10)
            {
                C.Logger("[ArmyUltraBossPlanner] Maximum passes must be between 1 and 10.", "Error");
                return;
            }

            C.Logger($"[ArmyUltraBossPlanner] Selected: {string.Join(", ", selected)}");
            Engine.Boot();
            if (Bot.Config.Get<bool>("CheckPrerequisites") && !new PrerequisitesChecker().Check())
                return;

            for (int pass = 1; pass <= maxPasses && !Bot.ShouldExit; pass++)
            {
                var pending = GetPendingBosses(selected);
                if (pending.Count == 0)
                {
                    C.Logger("[ArmyUltraBossPlanner] All selected bosses complete for the army.");
                    return;
                }

                C.Logger($"[ArmyUltraBossPlanner] Pass {pass}/{maxPasses}: {string.Join(", ", pending)}");
                foreach (string boss in pending)
                {
                    if (Bot.ShouldExit)
                        return;
                    RunBoss(boss);
                }
            }

            if (!Bot.ShouldExit)
            {
                var remaining = GetPendingBosses(selected);
                C.Logger(remaining.Count == 0
                    ? "[ArmyUltraBossPlanner] All selected bosses complete for the army."
                    : $"[ArmyUltraBossPlanner] Still pending after {maxPasses} pass(es): {string.Join(", ", remaining)}",
                    remaining.Count == 0 ? "Info" : "Warning");
            }
        }
        catch (TimeoutException ex)
        {
            C.Logger($"[ArmyUltraBossPlanner] {ex.Message}", "Error");
        }
        finally
        {
            Engine.DisableSkills();
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    private List<string> GetPendingBosses(HashSet<string> selected)
    {
        // Publish every boss key so different selections still produce the same
        // union of unfinished bosses on all four accounts.
        var pending = UltraQueue.GetSharedBossQueue(
            Ultra, Bot, C, AllBosses, BossSyncFile, ParticipantSyncFile,
            boss => !selected.Contains(boss) || IsBossComplete(boss)
        ).ToList();

        RequireFourFreshEntries(ParticipantSyncFile);
        RequireFourFreshEntries(BossSyncFile);
        return pending;
    }

    private void RequireFourFreshEntries(string file)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int count = Ultra.ReadLines(Ultra.ResolveSyncPath(file))
            .Select(line => line.Split(':'))
            .Count(parts => parts.Length >= 3
                && long.TryParse(parts[parts.Length - 1], out long timestamp)
                && now - timestamp <= 60);
        if (count < 4)
            throw new TimeoutException($"Only {count}/4 accounts reported in {file}. Start the planner on all four accounts.");
    }

    private bool IsBossComplete(string boss)
    {
        int questId = boss switch
        {
            "UltraEzrajal" => 8152,
            "UltraWarden" => 8153,
            "UltraEngineer" => 8154,
            "UltraAvatarTyndarius" => 8245,
            "ChampionDrakath" => 8300,
            "UltraNulgath" => 8692,
            "UltraDrago" => 8397,
            "UltraDarkon" => 8746,
            "UltraDage" => 8547,
            "UltraSpeaker" => 9173,
            "UltraGramiel" => 10301,
            _ => 0
        };
        return questId > 0 && UltraGeneral.IsQuestComplete(Bot, questId);
    }

    private void RunBoss(string boss)
    {
        switch (boss)
        {
            case "UltraEzrajal": new UltraEzrajalv3().RunBoss(); break;
            case "UltraWarden": new UltraWardenv3().RunBoss(); break;
            case "UltraEngineer": new UltraEngineerv3().RunBoss(); break;
            case "UltraAvatarTyndarius": new UltraAvatarTyndariusv3().RunBoss(); break;
            case "ChampionDrakath": new ChampionDrakathv3().RunBoss(); break;
            case "UltraNulgath": new UltraNulgathv3().RunBoss(); break;
            case "UltraDrago": new UltraDragov3().RunBoss(); break;
            case "UltraDarkon": new UltraDarkonv3().RunBoss(); break;
            case "UltraDage": new UltraDagev3().RunBoss(); break;
            case "UltraSpeaker": new UltraSpeakerv3().RunBoss(); break;
            case "UltraGramiel": new UltraGramielv3().RunBoss(); break;
        }
    }
}
