namespace DynamicWorld

module Shared =
  let unexpectedTypeErrorCase (x: obj) expectedType =
    let path, value =
      match x with
      | :? (PathAndValue<obj>) as pv -> pv.Path, pv.Value
      | _ -> [||], x
    Error <| UnexpectedValue { Path = path; ExpectedType = expectedType; ActualValue = value }


