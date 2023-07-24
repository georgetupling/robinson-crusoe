using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Json
{
    public static List<string> ToList(string jsonString) {

        List<string> jsonStrings = new List<string>();
        int bracketCount = 0;
        StringBuilder sb = new StringBuilder("", 100);

        for (int i = 0; i < jsonString.Length; i++) {
            if (jsonString[i] == '{') {
                bracketCount++;
            }
            if (bracketCount > 0) {
                sb.Append(jsonString[i]);
            }
            if (jsonString[i] == '}') {
                bracketCount--;
                if (bracketCount == 0) {
                    jsonStrings.Add(sb.ToString());
                    sb.Length = 0;
                }
            }
        }
        return jsonStrings;
    }
}
