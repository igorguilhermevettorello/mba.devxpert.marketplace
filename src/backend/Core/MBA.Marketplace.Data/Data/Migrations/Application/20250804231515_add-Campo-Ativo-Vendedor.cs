﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBA.Marketplace.Data.Data.Migrations.Application
{
    /// <inheritdoc />
    public partial class addCampoAtivoVendedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Vendedores",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Vendedores");
        }
    }
}
