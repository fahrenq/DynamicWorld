module Simple

open DynamicWorld
open FSharpx.Result

type Post = {
  Content: string
  Published: bool
  Views: decimal
  PrivateNote: string option
}

let parsePostEo i = result {
  let! content = i?Content |> asString
  let! published = i?Published |> asBool
  let! views = i?Views |> asDecimal
  let! privateNote = i?PrivateNote |> asString |> asOption

  return {
    Content = content
    Published = published
    Views = views
    PrivateNote = privateNote
  }
 }

