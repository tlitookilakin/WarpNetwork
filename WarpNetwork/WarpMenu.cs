using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpNetwork
{
    class WarpMenu : IClickableMenu
    {
        static IModHelper helper;
        static IMonitor monitor;

        public GameMenu parent;

        private Rectangle panel = new(384, 373, 18, 18);
        private readonly List<WarpLocation> locs;

        public WarpMenu(List<WarpLocation> locs, int x, int y, int width, int height, GameMenu parent)
      : base(x, y, width, height, false)
        {
            this.parent = parent;
            this.locs = locs;
            if(locs.Count < 1)
            {
                monitor.Log("Warp menu created with no destinations!",LogLevel.Warn);
                exitThisMenuNoSound();
            }
        }

        public static void Init(IModHelper Helper, IMonitor Monitor)
        {
            helper = Helper;
            monitor = Monitor;
            //Texture2D uiTex = helper.Content.Load<Texture2D>("LooseSprites/Cursors", ContentSource.GameContent);
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }
            drawTextureBox(b, Game1.mouseCursors, panel, xPositionOnScreen, yPositionOnScreen, width + 10, height + 10, Color.White);
            Utility.drawTextWithShadow(b, Game1.parseText(helper.Translation.Get("ui-label")), Game1.dialogueFont, new Vector2(xPositionOnScreen, yPositionOnScreen), Color.White);
            base.draw(b);
        }
    }
}
