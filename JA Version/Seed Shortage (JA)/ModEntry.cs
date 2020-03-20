using System;
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
        private ModConfig Config;
        private IDs ID = new IDs();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();

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
            //Grabs the JsonAssets API.
            IApi api = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");
            if (api != null)
            {
                //Logs the grabbing of IDs as a trace-level log.
                Monitor.Log("Getting PPJA seeds and saplings IDs...");

                //Get the IDs for Fruit & Veggies
                    ID.agaveSeedsID = api.GetObjectId("Blue Agave Seeds");
                    ID.cactusFlowerSeedsID = api.GetObjectId("Cactus Flower Seeds");
                    ID.aloePodID = api.GetObjectId("Aloe Pod");
                
                //Get the IDs for Mizu's Flowers
                    ID.pinkCatSeedsID = api.GetObjectId("Pink Cat Seeds");
                    ID.honeySuckleID = api.GetObjectId("Honeysuckle Starter");
                    ID.beeBalmSeedsID = api.GetObjectId("Bee Balm Seeds");
                
                //Get the IDs for Fantasy Crops
                    ID.coalSeedsID = api.GetObjectId("Coal Seeds");
                    ID.copperSeedsID = api.GetObjectId("Copper Seeds");
                    ID.ironSeedsID = api.GetObjectId("Iron Seeds");
                    ID.goldSeedsID = api.GetObjectId("Gold Seeds");
                    ID.iridiumSeedsID = api.GetObjectId("Iridium Seeds");
                    ID.doubloonSeedsID = api.GetObjectId("Doubloon Seeds");
                
                //Get the IDs for More Trees
                    ID.eucalyptusSaplingID = api.GetObjectId("Eucalyptus Sapling");
                    ID.fragrantWisteriaSaplingID = api.GetObjectId("Fragrant Wisteria Sapling");
                    ID.lemonSaplingID = api.GetObjectId("Lemon Sapling");
                    ID.limeSaplingID = api.GetObjectId("Lime Sapling");
                    ID.melaleucaSaplingID = api.GetObjectId("Melaleuca Sapling");
                    ID.vanillaSaplingID = api.GetObjectId("Vanilla Sapling");
                    ID.cocoaSaplingID = api.GetObjectId("Cocoa Sapling");
                    ID.breadfruitSaplingID = api.GetObjectId("Breadfruit Sapling");
                    ID.avocadoSaplingID = api.GetObjectId("Avocado Sapling");
                    ID.bananaSaplingID = api.GetObjectId("Banana Sapling");
                    ID.dragonFruitSaplingID = api.GetObjectId("Dragon Fruit Sapling");
                    ID.papayaSaplingID = api.GetObjectId("Papaya Sapling");
                    ID.durianSaplingID = api.GetObjectId("Durian Sapling");
                    ID.lycheeSaplingID = api.GetObjectId("Lychee Sapling");
                    ID.mangoSaplingID = api.GetObjectId("Mango Sapling");
                    ID.ylangYlangSaplingID = api.GetObjectId("Ylang Ylang Sapling");
                    ID.almondSaplingID = api.GetObjectId("Almond Sapling");
                    ID.asianPearSaplingID = api.GetObjectId("Asian Pear Sapling");
                    ID.cashewSaplingID = api.GetObjectId("Cashew Sapling");
                    ID.grannySmithSaplingID = api.GetObjectId("Granny Smith Sapling");
                    ID.walnutSaplingID = api.GetObjectId("Walnut Sapling");
                    ID.grapefruitSaplingID = api.GetObjectId("Grapefruit Sapling");
                    ID.oliveSaplingID = api.GetObjectId("Olive Sapling");
                    ID.pecanSaplingID = api.GetObjectId("Pecan Sapling");
                    ID.persimmonSaplingID = api.GetObjectId("Persimmon Sapling");
                    ID.cinnamonSaplingID = api.GetObjectId("Cinnamon Sapling");
                    ID.figSaplingID = api.GetObjectId("Fig Sapling");
                    ID.pearSaplingID = api.GetObjectId("Pear Sapling");
                    ID.pomeloSaplingID = api.GetObjectId("Pomelo Sapling");
                    ID.camphorSaplingID = api.GetObjectId("Camphor Sapling");

                //Get the IDs for Farmer to Florist
                    ID.magnoliaSaplingID = api.GetObjectId("Magnolia Sapling");
                    ID.hibiscusSaplingID = api.GetObjectId("Hibiscus Sapling");
                    ID.jasmineSaplingID = api.GetObjectId("Jasmine Sapling");

                //Get the IDs for Fresh Meat
                    ID.jerkySaplingID = api.GetObjectId("Jerky Sapling");
                    ID.beefvineSeedsID = api.GetObjectId("Beefvine Seeds");
                    ID.chevonvineSeedsID = api.GetObjectId("Chevonvine Seeds");
                    ID.chickenvineSeedsID = api.GetObjectId("Chickenvine Seeds");
                    ID.duckvineSeedsID = api.GetObjectId("Duckvine Seeds");
                    ID.muttonvineSeedsID = api.GetObjectId("Muttonvine Seeds");
                    ID.porkvineSeedsID = api.GetObjectId("Porkvinevine Seeds");
                    ID.rabbitvineSeedsID = api.GetObjectId("Rabbitvine Seeds");

                Monitor.Log("All IDs grabbed !");
            }  
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
                //Defines Hat-mouse for easier "shop owner checking" and exclusion for custom shops located in the forest.
                bool hatmouse = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);

                //Returns if the shop owner is Hat Mouse.
                if (hatmouse)
                    return;

                //Define "shopOwner" so l.171 doesn't freak out.
                string shopOwner = "hello";

                //Define "shopOwner" to be the shop owner's name if there's a portrait.
                if (shopMenu.portraitPerson != null)
                    shopOwner = shopMenu.portraitPerson.Name;

                //Assigns Harvey to be the shop owner of the clinic.
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Hospital")
                    shopOwner = "Harvey";

                //Assigns the Travelling merchant to be the shop owner when player is in the forest and not talking to Hat Mouse.
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Forest" && !hatmouse)
                    shopOwner = "Travelling";

                //Assings "Joja" to be the shop owner of JojaMart.
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "JojaMart")
                    shopOwner = "Joja";

                //Null check. Returns if there is no portrait, no name and it's not Hat Mouse. That way custom shops can work.
                if (shopMenu.portraitPerson == null && shopOwner == null && !hatmouse)
                    return;

                //Checks if the shop owner is Pierre.
                if (Config.PierreEnabled && shopMenu.portraitPerson != null && shopOwner == "Pierre")
                {
                    //Removes seeds from listing except for Strawberry Seeds, all the saplings (vanilla and mods -More Trees/Farmer to Florist-), and the meat seeds (Fresh Meat).
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.EndsWith("Sapling")
                        && !item.Name.Equals("Strawberry Seeds")
                        && !item.Name.Equals("Beefvine Seeds")
                        && !item.Name.Equals("Chevonvine Seeds")
                        && !item.Name.Equals("Chickenvine Seeds")
                        && !item.Name.Equals("Duckvine Seeds")
                        && !item.Name.Equals("Muttonvine Seeds")
                        && !item.Name.Equals("Porkvine Seeds")
                        && !item.Name.Equals("Rabbitvine Seeds"));

                    //Removes seeds from shop except for Strawberry Seeds, all the saplings (vanilla and mods -More Trees/Farmer to Florist-), and the meat seeds (Fresh Meat).
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(PierreSeeds =>
                        PierreSeeds is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && obj.ParentSheetIndex != 745
                        && obj.ParentSheetIndex != 628
                        && obj.ParentSheetIndex != 629
                        && obj.ParentSheetIndex != 630
                        && obj.ParentSheetIndex != 631
                        && obj.ParentSheetIndex != 632
                        && obj.ParentSheetIndex != 633
                        && obj.ParentSheetIndex != ID.fragrantWisteriaSaplingID
                        && obj.ParentSheetIndex != ID.lemonSaplingID
                        && obj.ParentSheetIndex != ID.limeSaplingID
                        && obj.ParentSheetIndex != ID.melaleucaSaplingID
                        && obj.ParentSheetIndex != ID.vanillaSaplingID
                        && obj.ParentSheetIndex != ID.cocoaSaplingID
                        && obj.ParentSheetIndex != ID.avocadoSaplingID
                        && obj.ParentSheetIndex != ID.almondSaplingID
                        && obj.ParentSheetIndex != ID.cashewSaplingID
                        && obj.ParentSheetIndex != ID.asianPearSaplingID
                        && obj.ParentSheetIndex != ID.grannySmithSaplingID
                        && obj.ParentSheetIndex != ID.grapefruitSaplingID
                        && obj.ParentSheetIndex != ID.oliveSaplingID
                        && obj.ParentSheetIndex != ID.pecanSaplingID
                        && obj.ParentSheetIndex != ID.persimmonSaplingID
                        && obj.ParentSheetIndex != ID.cinnamonSaplingID
                        && obj.ParentSheetIndex != ID.figSaplingID
                        && obj.ParentSheetIndex != ID.pearSaplingID
                        && obj.ParentSheetIndex != ID.pomeloSaplingID
                        && obj.ParentSheetIndex != ID.camphorSaplingID
                        && obj.ParentSheetIndex != ID.magnoliaSaplingID
                        && obj.ParentSheetIndex != ID.hibiscusSaplingID
                        && obj.ParentSheetIndex != ID.jasmineSaplingID
                        && obj.ParentSheetIndex != ID.walnutSaplingID
                        && obj.parentSheetIndex != ID.jerkySaplingID
                        && obj.parentSheetIndex != ID.beefvineSeedsID
                        && obj.parentSheetIndex != ID.chevonvineSeedsID
                        && obj.parentSheetIndex != ID.chickenvineSeedsID
                        && obj.parentSheetIndex != ID.duckvineSeedsID
                        && obj.parentSheetIndex != ID.muttonvineSeedsID
                        && obj.parentSheetIndex != ID.porkvineSeedsID
                        && obj.parentSheetIndex != ID.rabbitvineSeedsID).ToArray();
                    foreach (ISalable PierreSeeds in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(PierreSeeds);

                    //Checks if today is the Egg Festival and if Pierre is NOT allowed to have Strawberry Seeds.
                    if (Config.PierreNO_Strawberry
                            && Game1.isFestival()
                            && Game1.dayOfMonth == 13
                            && Game1.IsSpring)
                    {
                        //Removes Strawberry Seeds from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) => 
                            sale is Item item 
                            && item.Name.Equals("Strawberry Seeds"));

                        //Removes Strawberry Seeds from shop.
                        ISalable strawberrySeed = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.ParentSheetIndex == 745);
                        if (strawberrySeed != null) 
                            shopMenu.itemPriceAndStock.Remove(strawberrySeed);
                    }

                    //Checks if Pierre is NOT allowed to sell the meat seeds.
                    if(Config.PierreNO_MeatSeeds)
                    {
                        //Removes meat seeds (Fresh Meat) from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item
                            && item.Name.Equals("Beefvine Seeds")
                            && item.Name.Equals("Chevonvine Seeds")
                            && item.Name.Equals("Chickenvine Seeds")
                            && item.Name.Equals("Duckvine Seeds")
                            && item.Name.Equals("Muttonvine Seeds")
                            && item.Name.Equals("Porkvine Seeds")
                            && item.Name.Equals("Rabbitvine Seeds"));

                        //Removes meat seeds (Fresh Meat) from shop.
                        ISalable[] salable = shopMenu.itemPriceAndStock.Keys.Where(meatSeeds =>
                            meatSeeds is StardewValley.Object obj
                            && obj.ParentSheetIndex == ID.beefvineSeedsID
                            && obj.ParentSheetIndex == ID.chevonvineSeedsID
                            && obj.ParentSheetIndex == ID.chickenvineSeedsID
                            && obj.ParentSheetIndex == ID.duckvineSeedsID
                            && obj.ParentSheetIndex == ID.muttonvineSeedsID
                            && obj.ParentSheetIndex == ID.porkvineSeedsID
                            && obj.ParentSheetIndex == ID.rabbitvineSeedsID).ToArray();
                        foreach (ISalable meatSeeds in salable)
                            shopMenu.itemPriceAndStock.Remove(meatSeeds);
                    }
                }

                //Checks if the shop owner is Sandy.
                if (Config.SandyEnabled && shopMenu.portraitPerson != null && shopOwner == "Sandy")
                {
                    //Removes all seeds from listing except for the Cactus Seeds, her fruit saplings (More Trees) Cactus Flower Seeds (Fruits & Veggies) and Blue Agave Seeds (Fruits & Veggies).
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.EndsWith("Sapling")
                        && !item.Name.Equals("Cactus Flower Seeds")
                        && !item.Name.Equals("Blue Agave Seeds")
                        && !item.Name.Equals("Cactus Seeds"));

                    //Removes seeds from shop except for the Cactus Seeds, her fruit saplings (More Trees) Cactus Flower Seeds (Fruits & Veggies) and Blue Agave Seeds (Fruits & Veggies).
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(seedsNoAgaveCactus =>
                        seedsNoAgaveCactus is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && obj.ParentSheetIndex != 802
                        && obj.ParentSheetIndex != ID.agaveSeedsID
                        && obj.ParentSheetIndex != ID.cactusFlowerSeedsID
                        && obj.ParentSheetIndex != ID.bananaSaplingID
                        && obj.ParentSheetIndex != ID.dragonFruitSaplingID
                        && obj.ParentSheetIndex != ID.papayaSaplingID
                        && obj.ParentSheetIndex != ID.breadfruitSaplingID
                        && obj.ParentSheetIndex != ID.durianSaplingID
                        && obj.ParentSheetIndex != ID.lycheeSaplingID
                        && obj.ParentSheetIndex != ID.mangoSaplingID
                        && obj.ParentSheetIndex != ID.ylangYlangSaplingID).ToArray();
                    foreach (ISalable seedsNoAgaveCactus in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(seedsNoAgaveCactus);

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

                    //Checks if Sandy is NOT allowed to sell her (Fruits & Veggies) Year 3+ seeds.
                    if (Config.SandyNO_Year3Seeds)
                    {
                        //Removes Cactus Flower Seeds (Fruits & Veggies) and Blue Agave Seeds (Fruits & Veggies) from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item
                            && item.Name.Equals("Cactus Flower Seeds")
                            && item.Name.Equals("Blue Agave Seeds"));

                        //Removes Blue Agave Seeds (Fruits & Veggies) and Cactus Flower Seeds (Fruits & Veggies) from shop.
                        ISalable[] removeQueue2 = shopMenu.itemPriceAndStock.Keys.Where(seedsAgaveCactus =>
                            seedsAgaveCactus is StardewValley.Object obj
                            && obj.ParentSheetIndex == ID.agaveSeedsID
                            && obj.ParentSheetIndex == ID.cactusFlowerSeedsID).ToArray();
                        foreach (ISalable seedsAgaveCactus in removeQueue2)
                            shopMenu.itemPriceAndStock.Remove(seedsAgaveCactus);
                    }
                }

                //Checks if the shop owner is Marnie.
                if (Config.MarnieEnabled && shopMenu.portraitPerson != null && shopOwner == "Marnie")
                {
                    //Removes all seeds from listing except for Pink Cat Seeds (Mizu's Flowers), Honeysuckle Starter (Mizu's Flowers) and Bee Balm Seeds (Mizu's Flowers) .
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.Equals("Pink Cat Seeds")
                        && !item.Name.Equals("Honeysuckle Starter")
                        && !item.Name.Equals("Bee Balm"));

                    //Removes seeds from shop except for Pink Cat Seeds (Mizu's Flowers).
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(seedsNoPinkCatHoneyBee =>
                        seedsNoPinkCatHoneyBee is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && obj.ParentSheetIndex != ID.pinkCatSeedsID
                        && obj.parentSheetIndex != ID.honeySuckleID
                        && obj.parentSheetIndex != ID.beeBalmSeedsID).ToArray();
                    foreach (ISalable seedsNoPinkCatHoneyBee in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(seedsNoPinkCatHoneyBee);

                    //Checks is Marnie is NOT allowed to sell her beginner florist seeds Pink Cat (Mizu's Flowers).
                    if (Config.MarnieNO_PinkCat)
                    {
                        //Removes Pink Cat Seeds (Mizu's Flowers) from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item && item.Name.Equals("Pink Cat Seeds"));

                        //Removes Pink Cat Seeds (Mizu's Flowers) from shop.
                        ISalable pinkCatSeeds =
                        shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.ParentSheetIndex == ID.pinkCatSeedsID);
                        if (pinkCatSeeds != null)
                            shopMenu.itemPriceAndStock.Remove(pinkCatSeeds);
                    }

                    //Checks is Marnie is NOT allowed to sell Honeysuckle Starter (Mizu's Flowers).
                    if (Config.MarnieNO_Honeysuckle)
                    {
                        //Removes Honeysuckle Starter (Mizu's Flowers) from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item && item.Name.Equals("Honeysuckle Starter"));

                        //Removes Pink Cat Seeds (Mizu's Flowers) from shop.
                        ISalable honeysuckleStarter =
                        shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.ParentSheetIndex == ID.honeySuckleID);
                        if (honeysuckleStarter != null)
                            shopMenu.itemPriceAndStock.Remove(honeysuckleStarter);
                    }

                    //Checks is Marnie is NOT allowed to sell Bee Balm (Mizu's Flowers).
                    if (Config.MarnieNO_BeeBalm)
                    {
                        //Removes Bee Balm Seeds (PPJA - Mizu's Flowers) from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item && item.Name.Equals("Bee Balm Seeds"));

                        //Removes bee Balm Seeds (Mizu's Flowers) from shop.
                        ISalable beeBalmSeeds =
                        shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                            s is StardewValley.Object obj
                            && obj.ParentSheetIndex == ID.beeBalmSeedsID);
                        if (beeBalmSeeds != null)
                            shopMenu.itemPriceAndStock.Remove(beeBalmSeeds);
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

                //Checks if Harvey is the shop owner.
                if (Config.HarveyEnabled && shopOwner == "Harvey")
                {
                    //Removes Aloe Pod (Fruits & Veggies) from listing.
                    shopMenu.forSale.RemoveAll((ISalable sale) => 
                        sale is Item item 
                        && item.Name.Equals("Aloe Pod"));

                    //Removes Aloe Pod (Fruits & Veggies) from shop.
                    ISalable aloePod = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s => 
                        s is StardewValley.Object obj
                        && obj.parentSheetIndex == ID.aloePodID);
                    if(aloePod != null)
                        shopMenu.itemPriceAndStock.Remove(aloePod);
                }

                //Checks if the shop owner is Clint.
                if (Config.ClintNO_CoalSeeds && shopMenu.portraitPerson != null && shopOwner == "Clint")
                {
                    //Removes Coal Seeds (Fantasy Crops) from listing.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory);

                    //Removes Coal Seeds (Fantasy Crops) from shop.
                    ISalable coalSeed = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s => 
                        s is StardewValley.Object obj 
                        && obj.parentSheetIndex == ID.coalSeedsID);
                    if (coalSeed != null)
                        shopMenu.itemPriceAndStock.Remove(coalSeed);
                }

                //Checks if Krobus is the shop owner.
                if (Config.KrobusEnabled && shopMenu.portraitPerson != null && shopOwner == "Krobus")
                {
                    //Removes all seeds except for (Fantasy Crops) seeds from listing.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.Equals("Copper Seeds")
                        && !item.Name.Equals("Iron Seeds")
                        && !item.Name.Equals("Gold Seeds")
                        && !item.Name.Equals("Iridium Seeds")
                        && !item.Name.Equals("Doubloon Seeds"));

                    //Removes all seeds except for (Fantasy Crops) seeds from shop.
                    ISalable mixedSeeds = shopMenu.itemPriceAndStock.Keys.FirstOrDefault(s =>
                        s is StardewValley.Object obj
                        && obj.parentSheetIndex == 770);
                    if (mixedSeeds != null)
                        shopMenu.itemPriceAndStock.Remove(mixedSeeds);

                    //Checks if Krobus is NOT allowed to sell his (Fantasy Crops) seeds.
                    if(Config.KrobusNO_MetalSeeds)
                    {
                        //Removes (Fantasy Crops) seeds from listing.
                        shopMenu.forSale.RemoveAll((ISalable sale) =>
                            sale is Item item
                            && item.Name.Equals("Copper Seeds")
                            && item.Name.Equals("Iron Seeds")
                            && item.Name.Equals("Gold Seeds")
                            && item.Name.Equals("Iridium Seeds")
                            && item.Name.Equals("Doubloon Seeds"));

                        //Removes (Fantasy Crops) seeds from shop.
                        ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(seedsFantasyCrops =>
                            seedsFantasyCrops is StardewValley.Object obj
                            && obj.Category == StardewValley.Object.SeedsCategory
                            && obj.ParentSheetIndex == ID.copperSeedsID
                            && obj.parentSheetIndex == ID.ironSeedsID
                            && obj.parentSheetIndex == ID.goldSeedsID
                            && obj.parentSheetIndex == ID.iridiumSeedsID
                            && obj.parentSheetIndex == ID.doubloonSeedsID).ToArray();
                        foreach (ISalable seedsFantasyCrops in removeQueue)
                            shopMenu.itemPriceAndStock.Remove(seedsFantasyCrops);
                    }
                }

                //Checks is JojaMart is NOT allowed to sell seeds.
                if(Config.JojaEnabled && shopOwner == "Joja")
                {
                    //Removes seeds from JojaMart listing.
                    shopMenu.forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory);

                    //Removes seeds from JojaMart.
                    ISalable[] removeQueue = shopMenu.itemPriceAndStock.Keys.Where(jojaseeds =>
                        jojaseeds is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory).ToArray();
                    foreach (ISalable jojaseeds in removeQueue)
                        shopMenu.itemPriceAndStock.Remove(jojaseeds);
                }
            }
        }
    }
}