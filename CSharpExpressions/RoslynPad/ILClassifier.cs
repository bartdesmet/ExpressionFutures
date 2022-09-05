// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoslynPad
{
    // NB: This is a quick-n-dirty classifier for our IL string representation and is not meant to be
    //     a full-fledged IL language lexer and parser. It makes various assumptions about the format
    //     returned by our modified version of ClrTest.Reflection.
    //
    //     One particular goal of this classifier is to highlight branch labels that are used as jump
    //     instruction targets in order to make visual inspection of the basic blocks easier.

    static class ILClassifier
    {
        public static IEnumerable<ClassifiedSpan> Classify(string il)
        {
            var methods = new List<TextSpan>();

            var firstCharacterOfLine = true;
            var currentStart = 0;

            for (var i = 0; i < il.Length; i++)
            {
                var c = il[i];

                if (c == '\r')
                {
                    if (i + 1 < il.Length && il[i + 1] == '\n')
                    {
                        i++;
                    }

                    firstCharacterOfLine = true;
                }
                else if (c == '\n')
                {
                    firstCharacterOfLine = true;
                }
                else if (firstCharacterOfLine)
                {
                    firstCharacterOfLine = false;

                    if (c != '{' && c != '}' && !char.IsWhiteSpace(c))
                    {
                        if (currentStart != i)
                        {
                            methods.Add(TextSpan.FromBounds(currentStart, i));
                            currentStart = i;
                        }
                    }
                }
            }

            methods.Add(TextSpan.FromBounds(currentStart, il.Length));

            var res = Enumerable.Empty<ClassifiedSpan>();

            foreach (var method in methods)
            {
                res = res.Concat(ClassifyMethod(il, method));
            }

            res = res.OrderBy(s => s.TextSpan.Start).ToList();

            return res;
        }

        private static readonly Regex s_labelDefRegex = new Regex("[ ]+(?<label>IL_[0-9A-Fa-f]{4}):", RegexOptions.Compiled);
        private static readonly Regex s_labelRefRegex = new Regex("(?<label>IL_[0-9A-Fa-f]{4})[^:]", RegexOptions.Compiled);
        private static readonly Regex s_instKeywRegex = new Regex("[ ]+IL_[0-9A-Fa-f]{4}: (?<keyword>[a-z0-9\\.]+)", RegexOptions.Compiled);
        private static readonly Regex s_drctKeywRegex = new Regex("^[ ]*(?<keyword>\\.method|\\.try|catch|filter|finally|fault)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex s_strLiterRegex = new Regex("^.*ldstr[ ]*(?<literal>\".*\")", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex s_typeNameRegex = new Regex(@"\[(?<assembly>[a-zA-Z0-9\._+]+)\](?<namespace>([a-zA-Z0-9_]+\.)*)(?<type>[a-zA-Z0-9_+`]+)", RegexOptions.Compiled);
        private static readonly Regex s_commentsRegex = new Regex(@"(?<comment>//.*)$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex s_keyword1Regex = new Regex(@"[ ,(<]+(?<keyword>class|valuetype)", RegexOptions.Compiled);
        private static readonly Regex s_keyword2Regex = new Regex(@"[ ]+(?<keyword>instance|method|field)", RegexOptions.Compiled);
        private static readonly Regex s_keyword3Regex = new Regex(@"[ ,(<]+(?<keyword>object|void|native int|native uint|typedref|char|string|bool|float32|float64|int8|int16|int32|int64|uint8|uint16|uint32|uint64)", RegexOptions.Compiled);
        private static readonly Regex[] s_keywRegexes = new Regex[] { s_drctKeywRegex, s_keyword1Regex, s_keyword2Regex, s_keyword3Regex };

        private static IEnumerable<ClassifiedSpan> ClassifyMethod(string il, TextSpan span)
        {
            var res = new List<ClassifiedSpan>();

            var method = il.Substring(span.Start, span.Length);

            var labelDefs = new List<string>();
            var labelRefs = new HashSet<string>();

            var labelDefMatches = s_labelDefRegex.Matches(method);
            var labelRefMatches = s_labelRefRegex.Matches(method);

            foreach (Match m in labelRefMatches)
            {
                var label = m.Groups["label"];

                var labelRefSpan = new TextSpan(span.Start + label.Index, label.Length);
                res.Add(new ClassifiedSpan("labelRef", labelRefSpan));

                labelRefs.Add(label.Value);
            }

            foreach (Match m in labelDefMatches)
            {
                var label = m.Groups["label"];
                labelDefs.Add(label.Value);

                if (labelRefs.Contains(label.Value))
                {
                    var labelDefSpan = new TextSpan(span.Start + label.Index, label.Length);
                    res.Add(new ClassifiedSpan("labelDef", labelDefSpan));
                }
                else
                {
                    var labelDefSpan = new TextSpan(span.Start + label.Index, label.Length + 1  /*:*/);
                    res.Add(new ClassifiedSpan("labelDefUnused", labelDefSpan));
                }
            }

            foreach (Match m in s_instKeywRegex.Matches(method))
            {
                var keyword = m.Groups["keyword"];
                res.Add(new ClassifiedSpan("instruction", new TextSpan(span.Start + keyword.Index, keyword.Length)));
            }

            foreach (Match m in s_keywRegexes.SelectMany(r => r.Matches(method).Cast<Match>()))
            {
                var keyword = m.Groups["keyword"];
                res.Add(new ClassifiedSpan("keyword", new TextSpan(span.Start + keyword.Index, keyword.Length)));
            }

            foreach (Match m in s_strLiterRegex.Matches(method))
            {
                var literal = m.Groups["literal"];
                res.Add(new ClassifiedSpan("string", new TextSpan(span.Start + literal.Index, literal.Length)));
            }

            foreach (Match m in s_typeNameRegex.Matches(method))
            {
                var type = m.Groups["type"];
                var assembly = m.Groups["assembly"];
                var ns = m.Groups["namespace"];
                res.Add(new ClassifiedSpan("type", new TextSpan(span.Start + type.Index, type.Length)));
                res.Add(new ClassifiedSpan("assembly", new TextSpan(span.Start + assembly.Index, assembly.Length)));
                res.Add(new ClassifiedSpan("namespace", new TextSpan(span.Start + ns.Index, ns.Length)));
            }

            foreach (Match m in s_commentsRegex.Matches(method))
            {
                var comment = m.Groups["comment"];
                res.Add(new ClassifiedSpan("comment", new TextSpan(span.Start + comment.Index, comment.Length)));
            }

            return res;
        }
    }
}
