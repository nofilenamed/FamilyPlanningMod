using System.Collections.Generic;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Characters;

namespace FamilyPlanning
{
    /* ChildToken keep track of the child's name information
     * for the sake of providing to Content Patcher mods.
     * Content Patcher mods will patch the target "Characters/{{Loe2run.ChildToNPC/FirstChildName}}"
     * and I will try to load from that value when loading child sprites.
     * 
     * (Much of this code is identical to that in Child To NPC, my other mod)
     */

    internal class UpdateAgeChildToken : ChildToken
    {
        private string m_IsToddler;

        public UpdateAgeChildToken(int childNumber) : base(childNumber) { }

        public override bool UpdateContext()
        {
            string lastUpdateIsToddler = m_IsToddler;
            m_IsToddler = GetChildIsToddler(m_ChildNumber);
            return (m_IsToddler != lastUpdateIsToddler);
        }

        private static string GetChildIsToddler(int childNumber)
        {
            if (Context.IsWorldReady)
            {
                List<Child> children = Game1.player.getChildren();
                if (children != null && children.Count >= childNumber)
                    return (children[childNumber - 1].Age == 3) ? "true" : "false";
            }
            return null;
        }

        public override IEnumerable<string> GetValues(string input)
        {
            yield return m_IsToddler;
        }
    }

    internal class ChildToken
    {
        protected string m_ToddlerName;


        protected readonly int m_ChildNumber;

        public ChildToken(int childNumberIn)
        {
            m_ChildNumber = childNumberIn;
        }

        public virtual bool IsReady()
        {
            UpdateContext();
            return m_ToddlerName != null;
        }

        public bool AllowsInput()
        {
            return false;
        }

        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public virtual bool UpdateContext()
        {
            string lastUpdateName = m_ToddlerName;
            m_ToddlerName = GetChildName(m_ChildNumber);
            return (m_ToddlerName != lastUpdateName);
        }

        public virtual IEnumerable<string> GetValues(string input)
        {
            yield return m_ToddlerName;
        }

        private static string GetChildName(int childNumber)
        {
            if (Context.IsWorldReady)
            {
                List<Child> children = Game1.player.getChildren();
                if (children != null && children.Count >= childNumber)
                    return children[childNumber - 1].Name;
            }
            return null;
        }


    }
}
