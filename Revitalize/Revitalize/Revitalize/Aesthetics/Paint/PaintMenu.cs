﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System.Collections.Generic;
using System.Linq;
using System;
using Revitalize.Objects;
using StardewModdingAPI;

namespace Revitalize.Menus
{
    public class Pixel
    {
        public Color color;
        public ClickableTextureComponent component;

      public  Pixel(ClickableTextureComponent newComponent, Color newColor)
        {
            component = newComponent;
            color = newColor;
        }

        public Pixel(ClickableTextureComponent newComponent, int Red = 255, int Green = 255, int Blue = 255, int Alpha = 255)
        {
            component = newComponent;
            color = new Color(Red, Green, Blue, Alpha);
        }
    }


    public class PaintMenu : IClickableMenu
    {
        public const int colorPickerTimerDelay = 100;

        private int colorPickerTimer;

        private ColorPicker lightColorPicker;

        private List<ClickableComponent> labels = new List<ClickableComponent>();

        private List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();

        private List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();

        public List<Pixel> pixels = new List<Pixel>();


        public bool colorChanged;

        private ClickableTextureComponent okButton;

        private ClickableTextureComponent cancelButton;

        private ClickableTextureComponent randomButton;

        private TextBox nameBox;

        private TextBox farmnameBox;

        private TextBox favThingBox;

        private bool skipIntro;

        private bool wizardSource;

        private string hoverText;

        private string hoverTitle;

        private ColorPicker lastHeldColorPicker;

        public Canvas CanvasObject;

        private int timesRandom;

        public bool once;

        public PaintMenu(Canvas Obj) : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            this.setUpPositions();
            this.CanvasObject = Obj;
            colorChanged = false;
          //  this.height = this.height ;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            this.setUpPositions();
        }

        private void setUpPositions()
        {

            this.labels.Clear();

            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.pixels = new List<Pixel>();
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, (this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4) / 3, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);

            this.cancelButton = new ClickableTextureComponent("Cancel", new Rectangle(this.xPositionOnScreen + this.width / 4 - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, (this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4) / 3, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            this.randomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.pixelZoom * 12, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false);
            int num = Game1.tileSize * 2;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            if (!this.wizardSource)
            {
                //this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal", new object[0])));
            }
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 16, 1, 1), "Color"));
            this.lightColorPicker = new ColorPicker(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder);
            num += Game1.tileSize + 8;
            for(int x=1; x<=8; x++)
            {
                for(int y=1; y<=8; y++)
                {
                    this.pixels.Add(new Pixel(new ClickableTextureComponent("pixel", new Rectangle((int)(this.xPositionOnScreen*1.2f) + ((Game1.tileSize/2) * x),(int)( this.yPositionOnScreen *-2.0f)+ ((Game1.tileSize/2) * y) , Game1.tileSize/2, Game1.tileSize/2), null, null, Game1.content.Load<Texture2D>(Canvas.whiteTexture), new Rectangle(0, 0, 8, 8), 4f, false), Color.White));
                }

            }
          

            once = false;
            this.lightColorPicker.setColor(Color.White);
        }

        private void optionButtonClick(string name)
        {

            if (name == "Cancel")
            {
                Game1.exitActiveMenu();
                return;
            }

            if (name == "OK")
            {
                if (colorChanged == false)
                {
                    Game1.exitActiveMenu();
                    return;
                }
                //  StardewModdingAPI.Log.AsyncC(this.LightObject.lightColor);

                this.lightColorPicker.setColor(CanvasObject.drawColor);

                //  StardewModdingAPI.Log.AsyncC(this.LightObject.lightColor);


                //UTIL FUNCTION TO GET CORRECT COLOR
                CanvasObject.drawColor = this.lightColorPicker.getSelectedColor();
                //LightObject.lightColor = Util.invertColor(LightObject.lightColor);

                if (!this.canLeaveMenu())
                {
                    return;
                }
                //Game1.player.Name = this.nameBox.Text.Trim();
                //	Game1.player.favoriteThing = this.favThingBox.Text.Trim();
 
                    this.CanvasObject.isPainted = true;
                    Game1.exitActiveMenu();
                    if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
                    {
                        (Game1.currentMinigame as Intro).doneCreatingCharacter();
                    }
                    else if (this.wizardSource)
                    {
                        Game1.flashAlpha = 1f;
                        Game1.playSound("yoba");
                    }
                
            }
        
            Game1.playSound("coin");
        }

   

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            using (List<Pixel>.Enumerator enumerator = pixels.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent3 = (ClickableTextureComponent)enumerator.Current.component;
                    if (clickableTextureComponent3.containsPoint(x, y))
                    {
                        enumerator.Current.color = lightColorPicker.getSelectedColor();
                       // Log.AsyncM("WOOOOOO");
                        //  clickableTextureComponent3.scale = Math.Min(clickableTextureComponent3.scale + 0.02f, clickableTextureComponent3.baseScale + 0.1f);
                    }
                    else
                    {

                        //  clickableTextureComponent3.scale = Math.Max(clickableTextureComponent3.scale - 0.02f, clickableTextureComponent3.baseScale);
                    }
                }
            }

            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.optionButtonClick(this.okButton.name);
                this.okButton.scale -= 0.25f;
                this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
            }

            if (this.cancelButton.containsPoint(x, y))
            {
                this.optionButtonClick(this.cancelButton.name);
                this.cancelButton.scale -= 0.25f;
                this.cancelButton.scale = Math.Max(0.75f, this.cancelButton.scale);
            }

            else if (this.lightColorPicker.containsPoint(x, y))
            {

                CanvasObject.drawColor = this.lightColorPicker.click(x, y);
                CanvasObject.drawColor = Util.invertColor(CanvasObject.drawColor);
                // LightObject.lightColor = Util.invertColor(LightObject.lightColor);
                this.lastHeldColorPicker = this.lightColorPicker;
                colorChanged = true;
            }

            if (this.randomButton.containsPoint(x, y))
            {
                string cueName = "drumkit6";
                if (this.timesRandom > 0)
                {
                    switch (Game1.random.Next(15))
                    {
                        case 0:
                            cueName = "drumkit1";
                            break;
                        case 1:
                            cueName = "dirtyHit";
                            break;
                        case 2:
                            cueName = "axchop";
                            break;
                        case 3:
                            cueName = "hoeHit";
                            break;
                        case 4:
                            cueName = "fishSlap";
                            break;
                        case 5:
                            cueName = "drumkit6";
                            break;
                        case 6:
                            cueName = "drumkit5";
                            break;
                        case 7:
                            cueName = "drumkit6";
                            break;
                        case 8:
                            cueName = "junimoMeep1";
                            break;
                        case 9:
                            cueName = "coin";
                            break;
                        case 10:
                            cueName = "axe";
                            break;
                        case 11:
                            cueName = "hammer";
                            break;
                        case 12:
                            cueName = "drumkit2";
                            break;
                        case 13:
                            cueName = "drumkit4";
                            break;
                        case 14:
                            cueName = "drumkit3";
                            break;
                    }
                }
                Game1.playSound(cueName);
                this.timesRandom++;
                if (Game1.random.NextDouble() < 0.33)
                {
                    if (Game1.player.isMale)
                    {
                        //	Game1.player.changeAccessory(Game1.random.Next(19));
                    }
                    else
                    {
                        //	Game1.player.changeAccessory(Game1.random.Next(6, 19));
                    }
                }
                else
                {
                    //	Game1.player.changeAccessory(-1);
                }
                if (Game1.player.isMale)
                {
                    //Game1.player.changeHairStyle(Game1.random.Next(16));
                }
                else
                {
                    //	Game1.player.changeHairStyle(Game1.random.Next(16, 32));
                }
                Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.R /= 2;
                    c.G /= 2;
                    c.B /= 2;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.B = (byte)Game1.random.Next(15, 50);
                }

                if (Game1.random.NextDouble() < 0.25)
                {
                    //Game1.player.changeSkinColor(Game1.random.Next(24));
                }
                Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.R /= 2;
                    color.G /= 2;
                    color.B /= 2;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.B = (byte)Game1.random.Next(15, 50);
                }
                //Game1.player.changePants(color);
                Color c2 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                c2.R /= 2;
                c2.G /= 2;
                c2.B /= 2;
                if (Game1.random.NextDouble() < 0.5)
                {
                    c2.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c2.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c2.B = (byte)Game1.random.Next(15, 50);
                }
                //Game1.player.changeEyeColor(c2);
                this.randomButton.scale = (float)Game1.pixelZoom - 0.5f;


                // c2 = Util.invertColor(c2);
                colorChanged = true;
                this.lightColorPicker.setColor(c2);
                this.CanvasObject.drawColor = Util.invertColor(c2);

            }
        }

        public override void leftClickHeld(int x, int y)
        {
            this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer <= 0)
            {
                if (this.lastHeldColorPicker != null)
                {

                    if (this.lastHeldColorPicker.Equals(this.lightColorPicker))
                    {
                        colorChanged = true;
                        this.CanvasObject.drawColor = Util.invertColor(this.lightColorPicker.clickHeld(x, y));
                    }
                }
                this.colorPickerTimer = 100;
            }
        }

        public override void releaseLeftClick(int x, int y)
        {

            this.lightColorPicker.releaseClick();
            this.lastHeldColorPicker = null;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!this.wizardSource && key == Keys.Tab)
            {
                if (this.nameBox.Selected)
                {
                    this.farmnameBox.SelectMe();
                    this.nameBox.Selected = false;
                    return;
                }
                if (this.farmnameBox.Selected)
                {
                    this.farmnameBox.Selected = false;
                    this.favThingBox.SelectMe();
                    return;
                }
                this.favThingBox.Selected = false;
                this.nameBox.SelectMe();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            this.hoverTitle = "";
            using (List<ClickableComponent>.Enumerator enumerator = this.leftSelectionButtons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent = (ClickableTextureComponent)enumerator.Current;
                    if (clickableTextureComponent.containsPoint(x, y))
                    {
                        clickableTextureComponent.scale = Math.Min(clickableTextureComponent.scale + 0.02f, clickableTextureComponent.baseScale + 0.1f);
                    }
                    else
                    {
                        clickableTextureComponent.scale = Math.Max(clickableTextureComponent.scale - 0.02f, clickableTextureComponent.baseScale);
                    }
                }
            }
            using (List<ClickableComponent>.Enumerator enumerator = this.rightSelectionButtons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent2 = (ClickableTextureComponent)enumerator.Current;
                    if (clickableTextureComponent2.containsPoint(x, y))
                    {
                        clickableTextureComponent2.scale = Math.Min(clickableTextureComponent2.scale + 0.02f, clickableTextureComponent2.baseScale + 0.1f);
                    }
                    else
                    {
                        clickableTextureComponent2.scale = Math.Max(clickableTextureComponent2.scale - 0.02f, clickableTextureComponent2.baseScale);
                    }
                }
            }
          

            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
            }
            else
            {
                this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
            }


            if (this.cancelButton.containsPoint(x, y))
            {
                this.cancelButton.scale = Math.Min(this.cancelButton.scale + 0.02f, this.cancelButton.baseScale + 0.1f);
            }
            else
            {
                this.cancelButton.scale = Math.Max(this.cancelButton.scale - 0.02f, this.cancelButton.baseScale);
            }
            this.randomButton.tryHover(x, y, 0.25f);
            this.randomButton.tryHover(x, y, 0.25f);
        }

        public bool canLeaveMenu()
        {
            return this.wizardSource || (Game1.player.name.Length > 0 && Game1.player.farmName.Length > 0 && Game1.player.favoriteThing.Length > 0);
        }

        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);
            foreach (ClickableComponent current in this.labels)
            {
                string text = "";
                Color color = Game1.textColor;

                Utility.drawTextWithShadow(b, current.name, Game1.smallFont, new Vector2((float)current.bounds.X, (float)current.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                if (text.Length > 0)
                {
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(current.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (float)(current.bounds.Y + Game1.tileSize / 2)), color, 1f, -1f, -1, -1, 1f, 3);
                }
            }

            foreach(var v in this.pixels)
            {
                v.component.draw(b,v.color,1f);
            }
            this.cancelButton.draw(b, Color.White, 0.75f);

            if (this.canLeaveMenu())
            {
                this.okButton.draw(b, Color.White, 0.75f);
            }
            else
            {
                this.okButton.draw(b, Color.White, 0.75f);
                this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
            }

            this.lightColorPicker.draw(b);

            if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
            {
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, Game1.tileSize * 4), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            this.randomButton.draw(b);

            if (once == false)
            {
                Color c = Util.invertColor(CanvasObject.drawColor);

                this.lightColorPicker.setColor(c);
                once = true;
            }

            base.drawMouse(b);
        }
    }
}
