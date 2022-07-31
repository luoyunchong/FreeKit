using System.Text;

namespace IGeekFan.FreeKit.Extras.Extensions;

public static class StringExtensions
{
    private static readonly char[] Delimeters = { ' ', '-', '_' };

    public static long ToLong(this string @this)
    {
        return Convert.ToInt64(@this);
    }

    /// <summary>
    /// Indicates whether this string is null or an System.String.Empty string.
    /// </summary>
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
    /// <summary>
    ///  Indicates whether this string is not null or an System.String.Empty string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNotNullOrEmpty(this string? str) => !string.IsNullOrEmpty(str);

    /// <summary>
    /// indicates whether this string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    /// <summary>
    ///  indicates whether this string is not null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNotNullOrWhiteSpace(this string? str)=> !string.IsNullOrWhiteSpace(str);

    /// <summary>
    /// 帕斯卡命名法：大驼峰
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToPascalCase(this string source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return SymbolsPipe(
            source,
            '\0',
            (s, i) => new char[] { char.ToUpperInvariant(s) });
    }

    /// <summary>
    /// 小驼峰命名
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToCamelCase(this string source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return SymbolsPipe(
            source,
            '\0',
            (s, disableFrontDelimeter) =>
            {
                if (disableFrontDelimeter)
                {
                    return new char[] { char.ToLowerInvariant(s) };
                }

                return new char[] { char.ToUpperInvariant(s) };
            });
    }

    /// <summary>
    /// 短横线
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToKebabCase(this string source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return SymbolsPipe(
            source,
            '-',
            (s, disableFrontDelimeter) =>
            {
                if (disableFrontDelimeter)
                {
                    return new char[] { char.ToLowerInvariant(s) };
                }

                return new char[] { '-', char.ToLowerInvariant(s) };
            });
    }

    /// <summary>
    /// 蛇形命名法
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToSnakeCase(this string source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return SymbolsPipe(
            source,
            '_',
            (s, disableFrontDelimeter) =>
            {
                if (disableFrontDelimeter)
                {
                    return new char[] { char.ToLowerInvariant(s) };
                }

                return new char[] { '_', char.ToLowerInvariant(s) };
            });
    }

    public static string ToTrainCase(this string source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return SymbolsPipe(
            source,
            '-',
            (s, disableFrontDelimeter) =>
            {
                if (disableFrontDelimeter)
                {
                    return new char[] { char.ToUpperInvariant(s) };
                }

                return new char[] { '-', char.ToUpperInvariant(s) };
            });
    }

    private static string SymbolsPipe(
        string source,
        char mainDelimeter,
        Func<char, bool, char[]> newWordSymbolHandler)
    {
        var builder = new StringBuilder();

        bool nextSymbolStartsNewWord = true;
        bool disableFrontDelimeter = true;
        foreach (var symbol in source)
        {
            if (Delimeters.Contains(symbol))
            {
                if (symbol == mainDelimeter)
                {
                    builder.Append(symbol);
                    disableFrontDelimeter = true;
                }

                nextSymbolStartsNewWord = true;
            }
            else if (!char.IsLetterOrDigit(symbol))
            {
                builder.Append(symbol);
                disableFrontDelimeter = true;
                nextSymbolStartsNewWord = true;
            }
            else
            {
                if (nextSymbolStartsNewWord || char.IsUpper(symbol))
                {
                    builder.Append(newWordSymbolHandler(symbol, disableFrontDelimeter));
                    disableFrontDelimeter = false;
                    nextSymbolStartsNewWord = false;
                }
                else
                {
                    builder.Append(symbol);
                }
            }
        }

        return builder.ToString();
    }
}
