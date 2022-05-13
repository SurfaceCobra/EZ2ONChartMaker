using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{
    public static class TextDrawer
    {
        public static Image Draw(string text, Font font, Color textColor, int maxWidth)
        {
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            SizeF textSize = drawing.MeasureString(text, font, maxWidth);

            StringFormat sf = new StringFormat();
            sf.Trimming = StringTrimming.Word;
            img.Dispose();
            drawing.Dispose();

            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);
            drawing.CompositingQuality = CompositingQuality.HighQuality;
            drawing.InterpolationMode = InterpolationMode.HighQualityBilinear;
            drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
            drawing.SmoothingMode = SmoothingMode.HighQuality;
            drawing.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            drawing.Clear(Color.Transparent);

            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, new RectangleF(0, 0, textSize.Width, textSize.Height), sf);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();
            return img;
        }
    }
}
