# Task Management API 🚀

🔗 [簡報技術詳解](https://docs.google.com/presentation/d/15zhCLoS-W1WvkpIPMBhoeheRBofIomrQtAeAGdXfk4w/edit#slide=id.p)

**ASP.NET Core** 的後端 API，提供 **任務管理** 功能，支援 **JWT 驗證、角色權限管理**，並透過 **LINQ** 進行查詢最佳化（分頁、排序、模糊查詢）。
**日誌紀錄**功能則涵蓋了 **Middleware** 層級的**錯誤處理、身分認證與授權失敗紀錄**，以及 **ActionFilter** 層級的 **API 操作紀錄**。
新增 **Redis Cache** 功能，進一步優化查詢效率，提升系統效能。

## 📌 核心表格

- **AppUser** (使用者) 👉 用來測試 JWT 驗證
- **WorkOrder** (任務) 👉 用來測試 CRUD
- **ApiLog** (Api 日誌) 👉 用來紀錄 Api 操作紀錄
- **ErrorLog** (錯誤日誌) 👉 用來紀錄身分認證與授權失敗以及錯誤處理

## 📌 最簡可行產品 MVP

- ✅ 任務 新增 / 修改 / 刪除 / 查詢（LINQ → 分頁、排序、模糊查詢）
- ✅ 使用者註冊 & 登入 (JWT)
- ✅ 角色權限 (ASP.NET Core Identity)
- ✅ 日誌紀錄 (Middleware & ActionFilter)
- ✅ Redis Cache 優化查詢效率

## 核心技術

- 🔹 **ASP.NET Core Web API** 👉 開發 RESTful API
- 🔹 **Entity Framework Core** 👉 操作 MSSQL
- 🔹 **LINQ 查詢最佳化** 👉 分頁、排序、模糊查詢
- 🔹 **Swagger** 👉 API 文件生成
- 🔹 **JWT (Json Web Token)** 👉 用戶驗證與授權
- 🔹 **角色權限 (ASP.NET Core Identity)** 👉 限制 API 操作權限
- 🔹 **日誌紀錄 (Middleware & Actionfilter)** 👉 API 操作、身分認證與授權失敗紀錄，以及 錯誤處理
- 🔹 **Redis Cache** 👉 加速查詢效率
