using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlimFlan.IconEncoder;

namespace HddTaskMonitor
{
    class Logic
    {
        private readonly Color _okColor;
        private readonly Color _lowSpaceColor;

        private readonly string _drive;

        private readonly Dictionary<int, Icon> _iconCache = new Dictionary<int, Icon>();

        public Logic(string drive, Color okColor, Color lowSpaceColor)
        {
            _drive = drive;
            _okColor = okColor;
            _lowSpaceColor = lowSpaceColor;
        }

        private Icon GetOrRenderIcon(int free)
        {
            // all values over 99 turn into "OK"
            if (free > 99)
                free = 100;

            Icon icon;
            if (!_iconCache.TryGetValue(free, out icon))
            {
                icon = RenderIcon(free);
                _iconCache[free] = icon;
            }

            return icon;
        }

        private Icon RenderIcon(int free)
        {
            if (free > 99)
            {
                return RenderIcon("OK", _okColor);
            }
            else if (free > 10)
            {
                return RenderIcon(free.ToString("D2"), _okColor);
            }
            else
            {
                return RenderIcon(free.ToString("D2"), _lowSpaceColor);
            }
        }

        private Icon RenderIcon(string val, Color color)
        {
            Bitmap bmp = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // TODO: blink with a red rect when 
                //g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 16, 16);

                var font = new Font(FontFamily.GenericMonospace, 11);
                g.DrawString(val, font, new SolidBrush(color), -4, 0);
            }

            // See: https://www.codeproject.com/Articles/7122/Dynamically-Generating-Icons-safely
            //var icon = Icon.FromHandle(bmp.GetHicon());
            var icon = Converter.BitmapToIcon(bmp);
            return icon;
        }

        private string GetTooltip(long freeBytes)
        {
            if (freeBytes < 1024)
            {
                return $"{freeBytes} bytes free";
            }
            else if (freeBytes < (1024 * 1024))
            {
                double freeKb = freeBytes / 1024D;
                return $"{freeKb:#.#} KB free";
            }
            else if (freeBytes < (1024 * 1024 * 1024))
            {
                double freeMb = freeBytes / (1024D * 1024D);
                return $"{freeMb:#.#} MB free";
            }
            else if (freeBytes < (1024L * 1024L * 1024L * 1024L))
            {
                double freeGb = freeBytes / (1024D * 1024D * 1024D);
                return $"{freeGb:#.#} GB free";
            }
            else
            {
                double freeTb = freeBytes / (1024D * 1024D * 1024D * 1024D);
                return $"{freeTb:#.#} TB free";
            }
        }

        private long GetFreeBytes()
        {
            var driveInfo = new DriveInfo(_drive);
            return driveInfo.AvailableFreeSpace;
        }

        private int GetFreeGb(long freeBytes)
        {
            // showing 1 gb as 1024^3 bytes, the way windows does it
            int freeSpaceGb = (int)Math.Round(freeBytes / (double)(1024*1024*1024));
            return freeSpaceGb;
        }

        public (Icon icon, string tooltip) GetHddInfo()
        {
            long freeBytes = GetFreeBytes();

            Icon icon = GetOrRenderIcon(GetFreeGb(freeBytes));
            string tooltip = GetTooltip(freeBytes);
            

            return (icon, tooltip);
        }
    }
}
