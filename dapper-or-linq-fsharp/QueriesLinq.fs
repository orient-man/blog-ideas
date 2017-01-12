module DbAccess.QueriesLinq

open DbAccess.Dapper

type Result = { ColumnA: string; ColumnB: int }
type QueryParams { Id: 15 }

let getResults (ctx: DataContext) =
    query {
        for row in ctx.TableName do
        select ({ ColumnA = row.ColumnA; ColumnB = row.ColumnB }) }

let getResult id (ctx: DataContext) =
    query {
        for row in ctx.TableName do
        where (row.Id = id)
        select ({ ColumnA = row.ColumnA; ColumnB = row.ColumnB }) }

// Persistence
type RowId = RowId of int   // for type safety!
type TimeStamp = TimeStamp of int

let tryGetForUpdate (rows: IQueryable<TableName>) (RowId id) (TimeStamp t) =
    query {
        for row in rows do
        where (row.Id = id && row.TimeStamp = t) }
    |> Seq.tryPick Some

let update tryGetRow (request: UpdateRequest) =
    let copyData (row: RowEntity) =
        row.A <- request.A
        row

    tryGetRow request.Id request.TimeStamp
    |> Option.map copyData
    |> Option.iter persist

// OO wrapper for integration with C# where:
// public interface ITransactionScope : IDisposable
// {
//     void Complete();
// }
type DataAccess
    (   dataContext: DataContext,
        transactionFactory: Func<ITransactionScope>,
        dal: IDal<TableName>) =

    let tryGetRow = Queries.tryGetForUpdate dataContext.TableName
    let persist row = dal.PersistChanges(row, true)
    let update = Persistence.update persist tryGetRow

    interface IDataAccess with
        member this.GetResults() = Queries.getResults dataContext :> _

        member this.Update request =
            use tx = transactionFactory.Invoke()
            request |> update
            tx.Complete()