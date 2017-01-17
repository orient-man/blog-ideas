module IntegrationTests.Common

open System
open IntegrationTests.DbAccess
open IntegrationTests.DbMigrations

let private init (db: DatabaseFixture) =
    Migrator.StartMigrations(db.ConnectionString)
    //db.ExecuteNonQuery "CREATE TABLE ..."

let Database = DatabaseInstance(init)

let execute query = query |> Connection.execute Database.Instance.GetConnection