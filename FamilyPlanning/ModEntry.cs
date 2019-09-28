﻿using System;
using System.Reflection;
using StardewModdingAPI;
using Harmony;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using System.Collections.Generic;

namespace FamilyPlanning
{
    /* Family Planning: allow players to customize the number of children they have.
     * -> The player enters the number of children they want, for now it's a console command.
     *   -> If 0, they never get the question.
     *   -> If 1, they stop after 1.
     *   -> The default is 2, vanilla behavior. (if they don't already have more than 2 kids)
     *   -> If more than 2, then they get the event even after 2 children.
     * -> Also, this mods allows the player to customize the gender of the child at birth.
     */

    /* Documented issues:
     * -> I haven't comfirmed whether every language works with my message editing.
     * -> If you have more than 2 kids, kids will have to share beds.
     * -> Your spouse can't get pregnant until the previous child is a toddler. (not vanilla?)
     * -> The way children choose beds is by gender and age. If you have two or fewer kids, they each get their own bed.
     *    Once you get more than 2, they need to double up, so they'll try first to double up with a sibling of the same gender.
     * -> Currently disabled for multiplayer
     * -> (not so much an issue: you still get the Fullhouse achievment at 2 kids)
     */

    /* Harmony patches needed:
     *  -> StardewValley.NPC.canGetPregnant() -> determines the number of children you can have
     *  -> StardewValley.Characters.Child.reloadSprite() -> determines the sprite for a child
     *  -> StardewValley.Characters.Child.tenMinuteUpdate() -> tells the child where their bed is
     */

    /* Content Packs:
     * I've added support for Content Packs to this mod.
     * I'll add a pre-made Content Pack on Nexus that only needs your to add the existing png's.
     * Instructions for how to make a Content Pack are in the README.md on GitHub and to a lesser extent the ContentPackData class.
     */

    /* Multiplayer testing:
     * -> Pathoschild discovered a glitch in Stardew Valley 1.3.
     *    When player 2 enters player 1's house, player 2 loads player 1's child as if they were their own.
     *    (As seen by a dark skinned child becoming a light skinned child?)
     *    Therefore, if one player has a Family Planning content pack and the other doesn't,
     *    there are some serious issues (crashing).
     * -> Multiplayer compatibility: It's okay for only one player to have Family Planning,
     *    but if you want to have a content pack, then both players need Family Planning & that exact content pack.
     * -> (Also for the sake of multiplayer compatibility, save data is now saved to assets/savedata.json)
     */

    class ModEntry : Mod
    {
        private static FamilyData data;
        private static List<IContentPack> contentPacks;
        private static IMonitor monitor;
        public static IModHelper helper;
        private readonly int maxChildren = 4;

        public override void Entry(IModHelper helper)
        {
            //Console commands
            helper.ConsoleCommands.Add("get_max_children", "Returns the number of children you can have.", GetTotalChildrenConsole);
            helper.ConsoleCommands.Add("set_max_children", "Sets the value for how many children you can have. (Currently, the limit is " + maxChildren + ".) \nUsage: set_max_children <value>\n- value: the number of children you can have.", SetTotalChildrenConsole);
            //Event handlers
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            //Harmony
            HarmonyInstance harmony = HarmonyInstance.Create("Loe2run.FamilyPlanning");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //create variables
            monitor = Monitor;
            ModEntry.helper = helper;
            //Load content packs
            contentPacks = new List<IContentPack>();
            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                contentPacks.Add(contentPack);
            }
        }
        
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                data = Helper.Data.ReadJsonFile<FamilyData>("data/savedata.json");
            }
            catch (Exception)
            {
                data = new FamilyData();
                Helper.Data.WriteJsonFile<FamilyData>("data/savedata.json", data);
            }
        }
        
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.farmEvent != null && Game1.farmEvent is BirthingEvent)
            {
                Game1.farmEvent = new CustomBirthingEvent();
                Game1.farmEvent.setUp();
            }
        }
         
        public void GetTotalChildrenConsole(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            Monitor.Log("The number of children you can have is: " + GetFamilyData().TotalChildren);
        }

        public void SetTotalChildrenConsole(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            int input;
            try
            {
                input = int.Parse(args[0]);

                if (input > maxChildren)
                    Monitor.Log("This mod currently limits your maximum to " + maxChildren + ", so your input won't be accepted.");
                else if (input >= 0)
                {
                    data.TotalChildren = input;
                    Helper.Data.WriteJsonFile("data/savedata.json", data);
                    Monitor.Log("The number of children you can have has been set to " + input + ".");
                }
                else
                    Monitor.Log("Input value is out of bounds.");
            }
            catch (Exception e)
            {
                Monitor.Log(e.Message);
            }
        }
         
        public static FamilyData GetFamilyData()
        {
            return data;
        }
         
        public static Tuple<string, string> GetChildSpriteData(string childName)
        {
            foreach(IContentPack contentPack in contentPacks)
            {
                try
                {
                    ContentPackData cpdata = contentPack.ReadJsonFile<ContentPackData>("assets/data.json");
                    if (cpdata.ChildSpriteID == null)
                        return null;
                    foreach (string key in cpdata.ChildSpriteID.Keys)
                    {
                        if (key.Equals(childName))
                        {
                            cpdata.ChildSpriteID.TryGetValue(key, out Tuple<string, string> pair);
                            string assetName1 = contentPack.GetActualAssetKey("assets/" + pair.Item1);
                            string assetName2 = contentPack.GetActualAssetKey("assets/" + pair.Item2);
                            return new Tuple<string, string>(assetName1, assetName2);
                        }
                    }
                }
                catch (Exception e)
                {
                    monitor.Log("An exception occurred in Loe2run.FamilyPlanning while loading the child sprite.");
                    monitor.Log(e.Message);
                }
            }
            return null;
        }

        public static Tuple<int, string> GetSpouseDialogueData(string spouseName)
        {
            foreach (IContentPack contentPack in contentPacks)
            {
                try
                {
                    ContentPackData cpdata = contentPack.ReadJsonFile<ContentPackData>("assets/data.json");
                    if (cpdata.SpouseDialogue == null)
                        return null;
                    foreach(string key in cpdata.SpouseDialogue.Keys)
                    {
                        if (key.Equals(spouseName))
                        {
                            cpdata.SpouseDialogue.TryGetValue(key, out Tuple<int, string> spouseDialogue);
                            return spouseDialogue;
                        }
                    }
                }
                catch(Exception e)
                {
                    monitor.Log("An exception occurred in Loe2run.FamilyPlanning while loading spouse dialogue.");
                    monitor.Log(e.Message);
                }
            }
            return null;
        }
    }
}