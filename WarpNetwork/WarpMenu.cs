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

        private readonly Rectangle panel = new(384, 373, 18, 18);
        private readonly List<WarpLocation> locs;
        private readonly string title;
        private readonly int titleW;
        private static readonly Color shadow = Color.Black * 0.33f;

        public WarpMenu(List<WarpLocation> locs, int x, int y, int width, int height)
      : base(x, y, width, height, true)
        {
            this.locs = locs;
            if(locs.Count < 1)
            {
                monitor.Log("Warp menu created with no destinations!",LogLevel.Warn);
                exitThisMenuNoSound();
            }
            title = helper.Translation.Get("ui-label");
            titleW = (int)Game1.dialogueFont.MeasureString(title).X + 33 + 36;
        }

        public static void Init(IModHelper Helper, IMonitor Monitor)
        {
            helper = Helper;
            monitor = Monitor;
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }
            drawTextureBox(b, Game1.mouseCursors, panel, xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 3f);
            drawTitleBox(b, title);
            base.draw(b);
            drawMouse(b);
        }
        private void drawTitleBox(SpriteBatch b, string text)
        {
            int offset = (width - titleW) / 2;
            //shadows
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset - 6, yPositionOnScreen - 4, 36, 54), new Rectangle(325, 318, 12, 18), shadow);
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset + 36 - 6, yPositionOnScreen - 4, titleW - 36 - 36, 54), new Rectangle(337, 318, 1, 18), shadow);
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + width - offset - 42, yPositionOnScreen - 4, 36, 54), new Rectangle(338, 318, 12, 18), shadow);
            //scroll
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset, yPositionOnScreen - 10, 36, 54), new Rectangle(325, 318, 12, 18), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + offset + 36, yPositionOnScreen - 10, titleW - 36 - 36, 54), new Rectangle(337, 318, 1, 18), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOnScreen + width - offset - 36, yPositionOnScreen - 10, 36, 54), new Rectangle(338, 318, 12, 18), Color.White);
            //text
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont, new Vector2(xPositionOnScreen + offset + 36, yPositionOnScreen - 8), Game1.textColor);
        }
    }
}
