namespace App.Web

open System
open System.Web.Http.Controllers

type User = string
type Role = string

type Authorization() =
    static let mock _ = failwith "This function should be injected at startup"
    static let mutable isAuthorized: User * Role -> bool = mock

    static member IsAuthorized
        with get () = isAuthorized
        and set (v) = isAuthorized <- v

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, Inherited = false, AllowMultiple = false)>]
type HubAuthorizeAttribute() =
    inherit Microsoft.AspNet.SignalR.AuthorizeAttribute()

    override __.UserAuthorized(user) =
        if isNull user then new ArgumentNullException("user") |> raise
        // TODO: base.Roles can contain more than 1 role!
        Authorization.IsAuthorized(user.Identity.Name, base.Roles)

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, Inherited = false, AllowMultiple = false)>]
type WebApiAuthorizeAttribute() =
    inherit System.Web.Http.AuthorizeAttribute()

    let getUser (actionContext: HttpActionContext) =
        let principal = actionContext.RequestContext.Principal
        if isNull principal || isNull principal.Identity || String.IsNullOrEmpty principal.Identity.Name
        then None
        else Some principal.Identity.Name

    override __.IsAuthorized(actionContext: HttpActionContext) =
        // TODO: base.Roles can contain more than 1 role!
        match getUser actionContext with
        | None -> false
        | Some user -> Authorization.IsAuthorized(user, base.Roles)