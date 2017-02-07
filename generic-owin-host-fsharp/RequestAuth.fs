module App.WebHost.RequestAuth

open System
open System.Threading.Tasks
open Microsoft.Owin

let authorizeRequest isAuthorizedTo redirectTo (context: IOwinContext) (next: Func<Task>) =
    async {
        let user = context.Authentication.User
        let path = let p = context.Request.Path in p.ToString().ToLower()
        if isNull user || isNull user.Identity || user.Identity.Name |> isAuthorizedTo path |> not
        then context.Response.Redirect(redirectTo)
        else do! next.Invoke() |> Async.AwaitTask }
    |> Async.StartAsTask
    :> Task

let isRequestForIndexPath (path: string) =
    not (isNull path) && ["/" ; "/index.html"; ""] |> List.contains (path.ToLower())

let authorizeWhenIndex isAuthorizedToIndex path user =
    if isRequestForIndexPath path then isAuthorizedToIndex user else true

let authorizeToIndex isAuthorizedToIndex redirectTo =
    authorizeRequest (authorizeWhenIndex isAuthorizedToIndex) redirectTo