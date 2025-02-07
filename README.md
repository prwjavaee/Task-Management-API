Task Management API 🚀

📌 核心表格 :
AppUser (使用者) 👉 用來測試 JWT 驗證
WorkOrder (任務) 👉 用來測試 CRUD

📌 最簡可行產品 MVP (專注於資源操作，無額外多餘的 API) :
✅ 使用者註冊 & 登入 (JWT)
✅ 新增 / 修改 / 刪除 / 查詢任務
✅ 角色權限 (Admin/GeneralUser/AuthorizedUser)

📌 待擴充：
🚀 Middleware (Logging、ActionFilter)
🚀 加入 Redis Cache 優化查詢
🚀 加入 Docker 部屬

-------------------

核心技術
🔹 ASP.NET Core Web API 👉 開發 RESTful API
🔹 Entity Framework Core 👉 操作 MSSQL
🔹 Swagger 👉 API 文件生成
🔹 JWT (Json Web Token) 👉 用戶驗證與授權
🔹 角色權限 (Admin/GeneralUser/AuthorizedUser) 👉 限制 API 操作權限

後續擴充
🚀 Middleware 👉 日誌 & 錯誤處理
🚀 Redis Cache 👉 加速查詢效率
🚀 SignalR 即時通知 👉 新增任務時即時推送
🚀 Docker 部屬 👉 整合容器化部署

為何這個專案適合 MVP？
✅ 只有 2 張表(AppUser/WorkOrder)，避免 CRUD 過多，保持簡潔
✅ 重點在 JWT + 角色權限，核心功能的關鍵
✅ 方便後續加入 進階技術(Middleware、Redis、SignalR)，保持擴充性強