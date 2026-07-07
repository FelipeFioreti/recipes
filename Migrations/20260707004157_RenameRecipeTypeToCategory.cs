using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecipeTypeToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeTypes_RecipeTypeId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeTypes",
                table: "RecipeTypes");

            migrationBuilder.RenameTable(
                name: "RecipeTypes",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "RecipeTypeId",
                table: "Recipes",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_RecipeTypeId",
                table: "Recipes",
                newName: "IX_Recipes_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeTypes_Name",
                table: "Categories",
                newName: "IX_Categories_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                newName: "IX_RecipeTypes_Name");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Recipes",
                newName: "RecipeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes",
                newName: "IX_Recipes_RecipeTypeId");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "RecipeTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeTypes",
                table: "RecipeTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeTypes_RecipeTypeId",
                table: "Recipes",
                column: "RecipeTypeId",
                principalTable: "RecipeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
