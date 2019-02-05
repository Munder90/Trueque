using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Services
{
    public class FormatosC
    {
        public decimal toNum(string numR, string miles, string decimales)
        {
            if (numR == "-") numR = "0";
            string num = numR;
            int _i = 1;
            if (num.Contains("("))
                _i = -1;
            if (num != "" && num != null)
            {
                num = num.Replace("$", "");
                num = num.Replace("%", "");
                num = num.Replace("(", "");
                num = num.Replace(")", "");
                num = num.Replace(miles, "");
                num = num.Replace(decimales, ".");
            }
            else
            {
                num = "0.00";
            }

            return Convert.ToDecimal(num)*_i;
        }

        public string toShow(decimal num, string decimales)
        {
            int posi = 1;
            if (num < 0)
                posi = -1;
            num = num * posi;
            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');


            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = regresa + decimales + separa[1];
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = regresa + decimales + separa[1];
                }
            }
            else
            {
                regresa = "0" + decimales + "00";
            }
            if (posi == -1)
                regresa = "$(" + regresa + ")";
            else
                regresa = "$" + regresa + "";

            return regresa;
        }

        public string toShowG(decimal num, string decimales)
        {
            int posi = 1;
            if (num < 0)
                posi = -1;
            num = num * posi;
            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');


            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = regresa + decimales + separa[1];
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = regresa + decimales + separa[1];
                }
            }
            else
            {
                regresa = "0" + decimales + "00";
            }
            if (regresa == "0" + decimales + "00")
            {
                regresa = "( - )";
            }
            if (posi == -1)
                regresa = "$(" + regresa + ")";
            else
                regresa = "$" + regresa + "";

            return regresa;
        }

        public string toShowPorc(decimal num, string decimales)
        {
            int posi = 1;
            if (num < 0)
                posi = -1;
            num = num * posi;
            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');

            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = regresa + decimales + separa[1];
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = regresa + decimales + separa[1] ;
                }
            }
            else
            {
                regresa = "0" + decimales + "00";
            }
            if (posi == -1)
                regresa = "(" + regresa + ")" + "%";
            else
                regresa = "" + regresa + "%";


            return regresa;
        }

        public string toShowNum(decimal num, string decimales)
        {
            int posi = 1;
            if (num < 0)
                posi = -1;
            num = num * posi;
            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');

            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = regresa + decimales + separa[1];
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = regresa + decimales + separa[1];
                }
            }
            else
            {
                regresa = "0" + decimales + "00";
            }
            if (posi == -1)
                regresa = "(" + regresa + ")" + "";
            else
                regresa = "" + regresa + "";

            return regresa;
        }
    }
}
