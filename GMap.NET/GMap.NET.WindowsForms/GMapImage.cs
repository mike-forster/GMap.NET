using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using GMap.NET.Internals;
using GMap.NET.MapProviders;

namespace GMap.NET.WindowsForms
{
    /// <summary>
    ///     image abstraction
    /// </summary>
    public class GMapImage : PureImage
    {
        public Image Img;

        public override void Dispose()
        {
            if (Img != null)
            {
                Img.Dispose();
                Img = null;
            }

            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }
    }

    /// <summary>
    ///     image abstraction proxy
    /// </summary>
    public class GMapImageProxy : PureImageProxy
    {
        GMapImageProxy()
        {
        }

        public static void Enable()
        {
            GMapProvider.TileImageProxy = Instance;
        }

        public static readonly GMapImageProxy Instance = new GMapImageProxy();

        internal ColorMatrix ColorMatrix;

        static readonly bool Win7OrLater = Stuff.IsRunningOnWin7OrLater();

        public override PureImage FromStream(Stream stream)
        {
            try
            {
                var m = Image.FromStream(stream, true, !Win7OrLater);
                if (m != null)
                {
                    //return new GMapImage {Img = ColorMatrix != null ? ApplyColorMatrix(m, ColorMatrix) : m};
                    return new GMapImage {Img = ColorMatrix != null ? ApplyElevationTransform(m) : m};
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FromStream: " + ex);
            }

            return null;
        }

        public override bool Save(Stream stream, PureImage image)
        {
            var ret = image as GMapImage;
            bool ok = true;

            if (ret.Img != null)
            {
                // try png
                try
                {
                    ret.Img.Save(stream, ImageFormat.Png);
                }
                catch
                {
                    // try jpeg
                    try
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        ret.Img.Save(stream, ImageFormat.Jpeg);
                    }
                    catch
                    {
                        ok = false;
                    }
                }
            }
            else
            {
                ok = false;
            }

            return ok;
        }

        Bitmap ApplyColorMatrix(Image original, ColorMatrix matrix)
        {
            // create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);

            using (original) // destroy original
            {
                // get a graphics object from the new image
                using (var g = Graphics.FromImage(newBitmap))
                {
                    // set the color matrix attribute
                    using (var attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(matrix);
                        g.DrawImage(original,
                            new Rectangle(0, 0, original.Width, original.Height),
                            0,
                            0,
                            original.Width,
                            original.Height,
                            GraphicsUnit.Pixel,
                            attributes);
                    }
                }
            }

            return newBitmap;
        }

        Bitmap ApplyElevationTransform(Image original)
        {
            var newBitmap = new Bitmap(original);
            Color pixel;

            // Change each pixel colour based on elevation
            for (int i = 0; i < newBitmap.Width; i++)
            {
                for (int j = 0; j < newBitmap.Height; j++)
                {
                    pixel = newBitmap.GetPixel(i, j);
                    double elevation = 0.1 * (double)(65536 * pixel.R + 256 * pixel.G + pixel.B) - 10000;
                    newBitmap.SetPixel(i, j, mapElevationToColour(elevation));
                }
            }
            
            // Draw it
            using (var g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(newBitmap,
                    new Rectangle(0, 0, original.Width, original.Height),
                    0,
                    0,
                    original.Width,
                    original.Height,
                    GraphicsUnit.Pixel);
            }
            return newBitmap;
        }

        /// <summary>
        /// Map the elevation in meters to a colour to display
        /// </summary>
        /// <param name="elevation"></param>
        /// <returns></returns>
        private Color mapElevationToColourX(double elevation, ColorBlend cb)
        {

            int R, G, B;
            double maxElevation = 1300D;
            if (elevation > maxElevation)
            {
                return Color.FromArgb(255, 255, 255);
            }
            if (elevation < 0)
            {
                return Color.FromArgb(0, 0, 0);
            }
            double elevationPercent = elevation / maxElevation;
            int i = 1;
            while (elevationPercent >= cb.Positions[i])
            {
                i++;
            }
            R = cb.Colors[i].R - (int)(elevationPercent * (cb.Colors[i].R - cb.Colors[i - 1].R));
            G = cb.Colors[i].G - (int)(elevationPercent * (cb.Colors[i].G - cb.Colors[i - 1].G));
            B = cb.Colors[i].B - (int)(elevationPercent * (cb.Colors[i].B - cb.Colors[i - 1].B));
            //if ((R < 0) || (G < 0) || (B < 0))
            //if (elevationPercent > 0.4)
            //{
            //    Debug.Print("Hi");
            //}
            return Color.FromArgb(R, G, B);
        }

        readonly int[,] ColourArrayX = new int[,]
        {
            { 0x6e,0x95,0x4d },
            { 0x86,0xa6,0x5a },
            { 0x9e,0xb8,0x67 },
            { 0xb6,0xc9,0x74 },
            { 0xce,0xdb,0x81 },
            { 0xe6,0xed,0x8e },
            { 0xff,0xff,0x9c },
            { 0xe7,0xd9,0x84 },
            { 0xcf,0xb4,0x6c },
            { 0xb7,0x8f,0x54 },
            { 0x9f,0x6a,0x3c },
            { 0xa2,0x74,0x4c },
            { 0xa6,0x7e,0x5c },
            { 0xa9,0x88,0x6c },
            { 0xad,0x93,0x7d },
            { 0xb0,0x9d,0x8d },
            { 0xb4,0xa7,0x9e },
            { 0xbb,0xbc,0xbf }
        };
        readonly int[,] ColourArray = new int[,]
        {
            { 0x00, 0xc6, 0xa7 },
            { 0x00, 0xd9, 0x70 },
            { 0x00, 0xe9, 0x3e },
            { 0x00, 0xf9, 0x0e },
            { 0x37, 0xff, 0x00 },
            { 0x76, 0xff, 0x00 },
            { 0xb7, 0xff, 0x00 },
            { 0xf0, 0xf0, 0x00 },
            { 0xff, 0xc8, 0x00 },
            { 0xff, 0xb9, 0x00 },
            { 0xff, 0x98, 0x00 },
            { 0xfa, 0x88, 0x04 },
            { 0xea, 0x7f, 0x14 },
            { 0xd8, 0x7f, 0x26 },
            { 0xc7, 0x7f, 0x37 },
            { 0xa8, 0x71, 0x39 },
            { 0x79, 0x53, 0x2d },
            { 0x4a, 0x36, 0x22 },
            { 0x24, 0x1e, 0x18 }
        };

        private const int ElevationChangeBetweenColours = 75;

        private Color mapElevationToColour(double elevation)
        {
            int index = (int)(elevation / ElevationChangeBetweenColours);
            int offset = (int)(elevation % ElevationChangeBetweenColours);

            if (elevation < 5)
            {
                return Color.FromArgb(0, 0x54, 0xff);
            }
            Color col;
            if (index >= (ColourArray.Length / 3) - 1)
            {
                index = (ColourArray.Length) / 3 - 1;
                int R0 = ColourArray[index, 0];
                int G0 = ColourArray[index, 1];
                int B0 = ColourArray[index, 2];
                col = Color.FromArgb(0xFF, R0, G0, B0);
            }
            else
            {
                int R0 = ColourArray[index, 0];
                int G0 = ColourArray[index, 1];
                int B0 = ColourArray[index, 2];
                int R1 = ColourArray[index + 1, 0];
                int G1 = ColourArray[index + 1, 1];
                int B1 = ColourArray[index + 1, 2];

                R0 += offset * (R1 - R0) / ElevationChangeBetweenColours;
                G0 += offset * (G1 - G0) / ElevationChangeBetweenColours;
                B0 += offset * (B1 - B0) / ElevationChangeBetweenColours;

                col = Color.FromArgb(0xFF, R0, G0, B0);
            }
            return col;
        }
    }
}
