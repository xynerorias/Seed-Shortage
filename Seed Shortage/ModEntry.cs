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

namespace SeedShortage
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : StardewModdingAPI.Mod
    {
        private ModConfig config;
        private List<int> Exclusions = new List<int>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            
            helper.Events.Input.ButtonPressed += InputButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoopDayStarted;
            helper.Events.Display.MenuChanged += MenuChanged;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
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
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Grabbing IDs for the exceptions...", LogLevel.Debug);

            IApi api = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");
            Dictionary<string,int> VanillaSeeds = new Dictionary<string, int>(ID.Dict);
            List<string> exceptions = new List<string>(config.Exceptions.ToList());
            var NewList = exceptions.Where(s => VanillaSeeds.ContainsKey(s)).ToList();
            var dict = VanillaSeeds.Where(kvp => NewList.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            this.Exclusions = new List<int>(dict.Values);

            foreach (string item in NewList)
                exceptions.Remove(item);

            if(api != null)
            foreach (string ex in exceptions)
                this.Exclusions.Add(api.GetObjectId(ex));

            string log = string.Join(",", this.Exclusions.ToArray());
            Monitor.Log("IDs marked as exception: " + log, LogLevel.Trace);

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
        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu)
            {
                bool hatmouse = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
                bool magicboat = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.magicBoat"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
                bool travelnight = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.travelernightmarket"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
                string shopOwner = null;

                if (shopMenu.portraitPerson != null)
                    shopOwner = shopMenu.portraitPerson.Name;
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Hospital")
                    shopOwner = "Harvey";
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Forest" && !hatmouse)
                    shopOwner = "Travelling";
                if (Game1.currentLocation.Name == "JojaMart")
                    shopOwner = "Joja";
                if (magicboat)
                    shopOwner = "Magic Boat";
                if (hatmouse)
                    return;
                if (shopMenu.portraitPerson == null && shopOwner == null && !hatmouse)
                    return;

                Dictionary<ISalable, int[]> itemPriceAndStock = shopMenu.itemPriceAndStock;
                List<ISalable> forSale = shopMenu.forSale;

                if (config.JojaEnabled && shopOwner == "Joja")
                {
                    if (config.JojaPrices)
                    {
                        using (Dictionary<ISalable, int[]>.KeyCollection.Enumerator enumerator = itemPriceAndStock.Keys.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                ISalable now = enumerator.Current;
                                int[] array = itemPriceAndStock[now];
                                int price = now.salePrice();
                                if (now.Name.EndsWith("Seeds") || now.Name.EndsWith("Bulb") || now.Name.EndsWith("Starter") && !now.Name.Equals("Grass Starter"))
                                        array[0] = this.NewPrice(price);
                            }
                            string PricesUpdated = string.Format("Seed prices increased by {0} for {1}! Join us, thrive !", (object)config.PriceIncrease, (object)shopOwner);
                            Monitor.Log(PricesUpdated, LogLevel.Trace);
                        }
                    }
                    else
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

                        if (config.Exceptions != null)
                        {
                            string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                            Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                        }
                        else
                            Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                    }
                }

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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.SandyEnabled && shopMenu.portraitPerson != null && shopOwner == "Sandy")
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.MarnieEnabled && shopMenu.portraitPerson != null && shopOwner == "Marnie")
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.ClintEnabled && shopMenu.portraitPerson != null && shopOwner == "Clint")
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.KrobusEnabled && shopMenu.portraitPerson != null && shopOwner == "Krobus")
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.MarlonEnabled && shopMenu.portraitPerson != null && shopOwner == "Marlon")
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.TravellingEnabled && shopOwner == "Travelling" || travelnight)
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if (config.HarveyEnabled && shopOwner == "Harvey")
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

                    if (config.Exceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                }

                if(config.MagicBoatEnabled && shopOwner == "Magic Boat")
                {
                    if (config.MagicBoatPrices)
                    {
                        using (Dictionary<ISalable, int[]>.KeyCollection.Enumerator enumerator = itemPriceAndStock.Keys.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                ISalable now = enumerator.Current;
                                int[] array = itemPriceAndStock[now];
                                int price = now.salePrice();
                                if (now.Name.EndsWith("Seeds") || now.Name.EndsWith("Bulb") || now.Name.EndsWith("Starter"))
                                    array[0] = this.NewPrice(price);
                            }
                            string PricesUpdated = string.Format("{0} has some seeds but since they're rare, they are {1} more expensive!", (object)shopOwner, (object)config.PriceIncrease);
                            Monitor.Log(PricesUpdated, LogLevel.Trace);
                        }
                    }
                    else
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

                        if (config.Exceptions != null)
                        {
                            string ex = string.Format("{0}", string.Join(", ", config.Exceptions));
                            Monitor.Log(string.Format("Seeds removed from {0}, except for {1} !", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                        }
                        else
                            Monitor.Log(string.Format("Seeds removed from {0}!", (object)shopOwner, (LogLevel.Trace)));
                    }
                }
                else return;
            }
        }
        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ReturnedToTitle (object sender, ReturnedToTitleEventArgs e)
        {
            this.Exclusions.Clear();
        }
        private int NewPrice (int price)
        {
            string increase = config.PriceIncrease;
            if (increase.EndsWith("%"))
            {
                int num = int.Parse(increase.Substring(0, increase.Length - 1));
                int newprice = (int)((double)(100 + num) / 100 * (double)price);
                return newprice;
            }
            if (increase.EndsWith("x"))
            {
                int num = int.Parse(increase.Substring(0, increase.Length - 1));
                int newprice = price * num;
                return newprice;
            }
            else
            {
                string err = string.Format("{0} is not a valid increment value. Use either {0}% or {0}x in the config.", (object)config.PriceIncrease);
                Monitor.Log(err, LogLevel.Error);
                throw new ArgumentException(err);
            }
        }
    }
}