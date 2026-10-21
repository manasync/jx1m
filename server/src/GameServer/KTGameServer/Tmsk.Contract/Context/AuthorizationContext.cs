using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace KF.Contract
{
    [Serializable]
    public class KuaFuClientContext
    {
        private static readonly AsyncLocal<KuaFuClientContext> _currentContext = new AsyncLocal<KuaFuClientContext>();

        public static KuaFuClientContext Current
        {
            get => _currentContext.Value;
            set => _currentContext.Value = value;
        }

        public int ServerId;
        public int ClientId;
        public int GameType;
        public string Token;
        public Dictionary<int, int> MapClientCountDict;

        public KuaFuClientContext()
        {
            Token = NetHelper.GetLocalAddressIPs();
        }

        public static bool CheckConnectClient()
        {
            return Current != null;
        }
    }
}
