using System.Collections.Generic;
using UnityEngine;

namespace SkateHero.UIs
{
    public class UIManager
    {
        private static List<BasicUI> OpenUis = null;
        private static bool isInitialize = false;

        public static void Initialize()
        {
            if (isInitialize)
            {
                return;
            }
            OpenUis = new List<BasicUI>();
            isInitialize = true;
        }

        public static void AddOpenUI(BasicUI basicUI)
        {
            if (!CheckInitialize())
            {
                return;
            }

            //  Disable old UI page ui interactive
            var pageCount = OpenUis.Count;
            var newestUI = GetNewestUIPage();
            if (newestUI != null && pageCount > 1)
            {
                newestUI.UIInteractive = false;
            }

            if (!OpenUis.Contains(basicUI))
            {
                OpenUis.Add(basicUI);
            }
        }

        /// <summary>
        /// ENUM-CommandCallingFrom:
        /// BASICUI > By Class-BasicUI calling.
        /// OTHERSIDE > By other ref class calling, usually will call from here.
        /// </summary>
        public static void RemoveNewestOpenUI(CommandCallingFrom commandFromn)
        {
            if (!CheckInitialize())
            {
                return;
            }

            // If only one open ui, then return.
            var uiCount = OpenUis.Count;
            if (uiCount == 0 || uiCount == 1)
            {
                return;
            }

            if (commandFromn == CommandCallingFrom.BASICUI)
            {
                var newest = OpenUis[uiCount - 1];
                OpenUis.Remove(newest);
            } else if (commandFromn == CommandCallingFrom.OTHERSIDE)
            {
                var currentUI = GetNewestUIPage();
                if (currentUI != null)
                {
                    currentUI.Close();
                }
            }

            //  Enable old UI page ui interactive
            var newestUI = GetNewestUIPage();
            if (newestUI != null)
            {
                newestUI.UIInteractive = true;
            }
        }

        private static bool CheckInitialize()
        {
            if (!isInitialize)
            {
                Debug.LogError("UIManager not Initialize yet.");
            }
            return isInitialize;
        }

        private static BasicUI GetNewestUIPage()
        {
            var pageCount = OpenUis.Count;
            if (pageCount == 0)
            {
                return null;
            }
            return OpenUis[pageCount - 1];
        }
    }

    public enum CommandCallingFrom
    {
        BASICUI,
        OTHERSIDE,
    }
}
