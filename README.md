# 🏢 ERP System — Enterprise Resource Planning

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![Entity Framework](https://img.shields.io/badge/EF%20Core-10.0-green)
![xUnit](https://img.shields.io/badge/Tests-7%20Passed-brightgreen)

A full-featured Enterprise Resource Planning (ERP) system built with
ASP.NET Core MVC, Web API, MS SQL Server, and Entity Framework Core.

---

## ✨ Features

### 👥 Human Resources
- Employee management (Add, Edit, Delete, Search)
- Department & Designation management
- Employee profiles with joining date tracking

### 📦 Inventory Management
- Product catalog with categories and units
- Real-time stock tracking (Stock In / Stock Out)
- Low stock alerts and stock valuation
- Purchase Orders with approval workflow

### 🛒 Sales Management
- Customer management
- Sales Order creation with multiple items
- Invoice generation with PDF print
- Payment status tracking (Paid / Unpaid)

### 🏦 Finance & Accounts
- Accounts overview with income/expense summary
- Invoice management
- Financial reports

### 💰 Payroll
- Salary structure per employee
- Monthly payroll processing
- Payslip generation with print support
- PF and Tax deduction management

### 📊 Dashboard
- Real-time KPI cards (Employees, Products, Sales, Stock)
- Monthly sales chart (Chart.js)
- Recent orders table
- Quick stats panel

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Frontend | ASP.NET Core MVC, Razor Views, Bootstrap 5 |
| Backend API | ASP.NET Core Web API |
| Database | MS SQL Server |
| ORM | Entity Framework Core |
| Authentication | ASP.NET Identity + Cookie Auth |
| Architecture | Repository Pattern + Service Layer |
| Charts | Chart.js |
| Testing | xUnit + Moq (7 tests) |

---

## 📁 Project Structure
ERP.sln

├── ERP.Web          → ASP.NET Core MVC (UI)

├── ERP.API          → ASP.NET Core Web API

├── ERP.Core         → Models, DTOs, Interfaces, Services

├── ERP.Infrastructure → EF Core, Repositories, DbContext

└── ERP.Tests        → xUnit Unit Tests

---

## 🚀 How to Run

### Prerequisites
- .NET 10 SDK
- SQL Server (Express or full)
- Visual Studio 2022

### Steps

1. Clone the repository
```bash
git clone https://github.com/Patel15Harsh86/ERP-System.git
cd ERP-System
```

2. Update connection string in `ERP.API/appsettings.json`
and `ERP.Web/appsettings.json`:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=ERPDb;
Trusted_Connection=True;TrustServerCertificate=True;"
```

3. Run database migrations
```bash
dotnet ef database update --project ERP.Infrastructure/ERP.Infrastructure.csproj
--startup-project ERP.API/ERP.API.csproj
```

4. Run the web application
```bash
dotnet run --project ERP.Web/ERP.Web.csproj
```

5. Open browser → `http://localhost:5122`

### Default Login
- **Email:** admin@erp.com
- **Password:** Admin@123

---

## 🧪 Running Tests

```bash
dotnet test ERP.Tests/ERP.Tests.csproj
```

Expected output: `total: 7, failed: 0, succeeded: 7`

---

## 📸 Screenshots

### Dashboard
> Real-time KPI cards with live database data and Chart.js charts

### Employees
> Full CRUD with search, department and designation management

### Sales Orders
> Create orders, generate invoices, track payment status

### Payroll
> Process monthly payroll, generate printable payslips

---

## 👨‍💻 Developer

Built by **Harsh Patel** as a portfolio project to demonstrate
ASP.NET Core MVC, Web API, SQL Server, and clean architecture skills.

---

## 📄 License
MIT License