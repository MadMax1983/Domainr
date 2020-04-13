using System;

namespace Domainr.Core.Infrastructure
{
    public static class Uuid
    {
        public static Guid Create()
        {
            const int RPC_S_OK = 0;

            if (UnsafeNativeMethods.UuidCreateSequential(out var guid) != RPC_S_OK)
            {
                guid = Guid.NewGuid();
            }

            var str = guid.ToString().Split('-');

            var tmp = $"{str[4].Substring(0, 8)}-{str[4].Substring(8, 4)}-{str[2]}-{str[3]}-{str[0]}{str[1]}";

            Guid.TryParse(tmp, out guid);

            return guid;
        }
    }
}