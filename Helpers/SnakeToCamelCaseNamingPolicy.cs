using System.Text.Json;

namespace KcetasWeb.Helpers
{
    public class SnakeToCamelCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            
            var parts = name.Split('_');
            if (parts.Length == 1) return name;

            var camelCase = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    camelCase += char.ToUpper(parts[i][0]) + parts[i].Substring(1);
                }
            }
            return camelCase;
        }
    }
}
