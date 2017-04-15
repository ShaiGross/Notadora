using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringUtils
{
    public static class DiacriticsExtensions
    {
        private const string SPANISH_CULTULRE_NAME = "es-ES";
        private static readonly CompareInfo CompareInfo = CompareInfo.GetCompareInfo(SPANISH_CULTULRE_NAME);

        public static bool DiacriticsEndsWith(this string src, string suffix)
        {
            return CompareInfo.IsSuffix(src, suffix, CompareOptions.IgnoreNonSpace);
        }

        public static int DiacriticsLastIndexOf(this string str, string substr)
        {
            return CompareInfo.LastIndexOf(str, substr, CompareOptions.IgnoreNonSpace);
        }

        public static int DiacriticsCompareTo(this string str, string substr)
        {
            return CompareInfo.Compare(str, substr, CompareOptions.IgnoreNonSpace);
        }        
    }
}
