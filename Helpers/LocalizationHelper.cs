﻿namespace ModReloader.Helpers
{
    // Loc:
    // Short for Localization
    // This class is used to get the localization text for the ModReloader mod.
    // Is found in en-US.Mods.ModReloader.json and other localization files.
    public static class Loc
    {
        /// <summary>
        /// Gets the text for the given key from the ModReloader localization file.
        /// If no localization is found, the key itself is returned.
        /// Reference:
        /// https://github.com/ScalarVector1/DragonLens/blob/master/Helpers/LocalizationHelper.cs
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            if (Terraria.Localization.Language.Exists($"Mods.ModReloader.{key}"))
            {
                return Terraria.Localization.Language.GetTextValue($"Mods.ModReloader.{key}", args);
            }
            else if (Terraria.Localization.Language.Exists($"tModLoader.{key}"))
            {
                return Terraria.Localization.Language.GetTextValue($"tModLoader.{key}", args);
            }
            else
            {
                // returns vanilla localization if the key exists, or return the key
                return Terraria.Localization.Language.GetTextValue(key, args);
            }
        }
    }
}
