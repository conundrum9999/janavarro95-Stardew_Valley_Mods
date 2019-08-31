using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using PyTK.Extensions;
using PyTK.Types;
using Revitalize.Framework;
using Revitalize.Framework.Crafting;
using Revitalize.Framework.Environment;
using Revitalize.Framework.Factories.Objects;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.Furniture;
using Revitalize.Framework.Player;
using Revitalize.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardustCore.UIUtilities;
using StardustCore.Animations;
using StardewValley.Menus;
using Revitalize.Framework.Objects.Extras;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame;
using Revitalize.Framework.Objects.Items.Resources;
using Revitalize.Framework.Hacks;
using Revitalize.Framework.Configs;

namespace Revitalize
{

    //Bugs:
    //  -Chair tops cut off objects
    // -load content MUST be enabled for the table to be placed?????? WTF
    // TODO:
    /*  Add in crafting menu.
     *  Add in crafting table.
     *  Find way to hack vanilla furnace for more recipes.
     *
     * 
    // -Make this mod able to load content packs for easier future modding
    //
    //  -Multiple Lights On Object
    //  -Illumination Colors
    //  Furniture:
    //      -rugs (done, needs factory info/sprite)
    //      -tables (done)
    //      -lamps (done)
    //      -chairs (done)
    //      -benches (done but needs factory info/sprite)
    //      -dressers/other storage containers (Done!)
    //      -fun interactables
    //          -Arcade machines
    //      -More crafting tables
    //      -Baths (see chairs but swimming)
    //
    //  -Machines
    //      !=Energy
    //            Generators:
                  -solar
                  -burnable
                  -watermill
                  -windmill
                  -crank (costs stamina)
                  Storage:
                  -Batery Pack
             -Mini-greenhouse
                   -takes fertilizer which can do things like help crops grow or increase prodcuction yield/quality.
                   -takes crop/extended crop seeds
                   -takes sprinklers
                   -has grid (1x1, 2x2, 3x3, 4x4, 5x5) system for growing crops/placing sprinkers
                   -sprinkers auto water crops
                   -can auto harvest
                   -hover over crop to see it's info
                   -can be upgraded to grow crops from specific seasons with season stones (spring,summer, fall winter) (configurable if they are required)
                   -Add in season stone recipe

    //      -Furnace
    //      -Seed Maker
    //      -Stone Quarry
    //      -Mayo Maker
    //      -Cheese Maker
            -Yogurt Maker
                   -Fruit yogurts (artisan good)
    //      -Auto fisher
    //      -Auto Preserves
    //      -Auto Keg
    //      -Auto Cask
    //      -Calcinator (oil+stone)
    //  -Materials
    //      -Tin/Bronze/Alluminum/Silver?Platinum/Etc
            -titanium
            -Alloys!
                -Brass
                -Electrum
            -Mythrill
            -Steel
            -Star Metal
            -Star Steel
            -Cobalt
        -Liquids
            -oil
            -water
            -coal
            -juice???
            -lava?

        -Dyes!
            -Dye custom objects certain colors!
            -Rainbow Dye -(set a custom object to any color)
            -red, green, blue, yellow, pink, etc
            -Make dye from flowers/coal/algee/minerals/gems (black), etc
                -soapstone (washes off dye)
                -Lunarite (white)
        Dye Machine
            -takes custom object and dye
            -dyes the object
            -can use water to wash off dye.
            -maybe dye stardew valley items???
            -Dyed Wool (Artisan good)

        Menus:
    //  -Crafting Menu
    //  -Item Grab Menu (Extendable) (Done!)
    //   -Yes/No Dialogue Box
    //   -Multi Choice dialogue box


    //  -Gift Boxes

    //  Magic!
    //      -Alchemy Bags
    //      -Transmutation
    //      -Effect Crystals
    //      -Spell books
    //      -Potions!
    //      -Magic Meter
    //      -Connected chests (3 digit color code) much like Project EE2 from MC
    //
    //
    //  -Food
            -multi flavored sodas

    //  -Bigger chests
    //
    //  Festivals
    //      -Firework festival
    //      -Horse Racing Festival
            -Valentines day (Maybe make this just one holiday)
                -Spring. Male to female gifts.
                -Winter. Female to male gifts. 
    //  Stargazing???
    //      -Moon Phases+DarkerNight
    //  Bigger/Better Museum?
    // 
    //  Equippables!
    //      -accessories that provide buffs/regen/friendship
    //      -braclets/rings/broaches....more crafting for these???
    //      
    //  Music???
    //      -IDK maybe add in instruments???
    //      
    //  More buildings????
    //  
    //  More Animals???
    //  
    //  Readable Books?
    //  
    //  Custom NPCs for shops???
    //
    //  Minigames:
    //      Frisbee Minigame?
    //      HorseRace Minigame/Betting?
    //  
    //  Locations:
            -Make extra bus stop sign that travels between new towns/locations.
    //      -Small Island Home?
    //      -New town inspired by FOMT;Mineral Town/The Valley HM DS
    //
    //  More crops
    //      -RF Crops
    //      -HM Crops
    //
    //  More monsters
    //  -boss fights
    //
    //  More dungeons??

    //  More NPCS?

        Accessories
        (recover hp/stamina,max hp,more friendship ,run faster, take less damage, etc)
            -NEckalces
            -Broaches
            -Earings
            -Pendants

    */

    public class ModCore : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        /// <summary>
        /// Keeps track of custom objects.
        /// </summary>
        public static ObjectManager ObjectManager;

        /// <summary>
        /// Keeps track of all of the extra object groups.
        /// </summary>
        public static Dictionary<string, MultiTiledObject> ObjectGroups;

        public static PlayerInfo playerInfo;

        public static Serializer Serializer;

        public static Dictionary<GameLocation, MultiTiledObject> ObjectsToDraw;
        public static VanillaRecipeBook VanillaRecipeBook;

        public static Dictionary<Guid, CustomObject> CustomObjects;

        public static ConfigManager Configs;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = this.Monitor;
            Manifest = this.ModManifest;
            Configs = new ConfigManager();

            this.createDirectories();
            this.initailizeComponents();
            Serializer = new Serializer();
            playerInfo = new PlayerInfo();
            CustomObjects = new Dictionary<Guid, CustomObject>();

            //Loads in textures to be used by the mod.
            this.loadInTextures();

            //Loads in objects to be use by the mod.
            ObjectGroups = new Dictionary<string, MultiTiledObject>();
            ObjectManager = new ObjectManager(Manifest);
            ObjectManager.loadInItems();
            ObjectsToDraw = new Dictionary<GameLocation, MultiTiledObject>();

            //Adds in event handling for the mod.
            ModHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            ModHelper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            ModHelper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            ModHelper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            ModHelper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            ModHelper.Events.Player.Warped += ObjectManager.resources.OnPlayerLocationChanged;
            ModHelper.Events.GameLoop.DayStarted += ObjectManager.resources.DailyResourceSpawn;
            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.Input_CheckForObjectInteraction;
            ModHelper.Events.GameLoop.DayEnding += Serializer.DayEnding_CleanUpFilesForDeletion;
            ModHelper.Events.Display.RenderedWorld += ObjectInteractionHacks.Render_RenderCustomObjectsHeldInMachines;
            //ModHelper.Events.Display.Rendered += MenuHacks.EndOfDay_OnMenuChanged;
            //ModHelper.Events.GameLoop.Saved += MenuHacks.EndOfDay_CleanupForNewDay;
            ModHelper.Events.Multiplayer.ModMessageReceived += MultiplayerUtilities.GetModMessage;
            ModHelper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            ModHelper.Events.GameLoop.Saving += this.GameLoop_Saving;


            //Adds in recipes to the mod.
            VanillaRecipeBook = new VanillaRecipeBook();

            /*
            foreach(var v in Game1.objectInformation)
            {
                string name = v.Value.Split('/')[0];
                ModCore.log(name + "="+v.Key+","+Environment.NewLine,false);
            }
            */

        }

        private void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            /*
            foreach(var v in CustomObjects)
            {
                v.Value.updateInfo();
            }
            */
        }

        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            //MultiplayerUtilities.RequestALLGuidObjects();
        }

        /// <summary>
        /// Loads in textures to be used by the mod.
        /// </summary>
        private void loadInTextures()
        {
            TextureManager.AddTextureManager(Manifest, "Furniture");
            TextureManager.GetTextureManager(Manifest, "Furniture").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Objects", "Furniture"));
            TextureManager.AddTextureManager(Manifest, "InventoryMenu");
            TextureManager.GetTextureManager(Manifest, "InventoryMenu").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Menus", "InventoryMenu"));
            TextureManager.AddTextureManager(Manifest, "Resources.Ore");
            TextureManager.GetTextureManager(Manifest, "Resources.Ore").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Objects", "Resources", "Ore"));
            TextureManager.AddTextureManager(Manifest, "Items.Resources.Ore");
            TextureManager.GetTextureManager(Manifest, "Items.Resources.Ore").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Items", "Resources", "Ore"));
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            /*
            if(e.Button== SButton.U)
            {
                Game1.currentMinigame = new Revitalize.Framework.Minigame.SeasideScrambleMinigame.SeasideScramble();
            }
            */
            /*
            if (e.Button == SButton.Y)
            {
                //Game1.activeClickableMenu = new ItemGrabMenu(Game1.player.Items,false,true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),);
                List<Item> newItems = new List<Item>()
                {
                    new StardewValley.Object(184,10)
                };

                Game1.activeClickableMenu = new Revitalize.Framework.Menus.InventoryTransferMenu(100, 100, 500, 500, newItems, 36);
            }
            */
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            Serializer.returnToTitle();
            ObjectManager = new ObjectManager(Manifest);
        }
        /// <summary>
        /// Must be enabled for the tabled to be placed????
        /// </summary>
        private void loadContent()
        {

            MultiTiledComponent obj = new MultiTiledComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledComponent), Color.White), new BasicItemInformation("CoreObjectTest", "Omegasis.TEST1", "YAY FUN!", "Omegasis.Revitalize.MultiTiledComponent.Test", Color.White, -300, 0, false, 300, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16))), Color.White, false, null, null));
            MultiTiledComponent obj2 = new MultiTiledComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledComponent), Color.White), new BasicItemInformation("CoreObjectTest2", "Omegasis.TEST2", "Some fun!", "Omegasis.Revitalize.MultiTiledComponent.Test", Color.White, -300, 0, false, 300, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 16, 16, 16))), Color.White, false, null, null));
            MultiTiledComponent obj3 = new MultiTiledComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledComponent), Color.White), new BasicItemInformation("CoreObjectTest3", "Omegasis.TEST3", "NoFun", "Omegasis.Revitalize.MultiTiledComponent.Test", Color.White, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 32, 16, 16))), Color.Red, false, null, null));


            obj3.info.lightManager.addLight(new Vector2(Game1.tileSize), new LightSource(4, new Vector2(0, 0), 2.5f, Color.Orange.Invert()), obj3);

            MultiTiledObject bigObject = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledObject), Color.White), new BasicItemInformation("MultiTest", "Omegasis.BigTiledTest", "A really big object", "Omegasis.Revitalize.MultiTiledObject", Color.Blue, -300, 0, false, 500, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(), Color.White, false, null, null));

            bigObject.addComponent(new Vector2(0, 0), obj);
            bigObject.addComponent(new Vector2(1, 0), obj2);
            bigObject.addComponent(new Vector2(2, 0), obj3);

            Recipe pie = new Recipe(new Dictionary<Item, int>()
            {
                [bigObject] = 1
            }, new KeyValuePair<Item, int>(new Furniture(3, Vector2.Zero), 1), new StatCost(100, 50, 0, 0));

            ObjectManager.miscellaneous.Add("Omegasis.BigTiledTest", bigObject);


            Framework.Objects.Furniture.RugTileComponent rug1 = new RugTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(RugTileComponent), Color.White), new BasicItemInformation("Rug Tile", "Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", "A rug tile", "Rug", Color.Brown, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16))), Color.White, true, null, null));

            Framework.Objects.Furniture.RugMultiTiledObject rug = new RugMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(RugMultiTiledObject), Color.White, false), new BasicItemInformation("Simple Rug Test", "Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", "A simple rug for testing", "Rugs", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(), Color.White, true, null, null));

            rug.addComponent(new Vector2(0, 0), rug1);

            ObjectManager.miscellaneous.Add("Omegasis.Revitalize.Furniture.Rugs.RugTest", rug);



            FurnitureFactory.LoadFurnitureFiles();

            SeasideScramble sscGame = new SeasideScramble();
            ArcadeCabinetTile ssc1 = new ArcadeCabinetTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), typeof(ArcadeCabinetTile), Color.White), new BasicItemInformation("Seaside Scramble Arcade Game", "Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", "A arcade to play Seaside Scramble!", "Arcades", Color.LimeGreen, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new Animation(new Rectangle(0, 0, 16, 16)), new Dictionary<string, List<Animation>>()
            {
                {"Animated",new List<Animation>()
                {
                    new Animation(0,0,16,16,60),
                    new Animation(16,0,16,16,60)
                }
                }

            }, "Animated"), Color.White, false, null, null), new Framework.Objects.InformationFiles.Furniture.ArcadeCabinetInformation(sscGame, false));
            ArcadeCabinetTile ssc2 = new ArcadeCabinetTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), typeof(ArcadeCabinetTile), Color.White), new BasicItemInformation("Seaside Scramble Arcade Game", "Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", "A arcade to play Seaside Scramble!", "Arcades", Color.LimeGreen, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new Animation(new Rectangle(0, 16, 16, 16)), new Dictionary<string, List<Animation>>()
            {
                {"Animated",new List<Animation>()
                {
                    new Animation(0,16,16,16,60),
                    new Animation(16,16,16,16,60)
                }
                }

            }, "Animated"), Color.White, false, null, null), new Framework.Objects.InformationFiles.Furniture.ArcadeCabinetInformation(sscGame, false));

            ArcadeCabinetOBJ sscCabinet = new ArcadeCabinetOBJ(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), typeof(ArcadeCabinetOBJ), Color.White, true), new BasicItemInformation("Seaside Scramble Arcade Game", "Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", "A arcade to play Seaside Scramble!", "Arcades", Color.LimeGreen, -300, 0, false, 500, true, true, TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new AnimationManager(), Color.White, true, null, null));
            sscCabinet.addComponent(new Vector2(0, 0), ssc1);
            sscCabinet.addComponent(new Vector2(0, 1), ssc2);


            ObjectManager.miscellaneous.Add("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", sscCabinet);

            //ModCore.log("Added in SSC!");
        }

        private void createDirectories()
        {
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Configs"));

            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics"));
            //Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics","Furniture"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture", "Chairs"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture", "Lamps"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture", "Tables"));
        }

        /// <summary>
        /// Initialize all modular components for this mod.
        /// </summary>
        private void initailizeComponents()
        {
            DarkerNight.InitializeConfig();
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            DarkerNight.SetDarkerColor();
            playerInfo.update();
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            DarkerNight.CalculateDarkerNightColor();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            this.loadContent();

            /*
            if (Game1.IsServer || Game1.IsMultiplayer || Game1.IsClient)
            {
                throw new Exception("Can't run Revitalize in multiplayer due to lack of current support!");
            }
            */
            Serializer.afterLoad();
            ShopHacks.AddOreToClintsShop();
            ObjectInteractionHacks.AfterLoad_RestoreTrackedMachines();


            // Game1.player.addItemToInventory(GetObjectFromPool("Omegasis.BigTiledTest"));
            Game1.player.addItemToInventory(ObjectManager.getChair("Omegasis.Revitalize.Furniture.Chairs.OakChair"));
            //Game1.player.addItemToInventory(GetObjectFromPool("Omegasis.Revitalize.Furniture.Rugs.RugTest"));
            //Game1.player.addItemToInventory(ObjectManager.getTable("Omegasis.Revitalize.Furniture.Tables.OakTable"));
            //Game1.player.addItemToInventory(ObjectManager.getLamp("Omegasis.Revitalize.Furniture.Lamps.OakLamp"));

            //Game1.player.addItemToInventory(ObjectManager.getObject("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble",ObjectManager.miscellaneous));
            //Game1.player.addItemToInventory(ObjectManager.getStorageFuriture("Omegasis.Revitalize.Furniture.Storage.OakCabinet"));
            /*
            StardewValley.Tools.Axe axe = new StardewValley.Tools.Axe();
            Serializer.Serialize(Path.Combine(this.Helper.DirectoryPath, "AXE.json"), axe);
            axe =(StardewValley.Tools.Axe)Serializer.Deserialize(Path.Combine(this.Helper.DirectoryPath, "AXE.json"),typeof(StardewValley.Tools.Axe));
            //Game1.player.addItemToInventory(axe);
            */
            //Game1.player.addItemToInventory(ObjectManager.resources.ores["Test"].getOne());


            //Game1.player.addItemToInventory(ObjectManager.resources.getOre("Tin", 19));
            //Ore tin = ObjectManager.resources.getOre("Tin", 19);
            //Game1.player.addItemToInventory(ObjectManager.GetItem("TinIngot", 1));
            //Game1.player.addItemToInventory(new StardewValley.Object(388, 100));
            Game1.player.addItemsByMenuIfNecessary(new List<Item>()
            {
                new StardewValley.Object(Vector2.Zero, (int)Enums.SDVBigCraftable.Furnace),
                new StardewValley.Object((int)Enums.SDVObject.Coal,10),
                new StardewValley.Object((int)Enums.SDVObject.PrismaticShard,5),
                new StardewValley.Object((int)Enums.SDVObject.Emerald,1),
                new StardewValley.Object((int)Enums.SDVObject.Aquamarine,1),
                new StardewValley.Object((int)Enums.SDVObject.Ruby,1),
                new StardewValley.Object((int)Enums.SDVObject.Amethyst,1),
                new StardewValley.Object((int)Enums.SDVObject.Topaz,1),
                new StardewValley.Object((int)Enums.SDVObject.Jade,1),
                new StardewValley.Object((int)Enums.SDVObject.Diamond,1),
                new StardewValley.Object((int)Enums.SDVObject.IronBar,1),
            });
            //ModCore.log("Tin sells for: " + tin.sellToStorePrice());

            //ObjectManager.resources.spawnOreVein("Omegasis.Revitalize.Resources.Ore.Test", new Vector2(8, 7));
        }

        /*
        public static Item GetObjectFromPool(string objName)
        {
            if (customObjects.ContainsKey(objName))
            {
                CustomObject i = (CustomObject)customObjects[objName].getOne();
                return i;
            }
            else
            {
                throw new Exception("Object Key name not found: " + objName);
            }
        }
        */

        /// <summary>
        ///Logs information to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void log(object message, bool StackTrace = true)
        {
            if (StackTrace)
            {
                ModMonitor.Log(message.ToString() + " " + getFileDebugInfo());
            }
            else
            {
                ModMonitor.Log(message.ToString());
            }
        }

        public static string getFileDebugInfo()
        {
            string currentFile = new System.Diagnostics.StackTrace(true).GetFrame(2).GetFileName();
            int currentLine = new System.Diagnostics.StackTrace(true).GetFrame(2).GetFileLineNumber();
            return currentFile + " line:" + currentLine;
        }

        public static bool IsNullOrDefault<T>(T argument)
        {
            // deal with normal scenarios
            if (argument == null) return true;
            if (object.Equals(argument, default(T))) return true;

            // deal with non-null nullables
            Type methodType = typeof(T);
            if (Nullable.GetUnderlyingType(methodType) != null) return false;

            // deal with boxed value types
            Type argumentType = argument.GetType();
            if (argumentType.IsValueType && argumentType != methodType)
            {
                object obj = Activator.CreateInstance(argument.GetType());
                return obj.Equals(argument);
            }

            return false;
        }
    }
}
