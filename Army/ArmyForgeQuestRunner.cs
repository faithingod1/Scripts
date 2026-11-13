/*
name: Army Forge Quest Runner
description: Handles individual quest stages and requirements for Forge enhancements with synchronization hooks for army farming.
tags: army, forge, enhancement, runner, weapon, cape, helm
*/

#region Includes Area
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Good/GearOfAwe/CoreAwe.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Chaos/EternalDrakathSet.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Good/ArchPaladin.cs
//cs_include Scripts/Good/Paladin.cs
//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Other/Armor/FireChampionsArmor.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs
//cs_include Scripts/Darkon/Various/PrinceDarkonsPoleaxePreReqs.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/StarSinc.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/XansLair.cs
//cs_include Scripts/Story/Lair.cs
//cs_include Scripts/Good/GearOfAwe/Awescended.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Good/GearOfAwe/ArmorOfAwe.cs
//cs_include Scripts/Good/GearOfAwe/HelmOfAwe.cs
//cs_include Scripts/Good/SilverExaltedPaladin.cs
//cs_include Scripts/Other/Weapons/FortitudeAndHubris.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Story/J6Saga.cs
//cs_include Scripts/Story/Nation/Bamboozle.cs
//cs_include Scripts/Story/7DeadlyDragons/Extra/HatchTheEgg.cs
//cs_include Scripts/Other/ShadowDragonDefender.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Story/DjinnGate.cs
//cs_include Scripts/Story/ThirdSpell.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/Yokai.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Other/Armor/MalgorsArmorSet.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
//cs_include Scripts/Nation/Various/PrimeFiendShard.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Enhancement/UnlockForgeEnhancements.cs
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class ArmyForgeQuestRunner
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreDarkon Darkon { get => _Darkon ??= new CoreDarkon(); set => _Darkon = value; }
    private static CoreDarkon _Darkon;
    private static CoreSoWMats SOWM { get => _SOWM ??= new CoreSoWMats(); set => _SOWM = value; }
    private static CoreSoWMats _SOWM;
    private static CoreAwe Awe { get => _Awe ??= new CoreAwe(); set => _Awe = value; }
    private static CoreAwe _Awe;
    private static Core13LoC LOC { get => _LOC ??= new Core13LoC(); set => _LOC = value; }
    private static Core13LoC _LOC;
    private static CoreNSOD CorNSOD { get => _CorNSOD ??= new CoreNSOD(); set => _CorNSOD = value; }
    private static CoreNSOD _CorNSOD;
    private static CoreAstravia Astravia { get => _Astravia ??= new CoreAstravia(); set => _Astravia = value; }
    private static CoreAstravia _Astravia;
    private static Core7DD DD { get => _DD ??= new Core7DD(); set => _DD = value; }
    private static Core7DD _DD;
    private static CoreYnR YNR { get => _YNR ??= new CoreYnR(); set => _YNR = value; }
    private static CoreYnR _YNR;
    private static CoreSoW SoW { get => _SoW ??= new CoreSoW(); set => _SoW = value; }
    private static CoreSoW _SoW;
    private static CoreSepulchure CoreSS { get => _CoreSS ??= new CoreSepulchure(); set => _CoreSS = value; }
    private static CoreSepulchure _CoreSS;
    private static ArchPaladin AP { get => _AP ??= new ArchPaladin(); set => _AP = value; }
    private static ArchPaladin _AP;
    private static DragonOfTime DOT { get => _DOT ??= new DragonOfTime(); set => _DOT = value; }
    private static DragonOfTime _DOT;
    private static FireChampionsArmor FCA { get => _FCA ??= new FireChampionsArmor(); set => _FCA = value; }
    private static FireChampionsArmor _FCA;
    private static EternalDrakath ED { get => _ED ??= new EternalDrakath(); set => _ED = value; }
    private static EternalDrakath _ED;
    private static SepulchuresOriginalHelm Seppy { get => _Seppy ??= new SepulchuresOriginalHelm(); set => _Seppy = value; }
    private static SepulchuresOriginalHelm _Seppy;
    private static PrinceDarkonsPoleaxePreReqs PDPPR { get => _PDPPR ??= new PrinceDarkonsPoleaxePreReqs(); set => _PDPPR = value; }
    private static PrinceDarkonsPoleaxePreReqs _PDPPR;
    private static HeadoftheLegionBeast HOTLB { get => _HOTLB ??= new HeadoftheLegionBeast(); set => _HOTLB = value; }
    private static HeadoftheLegionBeast _HOTLB;
    private static Awescended Awescended { get => _Awescended ??= new Awescended(); set => _Awescended = value; }
    private static Awescended _Awescended;
    private static NulgathDemandsWork NDW { get => _NDW ??= new NulgathDemandsWork(); set => _NDW = value; }
    private static NulgathDemandsWork _NDW;
    private static ThirdSpell TSS { get => _TSS ??= new ThirdSpell(); set => _TSS = value; }
    private static ThirdSpell _TSS;
    private static LordOfOrder LOO { get => _LOO ??= new LordOfOrder(); set => _LOO = value; }
    private static LordOfOrder _LOO;
    private static SevenCircles Circles { get => _Circles ??= new SevenCircles(); set => _Circles = value; }
    private static SevenCircles _Circles;
    private static YokaiQuests Yokai { get => _Yokai ??= new YokaiQuests(); set => _Yokai = value; }
    private static YokaiQuests _Yokai;
    private static MalgorsArmorSet MAS { get => _MAS ??= new MalgorsArmorSet(); set => _MAS = value; }
    private static MalgorsArmorSet _MAS;
    private static PrimeFiendShard PFS { get => _PFS ??= new PrimeFiendShard(); set => _PFS = value; }
    private static PrimeFiendShard _PFS;
    private static UnlockForgeEnhancements Forge { get => _Forge ??= new UnlockForgeEnhancements(); set => _Forge = value; }
    private static UnlockForgeEnhancements _Forge;

    public Action<int, int, string>? RequirementCompleted { get; set; }
    public Action<string, Action>? SoloRequirement { get; set; }

    public static readonly int[] StageQuestIds =
    {
        8738, // 0: Forge (Weapon)
        8758, // 1: Forge (Cape)
        8828, // 2: Forge (Helm)
        8824, // 3: Vim (Helm)
        8825, // 4: Examen (Helm)
        8826, // 5: Anima (Helm)
        8827, // 6: Pneuma (Helm)
        8739, // 7: Lacerate (Weapon)
        8740, // 8: Smite (Weapon)
        8743, // 9: Absolution (Cape)
        8744, // 10: Vainglory (Cape)
        8745, // 11: Avarice (Cape)
        8822, // 12: Penitence (Cape)
        8823, // 13: Lament (Cape)
        9171, // 14: Praxis (Weapon)
        8821, // 15: Elysium (Weapon)
        8820, // 16: Acheron (Weapon)
        8741, // 17: Hero's Valiance (Weapon)
        9172, // 18: Dauntless (Weapon)
        9560, // 19: Ravenous (Weapon)
        8742  // 20: Arcana's Concerto (Weapon)
    };

    public static readonly string[] StageNames =
    {
        "Forge (Weapon) [Quest 8738]",
        "Forge (Cape) [Quest 8758]",
        "Forge (Helm) [Quest 8828]",
        "Vim (Helm) [Quest 8824]",
        "Examen (Helm) [Quest 8825]",
        "Anima (Helm) [Quest 8826]",
        "Pneuma (Helm) [Quest 8827]",
        "Lacerate (Weapon) [Quest 8739]",
        "Smite (Weapon) [Quest 8740]",
        "Absolution (Cape) [Quest 8743]",
        "Vainglory (Cape) [Quest 8744]",
        "Avarice (Cape) [Quest 8745]",
        "Penitence (Cape) [Quest 8822]",
        "Lament (Cape) [Quest 8823]",
        "Praxis (Weapon) [Quest 9171]",
        "Elysium (Weapon) [Quest 8821]",
        "Acheron (Weapon) [Quest 8820]",
        "Hero's Valiance (Weapon) [Quest 8741]",
        "Dauntless (Weapon) [Quest 9172]",
        "Ravenous (Weapon) [Quest 9560]",
        "Arcana's Concerto (Weapon) [Quest 8742]"
    };

    public static readonly int TotalStages = 21;

    public static bool IsStageCompleted(int stage)
    {
        if (stage < 0 || stage >= StageQuestIds.Length) return false;
        return CoreBots.Instance.isCompletedBefore(StageQuestIds[stage]);
    }

    public static bool IsStageBlocked(int stage, bool useInsignDaunt = false, bool useInsignConcerto = false)
    {
        if (IsStageCompleted(stage)) return false;

        switch (stage)
        {
            case 16: // Acheron: Dark Box & Dark Key
                return !CoreBots.Instance.CheckInventory(new[] { 38566, 38567 }, toInv: false);

            case 17: // Hero's Valiance: LoO Daily Quest 7165 & Drakath the Eternal
                return !CoreBots.Instance.isCompletedBefore(7165) ||
                       !CoreBots.Instance.CheckInventory("Drakath the Eternal", toInv: false);

            case 18: // Dauntless: Insignias
                return !CoreBots.Instance.CheckInventory("Malgor Insignia", 5, toInv: false) ||
                       !CoreBots.Instance.CheckInventory("Avatar Tyndarius Insignia", 10, toInv: false);

            case 19: // Ravenous: Gluttonous Maw or 10 Roentgeniums
                return !CoreBots.Instance.CheckInventory("Gluttonous Maw", toInv: false) &&
                       !CoreBots.Instance.CheckInventory("Roentgenium of Nulgath", 10, toInv: false);

            case 20: // Arcana's Concerto: Weekly 8746 & Insignias
                return !CoreBots.Instance.isCompletedBefore(8746) ||
                       !CoreBots.Instance.CheckInventory("King Drago Insignia", 5, toInv: false) ||
                       !CoreBots.Instance.CheckInventory("Darkon Insignia", 5, toInv: false);

            default:
                return false;
        }
    }

    public static int FirstIncompleteStage(bool skipBlocked = true, bool useInsignDaunt = false, bool useInsignConcerto = false)
    {
        for (int i = 0; i < TotalStages; i++)
        {
            if (IsStageCompleted(i))
                continue;
            if (skipBlocked && IsStageBlocked(i, useInsignDaunt, useInsignConcerto))
                continue;
            return i;
        }
        return TotalStages;
    }

    private void RunSolo(string map, Action farm)
    {
        if (SoloRequirement != null) SoloRequirement(map, farm);
        else farm();
    }

    public void RunStage(int stage, bool useGold = false, bool sellQuestClass = false, bool useInsignDaunt = false, bool useInsignConcerto = false)
    {
        string soloClass = Core.SoloClass;
        string farmClass = Core.FarmClass;
        string dodgeClass = Core.DodgeClass;
        string bossClass = Core.BossClass;

        try
        {
            // Generic makes Core.EquipClass keep the worn class without changing gear.
            Core.SoloClass = Core.FarmClass = Core.DodgeClass = Core.BossClass = "Generic";

            switch (stage)
            {
                case 0: DoForgeWeapon(useGold); break;
                case 1: DoForgeCape(useGold); break;
                case 2: DoForgeHelm(useGold); break;
                case 3: DoVim(useGold, sellQuestClass); break;
                case 4: DoExamen(useGold, sellQuestClass); break;
                case 5: DoAnima(useGold, sellQuestClass); break;
                case 6: DoPneuma(useGold, sellQuestClass); break;
                case 7: DoLacerate(useGold); break;
                case 8: DoSmite(useGold); break;
                case 9: DoAbsolution(useGold); break;
                case 10: DoVainglory(useGold); break;
                case 11: DoAvarice(useGold); break;
                case 12: DoPenitence(useGold); break;
                case 13: DoLament(useGold); break;
                case 14: DoPraxis(useGold); break;
                case 15: DoElysium(useGold); break;
                case 16: DoAcheron(useGold); break;
                case 17: DoHerosValiance(useGold); break;
                case 18: DoDauntless(useGold, useInsignDaunt); break;
                case 19: DoRavenous(); break;
                case 20: DoArcanasConcerto(useGold, useInsignConcerto); break;
                default:
                    Core.Logger($"Unknown stage {stage}");
                    break;
            }
        }
        finally
        {
            Core.SoloClass = soloClass;
            Core.FarmClass = farmClass;
            Core.DodgeClass = dodgeClass;
            Core.BossClass = bossClass;
        }
    }

    #region Stage Implementations

    public void DoForgeWeapon(bool useGold)
    {
        if (Core.isCompletedBefore(8738)) return;
        Core.Logger("Unlocking Enhancement: Forge (Weapon)");

        Farm.Experience(30);
        LOC.Escherion();
        Farm.BlacksmithingREP(4, useGold);

        Core.EnsureAccept(8738);

        if (!Core.CheckInventory("1st Lord Of Chaos Helm"))
            Core.KillEscherion("1st Lord Of Chaos Helm");
        RequirementCompleted?.Invoke(0, 1, "1st Lord Of Chaos Helm");

        if (!Core.CheckInventory("Chaos Dragonlord Helm"))
            Core.KillVath("Chaos Dragonlord Helm");
        RequirementCompleted?.Invoke(0, 2, "Chaos Dragonlord Helm");

        if (!Core.CheckInventory("Chaos Shogun Helmet"))
            Core.KillKitsune("Chaos Shogun Helmet");
        RequirementCompleted?.Invoke(0, 3, "Chaos Shogun Helmet");

        if (!Core.CheckInventory("Wolfwing Mask"))
            Core.HuntMonster("wolfwing", "Wolfwing", "Wolfwing Mask", isTemp: false);
        RequirementCompleted?.Invoke(0, 4, "Wolfwing Mask");

        Core.EnsureComplete(8738);
        RequirementCompleted?.Invoke(0, 5, "Forge (Weapon) Completed");
        Core.Logger("Enhancement Unlocked: Forge (Weapon)");
    }

    public void DoForgeCape(bool useGold)
    {
        if (Core.isCompletedBefore(8758)) return;
        Core.Logger("Unlocking Enhancement: Forge (Cape)");

        Farm.Experience(30);
        LOC.Kitsune();
        Farm.BlacksmithingREP(4, useGold);

        Core.EnsureAccept(8758);

        if (!Core.CheckInventory("Prismatic Celestial Wings"))
            Core.HuntMonster("lostruinswar", "Diabolical Warlord", "Prismatic Celestial Wings", isTemp: false);
        RequirementCompleted?.Invoke(1, 1, "Prismatic Celestial Wings");

        if (!Core.CheckInventory("Broken Wings"))
            Core.HuntMonster("lostruins", "Infernal Warlord", "Broken Wings", isTemp: false);
        RequirementCompleted?.Invoke(1, 2, "Broken Wings");

        if (!Core.CheckInventory("Shadow's Wings"))
            Core.HuntMonster("infernalspire", "Azkorath", "Shadow's Wings", isTemp: false);
        RequirementCompleted?.Invoke(1, 3, "Shadow's Wings");

        if (!Core.CheckInventory("Wings Of Destruction"))
            Core.HuntMonster("infernalspire", "Malxas", "Wings Of Destruction", isTemp: false);
        RequirementCompleted?.Invoke(1, 4, "Wings Of Destruction");

        Core.EnsureComplete(8758);
        RequirementCompleted?.Invoke(1, 5, "Forge (Cape) Completed");
        Core.Logger("Enhancement Unlocked: Forge (Cape)");
    }

    public void DoForgeHelm(bool useGold)
    {
        if (Core.isCompletedBefore(8828)) return;
        Core.Logger("Unlocking Enhancement: Forge (Helm)");

        Farm.Experience(30);
        Farm.BlacksmithingREP(4, useGold);

        Core.EnsureAccept(8828);

        if (!Core.CheckInventory("1st Lord Of Chaos Staff"))
            Core.KillEscherion("1st Lord Of Chaos Staff");
        RequirementCompleted?.Invoke(2, 1, "1st Lord Of Chaos Staff");

        if (!Core.CheckInventory("Chaos Dragonlord Axe"))
            Core.KillVath("Chaos Dragonlord Axe");
        RequirementCompleted?.Invoke(2, 2, "Chaos Dragonlord Axe");

        if (!Core.CheckInventory("Hanzamune Dragon Koi Blade"))
            Core.KillKitsune("Hanzamune Dragon Koi Blade");
        RequirementCompleted?.Invoke(2, 3, "Hanzamune Dragon Koi Blade");

        if (!Core.CheckInventory("Wrath of the Werepyre"))
            Core.HuntMonster("wolfwing", "Wolfwing", "Wrath of the Werepyre", isTemp: false);
        RequirementCompleted?.Invoke(2, 4, "Wrath of the Werepyre");

        Core.EnsureComplete(8828);
        RequirementCompleted?.Invoke(2, 5, "Forge (Helm) Completed");
        Core.Logger("Enhancement Unlocked: Forge (Helm)");
    }

    public void DoVim(bool useGold, bool sellQuestClass)
    {
        if (Core.isCompletedBefore(8824)) return;
        DoForgeHelm(useGold);

        Core.Logger("Unlocking Enhancement: Vim");
        Farm.BlacksmithingREP(7, useGold);

        Adv.GearStore(EnhAfter: true);
        Core.EnsureAccept(8824);

        if (Core.CheckClassRank(false, "Rogue") < 10)
        {
            Core.BuyItem("Classhalla", 172, "Rogue");
            Adv.RankUpClass("Rogue");
        }
        Adv.GearStore(true, EnhAfter: true);
        RequirementCompleted?.Invoke(3, 1, "Rogue Rank 10");

        if (!Core.CheckInventory("Ethereal Essence", 250))
            Core.KillMonster("Towerofdoom10", "r8", "Left", "*", "Ethereal Essence", 250, isTemp: false);
        RequirementCompleted?.Invoke(3, 2, "Ethereal Essence x250");

        Core.EnsureComplete(8824);
        if (sellQuestClass) Core.SellItem("Rogue");
        RequirementCompleted?.Invoke(3, 3, "Vim Completed");
        Core.Logger("Enhancement Unlocked: Vim");
    }

    public void DoExamen(bool useGold, bool sellQuestClass)
    {
        if (Core.isCompletedBefore(8825)) return;
        DoVim(useGold, sellQuestClass);

        Core.Logger("Unlocking Enhancement: Examen");

        Adv.GearStore(EnhAfter: true);
        Core.EnsureAccept(8825);

        if (Core.CheckClassRank(false, "Healer") < 10)
        {
            Core.BuyItem("Classhalla", 176, "Healer");
            Adv.RankUpClass("Healer");
        }
        Adv.GearStore(true, EnhAfter: true);
        RequirementCompleted?.Invoke(4, 1, "Healer Rank 10");

        if (!Core.CheckInventory("Ethereal Essence", 250))
            Core.KillMonster("Towerofdoom10", "r8", "Left", "*", "Ethereal Essence", 250, isTemp: false);
        RequirementCompleted?.Invoke(4, 2, "Ethereal Essence x250");

        Core.EnsureComplete(8825);
        if (sellQuestClass) Core.SellItem("Healer");
        RequirementCompleted?.Invoke(4, 3, "Examen Completed");
        Core.Logger("Enhancement Unlocked: Examen");
    }

    public void DoAnima(bool useGold, bool sellQuestClass)
    {
        if (Core.isCompletedBefore(8826)) return;
        DoExamen(useGold, sellQuestClass);

        Core.Logger("Unlocking Enhancement: Anima");

        Adv.GearStore(EnhAfter: true);
        Core.EnsureAccept(8826);

        if (Core.CheckClassRank(false, "Warrior") < 10)
        {
            Core.BuyItem("Classhalla", 170, "Warrior");
            Adv.RankUpClass("Warrior");
        }
        Adv.GearStore(true, EnhAfter: true);
        RequirementCompleted?.Invoke(5, 1, "Warrior Rank 10");

        if (!Core.CheckInventory("Ethereal Essence", 650))
            Core.KillMonster("Towerofdoom10", "r8", "Left", "*", "Ethereal Essence", 650, isTemp: false);
        RequirementCompleted?.Invoke(5, 2, "Ethereal Essence x650");

        Core.EnsureComplete(8826);
        if (sellQuestClass) Core.SellItem("Warrior");
        RequirementCompleted?.Invoke(5, 3, "Anima Completed");
        Core.Logger("Enhancement Unlocked: Anima");
    }

    public void DoPneuma(bool useGold, bool sellQuestClass)
    {
        if (Core.isCompletedBefore(8827)) return;
        DoAnima(useGold, sellQuestClass);

        Core.Logger("Unlocking Enhancement: Pneuma");

        Adv.GearStore(EnhAfter: true);
        Core.EnsureAccept(8827);

        if (Core.CheckClassRank(false, "Mage") < 10)
        {
            Core.BuyItem("Classhalla", 174, "Mage");
            Adv.RankUpClass("Mage");
        }
        Adv.GearStore(true, EnhAfter: true);
        RequirementCompleted?.Invoke(6, 1, "Mage Rank 10");

        if (!Core.CheckInventory("Ethereal Essence", 650))
            Core.KillMonster("Towerofdoom10", "r8", "Left", "*", "Ethereal Essence", 650, isTemp: false);
        RequirementCompleted?.Invoke(6, 2, "Ethereal Essence x650");

        Core.EnsureComplete(8827);
        if (sellQuestClass) Core.SellItem("Mage");
        RequirementCompleted?.Invoke(6, 3, "Pneuma Completed");
        Core.Logger("Enhancement Unlocked: Pneuma");
    }

    public void DoLacerate(bool useGold)
    {
        if (Core.isCompletedBefore(8739)) return;
        Core.Logger("Unlocking Enhancement: Lacerate");

        if (!Bot.Quests.IsUnlocked(8739))
        {
            if (!Bot.Quests.IsUnlocked(92))
            {
                if (!Bot.Quests.IsUnlocked(91))
                {
                    Core.EnsureAccept(90);
                    Core.HuntMonster("pirates", "Shark Bait", "Pirate Medallion", 12);
                    Core.EnsureComplete(90);
                }
                Core.EnsureAccept(91);
                Core.KillMonster("greenguardwest", "West1", "Left", "Kittarian", "Kittarian's Wallet", 2);
                Core.KillMonster("greenguardwest", "West9", "Left", "River Fishman", "River Fishman's Wallet", 2);
                Core.KillMonster("greenguardwest", "West10", "Left", "Slime", "Slime-Soaked Wallet", 2);
                Core.KillMonster("greenguardwest", "West3", "Left", "Frogzard", "Frogzard's Lint Hoard", 2);
                Core.KillMonster("greenguardwest", "West12", "Up", "Big Bad Boar", "Big Bad Boar's Wallet");
                Core.EnsureComplete(91);
            }
            if (!Story.QuestProgression(92))
            {
                Core.EnsureAccept(92);
                Core.HuntMonster("greenguardwest", "Breken the Vile", "Breken the Vile's Sword", 1);
                Core.HuntMonster("greenguardwest", "Ogug Stoneaxe", "Ogug Stoneaxe's Helmet", 1);
                Core.EnsureComplete(92);
            }
        }
        RequirementCompleted?.Invoke(7, 1, "Greenguardwest Unlocks");

        Farm.Experience(40);
        Farm.BlacksmithingREP(5, useGold);

        Core.EnsureAccept(8739);

        if (!Core.CheckInventory("Undead Plague Spear"))
            Core.HuntMonster("graveyard", "Big Jack Sprat", "Undead Plague Spear", isTemp: false);
        RequirementCompleted?.Invoke(7, 2, "Undead Plague Spear");

        if (!Core.CheckInventory("Kuro's Wrath"))
            Core.HuntMonster("river", "Kuro", "Kuro's Wrath", isTemp: false);
        RequirementCompleted?.Invoke(7, 3, "Kuro's Wrath");

        if (!Core.CheckInventory("Massive Horc Cleaver"))
        {
            Core.AddDrop("Massive Horc Cleaver");
            Core.EnsureAccept(279);
            Core.HuntMonster("warhorc", "Horc Master", "Boss Prize");
            Core.EnsureComplete(279);
        }
        RequirementCompleted?.Invoke(7, 4, "Massive Horc Cleaver");

        if (!Core.CheckInventory("Sword in the Stone"))
        {
            Core.AddDrop("Sword in the Stone");
            Core.EnsureAccept(316);
            Core.GetMapItem(54, 7, "greenguardeast");
            Core.HuntMonster("greenguardeast", "Spider", "Tiny Sword");
            Core.EnsureComplete(316);
        }
        RequirementCompleted?.Invoke(7, 5, "Sword in the Stone");

        if (!Core.CheckInventory("Forest Axe"))
        {
            Core.AddDrop("Forest Axe");
            Core.EnsureAccept(301);
            Core.GetMapItem(55, 4, "farm");
            Core.HuntMonster("farm", "Mosquito", "Mosquito Juice");
            Core.EnsureComplete(301);
        }
        RequirementCompleted?.Invoke(7, 6, "Forest Axe");

        Farm.BlackKnightOrb();
        RequirementCompleted?.Invoke(7, 7, "Black Knight Orb");

        Core.EnsureComplete(8739);
        RequirementCompleted?.Invoke(7, 8, "Lacerate Completed");
        Core.Logger("Enhancement Unlocked: Lacerate");
    }

    public void DoSmite(bool useGold)
    {
        if (Core.isCompletedBefore(8740)) return;
        Core.Logger("Unlocking Enhancement: Smite");

        Farm.Experience(60);
        Farm.BlacksmithingREP(6, useGold);
        CoreSS.ShadowfallRise();
        RequirementCompleted?.Invoke(8, 1, "Shadowfall Rise");

        Core.EnsureAccept(8740);

        if (!Core.CheckInventory("Death's Power", 3))
            Core.HuntMonster("shadowattack", "Death", "Death's Power", 3, isTemp: false);
        RequirementCompleted?.Invoke(8, 2, "Death's Power x3");

        if (!Core.CheckInventory("Chaotic Power", 7))
            Core.KillEscherion("Chaotic Power", 7);
        RequirementCompleted?.Invoke(8, 3, "Chaotic Power x7");

        if (!Core.CheckInventory("Empowered Essence", 50))
            Core.HuntMonster("shadowrealmpast", "Pure Shadowscythe", "Empowered Essence", 50, isTemp: false);
        RequirementCompleted?.Invoke(8, 4, "Empowered Essence x50");

        if (!Core.CheckInventory("Gem Power", 25))
            Core.HuntMonster("undergroundlabb", "Ultra Battle Gem", "Gem Power", 25, false);
        RequirementCompleted?.Invoke(8, 5, "Gem Power x25");

        Adv.BuyItem("alchemyacademy", 2116, "Power Tonic", 10);
        RequirementCompleted?.Invoke(8, 6, "Power Tonic x10");

        Core.EnsureComplete(8740);
        RequirementCompleted?.Invoke(8, 7, "Smite Completed");
        Core.Logger("Enhancement Unlocked: Smite");
    }

    public void DoAbsolution(bool useGold)
    {
        if (Core.isCompletedBefore(8743)) return;
        Core.Logger("Unlocking Enhancement: Absolution");

        Farm.Experience(90);
        Farm.GoodREP(10);
        Farm.BlacksmithingREP(9, useGold);

        int slimeSigil = 150;
        if (Core.CheckInventory(39091)) slimeSigil -= 100;
        if (Core.CheckInventory(39094)) slimeSigil -= 50;

        if (slimeSigil > 0)
        {
            Bot.Quests.UpdateQuest(5807);
            Core.KillMonster("charredpath", "r5", "Left", "Plague Spreader", "Slimed Sigil", slimeSigil, isTemp: false);
        }
        RequirementCompleted?.Invoke(9, 1, "Slimed Sigil x150");

        Adv.BuyItem("therift", 1399, "Ascended Paladin");
        Adv.BuyItem("therift", 1399, "Ascended Paladin Sword");
        RequirementCompleted?.Invoke(9, 2, "Ascended Paladin Items");

        Core.ChainComplete(8743);
        RequirementCompleted?.Invoke(9, 3, "Absolution Completed");
        Core.Logger("Enhancement Unlocked: Absolution");
    }

    public void DoVainglory(bool useGold)
    {
        if (Core.isCompletedBefore(8744)) return;
        DoAbsolution(useGold);

        Core.Logger("Unlocking Enhancement: Vainglory");
        Core.EnsureAccept(8744);
        Farm.BlacksmithingREP(9, useGold);

        if (Core.IsMember)
        {
            if (!Core.CheckInventory("Pauldron Relic"))
            {
                Farm.BladeofAweREP(10, false);
                Farm.Experience(55);
                Core.BuyItem("museum", 1130, "Armor of Awe Pass");
                Core.AddDrop("Pauldron Fragment");
                Core.RegisterQuests(4162);
                Core.HuntMonster("gravestrike", "Ultra Akriloth", "Pauldron Fragment", 15, false);
                Bot.Wait.ForPickup("Pauldron Fragment");
                Core.CancelRegisteredQuests();
                Core.BuyItem("museum", 1129, "Pauldron Relic");
            }
        }
        else
            Awe.GetAweRelic("Pauldron", 4160, 15, 15, "gravestrike", "Ultra Akriloth");
        RequirementCompleted?.Invoke(10, 1, "Pauldron Relic");

        Awe.GetAweRelic("Breastplate", 4163, 10, 10, "aqlesson", "Carnax");
        RequirementCompleted?.Invoke(10, 2, "Breastplate Relic");

        Awe.GetAweRelic("Vambrace", 4166, 15, 15, "bloodtitan", "Ultra Blood Titan");
        RequirementCompleted?.Invoke(10, 3, "Vambrace Relic");

        Awe.GetAweRelic("Gauntlet", 4169, 25, 5, "alteonbattle", "ULTRA Alteon");
        RequirementCompleted?.Invoke(10, 4, "Gauntlet Relic");

        Awe.GetAweRelic("Greaves", 4172, 10, 15, "bosschallenge", "Mutated Void Dragon");
        RequirementCompleted?.Invoke(10, 5, "Greaves Relic");

        Core.EnsureComplete(8744);
        RequirementCompleted?.Invoke(10, 6, "Vainglory Completed");
        Core.Logger("Enhancement Unlocked: Vainglory");
    }

    public void DoAvarice(bool useGold)
    {
        if (Core.isCompletedBefore(8745)) return;
        DoVainglory(useGold);

        Core.Logger("Unlocking Enhancement: Avarice");
        Farm.BlacksmithingREP(9, useGold);
        Core.EnsureAccept(8745);

        Circles.CirclesWar();
        RequirementCompleted?.Invoke(11, 1, "Circles War Story");

        HOTLB.Indulgence(50);
        RequirementCompleted?.Invoke(11, 2, "Indulgence x50");

        HOTLB.Penance(50);
        RequirementCompleted?.Invoke(11, 3, "Penance x50");

        Core.EnsureComplete(8745);
        RequirementCompleted?.Invoke(11, 4, "Avarice Completed");
        Core.Logger("Enhancement Unlocked: Avarice");
    }

    public void DoPenitence(bool useGold)
    {
        if (Core.isCompletedBefore(8822)) return;
        DoAvarice(useGold);

        Core.Logger("Unlocking Enhancement: Penitence");
        Farm.BlacksmithingREP(9, useGold);
        Core.EnsureAccept(8822);

        while (!Bot.ShouldExit && !Core.CheckInventory("Night Mare Scythe"))
        {
            Core.AddDrop("Night Mare Scythe");
            Core.EnsureAccept(3270);
            Core.KillMonster("doomvault", "r5", "Left", "Binky", "Yulgar's Lost Scythe");
            Core.EnsureComplete(3270);
        }
        RequirementCompleted?.Invoke(12, 1, "Night Mare Scythe");

        if (!Core.CheckInventory("Sapphire Orb", 100))
            Core.HuntMonster("frozenlair", "Legion Lich Lord", "Sapphire Orb", 100, isTemp: false);
        RequirementCompleted?.Invoke(12, 2, "Sapphire Orb x100");

        if (!Core.CheckInventory("Boreal Cavalier Bardiche"))
            Core.HuntMonster("icewing", "Warlord Icewing", "Boreal Cavalier Bardiche", isTemp: false);
        RequirementCompleted?.Invoke(12, 3, "Boreal Cavalier Bardiche");

        if (!Core.CheckInventory("Void Scale", 13))
            Core.KillArchFiendDragonLord("Void Scale", 13, isTemp: false);
        RequirementCompleted?.Invoke(12, 4, "Void Scale x13");

        Core.EnsureComplete(8822);
        RequirementCompleted?.Invoke(12, 5, "Penitence Completed");
        Core.Logger("Enhancement Unlocked: Penitence");
    }

    public void DoLament(bool useGold)
    {
        if (Core.isCompletedBefore(8823)) return;
        DoPenitence(useGold);

        Core.Logger("Unlocking Enhancement: Lament");
        Farm.BlacksmithingREP(9, useGold);
        Core.EnsureAccept(8823);

        if (!Core.CheckInventory("Doom Heart"))
            Core.HuntMonster("sepulchurebattle", "ULTRA Sepulchure", "Doom Heart", isTemp: false);
        RequirementCompleted?.Invoke(13, 1, "Doom Heart");

        if (!Core.CheckInventory("Heart of the Sun"))
            TSS.StoryLine(true);
        RequirementCompleted?.Invoke(13, 2, "Heart of the Sun");

        if (!Core.CheckInventory("Flame Heart", 10))
            Core.HuntMonster("ashfallcamp", "Smoldur", "Flame Heart", 10, isTemp: false);
        RequirementCompleted?.Invoke(13, 3, "Flame Heart x10");

        DD.Sloth();
        Adv.GearStore(EnhAfter: true);
        DD.HazMatSuit();
        if (!Core.CheckInventory("Bloodless Heart", 3))
            Core.HuntMonster("sloth", "Mutated Plague", "Bloodless Heart", 3, isTemp: false);
        Adv.GearStore(true, EnhAfter: true);
        RequirementCompleted?.Invoke(13, 4, "Bloodless Heart x3");

        Core.EnsureComplete(8823);
        RequirementCompleted?.Invoke(13, 5, "Lament Completed");
        Core.Logger("Enhancement Unlocked: Lament");
    }

    public void DoPraxis(bool useGold)
    {
        if (Core.isCompletedBefore(9171)) return;
        Core.Logger("Unlocking Enhancement: Praxis");

        Core.EnsureAccept(9171);
        Farm.BlacksmithingREP(6, useGold);

        Adv.BuyItem("thespan", 439, "Thief of Hours Armor");
        Adv.BuyItem("yulgar", 69, "Hashashin Armor");
        Adv.BuyItem("dragonkoiz", 95, "Imperial Chunin Clone");
        RequirementCompleted?.Invoke(14, 1, "Praxis Armors");

        if (!Core.CheckInventory("Dragon Rogue"))
            Core.HuntMonster("ectocave", "Ektorax", "Dragon Rogue", isTemp: false);
        RequirementCompleted?.Invoke(14, 2, "Dragon Rogue");

        YNR.Yami(3);
        RequirementCompleted?.Invoke(14, 3, "Yami rank 3");

        Yokai.Quests();
        RequirementCompleted?.Invoke(14, 4, "Yokai Quests");

        Core.AddDrop("Dragon Shinobi Token");
        Core.RegisterQuests(7924);
        while (!Bot.ShouldExit && !Core.CheckInventory("Dragon Shinobi Token", 100))
            Core.HuntMonster("shadowfortress", "1st Head of Orochi", "Perfect Orochi Scales", 10, isTemp: false);
        Core.CancelRegisteredQuests();
        RequirementCompleted?.Invoke(14, 5, "Dragon Shinobi Token x100");

        Adv.BuyItem("shadowfortress", 1968, 59465, shopItemID: 6869);
        Core.EnsureComplete(9171);
        RequirementCompleted?.Invoke(14, 6, "Praxis Completed");
        Core.Logger("Enhancement Unlocked: Praxis");
    }

    public void DoElysium(bool useGold)
    {
        if (Core.isCompletedBefore(8821)) return;
        Core.Logger("Unlocking Enhancement: Elysium");

        Farm.BlacksmithingREP(10, useGold);
        Core.EnsureAccept(8821);

        CorNSOD.BonesVoidRealm(15);
        RequirementCompleted?.Invoke(15, 1, "Bones Void Realm x15");

        YNR.BlademasterSwordScroll();
        RequirementCompleted?.Invoke(15, 2, "Blademaster Sword Scroll");

        NDW.NDWQuest(new[] { "Archfiend Essence Fragment" }, 3);
        RequirementCompleted?.Invoke(15, 3, "Archfiend Essence Fragment x3");

        Awescended.GetAwe();
        RequirementCompleted?.Invoke(15, 4, "Awescended Set");

        if (!Core.CheckInventory("The Divine Will"))
        {
            Core.Logger("Farming Azalith with the army squad for The Divine Will.");
            Core.HuntMonster("celestialpast", "Azalith", "The Divine Will", 1, false);
        }
        RequirementCompleted?.Invoke(15, 5, "The Divine Will");

        Core.EnsureComplete(8821);
        RequirementCompleted?.Invoke(15, 6, "Elysium Completed");
        Core.Logger("Enhancement Unlocked: Elysium");
    }

    public void DoAcheron(bool useGold)
    {
        if (Core.isCompletedBefore(8820)) return;
        if (!Core.CheckInventory(new[] { 38566, 38567 }, toInv: false))
        {
            Core.Logger("Skipping Acheron: Dark Box and/or Dark Key not owned.");
            return;
        }

        Core.Logger("Unlocking Enhancement: Acheron");
        Farm.BlacksmithingREP(8, useGold);
        Core.EnsureAccept(8820);

        Forge.VoidLodestone();
        RequirementCompleted?.Invoke(16, 1, "Void Lodestone");

        SoW.Tyndarius();
        Core.BuyItem(Bot.Map.Name, 1380, "The Power of Darkness");
        RequirementCompleted?.Invoke(16, 2, "The Power of Darkness");

        Core.RegisterQuests(5710);
        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Potion", 20))
        {
            if (!Core.IsMember)
                Core.HuntMonster("darkfortress", "Dark Elemental", "Dark Gem", isTemp: false);
            else
                Core.HuntMonster("ruins", "Dark Elemental", "Dark Gem", isTemp: false);
        }
        Core.CancelRegisteredQuests();
        RequirementCompleted?.Invoke(16, 3, "Dark Potion x20");

        if (!Core.CheckInventory("The Mortal Coil"))
            Core.HuntMonster("tercessuinotlim", "Nulgath", "The Mortal Coil", isTemp: false);
        RequirementCompleted?.Invoke(16, 4, "The Mortal Coil");

        Core.EnsureComplete(8820);
        RequirementCompleted?.Invoke(16, 5, "Acheron Completed");
        Core.Logger("Enhancement Unlocked: Acheron");
    }

    public void DoHerosValiance(bool useGold)
    {
        if (Core.isCompletedBefore(8741)) return;

        Farm.Experience(100);
        Farm.BlacksmithingREP(10, useGold);

        FCA.GetFireChampsArmor();
        DOT.GetDoT(rankUpClass: false, doExtra: false);
        ED.getSet(true, "Drakath the Eternal");
        if (!Core.CheckInventory("Drakath the Eternal"))
        {
            Core.Logger("Hero's Valiance cannot finish without Drakath the Eternal.");
            return;
        }
        RequirementCompleted?.Invoke(17, 1, "Drakath the Eternal");

        LOO.GetLoO(rankUpClass: false);
        if (!Core.isCompletedBefore(7165))
        {
            Core.Logger("Hero's Valiance cannot finish without Lord of Order.");
            return;
        }
        RequirementCompleted?.Invoke(17, 2, "Lord of Order");

        if (!Core.CheckInventory(23689))
        {
            Core.AddDrop("Eternity Blade");
            Core.EnsureAccept(3485);
            Core.HuntMonster("towerofdoom10", "Slugbutter", "Eternity Blade");
            Core.EnsureComplete(3485);
            Bot.Wait.ForPickup(23689);
        }
        RequirementCompleted?.Invoke(17, 3, "Eternity Blade");

        Core.EnsureAccept(8741);
        Seppy.GravelynsDoomFireToken();
        AP.GetAP(rankUpClass: false, ArmorOnly: true);
        Adv.BuyItem("darkthronehub", 1303, "ArchPaladin Armor");
        RequirementCompleted?.Invoke(17, 4, "ArchPaladin Armor & PreReqs");

        if (Core.CheckInventory("Drakath the Eternal"))
            Core.EnsureComplete(8741);
        RequirementCompleted?.Invoke(17, 5, "Hero's Valiance Completed");
        Core.Logger("Enhancement Unlocked: Hero's Valiance");
    }

    public void DoDauntless(bool useGold, bool useInsignDaunt)
    {
        if (Core.isCompletedBefore(9172)) return;
        Core.Logger("Unlocking Enhancement: Dauntless");

        Farm.BlacksmithingREP(10, useGold);
        string[] dauntlessItems = { "ShadowLord's Helm", "Malgor the ShadowLord", "Malgor's ShadowFlame Blade", "Infernal Flame Pyromancer" };

        Core.EnsureAccept(9172);

        if (!Core.CheckInventory(dauntlessItems))
        {
            SoW.CompleteCoreSoW();
            MAS.GetSet();

            if (!Core.CheckInventory("Malgor's ShadowFlame Blade"))
            {
                SOWM.ElementalCore(20);
                Core.HuntMonster("shadowgrove", "Titan Shadow Dragonlord", "ShadowFlame Dragon Blade", isTemp: false);
            }

            if (!Core.CheckInventory("Infernal Flame Pyromancer"))
            {
                SoW.Tyndarius();
                Core.AddDrop("Fire Avatar's Favor");
                Core.RegisterQuests(8244);
                while (!Bot.ShouldExit && !Core.CheckInventory("Fire Avatar's Favor", 75))
                {
                    Core.KillMonster("fireavatar", "r4", "Right", "*", "Onslaught Defeated", 6);
                    Core.KillMonster("fireavatar", "r6", "Left", "*", "Elemental Defeated", 6);
                    Bot.Wait.ForPickup("Fire Avatar's Favor");
                }
                Core.CancelRegisteredQuests();
            }

            if (useInsignDaunt)
            {
                Core.Unbank("Malgor Insignia", "Avatar Tyndarius Insignia");
                if (!Core.CheckInventory("Malgor's ShadowFlame Blade") && Core.CheckInventory("Malgor Insignia", 20))
                    Adv.BuyItem("ultraspeaker", 2248, "Malgor's ShadowFlame Blade");
                if (!Core.CheckInventory("Infernal Flame Pyromancer") && Core.CheckInventory("Avatar Tyndarius Insignia", 20))
                    Adv.BuyItem("fireavatar", 2038, "Infernal Flame Pyromancer");
            }
        }
        RequirementCompleted?.Invoke(18, 1, "Dauntless Pre-items");

        if (Core.CheckInventory(dauntlessItems) &&
            Core.CheckInventory("Malgor Insignia", 5) &&
            Core.CheckInventory("Avatar Tyndarius Insignia", 10))
        {
            Core.ChainComplete(9172);
            RequirementCompleted?.Invoke(18, 2, "Dauntless Completed");
            Core.Logger("Enhancement Unlocked: Dauntless");
        }
        else
        {
            Core.Logger("Missing required insignias or items for Dauntless. Abandoning quest.");
            Core.AbandonQuest(9172);
        }
    }

    public void DoRavenous()
    {
        if (Core.isCompletedBefore(9560)) return;
        Core.Logger("Unlocking Enhancement: Ravenous");

        Farm.Experience(100);
        PFS.Storyline();

        Adv.BuyItem(Bot.Map.Name, 2411, "Gluttonous Maw");
        RequirementCompleted?.Invoke(19, 1, "Gluttonous Maw");

        if (Core.CheckInventory("Gluttonous Maw"))
        {
            Core.ChainComplete(9560);
            RequirementCompleted?.Invoke(19, 2, "Ravenous Completed");
            Core.Logger("Enhancement Unlocked: Ravenous");
        }
        else
        {
            Core.Logger("Cannot complete Ravenous: Requires Gluttonous Maw (needs 10 Roentgeniums).");
        }
    }

    public void DoArcanasConcerto(bool useGold, bool useInsignConcerto)
    {
        if (Core.isCompletedBefore(8742)) return;
        Core.Logger("Unlocking Enhancement: Arcana's Concerto");

        Astravia.CompleteCoreAstravia();
        Farm.Experience(100);
        Farm.BlacksmithingREP(10, useGold);

        if (!Core.isCompletedBefore(8746))
        {
            Core.Logger("You must complete weekly quest 8746 for Arcana's Concerto.");
            return;
        }

        PDPPR.FarmPreReqs(useInsignConcerto);
        RequirementCompleted?.Invoke(20, 1, "Prince Darkon's Poleaxe PreReqs");

        if (!Core.CheckInventory("Prince Darkon's Poleaxe") || !Core.CheckInventory("Darkon's Debris 2 (Reconstructed)"))
        {
            if (useInsignConcerto && Core.CheckInventory("Darkon Insignia", 20))
                Adv.BuyItem("ultradarkon", 2147, "Darkon's Debris 2 (Reconstructed)");
        }
        RequirementCompleted?.Invoke(20, 2, "Arcana's Concerto Gear");

        if (Core.CheckInventory("Prince Darkon's Poleaxe") &&
            Core.CheckInventory("Darkon's Debris 2 (Reconstructed)") &&
            Core.CheckInventory("King Drago Insignia", 5) &&
            Core.CheckInventory("Darkon Insignia", 5))
        {
            Core.ChainComplete(8742);
            RequirementCompleted?.Invoke(20, 3, "Arcana's Concerto Completed");
            Core.Logger("Enhancement Unlocked: Arcana's Concerto");
        }
        else
        {
            Core.Logger("Arcana's Concerto requirements (items or insignias) not met.");
        }
    }

    #endregion
}
