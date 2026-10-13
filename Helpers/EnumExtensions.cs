using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MusteriYonetim.Helpers
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
                if (memberInfo.Length > 0)
                {
                    var displayAttribute = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
                    if (displayAttribute != null)
                    {
                        return displayAttribute.Name ?? enumValue.ToString();
                    }
                }
            }
            catch
            {
                // Enum tanımlanmamış bir tam sayı değerine sahipse ToString çalışabilir ancak yansıma patlayabilir.
            }
            
            return enumValue.ToString();
        }
    }
}
