# Task Management API 🚀

基於 ASP.NET Core 的任務管理 API，支援 JWT 驗證、角色權限、Redis 快取與日誌紀錄。整合 Docker 容器化與 GitHub Actions，自動化建置與測試流程，並透過 PowerShell 腳本實作一鍵部署，模擬完整 CI/CD 流程。

## 🔗 專案簡報

[技術簡報連結](https://docs.google.com/presentation/d/15zhCLoS-W1WvkpIPMBhoeheRBofIomrQtAeAGdXfk4w/edit#slide=id.p)

---

## ✅ 功能總覽

- ✅ 任務 CRUD（分頁、排序、模糊查詢，含 Redis 快取）
- ✅ 使用者註冊 / 登入（JWT）
- ✅ 角色權限管理（ASP.NET Core Identity）
- ✅ API 與錯誤日誌（Middleware、ActionFilter）
- ✅ Docker 化部署（含 docker-compose）
- ✅ PowerShell 部署腳本（deploy.ps1 / shutdown.ps1）
- ✅ GitHub Actions 自動化流程（Restore、Build、Test、Docker Build）
- ✅ xUnit 單元測試

---

## 🛠 技術架構

- 🔹 **後端框架**：ASP.NET Core Web API
- 🔹 **ORM 與資料庫**：Entity Framework Core、MSSQL、LINQ
- 🔹 **認證授權**：JWT、ASP.NET Core Identity
- 🔹 **日誌紀錄**：Middleware、ActionFilter
- 🔹 **快取優化**：Redis
- 🔹 **部署工具**：Docker、docker-compose、PowerShell 腳本
- 🔹 **CI/CD 自動化**：GitHub Actions
- 🔹 **測試框架**：xUnit
- 🔹 **API 文件**：Swagger

---
