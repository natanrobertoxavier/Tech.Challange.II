using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Tech.Challenge.I.Infrastructure.Migrations.Versions;

[ExcludeFromCodeCoverage]
[Migration((long)NumberVersions.CreateContactTable, "Create Contacts table")]
public class Version003 : Migration
{
    public override void Down()
    {
        throw new NotImplementedException();
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("Contacts"));

        table
            .WithColumn("FirstName").AsString(30).NotNullable()
            .WithColumn("LastName").AsString(30).NotNullable()
            .WithColumn("DDDId").AsGuid().NotNullable()
            .WithColumn("PhoneNumber").AsString(10).NotNullable()
            .WithColumn("Email").AsString(100).NotNullable();

        Create.ForeignKey("FK_DDDRegions_Contacts")
            .FromTable("Contacts").ForeignColumn("DDDId")
            .ToTable("DDDRegions").PrimaryColumn("Id");
    }
}
