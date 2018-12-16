using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAbpDemo.Infrastructure.EFCore.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditInfoLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    ImpersonatorUserId = table.Column<long>(nullable: true),
                    ImpersonatorTenantId = table.Column<int>(nullable: true),
                    ServiceName = table.Column<string>(nullable: true),
                    MethodName = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    ExecutionDuration = table.Column<int>(nullable: false),
                    ClientIpAddress = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    BrowserInfo = table.Column<string>(nullable: true),
                    CustomData = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditInfoLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Age = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsReview = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Age = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LearnLevel = table.Column<byte>(nullable: false),
                    TeacherId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "Age", "CreationTime", "CreatorUserId", "DeleterUserId", "DeletionTime", "IsActive", "IsDeleted", "IsReview", "LastModificationTime", "LastModifierUserId", "Name" },
                values: new object[] { 1L, 18, new DateTime(2018, 12, 14, 18, 26, 56, 549, DateTimeKind.Local), null, null, null, true, false, true, null, null, "朱老师" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Age", "CreationTime", "CreatorUserId", "DeleterUserId", "DeletionTime", "IsActive", "IsDeleted", "LastModificationTime", "LastModifierUserId", "LearnLevel", "Name", "TeacherId" },
                values: new object[] { 1L, 18, new DateTime(2018, 12, 14, 18, 26, 56, 551, DateTimeKind.Local), null, null, null, true, false, null, null, (byte)1, "学生1", 1L });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Age", "CreationTime", "CreatorUserId", "DeleterUserId", "DeletionTime", "IsActive", "IsDeleted", "LastModificationTime", "LastModifierUserId", "LearnLevel", "Name", "TeacherId" },
                values: new object[] { 2L, 36, new DateTime(2018, 12, 14, 18, 26, 56, 551, DateTimeKind.Local), null, null, null, true, false, null, null, (byte)4, "学生2", 1L });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TeacherId",
                table: "Students",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditInfoLog");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
