using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Course");

            migrationBuilder.EnsureSchema(
                name: "BaseSystem");

            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "Person",
                schema: "BaseSystem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 587, DateTimeKind.Local).AddTicks(4318)),
                    UserId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    Mobile = table.Column<string>(nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    FileId = table.Column<Guid>(nullable: false),
                    Ext = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Grade = table.Column<byte>(nullable: false),
                    IsVideo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                schema: "BaseSystem",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 577, DateTimeKind.Local).AddTicks(3581)),
                    UserId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    FaTitle = table.Column<string>(nullable: false),
                    DateChanged = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 576, DateTimeKind.Local).AddTicks(1890))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 526, DateTimeKind.Local).AddTicks(7592)),
                    UserId = table.Column<long>(nullable: false),
                    EnTitle = table.Column<string>(maxLength: 450, nullable: false),
                    FaTitle = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                schema: "Course",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 610, DateTimeKind.Local).AddTicks(6947)),
                    UserId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(maxLength: 150, nullable: false),
                    PersonId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    FileId = table.Column<Guid>(nullable: false),
                    Ext = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "BaseSystem",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 511, DateTimeKind.Local).AddTicks(1387)),
                    UserId = table.Column<long>(nullable: false),
                    PersonId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(maxLength: 450, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    SerialNumber = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 20, nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Mobile = table.Column<string>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false, defaultValue: false),
                    LastLoggedIn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "BaseSystem",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourse",
                schema: "Course",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 616, DateTimeKind.Local).AddTicks(2977)),
                    UserId = table.Column<long>(nullable: false),
                    PersonId = table.Column<long>(nullable: false),
                    CourseId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourse_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Course",
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentCourse_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "BaseSystem",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 563, DateTimeKind.Local).AddTicks(5521))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Users",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDelete = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 10, 25, 16, 59, 15, 572, DateTimeKind.Local).AddTicks(2638)),
                    AccessTokenHash = table.Column<string>(nullable: true),
                    AccessTokenExpiresDateTime = table.Column<DateTimeOffset>(nullable: false),
                    RefreshTokenIdHash = table.Column<string>(maxLength: 450, nullable: false),
                    RefreshTokenIdHashSource = table.Column<string>(maxLength: 450, nullable: true),
                    RefreshTokenExpiresDateTime = table.Column<DateTimeOffset>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    PersonId = table.Column<long>(nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToken_Person_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "BaseSystem",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_Id",
                schema: "BaseSystem",
                table: "Person",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_Key",
                schema: "BaseSystem",
                table: "Setting",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_Id",
                schema: "Course",
                table: "Course",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_PersonId",
                schema: "Course",
                table: "Course",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourse_CourseId",
                schema: "Course",
                table: "StudentCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourse_Id",
                schema: "Course",
                table: "StudentCourse",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourse_PersonId",
                schema: "Course",
                table: "StudentCourse",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_EnTitle",
                schema: "Users",
                table: "Role",
                column: "EnTitle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Id",
                schema: "Users",
                table: "Role",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Id",
                schema: "Users",
                table: "User",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_PersonId",
                schema: "Users",
                table: "User",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_Id",
                schema: "Users",
                table: "UserRole",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "Users",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                schema: "Users",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_Id",
                schema: "Users",
                table: "UserToken",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_PersonId",
                schema: "Users",
                table: "UserToken",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId",
                schema: "Users",
                table: "UserToken",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Setting",
                schema: "BaseSystem");

            migrationBuilder.DropTable(
                name: "StudentCourse",
                schema: "Course");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "UserToken",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "Course",
                schema: "Course");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "Person",
                schema: "BaseSystem");
        }
    }
}
