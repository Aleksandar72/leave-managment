using Microsoft.EntityFrameworkCore.Migrations;

namespace LeaveManagment.Data.Migrations
{
    public partial class fixIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveHistories_AspNetUsers_REQUESTEDBY_EMPLOYEE_Id",
                table: "LeaveHistories");

            migrationBuilder.DropIndex(
                name: "IX_LeaveHistories_REQUESTEDBY_EMPLOYEE_Id",
                table: "LeaveHistories");

            migrationBuilder.DropColumn(
                name: "REQUESTEDBY_EMPLOYEE_Id",
                table: "LeaveHistories");

            migrationBuilder.AlterColumn<string>(
                name: "RequestingEmployeeId",
                table: "LeaveHistories",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveHistories_RequestingEmployeeId",
                table: "LeaveHistories",
                column: "RequestingEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveHistories_AspNetUsers_RequestingEmployeeId",
                table: "LeaveHistories",
                column: "RequestingEmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveHistories_AspNetUsers_RequestingEmployeeId",
                table: "LeaveHistories");

            migrationBuilder.DropIndex(
                name: "IX_LeaveHistories_RequestingEmployeeId",
                table: "LeaveHistories");

            migrationBuilder.AlterColumn<string>(
                name: "RequestingEmployeeId",
                table: "LeaveHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "REQUESTEDBY_EMPLOYEE_Id",
                table: "LeaveHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveHistories_REQUESTEDBY_EMPLOYEE_Id",
                table: "LeaveHistories",
                column: "REQUESTEDBY_EMPLOYEE_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveHistories_AspNetUsers_REQUESTEDBY_EMPLOYEE_Id",
                table: "LeaveHistories",
                column: "REQUESTEDBY_EMPLOYEE_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
