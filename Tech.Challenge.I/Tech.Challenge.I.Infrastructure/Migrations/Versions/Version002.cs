using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Tech.Challenge.I.Infrastructure.Migrations.Versions;

[ExcludeFromCodeCoverage]
[Migration((long)NumberVersions.CreateDDDTable, "Create DDDRegions table")]
public class Version002 : Migration
{
    public override void Down()
    {
    }

    public override void Up()
    {
        var table = VersionBase.InsertStandardColumns(Create.Table("DDDRegions"));

        table
            .WithColumn("DDD").AsInt32().NotNullable()
            .WithColumn("Region").AsString(30).NotNullable();

        Execute.Sql(@"ALTER TABLE DDDRegions
                          ADD CONSTRAINT CK_Region CHECK (Region IN ('Norte', 'Nordeste', 'CentroOeste', 'Sudeste', 'Sul'));");
    }
}
