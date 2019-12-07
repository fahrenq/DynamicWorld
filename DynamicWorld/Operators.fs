namespace DynamicWorld

open System.Collections.Generic
open Shared

[<AutoOpen>]
module Operators =
  let rec lookup (o: obj) s =
    let parse pathSoFar (dic: IDictionary<string, obj>) =
      let nextPathSoFar = Array.append pathSoFar [| s |]
      match dic.TryGetValue s with
      | (true, v) -> Ok <| box { Path = nextPathSoFar; Value = v }
      | (false, _) -> Error <| KeyNotFound nextPathSoFar

    match o with
    | :? Result<obj, DynamicWorldError> as r -> Result.bind (fun x -> lookup x s) r
    | :? (PathAndValue<obj>) as pv when (pv.Value :? IDictionary<string, obj>) ->
      let pathSoFar = pv.Path
      let dic = pv.Value
      parse pathSoFar (dic :?> IDictionary<string, obj>)
    | :? IDictionary<string, obj> as dic ->
      parse [||] dic
    | x ->
      unexpectedTypeErrorCase x DynamicType.Dictionary

  let rec (?) (o: obj) s = lookup o s



