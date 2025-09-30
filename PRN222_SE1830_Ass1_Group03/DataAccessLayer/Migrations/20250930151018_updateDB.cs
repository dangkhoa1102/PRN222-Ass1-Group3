using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class updateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dealers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__dealers__3213E83F6EA72CB4", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    year = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    specifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    images = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stock_quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vehicles__3213E83F5CB7D8D6", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    dealer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83F0152B868", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_dealers",
                        column: x => x.dealer_id,
                        principalTable: "dealers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    order_number = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dealer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    payment_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "unpaid"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__orders__3213E83FD0233CAC", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_customers",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_dealers",
                        column: x => x.dealer_id,
                        principalTable: "dealers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_vehicles",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "test_drive_appointments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dealer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__test_dri__3213E83F8C9A05E0", x => x.id);
                    table.ForeignKey(
                        name: "FK_testdrive_customer",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_testdrive_dealer",
                        column: x => x.dealer_id,
                        principalTable: "dealers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_testdrive_vehicle",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__order_hi__3213E83F106C6018", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_history_orders",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_order_history_users",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__dealers__357D4CF9AF695B63",
                table: "dealers",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_history_created_by",
                table: "order_history",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_order_history_order_id",
                table: "order_history",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_dealer_id",
                table: "orders",
                column: "dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_vehicle_id",
                table: "orders",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "UQ__orders__730E34DF70A4AC38",
                table: "orders",
                column: "order_number",
                unique: true,
                filter: "[order_number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_test_drive_appointments_customer_id",
                table: "test_drive_appointments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_test_drive_appointments_dealer_id",
                table: "test_drive_appointments",
                column: "dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_test_drive_appointments_vehicle_id",
                table: "test_drive_appointments",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_dealer_id",
                table: "users",
                column: "dealer_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E616475455A96",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__F3DBC5725CDAFAE2",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_history");

            migrationBuilder.DropTable(
                name: "test_drive_appointments");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "dealers");
        }
    }
}
