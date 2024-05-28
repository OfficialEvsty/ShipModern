

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System;
using System.Text;
using ShipsModern.Logic.ShipSystem.ShipNavigation;

namespace ShipsForm.Logic.TilesSystem
{

    public static class MapConstructor
    {
        private static string s_mapFolderPath = Directory.GetCurrentDirectory() + "..\\..\\..\\..\\..\\Maps\\NSR.txt";
        private static StringBuilder m_map = new StringBuilder();
        private static List<string> s_map = new List<string>();
        private static string s_mapImgPath = "..\\..\\..\\..\\Maps\\NorthernSeaRoute.png";
        private static int i_delta = 20;
        public static string Map { get { return m_map.ToString(); } }

        /// <summary>
        /// Reads first txt file in folder.
        /// </summary>
        public static void ReadMap()
        {
            try
            {
                StreamReader sr = new StreamReader(s_mapFolderPath);
                DateTime lastWriteTime = File.GetLastWriteTime(s_mapFolderPath);

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
            catch (Exception ex)
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
                throw new Exception(s_mapFolderPath + " is empty.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Bitmap bitmap = new Bitmap(Image.FromFile(s_mapImgPath));
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color pixCol = bitmap.GetPixel(x, y);
                        bool hasAdded = false;
                        foreach (TileType t in types)
                        {
                            int deltaR = Math.Abs(pixCol.R - t.PixelRGB[0]);
                            int deltaG = Math.Abs(pixCol.G - t.PixelRGB[1]);
                            int deltaB = Math.Abs(pixCol.B - t.PixelRGB[2]);
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
                            m_map.Append("l");
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