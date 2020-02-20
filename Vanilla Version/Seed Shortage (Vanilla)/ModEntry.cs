using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;


namespace SeedShortageVanilla
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : StardewModdingAPI.Mod
    {
        private ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += InputButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoopDayStarted;
            helper.Events.Display.MenuChanged += MenuChanged;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        /// <summary>Raised after a new in-game day starts. Everything has already been initialised at this point.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoopDayStarted(object sender, DayStartedEventArgs e)
        {
            //Checks if the player has learned the recipe for Seed Maker.
            if (!Game1.player.craftingRecipes.Keys.Contains("Seed Maker"))
            {
                //Adds the recipe for Seed Maker to the player and logs it as a trace-level log.
                Monitor.Log("Adding Seed Maker recipe", LogLevel.Trace);
                Game1.player.craftingRecipes.Add("Seed Maker", 0);
            }
            //Checks if the player's farming level is at or above 0.
            if (Game1.player.farmingLevel >= 0)
            {
                //Removes the event from the game loop.
                Helper.Events.GameLoop.DayStarted -= GameLoopDayStarted;
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            //Checks if the menu is a shop menu.
            if (e.NewMenu is ShopMenu shopMenu)
            {
                //Gets the shop owner's name.
                string shopOwner = shopMenu.portraitPerson.Name;

                //Defines Hat-mouse.
                bool hatmouse = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("String\\StringsFromCSFiles:ShopMenu.cs.11494"));

                //Assigns the Travelling merchant to be the shop owner when player is in the forest and not talking to Hat Mouse.
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Forest" && !hatmouse)
                    shopOwner = "Travelling";

                //Returns if the shop owner is Hat Mouse.
                if (hatmouse)
                    return;

                //Null check. Returns if there is no portrait, and no name.
                if (shopMenu.portraitPerson == null || shopOwner == null)
                    return;

                //Checks if the shop owner is Pierre.
                if (Config.PierreEnabled && shopMenu.portraitPerson != null && shopOwner == "Pierre")
                {
                    //Removes seeds from listing except for Strawberry Seeds, and the fruit tree saplings.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.EndsWith("Sapling")
                        && !item.Name.Equals("Strawberry Seeds"));

                    //Removes seeds from shop except for Strawberry Seeds, and the fruit tree saplings.
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(seedNoStrawberryNoSaplings =>
                        seedNoStrawberryNoSaplings is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && obj.ParentSheetIndex != 745
                        && obj.ParentSheetIndex != 628
                        && obj.ParentSheetIndex != 629
                        && obj.ParentSheetIndex != 630
                        && obj.ParentSheetIndex != 631
                        && obj.ParentSheetIndex != 632
                        && obj.ParentSheetIndex != 633).ToArray();
                    foreach (ISalable seedNoStrawberryNoSaplings in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(seedNoStrawberryNoSaplings);

                    //Checks if today is the Egg Festival and if Pierre is NOT allowed to have Strawberry Seeds.
                    if (Config.PierreNO_SringFestivalSeeds
                            && Game1.isFestival()
                            && Game1.dayOfMonth == 13
                            && Game1.IsSpring)
                    {
                        //Removes Strawberry Seeds from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item
                            && item.Name.Equals("Strawberry Seeds"));

                        //Removes Strawberry Seeds from shop.
                        ISalable strawberrySeed =
                        shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.ParentSheetIndex == 745);
                        if (strawberrySeed != null)
                            shopMenu.itemPriceAndStock.Remove(strawberrySeed);
                    }
                }

                //Checks if the shop owner is Sandy.
                if (Config.SandyEnabled && shopMenu.portraitPerson != null && shopOwner == "Sandy")
                {
                    //Removes all seeds from listing except for the Cactus Seeds.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.Equals("Cactus Seeds"));

                    //Removes seeds from shop except for Cactus Seeds.
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(seedsNoCactus =>
                        seedsNoCactus is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && obj.ParentSheetIndex != 802).ToArray();
                    foreach (ISalable seedsNoCactus in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(seedsNoCactus);

                    //Checks if Sandy is NOT allowed to sell her indoors Cactus Seeds.
                    if (Config.SandyNO_CactusSeeds)
                    {
                        //Removes Cactus Seeds from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item
                            && item.Name.Equals("Cactus Seeds"));

                        //Removes Cactus Seeds from shop.
                        ISalable cactusSeed =
                        shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.ParentSheetIndex == 802);
                        if (cactusSeed != null)
                            shopMenu.itemPriceAndStock.Remove(cactusSeed);
                    }
                }

                //Checks if the shop owner is the travelling merchant.
                if (Config.TravellingEnabled && shopOwner == "Travelling")
                {
                    //Removes all seeds from listing except for Coffee Bean and Rare Seed.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                    sale is Item item
                    && item.Category == StardewValley.Object.SeedsCategory
                    && !item.Name.Equals("Coffee Bean")
                    && !item.Name.Equals("Rare Seed"));

                    //Removes all seeds from shop except for Coffee Bean and Rare Seed.
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(seedsNoCoffeeNoRare =>
                        seedsNoCoffeeNoRare is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && obj.ParentSheetIndex != 433
                        && obj.parentSheetIndex != 347).ToArray();
                    foreach (ISalable seedsNoCoffeeNoRare in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(seedsNoCoffeeNoRare);

                    //Checks if Travelling Merchant is NOT allowed to sell Coffee Bean.
                    if (Config.TravellingNO_Coffee)
                    {
                        //Removes Coffee Bean from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && !item.Name.Equals("Coffee Bean"));

                        //Removes Coffee Bean from shop.
                        ISalable coffeeBean = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                        s is StardewValley.Object obj
                        && obj.parentSheetIndex == 433);
                        shopMenu.itemPriceAndStock.Remove(coffeeBean);
                    }

                    //Checks if Travelling Merchant is NOT allowed to sell Rare Seed.
                    if (Config.TravellingNO_RareSeed)
                    {
                        //Removes Rare Seed from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item
                            && item.Name.Equals("Rare Seed"));

                        //Removes Rare Seed from shop.
                        ISalable rareSeed = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.parentSheetIndex == 347);
                        shopMenu.itemPriceAndStock.Remove(rareSeed);
                    }
                }

                //Checks if Krobus is the shop owner.
                if (Config.KrobusEnabled && shopMenu.portraitPerson != null && shopOwner == "Krobus")
                {
                    //Removes all seeds except for (Fantasy Crops) seeds from listing.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory);

                    //Removes all seeds except for (Fantasy Crops) seeds from shop.
                    ISalable mixedSeeds = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                        s is StardewValley.Object obj
                        && obj.parentSheetIndex == 770);
                    if (mixedSeeds != null)
                        shopMenu.itemPriceAndStock.Remove(mixedSeeds);
                }
            }
        }
    }
}