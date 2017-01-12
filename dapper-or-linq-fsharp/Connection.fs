module DbAccess.Connection

open System.Data.Common
open System.Data.SqlClient

let getConnection connectionString () = new SqlConnection(connectionString) :> DbConnection

let execute getConnection query = use cnx = getConnection () in query cnx