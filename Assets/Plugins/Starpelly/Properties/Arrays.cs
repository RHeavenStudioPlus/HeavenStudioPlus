using System;

namespace Starpelly.Properties
{
    public static class Arrays
    {
        public static void Push<T>(ref T[] table, object value)
        {
            Array.Resize(ref table, table.Length + 1);
            table.SetValue(value, table.Length - 1);
        }
    }
}