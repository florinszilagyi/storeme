using System;

namespace storeme.ViewModels
{
    /// <summary>
    /// Rights enumeration
    /// </summary>
    [Flags]
    public enum Rights
    {
        Exec = 1,
        Read = 2,
        Write = 4
    }
}