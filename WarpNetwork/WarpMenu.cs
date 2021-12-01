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
        static NinePatch bg;
        static NinePatch slotDark;
        static NinePatch slotLight;
        static NinePatch scrollBg;
        static NinePatch banner;
        static IModHelper helper;
        static IMonitor monitor;

        GameMenu parent;
        Rectangle panel;

        public WarpMenu(int x, int y, int width, int height, GameMenu parent)
      : base(x, y, width, height, false)
        {
            this.parent = parent;
            panel = new Rectangle(x, y, width, height);
        }

        public static void Init(IModHelper Helper, IMonitor Monitor)
        {
            helper = Helper;
            monitor = Monitor;
            Texture2D uiTex = helper.Content.Load<Texture2D>("LooseSprites/Cursors", ContentSource.GameContent);
            bg = new NinePatch(uiTex, new Rectangle(384, 373, 18, 18), new Rectangle(5, 6, 8, 7), Color.White);
            slotDark = new NinePatch(uiTex, new Rectangle(256, 256, 10, 10), new Rectangle(2, 2, 6, 6), Color.White);
            slotLight = new NinePatch(uiTex, new Rectangle(267, 256, 10, 10), new Rectangle(2, 2, 6, 6), Color.White);
            scrollBg = new NinePatch(uiTex, new Rectangle(403, 383, 6, 6), new Rectangle(1, 2, 3, 3), Color.White);
            banner = new NinePatch(uiTex, new Rectangle(325, 318, 25, 18), new Rectangle(12, 9, 1, 0), Color.White);
        }

        public override void draw(SpriteBatch b)
        {
            //scrolltab - 435, 463, 6, 10
            //close button - 337, 494, 12, 12
            //up arrow - 421, 459, 11, 12
            //down arrow - 421, 472, 11, 12
            bg.Draw(b, panel);
            slotDark.Draw(b, new Rectangle(panel.X + 8, panel.Y + 8, width - 16, 16));
            slotLight.Draw(b, new Rectangle(panel.X + 8, panel.Y + 24, width - 16, 16));
            banner.Draw(b, new Rectangle(panel.X + 8, panel.Y - 8, 128, 18));
            Utility.drawTextWithShadow(b, Game1.parseText(helper.Translation.Get("ui-label")), Game1.dialogueFont, new Vector2(panel.X + 12, panel.Y - 4), Color.White);
            base.draw(b);
        }
    }
}
