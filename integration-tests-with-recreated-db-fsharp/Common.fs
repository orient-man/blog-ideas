module IntegrationTests.Common

open System
open IntegrationTests.DbAccess
open IntegrationTests.DbMigrations

type Database private () =
    static let init () =
        let db = new DatabaseFixture()
        // FluentMigrations...
        Migrator.StartMigrations(db.ConnectionString)
        db

    static let instance = lazy (init ())
    static member Instance = instance.Value
    static member DropIfExist() =
        if instance.IsValueCreated then (instance.Value :> IDisposable).Dispose()

let execute query = query |> Connection.execute Database.Instance.GetConnection