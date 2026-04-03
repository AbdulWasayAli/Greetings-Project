using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Project.Migrations
{
    public partial class CreateUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),

                    Phone = table.Column<string>(type: "nvarchar(11)", nullable: false),

                    Email = table.Column<string>(type: "nvarchar(150)", nullable: false),

                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
