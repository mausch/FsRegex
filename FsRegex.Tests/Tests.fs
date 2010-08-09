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
let oneOf (l: string seq) = sprintf "(%s)" (l |> join "|") // does NOT assume literal elements!
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
    let day = digit |> timesRange 1 2
    let month = day
    let year = digit |> times 4
    let separator = "/"
    let rx = month + separator + day + separator + year
    Assert.Equal("\\d{1,2}/\\d{1,2}/\\d{4}", rx)

[<Fact>]
let emailTest() =
    let alphaNumSigns = chars 'A' 'Z' + chars '0' '9' + ".-"
    let local = oneOrMore (range (alphaNumSigns + "_%+"))
    let domain = oneOrMore (range alphaNumSigns) + literal "." + (range (chars 'A' 'Z') |> timesRange 2 4)
    let rx = wordBoundary + local + "@" + domain + wordBoundary
    Assert.Equal("\\b[A-Z0-9.-_%+]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", rx) // from http://www.regular-expressions.info/email.html

[<Fact>]
let ipTest() =
    let p25x = "25" + charRange '0' '5'
    let digit = charRange '0' '9'
    let p2xx = "2" + charRange '0' '4' + digit
    let pLessThan200 = zeroOrOne (oneOfChars ['0'; '1']) + digit + zeroOrOne digit
    let element = oneOf [p25x; p2xx; pLessThan200]
    let dot = literal "."
    let rx = wordBoundary + (Array.create 4 element |> join dot) + wordBoundary
    // from http://www.regular-expressions.info/examples.html
    Assert.Equal("\\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b", rx)
