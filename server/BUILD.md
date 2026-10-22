# Hướng Dẫn Vận Hành Server Kiếm Thế Mobile (JX1M) - Modern .NET 6 & Docker

Toàn bộ hệ thống server JX1M đã được chuyển đổi hoàn toàn sang **.NET 6 đa nền tảng** (chạy natively trên macOS / Linux / Windows / Docker). Không còn phụ thuộc vào Windows, IIS hay .NET Framework 4.7.2/4.8.

---

## 1. Kiến trúc hệ sinh thái Server

| Dịch vụ | Ngôn ngữ / Nền tảng | Port | Mô tả vai trò |
|---|---|---|---|
| **`mysql` (MariaDB)** | MariaDB 10.5 | `3306` | Cơ sở dữ liệu game (`gamedb`), quản lý nhân vật, trang bị, tiền tệ |
| **`KiemTheWebSDK`** | ASP.NET Core Minimal API (.NET 6) | `8081` (hoặc `81`) | API xác thực tài khoản, đăng ký, đăng nhập, nạp thẻ, kích hoạt giftcode |
| **`GameDBServer`** | C# .NET 6 (Console) | `23001` | Trung gian truy xuất CSDL MySQL cho GameServer qua socket TCP nhị phân |
| **`GameServer`** | C# .NET 6 (Console) | `3001` (TCP game), `8889` (HTTP) | Logic game thế giới Kiếm Thế, bản đồ, chiến trường, quái, bang hội |

---

## 2. Cách chạy bằng Docker (Khuyến nghị)

Toàn bộ các dịch vụ được đóng gói sẵn trong `docker-compose.yml`.

### Khởi động cụm server:
```bash
./scripts/run_docker.sh
```
Hoặc dùng lệnh docker compose trực tiếp:
```bash
docker compose up -d
```

### Xem log thời gian thực:
```bash
docker compose logs -f
# Hoặc xem log dịch vụ cụ thể:
docker compose logs -f gameserver
docker compose logs -f gamedbserver
docker compose logs -f websdk
```

### Dừng cụm server:
```bash
./scripts/stop_docker.sh
# Hoặc:
docker compose down
```

---

## 3. Cách chạy trực tiếp trên máy chủ (Host / Native)

Dành cho môi trường phát triển local hoặc debug không cần qua container.

### Yêu cầu:
- .NET 6 SDK (`dotnet --version` >= 6.0)
- Cơ sở dữ liệu MySQL/MariaDB chạy tại `127.0.0.1:3306` đã import file `server/db/gamedb.sql` (User: `root`, Password: `123456`, DB: `gamedb`).

### Khởi động:
```bash
./scripts/run_local.sh
```

### Dừng:
```bash
./scripts/stop_local.sh
```

### Build thủ công từng thành phần:
```bash
# 1. Build GameDBServer
dotnet build server/src/GameDBServer/GameDBServer/GameDBServer.csproj -c Release

# 2. Build GameServer
dotnet build server/src/GameServer/KTGameServer/KTGameServer.sln -c Release

# 3. Build Web SDK
dotnet build server/src/WEB/KiemTheWebSDK/KiemTheWebSDK.csproj -c Release
```

---

## 4. Kiểm thử kết nối và tài khoản mẫu

Khi khởi động Web SDK lần đầu, hệ thống tự động khởi tạo cơ sở dữ liệu SQLite tại `/app/data/kiemthe_sdk.db` và nạp sẵn dữ liệu mặc định:

1. **Tài khoản test**:
   - Tên đăng nhập: `test1`
   - Mật khẩu: `123456`
2. **Máy chủ mặc định**:
   - Tên máy chủ: `Biện Kinh 1`
   - Địa chỉ: `127.0.0.1:3001`
3. **Endpoint kiểm tra trạng thái**:
   - `curl http://localhost:8081/health` ➔ `{"status":"healthy"}`
   - `curl http://localhost:8081/` ➔ Danh sách endpoint SDK
