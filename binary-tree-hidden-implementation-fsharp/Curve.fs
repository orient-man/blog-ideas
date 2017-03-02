open System

// points in time
type Tree<'point> =
    | Empty
    | Node of (DateTime * 'point) * left: Tree<'point> * right: Tree<'point>

module Tree =
    let rec insert (x, point) = function
        | Empty -> Node ((x, point), Empty, Empty)
        | Node ((y, _) as value, left, right) ->
            if (x > y) then
                Node (value, left, insert (x, point) right)
            elif (x < y) then
                Node (value, insert (x, point) left, right)
            else
                failwith "Point on that date already exists!"

    // from left to right
    let rec walkTree tree = seq {
        match tree with
        | Empty -> ()
        | Node ((_, point), left, right) ->
            yield! walkTree left
            yield point
            yield! walkTree right
    }

    let rec map f = function
        | Empty -> Empty
        | Node (v, left, right) -> Node (f v, map f left, map f right)

    let rec getGreaterOrEqual date tree = seq {
        match tree with
        | Empty -> ()
        | Node ((nodeDate, point), left, right) ->
            if (date > nodeDate) then
                yield! getGreaterOrEqual date right
            elif (date < nodeDate) then
                yield! getGreaterOrEqual date left
                yield point
                yield! walkTree right
            else
                yield point
                yield! walkTree right
    }

    let rec getLessOrEqual date tree = seq {
        match tree with
        | Empty -> ()
        | Node ((nodeDate, point), left, right) ->
            if (date > nodeDate) then
                yield! getLessOrEqual date right
                yield point
                yield! walkTree left
            elif (date < nodeDate) then
                yield! getLessOrEqual date left
            else
                yield point
                yield! walkTree left
    }

    let rec getAtDay day tree = seq {
        match tree with
        | Empty -> ()
        | Node ((nodeDate, point), left, right) ->
            if (day > nodeDate.Date) then
                yield! getAtDay day right
            elif (day < nodeDate.Date) then
                yield! getAtDay day left
            else
                yield point
                yield! getAtDay day left
                yield! getAtDay day right
    }

open Tree

// wrapper for C# clients hides Tree usase
// functional equivalent:
// type Curve<'point> = private Curve of Tree<'point>
type Curve<'point when 'point: null> private (tree: Tree<'point>) =
    new (values: seq<_>) = Curve(Seq.foldBack insert values Empty)

    member __.ToSeq() = tree |> walkTree
    member __.TryGetNext(date) = tree |> getGreaterOrEqual date |> Seq.tryHead |> Option.toObj
    member __.TryGetPrev(date) = tree |> getLessOrEqual date |> Seq.tryHead |> Option.toObj
    member __.TryGetAt(date: DateTime) =
        tree |> getAtDay date.Date |> Seq.truncate 2 |> List.ofSeq |> function [a] -> a | _ -> null

    member __.Shift(shiftMethod: Func<_, _>) : Curve<'point> = tree |> map shiftMethod.Invoke |> Curve

type CurveF<'point> = private CurveF of Tree<'point>

module CurveF =
    let create (values: seq<_>) = Seq.foldBack insert values Empty |> CurveF
    let toSeq (CurveF tree)  = tree |> walkTree
    let tryGetNext date (CurveF tree) = tree |> getGreaterOrEqual date |> Seq.tryHead
    let tryGetPrev date (CurveF tree) = tree |> getLessOrEqual date |> Seq.tryHead
    let tryGetAt (date: DateTime) (CurveF tree) =
        tree |> getAtDay date.Date |> Seq.truncate 2 |> List.ofSeq |> function [a] -> Some a | _ -> None
    let shift shiftMethod (CurveF tree) = tree |> map shiftMethod |> CurveF

// Usage:
let date d = let start = DateTime(2017, 01, 01) in start.AddDays(float d)
let rates = [(date 5, "c"); (date 9, "e"); (date 7, "d"); (date 1, "a"); (date 3, "b")]
let curve = Curve(rates)
let curveF = rates |> CurveF.create

curve.ToSeq() |> Seq.fold (+) "" // "abcde"
curveF |> CurveF.toSeq |> Seq.fold (+) "" // "abcde"
curve.TryGetNext(date 4) // "c"
curve.TryGetNext(date 5) // "c"
curve.TryGetPrev(date 6) // "c"
curve.TryGetPrev(date 5) // "c"

let dayC = date 5
Curve(rates).TryGetAt(dayC) // "c"
Curve((dayC.AddMinutes -10., "x") :: rates).TryGetAt(dayC) // "c"
Curve((dayC.AddMinutes 10., "x") :: rates).TryGetAt(dayC) // null

curve.Shift(fun (d, r) -> (d.AddDays 1., r.ToUpper())).ToSeq() |> Seq.fold (+) "" // "ABCDE"
curveF |> CurveF.shift (fun (d, r) -> (d.AddDays 1., r.ToUpper())) |> CurveF.toSeq |> Seq.fold (+) "" // "ABCDE"