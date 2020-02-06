using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.ReadModel.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    InterestRate = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    IsFrozen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
