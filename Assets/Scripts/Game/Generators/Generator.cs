public abstract class Generator {
    public GeneratorPanel panel;

    protected bool on;

    public abstract float Output(float time, Season season);

    public virtual void Update() { }

    public void CallUpdate()
    {
        // get the number of times this has been called
        var type = GetType();
        var field = type.GetField("number");
        int n = (int)field.GetValue(null);

        n++;

        if (n <= panel.activeSlider.value)
        {
            on = true;
        }
        else
        {
            on = false;
        }

        // if called on all objects, start again next time
        if(n == panel.Number)
        {
            n = 0;
        }

        // pass back
        field.SetValue(null, n);

        Update();
    }
}
