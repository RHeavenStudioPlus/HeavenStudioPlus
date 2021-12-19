using System;
using System.Runtime.InteropServices;
using Starpelly.Enums.Windows;

namespace Starpelly.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }
}