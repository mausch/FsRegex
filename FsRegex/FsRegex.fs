module FsRegex

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
let extEscapeC = string >> extEscape
let oneOfChars (l: char seq) = sprintf "[%s]" (l |> Seq.map extEscapeC |> sjoin)
let noneOfChars (l: char seq) = sprintf "[^%s]" (l |> Seq.map extEscapeC |> sjoin)
let oneOf (l: string seq) = sprintf "(%s)" (l |> join "|") // does NOT assume literal elements!
let charRange (startChar: char) (endChar: char) = sprintf "[%s-%s]" (extEscapeC startChar) (extEscapeC endChar)
let chars (startChar: char) (endChar: char) = sprintf "%s-%s" (extEscapeC startChar) (extEscapeC endChar)
let range s = sprintf "[%s]" s
let notRange s = sprintf "[^%s]" s