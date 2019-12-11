namespace DynamicWorld

[<AutoOpen>]
module Types =
  type Path = string []

  type PathAndValue<'t> =
    {
      Path: Path
      Value: 't
    }

  [<RequireQualifiedAccess>]
  type DynamicType =
    | Dictionary
    | String
    | DateTime
    | Bool
    | Decimal
    | Sequence

  type UnexpectedValueTypeError =
    {
      Path: string []
      ExpectedType: DynamicType
      ActualValue: obj
    }

  type DynamicWorldError =
    | KeyNotFound of Path
    | InvalidValue of Path * string
    | UnexpectedValueType of UnexpectedValueTypeError

  type StepResult<'t> = Result<'t, DynamicWorldError>


