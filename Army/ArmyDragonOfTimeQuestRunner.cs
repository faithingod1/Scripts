/*
name: DragonOfTime
description: This bot farms the Dragon of Time class for you.
tags: class, dot, damage over time, dragon of time, timeinn
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class ArmyDragonOfTimeQuestRunner
{
    public System.Action<int, int, string>? RequirementCompleted { get; set; }
    public System.Action<string, System.Action>? SoloRequirement { get; set; }
    private void RunSolo(string map, System.Action farm)
    {
        if (SoloRequirement != null) SoloRequirement(map, farm);
        else farm();
    }

    private void ArmyGoldenBlade()
    {
        var quests = new (int id, string name, System.Action farm)[]
        {
            (5669, "Horc Tutor Trainer", () => Story.KillQuest(5669, "tutor", "Horc Tutor Trainer")),
            (5670, "Piggy Drake", () => Story.KillQuest(5670, "prison", "Piggy Drake")),
            (5671, "Phedra", () => Story.KillQuest(5671, "lavarun", "Phedra")),
            (5672, "Chaorrupted Armor", () => Story.KillQuest(5672, "chaoscrypt", "Chaorrupted Armor")),
            (5673, "Sketchy Frogzard", () => Story.KillQuest(5673, "j6", "Sketchy Frogzard")),
            (5674, "Orcpath map items", () => Story.MapItemQuest(5674, "orcpath", 5143, 3)),
            (5675, "Red Dragon", () => Story.KillQuest(5675, "lair", "Red Dragon")),
            (5676, "Well map item", () => Story.MapItemQuest(5676, "well", 5144)),
            (5677, "GreenRat", () => Story.KillQuest(5677, "cellar", "GreenRat")),
        };
        bool owned = Core.CheckInventory("Golden Blade of Fate");
        bool chainDone = owned || Core.isCompletedBefore(5679, log: false);
        // Everyone visits every checkpoint, including accounts which already own the blade.
        for (int i = 0; i < quests.Length; i++)
        {
            if (Bot.ShouldExit) return;
            if (!chainDone && !Core.isCompletedBefore(quests[i].id, log: false)) quests[i].farm();
            RequirementCompleted?.Invoke(7718, i + 1, "Golden Blade: " + quests[i].name);
        }
        if (!chainDone && !Core.isCompletedBefore(5678, log: false))
        {
            Core.EnsureAccept(5678);
            if (!Core.CheckInventory(38354)) Core.HuntMonster("sepulchure", "Dark Sepulchure", "Dark Spirit");
        }
        RequirementCompleted?.Invoke(7718, 10, "Golden Blade: Dark Spirit");
        if (!chainDone && !Core.isCompletedBefore(5678, log: false)) Core.EnsureComplete(5678);
        if (!owned)
        {
            Core.AddDrop("Golden Blade of Fate");
            Core.EnsureAccept(5679);
            Core.GetMapItem(5145, 1, "yulgar");
        }
        RequirementCompleted?.Invoke(7718, 11, "Golden Blade: Yulgar map item");
        if (!owned)
        {
            Core.EnsureComplete(5679);
            Bot.Wait.ForPickup("Golden Blade of Fate");
        }
    }

    private void ArmyDoomwoodPart3()
    {
        bool owned = Core.CheckInventory("Pink Blade of Destruction");
        bool chainDone = owned || Core.isCompletedBefore(7653, log: false);
var quests = new (int id, string name, System.Action farm)[]
        {
            (7589, "Bullies", () => { Core.EnsureAccept(7589); Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Slain", 4); Core.HuntMonster("thorngarde", "NecroDrone", "NecroDrone Slain", 1); Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Slain", 1); Core.EnsureComplete(7589); }),
            (7590, "Free the People", () => { Core.EnsureAccept(7590); Core.GetMapItem(7485, 5, "thorngarde"); Core.HuntMonster("thorngarde", "NecroMech", "Deadtech Key", 5); Core.EnsureComplete(7590); }),
            (7591, "Sample It", () => { Story.KillQuest(7591, "thorngarde", "NecroMech"); }),
            (7592, "Swat the Flies", () => { Story.KillQuest(7592, "thorngarde", "CryptHacker"); }),
            (7593, "Reclaim the Supplies", () => { Core.EnsureAccept(7593); Core.HuntMonster("thorngarde", "CryptHacker", "Cured Meat", 3); Core.HuntMonster("thorngarde", "NecroMech", "Bag of Grain", 3); Core.HuntMonster("thorngarde", "NecroDrone", "Jug of Water", 5); Core.EnsureComplete(7593); }),
            (7594, "Who's That?", () => { Story.MapItemQuest(7594, "thorngarde", 7486); }),
            (7595, "Find my Stuff", () => { Core.EnsureAccept(7595); Core.GetMapItems(new[] { (7487, 1), (7488, 1) }, "thorngarde"); Core.HuntMonster("thorngarde", "NecroDrone", "Kyger's Bunny", 1); Core.EnsureComplete(7595); }),
            (7596, "Save my Friends", () => { Core.EnsureAccept(7596); Core.GetMapItems(new[] { (7489, 1), (7490, 1), (7491, 1) }, "thorngarde"); Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Slain", 3); Core.EnsureComplete(7596); }),
            (7597, "Pull the Wires", () => { Story.KillQuest(7597, "thorngarde", "CryptHacker"); }),
            (7598, "Dress-up Time!", () => { Core.EnsureAccept(7598); Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Headgear", 1); Core.HuntMonster("thorngarde", "NecroDrone", "Deadtech Pieces", 5); Core.EnsureComplete(7598); }),
            (7599, "I Need a Distraction", () => { Core.EnsureAccept(7599); Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Slain", 5); Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Slain", 5); Core.HuntMonster("thorngarde", "NecroDrone", "NecroDrone Slain", 5); Core.EnsureComplete(7599); }),
            (7600, "Zyrus is Lost", () => { Story.KillQuest(7600, "thorngarde", "Zyrus the BioKnight"); }),
            (7601, "Investigate the Tech", () => { Core.EnsureAccept(7601); Core.HuntMonster("thorngarde", "NecroDrone", "Deadtech Power Core", 7); Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Targeting Systems", 5); Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Circuitry", 15); Core.HuntMonster("thorngarde", "Zyrus the BioKnight", "BioKnight Engine", 3); Core.EnsureComplete(7601); }),
            (7603, "Defeat the Deadtech", () => { Story.KillQuest(7603, "stonewood", "NecroDrone"); }),
            (7604, "Gather Spell Components", () => { Core.EnsureAccept(7604); Core.GetMapItem(7510, 6, "stonewood"); Core.HuntMonster("stonewood", "Doomwood Treeant", "Treeant Pollen", 8); Core.EnsureComplete(7604); }),
            (7605, "A Few Things More", () => { Core.EnsureAccept(7605); Core.HuntMonster("stonewood", "NecroDrone", "Fresh Marrow", 8); Core.HuntMonster("stonewood", "NecroMech", "Pristine Wishbone", 1); Core.EnsureComplete(7605); }),
            (7606, "Search for the Axe", () => { Story.MapItemQuest(7606, "stonewood", 7511, 4); Story.MapItemQuest(7606, "stonewood", 7512); }),
            (7607, "Defeat the BioKnight", () => { Story.KillQuest(7607, "stonewood", "BioKnight"); }),
            (7608, "Take the Axe!", () => { Story.MapItemQuest(7608, "stonewood", 7513); }),
            (7609, "Find the Keys", () => { Story.KillQuest(7609, "techdungeon", "NecroMech"); }),
            (7610, "Ew, Rats", () => { Core.EnsureAccept(7610); Core.GetMapItem(7515, 6, "techdungeon"); Core.HuntMonster("techdungeon", "Rotting Rat", "Rat Slain", 10); Core.EnsureComplete(7610); }),
            (7611, "Grab Their Gear", () => { Core.EnsureAccept(7611); Core.KillMonster("techdungeon", "r4", "Left", "DoomBorg Guard", "Guard Armor Piece", 5); Core.KillMonster("techdungeon", "r4", "Left", "DoomBorg Guard", "Guard Helm"); Core.KillMonster("techdungeon", "r4", "Left", "DoomBorg Guard", "Deadtech Polearm"); Core.EnsureComplete(7611); }),
            (7612, "Talk to the Guard", () => { Story.MapItemQuest(7612, "techdungeon", 7514); }),
            (7613, "Let Them Out!", () => { Story.MapItemQuest(7613, "techdungeon", 7516); }),
            (7614, "Head on Out", () => { Core.EnsureAccept(7614); Core.GetMapItem(7517, 1, "techdungeon"); Core.HuntMonster("techdungeon", "DoomBorg Guard", "Deadtech Guard Slain", 12); Core.EnsureComplete(7614); }),
            (7615, "Take Him Down", () => { Story.KillQuest(7615, "techdungeon", "Kalron the Cryptborg"); }),
            (7623, "Cat Ears?", () => { Core.EnsureAccept(7623); Core.GetMapItem(7528, 1, "stonewooddeep"); Core.EnsureComplete(7623); Core.Logger("Cutscene > Wait > Jump > Continue"); Bot.Wait.ForCellChange("Cut2"); Core.Sleep(2500); Core.JumpWait(); }),
            (7624, "EN GARDE!", () => { Core.EquipClass(ClassType.Solo); Core.EnsureAccept(7624); Core.KillMonster("stonewooddeep", "r2", "Right", "Asherion", "Defeat Asherion"); Core.EnsureComplete(7624); }),
            (7625, "The Light Of Destiny", () => { Core.EquipClass(ClassType.Farm); Core.EnsureAccept(7625); Core.GetMapItem(7529, 1, "stonewooddeep"); Core.EnsureComplete(7625); Core.Logger("Cutscene > Wait > Jump > Continue"); Bot.Wait.ForCellChange("Cut2"); Core.Sleep(2500); Core.JumpWait(); }),
            (7626, "Stuff for Dummies", () => { Core.EnsureAccept(7626); Core.KillMonster("stonewooddeep", "r2", "Right", "Doomwood Treeant", "Sturdy Wood", 8); Story.MapItemQuest(7626, "stonewooddeep", 7530, 8); }),
            (7627, "Build the Dummies", () => { Core.EnsureAccept(7627); Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Treeant", "Area Cleared", 10); Core.GetMapItem(7531, 6, "stonewooddeep"); Core.EnsureComplete(7627); Core.Jump("r2"); Core.Sleep(2500); Core.Jump("r3"); }),
            (7628, "Battle the Dummies", () => { Core.EnsureAccept(7628); Core.KillMonster("stonewooddeep", "r3", "Left", "Target Dummy", "Target Dummy Slain", 6); Core.EnsureComplete(7628); }),
            (7629, "Lesson 1: Bravery", () => { Core.EnsureAccept(7629); Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Slime", "Slime Slain", 10); Core.EnsureComplete(7629); }),
            (7630, "Lesson 2: Armor", () => { Core.EnsureAccept(7630); Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Slime", "Stolen Armor", 7); Core.EnsureComplete(7630); }),
            (7631, "Lesson 3: Protection", () => { Core.EnsureAccept(7631); Core.KillMonster("stonewooddeep", "r7", "Left", "CryptHacker", "Crypthacker Slain", 10); Core.KillMonster("stonewooddeep", "r7", "Left", "CryptHacker", "Unidentified Clue"); Core.EnsureComplete(7631); }),
            (7632, "Lesson 4: Gather Intelligence", () => { Story.KillQuest(7632, "stonewooddeep", "CryptHacker"); }),
            (7633, "Lesson 5: Demolition", () => { Core.EnsureAccept(7633); Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Slime", "Acid Ooze", 6); Core.KillMonster("stonewooddeep", "r7", "Right", "NecroDrone", "Explosive Tech", 6); Core.EnsureComplete(7633); }),
            (7634, "Lesson 6: Searching", () => { Story.MapItemQuest(7634, "stonewooddeep", 7532); }),
            (7635, "Get the Axe", () => { Story.MapItemQuest(7635, "stonewooddeep", 7533); Core.Logger("Cutscene > Wait > Jump > Continue"); Bot.Wait.ForCellChange("Cut2"); Core.Sleep(2500); Core.JumpWait(); }),
            (7636, "Never Give Up", () => { Core.EquipClass(ClassType.Solo); Story.KillQuest(7636, "stonewooddeep", "Sir Kut"); }),
            (7637, "I CPU U!", () => { Story.KillQuest(7637, "techfortress", "CryptHacker"); }),
            (7638, "Keep Droning On", () => { Story.KillQuest(7638, "techfortress", "NecroDrone"); }),
            (7639, "Gathering Data", () => { Story.KillQuest(7639, "techfortress", "CryptHacker"); }),
            (7640, "Signal Flare", () => { Core.EnsureAccept(7640); Core.GetMapItem(7561, 1, "techfortress"); Core.HuntMonster("techfortress", "NecroDrone", "Flare", 1); Core.EnsureComplete(7640); }),
            (7641, "Medal Time", () => { Core.EnsureAccept(7641); Core.HuntMonster("techfortress", "NecroDrone", "NecroMedals", 5); Core.EnsureComplete(7641); }),
            (7642, "Hidden Data", () => { Story.KillQuest(7642, "techfortress", "NecroMech"); }),
            (7643, "Pass the CPU", () => { Story.KillQuest(7643, "techfortress", "CPU"); }),
            (7644, "The Guards", () => { Story.KillQuest(7644, "techfortress", "DoomBorg Guard"); }),
            (7645, "Circui-try This", () => { Story.KillQuest(7645, "techfortress", "DoomBorg Guard"); }),
            (7646, "The Thing", () => { Story.KillQuest(7646, "techfortress", "Vortrix"); }),
            (7653, "The Final Form", () => { Story.KillQuest(7653, "techfortress", "MechaVortrix"); }),
        };
        for (int i = 0; i < quests.Length; i++)
        {
            if (Bot.ShouldExit) return;
            if (!chainDone && !Core.isCompletedBefore(quests[i].id, log: false)) quests[i].farm();
            RequirementCompleted?.Invoke(7718, 10000 + i + 1, "Doomwood: " + quests[i].name);
        }
    }

    private void ArmyMineCrafting()
    {
        bool owned = Core.CheckInventory("Pink Blade of Destruction");
        bool chainDone = owned || Core.isCompletedBefore(2084, log: false);
        var blod = new CoreBLOD();
        var quests = new (int id, string name, System.Action farm)[]
        {
            (2066, "Reforging the Blinding Light", () => Story.BuyQuest(2066, "doomwood", 276, "Blinding Light of Destiny Handle")),
            (2067, "Secret Order of Undead Slayers", () => Story.BuyQuest(2067, "doomwood", 276, "Bonegrinder Medal")),
            (2082, "Essential Essences", () => { Core.EnsureAccept(2082); Core.HuntMonster("battleunderb", "Skeleton Warrior", "Undead Essence", 25, isTemp: false); Core.EnsureComplete(2082); }),
            (2083, "Bust some Dust", () => { Core.EnsureAccept(2083); Core.HuntMonster("battleunderb", "Skeleton Warrior", "Bone Dust", 40, isTemp: false); Core.EnsureComplete(2083); }),
            (2084, "A Loyal Follower", () => { Core.EnsureAccept(2084); blod.SpiritOrb(100); Core.HuntMonster("timevoid", "Ephemerite", "Celestial Compass"); Core.EnsureComplete(2084); }),
        };
        for (int i = 0; i < quests.Length; i++)
        {
            if (Bot.ShouldExit) return;
            if (!chainDone && !Core.isCompletedBefore(quests[i].id, log: false)) quests[i].farm();
            RequirementCompleted?.Invoke(7718, 10060 + i + 1, "MineCrafting: " + quests[i].name);
        }
    }

    private void ArmyPinkBlade()
    {
        bool owned = Core.CheckInventory("Pink Blade of Destruction");
        ArmyDoomwoodPart3();
        ArmyMineCrafting();
        var blod = new CoreBLOD();
        for (int so = 100; so <= 500 && !Bot.ShouldExit; so += 100)
        {
            if (!owned && !Core.CheckInventory("Spirit Orb", so))
                blod.SpiritOrb(so);
            RequirementCompleted?.Invoke(7718, 10070 + (so / 100), $"Spirit Orbs: {so}/500");
        }
        if (!owned)
        {
            Core.AddDrop("Fuchsia Dye", "Zealous Badge", "Pink Blade of Destruction");
            Core.EnsureAccept(7650);
        }
        // Fixed cumulative targets keep clients aligned even with different starting stacks.
        for (int target = 1; target <= 50 && !Bot.ShouldExit; target++)
        {
            bool need = !owned && !Core.CheckInventory("Fuchsia Dye", target);
            if (need) { Core.EnsureAccept(1487); Core.HuntMonster("natatorium", "Anglerfish", "Pink Coral", 3, log: false); }
            RequirementCompleted?.Invoke(7718, 10100 + target * 3, $"Fuchsia Dye {target}/50: Pink Coral");
            if (need) Core.HuntMonster("bloodtuskwar", "Chaotic Vulture", "Amaranth Flower", 5, log: false);
            RequirementCompleted?.Invoke(7718, 10101 + target * 3, $"Fuchsia Dye {target}/50: Amaranth Flower");
            if (need) { Core.EnsureComplete(1487); Bot.Wait.ForPickup("Fuchsia Dye"); }
            RequirementCompleted?.Invoke(7718, 10102 + target * 3, $"Fuchsia Dye {target}/50");
        }
        for (int target = 1; target <= 5 && !Bot.ShouldExit; target++)
        {
            bool need = !owned && !Core.CheckInventory("Zealous Badge", target);
            if (need)
            {
                Core.EnsureAccept(7616);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("techdungeon", "Kalron the Cryptborg", "Immutable Dedication", 7, log: false);
            }
            RequirementCompleted?.Invoke(7718, 11100 + target * 3, $"Zealous Badge {target}/5: Immutable Dedication");
            if (need)
            {
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("techdungeon", "DoomBorg Guard", "Paladin Armor Scraps", 30, log: false);
            }
            RequirementCompleted?.Invoke(7718, 11101 + target * 3, $"Zealous Badge {target}/5: Paladin Armor Scraps");
            if (need) { Core.EnsureComplete(7616); Bot.Wait.ForPickup("Zealous Badge"); }
            RequirementCompleted?.Invoke(7718, 11102 + target * 3, $"Zealous Badge {target}/5");
        }
        if (!owned)
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("undergroundlabb", "Ultra BrutalCorn", "Unicorn Essence", 5, false, false);
        }
        RequirementCompleted?.Invoke(7718, 12000, "Pink Blade: Unicorn Essence");
        if (!owned) Core.HuntMonster("undergroundlabb", "Ultra Battle Gem", "Gem Power", 5, false, false);
        RequirementCompleted?.Invoke(7718, 12001, "Pink Blade: Gem Power");
        if (!owned) { Core.EnsureComplete(7650, 55884); Bot.Wait.ForPickup("Pink Blade of Destruction"); }
    }

    private void ArmyEgg()
    {
        bool owned = Core.CheckInventory("Mysterious Egg");
        if (!owned) { Core.AddDrop("Mysterious Egg"); Core.EnsureAccept(6171); }
        var keys = new (string item, System.Action farm)[]
        {
            ("Key of Pride", () => Core.KillMonster("pride", "r13", "Left", "Valsarian", "Key of Pride", isTemp: false)),
            ("Key of Gluttony", () => Core.KillMonster("gluttony", "Enter2", "Top", "Deflated Glutus", "Key of Gluttony", isTemp: false)),
            ("Key of Greed", () => Core.KillMonster("greed", "r16", "Left", "Goregold", "Key of Greed", isTemp: false)),
            ("Key of Sloth", () => { new Core7DD().HazMatSuit(); Core.HuntMonster("sloth", "Phlegnn", "Key of Sloth", isTemp: false); }),
            ("Key of Lust", () => Core.HuntMonster("lust", "Lascivia", "Key of Lust", isTemp: false)),
            ("Key of Envy", () => { Bot.Quests.UpdateQuest(6000); Core.HuntMonster("maloth", "Maloth", "Key of Envy", isTemp: false); }),
            ("Key of Wrath", () => Core.HuntMonster("wrath", "Gorgorath", "Key of Wrath", isTemp: false)),
        };
        for (int i = 0; i < keys.Length; i++)
        {
            if (Bot.ShouldExit) return;
            if (!owned && !Core.CheckInventory(keys[i].item)) keys[i].farm();
            RequirementCompleted?.Invoke(7724, i + 1, keys[i].item);
        }
        if (!owned) { Core.EnsureComplete(6171, 42497); Bot.Wait.ForPickup("Mysterious Egg"); }
    }
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;
    private static GoldenBladeOfFate GBoF
    {
        get => _GBoF ??= new GoldenBladeOfFate();
        set => _GBoF = value;
    }
    private static GoldenBladeOfFate _GBoF;
    private static PinkBladeOfDestruciton PBoD
    {
        get => _PBoD ??= new PinkBladeOfDestruciton();
        set => _PBoD = value;
    }
    private static PinkBladeOfDestruciton _PBoD;
    private static CoreQOM QOM
    {
        get => _QOM ??= new CoreQOM();
        set => _QOM = value;
    }
    private static CoreQOM _QOM;
    private static CoreToD TOD
    {
        get => _TOD ??= new CoreToD();
        set => _TOD = value;
    }
    private static CoreToD _TOD;
    private static MysteriousEgg Egg
    {
        get => _Egg ??= new MysteriousEgg();
        set => _Egg = value;
    }
    private static MysteriousEgg _Egg;
    private static CoreSummer Coll
    {
        get => _Coll ??= new CoreSummer();
        set => _Coll = value;
    }
    private static CoreSummer _Coll;
    private static Borgars Borgars
    {
        get => _Borgars ??= new Borgars();
        set => _Borgars = value;
    }
    private static Borgars _Borgars;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetDoT();

        Core.SetOptions(false);
    }

    private string[] Extras =
    {
        "Dragon of Time Horns",
        "Dragon of Time Horns + Ponytail",
        "Dragon of Time Wings + Tail",
    };

    public void GetDoT(bool rankUpClass = true, bool doExtra = true, bool DotArmorOnly = false, int stopAfterQuestId = 0)
    {
        if (doExtra ? Core.CheckInventory(Extras) : Core.CheckInventory("Dragon of Time"))
            return;

        Story.PreLoad(this);

        // Acquiring Ancient Secrets 7716
        if (!Story.QuestProgression(7716))
        {
            Core.EnsureAccept(7716);

            Core.EquipClass(ClassType.Farm);
            Bot.Quests.UpdateQuest(4614);
            Core.KillMonster("mummies", "Enter", "Spawn", "Mummy", "Lost Hieroglyphic", 30, false);
            RequirementCompleted?.Invoke(7716, 10000, "Lost Hieroglyphic" );

            Farm.LoremasterREP(4);
            RequirementCompleted?.Invoke(7716, 20000, "Farm.LoremasterREP" );

            Core.BuyItem("librarium", 651, "Myths of Lore");
            RequirementCompleted?.Invoke(7716, 30000, "Myths of Lore" );

            Core.KillMonster("timelibrary", "FrameAQ", "Left", "*", "Historia Page", 100, false);
            RequirementCompleted?.Invoke(7716, 40000, "Historia Page" );

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("kingcoal", "Frost King", "Frost King's Story", isTemp: false);
            RequirementCompleted?.Invoke(7716, 50000, "Frost King's Story" );

            RunSolo("baconcatyou", () => Core.KillMonster("baconcatyou", "Enter", "Spawn", "*", "Your Own Memories", isTemp: false));
            RequirementCompleted?.Invoke(7716, 60000, "Your Own Memories" );
            Story.ChainQuest(7716);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Helm", "Dragon of Time Ponytail");
        }

        if (stopAfterQuestId == 7716) return;
        // Time to Train Yourself 7717
        if (!Story.QuestProgression(7717))
        {
            Core.EnsureAccept(7717);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("dragonchallenge", "Desoloth the Final", "Desoloth's Destructive Aura", isTemp: false);
            RequirementCompleted?.Invoke(7717, 10000, "Desoloth's Destructive Aura" );

            Bot.Quests.UpdateQuest(899);
            Core.HuntMonster("blindingsnow", "Nythera", "Nythera's Patience", isTemp: false);
            RequirementCompleted?.Invoke(7717, 20000, "Nythera's Patience" );

            Core.AddDrop("Key of Greed");
            Core.HuntMonster("greed", "Goregold", "Goregold's Luck", isTemp: false);
            RequirementCompleted?.Invoke(7717, 30000, "Goregold's Luck" );
            Core.HuntMonster("darkplane", "Victorious", "Victorious's Dignity", isTemp: false);
            RequirementCompleted?.Invoke(7717, 40000, "Victorious's Dignity" );

            Core.KillTrigoras("Trigoras's Tenacity", 3, Phase: 2);
            RequirementCompleted?.Invoke(7717, 50000, "Trigoras's Tenacity" );

            Story.ChainQuest(7717);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Runes", "Dragon of Time Wings", "Dragon of Time Tail");
        }

        if (stopAfterQuestId == 7717) return;
        // Do You Have the Time? 7718
        if (!Story.QuestProgression(7718))
        {
            Farm.Experience(31);
            Core.EnsureAccept(7718);

            ArmyGoldenBlade();
            RequirementCompleted?.Invoke(7718, 10000, "GBoF.GetGBoF" );

            ArmyPinkBlade();
            RequirementCompleted?.Invoke(7718, 20000, "PBoD.GetPBoD" );

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("underworld", "Laken", "Cross-Era Stabilizer", isTemp: false);
            RequirementCompleted?.Invoke(7718, 30000, "Cross-Era Stabilizer" );

            if (!Core.CheckInventory("Chronomancer's Codex"))
            {
                if (!Core.CheckInventory("Dragonoid of Hours"))
                    Core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false);
            }
            RequirementCompleted?.Invoke(7718, 35000, "Dragonoid of Hours" );

            if (!Core.CheckInventory("Chronomancer's Codex"))
                Core.HuntMonster("timespace", "Chaos Lord Iadoa", "Chronomancer's Codex", isTemp: false);
            RequirementCompleted?.Invoke(7718, 40000, "Chronomancer's Codex" );

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("arena", "Timestream Rider", "Timestream String", 100, false);
            RequirementCompleted?.Invoke(7718, 50000, "Timestream String" );

            Story.ChainQuest(7718);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time FangBlade", "Dual Dragon of Time FangBlades");
        }

        if (stopAfterQuestId == 7718) return;
        // Through the Wormhole 7719
        if (!Story.QuestProgression(7719))
        {
            Farm.Experience(40);
            Core.EnsureAccept(7719);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("cathedral", "Incarnation of Time", "Time Loop Broken", isTemp: false);
            RequirementCompleted?.Invoke(7719, 10000, "Time Loop Broken" );

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("portalwar", "r4", "Right", "*", "Anomaly Silenced", 100, false);
            RequirementCompleted?.Invoke(7719, 20000, "Anomaly Silenced" );

            Core.HuntMonster("portalmaze", "ChronoLord", "Chronolord Stopped", 50, false);
            RequirementCompleted?.Invoke(7719, 30000, "Chronolord Stopped" );

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("ubear", "Cornholio", "Is This a Wormhole?", isTemp: false);
            RequirementCompleted?.Invoke(7719, 40000, "Is This a Wormhole?" );

            Core.EnsureComplete(7719);
        }

        if (stopAfterQuestId == 7719) return;
        // If we are only getting the armor, we can return here.
        if (DotArmorOnly)
            return;

        // Rend 7720
        if (!Story.QuestProgression(7720))
        {
            Farm.Experience(60);
            Core.EnsureAccept(7720);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("lairdefend", "Dragon Summoner", "Dimensional Dragon Portal", 2, false);
            RequirementCompleted?.Invoke(7720, 10000, "Dimensional Dragon Portal" );
            Core.HuntMonster("bosschallenge", "Grievous Inbunche", "Brutal Slash Studied", 10, isTemp: false);
            RequirementCompleted?.Invoke(7720, 20000, "Brutal Slash Studied" );
            Core.KillMonster("hydrachallenge", "h90", "Left", "*", "Epic Hydra Fang", 125, false);
            RequirementCompleted?.Invoke(7720, 30000, "Epic Hydra Fang" );

            Story.ChainQuest(7720);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Daggers", "Dragon of Time Cleaver");
        }

        if (stopAfterQuestId == 7720) return;
        // Confluence of Fates 7721
        if (!Story.QuestProgression(7721))
        {
            Farm.Experience(60);
            Core.EnsureAccept(7721);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("ivoliss", "Ivoliss", "Sword of Voids", isTemp: false);
            Bot.Wait.ForPickup("Sword of Voids");
            RequirementCompleted?.Invoke(7721, 10000, "Sword of Voids" );

            for (int r = 25; r <= 100 && !Bot.ShouldExit; r += 25)
            {
                if (!Core.CheckInventory("Darkon's Receipt", r))
                    Darkon.FarmReceipt(r);
                RequirementCompleted?.Invoke(7721, 10000 + (r * 100), $"Darkon's Receipt: {r}/100");
            }

            QOM.TheReshaper();
            RequirementCompleted?.Invoke(7721, 30000, "QOM.TheReshaper" );
            if (!Core.CheckInventory("Semiramis Feather"))
            {
                Core.AddDrop("Semiramis Feather");
                // Take Down Terrane 6286
                Core.EquipClass(ClassType.Solo);
                Core.EnsureAccept(6286);
                Core.KillMonster("guardiantree", "r12", "Left", "Terrane");
                Core.EnsureComplete(6286);
                Bot.Wait.ForPickup("Semiramis Feather");
            }
            RequirementCompleted?.Invoke(7721, 40000, "Semiramis Feather" );

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("aqw3d", "r13", "Bottom", "*", "Cross-Dimensional Weapons", 300, isTemp: false);
            RequirementCompleted?.Invoke(7721, 50000, "Cross-Dimensional Weapons" );
            TOD.ShiftingPyramid();
            RequirementCompleted?.Invoke(7721, 60000, "TOD.ShiftingPyramid" );
            if (!Core.CheckInventory("Starlight Singularity"))
            {
                Core.AddDrop("Starlight Singularity");
                // Serpent of the Stars 5186
                Core.EnsureAccept(5186);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("whitehole", "Mehensi Serpent", "Mehen Slain");
                Core.EnsureComplete(5186);
                Bot.Wait.ForPickup("Starlight Singularity");
            }
            RequirementCompleted?.Invoke(7721, 70000, "Starlight Singularity" );

            Coll.Collector();
            RequirementCompleted?.Invoke(7721, 80000, "Coll.Collector" );
            Adv.BuyItem("collection", 325, 56815, shopItemID: 6448);
            RequirementCompleted?.Invoke(7721, 90000, "Adv.BuyItem" );
            Story.ChainQuest(7721);

            Bot.Wait.ForPickup("*");
            Core.ToBank("Ascended Dragon of Time Runes", "Runes Of Time");
        }

        if (stopAfterQuestId == 7721) return;
        // Dragon's Will 7722
        if (!Story.QuestProgression(7722))
        {
            Farm.Experience(65);
            Core.EnsureAccept(7722);

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("moonlab", "Slime Mold", "Unyielding Slime", 300, false);
            RequirementCompleted?.Invoke(7722, 10000, "Unyielding Slime" );

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("bosschallenge", "Mutated Void Dragon", "Omnipotent Cells", 20, false);
            RequirementCompleted?.Invoke(7722, 20000, "Omnipotent Cells" );
            if (!Core.CheckInventory("Dragon's Plasma", 20))
            {
                //why the fuck was the class buffed!?
                InventoryItem? usethis = Bot.Inventory.Items.Concat(Bot.Bank.Items).FirstOrDefault(n => n.Name.Equals("Yami no Ronin") || n.Name.StartsWith("Chaos Slayer"));
                if (usethis != null)
                {
                    Core.Equip(usethis.ID);
                    Core.Equip(Core.FarmGear);
                    if (usethis.Name.Equals("Yami no Ronin"))
                        Bot.Skills.StartAdvanced("1 | 4");
                }
                else
                    Core.EquipClass(ClassType.Dodge);
                Core.HuntMonster("underlair", "ArchFiend Dragonlord", "Dragon's Plasma", 20, false);
            }
            RequirementCompleted?.Invoke(7722, 30000, "Dragon's Plasma" );
            Core.JumpWait();
            Core.EquipClass(ClassType.Solo);

            Core.HuntMonster("chaoskraken", "Chaos Kraken", "Chaotic Invertebrae", 20, false);
            RequirementCompleted?.Invoke(7722, 40000, "Chaotic Invertebrae" );

            Bot.Quests.UpdateQuest(9, 159);
            Core.HuntMonster("towerofdoom9", "Dread Fang", "Cryostatic Essence", 20, false);
            RequirementCompleted?.Invoke(7722, 50000, "Cryostatic Essence" );

            Core.HuntMonster("castleroof", "Ultra Chaos Dragon", "Salvaged Chaos Dragon Biomass", 20, false);
            RequirementCompleted?.Invoke(7722, 60000, "Salvaged Chaos Dragon Biomass" );
            Story.ChainQuest(7722);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Reaper", "Dragon of Time WingBlade");
        }

        if (stopAfterQuestId == 7722) return;
        // Burning Fates 7723
        if (!Story.QuestProgression(7723))
        {
            Farm.Experience(70);
            Core.EnsureAccept(7723);

            Core.EquipClass(ClassType.Farm);
            for (int fe = 500; fe <= 3000 && !Bot.ShouldExit; fe += 500)
            {
                if (!Core.CheckInventory("Fire Essence", fe))
                    Core.KillMonster("volcano", "r10", "Left", "Fire Imp", "Fire Essence", fe, false);
                RequirementCompleted?.Invoke(7723, fe, $"Fire Essence: {fe}/3000");
            }
            RequirementCompleted?.Invoke(7723, 10000, "Fire Essence (complete)");

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("charredplains", "Akriloth", "Akriloth's Flametongue", 100, false);
            RequirementCompleted?.Invoke(7723, 20000, "Akriloth's Flametongue" );
            Core.HuntMonster("ultraphedra", "Ultra Phedra", "Immortal Embers", 50, false);
            RequirementCompleted?.Invoke(7723, 30000, "Immortal Embers" );
            Core.HuntMonster("thevoid", "Reaper", "Ashes from the Void Realm", 50, false);
            RequirementCompleted?.Invoke(7723, 40000, "Ashes from the Void Realm" );
            Story.ChainQuest(7723);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Ascended Dragon of Time");
        }

        if (stopAfterQuestId == 7723) return;
        // Hero's Heartbeat 7724
        if (!Story.QuestProgression(7724) || !Core.CheckInventory("Dragon of Time"))
        {
            Farm.Experience(75);
            if (!Core.CheckInventory("Blade of Awe"))
                Farm.BladeofAweREP(6, true);
            Core.EnsureAccept(7724);

            ArmyEgg();
            RequirementCompleted?.Invoke(7724, 10000, "Egg.GetMysteriousEgg" );

            Core.EquipClass(ClassType.Solo);
            int chaosProgress = Bot.Flash.CallGameFunction<int>("world.getQuestValue", 182);
            try
            {
                Bot.Quests.UpdateQuest(3880);
                RunSolo("chaoslord", () => Core.KillMonster("chaoslord", "r2", "Left", "*", "Conquered Past", isTemp: false));
            }
            finally
            {
                Bot.Quests.UpdateQuest(chaosProgress, 182);
            }
            RequirementCompleted?.Invoke(7724, 20000, "Conquered Past" );
            Bot.Quests.UpdateQuest(10, 159);
            Core.HuntMonster("towerofdoom10", "Slugbutter", "Slugbutter Trophy", 100, false);
            RequirementCompleted?.Invoke(7724, 30000, "Slugbutter Trophy" );
            Core.EquipClass(ClassType.Dodge);
            Core.HuntMonster("icewing", "Warlord Icewing", "Icewing's Laurel", 30, false);
            RequirementCompleted?.Invoke(7724, 40000, "Icewing's Laurel" );
            Story.ChainQuest(7724);
            Bot.Wait.ForPickup("Dragon of Time");
            Bot.Wait.ForPickup("*");
            Core.ToBank("Ascended Dragon of Time Morph", "Ascended Dragon of Time Wings");
        }

        if (stopAfterQuestId == 7724) return;
        if (rankUpClass)
            Adv.RankUpClass("Dragon of Time");

        // I'm Loving It My Way 7725
        if (doExtra)
        {
            Borgars.StoryLine();
            Farm.Experience(75);
            Core.AddDrop("Burger Buns");
            Core.EquipClass(ClassType.Solo);
            Bot.Drops.Add(
                Extras.Where(x => Core.CheckInventory(x, toInv: false)).Select(x => x).ToArray()
            );
            foreach (
                string Item in Core.QuestRewards(7725)
                    .Where(x => !Core.CheckInventory(x, toInv: false))
                    .Select(x => x)
                    .ToArray()
            )
            {
                Core.EnsureAccept(7725);
                if (!Core.CheckInventory("Borgar"))
                {
                    while (!Bot.ShouldExit && !Core.CheckInventory("Burger Buns", 5))
                    {
                        // Burglinster's Revenge 7522
                        Core.EnsureAccept(7522);
                        Core.HuntMonster("borgars", "Burglinster", "Burglinster Cured");
                        Core.EnsureComplete(7522);
                        Bot.Wait.ForPickup("Burger Buns");
                    }
                    Core.BuyItem("borgars", 1884, 54650, shopItemID: 7387);
                }

                Core.EnsureCompleteChoose(7725, Extras);
                Bot.Wait.ForPickup("*");
            }
            Core.ToBank(
                "Dragon of Time Horns",
                "Dragon of Time Horns + Ponytail",
                "Dragon of Time Wings + Tail"
            );
        }
    }

    public void DotArmor()
    {
        if (Core.CheckInventory("Dragon of Time Armor"))
            return;

        if (!Core.isCompletedBefore(7719))
            GetDoT(false, false, true);

        Farm.Experience(40);
        Core.EnsureAccept(7719);

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("cathedral", "Incarnation of Time", "Time Loop Broken", isTemp: false);

        Core.EquipClass(ClassType.Farm);
        Core.KillMonster("portalwar", "r4", "Right", "*", "Anomaly Silenced", 100, false);

        Core.HuntMonster("portalmaze", "ChronoLord", "Chronolord Stopped", 50, false);

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("ubear", "Cornholio", "Is This a Wormhole?", isTemp: false);

        Core.EnsureComplete(7719);
        Bot.Wait.ForDrop("Dragon of Time Armor");
        Bot.Wait.ForPickup("Dragon of Time Armor");
    }
}

