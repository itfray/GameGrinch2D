using System.Collections.Generic;


public static class StringExtension
{
    /// <summary>
    /// Reads lines from a string from start to end
    /// </summary>
    /// <param name="text"> string </param>
    /// <returns> IEnumerable<string> </returns>
    public static IEnumerable<string> ReadLines(this string text)
    {
        return ReadLines(text, true);
    }

    /// <summary>
    /// Reads lines from a string from end to start
    /// </summary>
    /// <param name="text"> string </param>
    /// <returns> IEnumerable<string> </returns>
    public static IEnumerable<string> ReadLinesEnd(this string text)
    {
        return ReadLines(text, false);
    }

    /// <summary>
    /// Reads lines from a string
    /// </summary>
    /// <param name="text"> string </param>
    /// <param name="fromLeft"> true: from left to right reading, false: from right to left </param>
    /// <returns> IEnumerable<string> </returns>
    private static IEnumerable<string> ReadLines(this string text, bool fromLeft)
    {
        int pos;                                                            // position in text
        int ind;                                                            // position in text whom is start/end for new line
        int one;                                                            // increment/decrement
        int end_pos;                                                        // end postion in text
        System.Func<int, int, bool> cond_loop;                              // condition for completing of loop

        if (fromLeft)                                                       // if read text from left to right
        {
            pos = 0;
            one = 1;
            cond_loop = (a, b) => a <= b;
        }
        else
        {
            pos = text.Length - 1;
            one = -1;
            cond_loop = (a, b) => a >= b;
        }
        ind = pos;
        end_pos = text.Length - 1 - pos;

        while (cond_loop(pos, end_pos))
        {
            if (text[pos] == '\n' || pos == end_pos)                        // if line break or text end
            {
                if (pos == end_pos) pos += one;

                int line_len;                                               // length line
                int line_istart;                                            // index of line's first element in text
                if (fromLeft)
                {
                    line_len = pos - ind;
                    line_istart = ind;
                }
                else
                {
                    line_len = ind - pos;
                    line_istart = pos + 1;
                }

                char[] line = new char[line_len];
                text.CopyTo(line_istart, line, 0, line_len);                // copy line of text
                ind = pos + one;
                yield return new string(line);                              // return line
            }
            pos += one;
        }
    }

    /// <summary>
    /// Calculate count lines in string
    /// </summary>
    /// <param name="text"> string </param>
    /// <returns> count lines </returns>
    public static int CountLines(this string text)
    {
        int pos = 0;                                                            // position in text
        int count = 0;
        while (pos < text.Length)
        {
            if (text[pos] == '\n' || pos == text.Length - 1)                    // if line break or text end
                count++;
            pos++;
        }
        return count;
    }
}