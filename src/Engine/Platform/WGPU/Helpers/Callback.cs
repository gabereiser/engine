using System;

namespace Red.Platform.WGPU.Helpers;

public class CallbackContext<T, V>
{
    public Action<T, V, string, IntPtr> Delegate { get; set; }
    public IntPtr UserData { get; set; }
}
public class Callback<T>
{
    public Action<T, string, IntPtr> Delegate { get; set; }
    public IntPtr UserData { get; set; }
}
