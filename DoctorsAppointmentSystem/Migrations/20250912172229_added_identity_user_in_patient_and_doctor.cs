using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorsAppointmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class added_identity_user_in_patient_and_doctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Doctors");
        }
    }
}
