using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Common.Players;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using static ModHelper.UI.Elements.OptionElement;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behavior like God, Noclip, etc.
    /// </summary>
    public class PlayerPanel : OptionPanel
    {
        public List<OptionElement> cheatOptions = new();
        private readonly OptionElement toggleAll;

        public PlayerPanel() : base(title: "Player", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader("Player", hover: "Modify player abilities");

            // Automatically create an option for each cheat
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            // Log.Info("cheats: " + p.GetCheats().Count);

            foreach (var cheat in p.GetCheats())
            {
                if (cheat.Name == "God")
                {
                    // Special case for God mode, we need to send a packet to the server to sync the state in multiplayer
                    var godOption = AddOption(
                        text: cheat.Name,
                        leftClick: cheat.ToggleGod,
                        hover: cheat.Description
                    );
                    cheatOptions.Add(godOption);
                    continue;
                }

                var option = AddOption(
                    text: cheat.Name,
                    leftClick: cheat.Toggle,
                    hover: cheat.Description
                );
                cheatOptions.Add(option);
            }
            AddSlider(
                title: "Mine Radius",
                min: 1,
                max: 50,
                defaultValue: 3,
                onValueChanged: value => MineAura.mineRange = (int)value,
                increment: 1,
                hover: "Mine all tiles around you when moving (not MP-supported)",
                textSize: 0.9f
            );
            toggleAll = AddOption("Toggle All", ToggleAll, "Toggle all player abilities on/off");
            AddPadding();

            AddHeader("Actions");
            AddPadding(5);

            AddAction(ClearInventory, "Clear Inventory", "Clears your inventory except favorited items");

            AddAction(RevealMap, "Reveal Map", "The world map becomes completely explored for this character permanently");
        }

        private void RevealMap()
        {
            // Ensure it's only running on the client side
            if (Main.netMode == NetmodeID.Server)
                return;

            // task because its computationally expensive to  fully reveal the map and run on the main thread
            Task.Run(() =>
            {
                byte brightness = (byte)MathHelper.Clamp(255f * (100 / 100f), 1f, 255f);

                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        if (WorldGen.InWorld(i, j, 0))
                        {
                            Main.Map.Update(i, j, brightness);
                        }
                    }
                }

                Main.refreshMap = true;
                ChatHelper.NewText("Map fully revealed!");
            });
        }

        private void ClearInventory()
        {
            // start at 10 to skip the hotbar
            for (int i = 10; i < Main.LocalPlayer.inventory.Length; i++)
            {
                Item item = Main.LocalPlayer.inventory[i];
                if (!item.favorited)
                {
                    item.TurnToAir(false);
                }
            }
            ChatHelper.NewText("Inventory cleared");
        }

        private void ToggleAll()
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            // Decide whether to enable or disable everything
            bool anyOff = p.GetCheats().Exists(c => c.GetValue() == false);
            // If at least one is off, we enable them all; if all are on, we disable them all
            bool newVal = anyOff;
            p.SetAllCheats(newVal);

            // Update each option’s UI text
            State newState = newVal ? State.Enabled : State.Disabled;
            foreach (OptionElement option in cheatOptions)
            {
                option.SetState(newState);
            }
            // Set itself
            toggleAll.SetState(newState);
        }
    }
}
