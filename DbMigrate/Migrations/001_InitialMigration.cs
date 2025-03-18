﻿using FluentMigrator;
using PasswordKeeper.DAO;

namespace DbMigrate.Migrations;

/// <summary>
/// The initial database migration.
/// </summary>
[Migration(001)]
public class InitialMigration : Migration 
{
    /// <inheritdoc cref="MigrationBase.Up" />
    public override void Up()
    {
        this.Create.Table(nameof(KeyData)).InSchema(Program.DatabaseName).WithColumn(nameof(KeyData.Id))
            .AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(KeyData.JwtSecurityKey))
            // Add maximum of 4096-bit key as base64 string
            .AsString(Helpers.Base64ByteCount(Helpers.ByteCountFromBits(4096))).NotNullable();
        
        this.Create.Table(nameof(User)).InSchema(Program.DatabaseName)
            .WithColumn(nameof(User.Id)).AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(User.UserName)).AsString(255).NotNullable().Unique()
            .WithColumn(nameof(User.PasswordHash)).AsString(1000).NotNullable()
            .WithColumn(nameof(User.PasswordSalt)).AsString(1000).NotNullable();
    }

    /// <inheritdoc cref="MigrationBase.Down" />
    public override void Down()
    {
        this.Delete.Table(nameof(KeyData)).InSchema(Program.DatabaseName);
        this.Delete.Table(nameof(User)).InSchema(Program.DatabaseName);
    }
}