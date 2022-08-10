using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment2.Migrations
{
    public partial class fixAd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "brokerageId",
                table: "ad",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ad_brokerageId",
                table: "ad",
                column: "brokerageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ad_brokerage_brokerageId",
                table: "ad",
                column: "brokerageId",
                principalTable: "brokerage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ad_brokerage_brokerageId",
                table: "ad");

            migrationBuilder.DropIndex(
                name: "IX_ad_brokerageId",
                table: "ad");

            migrationBuilder.DropColumn(
                name: "brokerageId",
                table: "ad");
        }
    }
}
