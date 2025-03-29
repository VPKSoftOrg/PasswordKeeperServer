﻿using FluentMigrator;
using PasswordKeeper.DAO;
using PasswordKeeper.DatabaseMigrations.Extensions;

namespace PasswordKeeper.DatabaseMigrations.Migrations;

/// <summary>
/// The initial database migration.
/// </summary>
[Migration(001)]
public class InitialMigration : Migration 
{
    /// <inheritdoc cref="MigrationBase.Up" />
    public override void Up()
    {
        var isSqlite = Program.IsTestDb;
        
        this.Create.Table(nameof(KeyData)).InSchemaIf(Program.DatabaseName, !isSqlite)
            .WithColumn(nameof(KeyData.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(KeyData.JwtSecurityKey))
            // Add maximum of 4096-bit key as base64 string
            .AsString(Helpers.Base64ByteCount(Helpers.ByteCountFromBits(4096))).NotNullable();

        this.Create.Table(nameof(User)).InSchemaIf(Program.DatabaseName, !isSqlite)
            .WithColumn(nameof(User.Id)).AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(User.Username)).AsString(255).NotNullable().Unique()
            .WithColumn(nameof(User.UserFullName)).AsString(512).NotNullable()
            .WithColumn(nameof(User.PasswordHash)).AsString(1000).NotNullable()
            .WithColumn(nameof(User.PasswordSalt)).AsString(1000).NotNullable()
            .WithColumn(nameof(User.IsAdmin)).AsBoolean().NotNullable().WithDefaultValue(false);

        this.Create.Table(nameof(Collection)).InSchemaIf(Program.DatabaseName, !isSqlite)
            .WithColumn(nameof(Collection.Id)).AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(Collection.Name)).AsString(255).NotNullable().Unique()
            .WithColumn(nameof(Collection.ChallengeKeyHash)).AsString(1000).Nullable()
            .WithColumn(nameof(Collection.ChallengeKeySalt)).AsString(1000).Nullable();

        this.Create.Table(nameof(CollectionSettings)).InSchemaIf(Program.DatabaseName, !isSqlite)
            .WithColumn(nameof(CollectionSettings.Id)).AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(CollectionSettings.JsonSettings)).AsString(int.MaxValue)
            .WithColumn(nameof(CollectionSettings.CollectionId)).AsInt64().NotNullable()
            .ForeignKey(nameof(Collection), nameof(Collection.Id));
        
        this.Create.Table(nameof(CollectionItem)).InSchemaIf(Program.DatabaseName, !isSqlite)
            .WithColumn(nameof(CollectionItem.Id)).AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn(nameof(CollectionItem.CollectionId)).AsInt64().NotNullable()
            .ForeignKey(nameof(Collection), nameof(Collection.Id))
            .WithColumn(nameof(CollectionItem.ItemData)).AsString(int.MaxValue).NotNullable();
    }

    /// <inheritdoc cref="MigrationBase.Down" />
    public override void Down()
    {
        var isSqlite = Program.IsTestDb;
        
        this.Delete.Table(nameof(KeyData)).InSchemaIf(Program.DatabaseName, !isSqlite);
        this.Delete.Table(nameof(CollectionSettings)).InSchemaIf(Program.DatabaseName, !isSqlite);
        this.Delete.Table(nameof(CollectionItem)).InSchemaIf(Program.DatabaseName, !isSqlite);
        this.Delete.Table(nameof(Collection)).InSchemaIf(Program.DatabaseName, !isSqlite);
        this.Delete.Table(nameof(User)).InSchemaIf(Program.DatabaseName, !isSqlite);
    }
}