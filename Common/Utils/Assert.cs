

class Assert
{
    public static void IsNotNull(object obj)
    {
        System.Diagnostics.Debug.Assert(obj != null);
    }

    public static void IsNull(object obj)
    {
        System.Diagnostics.Debug.Assert(obj == null);
    }

    public static void IsTrue(object obj)
    {
        System.Diagnostics.Debug.Assert((bool)obj == true);
    }

    public static void IsFalse(object obj)
    {
        System.Diagnostics.Debug.Assert((bool)obj == false);
    }
}
