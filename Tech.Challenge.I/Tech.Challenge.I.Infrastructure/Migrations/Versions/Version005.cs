using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace Tech.Challenge.I.Infrastructure.Migrations.Versions;

[ExcludeFromCodeCoverage]
[Migration((long)NumberVersions.AddUserIdTableContact, "Add userId table contacts")]
public class Version005 : Migration
{
    public override void Down()
    {
        throw new NotImplementedException();
    }

    public override void Up()
    {
        Alter.Table("Contacts")
        .AddColumn("UserId").AsGuid().NotNullable();

        Execute.Sql(@"
                    START TRANSACTION;

                    UPDATE contacts
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

        Create.ForeignKey("FK_Contacts_Users")
            .FromTable("Contacts").ForeignColumn("UserId")
            .ToTable("Users").PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.None);
    }
}
