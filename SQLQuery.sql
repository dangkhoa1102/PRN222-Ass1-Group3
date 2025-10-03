-- =============================================
-- ELECTRIC VEHICLE DEALER MANAGEMENT SYSTEM
-- SQL SERVER DATABASE CREATION SCRIPT (GUID VERSION)
-- =============================================

-- 0️⃣ Tạo database
CREATE DATABASE EVDealerSystem;
GO
USE EVDealerSystem;
GO

-- =============================================
-- 1️⃣ TẠO BẢNG
-- =============================================

-- 1. Dealers
CREATE TABLE dealers (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(100) NOT NULL,
    code NVARCHAR(20) NOT NULL UNIQUE,
    address NVARCHAR(MAX),
    phone NVARCHAR(20),
    email NVARCHAR(100),
    created_at DATETIME2 DEFAULT GETDATE(),
    is_active BIT DEFAULT 1
);

-- 2. Users
CREATE TABLE users (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    username NVARCHAR(50) NOT NULL UNIQUE,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    full_name NVARCHAR(100),
    phone NVARCHAR(20),
    role NVARCHAR(20),
    dealer_id UNIQUEIDENTIFIER NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    is_active BIT DEFAULT 1,
    CONSTRAINT FK_users_dealers FOREIGN KEY (dealer_id) REFERENCES dealers(id)
);

-- 3. Vehicles
CREATE TABLE vehicles (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(100) NOT NULL,
    brand NVARCHAR(50),
    model NVARCHAR(50),
    year INT,
    price DECIMAL(15,2) NOT NULL,
    description NVARCHAR(MAX),
    specifications NVARCHAR(MAX), -- JSON string
    images NVARCHAR(MAX),         -- JSON array URLs
    stock_quantity INT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    is_active BIT DEFAULT 1
);

-- 4. Orders
CREATE TABLE orders (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    order_number NVARCHAR(30) UNIQUE,
    customer_id UNIQUEIDENTIFIER NOT NULL,
    dealer_id UNIQUEIDENTIFIER NOT NULL,
    vehicle_id UNIQUEIDENTIFIER NOT NULL,
    total_amount DECIMAL(15,2) NOT NULL,
    status NVARCHAR(20),
    payment_status NVARCHAR(20),
    notes NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_orders_customers FOREIGN KEY (customer_id) REFERENCES users(id),
    CONSTRAINT FK_orders_dealers FOREIGN KEY (dealer_id) REFERENCES dealers(id),
    CONSTRAINT FK_orders_vehicles FOREIGN KEY (vehicle_id) REFERENCES vehicles(id)
);

-- 5. Order History
CREATE TABLE order_history (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    order_id UNIQUEIDENTIFIER NOT NULL,
    status NVARCHAR(50) NOT NULL,
    notes NVARCHAR(MAX),
    created_by UNIQUEIDENTIFIER NOT NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_order_history_orders FOREIGN KEY (order_id) REFERENCES orders(id),
    CONSTRAINT FK_order_history_users FOREIGN KEY (created_by) REFERENCES users(id)
);
-- 6. Test Drive Appointments (Lịch hẹn lái thử)
CREATE TABLE test_drive_appointments (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    customer_id UNIQUEIDENTIFIER NOT NULL,
    dealer_id UNIQUEIDENTIFIER NOT NULL,
    vehicle_id UNIQUEIDENTIFIER NOT NULL,
    appointment_date DATETIME2 NOT NULL, -- Thời gian hẹn lái thử
    status NVARCHAR(20),
    notes NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),

    CONSTRAINT FK_testdrive_customer FOREIGN KEY (customer_id) REFERENCES users(id),
    CONSTRAINT FK_testdrive_dealer FOREIGN KEY (dealer_id) REFERENCES dealers(id),
    CONSTRAINT FK_testdrive_vehicle FOREIGN KEY (vehicle_id) REFERENCES vehicles(id)
);
-- =============================================
-- 5️⃣ SAMPLE DATA
-- =============================================

INSERT INTO dealers (name, code, address, phone, email) VALUES
(N'Đại lý Hà Nội', 'HN001', N'123 Đường ABC, Hà Nội', '0241234567', 'hanoi@dealer.com'),
(N'Đại lý TP.HCM', 'HCM001', N'456 Đường XYZ, TP.HCM', '0281234567', 'hcm@dealer.com'),
(N'Đại lý Đà Nẵng', 'DN001', N'789 Đường DEF, Đà Nẵng', '0361234567', 'danang@dealer.com');

DECLARE @dealerHN UNIQUEIDENTIFIER = (SELECT id FROM dealers WHERE code='HN001');

INSERT INTO users (username,email,password,full_name,phone,role,dealer_id) VALUES
('admin','admin@system.com','123456',N'Administrator','0123456789','admin',NULL),
('evm_staff1','evm1@company.com','123456',N'Nguyễn Văn A','0123456790','evm_staff',NULL),
('manager_hn','manager.hn@dealer.com','123456',N'Trần Thị B','0123456791','dealer_manager',@dealerHN),
('staff_hn1','staff1.hn@dealer.com','123456',N'Lê Văn C','0123456792','dealer_staff',@dealerHN),
('customer1','customer1@email.com','123456',N'Phạm Thị D','0123456793','customer',NULL);

INSERT INTO vehicles (name,brand,model,year,price,description,specifications,stock_quantity) VALUES
(N'VinFast VF8','VinFast','VF8',2024,1200000000,
 N'SUV điện cao cấp với thiết kế hiện đại',
 N'{"battery":"87.7kWh","range":"420km","power":"300kW"}',10),
(N'Tesla Model 3','Tesla','Model 3',2024,1500000000,
 N'Sedan điện thông minh hàng đầu thế giới',
 N'{"battery":"75kWh","range":"510km","power":"239kW"}',5),
(N'BYD Atto 3','BYD','Atto 3',2024,800000000,
 N'Crossover điện giá phải chăng',
 N'{"battery":"60.5kWh","range":"480km","power":"150kW"}',15);
GO
-- =============================================
-- Thêm dữ liệu cho dealers (5 bản ghi)
-- =============================================
INSERT INTO dealers (name, code, address, phone, email) VALUES
(N'Đại lý Hải Phòng', 'HP001', N'12 Lạch Tray, Hải Phòng', '0225123456', 'haiphong@dealer.com'),
(N'Đại lý Cần Thơ', 'CT001', N'34 Nguyễn Trãi, Cần Thơ', '0292123456', 'cantho@dealer.com'),
(N'Đại lý Huế', 'HUE001', N'56 Hùng Vương, Huế', '0234123456', 'hue@dealer.com'),
(N'Đại lý Nha Trang', 'NT001', N'78 Trần Phú, Nha Trang', '0258123456', 'nhatrang@dealer.com'),
(N'Đại lý Bình Dương', 'BD001', N'90 Đại lộ Bình Dương, Bình Dương', '0274123456', 'binhduong@dealer.com');

-- Lấy dealer ID cho Hải Phòng
DECLARE @dealerHP UNIQUEIDENTIFIER = (SELECT id FROM dealers WHERE code='HP001');

-- =============================================
-- Thêm dữ liệu cho users (5 bản ghi)
-- =============================================
INSERT INTO users (username,email,password,full_name,phone,role,dealer_id) VALUES
('customer2','customer2@email.com','123456',N'Ngô Văn E','0912345678','customer',NULL),
('customer3','customer3@email.com','123456',N'Đỗ Thị F','0922345678','customer',NULL),
('staff_hp1','staff1.hp@dealer.com','123456',N'Bùi Văn G','0932345678','dealer_staff',@dealerHP),
('manager_hp','manager.hp@dealer.com','123456',N'Phan Thị H','0942345678','dealer_manager',@dealerHP),
('evm_staff2','evm2@company.com','123456',N'Nguyễn Văn I','0952345678','evm_staff',NULL);

-- =============================================
-- Thêm dữ liệu cho vehicles (5 bản ghi)
-- =============================================
INSERT INTO vehicles (name,brand,model,year,price,description,specifications,stock_quantity) VALUES
(N'VinFast VF9','VinFast','VF9',2024,1800000000,
 N'SUV điện cỡ lớn sang trọng',
 N'{"battery":"92kWh","range":"550km","power":"320kW"}',8),
(N'Tesla Model Y','Tesla','Model Y',2024,1700000000,
 N'Crossover điện cao cấp',
 N'{"battery":"82kWh","range":"505km","power":"300kW"}',12),
(N'Nissan Leaf','Nissan','Leaf',2023,750000000,
 N'Hatchback điện phổ thông',
 N'{"battery":"40kWh","range":"270km","power":"110kW"}',20),
(N'Hyundai Ioniq 5','Hyundai','Ioniq 5',2024,1300000000,
 N'Crossover điện phong cách tương lai',
 N'{"battery":"72.6kWh","range":"480km","power":"225kW"}',10),
(N'Kia EV6','Kia','EV6',2024,1250000000,
 N'Xe điện thể thao với khả năng sạc nhanh',
 N'{"battery":"77.4kWh","range":"528km","power":"239kW"}',9);

-- =============================================
-- Thêm dữ liệu cho orders (5 bản ghi)
-- =============================================
DECLARE @customer2 UNIQUEIDENTIFIER = (SELECT id FROM users WHERE username='customer2');
DECLARE @customer3 UNIQUEIDENTIFIER = (SELECT id FROM users WHERE username='customer3');
DECLARE @vehicleVF9 UNIQUEIDENTIFIER = (SELECT id FROM vehicles WHERE model='VF9');
DECLARE @vehicleModelY UNIQUEIDENTIFIER = (SELECT id FROM vehicles WHERE model='Model Y');
DECLARE @vehicleLeaf UNIQUEIDENTIFIER = (SELECT id FROM vehicles WHERE model='Leaf');

INSERT INTO orders (order_number,customer_id,dealer_id,vehicle_id,total_amount,status,payment_status,notes)
VALUES
('ORD001',@customer2,@dealerHP,@vehicleVF9,1800000000,'Pending','Unpaid',N'Khách hàng đang xem xét'),
('ORD002',@customer3,@dealerHP,@vehicleModelY,1700000000,'Confirmed','Paid',N'Khách hàng đã đặt cọc'),
('ORD003',@customer2,@dealerHP,@vehicleLeaf,750000000,'Processing','Paid',N'Đang xử lý giao xe'),
('ORD004',@customer3,@dealerHP,@vehicleVF9,1800000000,'Shipped','Paid',N'Xe đang trên đường vận chuyển'),
('ORD005',@customer2,@dealerHP,@vehicleModelY,1700000000,'Completed','Paid',N'Đơn hàng đã hoàn tất');

-- =============================================
-- Thêm dữ liệu cho order_history (5 log / order = 25 bản ghi)
-- =============================================
DECLARE @userStaffHP UNIQUEIDENTIFIER = (SELECT id FROM users WHERE username='staff_hp1');

INSERT INTO order_history (order_id,status,notes,created_by)
SELECT id,'Created',N'Đơn hàng được tạo',@userStaffHP FROM orders WHERE order_number='ORD001'
UNION ALL
SELECT id,'Confirmed',N'Đơn hàng đã được xác nhận',@userStaffHP FROM orders WHERE order_number='ORD001'
UNION ALL
SELECT id,'Processing',N'Đơn hàng đang xử lý',@userStaffHP FROM orders WHERE order_number='ORD001'
UNION ALL
SELECT id,'Shipped',N'Đơn hàng đã vận chuyển',@userStaffHP FROM orders WHERE order_number='ORD001'
UNION ALL
SELECT id,'Completed',N'Đơn hàng hoàn tất',@userStaffHP FROM orders WHERE order_number='ORD001';

INSERT INTO order_history (order_id,status,notes,created_by)
SELECT id,'Created',N'Đơn hàng được tạo',@userStaffHP FROM orders WHERE order_number='ORD002'
UNION ALL
SELECT id,'Confirmed',N'Đơn hàng đã được xác nhận',@userStaffHP FROM orders WHERE order_number='ORD002'
UNION ALL
SELECT id,'Processing',N'Đơn hàng đang xử lý',@userStaffHP FROM orders WHERE order_number='ORD002'
UNION ALL
SELECT id,'Shipped',N'Đơn hàng đã vận chuyển',@userStaffHP FROM orders WHERE order_number='ORD002'
UNION ALL
SELECT id,'Completed',N'Đơn hàng hoàn tất',@userStaffHP FROM orders WHERE order_number='ORD002';

INSERT INTO order_history (order_id,status,notes,created_by)
SELECT id,'Created',N'Đơn hàng được tạo',@userStaffHP FROM orders WHERE order_number='ORD003'
UNION ALL
SELECT id,'Confirmed',N'Đơn hàng đã được xác nhận',@userStaffHP FROM orders WHERE order_number='ORD003'
UNION ALL
SELECT id,'Processing',N'Đơn hàng đang xử lý',@userStaffHP FROM orders WHERE order_number='ORD003'
UNION ALL
SELECT id,'Shipped',N'Đơn hàng đã vận chuyển',@userStaffHP FROM orders WHERE order_number='ORD003'
UNION ALL
SELECT id,'Completed',N'Đơn hàng hoàn tất',@userStaffHP FROM orders WHERE order_number='ORD003';

INSERT INTO order_history (order_id,status,notes,created_by)
SELECT id,'Created',N'Đơn hàng được tạo',@userStaffHP FROM orders WHERE order_number='ORD004'
UNION ALL
SELECT id,'Confirmed',N'Đơn hàng đã được xác nhận',@userStaffHP FROM orders WHERE order_number='ORD004'
UNION ALL
SELECT id,'Processing',N'Đơn hàng đang xử lý',@userStaffHP FROM orders WHERE order_number='ORD004'
UNION ALL
SELECT id,'Shipped',N'Đơn hàng đã vận chuyển',@userStaffHP FROM orders WHERE order_number='ORD004'
UNION ALL
SELECT id,'Completed',N'Đơn hàng hoàn tất',@userStaffHP FROM orders WHERE order_number='ORD004';

INSERT INTO order_history (order_id,status,notes,created_by)
SELECT id,'Created',N'Đơn hàng được tạo',@userStaffHP FROM orders WHERE order_number='ORD005'
UNION ALL
SELECT id,'Confirmed',N'Đơn hàng đã được xác nhận',@userStaffHP FROM orders WHERE order_number='ORD005'
UNION ALL
SELECT id,'Processing',N'Đơn hàng đang xử lý',@userStaffHP FROM orders WHERE order_number='ORD005'
UNION ALL
SELECT id,'Shipped',N'Đơn hàng đã vận chuyển',@userStaffHP FROM orders WHERE order_number='ORD005'
UNION ALL
SELECT id,'Completed',N'Đơn hàng hoàn tất',@userStaffHP FROM orders WHERE order_number='ORD005';

-- =============================================
-- Thêm dữ liệu cho test_drive_appointments (5 bản ghi)
-- =============================================
DECLARE @vehicleIoniq UNIQUEIDENTIFIER = (SELECT id FROM vehicles WHERE model='Ioniq 5');
DECLARE @vehicleEV6 UNIQUEIDENTIFIER = (SELECT id FROM vehicles WHERE model='EV6');

INSERT INTO test_drive_appointments (customer_id,dealer_id,vehicle_id,appointment_date,status,notes)
VALUES
(@customer2,@dealerHP,@vehicleIoniq,'2025-10-10 10:00:00','Pending',N'Lịch hẹn test drive cho Hyundai Ioniq 5'),
(@customer3,@dealerHP,@vehicleEV6,'2025-10-11 14:00:00','Confirmed',N'Khách hàng xác nhận test drive Kia EV6'),
(@customer2,@dealerHP,@vehicleModelY,'2025-10-12 09:00:00','Pending',N'Lịch hẹn lái thử Tesla Model Y'),
(@customer3,@dealerHP,@vehicleLeaf,'2025-10-13 15:00:00','Completed',N'Khách hàng đã test drive Nissan Leaf'),
(@customer2,@dealerHP,@vehicleVF9,'2025-10-14 16:00:00','Canceled',N'Khách hủy lịch hẹn VF9');
