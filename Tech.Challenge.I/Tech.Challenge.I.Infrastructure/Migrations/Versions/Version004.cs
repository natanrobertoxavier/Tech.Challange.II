using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Tech.Challenge.I.Infrastructure.Migrations.Versions;

[ExcludeFromCodeCoverage]
[Migration((long)NumberVersions.AddUserIdTableDDD, "Add userId table dddregions")]
public class Version004 : Migration
{
    public override void Down()
    {
        throw new NotImplementedException();
    }

    public override void Up()
    {
        Alter.Table("DDDRegions")
        .AddColumn("UserId").AsGuid().NotNullable();

        Execute.Sql(@"
                    START TRANSACTION;

                    UPDATE dddregions
                    SET userId = (
                        SELECT Id
                        FROM users
                        ORDER BY RegistrationDate
                        LIMIT 1
                    )
                    WHERE EXISTS (
                        SELECT 1
                        FROM users
                    );
                    
                    COMMIT;
                    ");

        Create.ForeignKey("FK_DDDRegions_Users")
            .FromTable("DDDRegions").ForeignColumn("UserId")
            .ToTable("Users").PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.None);
    }
}
