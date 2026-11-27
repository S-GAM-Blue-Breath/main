using System.Collections.Generic;
using System.Text;

// 엑셀에서 저장한 CSV를 한 줄씩 칸으로 나눠 주는 도우미
// "알잖아 나, 할 수 있는 거ㅋ" 처럼 따옴표 안에 쉼표가 있어도 제대로 읽어요.
public static class CsvReader
{
    public static List<string[]> Parse(string text)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        text = text.TrimStart('﻿'); // UTF-8 BOM 제거

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // "" 는 따옴표 한 개를 뜻함
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
                continue;
            }

            switch (c)
            {
                case '"':
                    inQuotes = true;
                    break;
                case ',':
                    fields.Add(current.ToString());
                    current.Clear();
                    break;
                case '\r':
                    break;
                case '\n':
                    fields.Add(current.ToString());
                    current.Clear();
                    rows.Add(fields.ToArray());
                    fields.Clear();
                    break;
                default:
                    current.Append(c);
                    break;
            }
        }

        if (current.Length > 0 || fields.Count > 0)
        {
            fields.Add(current.ToString());
            rows.Add(fields.ToArray());
        }

        return rows;
    }

    // 칸이 없으면 빈 문자열을 돌려줌
    public static string Get(string[] row, int column)
    {
        return column < row.Length ? row[column].Trim() : string.Empty;
    }
}
