# Task Management API 🚀

🔗 [簡報技術詳解](https://docs.google.com/presentation/d/15zhCLoS-W1WvkpIPMBhoeheRBofIomrQtAeAGdXfk4w/edit#slide=id.p) (完善中)

**ASP.NET Core** 的後端 API，提供 **任務管理** 功能，支援 **JWT 驗證、角色權限管理**，並透過 **LINQ** 進行查詢最佳化（分頁、排序、模糊查詢）。未來計畫加入 **Middleware 日誌紀錄、Redis Cache 優化查詢** 進一步強化功能。

## 📌 核心表格

- **AppUser** (使用者) 👉 用來測試 JWT 驗證
- **WorkOrder** (任務) 👉 用來測試 CRUD

## 📌 最簡可行產品 MVP

- ✅ 任務 新增 / 修改 / 刪除 / 查詢（LINQ → 分頁、排序、模糊查詢）
- ✅ 使用者註冊 & 登入 (JWT)
- ✅ 角色權限 (ASP.NET Core Identity)

## 核心技術

- 🔹 **ASP.NET Core Web API** 👉 開發 RESTful API
- 🔹 **Entity Framework Core** 👉 操作 MSSQL
- 🔹 **LINQ 查詢最佳化** 👉 分頁、排序、模糊查詢
- 🔹 **Swagger** 👉 API 文件生成
- 🔹 **JWT (Json Web Token)** 👉 用戶驗證與授權
- 🔹 **角色權限 (ASP.NET Core Identity)** 👉 限制 API 操作權限

## 後續待擴充

- 🚀 **Middleware** 👉 日誌 & 錯誤處理
- 🚀 **Redis Cache** 👉 加速查詢效率
- 🚀 **SignalR 即時通知** 👉 新增任務時即時推送
- 🚀 **Docker 部屬** 👉 整合容器化部署
