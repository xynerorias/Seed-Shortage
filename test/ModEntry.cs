using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using JsonAssets;

namespace SeedShortageJA
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : StardewModdingAPI.Mod
    {
        private ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            
            helper.Events.Input.ButtonPressed += InputButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoopDayStarted;
            helper.Events.Display.MenuChanged += MenuChanged;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }
        
        /// <summary>Raised before/after the game reads data from a save file and initialises the world (including when day one starts on a new save).</summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data</param>
        public void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Grabbing IDs for the exceptions...", LogLevel.Debug);

            IApi api = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");
            Dictionary<string, int> dic = new Dictionary<string, int>(ID.Dict);
            List<string> list = new List<string>(config.Exceptions.ToList());
            var list2 = list.Where(s => dic.ContainsKey(s)).ToList();
            var dict = dic.Where(kvp => list2.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (KeyValuePair<string,int> exceptions in dict)
                this.Exclusions = new HashSet<int>(dict.Values);
            foreach (string item in list2)
                list.Remove(item);
            foreach(string ex in list)
                this.Exclusions.Add(api.GetObjectId(ex));

            Monitor.Log("All IDs grabbed !", LogLevel.Debug);
        }

        /// <summary>Raised after a new in-game day starts. Everything has already been initialised at this point.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoopDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.player.craftingRecipes.Keys.Contains("Seed Maker"))
            {
                Monitor.Log("Adding Seed Maker recipe", LogLevel.Trace);
                Game1.player.craftingRecipes.Add("Seed Maker", 0);
            }
            if (Game1.player.farmingLevel >= 0)
                Helper.Events.GameLoop.DayStarted -= GameLoopDayStarted;
            
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu)
            {
                bool hatmouse = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
                if (hatmouse)
                    return;

                string shopOwner = null;
                if (shopMenu.portraitPerson != null)
                    shopOwner = shopMenu.portraitPerson.Name;
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Hospital")
                    shopOwner = "Harvey";
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Forest" && !hatmouse)
                    shopOwner = "Travelling";
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "JojaMart")
                    shopOwner = "Joja";
                if (shopMenu.portraitPerson == null && shopOwner == null && !hatmouse)
                    return;

                Dictionary<ISalable, int[]> itemPriceAndStock = shopMenu.itemPriceAndStock;
                List<ISalable> forSale = shopMenu.forSale;

                if (config.PierreEnabled && shopMenu.portraitPerson != null && shopOwner == "Pierre")
                {
                    forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.EndsWith("Sapling")
                        && !item.Name.Equals(config.Exceptions));

                    List<ISalable> unwanted = itemPriceAndStock.Keys.Where(item =>
                        item is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && !Exclusions.Contains(obj.ParentSheetIndex)
                        && !item.Name.EndsWith("Sapling")).ToList();

                    foreach (ISalable item in unwanted)
                        itemPriceAndStock.Remove(item);
                    shopMenu.setItemPriceAndStock(itemPriceAndStock);
                }
            }
        }
        private HashSet<int> Exclusions = new HashSet<int>();
    }
}