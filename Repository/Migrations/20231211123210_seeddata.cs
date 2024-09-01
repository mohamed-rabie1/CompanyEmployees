using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.Metrics;
using System.Net;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Companies",
            columns: new[] { "CompanyId", "Name", "Address", "Country" },
            values: new object[,]
            {
                { new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), "IT_Solutions Ltd", "583 Wall Dr. Gwynn Oak, MD 21207","USA" },
                { new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"), "Admin_Solutions Ltd", "312 Forest Avenue, BF 923","USA" }
                
            });

            migrationBuilder.InsertData(
           table: "Employees",
           columns: new[] { "EmployeeId", "Name", "Age", "Position" , "CompanyId" },
           values: new object[,]
           {
                { Guid.NewGuid().ToString(), "Jana McLeaf", 30,"Software developer",new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870") },
                { Guid.NewGuid().ToString(), "Sam Raiden", 35,"Web developer",new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870") },
                { Guid.NewGuid().ToString(), "Kane Miller", 20,"Adminstrator",new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3") }
           });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "Companies");
            migrationBuilder.DropTable(
            name: "Employees");
        }
    }
}
