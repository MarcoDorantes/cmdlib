using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace utility
{
    public static class Escape
    {
        public static void Write(string format, params object[] args)
        {
            Write(Console.Out, format, args);
        }

        public static void Write(System.IO.TextWriter writer, string format, params object[] args)
        {
            try
            {
                for (int k = 0; k < format.Length; ++k)
                {
                    char c = format[k];
                    switch (c)
                    {
                        case '$':
                        case '#':
                            #region color escaping
                            {
                                string tag;
                                int end_escape;
                                if (!try_parse(format, k, '|', out tag, out end_escape))
                                {
                                    break;
                                }
                                if (tag == "reset")
                                {
                                    Console.ResetColor();
                                    k = end_escape;
                                }
                                else
                                {
                                    ConsoleColor color;
                                    if (Enum.TryParse<ConsoleColor>(tag, true, out color))
                                    {
                                        if (c == '$')
                                        {
                                            Console.ForegroundColor = color;
                                        }
                                        else
                                        {
                                            Console.BackgroundColor = color;
                                        }
                                        k = end_escape;
                                    }
                                    else
                                    {
                                        writer.Write(c);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case '{':
                            #region placeholding
                            {
                                string tag;
                                int end_placeholder;
                                if (try_parse(format, k, '}', out tag, out end_placeholder))
                                {
                                    int index = int.Parse(tag.Substring(0, 1));
                                    string isolatedformat = "{0" + tag.Substring(1) + "}";
                                    writer.Write(isolatedformat, args[index]);
                                    k = end_placeholder;
                                }
                                else
                                    throw new FormatException("Input string was not in a correct format.");
                            }
                            #endregion
                            break;
                        default:
                            writer.Write(c);
                            break;
                    }
                }
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static bool try_parse(string format, int start_escape, char tag_end, out string tag, out int end_escape)
        {
            tag = string.Empty;
            end_escape = -1;
            end_escape = format.IndexOf(tag_end, start_escape);
            if (end_escape < 0) return false;
            int tag_size = end_escape - start_escape - 1;
            if (tag_size <= 0) return false;
            tag = format.Substring(start_escape + 1, tag_size);
            return true;
        }
    }
}