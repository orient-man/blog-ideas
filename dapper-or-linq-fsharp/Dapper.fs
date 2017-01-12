module DbAccess.Dapper

open System.Collections.Generic
open System.Data.Common
open System.Dynamic
open Dapper

let dapperQuery<'Result> (query: string) (connection: DbConnection) =
    connection.Query<'Result>(query)

let dapperParametrizedQuery<'Result>
    (query: string)
    (param: obj)
    (connection: DbConnection)
    : 'Result seq =
    connection.Query<'Result>(query, param)

let dapperMapParametrizedQuery<'Result>
    (query: string)
    (param: Map<string, _>)
    (connection: DbConnection)
    : 'Result seq =
    let expando = ExpandoObject()
    let expandoDictionary = expando :> IDictionary<string, obj>
    for paramValue in param do
        expandoDictionary.Add(paramValue.Key, paramValue.Value :> obj)
    connection |> dapperParametrizedQuery query expando