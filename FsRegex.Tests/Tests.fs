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
let zeroOrOne a = a + "?"
let times (c: int) a = a + sprintf "{%d}" c
let moreThanTimes (c: int) a = a + sprintf "{%d,}" c
let timesRange (startTimes: int) (endTimes: int) a = a + sprintf "{%d,%d}" startTimes endTimes
let digit = "\\d"
let whiteSpace = "\\s"
let word = "\\w"
let tab = "\\t"
let extEscape s =
    let r = Regex.Escape s
    r.Replace("-", "\\-")
let oneOfChars (l: char seq) = sprintf "[%s]" (l |> Seq.map string |> Seq.map extEscape |> sjoin)
let noneOfChars (l: char seq) = sprintf "[^%s]" (l |> Seq.map string |> Seq.map extEscape |> sjoin)
let oneOf (l: string seq) = sprintf "(%s)" (l |> Seq.map Regex.Escape |> join "|")
let range startChar endChar = sprintf "[%c-%c]" startChar endChar

[<Fact>]
let t1 () = 
    let rx = startLine + zeroOrMore digit + endLine
    Assert.Equal("^\\d*$", rx)

[<Fact>]
let oneOfCharsTest () =
    let rx = oneOfChars ['0'; '-'; '9']
    Assert.Equal("[0\-9]", rx)

[<Fact>]
let rangeTest () =
    let rx = range '0' '9'
    Assert.Equal("[0-9]", rx)