using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSBI.Migrations
{
    public partial class addOtp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Otp",
                table: "InternetBankings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Otp",
                table: "InternetBankings");
        }
    }
}
