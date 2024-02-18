using System;
using System.Linq;

namespace MitamatchOperations.Lib
{
    public struct Indoc(string Doc)
    {
        public readonly string Text {
            get
            {
                var trim = Doc
                    .Split('\n')
                    .Where(x => x.Length > 0)
                    .Select(line => line.TakeWhile(char.IsWhiteSpace).Count())
                    .Min();
                return string.Join('\n', Doc.Split('\n').Select(line => line.Length > trim ? line.Substring(trim) : line));
            }
        }
    }
}
