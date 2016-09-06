using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Execute;

namespace Traffix.Migrations.Extensions
{
    public static class MigratorExtensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax WithAuditInfo(
            this ICreateTableColumnOptionOrWithColumnSyntax ct)
        {
            return ct
                .WithColumn("CreatedBy").AsString(255).NotNullable()
                .WithColumn("CreatedTime").AsDateTime().NotNullable()
                .WithColumn("ModifiedBy").AsString(255).NotNullable()
                .WithColumn("ModifiedTime").AsDateTime().NotNullable();
        }

        public static void DropViewIfExists(this IExecuteExpressionRoot execute, string viewName)
        {
            execute.Sql(string.Format("IF EXISTS(select * FROM sys.views where name = '{0}') DROP VIEW [{0}]", viewName));
        }

        public static void DropTableIfExists(this IExecuteExpressionRoot execute, string tableName)
        {
            execute.Sql(string.Format("IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}')) BEGIN DROP TABLE [{0}] END", tableName));
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax AsMaxString(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsString(int.MaxValue);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AsMaxString(this IAlterTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsString(int.MaxValue);
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithId(
            this ICreateTableWithColumnOrSchemaSyntax ct)
        {
            return ct
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity();
        }
    }
}
