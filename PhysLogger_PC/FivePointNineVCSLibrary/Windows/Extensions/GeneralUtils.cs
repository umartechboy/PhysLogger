using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FivePointNine.Numbers
{
    public class NumberUtils
    {
        internal static string ExtractValueFromPrefixedValue(string value)
        {
            value = cleanNum(value);
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] < '-' || value[i] == '/' || value[i] > '9')
                    return value.Substring(0, i);
            }
            return value;
        }
        internal static string cleanNum(string value)
        {
            return value.Replace(" ", "").Replace("\t", "");
        }
        internal static string ExtractPrefixAndUnitFromValue(string value)
        {
            value = cleanNum(value);
            return value.Substring(ExtractValueFromPrefixedValue(value).Length);
        }
        internal static Prefix GetPrefixFromUnits(string pref)
        {
            for (int i = 24; i >= -24;)
            {
                Prefix p = (Prefix)i;
                string shortForm = p.ToString()[0].ToString();
                if (p == Prefix.micro)
                    shortForm = "u";
                if (p == Prefix.deca)
                    shortForm = "da";
                if (pref.StartsWith(shortForm))
                    return p;

                if (i > -3 && i <= 3)
                    i--;
                else
                    i -= 3;

                if (i == 0)
                    i--;
            }
            return Prefix.one;
        }
        internal static string ExtractUnitsFromPrefix(string pref)
        {
            pref = cleanNum(pref);
            for (int i = 24; i >= -24;)
            {
                Prefix p = (Prefix)i;
                string shortForm = p.ToString()[0].ToString();
                if (p == Prefix.deca)
                    shortForm = "da";
                if (p == Prefix.micro)
                    shortForm = "u";
                if (pref.StartsWith(p.ToString()))
                    return pref.Substring(p.ToString().Length);
                else if (pref.StartsWith(shortForm))
                    return pref.Substring(shortForm.Length);
                if (i > -3 && i <= 3)
                    i--;
                else
                    i -= 3;

                if (i == 0)
                    i--;
            }
            return pref;
        }
       
        /// <summary>
        /// Selects a suitable prefix for the value to keep 1-3 significant figures on the left of the decimal point.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit">Optional unit to be put after the prefix</param>
        /// <returns></returns>
        public static string AddPrefix(double value, string unit = "", int padding = 0)
        {
            string[] superPrefix = new string[] { "K", "M", "G", "T", "P", "A", };
            string[] subPrefix = new string[] { "m", "u", "n", "p", "f", "a" };
            double v = Math.Abs(value);
            bool isNeg = value < 0;
            int exp = 0;
            while (v - Math.Floor(v) > 0)
            {
                if (exp >= 18)
                    break;
                exp += 3;
                v *= 1000;
                v = Math.Round(v, 12);
            }


            while (Math.Floor(v).ToString().Length > 3)
            {
                if (exp <= -18)
                    break;
                exp -= 3;
                v /= 1000;
                v = Math.Round(v, 12);
            }
            var vStr = v.ToString();
            if (padding > 0)
            {
                var floor = Math.Floor(v).ToString();
                var decimals = (v - Math.Floor(v)).ToString();
                if (decimals.Contains("."))
                    decimals = decimals.Substring(decimals.IndexOf(".") + 1);
                decimals = decimals.PadRight(padding, '0');
                vStr = floor + "." + decimals;

            }
            if (exp > 0)
                return (isNeg ? "-" : "") + vStr + subPrefix[exp / 3 - 1] + unit;
            else if (exp < 0)
                return (isNeg ? "-" : "") + vStr + superPrefix[-exp / 3 - 1] + unit;
            return (isNeg ? "-" : "") + vStr + unit;
        }
    }
    public class PrefixedValue
    {
        public PrefixedValue MatchPrefix(PrefixedValue reference)
        {
            var ans = Clone();
            ans.Prefix = reference.Prefix;
            ans.D = ans.Value;

            return ans;
        }
        public PrefixedValue Clone()
        {
            return new PrefixedValue(Value, Prefix, Units);
        }
        public PrefixedValue(double value = 0, Prefix prefix = Prefix.one, string unit = "")
        {
            Value = value;
            Prefix = prefix;
            Units = unit;
        }
        public double Value { get; set; } = 0;
        public Prefix Prefix { get; set; } = Prefix.one;
        public double D
        {
            get { return Value * Math.Pow(10, (int)Prefix); }
            set { double v = value; Value = v / Math.Pow(10, (int)Prefix); }
        }
        public string Units { get; set; } = "";
        public static implicit operator PrefixedValue(double num)
        {
            return new PrefixedValue(num);
        }
        public static implicit operator PrefixedValue(string str)
        {
            try
            {
                return new PrefixedValue(
                    Convert.ToDouble(NumberUtils.cleanNum(NumberUtils.ExtractValueFromPrefixedValue(str))),
                    NumberUtils.GetPrefixFromUnits(NumberUtils.ExtractPrefixAndUnitFromValue(str)),
                    NumberUtils.ExtractUnitsFromPrefix(NumberUtils.ExtractPrefixAndUnitFromValue(str))
                    );

            }
            catch (Exception ex) { throw new Exception("The value could not be parsed."); }
        }
        public static implicit operator double (PrefixedValue value)
        {
            return value.D;
        }
        public static implicit operator float(PrefixedValue value)
        {
            return (float)value.D;
        }

        public string ToString(int round)
        {
            string p = (Prefix != Prefix.one ? Prefix.ToString() : " ")[0].ToString();
            if (p == "d")
                if (Prefix == Prefix.deca)
                    p = "da";
            if (p == "m")
                if (Prefix == Prefix.micro)
                    p = "u";
            if (p == " ") p = "";
            if (round < 0)
                return Value.ToString() + p + Units;
            else
                return Math.Round(Value, round).ToString() + p + Units;
        }
        public override string ToString()
        {
            return ToString(-1);
        }
    }
    public enum Prefix : int
    {
        yocto = -24,
        zepto = -21,
        atto = -18,
        femto = -15,
        pico = -12,
        nano = -9,
        micro = -6,
        milli = -3,
        centi = -2,
        deci = -1,
        one = 0,
        deca = 1,
        hecto = 2,
        kilo = 3,
        Mega = 6,
        Giga = 9,
        Tera = 12,
        Peta = 15,
        Exa = 18,
        Zeta = 21,
        Yocta = 24
    }
}