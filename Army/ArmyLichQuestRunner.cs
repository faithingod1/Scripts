/*
name: Army Lich Quest Runner
description: Handles individual quest stages and requirements for the Lich class with synchronization hooks for army farming.
tags: army, lich, class, runner, necronomicon
*/

#region Includes Area
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Seasonal/Friday13th/MergeShops/ColossalWaresMerge.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Evil/VordredsArmor.cs
//cs_include Scripts/Farm/REP/GrimskullTrollingRep.cs
//cs_include Scripts/Prototypes/MoreSkullsBoss.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Other/MergeShops/InfernalArenaMerge.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/DoomVault.cs
//cs_include Scripts/Story/DoomVaultB.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalArena.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Prototypes/Grimgaol.cs
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ArmyLichQuestRunner
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    private static Grimgaol GrimGaolRun
    {
        get => _GrimGaolRun ??= new Grimgaol();
        set => _GrimGaolRun = value;
    }
    private static Grimgaol _GrimGaolRun;

    public Action<int, int, string>? RequirementCompleted { get; set; }
    public Action<string, Action>? SoloRequirement { get; set; }

    private void RunSolo(string map, Action farm)
    {
        if (SoloRequirement != null)
            SoloRequirement(map, farm);
        else
            farm();
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetLich(rankUpClass: true);
        Core.SetOptions(false);
    }

    public static int FirstIncompleteStage(bool rankUpClass = true)
    {
        // Total stages: 5 (0: Prereqs, 1: Klaatu, 2: Nicto, 3: Verata, 4: Lich Class)
        // Stage 5 indicates all done and class maxed
        if (CoreBots.Instance.CheckInventory(94824, toInv: false) || CoreBots.Instance.CheckInventory("Lich", toInv: false))
        {
            if (rankUpClass && CoreBots.Instance.CheckClassRank(false, "Lich") < 10)
                return 4;
            return 5;
        }

        bool hasKlaatu = CoreBots.Instance.CheckInventory("Klaatu's Necronomicon", toInv: false);
        bool hasNicto = CoreBots.Instance.CheckInventory("Nicto's Necronomicon", toInv: false);
        bool hasVerata = CoreBots.Instance.CheckInventory("Verata's Necronomicon", toInv: false);

        if (hasKlaatu && hasNicto && hasVerata)
            return 4;

        bool prereqsDone = IScriptInterface.Instance.Player.Level >= 80 &&
                           Farm.FactionRank("Doomwood") >= 10 &&
                           Farm.FactionRank("Evil") >= 10;

        if (!prereqsDone)
            return 0;

        if (!hasKlaatu)
            return 1;

        if (!hasNicto)
            return 2;

        if (!hasVerata)
            return 3;

        return 4;
    }

    public void GetLich(bool rankUpClass = true, int stopAfterStage = -1)
    {
        string soloClass = Core.SoloClass;
        string farmClass = Core.FarmClass;
        string dodgeClass = Core.DodgeClass;
        string bossClass = Core.BossClass;
        try
        {
            // Generic makes EquipClass start the worn class's skills without changing gear.
            // Apply to every role so nested farming helpers preserve the team composition too.
            Core.SoloClass = Core.FarmClass = Core.DodgeClass = Core.BossClass = "Generic";
            Core.Logger($"Keeping equipped class: {Bot.Player.CurrentClass?.Name}. Lich auto-ranking is disabled.");
            RunLichStages(rankUpClass: false, stopAfterStage: stopAfterStage);
        }
        finally
        {
            Core.SoloClass = soloClass;
            Core.FarmClass = farmClass;
            Core.DodgeClass = dodgeClass;
            Core.BossClass = bossClass;
        }
    }

    private void RunLichStages(bool rankUpClass, int stopAfterStage)
    {
        if (FirstIncompleteStage(rankUpClass) == 5)
            return;

        // Stage 0: Prerequisites (Level 80, Doomwood REP 10, Evil REP 10, Gold)
        DoPrerequisites(stopAfterStage);
        if (stopAfterStage == 0) return;

        // Stage 1: Klaatu's Necronomicon (Quest 10338)
        DoKlaatuNecronomicon(stopAfterStage);
        if (stopAfterStage == 1) return;

        // Stage 2: Nicto's Necronomicon (Shop 2591 in /moreskulls)
        DoNictosNecronomicon(stopAfterStage);
        if (stopAfterStage == 2) return;

        // Stage 3: Verata's Necronomicon (Shop 2362 in /gaolcell)
        DoVeratasNecronomicon(stopAfterStage);
        if (stopAfterStage == 3) return;

        // Stage 4: Lich Class Quest (Quest 10339) & Rank Up
        DoLichClassQuest(rankUpClass);
    }

    private void DoPrerequisites(int stopAfterStage)
    {
        if (Bot.Player.Level >= 80 && Farm.FactionRank("Doomwood") >= 10 && Farm.FactionRank("Evil") >= 10)
            return;

        Core.Logger("Farming prerequisites for Lich class...");

        // Level 80 requirement
        if (Bot.Player.Level < 80)
        {
            Core.Logger("Level 80 required. Leveling up...");
            Farm.Experience(80);
        }
        RequirementCompleted?.Invoke(0, 1, "Level 80");

        // Doomwood Rank 10
        if (Farm.FactionRank("Doomwood") < 10)
        {
            Core.Logger("Doomwood Rank 10 required. Farming Doomwood Rep...");
            Farm.DoomWoodREP();
        }
        RequirementCompleted?.Invoke(0, 2, "Doomwood Rank 10");

        // Evil Rank 10
        if (Farm.FactionRank("Evil") < 10)
        {
            Core.Logger("Evil Rank 10 required. Farming Evil Rep...");
            Farm.EvilREP();
        }
        RequirementCompleted?.Invoke(0, 3, "Evil Rank 10");

        // Gold for Vouchers and Shadow Lich (6.625M)
        if (Bot.Player.Gold < 6625000 && !Core.CheckInventory("Gold Voucher 100k", 66))
        {
            Core.Logger("Farming gold for Vouchers (6,625,000)...");
            Farm.Gold(6625000);
        }
        RequirementCompleted?.Invoke(0, 4, "Gold (6,625,000)");
    }

    private void DoKlaatuNecronomicon(int stopAfterStage)
    {
        if (Core.CheckInventory("Klaatu's Necronomicon", toInv: false))
            return;

        Core.Logger("Starting Klaatu's Necronomicon (Quest 10338)...");
        if (!Core.isCompletedBefore(10287))
        {
            LichWarPrereqs();
            FarmPristineSkulls(0, storyOnly: true);
        }
        if (Bot.ShouldExit) return;
        AcceptQuestChecked(10338);

        // Step 1: Bone Dust x6666 from Imperial Noxus
        if (!Core.CheckInventory("Bone Dust", 6666))
        {
            // Match ArmySpiritOrbs: initialize the encounter's quest state before loading r5.
            // Reload an already-open map so the boss room uses the refreshed state.
            Core.Logger("Preparing Imperial Noxus encounter in /necroproject...");
            Bot.Quests.UpdateQuest(9901);
            if (Bot.Map.Name.Equals("necroproject", StringComparison.OrdinalIgnoreCase))
                Core.Join("battleon");
            Core.Join("necroproject");
            Core.Jump("r5", "Left");
            Bot.Sleep(1500);
            Core.AddDrop("Bone Dust");
            Core.EquipClass(ClassType.Solo);
            Core.FarmingLogger("Bone Dust", 6666);
            while (!Bot.ShouldExit && !Core.CheckInventory("Bone Dust", 6666))
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }

                var noxus = Core.FindMonster("necroproject", "Imperial Noxus");
                if (noxus == null)
                {
                    Bot.Sleep(1000);
                    continue;
                }

                if (Bot.Player.Cell != noxus.Cell)
                {
                    Core.Jump(noxus.Cell, "Left");
                    Bot.Wait.ForCellChange(noxus.Cell);
                }

                if (Bot.ShouldExit) break;
                if (!Bot.Skills.TimerRunning)
                    Core.EquipClass(ClassType.Solo);

                // Retry explicitly: a stale HasTarget flag must not block combat after a kill.
                if (Bot.Player.Alive && noxus.HP > 0)
                    Bot.Combat.Attack(noxus.MapID);

                Bot.Sleep(500);
            }
        }
        RequirementCompleted?.Invoke(1, 1, "Bone Dust (6666)");

        // Step 2: Shadow Lich armor
        if (!Core.CheckInventory("Shadow Lich"))
        {
            Farm.EvilREP();
            Adv.BuyItem("Shadowfall", 89, "Shadow Lich");
        }
        RequirementCompleted?.Invoke(1, 2, "Shadow Lich");

        // Step 3: Lich of the Stone armor
        if (!Core.CheckInventory("Lich Of The Stone"))
        {
            Core.AddDrop("Lich Of The Stone");
            Core.EquipClass(ClassType.Solo);
            while (!Bot.ShouldExit && !Core.CheckInventory("Lich Of The Stone"))
            {
                Core.HuntMonster("DarkoviaForest", "Lich of the Stone", "Lich Of The Stone", isTemp: false);
                Bot.Wait.ForPickup("Lich Of The Stone");
            }
        }
        RequirementCompleted?.Invoke(1, 3, "Lich of the Stone");

        // Step 4: Sapphire Orb x113
        if (!Core.CheckInventory("Sapphire Orb", 113))
        {
            Core.AddDrop("Sapphire Orb");
            Core.EquipClass(ClassType.Dodge);
            Core.FarmingLogger("Sapphire Orb", 113);
            while (!Bot.ShouldExit && !Core.CheckInventory("Sapphire Orb", 113))
            {
                Core.HuntMonster("frozenlair", "Legion Lich Lord", "Sapphire Orb", 113, isTemp: false);
                Bot.Wait.ForPickup("Sapphire Orb");
            }
        }
        RequirementCompleted?.Invoke(1, 4, "Sapphire Orb (113)");

        // Step 5: Map item in necroproject
        Core.GetMapItem(14740, 1, "necroproject");
        RequirementCompleted?.Invoke(1, 5, "Necroproject Map Item");

        // Step 6: Complete Quest 10338
        Core.AddDrop("Klaatu's Necronomicon");
        CompleteQuestChecked(10338);
        Bot.Wait.ForPickup("Klaatu's Necronomicon");
        RequirementCompleted?.Invoke(1, 6, "Klaatu's Necronomicon Completed");
    }

    private void DoNictosNecronomicon(int stopAfterStage)
    {
        if (Core.CheckInventory("Nicto's Necronomicon", toInv: false))
            return;

        Core.Logger("Starting Nicto's Necronomicon (Shop 2591)...");

        // Step 1: Gold Voucher 100k x66
        if (!Core.CheckInventory("Gold Voucher 100k", 66))
        {
            if (Bot.Player.Gold < 6600000)
                Farm.Gold(6600000);
            Adv.BuyItem("spellcraft", 693, "Gold Voucher 100k", 66);
        }
        RequirementCompleted?.Invoke(2, 1, "Gold Voucher 100k (66)");

        // Step 2: LichWar progression (unlocks quest 10287 / MoreSkulls boss)
        LichWarPrereqs();
        RequirementCompleted?.Invoke(2, 2, "LichWar Prerequisites");

        // Step 3: Grimskull's Favor x776
        if (!Core.CheckInventory("Grimskull's Favor", 776))
        {
            Core.AddDrop("Grimskull's Favor");
            Core.EquipClass(ClassType.Farm);
            Core.FarmingLogger("Grimskull's Favor", 776);
            Core.RegisterQuests(10282, 10283);
            try
            {
                Core.HuntMonster("lichwar", "Noxus Warrior", "Grimskull's Favor", 776, isTemp: false);
            }
            finally { Core.CancelRegisteredQuests(); }
        }
        RequirementCompleted?.Invoke(2, 3, "Grimskull's Favor (776)");

        // Step 4: Noxus' Favor x776
        if (!Core.CheckInventory("Noxus' Favor", 776))
        {
            Core.AddDrop("Noxus' Favor");
            Core.EquipClass(ClassType.Farm);
            Core.FarmingLogger("Noxus' Favor", 776);
            Core.RegisterQuests(10278, 10279);
            try
            {
                Core.HuntMonster("lichwar", "Grim Soldier", "Noxus' Favor", 776, isTemp: false);
            }
            finally { Core.CancelRegisteredQuests(); }
        }
        RequirementCompleted?.Invoke(2, 4, "Noxus' Favor (776)");

        // Step 5: Pristine Skull x66 (MoreSkulls World Boss)
        if (!Core.CheckInventory("Pristine Skull", 66))
        {
            FarmPristineSkulls(66);
        }
        RequirementCompleted?.Invoke(2, 5, "Pristine Skull (66)");

        // Step 6: Buy Nicto's Necronomicon from shop 2591
        Core.AddDrop("Nicto's Necronomicon");
        Adv.BuyItem("moreskulls", 2591, "Nicto's Necronomicon");
        Bot.Wait.ForPickup("Nicto's Necronomicon");
        RequirementCompleted?.Invoke(2, 6, "Nicto's Necronomicon Purchased");
    }

    private void DoVeratasNecronomicon(int stopAfterStage)
    {
        if (Core.CheckInventory("Verata's Necronomicon", toInv: false))
            return;

        Core.Logger("Starting Verata's Necronomicon (Shop 2362)...");

        // Step 1: Grimskull Trolling Rep Rank 10
        if (Farm.FactionRank("Grimskull Trolling") < 10)
        {
            throw new InvalidOperationException("Grimskull Trolling Rank 10 is required. " +
                "The Grimgaol helper switches classes and gear, so it cannot run while keeping your equipped class. " +
                "Complete the reputation separately, then restart ArmyLich.");
        }
        RequirementCompleted?.Invoke(3, 1, "Grimskull Trolling Rank 10");

        // Step 2: Buy Verata's Necronomicon from gaolcell shop 2362
        Core.AddDrop("Verata's Necronomicon");
        RunSolo("gaolcell", () => Core.BuyItem("gaolcell", 2362, 94051, shopItemID: 60505));
        Bot.Wait.ForPickup("Verata's Necronomicon");
        RequirementCompleted?.Invoke(3, 2, "Verata's Necronomicon Purchased");
    }

    private void DoLichClassQuest(bool rankUpClass)
    {
        if (!Core.CheckInventory(94824, toInv: false) && !Core.CheckInventory("Lich", toInv: false))
        {
            Core.Logger("Completing Lich Class Quest (10339)...");
            Core.AddDrop(94824);
            Core.AddDrop("Lich");

            // Ensure all 3 books are in player inventory
            if (!Core.CheckInventory(94050, toInv: true) ||
                !Core.CheckInventory(94052, toInv: true) ||
                !Core.CheckInventory(94051, toInv: true))
                throw new InvalidOperationException("Cannot turn in Lich Class: one or more Necronomicons are missing.");

            RequirementCompleted?.Invoke(4, 1, "3 Necronomicons Verified");

            if (!Core.isCompletedBefore(10287))
            {
                LichWarPrereqs();
                FarmPristineSkulls(0, storyOnly: true);
            }
            AcceptQuestChecked(10339);
            CompleteQuestChecked(10339);
            Bot.Wait.ForPickup(94824);
            RequirementCompleted?.Invoke(4, 2, "Lich Class Unlocked");
        }

        if (rankUpClass && Core.CheckClassRank(false, "Lich") < 10)
        {
            Core.Logger("Ranking up Lich Class to Rank 10...");
            Adv.GearStore(EnhAfter: true);
            Adv.SmartEnhance("Lich");
            Farm.Experience(rankUpClass: true);
            Adv.GearStore(true, EnhAfter: true);
            RequirementCompleted?.Invoke(4, 3, "Lich Class Rank 10");
        }
    }

    private void AcceptQuestChecked(int questId)
    {
        if (Bot.ShouldExit) throw new OperationCanceledException();
        if (!Core.EnsureAccept(questId) || !Bot.Quests.IsInProgress(questId))
            throw new InvalidOperationException($"Quest {questId} was not accepted. Check its unlock requirements and account access.");
    }

    private void CompleteQuestChecked(int questId)
    {
        if (Bot.ShouldExit) throw new OperationCanceledException();
        if (!Core.EnsureComplete(questId))
            throw new InvalidOperationException($"Quest {questId} could not be turned in. Its requirements may be incomplete.");
    }

    public void LichWarPrereqs()
    {
        if (Core.isCompletedBefore(10281) || Core.isCompletedBefore(10287))
            return;

        // QuestProgression tries to recover locked quests before returning. Complete
        // the actual prerequisites first instead of asking it about locked quest 10284.
        if (!Core.isCompletedBefore(10284) && !Core.isCompletedBefore(10280))
        {
            Core.EquipClass(ClassType.Farm);
            if (!Core.isCompletedBefore(10283) && !Core.isCompletedBefore(10279))
            {
                if (!Core.isCompletedBefore(10282) && !Core.isCompletedBefore(10278))
                {
                    Core.Logger("Unlocking LichWar: Grimskull War Medal (10282).");
                    AcceptQuestChecked(10282);
                    Core.HuntMonster("lichwar", "Noxus Warrior", "Grimskull War Medal", 5);
                    if (Bot.ShouldExit) return;
                    CompleteQuestChecked(10282);
                }

                if (Bot.ShouldExit) return;
                Core.Logger("Unlocking LichWar: Mega Grimskull War Medal (10283).");
                AcceptQuestChecked(10283);
                Core.HuntMonster("lichwar", "Noxus Warrior", "Mega Grimskull War Medal", 3);
                if (Bot.ShouldExit) return;
                CompleteQuestChecked(10283);
            }

            if (Bot.ShouldExit) return;
            Core.Logger("Unlocking LichWar: Undead Giants (10284).");
            AcceptQuestChecked(10284);
            Core.HuntMonster("lichwar", "Noxus Giant", "Noxus Giant Slain", 5);
            if (Bot.ShouldExit) return;
            CompleteQuestChecked(10284);
        }

        if (Bot.ShouldExit) return;
        Core.Logger("Unlocking LichWar: It Takes Two to Tango (10281).");
        AcceptQuestChecked(10281);
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("lichwar", "Rax-goreless", "Rax-goreless Defeated");
        Core.HuntMonster("lichwar", "Armadeddon", "Armadeddon Defeated");
        CompleteQuestChecked(10281);
    }

    private CancellationTokenSource? _moreSkullsCts;
    private DateTime _lastZoneChange = DateTime.MinValue;
    private readonly TimeSpan _zoneChangeCooldown = TimeSpan.FromSeconds(2);
    private string _currentZone = "";
    private Task? _zoneMovementTask;

    public void FarmPristineSkulls(int quant = 66, bool storyOnly = false)
    {
        if (Core.isCompletedBefore(10287) && (storyOnly || Core.CheckInventory("Pristine Skull", quant)))
            return;

        Core.EquipClass(ClassType.Solo);
        _moreSkullsCts = new CancellationTokenSource();
        Bot.Events.ExtensionPacketReceived += HandleMoreSkullsZone;
        Core.AddDrop("Pristine Skull");
        Bot.Options.AttackWithoutTarget = true;
        Core.FarmingLogger("Pristine Skull", quant);

        try
        {
            while (!Bot.ShouldExit && (!Core.isCompletedBefore(10287) ||
                (!storyOnly && !Core.CheckInventory("Pristine Skull", quant))))
            {
                int questId = !Core.isCompletedBefore(10287) ? 10287 : Core.IsMember ? 10289 : 10288;
                AcceptQuestChecked(questId);

                while (!Bot.ShouldExit && !Bot.Player.Alive) { Bot.Sleep(500); }

                if (!Bot.Map.Name.Equals("moreskulls", StringComparison.OrdinalIgnoreCase))
                    Core.Join("moreskulls", "r2", "Left");

                if (Bot.Player.Cell != "r2")
                    Core.Jump("r2");

                if (Bot.ShouldExit) break;
                if (!Bot.Skills.TimerRunning)
                    Core.EquipClass(ClassType.Solo);
                // Keep checking turn-ins while the boss is alive, and recover stale targets.
                Bot.Combat.Attack("*");

                if (Bot.Quests.CanCompleteFullCheck(questId))
                    CompleteQuestChecked(questId);

                Bot.Sleep(500);
            }
        }
        finally
        {
            _moreSkullsCts?.Cancel();
            _moreSkullsCts?.Dispose();
            _moreSkullsCts = null;
            Bot.Events.ExtensionPacketReceived -= HandleMoreSkullsZone;
            Bot.Options.AttackWithoutTarget = false;
        }
    }

    private void HandleMoreSkullsZone(dynamic packet)
    {
        if (_moreSkullsCts == null || _moreSkullsCts.IsCancellationRequested)
            return;

        string? type = packet["params"]?.type;
        if (type != "json")
            return;

        dynamic? data = packet["params"]?.dataObj;
        string? cmd = data?.cmd?.ToString();
        if (cmd != "event")
            return;

        dynamic? args = data?.args;
        string? zone = args?.zoneSet?.ToString()?.Trim();
        if (string.IsNullOrEmpty(zone) || zone == _currentZone)
            return;

        if (DateTime.UtcNow - _lastZoneChange < _zoneChangeCooldown)
            return;

        if (_zoneMovementTask is { IsCompleted: false })
            return;

        _currentZone = zone;
        _lastZoneChange = DateTime.UtcNow;

        CancellationToken token = _moreSkullsCts.Token;
        _zoneMovementTask = Task.Run(
            async () =>
            {
                try
                {
                    await Task.Delay(300, token);

                    int x = 0, y = 0;
                    switch (zone.ToUpper())
                    {
                        case "A":
                            if (Bot.Player.Position.X >= 741 && Bot.Player.Position.X <= 833 &&
                                Bot.Player.Position.Y >= 402 && Bot.Player.Position.Y <= 447)
                                return;
                            x = Bot.Random.Next(741, 833);
                            y = Bot.Random.Next(402, 447);
                            break;

                        case "B":
                            if (Bot.Player.Position.X >= 721 && Bot.Player.Position.X <= 819 &&
                                Bot.Player.Position.Y >= 330 && Bot.Player.Position.Y <= 371)
                                return;
                            x = Bot.Random.Next(721, 820);
                            y = Bot.Random.Next(330, 372);
                            break;

                        default:
                            return;
                    }

                    if (token.IsCancellationRequested || (Bot.Player.Position.X == x && Bot.Player.Position.Y == y))
                        return;

                    Bot.Player.WalkTo(x, y);
                    await Task.Delay(500, token);
                }
                catch (OperationCanceledException) { }
            },
            token
        );
    }
}
