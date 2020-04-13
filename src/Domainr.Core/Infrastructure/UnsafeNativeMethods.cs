using System;
using System.Runtime.InteropServices;

namespace Domainr.Core.Infrastructure
{
    internal static class UnsafeNativeMethods
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        public static extern int UuidCreateSequential(out Guid guid);
    }
}