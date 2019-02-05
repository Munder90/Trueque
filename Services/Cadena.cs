using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TAT001.Services
{
    public class Cadena
    {
        public string completaMaterial(string material)//RSG 07.06.2018---------------------------------------------
        {
            string m = material;
            try
            {
                long matnr1 = long.Parse(m);
                if (matnr1 != 0)
                {
                    int l = 18 - m.Length;
                    for (int i = 0; i < l; i++)
                    {
                        m = "0" + m;
                    }
                }
            }
            catch
            {
                return m;
            }
            return m;
        }
        public string completaCliente(string cliente)//RSG 07.06.2018---------------------------------------------
        {
            string m = cliente;
            try
            {
                long matnr1 = long.Parse(m);
                if (matnr1 != 0)
                {
                    int l = 10 - m.Length;
                    for (int i = 0; i < l; i++)
                    {
                        m = "0" + m;
                    }
                }
            }
            catch
            {
                return m;
            }
            return m;
        }
        public string concatena(string cade, string entry)
        {
            StringBuilder cadena = new StringBuilder();
            cadena.Insert(0, cade);
            cadena.Insert(cadena.Length, entry);

            return cadena.ToString();
        }
    }
}