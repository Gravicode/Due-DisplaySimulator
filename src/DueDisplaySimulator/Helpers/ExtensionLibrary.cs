using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DueDisplaySimulator.Helpers
{
    public static class ExtensionLibrary
    {
        static int WIDTH_SCREEN = 240;
        static int HEIGHT_SCREEN = 180;
        static Bitmap screenImg = null;
        static Graphics gfx = null;
        static bool IsGraphicsInitialized = false;
        static Image? Result { set; get; }

        public static EventHandler<string> Print;
        public static void PrintLn(string arg)
        {
            Trace.WriteLine(arg);
            Print?.Invoke(null, arg + Environment.NewLine);
        }

        public static void LcdShow()
        {
            if (!IsGraphicsInitialized) initGfx();
            gfx.Flush();
            Result = screenImg;
        }
        public static void DrawFillRect(int color, int x, int y, int width, int height)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
        
            gfx.FillRectangle(brush, new Rectangle(x, y, width, height));
        }
            public static void DrawRectangle(int color, int x, int y, int width, int height)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            var pen = new Pen(brush);
            gfx.DrawRectangle(pen, new Rectangle(x,y,width,height));
        }
            public static void LcdCircle(int color, int x, int y, int radius)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            var pen = new Pen(brush);
            gfx.DrawEllipse(pen, x, y, radius*2,radius*2);
        }
            public static void LcdPixel(int color, int x, int y)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            gfx.FillRectangle(brush, x, y, 1, 1);
        }
            public static void LcdLine(int color, int x1, int y1, int x2, int y2)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            var pen = new Pen(brush);
            gfx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
        }
            public static void LcdConfig(int address,int config,int chipselect,int datacontrol)
        {
            //do nothing
        }

        public static void LcdImg(byte[] img, int x, int y, int transform)
        {
            if (!IsGraphicsInitialized) initGfx();
            var bmp = new Bitmap(new MemoryStream(img));
            gfx.DrawImage(bmp, new Point(x, y));
            
        }
        public static void LcdText(string text, int color, int x, int y)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            Font font = new Font("Arial", 12);
            gfx.DrawString(text, font, brush, new PointF(x, y));
        }

        static Dictionary<int, string> colorMap = new()
        {
            {0, "0x000000" }
            ,
            {1, "0xFFFFFF"}
            ,
            {2, "0xFF0000"}
            ,
            {3, "0x32CD32"}
            ,
            {4, "0x0000FF"}
            ,
            {5, "0xFFFF00"}
            ,
            {6, "0x00FFFF"}
            ,
            {7, "0xFF00FF"}
            ,
            {8, "0xC0C0C0"}
            ,
            {9, "0x808080"}
            ,
            {10, "0x800000"}
            ,
            {11, "0xBAB86C"}
            ,
            {12, "0x00FF00"}
            ,
            {13, "0xA020F0"}
            ,
            {14, "0x008080"}
            ,
            {15, "0x000080"}
        };
        static Color GetColorByIndex(int color)
        {
            if (colorMap.ContainsKey(color))
            {
                var hexColor = colorMap[color];
                var col = System.Drawing.ColorTranslator.FromHtml(hexColor);
                return col;
            }
            return Color.FromArgb(color);
        }
        public static void LcdTextS(string text, int color, int x, int y, int scalewidth, int scaleheight)
        {
            
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            Font font = new Font("Arial", 12);
            gfx.DrawString(text, font, brush, new PointF(x, y));
        }
        public static void LcdClear(int color)
        {
            if (!IsGraphicsInitialized) initGfx();
            var brush = new SolidBrush(GetColorByIndex(color));
            gfx.FillRectangle(brush, new Rectangle(0, 0, WIDTH_SCREEN, HEIGHT_SCREEN));
        }

        public static void initGfx()
        {
            screenImg = new Bitmap(WIDTH_SCREEN, HEIGHT_SCREEN);
            gfx = Graphics.FromImage(screenImg);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            IsGraphicsInitialized = true;
            //LcdClear(Color.White.ToArgb());
        }

        public static Image? GetScreen()
        {
            return Result;
        }
    }
}
