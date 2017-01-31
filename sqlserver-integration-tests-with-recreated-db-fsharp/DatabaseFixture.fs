namespace IntegrationTests

open System
open System.Data.Common
open System.Data.SqlClient
open System.Transactions

type DatabaseFixture() =
    let config = ["<machine name>", "localhost"] |> Map.ofList

    let generateCatalogName() =
        let guid = let g = Guid.NewGuid() in g.ToString()
        guid |> Seq.filter Char.IsLetter |> Seq.truncate 10 |> String.Concat

    let catalog = "IntegrationTests_" + (generateCatalogName())

    let dataSource = config |> Map.tryFind Environment.MachineName |> defaultArg <| ".\SQLEXPRESS"

    let createConnectionString host name =
        sprintf "Data Source=%s;Initial Catalog=%s;Integrated Security=SSPI;" host name

    let getConnection connectionString =
        let conn = new SqlConnection(connectionString)
        conn.Open()
        conn :> DbConnection

    let executeNonQuery catalog query =
        let execute query (connection: DbConnection) =
            use cmd = connection.CreateCommand(CommandText = query)
            cmd.ExecuteNonQuery() |> ignore

        (dataSource, catalog) ||> createConnectionString |> getConnection |> execute query

    let connectionString = createConnectionString dataSource catalog

    do catalog |> sprintf "CREATE DATABASE %s" |> executeNonQuery "master"

    member __.ConnectionString = connectionString
    member __.GetConnection() = getConnection connectionString
    member __.ExecuteNonQuery(query) = executeNonQuery catalog query
    member __.AutoRollback() = new TransactionScope() :> IDisposable

    interface IDisposable with
        member __.Dispose() =
            (catalog, catalog)
            ||> sprintf "ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE %s"
            |> executeNonQuery "master"

type DatabaseInstance (init) =
    let create () =
        let db = new DatabaseFixture()
        db |> init
        db

    let instance = lazy (create ())

    member __.Instance = instance.Value
    member __.DropIfExist() =
        if instance.IsValueCreated then (instance.Value :> IDisposable).Dispose()