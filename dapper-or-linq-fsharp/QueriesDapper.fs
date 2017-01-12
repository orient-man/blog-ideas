module DbAccess.Queries

open DbAccess.Dapper

type Result = { ColumnA: string; ColumnB: int }
type QueryParams { Id: 15 }

let getResults connection =
    connection
    |> dapperQuery<Result> "SELECT ColumnA, ColumntB FROM [TableName]"
    |> Seq.toList

// insert/update are similar
// connection as last parameter is important!
let getResult id connection =
    connection
    |> dapperQuery<Result>
        "SELECT ColumnA, ColumntB FROM [TableName] WHERe Id = @Id"
        { Id = Id }
    |> Seq.tryHead

// Usage:
//  id |> Queries.getResult |> execute