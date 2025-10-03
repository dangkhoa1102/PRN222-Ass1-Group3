## PRN222 - Assignment Group 03

Ứng dụng ASP.NET Core MVC quản lý đại lý và xe (Vehicle & Dealer Management), bao gồm đặt lịch lái thử, quản lý đơn hàng, tài khoản người dùng và phân quyền.

### Công nghệ
- **.NET 8** (ASP.NET Core MVC)
- **Entity Framework Core** (Code-First/Database-First tùy theo cấu hình hiện có)
- SQL Server

### Cấu trúc giải pháp
```
PRN222_SE1830_Ass1_Group03.sln
├─ BusinessObjects/           # DTOs, Models (Entity)
├─ DataAccessLayer/           # DbContext, Repositories
├─ Services/                  # Service layer
└─ Group03_MVC/               # ASP.NET Core MVC (UI)
```

### Các account để login vào (Tất cả chung 1 password)
# username:
- customer1
- customer2
- staff_hp1
- staff_hn1
- evm_staff1
- evm_staff2
- manager_hp
- manager_hn
# password:
123456

