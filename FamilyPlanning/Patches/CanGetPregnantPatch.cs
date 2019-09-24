using System.Collections.Generic;
using Netcode;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using Harmony;

namespace FamilyPlanning.Patches
{
    [HarmonyPatch(typeof(StardewValley.NPC))]
    [HarmonyPatch("canGetPregnant")]
    class CanGetPregnantPatch
    {
        public static void Postfix(ref bool __result)
        {
            //if (this is Horse) return false;
            if (!Game1.IsMasterGame)
                return;

            int totalChildren = ModEntry.GetFamilyData().TotalChildren;
            __result = false;

            Farmer farmer = Game1.player;
            NPC spouse = farmer.getSpouse();

            NetBool trueBool = new NetBool(true);
            if (farmer == null || farmer.divorceTonight.Equals(trueBool))
                return;

            int heartLevelForNPC = farmer.getFriendshipHeartLevelForNPC(spouse.Name);
            Friendship spouseFriendship = farmer.GetSpouseFriendship();
            List<Child> children = farmer.getChildren();
            //spouse.DefaultMap = farmer.homeLocation.ToString();

            if ((Game1.getLocationFromName("FarmHouse") as FarmHouse).upgradeLevel < 2 || spouseFriendship.DaysUntilBirthing >= 0 || (heartLevelForNPC < 10 || farmer.GetDaysMarried() < 7))
                return;

            //this is surely not the most efficient way to check, but I want it to work first.
            //I think the youngest child is last on the list, but I'd need to check manually to verify.
            if (children.Count < totalChildren)
            {
                //If you have 0 children, skips straight to true
                foreach (Child child in children)
                {
                    //55 daysOld is toddler, and pregnancy is 14 days
                    if (child.daysOld < 41)
                    {
                        __result = false;
                        return;
                    }
                }
                __result = true;
            }
        }
    }
}
