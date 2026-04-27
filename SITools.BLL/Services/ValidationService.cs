using System.Text.RegularExpressions;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 输入验证服务（静态类）。
    /// 提供对用户输入数据的格式和合法性校验，包括：
    ///   1. 身份证号码18位校验（省份码、出生日期、校验位）
    ///   2. 费款所属期格式校验（yyyyMM）
    ///   3. 正数校验（用于验证月缴费基数等数值输入）
    ///
    /// 静态类（static class）无需创建实例即可直接调用其中的方法，
    /// 例如：ValidationService.IsValidIdCard("...")
    /// </summary>
    public static class ValidationService
    {
        // ---------------------------------------------------------------
        // 身份证校验位计算所需的两个静态常量
        // ---------------------------------------------------------------

        // 身份证前17位的加权系数（由国家标准 GB 11643-1999 规定）
        // 每个系数对应前17位中一个数字的权重，用于计算加权校验和
        private static readonly int[] _factor = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

        // 校验位对照表：加权和对11取余后，对应的合法校验码
        // 即：余数0→'1'，余数1→'0'，余数2→'X'，余数3→'9'，...
        private static readonly char[] _parity = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

        // 所有合法的省级行政区代码（身份证前2位）
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

        /// <summary>
        /// 验证身份证号码合法性（三重校验）：
        ///   1. 格式校验：符合18位身份证正则表达式
        ///   2. 省份码校验：前2位在合法省级代码列表中
        ///   3. 出生日期校验：第7-14位构成合法的年月日
        ///   4. 校验位校验：第18位由前17位加权求和计算所得
        /// 只有全部通过才返回 true。
        /// </summary>
        /// <param name="val">待验证的身份证号码字符串</param>
        /// <returns>true 表示合法，false 表示不合法</returns>
        public static bool IsValidIdCard(string val)
        {
            // 空值或长度不足直接排除
            if (string.IsNullOrWhiteSpace(val) || val.Length < 18)
                return false;

            // 依次做格式校验、出生日期校验、省份码校验（三者均通过才合法）
            return CheckCode(val) && CheckDate(val.Substring(6, 8)) && CheckProv(val.Substring(0, 2));
        }

        /// <summary>
        /// 校验身份证前2位（省份码）是否合法。
        /// </summary>
        private static bool CheckProv(string val)
        {
            if (int.TryParse(val, out int code))
                return _provinces.ContainsKey(code);
            return false;
        }

        // 出生日期正则：支持 1800-2099 年，月份01-12，日期支持01-31及特殊值10/20/30/31
        private static readonly Regex _datePattern =
            new Regex(@"^(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)$", RegexOptions.Compiled);

        // 整体身份证格式正则（用于初步格式检查）
        private static readonly Regex _codePattern =
            new Regex(@"^[1-9]\d{5}(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$", RegexOptions.Compiled);

        /// <summary>
        /// 校验身份证中出生日期部分（第7-14位，格式 yyyyMMdd）的合法性。
        /// 先用正则做格式粗筛，再用 DateTime.TryParse 做精确日期解析（自动处理月份天数边界）。
        /// </summary>
        private static bool CheckDate(string val)
        {
            if (!_datePattern.IsMatch(val)) return false;
            // 将8位日期字符串拆分为 "yyyy-MM-dd" 格式后尝试解析为 DateTime
            return DateTime.TryParse($"{val.Substring(0, 4)}-{val.Substring(4, 2)}-{val.Substring(6, 2)}", out var date)
                   && date.Month == int.Parse(val.Substring(4, 2));
        }

        /// <summary>
        /// 校验身份证整体格式及第18位校验码的合法性。
        ///
        /// 校验码算法（GB 11643-1999）：
        ///   1. 将前17位每位数字乘以对应权重（_factor），求和
        ///   2. 将求和结果对11取余，得到余数（0-10）
        ///   3. 查对照表（_parity）得到应有的校验码
        ///   4. 与身份证第18位（大写）进行比对，相同则通过
        /// </summary>
        private static bool CheckCode(string val)
        {
            if (!_codePattern.IsMatch(val)) return false;
            int sum = 0;
            // 对前17位逐位计算加权和
            for (int i = 0; i < 17; i++)
                sum += int.Parse(val[i].ToString()) * _factor[i];
            // 查对照表，比较实际第18位（转大写处理'x'='X'）
            return _parity[sum % 11].ToString() == val.Substring(17).ToUpper();
        }

        /// <summary>
        /// 验证费款所属期格式（yyyyMM，如 "202001" 表示2020年1月）。
        /// 要求：必须是6位数字，年份任意，月份01-12。
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>true 表示格式正确</returns>
        public static bool IsValidPeriod(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 6)
                return false;
            // 正则：4位数字年份 + 01-12月份
            return Regex.IsMatch(input, @"^\d{4}(0[1-9]|1[0-2])$");
        }

        /// <summary>
        /// 验证输入是否为正数（包含小数）。
        /// 如果能解析为 double 且大于0，则 value 输出解析结果并返回 true；
        /// 否则 value 为0，返回 false。
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="value">解析结果（仅当返回true时有效）</param>
        /// <returns>true 表示是正数</returns>
        public static bool IsPositiveNumber(string input, out double value)
        {
            value = 0;
            if (double.TryParse(input, out value))
                return value > 0;
            return false;
        }
    }
}
