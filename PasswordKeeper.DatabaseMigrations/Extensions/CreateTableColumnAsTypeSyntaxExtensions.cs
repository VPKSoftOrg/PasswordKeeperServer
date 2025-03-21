using FluentMigrator.Builders.Create.Table;

namespace PasswordKeeper.DatabaseMigrations.Extensions;

/// <summary>
/// Extension methods for the <see cref="ICreateTableColumnAsTypeSyntax"/> class.
/// </summary>
public static class CreateTableColumnAsTypeSyntaxExtensions
{
    /// <summary>
    /// Configures a table column as a text type with a specified length, optionally using an alternate syntax.
    /// </summary>
    /// <param name="syntax">The column type syntax to apply the text configuration to.</param>
    /// <param name="length">The maximum length of the text column.</param>
    /// <param name="useAlternateSyntax">
    /// Determines whether to use an alternate custom syntax. If <c>true</c>, the column is set as 
    /// a custom "TEXT(length) NOT NULL" type; otherwise, it is set as a standard string with the specified length.
    /// </param>
    /// <returns>The configured table column syntax with text type applied.</returns>
    public static ICreateTableColumnOptionOrWithColumnSyntax AsText(this ICreateTableColumnAsTypeSyntax syntax,
        int length, bool useAlternateSyntax)
    {
        return useAlternateSyntax ? syntax.AsCustom($"TEXT({length}) NOT NULL") : syntax.AsString(length);
    }
}