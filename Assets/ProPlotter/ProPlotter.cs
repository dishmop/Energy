using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


// TODO: default values
// TODO: sections of editor window for horizontal labes, vertical labels

[RequireComponent(typeof(RawImage))]
public class ProPlotter : MonoBehaviour {

    /// <summary>
    /// maximum value on y axis
    /// </summary>
    [Tooltip("The maximum value on the y-axis")]
    public float VerticalMax;

    /// <summary>
    /// minimum value on y axis
    /// </summary>
    [Tooltip("The minimum value on the y-axis")]
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

    /// <summary>
    /// string to prefix before each label along vertical axis
    /// </summary>
    [Tooltip("For example, if vertical axis is money, might be '$'")]
    public string VerticalAxisLabelPrefix;

    /// <summary>
    /// string to append after each label along vertical axis
    /// </summary>
    [Tooltip("For example, if vertical axis is volts, might be 'V'")]
    public string VerticalAxisLabelSuffix;

    /// <summary>
    /// string to prefix before each label along horizontal axis
    /// </summary>
    [Tooltip("For example, if horizontal axis is money, might be '$'")]
    public string HorizontalAxisLabelPrefix;

    /// <summary>
    /// string to append after each label along horizontal axis
    /// </summary>
    [Tooltip("For example, if horizontal axis is time, might be 's'")]
    public string HorizontalAxisLabelSuffix;

    /// <summary>
    /// x values less than the max so far will overwrite old ones
    /// </summary>
    public bool AllowOverwrite;

    /// <summary>
    /// automatically recalculates vertical limits when a point is drawn outside
    /// </summary>
    public bool AutoScale;

    /// <summary>
    /// If true, 1000 becomes 1k, 0.005 becomes 5m etc.
    /// </summary>
    public bool UseSIPrefixesVertical;

    /// <summary>
    /// If true, 1000 becomes 1k, 0.005 becomes 5m etc.
    /// </summary>
    public bool UseSIPrefixesHorizontal;

    /// <summary>
    /// If not using SI prefixes, this is the format string
    /// </summary>
    public string VerticalFormatString;

    /// <summary>
    /// If not using SI prefixes, this is the format string
    /// </summary>
    public string HorizontalFormatString;

    /// <summary>
    /// Font to use for the axis labels
    /// </summary>
    public Font LabelFont;

    /// <summary>
    /// If this is not null and there are enough members, these will be used for labels along x axis, instead of numbers
    /// </summary>
    public string[] HorizontalLabels = null;

    /// <summary>
    /// If this is not null and there are enough members, these will be used for labels along y axis, instead of numbers
    /// </summary>
    public string[] VerticalLabels = null;




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

    private bool needsUpdate; // have we added points

    private float labelOffset = 5f;

    private Material lineMat;

    public float VerticalMaxMin; // the minimum possible vertical max

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

        if (AutoScale)
        {
            VerticalMaxMin = VerticalMax;
        }

        texture = new Texture2D(WidthInTexels, HeightInTexels);

        image.texture = texture;
        

        needsUpdate = true;

        rightLimit = HorizontalRange;

        lineMat = new Material(Shader.Find("UI/Default"));
    }
    

    void Update()
    {

        // only redraw if something has changed
        if(needsUpdate)
        {
            if (AutoScale)
            {
                VerticalMax = VerticalMaxMin;
            }

            foreach (var plot in plots)
            {
                plot.Value.Trim();
            }



            var rt = RenderTexture.GetTemporary(WidthInTexels, HeightInTexels);
            RenderTexture.active = rt;
            GL.PushMatrix();
            lineMat.SetPass(0);
            GL.LoadOrtho();
            GL.Clear(false, true, BackgroundColour);

            if (AllowOverwrite)
            {
                GL.Begin(GL.QUADS);
                GL.Color(BackgroundColour*0.8f);
                float x = (LastValue-LeftLimit)/HorizontalRange;
                GL.Vertex3(x,1.0f,0);
                GL.Vertex3(1.0f, 1.0f,0);
                GL.Vertex3(1.0f, 0.0f,0);
                GL.Vertex3(x, 0.0f,0);

                GL.End();
            }


            GL.Begin(GL.LINES);


            Queue<GameObject> vertLabels = new Queue<GameObject>();
            Queue<GameObject> horzLabels = new Queue<GameObject>();

            // find existing labels
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "VertLabel")
                {
                    vertLabels.Enqueue(child.gameObject);
                }
                else if (child.gameObject.name == "HorzLabel")
                {
                    horzLabels.Enqueue(child.gameObject);
                }
            }

            Rect rect = image.GetPixelAdjustedRect();

            // horizontal lines
            float multiplier = VerticalMin / VerticalGridStep;
            float liney = VerticalGridStep * Mathf.Ceil(multiplier);

            int numLabels = Mathf.CeilToInt(VerticalRange / VerticalGridStep);
            int labelnum = 0;

            if (numLabels < 100)
            {

                while (liney < VerticalMax)
                {
                    float y = (liney - VerticalMin) / VerticalRange;

                    DrawLine(0, y, 1.0f, y, GridColour);

                    // vertical axis labels
                    GameObject label;
                    Text textfield;
                    if (vertLabels.Count > 0)
                    {
                        label = vertLabels.Dequeue();
                        textfield = label.GetComponent<Text>();
                    }
                    else
                    {
                        label = new GameObject();
                        label.name = "VertLabel";

                        label.transform.SetParent(transform, false);

                        textfield = label.AddComponent<Text>();
                        textfield.font = LabelFont;
                        textfield.color = Color.black;
                        textfield.alignment = TextAnchor.MiddleRight;

                        label.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 0.5f);
                        label.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 40f);
                    }


                    if (VerticalLabels != null && numLabels <= VerticalLabels.Length)
                    {
                        textfield.text = VerticalLabels[labelnum];
                        labelnum++;
                    }
                    else
                    {
                        string num = liney.ToString(VerticalFormatString);
                        if(UseSIPrefixesVertical)
                        {
                            num = SIPrefix(liney);
                        }
                        textfield.text = VerticalAxisLabelPrefix + num + VerticalAxisLabelSuffix;
                    }
                    label.GetComponent<RectTransform>().localPosition = new Vector3(-rect.width / 2 - labelOffset, rect.height * y - rect.height / 2, 0);

                    liney += VerticalGridStep;
                }
            }

            // vertical lines
            multiplier = LeftLimit / HorizontalGridStep;
            float linex = HorizontalGridStep * Mathf.Ceil(multiplier);

            numLabels = Mathf.CeilToInt(VerticalRange / VerticalGridStep);
            labelnum = 0;

            if (numLabels < 100)
            {
                while (linex < RightLimit)
                {
                    float x = (linex - LeftLimit) / HorizontalRange;

                    DrawLine(x, 0, x, 1, GridColour);

                    // horizontal axis labels
                    GameObject label;
                    Text textfield;
                    if (horzLabels.Count > 0)
                    {
                        label = horzLabels.Dequeue();
                        textfield = label.GetComponent<Text>();
                    }
                    else
                    {
                        label = new GameObject();
                        label.name = "HorzLabel";

                        label.transform.SetParent(transform, false);

                        textfield = label.AddComponent<Text>();
                        textfield.font = LabelFont;
                        textfield.color = Color.black;
                        textfield.alignment = TextAnchor.UpperCenter;

                        label.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
                        label.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 40f);
                    }

                    if (HorizontalLabels != null && numLabels <= HorizontalLabels.Length)
                    {
                        textfield.text = HorizontalLabels[labelnum];
                        labelnum++;
                    }
                    else
                    {
                        string num = linex.ToString(HorizontalFormatString);
                        if (UseSIPrefixesHorizontal)
                        {
                            num = SIPrefix(linex);
                        }
                        textfield.text = HorizontalAxisLabelPrefix + num + HorizontalAxisLabelSuffix;
                    }
                    label.GetComponent<RectTransform>().localPosition = new Vector3(rect.width * x - rect.width / 2, -rect.height / 2 - labelOffset, 0);


                    linex += HorizontalGridStep;
                }
            }


            // remove any unused labels
            while(vertLabels.Count > 0)
            {
                Destroy(vertLabels.Dequeue());
            }
            while(horzLabels.Count > 0)
            {
                Destroy(horzLabels.Dequeue());
            }

            // horizontal axis
            if (VerticalMin <= 0 && VerticalMax > 0) {
                float y = -VerticalMin / VerticalRange;
                DrawLine(0f, y, 1f, y, Color.black);
            }
            

            foreach(var plot in plots)
            {
                plot.Value.Draw();
            }

            GL.End();
            GL.PopMatrix();

            texture.ReadPixels(new Rect(0, 0, WidthInTexels, HeightInTexels), 0, 0);

            texture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

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

    // TODO: write
    public void RemovePlot()
    {

    }

    // TODO: write
    public void RemoveAll()
    {

    }

    /// <summary>
    /// Clears a particular line from the plotter
    /// </summary>
    /// <param name="plotname"></param>
    public void ClearPlot(string plotname)
    {
        plots[plotname].ClearPoints();
        needsUpdate = true;
    }

    /// <summary>
    /// Clears all lines  - but does not remove them - and resets the horizontal axes
    /// </summary>
    public void ClearAll()
    {
        foreach(var plot in plots)
        {
            ClearPlot(plot.Key);
        }

        rightLimit = HorizontalRange;
    }

    /// <summary>
    /// Add a point to existing line
    /// </summary>
    /// <param name="plotName">Name of the line to add to</param>
    /// <param name="horizontalValue">x coordinate of point</param>
    /// <param name="verticalValue">y coordinate of point</param>
    public void AddPoint(string plotName, float horizontalValue, float verticalValue)
    {
        if(AutoScale && verticalValue>VerticalMax)
        {
            VerticalMax = verticalValue;
        }
        if (AutoScale && verticalValue < VerticalMin)
        {
            VerticalMin = verticalValue;
        }

        plots[plotName].AddPoint(horizontalValue, verticalValue);

        needsUpdate = true;
    }

    float LastValue
    {
        get { float ret = float.NegativeInfinity;
        foreach (var plot in plots)
        {
            if (plot.Value.LastValue > ret)
            {
                ret = plot.Value.LastValue;
            }
        }

        return ret;
        }
    }
    

    public class Plot
    {
        ProPlotter parent;
        Color lineColour;

        Queue<Vector2> points;

        Queue<Vector2> oldPoints;

        public float LastValue;

        public Plot(ProPlotter Parent, Color LineColour)
        {
            parent = Parent;
            lineColour = LineColour;

            points = new Queue<Vector2>();
            oldPoints = new Queue<Vector2>();

            LastValue = float.NaN;
        }

        public void AddPoint(float horizontalValue, float verticalValue)
        {
            // must add after last point
            if(horizontalValue<LastValue)
            {
                if (parent.AllowOverwrite)
                {
                    oldPoints = points;
                    points = new Queue<Vector2>();

                    // clamp if necessary
                    if(horizontalValue < parent.LeftLimit)
                    {
                        horizontalValue = parent.LeftLimit;
                    }
                }
                else
                {
                    throw (new System.ArgumentException("horizontalValue must be greater than those previously added - if intended use AllowOverwrite"));
                }
            }

            // add new point to queue
            points.Enqueue(new Vector2(horizontalValue, verticalValue));
            LastValue = horizontalValue;

            if(horizontalValue > parent.RightLimit)
            {
                parent.RightLimit = horizontalValue;
            }
        }

        // remove any now unnecessary points from queue
        public void Trim()
        {
            // remove any from the old lot which have been overwritten
            while (oldPoints.Count>0 && oldPoints.Peek().x < LastValue)
            {
                oldPoints.Dequeue();
            }

            while (points.Count > 0 && points.Peek().x < parent.LeftLimit)
            {
                points.Dequeue();
            }

            if (parent.AutoScale)
            {
                foreach (var point in oldPoints)
                {
                    if (point.y > parent.VerticalMax)
                    {
                        parent.VerticalMax = point.y;
                    }
                }
                foreach (var point in points)
                {
                    if (point.y > parent.VerticalMax)
                    {
                        parent.VerticalMax = point.y;
                    }
                }
            }
        }

        public void Draw()
        {
            Draw(points, lineColour);

            if (oldPoints.Count > 0)
            {
                Draw(oldPoints, lineColour*0.8f);
            }
        }

        public void ClearPoints()
        {
            points.Clear();
            oldPoints.Clear();
            LastValue = float.NegativeInfinity;
        }

        void Draw(Queue<Vector2> pointsToDraw, Color colour)
        {
            GL.Color(colour);

            bool first = true;

            float x0, y0;
            float x1 = 0;
            float y1 = 0;

            foreach (Vector2 point in pointsToDraw)
            {
                x0 = x1;
                y0 = y1;

                x1 = (point.x - parent.LeftLimit) / parent.HorizontalRange;
                y1 = (point.y - parent.VerticalMin) / parent.VerticalRange;

                // clamp data inside to within our draw range
                if (y1 >= parent.HeightInTexels)
                {
                    y1 = parent.HeightInTexels - 1;
                }

                if (y1 < 0)
                {
                    y1 = 0;
                }
                
                if(first)
                {
                    first = false;
                } else
                {
                    parent.DrawLine(x0, y0, x1, y1);
                }
            }
        }
    }

    /// <summary>
    /// Draws a line on the graph
    /// </summary>
    /// <param name="x0">start point x position, in [0,1)</param>
    /// <param name="y0">start point y position, in [0,1)</param>
    /// <param name="x1">end point x position, in [0,1)</param>
    /// <param name="y1">end point y position, in [0,1)</param>
    /// <param name="colour">colour of the line</param>
    public void DrawLine(float x0, float y0, float x1, float y1, Color colour)
    {
        GL.Color(colour);
        GL.Vertex3(x0, y0, 0);
        GL.Vertex3(x1, y1, 0);
    }

    /// <summary>
    /// Draws a line on the graph, using whatever current colour is
    /// </summary>
    /// <param name="x0">start point x position, in [0,1)</param>
    /// <param name="y0">start point y position, in [0,1)</param>
    /// <param name="x1">end point x position, in [0,1)</param>
    /// <param name="y1">end point y position, in [0,1)</param>
    public void DrawLine(float x0, float y0, float x1, float y1)
    {
        GL.Vertex3(x0, y0, 0);
        GL.Vertex3(x1, y1, 0);
    }

    public static string SIPrefix(float number, uint dp=1)
    {
        if (number == 0f)
        {
            return "0";
        }

        if (number < 0)
        {
            return "-" + SIPrefix(-number, dp);
        }

        if(number >= 1e12f){
            return (number / 1e12f).ToString("N"+dp)+"T";
        }else if(number >= 1e9f){
            return (number / 1e9f).ToString("N" + dp) + "G";
        }else if(number >= 1e6f){
            return (number / 1e6f).ToString("N" + dp) + "M";
        }else if(number >= 1e3f){
            return (number / 1e3f).ToString("N" + dp) + "k";
        }
        else if (number >= 1e0f)
        {
            return (number / 1e0).ToString("N" + dp);
        }
        else if (number >= 1e-3f)
        {
            return (number / 1e3f).ToString("N" + dp)+"m";
        }
        else if (number >= 1e-6f)
        {
            return (number / 1e-6f).ToString("N" + dp) + "μ";
        }
        else if (number >= 1e-9f)
        {
            return (number / 1e-9f).ToString("N" + dp) + "n";
        }
        else
        {
            return (number / 1e-12f).ToString("N" + dp) + "p";
        } 
    }
}
