namespace Accounts.ReadModel.Migrations;

[Migration(20220220080600)]
public sealed class Create_Accounts_table : Migration
{
    public override void Up()
    {
        Create
            .Table("Accounts")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("Version").AsInt64().NotNullable()
            .WithColumn("ClientId").AsGuid().NotNullable()
            .WithColumn("InterestRate").AsDecimal().NotNullable()
            .WithColumn("Balance").AsDecimal().NotNullable()
            .WithColumn("IsFrozen").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Table("Accounts");
    }
}
