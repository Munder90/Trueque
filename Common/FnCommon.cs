using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Services;

namespace TAT001.Common
{
    public static class FnCommon
    {
        public static USUARIO ObtenerUsuario(TruequeEntities db, string user_id)
        {
            return db.USUARIOs.Where(a => a.ID.Equals(user_id)).FirstOrDefault();
        }
        public static string ObtenerSprasId(TruequeEntities db, string user_id)
        {
            return db.USUARIOs.Where(a => a.ID.Equals(user_id)).FirstOrDefault().SPRAS_ID;
        }
        public static string ObtenerTextoMnj(TruequeEntities db, int pagina_id, string campo_id, string user_id)
        {
            string spras_id = ObtenerSprasId(db, user_id);
            string texto = "";
            if (db.TEXTOes.Any(a => (a.PAGINA_ID.Equals(pagina_id) && a.SPRAS_ID.Equals(spras_id) && a.CAMPO_ID.Equals(campo_id))))
            {
                texto = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina_id) && a.SPRAS_ID.Equals(spras_id) && a.CAMPO_ID.Equals(campo_id))).FirstOrDefault().TEXTOS;
            }
            return texto;
        }
        public static void ObtenerTextos(TruequeEntities db, int pagina_id_textos, string user_id, ControllerBase controller)
        {
            var user = ObtenerUsuario(db, user_id);
            controller.ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina_id_textos) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

        }
        public static void ObtenerConfPage(TruequeEntities db, int pagina_id, string user_id, ControllerBase controller, int? pagina_id_textos = null)
        {
            var user = ObtenerUsuario(db, user_id);
            controller.ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            controller.ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            controller.ViewBag.usuario = user;
            controller.ViewBag.rol = user.PUESTO.PUESTOTs.FirstOrDefault(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).TXT50;
            controller.ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina_id)).FirstOrDefault().PAGINATs.FirstOrDefault(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).TXT50;
            if (pagina_id_textos != null)
            {
                pagina_id = pagina_id_textos.Value;
            }
            controller.ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina_id) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            controller.ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina_id) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.ACTIVO == true).ToList();
            controller.ViewBag.spras_id = user.SPRAS_ID;
        }




        public static int ObtenerPeriodoCalendario445(TruequeEntities db, string sociedad_id, string tsol_id, string usuario_id = null)
        {
            //tipo
            //PRE = PreCierre
            //CI =  Cierre
            DateTime fechaActual = DateTime.Now;
            short ejercicio = short.Parse(fechaActual.Year.ToString());
            CALENDARIO_AC calendarioAc = null;
            CALENDARIO_EX calendarioEx = null;

            calendarioAc = db.CALENDARIO_AC.FirstOrDefault(x =>
            x.ACTIVO &&
            x.SOCIEDAD_ID == sociedad_id &&
            x.TSOL_ID == tsol_id &&
            x.EJERCICIO == ejercicio &&
            (fechaActual >= DbFunctions.CreateDateTime(x.PRE_FROMF.Year, x.PRE_FROMF.Month, x.PRE_FROMF.Day, x.PRE_FROMH.Hours, x.PRE_FROMH.Minutes, x.PRE_FROMH.Seconds) &&
             fechaActual <= DbFunctions.CreateDateTime(x.PRE_TOF.Year, x.PRE_TOF.Month, x.PRE_TOF.Day, x.PRE_TOH.Hours, x.PRE_TOH.Minutes, x.PRE_TOH.Seconds)));
            if (calendarioAc != null)
            {
                return calendarioAc.PERIODO;
            }
            if (usuario_id != null)
            {
                calendarioEx = db.CALENDARIO_EX.FirstOrDefault(x =>
                    x.ACTIVO &&
                    x.SOCIEDAD_ID == sociedad_id &&
                    x.TSOL_ID == tsol_id &&
                    x.USUARIO_ID == usuario_id &&
                    x.EJERCICIO == ejercicio &&
                    (fechaActual >= DbFunctions.CreateDateTime(x.EX_FROMF.Year, x.EX_FROMF.Month, x.EX_FROMF.Day, x.EX_FROMH.Hours, x.EX_FROMH.Minutes, x.EX_FROMH.Seconds) &&
                    fechaActual <= DbFunctions.CreateDateTime(x.EX_TOF.Year, x.EX_TOF.Month, x.EX_TOF.Day, x.EX_TOH.Hours, x.EX_TOH.Minutes, x.EX_TOH.Seconds)));
                if (calendarioEx != null)
                {
                    return calendarioEx.PERIODO;
                }
            }

            calendarioAc = db.CALENDARIO_AC.FirstOrDefault(x =>
            x.ACTIVO &&
            x.SOCIEDAD_ID == sociedad_id &&
            x.TSOL_ID == tsol_id &&
            x.EJERCICIO == ejercicio &&
            (fechaActual >= DbFunctions.CreateDateTime(x.CIE_FROMF.Year, x.CIE_FROMF.Month, x.CIE_FROMF.Day, x.CIE_FROMH.Hours, x.CIE_FROMH.Minutes, x.CIE_FROMH.Seconds) &&
            fechaActual <= DbFunctions.CreateDateTime(x.CIE_TOF.Year, x.CIE_TOF.Month, x.CIE_TOF.Day, x.CIE_TOH.Hours, x.CIE_TOH.Minutes, x.CIE_TOH.Seconds)));
            if (calendarioAc != null)
            {
                return calendarioAc.PERIODO;
            }

            return 0;
        }
        public static bool ValidarPeriodoEnCalendario445(TruequeEntities db, string sociedad_id, string tsol_id, int periodo_id, string tipo, string usuario_id = null)
        {
            //tipo
            //PRE = PreCierre
            //CI =  Cierre
            bool esPeriodoAbierto = false;
            DateTime fechaActual = DateTime.Now;
            short ejercicio = short.Parse(fechaActual.Year.ToString());

            switch (tipo)
            {
                case "PRE":
                    esPeriodoAbierto = db.CALENDARIO_AC.Any(x =>
                    x.ACTIVO &&
                    x.SOCIEDAD_ID == sociedad_id &&
                    x.TSOL_ID == tsol_id &&
                    x.PERIODO == periodo_id &&
                    x.EJERCICIO == ejercicio &&
                    (fechaActual >= DbFunctions.CreateDateTime(x.PRE_FROMF.Year, x.PRE_FROMF.Month, x.PRE_FROMF.Day, x.PRE_FROMH.Hours, x.PRE_FROMH.Minutes, x.PRE_FROMH.Seconds) &&
                     fechaActual <= DbFunctions.CreateDateTime(x.PRE_TOF.Year, x.PRE_TOF.Month, x.PRE_TOF.Day, x.PRE_TOH.Hours, x.PRE_TOH.Minutes, x.PRE_TOH.Seconds)));
                    if (!esPeriodoAbierto && usuario_id != null)
                    {
                        esPeriodoAbierto = db.CALENDARIO_EX.Any(x =>
                          x.ACTIVO &&
                          x.SOCIEDAD_ID == sociedad_id &&
                          x.TSOL_ID == tsol_id &&
                          x.PERIODO == periodo_id &&
                          x.USUARIO_ID == usuario_id &&
                          x.EJERCICIO == ejercicio &&
                          (fechaActual >= DbFunctions.CreateDateTime(x.EX_FROMF.Year, x.EX_FROMF.Month, x.EX_FROMF.Day, x.EX_FROMH.Hours, x.EX_FROMH.Minutes, x.EX_FROMH.Seconds) &&
                          fechaActual <= DbFunctions.CreateDateTime(x.EX_TOF.Year, x.EX_TOF.Month, x.EX_TOF.Day, x.EX_TOH.Hours, x.EX_TOH.Minutes, x.EX_TOH.Seconds)));
                    }
                    break;
                case "CI":
                    esPeriodoAbierto = db.CALENDARIO_AC.Any(x =>
                        x.ACTIVO &&
                        x.SOCIEDAD_ID == sociedad_id &&
                        x.TSOL_ID == tsol_id &&
                        x.PERIODO == periodo_id &&
                        x.EJERCICIO == ejercicio &&
                        (fechaActual >= DbFunctions.CreateDateTime(x.CIE_FROMF.Year, x.CIE_FROMF.Month, x.CIE_FROMF.Day, x.CIE_FROMH.Hours, x.CIE_FROMH.Minutes, x.CIE_FROMH.Seconds) &&
                        fechaActual <= DbFunctions.CreateDateTime(x.CIE_TOF.Year, x.CIE_TOF.Month, x.CIE_TOF.Day, x.CIE_TOH.Hours, x.CIE_TOH.Minutes, x.CIE_TOH.Seconds)));

                    break;
                default:
                    break;
            }
            return esPeriodoAbierto;
        }
        public static List<SelectListItem> ObtenerCmbEjercicio()
        {
            DateTime fechaActual = DateTime.Now;
            int anio = fechaActual.Year;
            return new List<SelectListItem> {
                    new SelectListItem{Text=(anio).ToString(),Value=(anio).ToString(), Selected=true},
                    new SelectListItem{Text=(anio+1).ToString(),Value=(anio+1).ToString()},
            };
        }
        public static List<SelectListItem> ObtenerCmbPageSize()
        {
            return new List<SelectListItem> {
                    new SelectListItem{Text="10",Value="10"},
                    new SelectListItem{Text="25",Value="25"},
                    new SelectListItem{Text="50",Value="50"},
                    new SelectListItem{Text="100",Value="100"}
            };
        }


        public static MATERIAL ObtenerMaterial(TruequeEntities db, string user_id, string material_id)
        {
            string spras_id = ObtenerSprasId(db, user_id);
            MATERIAL material = db.MATERIALs.Where(x => x.ID == material_id).FirstOrDefault();

            if (material.MATERIALTs.Any(x => x.SPRAS == spras_id))
            {
                MATERIALT mt = material.MATERIALTs.First(x => x.SPRAS == spras_id);
                material.MAKTX = mt.MAKTX;
                material.MAKTG = mt.MAKTG;
            }
            return material;


        }
        public static List<MATERIALGP> ObtenerMaterialGroups(TruequeEntities db)
        {
            return db.MATERIALGPs.Where(a => a.ACTIVO).ToList();
        }
        public static MATERIALGP ObtenerMaterialGroup(TruequeEntities db, string materialgp_id)
        {
            return db.MATERIALGPs.Where(x => x.ID == materialgp_id).FirstOrDefault();
        }
        public static MATERIALGPT ObtenerTotalProducts(TruequeEntities db)
        {
            return db.MATERIALGPTs.Where(x => x.MATERIALGP_ID == "000" && x.SPRAS_ID == "EN").FirstOrDefault();
        }




        //public static List<CSP_PRESU_CLIENT_Result> ObtenerPresupuestoCliente(TruequeEntities db, string kunnr, string periodo)
        //{
        //    List<CSP_PRESU_CLIENT_Result> presupuesto = db.Database.SqlQuery<CSP_PRESU_CLIENT_Result>("CSP_PRESU_CLIENT @CLIENTE, @PERIODO",
        //    new SqlParameter("@CLIENTE", kunnr),
        //    new SqlParameter("@PERIODO", periodo)).ToList();
        //    return presupuesto;
        //}




        public static List<DOCUMENTOP_SP> ObtenerDocumentoP(TruequeEntities db, string spras_id, decimal num_doc, DateTime vigencia_de, DateTime vigencia_al)
        {
            List<DOCUMENTOP_SP> documentop = db.Database.SqlQuery<DOCUMENTOP_SP>("CPS_LISTA_DOCUMENTOP @NUM_DOC,@SPRAS_ID,@VIGENCIA_DE,@VIGENCIA_AL",
                new SqlParameter("@NUM_DOC", num_doc),
                new SqlParameter("@SPRAS_ID", spras_id),
                new SqlParameter("@VIGENCIA_DE", vigencia_de),
                new SqlParameter("@VIGENCIA_AL", vigencia_al)).ToList();

            return documentop;
        }


        public static List<SelectListItem> ObtenerCmbNivelesA()
        {
            return new List<SelectListItem> {
                    new SelectListItem{Text="Nivel 1",Value="1"},
                    new SelectListItem{Text="Nivel 2",Value="2"},
                    new SelectListItem{Text="Nivel 3",Value="3"},
                    new SelectListItem{Text="Nivel 4",Value="4"},
                    new SelectListItem{Text="Nivel 5",Value="5"}
            };
        }
        public static List<SelectListItem> ObtenerCmbNivel()
        {
            return new List<SelectListItem> {
                    //new SelectListItem{Text="1",Value="1"},
                    new SelectListItem{Text="2",Value="2"},
                    new SelectListItem{Text="3",Value="3"},
                    new SelectListItem{Text="4",Value="4"},
                    new SelectListItem{Text="5",Value="5"}
            };
        }
        public static List<SelectListItem> ObtenerCmbFrecuencia(string SPRAS_ID)
        {
            if (SPRAS_ID == "ES")
                return new List<SelectListItem> {
                    new SelectListItem{Text="Semana(s)",Value="S"},
                    new SelectListItem{Text="Mes(es)",Value="M"}
            };
            else
                return new List<SelectListItem> {
                    new SelectListItem{Text="Week(s)",Value="S"},
                    new SelectListItem{Text="Month(s)",Value="M"}
            };
        }
        public static List<SelectListItem> ObtenerCmbDias(string SPRAS_ID)
        {
            if (SPRAS_ID == "ES")
                return new List<SelectListItem> {
                    new SelectListItem{Text="Domingo",Value="D"},
                    new SelectListItem{Text="Lunes",Value="L"},
                    new SelectListItem{Text="Martes",Value="M"},
                    new SelectListItem{Text="Miércoles",Value="X"},
                    new SelectListItem{Text="Jueves",Value="J"},
                    new SelectListItem{Text="Viernes",Value="V"},
                    new SelectListItem{Text="Sábado",Value="S"}
            };
            else
                return new List<SelectListItem> {
                    new SelectListItem{Text="Sunday",Value="D"},
                    new SelectListItem{Text="Monday",Value="L"},
                    new SelectListItem{Text="Tuesday",Value="M"},
                    new SelectListItem{Text="Wednesday",Value="X"},
                    new SelectListItem{Text="Thursday",Value="J"},
                    new SelectListItem{Text="Friday",Value="V"},
                    new SelectListItem{Text="Saturday",Value="S"}
            };
        }
        public static List<SelectListItem> ObtenerCmbOrdinales(string SPRAS_ID)
        {
            if (SPRAS_ID == "ES")
                return new List<SelectListItem> {
                    new SelectListItem{Text="Primer",Value="1"},
                    new SelectListItem{Text="Segundo",Value="2"},
                    new SelectListItem{Text="Tercer",Value="3"},
                    new SelectListItem{Text="Cuarto",Value="4"},
                    new SelectListItem{Text="Último",Value="5"}
            };
            else
                return new List<SelectListItem> {
                    new SelectListItem{Text="First",Value="1"},
                    new SelectListItem{Text="Second",Value="2"},
                    new SelectListItem{Text="Third",Value="3"},
                    new SelectListItem{Text="Fourth",Value="4"},
                    new SelectListItem{Text="Last",Value="5"}
            };
        }

        public static string getDiaNombre(string nombre)
        {
            if (nombre == "D") return "Sunday";
            if (nombre == "L") return "Monday";
            if (nombre == "M") return "Tuesday";
            if (nombre == "X") return "Wednesday";
            if (nombre == "J") return "Thursday";
            if (nombre == "V") return "Friday";
            if (nombre == "S") return "Saturday";
            else return "";
        }
        public static int getDiaNum(string nombre)
        {
            if (nombre == "Sunday") return 7;
            if (nombre == "Monday") return 1;
            if (nombre == "Tuesday") return 2;
            if (nombre == "Wednesday") return 3;
            if (nombre == "Thursday") return 4;
            if (nombre == "Friday") return 5;
            if (nombre == "Saturday") return 6;
            else return 1;
        }
        public static DateTime obtenerProximaFecha(NEGOCIACION2 modelo, string tipomes)
        {
            DateTime proximafecha = modelo.FINICIO;
            if (modelo.FRECUENCIA == "S")
            {
                var diasemana = getDiaNombre(modelo.DIA_SEMANA);
                if (modelo.FINICIO.DayOfWeek.ToString() == diasemana)
                {
                    return modelo.FINICIO;
                }
                else
                {
                    var dia_semana = getDiaNum(getDiaNombre(modelo.DIA_SEMANA));
                    var dia_inicio = getDiaNum(modelo.FINICIO.DayOfWeek.ToString());
                    if (dia_inicio == 7)
                    {
                        if (dia_semana == 6)
                            proximafecha = proximafecha.AddDays(6);
                        else if (dia_semana == 5)
                            proximafecha = proximafecha.AddDays(5);
                        else if (dia_semana == 4)
                            proximafecha = proximafecha.AddDays(4);
                        else if (dia_semana == 3)
                            proximafecha = proximafecha.AddDays(3);
                        else if (dia_semana == 2)
                            proximafecha = proximafecha.AddDays(2);
                        else if (dia_semana == 1)
                            proximafecha = proximafecha.AddDays(1);
                    }
                    if (dia_inicio == 6)
                    {
                        if (dia_semana > dia_inicio)
                            proximafecha = proximafecha.AddDays((dia_semana - dia_inicio));
                        else if (dia_semana == 5)
                            proximafecha = proximafecha.AddDays(6);
                        else if (dia_semana == 4)
                            proximafecha = proximafecha.AddDays(5);
                        else if (dia_semana == 3)
                            proximafecha = proximafecha.AddDays(4);
                        else if (dia_semana == 2)
                            proximafecha = proximafecha.AddDays(3);
                        else if (dia_semana == 1)
                            proximafecha = proximafecha.AddDays(2);
                    }
                    if (dia_inicio == 5)
                    {
                        if (dia_semana > dia_inicio)
                            proximafecha = proximafecha.AddDays((dia_semana - dia_inicio));
                        else if (dia_semana == 4)
                            proximafecha = proximafecha.AddDays(6);
                        else if (dia_semana == 3)
                            proximafecha = proximafecha.AddDays(5);
                        else if (dia_semana == 2)
                            proximafecha = proximafecha.AddDays(4);
                        else if (dia_semana == 1)
                            proximafecha = proximafecha.AddDays(3);
                    }
                    if (dia_inicio == 4)
                    {
                        if (dia_semana > dia_inicio)
                            proximafecha = proximafecha.AddDays((dia_semana - dia_inicio));
                        else if (dia_semana == 3)
                            proximafecha = proximafecha.AddDays(6);
                        else if (dia_semana == 2)
                            proximafecha = proximafecha.AddDays(5);
                        else if (dia_semana == 1)
                            proximafecha = proximafecha.AddDays(dia_inicio);
                    }
                    if (dia_inicio == 3)
                    {
                        if (dia_semana > dia_inicio)
                            proximafecha = proximafecha.AddDays((dia_semana - dia_inicio));
                        else if (dia_semana == 2)
                            proximafecha = proximafecha.AddDays(6);
                        else if (dia_semana == 1)
                            proximafecha = proximafecha.AddDays(5);
                    }
                    if (dia_inicio == 2)
                    {
                        if (dia_semana > dia_inicio)
                            proximafecha = proximafecha.AddDays((dia_semana - dia_inicio));
                        else if (dia_semana == 1)
                            proximafecha = proximafecha.AddDays(6);
                    }
                    if (dia_inicio == 1)
                    {
                        proximafecha = proximafecha.AddDays((dia_semana - dia_inicio));
                    }
                }
            }
            else
            {
                if (tipomes == "1")
                {
                    if ((int)modelo.FINICIO.Day <= modelo.DIA_MES)
                    {
                        proximafecha = new DateTime(modelo.FINICIO.Year, modelo.FINICIO.Month, (int)modelo.DIA_MES);
                    }
                    else
                    {
                        proximafecha = new DateTime(modelo.FINICIO.Year, modelo.FINICIO.Month, (int)modelo.DIA_MES).AddMonths(modelo.FRECUENCIA_N);
                    }
                }
                else
                {
                    var dia = getDiaNombre(modelo.ORDINAL_DSEMANA);
                    var ordinal = modelo.ORDINAL_MES;
                    var diasmes = DateTime.DaysInMonth(modelo.FINICIO.Year, modelo.FINICIO.Month);
                    for (int i = 1; i <= diasmes; i++)
                    {
                        var fecha = new DateTime(modelo.FINICIO.Year, modelo.FINICIO.Month, i);
                        if (fecha.DayOfWeek.ToString() == dia)
                        {
                            var mes = fecha.Month;
                            fecha = fecha.AddDays((7 * (int)(modelo.ORDINAL_MES - 1)));
                            var mes2 = fecha.Month;
                            if (mes != mes2)
                                fecha = fecha.AddDays(-7);
                            if (modelo.FINICIO < fecha)
                                return fecha;
                            var fecha2 = new DateTime(modelo.FINICIO.Year, modelo.FINICIO.Month, 1).AddMonths(1);
                            var diasmes2 = DateTime.DaysInMonth(fecha2.Year, fecha2.Month);
                            for (int j = 1; j <= diasmes2; j++)
                            {
                                var fechatemp = new DateTime(fecha2.Year, fecha2.Month, j);
                                if (fechatemp.DayOfWeek.ToString() == dia)
                                {
                                    fechatemp = fechatemp.AddDays((7 * (int)(modelo.ORDINAL_MES - 1)));
                                    return fechatemp;
                                }
                            }
                        }
                    }

                }
            }
            return proximafecha;
        }
        public static decimal ObtenerImpuesto(TruequeEntities db, DOCUMENTO D, ref bool esNC, string[] categorias = null)
        {
            decimal impuesto = 0.0M;
            string[] tsolImp = new string[] { "NC", "NCA", "NCAS", "NCAM", "NCASM", "NCS", "NCI", "NCIA", "NCIAS", "NCIS" };
            string pais_id = D.PAIS_ID;
            if (tsolImp.Contains(D.TSOL_ID))
            {
                decimal KBETR;
                esNC = true;

                if (categorias != null)
                {
                    // Se obtiene Id de la categoria
                    Cadena cad = new Cadena();
                    string[] auxCategorias = categorias;
                    int i = 0;
                    bool esMaterial = false;
                    foreach (string cat in auxCategorias)
                    {
                        string catAux = cad.completaMaterial(cat);
                        if (!esMaterial)
                        {
                            esMaterial = db.MATERIALs.Any(x => x.ID == catAux);
                        }
                        if (esMaterial)
                        {
                            categorias[i] = db.MATERIALs.First(x => x.ID == catAux).MATERIALGP_ID;
                        }

                        i++;
                    }

                }
                if ((categorias != null && categorias.Any() && (categorias.Contains("605") || categorias.Contains("207"))) || db.DOCUMENTOPs.Any(x => (x.MATKL == "605" || x.MATKL == "207") && x.NUM_DOC == D.NUM_DOC))
                {
                    KBETR = db.IIMPUESTOes.First(x => x.MWSKZ == "A0").KBETR.Value;
                }
                else
                {
                    try
                    {
                        decimal concecutivo = db.CONPOSAPHs.First(x => x.TIPO_SOL == "NC" && x.SOCIEDAD == D.SOCIEDAD_ID && (x.TIPO_DOC == "YG" || x.TIPO_DOC == "DG")).CONSECUTIVO;
                        string tax_code = db.CONPOSAPPs.First(x => x.CONSECUTIVO == concecutivo).TAX_CODE;
                        KBETR = db.IIMPUESTOes.Any(x => x.MWSKZ == tax_code && x.LAND == pais_id) ? db.IIMPUESTOes.First(x => x.MWSKZ == tax_code && x.LAND == pais_id).KBETR.Value : 0.0M;
                    }
                    catch { KBETR = 0; }
                }
                impuesto = (D.MONTO_DOC_MD.Value * KBETR);
            }
            return impuesto;
        }
    }
}