using System;
using System.Threading;

public class Caller
{
    public static bool Try(Action func)
    {
        try
        {
            func();
            return true;
        }
        catch (Exception exp)
        {
            Logger.Error(exp);
            return false;
        }
    }

    public static void EnterMutex(Mutex mutex, Action voidFunc)
    {
        if (mutex.WaitOne())
        {
            voidFunc();
            mutex.ReleaseMutex();
        }
    }

    public static void TryEnterMutex(Mutex mutex, Action voidFunc)
    {
        try
        {
            EnterMutex(mutex, voidFunc);
        }
        catch (Exception exp)
        {
            mutex.ReleaseMutex();
            Logger.Error(exp);
        }
    }
}
