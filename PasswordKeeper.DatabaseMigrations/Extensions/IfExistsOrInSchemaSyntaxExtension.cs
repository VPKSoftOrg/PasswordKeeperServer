using FluentMigrator.Builders;

namespace PasswordKeeper.DatabaseMigrations.Extensions;

/// <summary>
/// Extension methods for a class implementing the <see cref="IIfExistsOrInSchemaSyntax"/> interface.
/// </summary>
public static class IfExistsOrInSchemaSyntaxExtension
{
    /// <summary>
    /// Conditionally sets the schema for a table creation operation.
    /// </summary>
    public static void InSchemaIf(this IIfExistsOrInSchemaSyntax tableSyntax, string schemaName, bool useSchema)
    {
        if (useSchema)
        {
            tableSyntax.InSchema(schemaName);
        }
    }
}