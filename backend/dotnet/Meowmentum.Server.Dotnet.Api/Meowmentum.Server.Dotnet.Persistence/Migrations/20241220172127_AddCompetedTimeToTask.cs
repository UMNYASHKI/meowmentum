﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meowmentum.Server.Dotnet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetedTimeToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Tasks");
        }
    }
}
