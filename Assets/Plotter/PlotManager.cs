using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlotManager : MonoBehaviour {

    Texture2D grid;

    public Color backgroundColor;

    private Dictionary<String, Plotter> plots = new Dictionary<String, Plotter>();


    void Start()
    {

        Canvas.ForceUpdateCanvases();

        Rect drawRect = GetComponent<RectTransform>().rect;

        float scalefactor = GetComponentInParent<Canvas>().scaleFactor;

        grid = new Texture2D((int)(drawRect.width*scalefactor), (int)(drawRect.height*scalefactor), TextureFormat.ARGB32, false, false);

        GetComponent<RawImage>().texture = grid;
    }

    void OnGUI()
    {

        //GUI.depth = GUIDepth;

        // If we have graphs then show them
        if (plots.Count > 0)
        {
            foreach (KeyValuePair<String, Plotter> p in plots)
            {
                if (!p.Value.child) 
                {
                    p.Value.Draw();
                }
            }
        }
    }

    /// <summary>
    /// Add a value to plot graph
    /// </summary>
    /// <param name="plotName"></param>
    /// <param name="value"></param>
    public void PlotAdd(String plotName, float value)
    {
        if (plots.ContainsKey(plotName)) plots[plotName].Add(value);
    }

    /// <summary>
    /// Instantiate a new new plot graph
    /// </summary>
    /// <param name="plotName"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void PlotCreate(String plotName, float min, float max, Color plotColor, Vector2 pos)
    {
        if (!plots.ContainsKey(plotName))
        {
            plots.Add(plotName, new Plotter(plotName, grid, min, max, plotColor, GetComponent<RectTransform>().rect));
        }
    }

    /// <summary>
    /// Create child plotter
    /// </summary>
    /// <param name="plotName"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="plotColor"></param>
    /// <param name="parent"></param>
    //public void PlotCreate(String plotName, float min, float max, Color plotColor, String parentName)
    //{
    //    if (!plots.ContainsKey(plotName) && plots.ContainsKey(parentName))
    //    {
    //        plots.Add(plotName, new Plotter(plotName, grid, min, max, plotColor, plots[parentName]));
    //    }
    //}

    //public void PlotCreate(String plotName, Color plotColor, String parentName)
    //{
    //    if (!plots.ContainsKey(plotName) && plots.ContainsKey(parentName))
    //        plots.Add(plotName, new Plotter(plotName, grid, plotColor, plots[parentName]));
    //}

    /// <summary>
    /// Plotter class for generating graphs
    /// </summary>
    public class Plotter
    {

        public Boolean child = false;
        private Plotter Parent;
        private String name = "";
        private Color plotColor = Color.green;
        Rect gridRect;

        private Texture2D grid;
        private int gridWidth;
        private int gridHeight;

        private float minValue;
        private float maxValue;
        private float scale;
        private int floor;
        private int top;

        private int verticalGridStep;

        private Color[] buffer;
        private int[] data;
        private int dataIndex = -1;
        private bool dataFull = false;

        private int zeroLine = -1;

        private Dictionary<String, Plotter> children = new Dictionary<string, Plotter>();

        public Plotter(String name, Texture2D blankGraph, float min, float max, Color plotColor, Rect drawRect)
        {

            this.name = name;
            this.plotColor = plotColor;
            gridRect = drawRect;//new Rect(pos.x, pos.y, blankGraph.width, blankGraph.height);

            grid = blankGraph;// new Texture2D(blankGraph.width, blankGraph.height);
            gridWidth = grid.width;
            gridHeight = grid.height;


            buffer = blankGraph.GetPixels();
            data = new int[gridWidth];

            floor = 0;
            top = gridHeight + floor;

            // Calculate verticle scale
            minValue = min;
            maxValue = max;
            scale = (max - min) / top;

            if (max > 0 && min < 0)
            {
                zeroLine = (int)((-minValue) / scale) + floor;
            }
            else
            {
                zeroLine = 0;
            }


            float verticalGridSize = 10.0f;

            verticalGridStep = (int)(verticalGridSize / scale);
        }

        /// <summary>
        /// Add data to buffer
        /// </summary>
        /// <param name="y">Value to add</param>
        public void Add(float y)
        {

            int yPos = floor;

            // Move to next position in buffer
            dataIndex++;
            if (dataIndex == gridWidth) { dataIndex = 0; dataFull = true; }

            // Add value to buffer. If outside range, then set to min/max
            if (y > maxValue) yPos = top;
            else if (y < minValue) yPos = floor;
            else yPos = (int)((y - minValue) / scale) + floor;

            data[dataIndex] = yPos;
        }


        /// <summary>
        /// Draw the graph.  (Must be called from OnGui)
        /// </summary>
        public void Draw()
        {
            // copy background into frame buffer
            Color[] frameBuffer = new Color[buffer.Length];
            Array.Copy(buffer, frameBuffer, buffer.Length);

            // Draw a line at Zero
            DrawLine(frameBuffer, gridWidth, gridHeight, 0, zeroLine, gridWidth-1, zeroLine, Color.black);

            // Draw horizontal gridlines above zero
            for (int y = zeroLine + verticalGridStep; y < gridHeight; y+= verticalGridStep)
            {
                DrawLine(frameBuffer, gridWidth, gridHeight, 0, y, gridWidth-1, y, Color.gray);
            }

            // and below zero
            for (int y = zeroLine - verticalGridStep; y >= 0; y -= verticalGridStep)
            {
                DrawLine(frameBuffer, gridWidth, gridHeight, 0, y, gridWidth-1, y, Color.gray);
            }



            int x = gridWidth-2;

            // Plot Data in buffer back from current position back to zero
            for (int i = dataIndex - 1; i >= 0; i--)
            {
                //grid.SetPixel(x, data[i], plotColor);

                DrawLine(frameBuffer, gridWidth, gridHeight, x, data[i], x + 1, data[i + 1], plotColor);
                x--;
            }

            // Plot data in buffer from last position down to current position
            if (dataFull)
            {
                if (x >= 0)
                {
                    DrawLine(frameBuffer, gridWidth, gridHeight, x, data[gridWidth - 1], x + 1, data[0], plotColor);
                    x--;

                    for (int i = gridWidth - 2; i > dataIndex; i--)
                    {
                        //grid.SetPixel(x, data[i], plotColor);

                        DrawLine(frameBuffer, gridWidth, gridHeight, x, data[i], x + 1, data[i + 1], plotColor);

                        x--;
                    }
                }
            }

            // set the buffer to the texture
            grid.SetPixels(frameBuffer);

            // Update texture with pixels
            grid.Apply(false);

            //// Draw all children graphs
            //if (children.Count > 0)
            //{
            //    foreach (KeyValuePair<String, Plotter> p in children)
            //    {
            //        p.Value.DrawChild();
            //    }

            //}

            // Draw graph
            //GUI.DrawTexture(gridRect, grid);

        }


        void DrawLine(Color[] buffer, int width, int height, int x0, int y0, int x1, int y1, Color col)
        {
            if (x0 < 0 || x0 >= width || y0 < 0 || y0 >= height)
            {
                throw (new System.Exception("point 0 out of range"));
            }

            if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height)
            {
                throw (new System.Exception("point 1 out of range"));
            }


            int dy = (int)(y1 - y0);
            int dx = (int)(x1 - x0);
            int stepx, stepy;

            if (dy < 0) { dy = -dy; stepy = -1; }
            else { stepy = 1; }
            if (dx < 0) { dx = -dx; stepx = -1; }
            else { stepx = 1; }

            dy <<= 1;
            dx <<= 1;

            float fraction = 0;

            buffer[y0 * width + x0] = col;
            buffer[y1 * width + x1] = col;
            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while (Mathf.Abs(x0 - x1) > 1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    buffer[y0 * width + x0] = col;
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while (Mathf.Abs(y0 - y1) > 1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    buffer[y0 * width + x0] = col;
                }
            }
        }

    }
}
