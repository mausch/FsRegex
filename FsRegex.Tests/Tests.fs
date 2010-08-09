module Tests

open Xunit
open System
open System.Text.RegularExpressions

let join (s: string) (a: 'a seq) = String.Join(s, a)
let sjoin a = join "" a
let startLine = "^"
let endLine = "$"
let anyChar = "."
let zeroOrMore a = a + "*"
let oneOrMore a = a + "+"
let zeroOrOne a = a + "?"
let times (c: int) a = a + sprintf "{%d}" c
let moreThanTimes (c: int) a = a + sprintf "{%d,}" c
let timesRange (startTimes: int) (endTimes: int) a = a + sprintf "{%d,%d}" startTimes endTimes
let digit = "\\d"
let whiteSpace = "\\s"
let word = "\\w"
let wordBoundary = "\\b"
let tab = "\\t"
let literal = Regex.Escape
let extEscape s =
    let r = Regex.Escape s
    r.Replace("-", "\\-")
let oneOfChars (l: char seq) = sprintf "[%s]" (l |> Seq.map string |> Seq.map extEscape |> sjoin)
let noneOfChars (l: char seq) = sprintf "[^%s]" (l |> Seq.map string |> Seq.map extEscape |> sjoin)
let oneOf (l: string seq) = sprintf "(%s)" (l |> Seq.map literal |> join "|") // should I assume literal ?
let extEscapeC = string >> extEscape
let charRange startChar endChar = sprintf "[%s-%s]" (extEscapeC startChar) (extEscapeC endChar)
let chars startChar endChar = sprintf "%s-%s" (extEscapeC startChar) (extEscapeC endChar)
let range s = sprintf "[%s]" s
let notRange s = sprintf "[^%s]" s

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
    let rx = charRange '0' '9'
    Assert.Equal("[0-9]", rx)

[<Fact>]
let notRangeTest() =
    let rx = oneOrMore (notRange whiteSpace)
    Assert.Equal("[^\\s]+", rx)

[<Fact>]
let dateTest() =
    let rx = (digit |> timesRange 1 2) + "/" + (digit |> timesRange 1 2) + "/" + (digit |> times 4)
    Assert.Equal("\\d{1,2}/\\d{1,2}/\\d{4}", rx)

[<Fact>]
let emailTest() =
    let alphaNumSigns = chars 'A' 'Z' + chars '0' '9' + ".-"
    let local = oneOrMore (range (alphaNumSigns + "_%+"))
    let domain = oneOrMore (range alphaNumSigns) + literal "." + (range (chars 'A' 'Z') |> timesRange 2 4)
    let rx = wordBoundary + local + "@" + domain + wordBoundary
    Assert.Equal("\\b[A-Z0-9.-_%+]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", rx) // from http://www.regular-expressions.info/email.html
