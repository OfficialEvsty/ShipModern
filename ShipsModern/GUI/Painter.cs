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
using System.Windows.Shapes;
using System.Linq;

namespace ShipsForm.GUI
{
    public class UIIdentifier
    {
        public static readonly DependencyProperty Id =
            DependencyProperty.RegisterAttached("Id", typeof(int), typeof(UIIdentifier), new PropertyMetadata(default(int)));

        public static void SetId(UIElement element, int id)
        {
            element.SetValue(Id, id);
        }

        public static int GetId(UIElement element)
        {
            return (int)element.GetValue(Id);
        }
    }


    public class Painter
    {
        private WriteableBitmap m_wBitmap;
        private Canvas m_canvas;

        private int[] i_activeUIIndexes;

        private int i_lineWidth = 1;

        private static int i_shiftX = 0;
        private static int i_shiftY = 0;

        private float i_cellScale = ScaleChanger.Scale;
        private Field m_field;

        private SolidColorBrush[] m_brushes;

        private Size m_primScreenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

        private Dictionary<int, IDrawable> m_models = Manager.Drawings;

        private Dictionary<int, IPathDrawable> m_paths = Manager.Paths;

        public static Action<int[]>? UpdateCaravanShips;

        private bool IsUIElementOnCanvas(int id)
        {
            foreach (UIElement ui in m_canvas.Children)
            {
                if (UIIdentifier.GetId(ui) == id)
                {
                    return true;
                }
            }
            return false;
        }

        private UIElement GetUIElement(int id) 
        {
            foreach (UIElement ui in m_canvas.Children)
            {
                if(UIIdentifier.GetId(ui) == id)
                {
                    return ui;
                }
                    
            }
            throw new Exception("UIElement with directed ID doesn't exist in this scope.");
        }
        private void AddUI(int id, bool isModel = true)
        {
            UIElement CreateUiModel(int id)
            {
                var model = m_models[id];

                ImageSource skin = model.GetSkin(model.GetSize());

                Image img = new Image();
                img.Source = skin;
                return img;
            }

            UIElement CreateUiShipPath(int id)
            {
                Polyline p = new Polyline();
                return p;
            }

            if (isModel)
            {

                var ui = CreateUiModel(id);
                UIIdentifier.SetId(ui, id);
                m_canvas.Children.Add(ui);
            }
            else
            {
                var ui = CreateUiShipPath(id);
                UIIdentifier.SetId(ui, id);
                m_canvas.Children.Insert(0, ui);
            }
        }
        private void RemoveUI(int id)
        {
            m_canvas.Children.Remove(GetUIElement(id));
                     
        }


        public Size Size { get { return m_primScreenSize; } }

        public Painter(Image img, Canvas modelCvas, Field field, Configuration? sett = null)
        {
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);
            m_wBitmap = new WriteableBitmap((int)m_primScreenSize.Width, (int)m_primScreenSize.Height, 300, 300, PixelFormats.Rgb24, null);
            img.Source = m_wBitmap;
            m_canvas = modelCvas;
            m_field = field;
            UpdateCaravanShips += ClearUIs;


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
            //DrawShipPaths();
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
            int RightBound = Math.Min((int)((Size.Width + -i_shiftX)/ TileWidth / CellScale), m_field.MapWidth);
            int TopBound = Math.Max(0, (int)Math.Ceiling(-i_shiftY / TileWidth / CellScale));
            int BottomBound = Math.Min((int)((Size.Height + -i_shiftY) / TileWidth / CellScale), m_field.MapLength);
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

        /// <summary>
        /// Removes inactive uielements from canvas.
        /// </summary>
        /// <param name="ids"></param>
        private void ClearUIs(int[] ids)
        {
            if (Application.Current is null)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                int[] checkedIds = ids.Where(id => m_canvas.Children.OfType<UIElement>().Select(x => UIIdentifier.GetId(x)).ToArray()
.Any(x => x == id)).ToArray();
                foreach (int id in checkedIds)
                {
                    RemoveUI(id);
                }
            });
        }

        /// <summary>
        /// Checks inactive IDrawable and removes them.
        /// </summary>
        private void ControlActiveUIElements()
        {
            int[] inactiveIDs;
            inactiveIDs = m_models.ToDictionary(entry => entry.Key, entry => entry.Value)
            .Where(x => x.Value.GetCurrentPoint() == null)
            .Select(x => x.Key)
            .Concat(m_paths.ToDictionary(entry => entry.Key, entry => entry.Value)
            .Where(x => !x.Value.IsPath())
            .Select(x => x.Key))
            .ToArray();


            ClearUIs(inactiveIDs);
            i_activeUIIndexes = m_models.Where(x => !inactiveIDs.Any(y => y == x.Key)).Select(x => x.Key).Concat(m_paths.Where(x => !inactiveIDs.Any(y => y == x.Key)).Select(x => x.Key)).ToArray();
        }

        private void DrawModels()
        {
            // || m_canvasElementsDict.Any(y => y.Key == x && y.Value.RenderSize.Height == )
             Application.Current.Dispatcher.Invoke(() =>
            {
                int[] modelIds = i_activeUIIndexes.Where(x => m_models.Any(y => y.Key == x)).ToArray();
                int[] uiIdsOnCanvas = m_canvas.Children.OfType<UIElement>().Select(x => UIIdentifier.GetId(x)).ToArray();
                int[] unshownIds = modelIds
                    .Where(x => !uiIdsOnCanvas.Any(y => y == x)).ToArray();



                int[] resizedIds = modelIds.Where(x => uiIdsOnCanvas
                .Any(y => y == x && ((Image)GetUIElement(y)).Height != m_models[x].GetSize())).ToArray();
                foreach (int id in unshownIds)
                {
                    if(IsUIElementOnCanvas(id)) { continue; }
                    AddUI(id, isModel: true);
                }

                foreach (int id in resizedIds)
                {
                    if (IsUIElementOnCanvas(id)) { continue; }
                    AddUI(id, isModel: true);
                }

                foreach (int id in modelIds)
                {
                    var model = m_models[id];
                    var ui = GetUIElement(id);


                    SupportEntities.Point? currentPoint = model.GetCurrentPoint();

                    if (currentPoint == null)
                        continue;
                    double rotation = model.GetRotation();
                    Size modelSize = new Size(ui.RenderSize.Width, ui.RenderSize.Height);
                    int modifier = (int)(TileWidth * CellScale);
                    int padding = (int)(TileWidth * CellScale / 2);
                    int centerX = (int)(modelSize.Width / 2);
                    int centerY = (int)(modelSize.Height / 2);
                    int pX = padding - centerX;
                    int pY = padding - centerY;
                    RotateImage((Image)ui, rotation, new Point(centerX, centerY));
                    Point positionToDrawAModel = new Point((int)(currentPoint.X) * modifier + pX + i_shiftX, (int)(currentPoint.Y) * modifier + pY + i_shiftY);
                    Canvas.SetLeft(ui, positionToDrawAModel.X);
                    Canvas.SetTop(ui, positionToDrawAModel.Y);
                }
            });
                
        }

        public void DrawShipPaths()
        {
            int[] activePaths = i_activeUIIndexes.Where(x => m_paths.Any(y => y.Key == x)).ToArray();
            foreach(int id in activePaths)
            {
                Polyline pathLine;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var observer = m_paths[id];
                    if (!observer.IsPath()) { return; }
                    if (!IsUIElementOnCanvas(id)) { AddUI(id, isModel: false); }                    
                    pathLine = (Polyline)GetUIElement(id);
                    var points = observer.GetPoints();
                    var scaledPoints = new Point[points.Length];
                    for (int i = 0; i < points.Length; i++)
                        scaledPoints[i] = new Point((int)(points[i].X * CellScale + i_shiftX), (int)(points[i].Y * CellScale + i_shiftY));
                    PointCollection pc = new PointCollection(scaledPoints);
                    pathLine.Points = pc;
                    pathLine.StrokeThickness = 2;
                    pathLine.Stroke = Brushes.DarkGoldenrod;
                    Canvas.SetLeft(pathLine, 0);
                    Canvas.SetTop(pathLine, 0);
                        
                });
                   
            }
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
                Size bounds = new Size((int)(m_field.MapWidth * TileWidth * CellScale)-1,
                                         (int)(m_field.MapLength * TileWidth * CellScale)-1);
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
            try 
            {
                ControlActiveUIElements();
                DrawModels();
                DrawShipPaths();
            } catch (Exception ex) { }
            
        }
    }
}
