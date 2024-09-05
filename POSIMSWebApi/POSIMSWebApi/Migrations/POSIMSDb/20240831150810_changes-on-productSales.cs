using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSIMSWebApi.Migrations.POSIMSDb
{
    /// <inheritdoc />
    public partial class changesonproductSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSales_Products_ProductId",
                table: "ProductSales");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductSales",
                newName: "ProductPriceId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSales",
                newName: "IX_ProductSales_ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSales_ProductPrices_ProductPriceId",
                table: "ProductSales",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSales_ProductPrices_ProductPriceId",
                table: "ProductSales");

            migrationBuilder.RenameColumn(
                name: "ProductPriceId",
                table: "ProductSales",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSales_ProductPriceId",
                table: "ProductSales",
                newName: "IX_ProductSales_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSales_Products_ProductId",
                table: "ProductSales",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
