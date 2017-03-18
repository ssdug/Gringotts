# Useful Database Commands

This project uses Entity Framework 6 Migrations to manage database schema.

Migrations are stored in Data\Migrations and are created programatically based on the ApplicationDbContext class and its associated configuration classes. ApplicationDbContext specifies the connection string to use for all operations. This connection string will be used when running migrations.

The default connection string name is DefaultConnection.

DefaultConnection is defined in the web.config as: data source=.; #use the default instance on the local machine initial catalog=Gringotts #database named Gringotts

The migration system creates a _MigrationHistory table to track migration
state.

Here are some helpful commands. they will all respect the _MigrationHistory
table.

`Update-Database`

- Will run all needed migrations on DefaultConnection. If the database does not exist it will be created.

`Update-Database -TargetMigration "MigrationName"`

- Will run all the needed downgrade migrations to get to the specified version.

`Add-Migration {Name}`

- Creates a new migration named {Name} based on the current Model state of ApplicationDbContext. If no changes are detected, the migration is created as an empty migration that can be used to hand craft a migration step. This is useful for adding initial state data or doing data cleanup.

`Get-Migrations`

- Displays the migrations that have been applied to the target database.

`Update-Database -Script -SourceMigration $InitialDatabase`

- Generates a deployment script that respects the _MigrationHistory table that can be used in protected environments.

`Update-Database -Script -TargetMigration $InitialDatabase`

- Generates a destructive script that removes all tables from the database