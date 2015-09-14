using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RawImage))]
public class FreePlotter : MonoBehaviour {

    /// <summary>
    /// maximum value on y axis
    /// </summary>
    public float VerticalMax;

    /// <summary>
    /// minimum value on y axis
    /// </summary>
    public float VerticalMin;

    /// <summary>
    /// total range of x axis
    /// </summary>
    public float HorizontalRange;

    /// <summary>
    /// distance between horizontal gridlines
    /// </summary>
    public float VerticalGridStep;

    /// <summary>
    /// distance between vertical grid lines
    /// </summary>
    public float HorizontalGridStep;

    /// <summary>
    /// width of internal texture
    /// Set to 0 to automatically calculate
    /// </summary>
    public int WidthInTexels;

    /// <summary>
    /// height of internal texture
    /// Set to 0 to automatically calculate
    /// </summary>
    public int HeightInTexels;

    /// <summary>
    /// background colour of plot. can be transparent
    /// </summary>
    public Color BackgroundColour;

    /// <summary>
    /// grid line colour
    /// </summary>
    public Color GridColour;


    public float VerticalRange
    {
        get { return VerticalMax - VerticalMin; }
    }

    private float rightLimit;

    public float RightLimit
    {
        get { return rightLimit; }
        set { rightLimit = value; }
    }

    public float LeftLimit
    {
        get { return rightLimit - HorizontalRange; }
    }

    private Dictionary<string, Plot> plots = new Dictionary<string, Plot>(); // collection of lines on this plot


    private RawImage image; // the rawimage component we draw to
    private Texture2D texture; // the texture we actually draw to

    private Color[] buffer; // the buffer we actually actually draw to

    private bool needsUpdate; // have we added points

    void Start()
    {
        image = GetComponent<RawImage>();

        // only works for canvas in screen space!
        if (HeightInTexels == 0 || WidthInTexels == 0)
        {
            Rect rect = image.GetPixelAdjustedRect();
            HeightInTexels = (int)(rect.height);
            WidthInTexels = (int)(rect.width);
        }

        if(VerticalMax<=VerticalMin)
        {
            throw (new System.ArgumentOutOfRangeException("VerticalMax must be greater than VerticalMin"));
        }

        texture = new Texture2D(WidthInTexels, HeightInTexels);

        buffer = texture.GetPixels();

        image.texture = texture;
        

        needsUpdate = true;

        rightLimit = HorizontalRange;
    }

    void Update()
    {
        // only redraw if something has changed
        if(needsUpdate)
        {
            Clear();

            // horizontal lines
            float multiplier = VerticalMin / VerticalGridStep;
            float liney = VerticalGridStep * Mathf.Ceil(multiplier);

            while (liney < VerticalMax)
            {
                int y = (int)(HeightInTexels * (liney - VerticalMin) / VerticalRange);

                DrawLine(0, y, WidthInTexels - 1, y, GridColour);

                liney += VerticalGridStep;
            }

            // vertical lines
            multiplier = LeftLimit / HorizontalGridStep;
            float linex = HorizontalGridStep * Mathf.Ceil(multiplier);

            while (linex < RightLimit)
            {
                int x = (int)(WidthInTexels * (linex - LeftLimit) / HorizontalRange);

                DrawLine(x, 0, x, HeightInTexels - 1, GridColour);

                linex += HorizontalGridStep;
            }

            // horizontal axis
            if (VerticalMin <= 0 && VerticalMax > 0) {
                int y = (int)(-HeightInTexels * VerticalMin / VerticalRange);
                DrawLine(0, y, WidthInTexels - 1, y, Color.black);
            }



            foreach(var plot in plots)
            {
                plot.Value.Trim();
                plot.Value.Draw();
            }
            texture.SetPixels(buffer);
            texture.Apply();

            needsUpdate = false;
        }
    }

    /// <summary>
    /// Create a new line on the plotter
    /// </summary>
    /// <param name="name">Name of the line</param>
    /// <param name="lineColour">Colour of the line</param>
    public void NewPlot(string name, Color lineColour)
    {
        plots.Add(name, new Plot(this, lineColour));
    }

    /// <summary>
    /// Add a point to existing line
    /// </summary>
    /// <param name="plotName">Name of the line to add to</param>
    /// <param name="horizontalValue">x coordinate of point</param>
    /// <param name="verticalValue">y coordinate of point</param>
    public void AddPoint(string plotName, float horizontalValue, float verticalValue)
    {
        if(verticalValue >= VerticalMax || verticalValue < VerticalMin)
        {
            throw (new System.ArgumentOutOfRangeException("verticalValue not in range - allow some tolerance"));
        }

        plots[plotName].AddPoint(horizontalValue, verticalValue);

        needsUpdate = true;
    }




    // clears plot to BackgroundColour
    void Clear()
    {
        for(uint i=0; i<buffer.Length; i++)
        {
            buffer[i] = BackgroundColour;
        }
    }

    public class Plot
    {
        FreePlotter parent;
        Color lineColour;

        Queue<Vector2> points;

        float lastValue;

        public Plot(FreePlotter Parent, Color LineColour)
        {
            parent = Parent;
            lineColour = LineColour;

            points = new Queue<Vector2>();

            lastValue = float.NaN;
        }

        public void AddPoint(float horizontalValue, float verticalValue)
        {
            // must add after last point
            if(horizontalValue<lastValue)
            {
                throw (new System.ArgumentException("horizontalValue must be greater than those previously added"));
            }

            // add new point to queue
            points.Enqueue(new Vector2(horizontalValue, verticalValue));

            if(horizontalValue > parent.RightLimit)
            {
                parent.RightLimit = horizontalValue;
            }
        }

        // remove any now unnecessary points from queue
        public void Trim()
        {
            if(points.Count == 0)
            {
                return;
            }
            while (points.Peek().x < parent.LeftLimit)
            {
                points.Dequeue();
            }
        }

        public void Draw()
        {
            bool first = true;

            int x0, y0;
            int x1 = 0;
            int y1 = 0;

            foreach(Vector2 point in points)
            {
                x0 = x1;
                y0 = y1;

                x1 = (int)(parent.WidthInTexels * (point.x - parent.LeftLimit) / parent.HorizontalRange);
                y1 = (int)(parent.HeightInTexels * (point.y - parent.VerticalMin) / parent.VerticalRange);
                
                if(first)
                {
                    first = false;
                } else
                {
                    parent.DrawLine(x0, y0, x1, y1, lineColour);
                }
            }
        }
    }



    // Bresenham's algorithm, from https://www.cs.umd.edu/class/fall2003/cmsc427/bresenham.html
    public void DrawLine(int x0, int y0, int x1, int y1, Color colour)
    {
        int x = x0;
        int y = y0;

        int dx = x1 - x0;
        int s1 = dx > 0 ? 1 : (dx < 0 ? -1 : 0);
        dx = dx > 0 ? dx : -dx;
        int dy = y1 - y0;
        int s2 = dy > 0 ? 1 : (dy < 0 ? -1 : 0);
        dy = dy > 0 ? dy : -dy;

        bool swap = false;

        if (dy > dx)
        {
            int temp = dx;
            dx = dy;
            dy = temp;
            swap = true;
        }

        int D = 2 * dy - dx;

        for (int i = 0; i < dx; i++)
        {
            DrawPoint(x, y, colour);

            while(D>=0)
            {
                D -= 2 * dx;
                if(swap)
                {
                    x += s1;
                } else
                {
                    y += s2;
                }
            }

            D += 2 * dy;

            if(swap)
            {
                y += s2;
            } else
            {
                x += s1;
            }
        }
    }

    void DrawPoint(int x, int y, Color colour)
    {
        buffer[x + WidthInTexels * y] = colour;
    }
}
