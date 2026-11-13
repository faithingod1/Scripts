/*
name: Army Farmer Joe (1 Farmer + up to 3 Supporters)
description: Runs the full Farmer Joe (0FarmerJoeDoAll) progression on 1 low-level account while up to 3 high-level accounts follow, protect, and carry in combat.
tags: army, farmerjoe, carry, zero to hero, support, leveling
*/

#region includes
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Dailies/0AllDailies.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Enhancement/InventoryEnhancer.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/GearOfAwe/CapeOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Hollowborn/MergeShops/ShadowrealmMerge.cs
//cs_include Scripts/Dailies/MineCrafting.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Nation/AFDL/EnoughDOOMforanArchfiend.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Other/Classes/DragonShinobi.cs
//cs_include Scripts/Other/Classes/REP-based/EternalInversionist.cs
//cs_include Scripts/Other/Classes/REP-based/GlacialBerserker.cs
//cs_include Scripts/Other/Classes/REP-based/Shaman.cs
//cs_include Scripts/Other/Classes/REP-based/StoneCrusher.cs
//cs_include Scripts/Other/Classes/ScarletSorceress.cs
//cs_include Scripts/Other/Classes/BloodSorceress.cs
//cs_include Scripts/Other/FreeBoosts/FreeBoostsQuest(10mns)[Rng].cs
//cs_include Scripts/Other/Weapons/BurningBladeOfAbezeth.cs
//cs_include Scripts/Other/Weapons/BurningBlade.cs
//cs_include Scripts/Other/Weapons/DualChainSawKatanas.cs
//cs_include Scripts/Other/Weapons/EnchantedVictoryBladeWeapons.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/Tutorial.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Other/FarmJoeKits/CoreFarmerJoe.cs
//cs_include Scripts/Tools/BankAllItems.cs
//cs_include Scripts/Story/Friendship.cs
//cs_include Scripts/Other/Classes/REP-based/MasterRanger.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Other/MergeShops/SynderesMerge.cs
//cs_include Scripts/Other/Classes/REP-based/DarkbloodStormKing.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Story/AgeofRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/YulgarsUndineMerge.cs
//cs_include Scripts/Hollowborn/MergeShops/DawnFortressMerge.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Other/MergeShops/CelestialChampMerge.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Other/Classes/Daily-Classes/BlazeBinder.cs
//cs_include Scripts/Nation/Various/Archfiend.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Dailies/Cryomancer.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/ExaltedApotheosisPreReqs.cs
//cs_include Scripts/Story/Mazumi.cs
//cs_include Scripts/Other/Classes/Dragonslayer.cs
//cs_include Scripts/Other/Classes/FrostSpiritReaver.cs
//cs_include Scripts/Seasonal/Frostvale/NorthlandsMonk.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinTreasuryMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs
//cs_include Scripts/Other/Classes/KingsEcho.cs
#endregion includes

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Players;
using Skua.Core.Options;
using Skua.Core.Utils;

public class ArmyFarmerJoe
{
    private sealed class Status
    {
        public string State = "";
        public string Map = "";
        public string Cell = "";
        public bool Fighting;
        public int RoomId;
        public DateTime PublishedUtc;
    }

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarmerJoe CFJ
    {
        get => _CFJ ??= new CoreFarmerJoe();
        set => _CFJ = value;
    }
    private static CoreFarmerJoe? _CFJ;

    private volatile string _state = "STARTING";
    private readonly object _statusWriteLock = new();
    private string _supportClass = "generic";
    private DateTime _farmerMissingSince = DateTime.MinValue;
    private DateTime _lastRoomJoin = DateTime.MinValue;
    private DateTime _startedUtc;
    private DateTime _lastPublishError = DateTime.MinValue;
    private DateTime _lastMovementWarning = DateTime.MinValue;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyFarmerJoe";
    public List<IOption> Options = new List<IOption>
    {
        new Option<string>("FarmerAccount", "Farmer account (Low-level)", "Account that runs the normal Farmer Joe DoAll progression.", ""),
        new Option<string>("SupportAccount1", "Support account 1 (High-level)", "First combat supporter. Keeps its equipped class and enhancements.", ""),
        new Option<string>("SupportAccount2", "Support account 2 (High-level)", "Second combat supporter (leave blank if unused).", ""),
        new Option<string>("SupportAccount3", "Support account 3 (High-level)", "Third combat supporter (leave blank if unused).", ""),
        new Option<int>("PrivateRoom", "Private room number", "Use the same roster, server, and private room on all clients (1000 to 999999).", 123456),
    }.Concat(CFJ.Options).ToList();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        try
        {
            Bot.Config?.Load();
            _state = "STARTING";
            // Core.SetOptions starts skills; wait for the team before combat.
            StopCombat();
            _startedUtc = DateTime.UtcNow;
            _farmerMissingSince = DateTime.MinValue;
            _lastRoomJoin = DateTime.MinValue;
            _lastPublishError = DateTime.MinValue;
            _lastMovementWarning = DateTime.MinValue;

            string farmer = Bot.Config?.Get<string>("FarmerAccount")?.Trim() ?? "";
            var supporters = new List<string>();
            for (int slot = 1; slot <= 3; slot++)
            {
                string sup = Bot.Config?.Get<string>($"SupportAccount{slot}")?.Trim() ?? "";
                if (sup.Length > 0 && !supporters.Contains(sup, StringComparer.OrdinalIgnoreCase))
                    supporters.Add(sup);
            }

            int room = Bot.Config?.Get<int>("PrivateRoom") ?? 123456;

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

            string farmerSyncPath = Path.Combine(folder, $"ArmyFJ_{rosterKey}_farmer.sync");
            string mySyncPath = isFarmer
                ? farmerSyncPath
                : Path.Combine(folder, $"ArmyFJ_{rosterKey}_{Core.Username().ToLowerInvariant()}.sync");

            Core.Logger($"Army Farmer Joe: Running as {(isFarmer ? "FARMER" : "SUPPORTER")}. Farmer [{farmer}], Supporters [{string.Join(", ", supporters)}].");

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
                    RunFarmer(farmerSyncPath, supporters);
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
                StopCombat();
            }
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void RunFarmer(string myPath, List<string> supporters)
    {
        // Detect the actual room roster, without depending on supporter files.
        // If already assembled, keep the group in its current map.
        if (!supporters.Any(IsPlayerInRoom))
        {
            while (!Bot.ShouldExit && !TryExitCombat())
                Bot.Sleep(200);
            if (Bot.ShouldExit) return;
            Bot.Wait.ForActionCooldown(GameActions.Transfer);
            Bot.Map.JoinPacket($"battleon-{Core.PrivateRoomNumber}", "Enter", "Spawn");
            Bot.Wait.ForMapLoad("battleon");
        }

        _state = "READY";
        Publish(myPath);
        DateTime lastLog = DateTime.MinValue;
        while (!Bot.ShouldExit && !supporters.Any(IsPlayerInRoom))
        {
            if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
            {
                Core.Logger($"Farmer: Waiting in [{Bot.Map.Name}] for [{string.Join(", ", supporters)}]. " +
                    $"Players detected: [{string.Join(", ", Bot.Map.PlayerNames?.ToArray() ?? Array.Empty<string>())}].");
                lastLog = DateTime.UtcNow;
            }
            Bot.Sleep(200);
        }

        if (Bot.ShouldExit) return;

        _state = "FARMING";
        Publish(myPath);
        Core.Logger("Supporter detected! Starting the normal Farmer Joe DoAll progression.");
        try
        {
            // The same entry point used by Other/FarmJoeKits/0FarmerJoeDoAll.cs.
            // Only the farmer runs progression, quests, gear upgrades, and pickups.
            CFJ.DoAll();
            _state = Bot.ShouldExit ? "STOPPED" : "DONE";
            Core.Logger(_state == "DONE"
                ? "Farmer Joe DoAll finished. Supporters can stop."
                : "Farmer Joe was stopped before completion.");
        }
        catch (Exception ex)
        {
            _state = "STOPPED";
            Core.Logger($"Farmer encountered an error: {ex.Message}");
        }
        finally
        {
            Publish(myPath);
        }
    }

    private void RunSupporter(string farmerName, string farmerSyncPath)
    {
        if (!PrepareSupport())
            return;

        _state = "READY";
        bool metFarmer = false;
        bool sawActiveFarmer = false;
        DateTime lastLog = DateTime.MinValue;
        Core.Logger($"Supporter ready. Waiting to meet Farmer [{farmerName}]...");

        while (!Bot.ShouldExit)
        {
            Status? farmerStatus = Read(farmerSyncPath);
            if (farmerStatus == null)
            {
                StopCombat();
                if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
                {
                    Core.Logger($"Waiting for a fresh farmer heartbeat [{farmerSyncPath}]. Check that all clients use the same roster and room.");
                    lastLog = DateTime.UtcNow;
                }
                Bot.Sleep(200);
                continue;
            }

            if (farmerStatus.State == "DONE" || farmerStatus.State == "STOPPED")
            {
                StopCombat();
                // Ignore terminal files from an earlier run when starting
                // supporters before the farmer. Honor this run's terminal state
                // even if the farmer finishes before the first meeting.
                if (sawActiveFarmer || farmerStatus.PublishedUtc >= _startedUtc)
                {
                    _state = farmerStatus.State;
                    Core.Logger($"Farmer [{farmerName}] reported {_state}. Supporter halting.");
                    return;
                }
            }
            else if (farmerStatus.State == "READY" || farmerStatus.State == "FARMING")
            {
                sawActiveFarmer = true;
                try
                {
                    FollowAndAssist(farmerName, farmerStatus, metFarmer && farmerStatus.State == "FARMING");
                    if (!metFarmer && IsPlayerInRoom(farmerName) &&
                        CleanMap(Bot.Map.Name) == farmerStatus.Map &&
                        (farmerStatus.RoomId <= 0 || Bot.Map.RoomID == farmerStatus.RoomId))
                    {
                        metFarmer = true;
                        _state = "SUPPORTING";
                        Core.Logger($"Farmer [{farmerName}] found. Following and carrying in combat.");
                    }
                }
                catch (Exception ex)
                {
                    StopCombat();
                    if (DateTime.UtcNow - lastLog >= TimeSpan.FromSeconds(10))
                    {
                        Core.Logger($"Support assist retry: {ex.Message}");
                        lastLog = DateTime.UtcNow;
                    }
                }
            }
            else
                StopCombat();

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
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;
        if (Bot.Skills.TimerRunning)
            Bot.Skills.Stop();
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();
    }

    private bool TryExitCombat()
    {
        StopCombat();
        if (Bot.ShouldExit || !Bot.Player.Loaded || !Bot.Player.Alive)
            return false;

        // Deselecting a target does not clear the game's combat state.
        // Skua.Exit cancels auto-attack, jumps in the current cell, and waits
        // for combat to end. Retry on a later poll if the server keeps us locked.
        if (Bot.Player.InCombat)
            Bot.Combat.Exit();

        if (Bot.Player.InCombat && DateTime.UtcNow - _lastMovementWarning >= TimeSpan.FromSeconds(10))
        {
            _lastMovementWarning = DateTime.UtcNow;
            Core.Logger($"Movement waiting for combat exit in [{Bot.Map.Name}/{Bot.Player.Cell}], room ID {Bot.Map.RoomID}. Retrying combat exit.");
        }
        return !Bot.ShouldExit && Bot.Player.Loaded && Bot.Player.Alive && !Bot.Player.InCombat;
    }

    private void TransferToFarmer(string targetMap, int farmerRoomId)
    {
        StopCombat();
        if (DateTime.UtcNow - _lastRoomJoin < TimeSpan.FromSeconds(3) || !TryExitCombat())
            return;
        if (!Bot.Wait.ForActionCooldown(GameActions.Transfer, 2) || Bot.ShouldExit)
            return;

        _lastRoomJoin = DateTime.UtcNow;
        Core.Logger($"Following farmer: [{Bot.Map.Name}], room ID {Bot.Map.RoomID} -> [{targetMap}-{Core.PrivateRoomNumber}], farmer room ID {farmerRoomId}.");
        // Map.Join's internal loop skips transfers when the base map name
        // already matches, even with ignoreCheck. JoinPacket forces the transfer.
        Bot.Map.JoinPacket($"{targetMap}-{Core.PrivateRoomNumber}", "Enter", "Spawn");
        Bot.Wait.ForMapLoad(targetMap, 2);
        // Map load alone cannot verify a room instance. The next poll checks
        // both RoomID and the farmer's presence before allowing any combat.
    }

    private void FollowAndAssist(string leader, Status status, bool allowCombat)
    {
        string targetMap = CleanMap(status.Map);
        if (!Bot.Player.Loaded || !Bot.Player.Alive || targetMap.Length == 0 || targetMap == "whitemap")
        {
            StopCombat();
            return;
        }

        if (CleanMap(Bot.Map.Name) != targetMap)
        {
            TransferToFarmer(targetMap, status.RoomId);
            return;
        }

        bool wrongRoom = status.RoomId > 0 && Bot.Map.RoomID != status.RoomId;
        if (wrongRoom || !IsPlayerInRoom(leader))
        {
            StopCombat();
            if (_farmerMissingSince == DateTime.MinValue)
                _farmerMissingSince = DateTime.UtcNow;
            if (wrongRoom || DateTime.UtcNow - _farmerMissingSince >= TimeSpan.FromSeconds(2))
                TransferToFarmer(targetMap, status.RoomId);
            return;
        }
        _farmerMissingSince = DateTime.MinValue;

        PlayerInfo? player = Bot.Map.Players?.FirstOrDefault(p =>
            string.Equals(p.Name?.Trim(), leader.Trim(), StringComparison.OrdinalIgnoreCase));
        string targetCell = !string.IsNullOrWhiteSpace(player?.Cell)
            ? player.Cell.Trim()
            : status.Cell.Trim();

        if (targetCell.Length == 0)
        {
            StopCombat();
            return;
        }

        if (!string.Equals(Bot.Player.Cell?.Trim(), targetCell, StringComparison.OrdinalIgnoreCase))
        {
            if (!TryExitCombat())
                return;

            Bot.Map.Jump(targetCell, string.IsNullOrWhiteSpace(player?.Pad) ? "Left" : player.Pad, false);
            Bot.Wait.ForCellChange(targetCell);
            return;
        }

        if (!allowCombat)
        {
            StopCombat();
            return;
        }

        if (!Bot.Skills.TimerRunning)
            Bot.Skills.StartAdvanced(_supportClass, false);

        if ((!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0) &&
            (status.Fighting || Bot.Player.InCombat || Bot.Monsters.CurrentMonsters?.Any(m => m.Alive) == true))
            Bot.Combat.Attack("*");
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
                int roomId = Bot.Map?.RoomID ?? 0;
                string temporary = path + ".tmp";
                File.WriteAllText(temporary, $"{_state}|{map}|{cell}|{(fighting ? 1 : 0)}|{roomId}");
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
                RoomId = fields.Length >= 5 && int.TryParse(fields[4], out int roomId) ? roomId : 0,
                PublishedUtc = publishedUtc,
            };
        }
        catch (Exception) { return null; }
    }
}

