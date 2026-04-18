using System.Text.RegularExpressions;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 输入验证服务
    /// </summary>
    public static class ValidationService
    {
        private static readonly int[] _factor = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        private static readonly char[] _parity = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

        private static readonly Dictionary<int, string> _provinces = new()
        {
            {11,"北京"},{12,"天津"},{13,"河北"},{14,"山西"},{15,"内蒙古"},
            {21,"辽宁"},{22,"吉林"},{23,"黑龙江"},{31,"上海"},{32,"江苏"},
            {33,"浙江"},{34,"安徽"},{35,"福建"},{36,"江西"},{37,"山东"},
            {41,"河南"},{42,"湖北"},{43,"湖南"},{44,"广东"},{45,"广西"},
            {46,"海南"},{50,"重庆"},{51,"四川"},{52,"贵州"},{53,"云南"},
            {54,"西藏"},{61,"陕西"},{62,"甘肃"},{63,"青海"},{64,"宁夏"},
            {65,"新疆"},{71,"台湾"},{81,"香港"},{82,"澳门"}
        };

        /// <summary>验证身份证号码合法性</summary>
        public static bool IsValidIdCard(string val)
        {
            if (string.IsNullOrWhiteSpace(val) || val.Length < 18)
                return false;
            return CheckCode(val) && CheckDate(val.Substring(6, 8)) && CheckProv(val.Substring(0, 2));
        }

        private static bool CheckProv(string val)
        {
            if (int.TryParse(val, out int code))
                return _provinces.ContainsKey(code);
            return false;
        }

        private static readonly Regex _datePattern =
            new Regex(@"^(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)$", RegexOptions.Compiled);

        private static readonly Regex _codePattern =
            new Regex(@"^[1-9]\d{5}(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$", RegexOptions.Compiled);

        private static bool CheckDate(string val)
        {
            if (!_datePattern.IsMatch(val)) return false;
            return DateTime.TryParse($"{val.Substring(0, 4)}-{val.Substring(4, 2)}-{val.Substring(6, 2)}", out var date)
                   && date.Month == int.Parse(val.Substring(4, 2));
        }

        private static bool CheckCode(string val)
        {
            if (!_codePattern.IsMatch(val)) return false;
            int sum = 0;
            for (int i = 0; i < 17; i++)
                sum += int.Parse(val[i].ToString()) * _factor[i];
            return _parity[sum % 11].ToString() == val.Substring(17).ToUpper();
        }

        /// <summary>验证费款所属期格式（yyyyMM）</summary>
        public static bool IsValidPeriod(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 6)
                return false;
            return Regex.IsMatch(input, @"^\d{4}(0[1-9]|1[0-2])$");
        }

        /// <summary>验证是否为正数（含小数）</summary>
        public static bool IsPositiveNumber(string input, out double value)
        {
            value = 0;
            if (double.TryParse(input, out value))
                return value > 0;
            return false;
        }
    }
}
