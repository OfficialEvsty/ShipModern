
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShipsForm.Logic.TilesSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PixelColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    public static class MapConstructor
    {
        private static string s_mapFolderPath = Directory.GetCurrentDirectory()+ "..\\..\\..\\..\\..\\Maps\\NSR.txt";
        private static StringBuilder m_map = new StringBuilder();
        private static List<string> s_map = new List<string>();
        private static string s_mapImgPath = "..\\..\\..\\..\\Maps\\NorthernSeaRoute.png";
        private static int i_delta = 32;
        public static string Map { get { return m_map.ToString(); } }

        public static void CopyPixels(this BitmapSource source, PixelColor[,] pixels, int stride, int offset)
        {
            var height = source.PixelHeight;
            var width = source.PixelWidth;
            var pixelBytes = new byte[height * width];
            source.CopyPixels(pixelBytes, stride, 0);
            int y0 = offset / width;
            int x0 = offset - width * y0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    pixels[x + x0, y + y0] = new PixelColor
                    {
                        Blue = pixelBytes[(y * width + x) + 0],
                        Green = pixelBytes[(y * width + x) + 1],
                        Red = pixelBytes[(y * width + x) + 2],
                    };
        }

        /// <summary>
        /// Reads first txt file in folder.
        /// </summary>
        public static void ReadMap()
        {
            try
            {
                StreamReader sr = new StreamReader(s_mapFolderPath);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    s_map.Add(line);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Writes string map data to file and stores it in folder.
        /// </summary>
        /// <param name="strMap"></param>
        private static void WriteMap(List<string> strMap)
        {
            try
            {
                StreamWriter sw = new StreamWriter(s_mapFolderPath);
                foreach (string str in strMap)
                    sw.WriteLine(str);
                sw.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Produces character-map from image bitmap data.
        /// </summary>
        /// <param name="bitmap"></param>
        public static List<string> GenMap(List<TileType> types)
        {
            try
            {
                ReadMap();
                if (s_map.Count > 0)
                    return s_map;
                throw new Exception(s_mapFolderPath+" is empty.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

                BitmapImage bitmap = new BitmapImage(new Uri(s_mapImgPath, System.UriKind.Relative));
                int height = bitmap.PixelHeight;
                int width = bitmap.PixelWidth;
                int stride = width * bitmap.Format.BitsPerPixel / 8;
                PixelColor[,] pixels = new PixelColor[width, height];
                CopyPixels(bitmap, pixels, stride, 0);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        bool hasAdded = false;
                        var pixCol = pixels[x, y];
                        foreach (TileType t in types)
                        {
                            int deltaR = Math.Abs(pixCol.Red - t.PixelRGB[0]);
                            int deltaG = Math.Abs(pixCol.Green - t.PixelRGB[1]);
                            int deltaB = Math.Abs(pixCol.Blue - t.PixelRGB[2]);
                            bool isColorSimiliar = deltaR < i_delta &&
                                                   deltaG < i_delta &&
                                                   deltaB < i_delta;
                            if (isColorSimiliar)
                            {
                                m_map.Append(t.Symbol);
                                hasAdded = true;
                            }
                        }
                        if (!hasAdded)
                            m_map.Append(" ");
                        hasAdded = false;
                    }
                    s_map.Add(m_map.ToString());
                    m_map.Clear();
                }
                
                WriteMap(s_map);
                return s_map;
            }    
        }
    }
}
