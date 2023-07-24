using System.Text;

public static class ResourceName
{
    // Generates the correct sprite name for a given card name
    public static string GetSpriteName(string cardName) {
        string[] words = cardName.Trim().Replace("'", "").Split(' ');
        if (words.Length == 0) {
            return "";
        }
        StringBuilder output = new StringBuilder();
        foreach(string word in words) {
            if (word.Length == 0) {
                continue;
            }
            string outputWord;
            if (word.Length == 1) {
                outputWord = word[0].ToString().ToUpper();
            } else {
                outputWord = word[0].ToString().ToUpper() + word.Substring(1).ToLower();
            }
            output.Append(outputWord);
        }
        output.Append("Sprite");
        return output.ToString();
    }
}
