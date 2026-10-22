using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using KiemTheWebSDK.Database;
using KiemTheWebSDK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoBuf;

var builder = WebApplication.CreateBuilder(args);

// Database configuration: SQLite by default (file kiemthe_sdk.db)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=kiemthe_sdk.db";
builder.Services.AddDbContext<SdkDbContext>(options => options.UseSqlite(connectionString));

// Secret key matching GameServer AppConfig.xml <Token ... webkey="9377(*)#mst9" />
var webKey = builder.Configuration["WebKey"] ?? "9377(*)#mst9";

var app = builder.Build();

// Ensure DB is initialized and seed default server & test account
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SdkDbContext>();
    db.Database.EnsureCreated();

    if (!db.GameServers.Any())
    {
        db.GameServers.Add(new GameServerEntry
        {
            nServerID = 1,
            nServerOrder = 1,
            strServerName = "Biện Kinh 1",
            strURL = "127.0.0.1",
            nServerPort = 3001,
            nOnlineNum = 0,
            nStatus = 1,
            isTestServer = 3,
            HttpServicePort = 8889,
            strMaintainTxt = "Máy chủ hoạt động bình thường"
        });
        db.SaveChanges();
    }

    if (!db.LoginAccounts.Any())
    {
        db.LoginAccounts.Add(new LoginAccount
        {
            LoginName = "test1",
            Password = CreateMD5("123456"),
            Phone = "0901234567",
            Email = "test1@kiemthe.local",
            Status = 0,
            Date = DateTime.UtcNow,
            AccessToken = ""
        });
        db.SaveChanges();
    }
}

// Helpers
static string CreateMD5(string input)
{
    using var md5 = MD5.Create();
    var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
    var sb = new StringBuilder();
    for (int i = 0; i < bytes.Length; i++)
        sb.Append(bytes[i].ToString("x2"));
    return sb.ToString();
}

static byte[] ToProtoBytes<T>(T obj)
{
    using var ms = new MemoryStream();
    Serializer.Serialize(ms, obj);
    return ms.ToArray();
}

static T FromProtoBytes<T>(byte[] data)
{
    using var ms = new MemoryStream(data);
    return Serializer.Deserialize<T>(ms);
}

// Enable Static Files serving for local CDN (/res)
app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
{
    ServeUnknownFileTypes = true
});

app.MapGet("/res/Version.xml", () => {
    string p = File.Exists("/app/wwwroot/res/Version.xml") ? "/app/wwwroot/res/Version.xml" : "client/src/Assets/StreamingAssets/Version.xml";
    if (File.Exists(p)) return Results.File(Path.GetFullPath(p), "application/xml");
    return Results.NotFound();
});

app.MapGet("/res/UpdateList.xml", () => {
    string p = File.Exists("/app/wwwroot/res/UpdateList.xml") ? "/app/wwwroot/res/UpdateList.xml" : "client/src/Assets/StreamingAssets/UpdateList.xml";
    if (File.Exists(p)) return Results.File(Path.GetFullPath(p), "application/xml");
    return Results.NotFound();
});

app.MapGet("/res/{device}/Version.xml", (string device) => {
    string p = File.Exists("/app/wwwroot/res/Version.xml") ? "/app/wwwroot/res/Version.xml" : "client/src/Assets/StreamingAssets/Version.xml";
    if (File.Exists(p)) return Results.File(Path.GetFullPath(p), "application/xml");
    return Results.NotFound();
});

app.MapGet("/res/{device}/UpdateList.xml", (string device) => {
    string p = File.Exists("/app/wwwroot/res/UpdateList.xml") ? "/app/wwwroot/res/UpdateList.xml" : "client/src/Assets/StreamingAssets/UpdateList.xml";
    if (File.Exists(p)) return Results.File(Path.GetFullPath(p), "application/xml");
    return Results.NotFound();
});

// ==========================================
// 1. LoginSDK.aspx
// ==========================================
app.MapPost("/LoginSDK.aspx", async (HttpContext ctx, SdkDbContext db) =>
{
    var form = await ctx.Request.ReadFormAsync();
    string userName = form["UserName"].ToString()?.Trim() ?? "";
    string password = form["Password"].ToString() ?? "";
    string loginType = form["LoginType"].ToString() ?? "0";
    string ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

    var rep = new LoginRep();

    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
    {
        rep.ErrorCode = -10;
        rep.ErorrMsg = "Tên đăng nhập không hợp lệ";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9_]+$"))
    {
        rep.ErrorCode = -10;
        rep.ErorrMsg = "Tên đăng nhập chứa ký tự đặc biệt";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    string md5Pass = CreateMD5(password);
    var user = await db.LoginAccounts.FirstOrDefaultAsync(x => x.LoginName == userName && x.Password == md5Pass);

    if (user != null)
    {
        if (user.Status != 0)
        {
            rep.ErrorCode = -3;
            rep.ErorrMsg = $"Tài khoản đang bị khóa. Lý do: {user.Note}";
            return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
        }

        string token = CreateMD5(userName + DateTime.UtcNow.Ticks);
        user.AccessToken = token;
        user.TokenTimeExp = DateTime.UtcNow.AddDays(2);
        user.LastLoginTime = DateTime.UtcNow;
        user.LastIPLogin = ip;
        await db.SaveChangesAsync();

        rep.ErrorCode = 0;
        rep.ErorrMsg = "Đăng nhập thành công!";
        rep.AccessToken = token;
        rep.LastLoginIP = ip;
        rep.LastLoginTime = user.LastLoginTime.ToString();
    }
    else
    {
        rep.ErrorCode = -2;
        rep.ErorrMsg = "Sai tên đăng nhập hoặc mật khẩu!";
    }

    return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
});

// ==========================================
// 2. RegisterSDK.aspx
// ==========================================
app.MapPost("/RegisterSDK.aspx", async (HttpContext ctx, SdkDbContext db) =>
{
    var form = await ctx.Request.ReadFormAsync();
    string userName = form["UserName"].ToString()?.Trim() ?? "";
    string password = form["Password"].ToString() ?? "";
    string email = form["Email"].ToString()?.Trim() ?? "";
    string phone = form["PhoneNumber"].ToString()?.Trim() ?? "";

    var rep = new RegisterRep();

    if (userName.Length < 4 || userName.Length > 20 || !Regex.IsMatch(userName, @"^[a-zA-Z0-9_]+$"))
    {
        rep.ErrorCode = -1;
        rep.ErorrMsg = "Tên đăng nhập phải từ 4-20 ký tự chữ và số";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    if (string.IsNullOrEmpty(password) || password.Length < 6)
    {
        rep.ErrorCode = -1;
        rep.ErorrMsg = "Mật khẩu phải tối thiểu 6 ký tự";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    var exists = await db.LoginAccounts.AnyAsync(x => x.LoginName == userName);
    if (exists)
    {
        rep.ErrorCode = -1;
        rep.ErorrMsg = "Tên đăng nhập đã có người sử dụng!";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    var newUser = new LoginAccount
    {
        LoginName = userName,
        Password = CreateMD5(password),
        Email = email,
        Phone = phone,
        Status = 0,
        Date = DateTime.UtcNow,
        AccessToken = ""
    };
    db.LoginAccounts.Add(newUser);
    await db.SaveChangesAsync();

    rep.ErrorCode = 0;
    rep.ErorrMsg = "Đăng ký tài khoản thành công!";
    return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
});

// ==========================================
// 3. GetServerList.aspx & GetServerListTest.aspx
// ==========================================
async Task<IResult> HandleServerList(HttpContext ctx, SdkDbContext db)
{
    string strUID = "0";
    if (ctx.Request.HasFormContentType)
    {
        var form = await ctx.Request.ReadFormAsync();
        strUID = form["strUID"].ToString() ?? "0";
    }
    else if (ctx.Request.Query.ContainsKey("strUID"))
    {
        strUID = ctx.Request.Query["strUID"].ToString() ?? "0";
    }

    int userId = 0;
    if (strUID.Contains('_'))
        int.TryParse(strUID.Split('_')[0], out userId);
    else
        int.TryParse(strUID, out userId);

    var serverDataEx = new BuffServerListDataEx();
    var servers = await db.GameServers.ToListAsync();

    foreach (var s in servers)
    {
        serverDataEx.ListServerData.Add(new BuffServerInfo
        {
            nServerID = s.nServerID ?? 1,
            nServerOrder = s.nServerOrder ?? 1,
            strServerName = s.strServerName ?? "Server",
            strURL = s.strURL ?? "127.0.0.1",
            nServerPort = s.nServerPort ?? 3001,
            nOnlineNum = s.nOnlineNum ?? 0,
            nStatus = s.nStatus ?? 1,
            strMaintainTxt = s.strMaintainTxt ?? "",
            Msg = s.strMaintainTxt ?? "",
            strMaintainStarTime = s.strMaintainStarTime?.ToString() ?? "",
            strMaintainTerminalTime = s.strMaintainTerminalTime?.ToString() ?? ""
        });
    }

    serverDataEx.RecommendListServerData = serverDataEx.ListServerData;

    if (userId > 0)
    {
        var user = await db.LoginAccounts.FindAsync(userId);
        if (user?.LastServerLogin > 0)
        {
            var last = serverDataEx.ListServerData.FirstOrDefault(x => x.nServerID == user.LastServerLogin);
            if (last != null)
                serverDataEx.LastServerLogin = last;
        }
    }

    if (serverDataEx.LastServerLogin.nServerID == 0 && serverDataEx.ListServerData.Count > 0)
    {
        serverDataEx.LastServerLogin = serverDataEx.ListServerData[0];
    }

    return Results.Bytes(ToProtoBytes(serverDataEx), "application/octet-stream");
}

app.MapMethods("/GetServerList.aspx", new[] { "GET", "POST" }, HandleServerList);
app.MapMethods("/GetServerListTest.aspx", new[] { "GET", "POST" }, HandleServerList);
app.MapMethods("/GetServerListLocalHost.aspx", new[] { "GET", "POST" }, HandleServerList);

// ==========================================
// 4. VerifyAccount.aspx
// ==========================================
app.MapPost("/VerifyAccount.aspx", async (HttpContext ctx, SdkDbContext db) =>
{
    using var ms = new MemoryStream();
    await ctx.Request.Body.CopyToAsync(ms);
    byte[] body = ms.ToArray();

    var verifyRep = new ServerVerifySIDData();

    if (body.Length == 0)
    {
        verifyRep.strPlatformUserID = "-10";
        verifyRep.strCM = "Dữ liệu rỗng!";
        return Results.Bytes(ToProtoBytes(verifyRep), "application/octet-stream");
    }

    ClientVerifySIDData clientData = null;
    try
    {
        clientData = FromProtoBytes<ClientVerifySIDData>(body);
    }
    catch { }

    if (clientData == null || string.IsNullOrEmpty(clientData.strSID))
    {
        verifyRep.strPlatformUserID = "-10";
        verifyRep.strCM = "Sai dữ liệu truyền lên!";
        return Results.Bytes(ToProtoBytes(verifyRep), "application/octet-stream");
    }

    var user = await db.LoginAccounts.FirstOrDefaultAsync(x => x.AccessToken == clientData.strSID);
    if (user != null)
    {
        verifyRep.strPlatformUserID = $"{user.ID}_{user.LoginName}";
        verifyRep.strAccountName = user.LoginName;
        verifyRep.strCM = "1";
        verifyRep.lTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // MD5(strPlatformUserID + strAccountName + lTime + strCM + webKey)
        string rawSignature = $"{verifyRep.strPlatformUserID}{verifyRep.strAccountName}{verifyRep.lTime}{verifyRep.strCM}{webKey}";
        verifyRep.strToken = CreateMD5(rawSignature);
    }
    else
    {
        verifyRep.strPlatformUserID = "-1";
        verifyRep.strAccountName = "";
        verifyRep.strCM = "Sai tài khoản hoặc mật khẩu";
        verifyRep.strToken = "";
        verifyRep.lTime = 0;
    }

    return Results.Bytes(ToProtoBytes(verifyRep), "application/octet-stream");
});

// ==========================================
// 5. ActiveGiftCode.aspx (used by GameServer)
// ==========================================
app.MapPost("/ActiveGiftCode.aspx", async (HttpContext ctx, SdkDbContext db) =>
{
    using var ms = new MemoryStream();
    await ctx.Request.Body.CopyToAsync(ms);
    byte[] body = ms.ToArray();

    var rep = new GiftCodeRep();
    GiftCodeRequest req = null;
    try
    {
        req = FromProtoBytes<GiftCodeRequest>(body);
    }
    catch { }

    if (req == null || string.IsNullOrEmpty(req.CodeActive))
    {
        rep.Status = -1;
        rep.Msg = "Mã quà tặng không hợp lệ!";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    var codeEntry = await db.GiftCodes.FirstOrDefaultAsync(x => x.Code == req.CodeActive);
    if (codeEntry == null)
    {
        rep.Status = -1;
        rep.Msg = "Mã quà tặng không tồn tại!";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    if (codeEntry.ServerID != 0 && codeEntry.ServerID != req.ServerID)
    {
        rep.Status = -1;
        rep.Msg = "Mã quà tặng không áp dụng cho máy chủ này!";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    int usedCount = await db.GiftCodeLogs.CountAsync(x => x.Code == req.CodeActive);
    if (usedCount >= codeEntry.MaxActive)
    {
        rep.Status = -1;
        rep.Msg = "Mã quà tặng đã hết lượt sử dụng!";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    bool alreadyUsedByRole = await db.GiftCodeLogs.AnyAsync(x => x.Code == req.CodeActive && x.RoleID == req.RoleActive);
    if (alreadyUsedByRole)
    {
        rep.Status = -1;
        rep.Msg = "Nhân vật đã sử dụng mã quà tặng này rồi!";
        return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
    }

    db.GiftCodeLogs.Add(new GiftCodeLogEntry
    {
        Code = req.CodeActive,
        ServerID = req.ServerID,
        RoleID = req.RoleActive,
        Time = DateTime.UtcNow
    });
    await db.SaveChangesAsync();

    rep.Status = 0;
    rep.Msg = "Kích hoạt mã quà tặng thành công!";
    rep.GiftItem = codeEntry.GoodsList ?? "";
    return Results.Bytes(ToProtoBytes(rep), "application/octet-stream");
});

// ==========================================
// 6. UpdateServerStatus.aspx (used by GameServer)
// ==========================================
app.MapPost("/UpdateServerStatus.aspx", async (HttpContext ctx, SdkDbContext db) =>
{
    var form = await ctx.Request.ReadFormAsync();
    if (int.TryParse(form["ServerID"], out int serverId))
    {
        var s = await db.GameServers.FirstOrDefaultAsync(x => x.nServerID == serverId);
        if (s != null)
        {
            if (int.TryParse(form["OnlineNum"], out int onlineNum))
                s.nOnlineNum = onlineNum;
            if (int.TryParse(form["Status"], out int status))
                s.nStatus = status;
            await db.SaveChangesAsync();
        }
    }
    return Results.Ok("OK");
});

// ==========================================
// Health & Diagnostic info
// ==========================================
app.MapGet("/", () => Results.Ok(new
{
    status = "running",
    service = "KiemTheWebSDK",
    version = "1.0.0-net6",
    endpoints = new[]
    {
        "/LoginSDK.aspx",
        "/RegisterSDK.aspx",
        "/GetServerList.aspx",
        "/VerifyAccount.aspx",
        "/ActiveGiftCode.aspx"
    }
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
