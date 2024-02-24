namespace mitama.Algorithm;

internal class Algo
{
    public static int LevenshteinDistance(string str1, string str2)
    {
        var n1 = 0;
        var n2 = str2!.Length + 2;
        var d = new int[n2 << 1];

        for (var i = 0; i < n2; i++)
        {
            d[i] = i;
        }

        d[n2 - 1]++;
        d[^1] = 0;

        for (var i = 0; i < str1!.Length; i++)
        {
            d[n2] = i + 1;

            foreach (var t in str2)
            {
                var v = d[n1++];

                if (str1[i] == t)
                {
                    v--;
                }

                v = (v < d[n1]) ? v : d[n1];
                v = (v < d[n2]) ? v : d[n2];

                d[++n2] = ++v;
            }

            n1 = d[n1 + 1];
            n2 = d[n2 + 1];
        }

        return d[d.Length - n2 - 2];
    }

    public static float LevenshteinRate(string str1, string str2)
    {
        var len1 = str1?.Length ?? 0;
        var len2 = str2?.Length ?? 0;

        if (len1 > len2)
        {
            (len1, len2) = (len2, len1);
        }

        if (len1 == 0)
        {
            return (len2 == 0) ? 0.0f : 1.0f;
        }

        return LevenshteinDistance(str1, str2) / (float)len2;
    }
}
