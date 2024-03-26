using ShipsForm.Data;
using ShipsForm.Graphic;
using ShipsForm.GUI.Elements;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.Logic;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShipsForm.GUI
{
    public class Painter
    {
        private WriteableBitmap m_wBitmap;
        private Canvas m_canvas;
        private int i_lineWidth = 1;

        private static int i_shiftX = 0;
        private static int i_shiftY = 0;

        private float i_cellScale = ScaleChanger.Scale;
        private Field m_field;

        private SolidColorBrush[] m_brushes;

        private Size m_primScreenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

        private List<IDrawable> m_models = Manager.Drawings;

        public Size Size { get { return m_primScreenSize; } }

        public Painter(Image img, Canvas cvas, Field field, Configuration? sett = null)
        {

            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);
            m_wBitmap = new WriteableBitmap((int)m_primScreenSize.Width, (int)m_primScreenSize.Height, 300, 300, PixelFormats.Rgb24, null);
            img.Source = m_wBitmap;
            m_canvas = cvas;
            m_field = field;
            
            if (m_field.Types is null)
                throw new Exception();
            m_brushes = new SolidColorBrush[m_field.Types.Count];
            for (int i = 0; i < m_field.Types.Count; i++)
                m_brushes[i] = new SolidColorBrush(m_field.Types[i].Color);


            if (sett != null)
            {
                //put code here later
                TileWidth = sett.TileWidth;
            }
            ScaleChanger.Scale = CellScale;
            ScaleChanger.OnChangeScale += ChangeScale;
            DrawCells();
        }

        public float CellScale
        {
            get { return i_cellScale; }
            private set { i_cellScale = value; }
        }

        public void ChangeScale()
        {
            float prevScale = i_cellScale;
            i_cellScale = ScaleChanger.Scale;
            float correlation = i_cellScale / prevScale;
            i_shiftX = (int)(i_shiftX * correlation);
            i_shiftY = (int)(i_shiftY * correlation);
            Console.WriteLine($"Scale was changed on: {i_cellScale}");
            DrawCells();
            DrawFrame();
        }

        public int TileWidth
        {
            get { return i_lineWidth; }
            set { i_lineWidth = value; }
        }
 
        ~Painter()
        {
            ScaleChanger.OnChangeScale -= ChangeScale;
        }

        private void DrawCells()
        {
            int LeftBound = Math.Max(0, (int)Math.Ceiling(-i_shiftX / TileWidth / CellScale));
            int RightBound = Math.Min((int)((Size.Width + -i_shiftX)/ TileWidth / CellScale), m_field.MapLength);
            int TopBound = Math.Max(0, (int)Math.Ceiling(-i_shiftY / TileWidth / CellScale));
            int BottomBound = Math.Min((int)((Size.Height + -i_shiftY) / TileWidth / CellScale), m_field.MapWidth);
            for (int i = LeftBound; i < RightBound; i++)
            {
                int prevTileId = m_field.GetTileId(i, TopBound);
                int twinsAmount = 0;
                Point prevTile = new Point(i, TopBound);

                void FillCustomRect()
                {                    
                    var colorBrush = m_brushes[m_field.GetTileId((int)prevTile.X, (int)prevTile.Y)];
                    var color = colorBrush.Color;
                    var colorData = new byte[] { color.R, color.G, color.B};
                    Int32Rect rect = new Int32Rect((int)(prevTile.X * TileWidth * CellScale) + i_shiftX,
                        (int)(prevTile.Y * TileWidth * CellScale) + i_shiftY,
                        (int)(TileWidth * CellScale), (int)(TileWidth * CellScale) * (1 + twinsAmount));
                    var pixels = new byte[rect.Width * rect.Height * colorData.Length];
                    int index = 0;
                    for (int i = 0; i < rect.Width; i++)
                        for (int j = 0; j < rect.Height; j++)
                            for (int k = 0; k < colorData.Length; k++)
                                pixels[index++] = colorData[k];
                    m_wBitmap.WritePixels(rect, pixels, (rect.Width * m_wBitmap.Format.BitsPerPixel + 7) / 8, 0) ;

                }

                for (int j = TopBound+1; j < BottomBound; j++)
                {
                    int currentTileId = m_field.GetTileId(i, j);
                    if (prevTileId == currentTileId)
                        twinsAmount++;
                    
                    else
                    {
                        FillCustomRect();
                        prevTile = new Point(i, j);
                        twinsAmount = 0;
                    }    
                    prevTileId = currentTileId;
                }
                FillCustomRect();
            }
                
                           
        }

        private void ClearMap()
        {
            if (Application.Current is null)
                return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                m_canvas.Children.Clear();
            });
        }

        private void DrawModels()
        {
            /*Image models_area = (Image)m_cells_bitmap.Clone();
            Graphics g = Graphics.FromImage(models_area);

            */
            foreach(IDrawable model in m_models)
            {
                if (Application.Current is null)
                    break;
                SupportEntities.Point? currentPoint = model.GetCurrentPoint();
                if (currentPoint == null)
                    continue;
                Application.Current.Dispatcher.Invoke(() => 
                {                  
                    ImageSource skin = model.GetSkin(model.GetSize());
                    double rotation = model.GetRotation();

                    Image img = new Image();
                    img.Source = skin;


                    Size modelSize = new Size(img.Source.Width, img.Source.Height);
                    int modifier = (int)(TileWidth * CellScale);
                    int padding = (int)(TileWidth * CellScale / 2);
                    int centerX = (int)(modelSize.Width / 2);
                    int centerY = (int)(modelSize.Height / 2);
                    int pX = padding - centerX;
                    int pY = padding - centerY;
                    RotateImage(img, rotation, new Point(centerX, centerY));
                    Point positionToDrawAModel = new Point((int)(currentPoint.X) * modifier + pX + i_shiftX, (int)(currentPoint.Y) * modifier + pY + i_shiftY);
                    m_canvas.Children.Add(img);
                    Canvas.SetLeft(img, positionToDrawAModel.X);
                    Canvas.SetTop(img, positionToDrawAModel.Y);
                });
                
            }
            /*
            foreach (IDrawable model in m_models)
            {
                SupportEntities.Point? currentPoint = model.GetCurrentPoint();
                if (currentPoint == null)
                    continue;
                Image skin = model.GetSkin();
                double rotation = model.GetRotation();

                Size modelSize = new Size(model.GetSize(), model.GetSize());
                int modifier = (int)(TileWidth * CellScale);
                int padding = (int)(TileWidth * CellScale / 2);
                int pX = padding - modelSize.Width / 2;
                int pY = padding - modelSize.Height / 2;
                Point positionToDrawAShip = new Point((int)(currentPoint.X) * modifier + pX + i_shiftX, (int)(currentPoint.Y) * modifier + pY + i_shiftY);
                Image sizedImg = ChangeSizeImage(skin, modelSize);
                Image rotatedImg = RotateImage(sizedImg, rotation);

                
                g.DrawImage(rotatedImg, positionToDrawAShip);
            }
            this.Image = models_area;*/
        }

        public void OnShift(int stX, int stY)
        {
            /// <summary>
            /// Checks if control will stay in screen bounds under shifting.
            /// </summary>
            /// <param name="stX">Shifting on X</param>
            /// <param name="stY">Shifting on Y</param>
            /// <returns></returns>
            Point GetCheckedBounds(int x, int y)

            {
                Size bounds = new Size((int)(m_field.MapLength * TileWidth * CellScale)-1,
                                         (int)(m_field.MapWidth * TileWidth * CellScale)-1);
                int bottomBound = (int)m_primScreenSize.Height;
                int rightBound = (int)m_primScreenSize.Width;
                int topBound = 0, leftBound = 0;
                int changedX = (int)Math.Min(Math.Max(i_shiftX + x, -bounds.Width + rightBound), leftBound);
                int changedY = (int)Math.Min(Math.Max(i_shiftY + y, -bounds.Height + bottomBound), topBound);
                return new Point(changedX, changedY);
            }
            Point checkedShifts = GetCheckedBounds(stX, stY);
            i_shiftX = (int)(checkedShifts.X - checkedShifts.X % TileWidth);
            i_shiftY = (int)(checkedShifts.Y - checkedShifts.Y % TileWidth);
            DrawCells();
            DrawFrame();
        }

        public static void RotateImage(Image img, double rotationAngle, Point center)
        {
            RotateTransform rotateTransform = new RotateTransform(rotationAngle, center.X, center.Y);
            img.RenderTransform = rotateTransform;
        }

        /// <summary>
        /// Creates bitmap instance directing Size parameter.
        /// </summary>
        /// <param name="size"></param>
        /// <returns>Bitmap instance</returns>

        public void DrawFrame()
        {
            ClearMap();
            DrawModels();
        }
    }
}
