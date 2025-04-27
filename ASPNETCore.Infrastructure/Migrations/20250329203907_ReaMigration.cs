using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPNETCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserRole_UserRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Reservations_ReservationId1",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ResStatus_ResStatusId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ReservationId1",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResStatus",
                table: "ResStatus");

            migrationBuilder.DropColumn(
                name: "ReservationId1",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "UserRoleId",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "ResStatus",
                newName: "ResStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldUnicode: false,
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusType",
                table: "ResStatuses",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResStatuses",
                table: "ResStatuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ResStatuses_ResStatusId",
                table: "Reservations",
                column: "ResStatusId",
                principalTable: "ResStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ResStatuses_ResStatusId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResStatuses",
                table: "ResStatuses");

            migrationBuilder.RenameTable(
                name: "ResStatuses",
                newName: "ResStatus");

            migrationBuilder.AddColumn<int>(
                name: "ReservationId1",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "varchar(256)",
                unicode: false,
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserRoleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusType",
                table: "ResStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResStatus",
                table: "ResStatus",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ReservationId1",
                table: "Contracts",
                column: "ReservationId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserRoleId",
                table: "AspNetUsers",
                column: "UserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserRole_UserRoleId",
                table: "AspNetUsers",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Reservations_ReservationId1",
                table: "Contracts",
                column: "ReservationId1",
                principalTable: "Reservations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ResStatus_ResStatusId",
                table: "Reservations",
                column: "ResStatusId",
                principalTable: "ResStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
