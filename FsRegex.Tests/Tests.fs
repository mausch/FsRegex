module Tests

open Xunit
open System
open System.Text.RegularExpressions

let join (s: string) (a: 'a seq) = String.Join(s, a)
let sjoin a = join "" a
let startLine = "^"
let endLine = "$"
let zeroOrMore a = a + "*"
let oneOrMore a = a + "+"
let digit = "\\d"
let whiteSpace = "\\s"
let extEscape s =
    let r = Regex.Escape s
    r.Replace("-", "\\-")
let oneOf (l: char seq) = sprintf "[%s]" (l |> Seq.map string |> Seq.map extEscape |> sjoin)

[<Fact>]
let t1 () = 
    let rx = startLine + zeroOrMore digit + endLine
    Assert.Equal("^\\d*$", rx)

[<Fact>]
let t2 () =
    let rx = oneOf ['0'; '-'; '9']
    Assert.Equal("[0\-9]", rx)
    ()