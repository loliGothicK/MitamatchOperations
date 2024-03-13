using System;
using System.Linq;

namespace Mitama.Lib;

public class Indoc
{
    public static string Unindent(string Doc)
    {
        var trim = Doc
            .Split('\n')
            .Where(x => x.Length > 0)
            .Select(line => line.TakeWhile(char.IsWhiteSpace).Count())
            .Min();
        return string.Join('\n', Doc.Split('\n').Select(line => line.Length > trim ? line[trim..] : line));
    }
}
