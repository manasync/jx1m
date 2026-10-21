using System;

namespace Server.Tools
{
    public static class RemotingHelper
    {
        public static object GetObject(Type type, string url)
        {
            // Trong .NET Core / .NET 8 Standalone, KuaFu Remoting tạm ngắt kết nối
            // Khi nâng cấp lên cụm liên server, thay thế bằng gRPC client factory tại đây
            return null;
        }
    }
}
