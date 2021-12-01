using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpNetwork
{
    struct NinePatch
    {
        public Texture2D source;
        public Rectangle outer;
        public Rectangle inner;
        public Color color;

        public NinePatch(Texture2D source, Rectangle outer, Rectangle inner, Color color)
        {
            this.source = source;
            this.outer = outer;
            this.inner = inner;
            this.color = color;
        }

        public void Draw(SpriteBatch b, Rectangle region)
        {
            DrawRow(b, new Rectangle(region.X, region.Y, region.Width, inner.Y), inner.Y, outer.Y);
            DrawRow(b, new Rectangle(region.X, region.Y + inner.Y, region.Width, region.Height - (outer.Height - inner.Height)), inner.Height, inner.Y);
            DrawRow(b, new Rectangle(region.X, region.Y + region.Height - (outer.Height - inner.Bottom), region.Width, outer.Height - inner.Bottom), inner.Bottom, outer.Height - inner.Bottom);
        }
        void DrawRow(SpriteBatch b, Rectangle region, int srch, int srcy)
        {
            b.Draw(source, new Rectangle(region.X, region.Y, inner.X, region.Height), new Rectangle(outer.X, srcy, inner.X, srch), color);
            b.Draw(source, new Rectangle(region.X + inner.X, region.Y, region.Width - (outer.Width - inner.Width), region.Height), 
                new Rectangle(inner.X, srcy, inner.Width, srch), color);
            b.Draw(source, new Rectangle(region.X + inner.X + inner.Width, region.Y, outer.Width - (inner.Width + inner.X), region.Height), 
                new Rectangle(inner.X + inner.Width, srcy, outer.Width - (inner.Width + inner.X), srch), color);
        }
    }
}
