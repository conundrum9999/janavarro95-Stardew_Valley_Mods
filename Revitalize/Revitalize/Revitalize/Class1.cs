﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using System.Xml.Serialization;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Quests;
using Newtonsoft.Json;
using System.IO;
using Revitalize.Resources;
using Revitalize.Objects;
using Revitalize.Objects.Machines;
using StardewValley.Locations;
using Revitalize.Locations;
using Revitalize.Menus;

namespace Revitalize
{
    /// <summary>
    /// TODO:
    /// Get Time lapse code working so that machines propperly work though the night since I technically remove them.
    /// Art. Lots of Art.
    /// Clean up the freaking code. Geeze it's messy.
    /// 
    /// </summary>


    public class Class1 :Mod
    {
        public static string key_binding="P";
        public static string key_binding2 = "E";
        public static string path;
        bool hasCleanedUp;
        const int range = 1;

        bool gametick;

        bool mapWipe;
        
        List<GameLoc> newLoc;

        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.ControlEvents.KeyPressed += ShopCall;
            StardewModdingAPI.Events.GameEvents.UpdateTick +=gameMenuCall;
            StardewModdingAPI.Events.GameEvents.UpdateTick += BedCleanUpCheck;
            StardewModdingAPI.Events.GameEvents.GameLoaded += GameEvents_GameLoaded;
            StardewModdingAPI.Events.GameEvents.OneSecondTick += MapWipe;

            hasCleanedUp = true;
            path = Helper.DirectoryPath;
            newLoc = new List<GameLoc>();
        }


        private void GameEvents_GameLoaded(object sender, EventArgs e)
        {
            Dictionaries.initializeDictionaries();
           

            mapWipe = true;

        }


        public void MapWipe(object sender, EventArgs e)
        {
          
            if (mapWipe == false) return;
            if (Game1.hasLoadedGame == false) return;
            if (Game1.player == null) return;
            if (Game1.player.currentLocation == null) return;
            if (Game1.player.isMoving() == true)
            {


                foreach (var v in Game1.locations)
                {
                    GameLoc R = (new GameLoc(v.Map, v.name));

                    if (R.name == "Town" || R.name == "town")
                    {
                        Log.AsyncO("Adding Town");
                        R = new ModTown(v.Map, v.name);
                    }
                    newLoc.Add(R);
                    Log.AsyncC("DONE1");
                }
                Game1.locations.Clear();
                foreach (var v in newLoc)
                {
                    Game1.locations.Add(v);
                    Log.AsyncC("DONE2");
                }
                Log.AsyncC("DONE");
                mapWipe = false;
            }


            
            
        }

        private void BedCleanUpCheck(object sender, EventArgs e)
        {
            //Game1.options.menuButton = null;

            if (Game1.hasLoadedGame == false) return;
            if (Game1.player == null) return;
            if (Game1.player.currentLocation == null) return;
            //Log.Info(Game1.activeClickableMenu.GetType());
            Vector2 vec = new Vector2(Game1.getMouseX()/Game1.tileSize, Game1.getMouseY()/Game1.tileSize);
           // Log.AsyncY(vec);
            //if (Game1.player.ActiveObject as Light != null) Log.AsyncO((Game1.player.ActiveObject as Light).canBePlacedHere(Game1.player.currentLocation,vec ));

            if ((Game1.player.ActiveObject as Decoration) != null)
            {
                Log.AsyncM((Game1.player.ActiveObject as Decoration).drawPosition);
            }

            if (Game1.player.currentLocation.name == "FarmHouse")
            {
                Vector2 playerAdj = Game1.player.mostRecentBed;

                int x = Convert.ToInt32(playerAdj.X)/Game1.tileSize;
                int y = Convert.ToInt32(playerAdj.Y)/Game1.tileSize;


                
                    if ((Game1.player.getTileY() >= y - range && Game1.player.getTileY() <= y + range) && (Game1.player.getTileX() >= x - range && Game1.player.getTileY() <= x + range))
                    {
                    if (hasCleanedUp == false)
                    {
                        Log.AsyncC("CleanUp!");
                        CleanUp.cleanUpInventory();
                        hasCleanedUp = true;
                    }
                }
                    else
                    {
                    CleanUp.restoreInventory();
                        hasCleanedUp = false;
                    }     
            }
        }

       

        private void gameMenuCall(object sender, EventArgs e)
        {

            
            if (gametick == true)
            {
               // System.Threading.Thread.Sleep(1);
               
                   Game1.activeClickableMenu = new GameMenu();
            }
            gametick = false;
            
        }


        private void ShopCall(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {
            Game1.player.money = 9999;
            Log.AsyncG(Game1.tileSize);

            //Game1.timeOfDay = 2500;
            if (Game1.activeClickableMenu != null) return;
            if (e.KeyPressed.ToString() == key_binding)
            {

                List<Item> objShopList = new List<Item>();

               
                objShopList.Add(new Decoration(1120, Vector2.Zero));
                //  objShopList.Add(new Spawner(3, Vector2.Zero, 9));
                objShopList.Add(new Light(3, Vector2.Zero, LightColors.Aquamarine));

               // my_shop_list.Add((new Decoration(1120, Vector2.Zero)));
                Game1.activeClickableMenu = new StardewValley.Menus.ShopMenu(objShopList, 0, null);
                
                if (Game1.player == null) return;
                
            }

            
            if (e.KeyPressed.ToString() == key_binding2)
            {
                gametick = true;
            
             

            //    string load = Path.Combine(PathOnDisk, "this_thing.json");
            //    Game1.player=ReadFromJsonFile<StardewValley.Farmer>(load);
            }
            
            }
            

    }
}
