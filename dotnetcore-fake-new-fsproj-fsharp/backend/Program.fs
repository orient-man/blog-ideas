// Learn more about F# at http://fsharp.org

open System
open Newtonsoft.Json

[<EntryPoint>]
let main argv =
    (Some "Hello World from F#!") :> obj
    |> JsonConvert.SerializeObject
    |> printfn "%s"
    0 // return an integer exit code
