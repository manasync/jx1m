using ProtoBuf;
using System.Collections.Generic;

namespace KiemTheWebSDK.Models
{
    [ProtoContract]
    public class LoginRep
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; } = 0;

        [ProtoMember(2)]
        public string ErorrMsg { get; set; } = "";

        [ProtoMember(3)]
        public string AccessToken { get; set; } = "";

        [ProtoMember(4)]
        public string LastLoginTime { get; set; } = "";

        [ProtoMember(5)]
        public string LastLoginIP { get; set; } = "";
    }

    [ProtoContract]
    public class RegisterRep
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; } = 0;

        [ProtoMember(2)]
        public string ErorrMsg { get; set; } = "";
    }

    [ProtoContract]
    public class BuffServerInfo
    {
        [ProtoMember(1)]
        public int nServerOrder { get; set; }

        [ProtoMember(2)]
        public int nServerID { get; set; }

        [ProtoMember(3)]
        public int nOnlineNum { get; set; }

        [ProtoMember(4)]
        public List<int> listMapOnline { get; set; } = new List<int>();

        [ProtoMember(5)]
        public string strServerName { get; set; } = string.Empty;

        [ProtoMember(6)]
        public string strStartTime { get; set; } = string.Empty;

        [ProtoMember(7)]
        public int nStatus { get; set; }

        [ProtoMember(8)]
        public string strURL { get; set; } = string.Empty;

        [ProtoMember(9)]
        public int nServerPort { get; set; }

        [ProtoMember(10)]
        public string strMaintainTxt { get; set; } = string.Empty;

        [ProtoMember(11)]
        public string strMaintainStarTime { get; set; } = string.Empty;

        [ProtoMember(12)]
        public string strMaintainTerminalTime { get; set; } = string.Empty;

        [ProtoMember(13)]
        public string Msg { get; set; } = string.Empty;
    }

    [ProtoContract]
    public class BuffServerListDataEx
    {
        [ProtoMember(1)]
        public List<BuffServerInfo> ListServerData { get; set; } = new List<BuffServerInfo>();

        [ProtoMember(2)]
        public List<BuffServerInfo> RecommendListServerData { get; set; } = new List<BuffServerInfo>();

        [ProtoMember(3)]
        public BuffServerInfo LastServerLogin { get; set; } = new BuffServerInfo();
    }

    [ProtoContract]
    public class ClientVerifySIDData
    {
        [ProtoMember(1)]
        public string strSID { get; set; } = "";

        [ProtoMember(2)]
        public long lTime { get; set; }

        [ProtoMember(3)]
        public string strMD5 { get; set; } = "";
    }

    [ProtoContract]
    public class ServerVerifySIDData
    {
        [ProtoMember(1)]
        public string strPlatformUserID { get; set; } = "";

        [ProtoMember(2)]
        public string strAccountName { get; set; } = "";

        [ProtoMember(3)]
        public long lTime { get; set; }

        [ProtoMember(4)]
        public string strCM { get; set; } = "1";

        [ProtoMember(5)]
        public string strToken { get; set; } = "";
    }

    [ProtoContract]
    public class GiftCodeRequest
    {
        [ProtoMember(1)]
        public int RoleActive { get; set; }

        [ProtoMember(2)]
        public string CodeActive { get; set; } = "";

        [ProtoMember(3)]
        public int ServerID { get; set; }
    }

    [ProtoContract]
    public class GiftCodeRep
    {
        [ProtoMember(1)]
        public int Status { get; set; }

        [ProtoMember(2)]
        public string Msg { get; set; } = "";

        [ProtoMember(3)]
        public string GiftItem { get; set; } = "";
    }
}
