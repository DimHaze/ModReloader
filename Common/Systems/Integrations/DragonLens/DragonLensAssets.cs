﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace ModReloader.Common.Systems.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    internal static class DragonLensAssets
    {
        public static class Misc
        {
            public static Asset<Texture2D> GlowAlpha = ModContent.Request<Texture2D>("DragonLens/Assets/Misc/GlowAlpha");
        }

    }
}
