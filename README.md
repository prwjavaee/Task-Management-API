# Task Management API 🚀

🔗 [簡報技術詳解](https://docs.google.com/presentation/d/15zhCLoS-W1WvkpIPMBhoeheRBofIomrQtAeAGdXfk4w/edit#slide=id.p)

**ASP.NET Core** 的後端 API，提供 **任務管理** 功能，支援 **JWT 驗證、角色權限管理**，並透過 **LINQ** 進行查詢最佳化（分頁、排序、模糊查詢）。
**日誌紀錄**功能則涵蓋了 **Middleware** 層級的**錯誤處理、身分認證與授權失敗紀錄**，以及 **ActionFilter** 層級的 **API 操作紀錄**。
新增 **Redis Cache** 功能，進一步優化查詢效率，提升系統效能。
同時導入 **Docker 容器化部署**，強化系統的可攜性與部署一致性，為未來 CI/CD 整合打下基礎。

> 🔥 目前已建置基本的 **GitHub Actions CI/CD 自動化流程**，確保每次推送 (Push) 都能自動還原、建置、測試及打包 Docker 映像檔，提升開發與部署效率。
>
> ✅ 並逐步導入 **xUnit 單元測試**，確保關鍵邏輯正確性，提升系統穩定度與可靠性。

---

## 📌 核心表格

- **AppUser** (使用者) 👉 用來測試 JWT 驗證
- **WorkOrder** (任務) 👉 用來測試 CRUD
- **ApiLog** (Api 日誌) 👉 用來紀錄 Api 操作紀錄
- **ErrorLog** (錯誤日誌) 👉 用來紀錄身分認證與授權失敗以及錯誤處理

---

## 📌 最簡可行產品 MVP

- ✅ 任務 新增 / 修改 / 刪除 / 查詢（LINQ → 分頁、排序、模糊查詢）
- ✅ 使用者註冊 & 登入 (JWT)
- ✅ 角色權限 (ASP.NET Core Identity)
- ✅ 日誌紀錄 (Middleware & ActionFilter)
- ✅ Redis Cache 優化查詢效率
- ✅ Docker 化部署環境，提升開發與測試的一致性
- ✅ GitHub Actions 自動化測試與打包流程（CI/CD）
- ✅ 初步導入 xUnit 單元測試驗證

---

## 📌 核心技術

- 🔹 **ASP.NET Core Web API** 👉 開發 RESTful API
- 🔹 **Entity Framework Core** 👉 操作 MSSQL
- 🔹 **LINQ 查詢最佳化** 👉 分頁、排序、模糊查詢
- 🔹 **Swagger** 👉 API 文件生成
- 🔹 **JWT (Json Web Token)** 👉 用戶驗證與授權
- 🔹 **角色權限 (ASP.NET Core Identity)** 👉 限制 API 操作權限
- 🔹 **日誌紀錄 (Middleware & ActionFilter)** 👉 API 操作、身分認證與授權失敗紀錄，以及 錯誤處理
- 🔹 **Redis Cache** 👉 加速查詢效率
- 🔹 **Docker / Docker Compose** 👉 建立可攜式部署環境
- 🔹 **GitHub Actions (CI/CD)** 👉 自動還原、建置、測試與打包
- 🔹 **xUnit** 👉 單元測試框架，提升程式邏輯可靠性
