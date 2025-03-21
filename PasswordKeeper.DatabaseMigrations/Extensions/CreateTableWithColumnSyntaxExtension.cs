using FluentMigrator.Builders.Create.Table;

namespace PasswordKeeper.DatabaseMigrations.Extensions;

/// <summary>
/// Extension methods for the <see cref="ICreateTableWithColumnOrSchemaSyntax"/> class.
/// </summary>
public static class CreateTableWithColumnSyntaxExtension
{

    /// <summary>
    /// Conditionally sets the schema for a table creation operation.
    /// </summary>
    /// <param name="syntax">The table creation syntax to apply the schema to.</param>
    /// <param name="schemaName">The name of the schema to use if <paramref name="useSchema"/> is <c>true</c>.</param>
    /// <param name="useSchema">Determines whether to apply the schema name. If <c>true</c>, the schema is applied; otherwise, it is not.</param>
    /// <returns>The table creation syntax with or without the schema applied.</returns>
    public static ICreateTableWithColumnSyntax InSchemaIf(this ICreateTableWithColumnOrSchemaSyntax syntax,
        string schemaName, bool useSchema)
    {
        return useSchema ? syntax.InSchema(schemaName) : syntax;
    }
}