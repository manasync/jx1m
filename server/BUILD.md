# Build server (project tái tạo)

`.sln`/`.csproj` gốc không có trong source nên đã được **tạo lại** (SDK-style, .NET Framework).
Build trên **Windows + Visual Studio 2019/2022** (hoặc `msbuild`). macOS/Linux không build được .NET Framework.

## Solution

| Solution | Project | Loại | TFM |
|---|---|---|---|
| `src/GameServer/KTGameServer/KTGameServer.sln` | Tmsk.Contract | library | net472 |
| | KF.CenterServer.Remoting | library | net472 |
| | KF.CenterServer.Hosting | exe | net472 |
| | GameServer | exe | net472 |
| `src/GameDBServer/GameDBServer.sln` | GameDBServer | exe (x64) | net48 |

Thứ tự phụ thuộc (VS tự lo): `Tmsk.Contract` → `KF.CenterServer.Remoting` → `KF.CenterServer.Hosting`; `GameServer` → `Tmsk.Contract`.

## Cách build
1. Mở `.sln` bằng Visual Studio → chuột phải solution → **Restore NuGet Packages**.
2. Build. NuGet deps tải từ nuget.org; DLL cục bộ (Tmsk.Tools, Neo.Lua, UnityEngine, MoonSharp, MySQLDriverCS, IBatisNet, EnterpriseLibrary, SQLite) tham chiếu qua HintPath tới file có sẵn.

## Caveat (đọc trước khi kêu lỗi)
- **Không compile-verify được lúc tạo** (máy tạo là macOS). Có thể còn thiếu vài `<Reference Include="System.*">` — VS sẽ báo đỏ, thêm vào là xong.
- **`.cs` gom tự động** (`**/*.cs`). Nếu project gốc từng loại trừ file nào thì phải `<Compile Remove="...">`.
- **LogDBServer KHÔNG có source** — chỉ còn config/log. Không tạo được project. Muốn chạy thì phục hồi exe prebuilt:
  `git checkout HEAD -- server/bin/LogDBServer`
- **Bitness native**: `libmysql.dll` (KF.CenterServer.Remoting) và `libMySQL.dll` (GameDBServer) phụ thuộc x86/x64. GameDBServer đã ép `x64`. Nếu KF.Remoting lỗi nạp libmysql, đặt Platform x86/x64 cho khớp file DLL.
- `Tmsk.Tools.dll` được **khôi phục từ git** vào `src/GameServer/KTGameServer/libs/` (trước nằm trong bin đã xoá).
- DLL prebuilt cũ đã bị xoá khi clear build — build lại sẽ sinh mới trong `bin/`.

## Nhắc lại (từ bước trước)
Đã đổi về placeholder, cần điền lại trước khi chạy thật: connection string DB (`app.config`), URL SDK nạp thẻ (`GameManager.cs`), KuaFu URI, và bật lại Telegram/telemetry nếu muốn (đã gỡ).
