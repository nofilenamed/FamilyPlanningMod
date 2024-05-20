using System;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace FamilyPlanning.Patches
{
    class ChildReloadSpritePatch
    {
        public static void Postfix(Child child)
        {
            //Added in the 1.4 update
            //(Presumably this fixes the multiplayer glitches)
            if (Game1.IsMasterGame && child.idOfParent.Value == 0L)
            {
                int uniqueMultiplayerId = (int)Game1.MasterPlayer.UniqueMultiplayerID;
                if (Game1.currentLocation is FarmHouse)
                {
                    FarmHouse currentLocation = Game1.currentLocation as FarmHouse;
                    if (currentLocation.owner != null)
                        uniqueMultiplayerId = (int)currentLocation.owner.UniqueMultiplayerID;
                }
                child.idOfParent.Value = uniqueMultiplayerId;
            }

            if (child.Sprite == null || child.Sprite.textureName.Contains("Characters\\") || (child.Age >= 3 && child.Sprite.CurrentFrame == 0))
            {
                //Try to load the child sprite from a content pack
                Tuple<string, string> assetNames = ModEntry.GetChildSpriteData(child.Name);
                if (assetNames != null)
                {
                    string assetKey = child.Age >= 3 ? assetNames.Item2 : assetNames.Item1;
                    child.Sprite = new AnimatedSprite(assetKey);
                }
                //If that fails, try to load the child sprite from a Content Patcher content pack
                try
                {
                    child.Sprite = new AnimatedSprite("Characters\\Child_" + child.Name);
                }
                catch (Exception) { }

                //If that fails, load the vanilla sprite
                child.Sprite ??= new AnimatedSprite(child.Age >= 3 ? "Characters\\Toddler" + (child.Gender == 0 ? "" : "_girl") + (child.darkSkinned.Value ? "_dark" : "") : "Characters\\Baby" + (child.darkSkinned.Value ? "_dark" : ""));
            }
            //This is default behavior, applies to anyone
            child.HideShadow = true;
            switch (child.Age)
            {
                case 0:
                    child.Sprite.CurrentFrame = 0;
                    child.Sprite.SpriteWidth = 22;
                    child.Sprite.SpriteHeight = 16;
                    break;
                case 1:
                    child.Sprite.CurrentFrame = 4;
                    child.Sprite.SpriteWidth = 22;
                    child.Sprite.SpriteHeight = 32;
                    break;
                case 2:
                    child.Sprite.CurrentFrame = 32;
                    child.Sprite.SpriteWidth = 22;
                    child.Sprite.SpriteHeight = 16;
                    break;
                case 3:
                    child.Sprite.CurrentFrame = 0;
                    child.Sprite.SpriteWidth = 16;
                    child.Sprite.SpriteHeight = 32;
                    child.HideShadow = false;
                    break;
            }

            child.Sprite.UpdateSourceRect();
            child.Breather = false;
        }
    }
}
