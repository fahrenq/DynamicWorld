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

  type UnexpectedValueError =
    {
      Path: string []
      ExpectedType: DynamicType
      ActualValue: obj
    }

  type DynamicWorldError =
    | KeyNotFound of Path
    | UnexpectedValue of UnexpectedValueError

  type StepResult<'t> = Result<'t, DynamicWorldError>


