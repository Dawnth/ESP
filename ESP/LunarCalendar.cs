using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ESP
{
  /*
  * ChineseCalendarGB.java
  * Copyright (c) 1997-2002 by Dr. Herong Yang
  * 中国农历算法- 实用于公历1901 年至2100 年之间的200 年
  */
    public class LunarCalendar
    {
        #region static fileds
        private static int[] DAYS_COUNT_IN_GREGORIAN_MONTH = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static string[] TIANGAN_NAMES = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private static string[] DIZHI_NAMES = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        private static string[] SHENGXIAO_NAMES = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        private static string[] MONTH_NAMES = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
        private static string[] CHINESE_MONTH_NAMES = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊" };
        private static string[] PRINCIPLE_TERM_NAMES = { "大寒", "雨水", "春分", "谷雨", "夏满", "夏至", "大暑", "处暑", "秋分", "霜降", "小雪", "冬至" };
        private static string[] SECTIONDARY_TERM_NAME = { "小寒", "立春", "惊蛰", "清明", "立夏", "芒种", "小暑", "立秋", "白露", "寒露", "立冬", "大雪" };

        // 农历月份大小压缩表，两个字节表示一年。两个字节共十六个二进制位数，
        // 前四个位数表示闰月月份，后十二个位数表示十二个农历月份的大小。
        private static int[] chineseMonths = { 
           0x00,0x04,0xad,0x08,0x5a,0x01,0xd5,0x54,0xb4,0x09,0x64,0x05,0x59,0x45,
           0x95,0x0a,0xa6,0x04,0x55,0x24,0xad,0x08,0x5a,0x62,0xda,0x04,0xb4,0x05,
           0xb4,0x55,0x52,0x0d,0x94,0x0a,0x4a,0x2a,0x56,0x02,0x6d,0x71,0x6d,0x01,
           0xda,0x02,0xd2,0x52,0xa9,0x05,0x49,0x0d,0x2a,0x45,0x2b,0x09,0x56,0x01,
           0xb5,0x20,0x6d,0x01,0x59,0x69,0xd4,0x0a,0xa8,0x05,0xa9,0x56,0xa5,0x04,
           0x2b,0x09,0x9e,0x38,0xb6,0x08,0xec,0x74,0x6c,0x05,0xd4,0x0a,0xe4,0x6a,
           0x52,0x05,0x95,0x0a,0x5a,0x42,0x5b,0x04,0xb6,0x04,0xb4,0x22,0x6a,0x05,
           0x52,0x75,0xc9,0x0a,0x52,0x05,0x35,0x55,0x4d,0x0a,0x5a,0x02,0x5d,0x31,
           0xb5,0x02,0x6a,0x8a,0x68,0x05,0xa9,0x0a,0x8a,0x6a,0x2a,0x05,0x2d,0x09,
           0xaa,0x48,0x5a,0x01,0xb5,0x09,0xb0,0x39,0x64,0x05,0x25,0x75,0x95,0x0a,
           0x96,0x04,0x4d,0x54,0xad,0x04,0xda,0x04,0xd4,0x44,0xb4,0x05,0x54,0x85,
           0x52,0x0d,0x92,0x0a,0x56,0x6a,0x56,0x02,0x6d,0x02,0x6a,0x41,0xda,0x02,
           0xb2,0xa1,0xa9,0x05,0x49,0x0d,0x0a,0x6d,0x2a,0x09,0x56,0x01,0xad,0x50,
           0x6d,0x01,0xd9,0x02,0xd1,0x3a,0xa8,0x05,0x29,0x85,0xa5,0x0c,0x2a,0x09,
           0x96,0x54,0xb6,0x08,0x6c,0x09,0x64,0x45,0xd4,0x0a,0xa4,0x05,0x51,0x25,
           0x95,0x0a,0x2a,0x72,0x5b,0x04,0xb6,0x04,0xac,0x52,0x6a,0x05,0xd2,0x0a,
           0xa2,0x4a,0x4a,0x05,0x55,0x94,0x2d,0x0a,0x5a,0x02,0x75,0x61,0xb5,0x02,
           0x6a,0x03,0x61,0x45,0xa9,0x0a,0x4a,0x05,0x25,0x25,0x2d,0x09,0x9a,0x68,
           0xda,0x08,0xb4,0x09,0xa8,0x59,0x54,0x03,0xa5,0x0a,0x91,0x3a,0x96,0x04,
           0xad,0xb0,0xad,0x04,0xda,0x04,0xf4,0x62,0xb4,0x05,0x54,0x0b,0x44,0x5d,
           0x52,0x0a,0x95,0x04,0x55,0x22,0x6d,0x02,0x5a,0x71,0xda,0x02,0xaa,0x05,
           0xb2,0x55,0x49,0x0b,0x4a,0x0a,0x2d,0x39,0x36,0x01,0x6d,0x80,0x6d,0x01,
           0xd9,0x02,0xe9,0x6a,0xa8,0x05,0x29,0x0b,0x9a,0x4c,0xaa,0x08,0xb6,0x08,
           0xb4,0x38,0x6c,0x09,0x54,0x75,0xd4,0x0a,0xa4,0x05,0x45,0x55,0x95,0x0a,
           0x9a,0x04,0x55,0x44,0xb5,0x04,0x6a,0x82,0x6a,0x05,0xd2,0x0a,0x92,0x6a,
           0x4a,0x05,0x55,0x0a,0x2a,0x4a,0x5a,0x02,0xb5,0x02,0xb2,0x31,0x69,0x03,
           0x31,0x73,0xa9,0x0a,0x4a,0x05,0x2d,0x55,0x2d,0x09,0x5a,0x01,0xd5,0x48,
           0xb4,0x09,0x68,0x89,0x54,0x0b,0xa4,0x0a,0xa5,0x6a,0x95,0x04,0xad,0x08,
           0x6a,0x44,0xda,0x04,0x74,0x05,0xb0,0x25,0x54,0x03
        };
        // 初始日，公历农历对应日期：
        // 公历1901 年1 月1 日，对应农历4598 年11 月11 日
        private static int baseYear = 1901;
        private static int baseMonth = 1;
        private static int baseDate = 1;
        private static int baseIndex = 0;
        private static int baseChineseYear = 4598 - 1;
        private static int baseChineseMonth = 11;
        private static int baseChineseDate = 11;
        private static int[][] sectionalTermMap = new int[][]{
           new int[]{7,6,6,6,6,6,6,6,6,5,6,6,6,5,5,6,6,5,5,5,5,5,5,5,5,4,5,5},  
           new int[]{5,4,5,5,5,4,4,5,5,4,4,4,4,4,4,4,4,3,4,4,4,3,3,4,4,3,3,3},  
           new int[]{6,6,6,7,6,6,6,6,5,6,6,6,5,5,6,6,5,5,5,6,5,5,5,5,4,5,5,5,5},
           new int[]{5,5,6,6,5,5,5,6,5,5,5,5,4,5,5,5,4,4,5,5,4,4,4,5,4,4,4,4,5},
           new int[]{6,6,6,7,6,6,6,6,5,6,6,6,5,5,6,6,5,5,5,6,5,5,5,5,4,5,5,5,5},
           new int[]{6,6,7,7,6,6,6,7,6,6,6,6,5,6,6,6,5,5,6,6,5,5,5,6,5,5,5,5,4,5,5,5,5},
           new int[]{7,8,8,8,7,7,8,8,7,7,7,8,7,7,7,7,6,7,7,7,6,6,7,7,6,6,6,7,7},
           new int[]{8,8,8,9,8,8,8,8,7,8,8,8,7,7,8,8,7,7,7,8,7,7,7,7,6,7,7,7,6,6,7,7,7},
           new int[]{8,8,8,9,8,8,8,8,7,8,8,8,7,7,8,8,7,7,7,8,7,7,7,7,6,7,7,7,7},
           new int[]{9,9,9,9,8,9,9,9,8,8,9,9,8,8,8,9,8,8,8,8,7,8,8,8,7,7,8,8,8},
           new int[]{8,8,8,8,7,8,8,8,7,7,8,8,7,7,7,8,7,7,7,7,6,7,7,7,6,6,7,7,7},
           new int[]{7,8,8,8,7,7,8,8,7,7,7,8,7,7,7,7,6,7,7,7,6,6,7,7,6,6,6,7,7} 
        };
        private static int[][] sectionalTermYear = new int[][]{
           new int[]{13,49,85,117,149,185,201,250,250},
           new int[]{13,45,81,117,149,185,201,250,250},
           new int[]{13,48,84,112,148,184,200,201,250},
           new int[]{13,45,76,108,140,172,200,201,250},
           new int[]{13,44,72,104,132,168,200,201,250},
           new int[]{5 ,33,68,96 ,124,152,188,200,201},
           new int[]{29,57,85,120,148,176,200,201,250},
           new int[]{13,48,76,104,132,168,196,200,201},
           new int[]{25,60,88,120,148,184,200,201,250},
           new int[]{16,44,76,108,144,172,200,201,250},
           new int[]{28,60,92,124,160,192,200,201,250},
           new int[]{17,53,85,124,156,188,200,201,250} 
        };
        private static int[][] principleTermMap = new int[][]{
            new int[]{21,21,21,21,21,20,21,21,21,20,20,21,21,20,20,20,20,20,20,20,20,19,20,20,20,19,19,20},
            new int[]{20,19,19,20,20,19,19,19,19,19,19,19,19,18,19,19,19,18,18,19,19,18,18,18,18,18,18,18},
            new int[]{21,21,21,22,21,21,21,21,20,21,21,21,20,20,21,21,20,20,20,21,20,20,20,20,19,20,20,20,20},
            new int[]{20,21,21,21,20,20,21,21,20,20,20,21,20,20,20,20,19,20,20,20,19,19,20,20,19,19,19,20,20},
            new int[]{21,22,22,22,21,21,22,22,21,21,21,22,21,21,21,21,20,21,21,21,20,20,21,21,20,20,20,21,21},
            new int[]{22,22,22,22,21,22,22,22,21,21,22,22,21,21,21,22,21,21,21,21,20,21,21,21,20,20,21,21,21},
            new int[]{23,23,24,24,23,23,23,24,23,23,23,23,22,23,23,23,22,22,23,23,22,22,22,23,22,22,22,22,23},
            new int[]{23,24,24,24,23,23,24,24,23,23,23,24,23,23,23,23,22,23,23,23,22,22,23,23,22,22,22,23,23},
            new int[]{23,24,24,24,23,23,24,24,23,23,23,24,23,23,23,23,22,23,23,23,22,22,23,23,22,22,22,23,23},
            new int[]{24,24,24,24,23,24,24,24,23,23,24,24,23,23,23,24,23,23,23,23,22,23,23,23,22,22,23,23,23},
            new int[] {23,23,23,23,22,23,23,23,22,22,23,23,22,22,22,23,22,22,22,22,21,22,22,22,21,21,22,22,22},
            new int[] {22,22,23,23,22,22,22,23,22,22,22,22,21,22,22,22,21,21,22,22,21,21,21,22,21,21,21,21,22}
        };
        private static int[][] principleTermYear = new int[][]{
           new int[]{13,45,81,113,149,185,201},    
           new int[]{21,57,93,125,161,193,201},    
           new int[]{21,56,88,120,152,188,200,201},
           new int[]{21,49,81,116,144,176,200,201},
           new int[]{17,49,77,112,140,168,200,201},
           new int[]{28,60,88,116,148,180,200,201},
           new int[]{25,53,84,112,144,172,200,201},
           new int[]{29,57,89,120,148,180,200,201},
           new int[]{17,45,73,108,140,168,200,201},
           new int[]{28,60,92,124,160,192,200,201},
           new int[]{16,44,80,112,148,180,200,201},
           new int[]{17,53,88,120,156,188,200,201} 
        };
        // 大闰月的闰年年份
        private static int[] bigLeapMonthYears = { 6, 14, 19, 25, 33, 36, 38, 41, 44, 52, 55, 79, 117, 136, 147, 150, 155, 158, 185, 193 };
        #endregion

        #region static methods
        public static bool IsGregorianLeapYear(int year)
        {
            bool isLeap = false;
            if (year % 4 == 0) isLeap = true;
            if (year % 100 == 0) isLeap = false;
            if (year % 400 == 0) isLeap = true;
            return isLeap;
        }

        public static int DaysInGregorianMonth(int y, int m)
        {
            int d = DAYS_COUNT_IN_GREGORIAN_MONTH[m - 1];
            if (m == 2 && IsGregorianLeapYear(y)) d++; // 公历闰年二月多一天
            return d;
        }

        public static int DayOfYear(int y, int m, int d)
        {
            int c = 0;
            for (int i = 1; i < m; i++)
            {
                c = c + DaysInGregorianMonth(y, i);
            }
            c = c + d;
            return c;
        }

        public static int DayOfWeek(int y, int m, int d)
        {
            int w = 1; // 公历一年一月一日是星期一，所以起始值为星期日
            y = (y - 1) % 400 + 1; // 公历星期值分部400 年循环一次
            int ly = (y - 1) / 4; // 闰年次数
            ly = ly - (y - 1) / 100;
            ly = ly + (y - 1) / 400;
            int ry = y - 1 - ly; // 常年次数
            w = w + ry; // 常年星期值增一
            w = w + 2 * ly; // 闰年星期值增二
            w = w + DayOfYear(y, m, d);
            w = (w - 1) % 7 + 1;
            return w;
        }

        public static int DaysInChineseMonth(int y, int m)
        {
            // 注意：闰月m < 0
            int index = y - baseChineseYear + baseIndex;
            int v = 0;
            int l = 0;
            int d = 30;
            if (1 <= m && m <= 8)
            {
                v = chineseMonths[2 * index];
                l = m - 1;
                if (((v >> l) & 0x01) == 1) d = 29;
            }
            else if (9 <= m && m <= 12)
            {
                v = chineseMonths[2 * index + 1];
                l = m - 9;
                if (((v >> l) & 0x01) == 1) d = 29;
            }
            else
            {
                v = chineseMonths[2 * index + 1];
                v = (v >> 4) & 0x0F;
                if (v != Math.Abs(m))
                {
                    d = 0;
                }
                else
                {
                    d = 29;
                    for (int i = 0; i < bigLeapMonthYears.Length; i++)
                    {
                        if (bigLeapMonthYears[i] == index)
                        {
                            d = 30;
                            break;
                        }
                    }
                }
            }
            return d;
        }
        public static int NextChineseMonth(int y, int m)
        {
            int n = Math.Abs(m) + 1;
            if (m > 0)
            {
                int index = y - baseChineseYear + baseIndex;
                int v = chineseMonths[2 * index + 1];
                v = (v >> 4) & 0x0F;
                if (v == m) n = -m;
            }
            if (n == 13) n = 1;
            return n;
        }
        public static int SectionalTerm(int y, int m)
        {
            if (y < 1901 || y > 2100) return 0;
            int index = 0;
            int ry = y - baseYear + 1;
            while (ry >= sectionalTermYear[m - 1][index]) index++;
            int term = sectionalTermMap[m - 1][4 * index + ry % 4];
            if ((ry == 121) && (m == 4)) term = 5;
            if ((ry == 132) && (m == 4)) term = 5;
            if ((ry == 194) && (m == 6)) term = 6;
            return term;
        }
        public static int PrincipleTerm(int y, int m)
        {
            if (y < 1901 || y > 2100) return 0;
            int index = 0;
            int ry = y - baseYear + 1;
            while (ry >= principleTermYear[m - 1][index]) index++;
            int term = principleTermMap[m - 1][4 * index + ry % 4];
            if ((ry == 171) && (m == 3)) term = 21;
            if ((ry == 181) && (m == 5)) term = 21;
            return term;
        }

        public static string GetTextLine(int s, string t)
        {
            string str = "                                         "
                    + "  " + "                                         ";
            if (t != null && s < str.Length && s + t.Length < str.Length)
                str = str.Substring(0, s) + t + str.Substring(s + t.Length);
            return str;
        }
        #endregion

        private static int gregorianYear = 1901;
        private static int gregorianMonth = 1;
        private static int gregorianDate = 1;
        private static bool isGregorianLeap;
        private static int dayOfYear;

        /**//// <summary>
        /// 周日------- 一星期的第一天
        /// </summary>
        private static int dayOfWeek;
        private static int chineseYear;

        /**//// <summary>
        /// 负数表示闰月
        /// </summary>
        private static int chineseMonth;

        private static int chineseDate;

        //24节气
        private static int sectionalTerm;
        private static int principleTerm;

        public LunarCalendar()
        {
            SetGregorian(1901, 1, 1);
        }

        public static void SetGregorian(int y, int m, int d)
        {
            gregorianYear = y;
            gregorianMonth = m;
            gregorianDate = d;
            isGregorianLeap = IsGregorianLeapYear(y);
            dayOfYear = DayOfYear(y, m, d);
            dayOfWeek = DayOfWeek(y, m, d);
            chineseYear = 0;
            chineseMonth = 0;
            chineseDate = 0;
            sectionalTerm = 0;
            principleTerm = 0;
        }


        /**//// <summary>
        /// 根据设定的公历（阳历）年月人计算农历年月日天干地支
        /// </summary>
        /// <returns>是否得到结果，得到结果为0，否则为1</returns>
        public static int ComputeChineseFields()
        {
            if (gregorianYear < 1901 || gregorianYear > 2100) return 1;
            int startYear = baseYear;
            int startMonth = baseMonth;
            int startDate = baseDate;
            chineseYear = baseChineseYear;
            chineseMonth = baseChineseMonth;
            chineseDate = baseChineseDate;
            // 第二个对应日，用以提高计算效率
            // 公历2000 年1 月1 日，对应农历4697 年11 月25 日
            if (gregorianYear >= 2000)
            {
                startYear = baseYear + 99;
                startMonth = 1;
                startDate = 1;
                chineseYear = baseChineseYear + 99;
                chineseMonth = 11;
                chineseDate = 25;
            }
            int daysDiff = 0;
            for (int i = startYear; i < gregorianYear; i++)
            {
                daysDiff += 365;
                if (IsGregorianLeapYear(i)) daysDiff += 1; // leap year
            }
            for (int i = startMonth; i < gregorianMonth; i++)
            {
                daysDiff += DaysInGregorianMonth(gregorianYear, i);
            }
            daysDiff += gregorianDate - startDate;

            chineseDate += daysDiff;
            int lastDate = DaysInChineseMonth(chineseYear, chineseMonth);
            int nextMonth = NextChineseMonth(chineseYear, chineseMonth);
            while (chineseDate > lastDate)
            {
                if (Math.Abs(nextMonth) < Math.Abs(chineseMonth)) chineseYear++;
                chineseMonth = nextMonth;
                chineseDate -= lastDate;
                lastDate = DaysInChineseMonth(chineseYear, chineseMonth);
                nextMonth = NextChineseMonth(chineseYear, chineseMonth);
            }
            return 0;
        }


        /**//// <summary>
        /// 计算24节气
        /// </summary>
        /// <returns></returns>
        public static int ComputeSolarTerms()
        {
            if (gregorianYear < 1901 || gregorianYear > 2100) return 1;
            sectionalTerm = SectionalTerm(gregorianYear, gregorianMonth);
            principleTerm = PrincipleTerm(gregorianYear, gregorianMonth);
            return 0;
        }

        //public override string ToString()
        public static string myString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("Gregorian Year: " + gregorianYear + "\r\n");
            buf.Append("Gregorian Month: " + gregorianMonth + "\r\n");
            buf.Append("Gregorian Date: " + gregorianDate + "\r\n");
            buf.Append("Is Leap Year: " + isGregorianLeap + "\r\n");
            buf.Append("Day of Year: " + dayOfYear + "\r\n");
            buf.Append("Day of Week: " + dayOfWeek + "\r\n");
            buf.Append("Chinese Year: " + chineseYear + "\r\n");
            buf.Append("Heavenly Stem: " + TIANGAN_NAMES[((chineseYear - 1) % 10)] + "\r\n");
            buf.Append("Earthly Branch: " + DIZHI_NAMES[((chineseYear - 1) % 12)] + "\r\n");
            buf.Append("Chinese Month: " + chineseMonth + "\r\n");
            buf.Append("Chinese Date: " + chineseDate + "\r\n");
            buf.Append("Sectional Term: " + sectionalTerm + "\r\n");
            buf.Append("Principle Term: " + principleTerm + "\r\n");
            return buf.ToString();
        }


        /**//// <summary>
        /// 计算指定日期的明天的农历
        /// </summary>
        public void RollUpOneDay()
        {
            dayOfWeek = dayOfWeek % 7 + 1;
            dayOfYear++;
            gregorianDate++;
            int days = DaysInGregorianMonth(gregorianYear, gregorianMonth);
            if (gregorianDate > days)
            {
                gregorianDate = 1;
                gregorianMonth++;
                if (gregorianMonth > 12)
                {
                    gregorianMonth = 1;
                    gregorianYear++;
                    dayOfYear = 1;
                    isGregorianLeap = IsGregorianLeapYear(gregorianYear);
                }
                sectionalTerm = SectionalTerm(gregorianYear, gregorianMonth);
                principleTerm = PrincipleTerm(gregorianYear, gregorianMonth);
            }
            chineseDate++;
            days = DaysInChineseMonth(chineseYear, chineseMonth);
            if (chineseDate > days)
            {
                chineseDate = 1;
                chineseMonth = NextChineseMonth(chineseYear, chineseMonth);
                if (chineseMonth == 1) chineseYear++;
            }
        }

        public static void mylunar(System.Windows.Forms.TextBox tx)
        {
            tx.AppendText(myString());
        }
    }
}

