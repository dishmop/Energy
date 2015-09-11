using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlotManager : MonoBehaviour {

    Texture2D grid;

    public Color backgroundColor;

    private Dictionary<String, Plotter> plots = new Dictionary<String, Plotter>();

    public float min;
    public float max;
    public float verticalGridSize;

    private float minValue;
    private float maxValue;
    private float scale;
    private int floor;
    private int top;

    private int zeroLine = -1;

    private float verticalGridStep;

    Color[] buffer;


    void Start()
    {

        Canvas.ForceUpdateCanvases();

        Rect drawRect = GetComponent<RectTransform>().rect;

        float scalefactor = GetComponentInParent<Canvas>().scaleFactor;

        grid = new Texture2D((int)(drawRect.width*scalefactor), (int)(drawRect.height*scalefactor), TextureFormat.ARGB32, false, false);

        GetComponent<RawImage>().texture = grid;



        buffer = grid.GetPixels();


        floor = 0;
        top = grid.height + floor;

        if (min >= max)
        {
            throw (new System.Exception("Max must be greater than Min"));
        }

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

        verticalGridStep = verticalGridSize / scale;
    }

    void Update()
    {
        // copy background into frame buffer
        Color[] frameBuffer = new Color[buffer.Length];
        Array.Copy(buffer, frameBuffer, buffer.Length);

        // Draw a line at Zero
        DrawLine(frameBuffer, grid.width, grid.height, 0, zeroLine, grid.width - 1, zeroLine, Color.black);

        // Draw horizontal gridlines above zero
        for (float y = zeroLine + verticalGridStep; y < grid.height; y += verticalGridStep)
        {
            DrawLine(frameBuffer, grid.width, grid.height, 0, (int)y, grid.width - 1, (int)y, Color.gray);
        }

        // and below zero
        for (float y = zeroLine - verticalGridStep; y >= 0; y -= verticalGridStep)
        {
            DrawLine(frameBuffer, grid.width, grid.height, 0, (int)y, grid.width - 1, (int)y, Color.gray);
        }


        // If we have graphs then show them
        if (plots.Count > 0)
        {
            foreach (KeyValuePair<String, Plotter> p in plots)
            {
                if (!p.Value.child) 
                {
                    p.Value.Draw(frameBuffer);
                }
            }
        }

        // set the buffer to the texture
        grid.SetPixels(frameBuffer);

        // Update texture with pixels
        grid.Apply(false);
    }

    public void PlotAdd(String plotName, float value)
    {
        if (plots.ContainsKey(plotName)) plots[plotName].Add(value);
    }


    public void PlotCreate(String plotName, Color plotColor)
    {
        if (!plots.ContainsKey(plotName))
        {
            plots.Add(plotName, new Plotter(grid.width, grid.height, minValue, maxValue, floor,top,scale, plotColor));
        }
    }


    public class Plotter
    {

        public Boolean child = false;
        private Plotter Parent;
        private Color plotColor = Color.green;

        private int gridWidth;
        private int gridHeight;

        private float minValue;
        private float maxValue;
        private float scale;
        private int floor;
        private int top;

        private Color[] buffer;
        private int[] data;
        private int dataIndex = -1;
        private bool dataFull = false;

        public Plotter(int gridwidth, int gridheight, float minvalue, float maxvalue, int Floor, int Top, float Scale, Color plotColor)
        {
            this.plotColor = plotColor;

            gridWidth = gridwidth;
            gridHeight = gridheight;

            data = new int[gridWidth];

            floor = Floor;
            top = Top;

            // Calculate verticle scale
            minValue = minvalue;
            maxValue = maxvalue;
            scale = Scale;
        }

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


        public void Draw(Color[] frameBuffer)
        {
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
        }
    }

    static void DrawLine(Color[] buffer, int width, int height, int x0, int y0, int x1, int y1, Color col)
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
