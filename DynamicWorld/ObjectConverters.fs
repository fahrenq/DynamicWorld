namespace DynamicWorld

open Shared
open System
open System.Collections.Generic
open System.Globalization

[<AutoOpen>]
module ObjectConverters =
  let asOption: StepResult<'t> -> StepResult<'t option> =
    function
    | Ok a -> Ok <| Some a
    | Error e ->
      match e with
      | KeyNotFound _ -> Ok None
      | KeyIsNull _ -> Ok None
      | x -> Error x

  let asOptionIfUndefined: StepResult<'t> -> StepResult<'t option> =
    function
    | Ok a -> Ok <| Some a
    | Error e ->
      match e with
      | KeyNotFound _ -> Ok None
      | x -> Error x

  let rec asString: obj -> StepResult<string> =
    function
    | :? (StepResult<obj>) as x -> x |> Result.bind asString
    | :? (PathAndValue<obj>) as x when (x.Value = null) -> Error <| KeyIsNull x.Path
    | :? (PathAndValue<obj>) as x when (x.Value :? string) -> Ok(x.Value :?> string)
    | :? string as x -> Ok x
    | x -> unexpectedTypeErrorCase x DynamicType.String

  let rec asDateTimeOffset: obj -> StepResult<DateTimeOffset> =
    function
    | :? (StepResult<obj>) as x -> x |> Result.bind asDateTimeOffset
    | :? (PathAndValue<obj>) as x when (x.Value = null) -> Error <| KeyIsNull x.Path
    | :? (PathAndValue<obj>) as x when (x.Value :? DateTimeOffset) -> Ok(x.Value :?> DateTimeOffset)
    | :? (PathAndValue<obj>) as x when (x.Value :? string) ->
      match DateTimeOffset.TryParse(x.Value :?> string, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) with
      | true, d -> Ok d
      | false, _ -> unexpectedTypeErrorCase x DynamicType.DateTime
    | :? DateTimeOffset as x -> Ok x
    | x -> unexpectedTypeErrorCase x DynamicType.DateTime

  let rec asBool: obj -> StepResult<bool> =
    function
    | :? (StepResult<obj>) as x -> x |> Result.bind asBool
    | :? (PathAndValue<obj>) as x when (x.Value = null) -> Error <| KeyIsNull x.Path
    | :? (PathAndValue<obj>) as x when (x.Value :? bool) -> Ok(x.Value :?> bool)
    | :? bool as x -> Ok x
    | x -> unexpectedTypeErrorCase x DynamicType.Bool

  let rec asInt: obj -> StepResult<int> =
    function
    | :? (StepResult<obj>) as x -> x |> Result.bind asInt
    | :? (PathAndValue<obj>) as x when (x.Value = null) -> Error <| KeyIsNull x.Path
    | :? (PathAndValue<obj>) as x when (x.Value :? int) -> Ok (x.Value :?> int)
    | :? int as x -> Ok <| x
    | x -> unexpectedTypeErrorCase x DynamicType.Integer32

  let rec asDecimal: obj -> StepResult<decimal> =
    function
    | :? (StepResult<obj>) as x -> x |> Result.bind asDecimal
    | :? (PathAndValue<obj>) as x when (x.Value = null) -> Error <| KeyIsNull x.Path
    | :? (PathAndValue<obj>) as x when (x.Value :? int) -> Ok <| decimal (x.Value :?> int)
    | :? (PathAndValue<obj>) as x when (x.Value :? int64) -> Ok <| decimal (x.Value :?> int64)
    | :? (PathAndValue<obj>) as x when (x.Value :? single) -> Ok <| decimal (x.Value :?> single)
    | :? (PathAndValue<obj>) as x when (x.Value :? double) -> Ok <| decimal (x.Value :?> double)
    | :? (PathAndValue<obj>) as x when (x.Value :? decimal) -> Ok(x.Value :?> decimal)
    | :? int as x -> Ok <| decimal x
    | :? int64 as x -> Ok <| decimal x
    | :? single as x -> Ok <| decimal x
    | :? double as x -> Ok <| decimal x
    | :? decimal as x -> Ok <| decimal x
    | x -> unexpectedTypeErrorCase x DynamicType.Decimal

  let rec asSeq: obj -> StepResult<seq<obj>> =
    function
    | :? (StepResult<obj>) as x -> x |> Result.bind asSeq
    | :? (PathAndValue<obj>) as x when (x.Value = null) -> Error <| KeyIsNull x.Path
    | :? (PathAndValue<obj>) as x when (x.Value :? IEnumerable<obj>) ->
      Ok <| (x.Value :?> seq<obj>)
    | :? (IEnumerable<obj>) as x -> Ok x
    | x -> unexpectedTypeErrorCase x DynamicType.Sequence

  let rec asList (x: obj): StepResult<obj list> =
    x |> asSeq |> Result.map Seq.toList

