using System;
using System.Collections.Generic;
using System.Linq;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Models.Dao;
using TAT001.Services;

namespace TAT001.Common
{
    public static class FnCommonCarta
    {
        readonly static TiposSolicitudesDao tiposSolicitudesDao = new TiposSolicitudesDao();

        public static string ObtenerTexto(TruequeEntities db, string spras_id, string campo)
        {
            return db.TEXTOCVs.Where(x => x.SPRAS_ID == spras_id && x.CAMPO == campo).Select(x => x.TEXTO).FirstOrDefault();
        }

        public static void ObtenerCartaProductos(TruequeEntities db, DOCUMENTO d,CartaD cd, CartaV cv, string spras_id,bool guardar,
            ref List<string> lista,
            ref List<listacuerpoc> armadoCuerpoTab,
            ref List<string> armadoCuerpoTabStr,  
            ref List<int> numfilasTabla,
            ref List<string>  cabeza,
            ref bool editmonto)
        {
            int contadorTabla = 0;
            string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos
            bool fact =  db.TSOLs.First(ts => ts.ID == d.TSOL_ID).FACTURA;
           
            var con = db.DOCUMENTOPs.Select(x => new { x.NUM_DOC, x.VIGENCIA_DE, x.VIGENCIA_AL }).Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).GroupBy(f => new { f.VIGENCIA_DE, f.VIGENCIA_AL }).ToList();
                foreach (var item in con)
                {
                    lista.Add(item.Key.VIGENCIA_DE.ToString() + item.Key.VIGENCIA_AL.ToString());
                }
            int indexp = 1;
            for (int i = 0; i < lista.Count; i++)
                {
                    contadorTabla = 0;

                    DateTime a1 = DateTime.Parse(lista[i].Remove(lista[i].Length / 2));
                    DateTime a2 = DateTime.Parse(lista[i].Remove(0, lista[i].Length / 2));

                List<DOCUMENTOP_SP> con2 = FnCommon.ObtenerDocumentoP(db, spras_id,d.NUM_DOC,a1,a2);



                //Definición si la distribución es monto o porcentaje
                string porclass = "";//B20180710 MGC 2018.07.18 total es input o text
                    string totalm = "";//B20180710 MGC 2018.07.18 total es input o text
               
                if (d.TIPO_TECNICO == "M")
                    {
                        porclass = " tipom";
                        totalm = " total";
                    }
                    else if (d.TIPO_TECNICO == "P")
                    {
                        porclass = " tipop";
                        totalm = " ni";
                    }

                if (con2.Count > 0)
                {
                    //Listado para html CartaV
                    if (armadoCuerpoTab != null)
                    {
                        ObtenerCartaProductosHtml(con2, decimales, porclass, totalm, fact,
                            ref armadoCuerpoTab,
                            ref contadorTabla);
                    }
                    //Listado para html CartaD y Listado Pdf CartaD y CartaV
                    if (armadoCuerpoTabStr != null)
                    {
                        ObtenerCartaProductosPdf(db, cd, cv, con2, decimales, porclass, totalm, fact, guardar,
                            ref indexp,
                            ref armadoCuerpoTabStr,
                            ref contadorTabla);
                    }
                }
                else
                {
                    List<DOCUMENTOP_CAT> con3 = db.DOCUMENTOPs
                                        .Where(x => x.NUM_DOC.Equals(d.NUM_DOC) && x.VIGENCIA_DE == a1 && x.VIGENCIA_AL == a2)
                                        .Join(db.MATERIALGPs, x => x.MATKL, y => y.ID, (x, y) => new DOCUMENTOP_CAT
                                        {
                                            NUM_DOC = x.NUM_DOC,
                                            MATNR = x.MATNR,
                                            MATKL = x.MATKL,
                                            ID = y.ID,
                                            MONTO = x.MONTO,
                                            PORC_APOYO = x.PORC_APOYO,
                                            TXT50 = y.DESCRIPCION,//RSG 03.10.2018
                                            MONTO_APOYO = x.MONTO_APOYO,
                                            RESTA = (x.MONTO - x.MONTO_APOYO),
                                            PRECIO_SUG = x.PRECIO_SUG,
                                            APOYO_EST = x.APOYO_EST,
                                            APOYO_REAL = x.APOYO_REAL,
                                            VOLUMEN_EST = x.VOLUMEN_EST,
                                            VOLUMEN_REAL = x.VOLUMEN_REAL,
                                            VIGENCIA_DE = x.VIGENCIA_DE,
                                            VIGENCIA_AL = x.VIGENCIA_AL
                                        }) //B20180710 MGC 2018.07.10 Se agregó x.VOLUMEN_EST, x.VOLUMEN_REAL})
                                        .ToList();
                    if (d.TIPO_TECNICO == "P")
                    {
                        editmonto = true;
                    }

                    //Listado para Html CartaV
                    if (armadoCuerpoTab != null)
                    {
                        ObtenerCartaCategoriasHtml(con3, decimales, fact, d.TIPO_TECNICO,
                            ref armadoCuerpoTab,
                            ref contadorTabla);
                    }
                    //Listado para html CartaD y Listado Pdf CartaD y CartaV
                    if (armadoCuerpoTabStr != null)
                    {
                        ObtenerCartaCategoriasPdf(db, cd, cv, con3, decimales, fact, guardar,
                            ref indexp,
                            ref armadoCuerpoTabStr,
                            ref contadorTabla);
                    }
                }
                    numfilasTabla.Add(contadorTabla);
                }

            if (cd == null && cv==null)
            {
                cabeza.Add(ObtenerTexto(db, spras_id, "materialC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "categoriaC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "descripcionC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "costouC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "apoyopoC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "apoyopiC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "costoaC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "preciosC"));
                //Volumen
                if (fact) { cabeza.Add(ObtenerTexto(db, spras_id, "volumenrC")); }
                else { cabeza.Add(ObtenerTexto(db, spras_id, "volumeneC")); }
                //Apoyo
                if (fact) { cabeza.Add(ObtenerTexto(db, spras_id, "apoyorC")); }
                else { cabeza.Add(ObtenerTexto(db, spras_id, "apoyoeC")); }
            }
            else
            {
                if ((cd != null && cd.material_x) || (cv != null && cv.material_x)) { cabeza.Add(ObtenerTexto(db, spras_id, "materialC")); }
                cabeza.Add(ObtenerTexto(db, spras_id, "categoriaC"));
                cabeza.Add(ObtenerTexto(db, spras_id, "descripcionC"));
                if ((cd != null && cd.costoun_x) || (cv != null && cv.costoun_x)) { cabeza.Add(ObtenerTexto(db, spras_id, "costouC")); }
                if ((cd != null && cd.apoyo_x) || (cv != null && cv.apoyo_x)) { cabeza.Add(ObtenerTexto(db, spras_id, "apoyopoC")); }
                if ((cd != null && cd.apoyop_x) || (cv != null && cv.apoyop_x)) { cabeza.Add(ObtenerTexto(db, spras_id, "apoyopiC")); }
                if ((cd != null && cd.costoap_x) || (cv != null && cv.costoap_x)) { cabeza.Add(ObtenerTexto(db, spras_id, "costoaC")); }
                if ((cd != null && cd.precio_x) || (cv != null && cv.precio_x)) { cabeza.Add(ObtenerTexto(db, spras_id, "preciosC")); }
                if ((cd != null && cd.volumen_x) || (cv != null && cv.volumen_x))
                {
                    if (fact) { cabeza.Add(ObtenerTexto(db, spras_id, "volumenrC")); }
                    else { cabeza.Add(ObtenerTexto(db, spras_id, "volumeneC")); }
                }
                if ((cd != null && cd.apoyototal_x) || (cv != null && cv.apoyototal_x))
                {
                    if (fact) { cabeza.Add(ObtenerTexto(db, spras_id, "apoyorC")); }
                    else { cabeza.Add(ObtenerTexto(db, spras_id, "apoyoeC")); }
                }
            }
            
        }

        public static void ObtenerCartaProductosPdf(TruequeEntities db, CartaD cd, CartaV cv, List<DOCUMENTOP_SP> con2, string decimales, string porclass, string totalm, bool fact,bool guardar,
            ref int indexp,
            ref List<string> armadoCuerpoTabStr,
            ref int contadorTabla)
        {
            FormatosC format = new FormatosC();
            foreach (var item2 in con2)
            {
                if (guardar)
                {
                    int pos = db.CARTAs.Where(a => a.NUM_DOC.Equals(cv.num_doc)).OrderByDescending(a => a.POS).First().POS;
                    GuardarCartaPMateriales(db, cv, item2.MATNR, pos, ref indexp, fact, item2.VIGENCIA_DE.Value, item2.VIGENCIA_AL.Value);
                }
                if (cd != null || cv != null)
                {
                    DOCUMENTOP_MOD docmod = new DOCUMENTOP_MOD();
                    if (cv != null)
                    {
                        docmod = cv.DOCUMENTOP.FirstOrDefault(x => x.MATNR == item2.MATNR.TrimStart('0'));
                    }
                    if (cd != null)
                    {
                        docmod = new DOCUMENTOP_MOD
                        {
                            MONTO = item2.MONTO,
                            PORC_APOYO= item2.PORC_APOYO,
                            MONTO_APOYO = item2.MONTO_APOYO,
                            PRECIO_SUG = item2.PRECIO_SUG,
                            VOLUMEN_REAL = item2.VOLUMEN_REAL,
                            VOLUMEN_EST = item2.VOLUMEN_EST,
                            APOYO_REAL=item2.APOYO_REAL,
                            APOYO_EST=item2.APOYO_EST

                        };
                    }
                    if ((cd != null && cd.material_x) || (cv != null && cv.material_x))
                    {
                        armadoCuerpoTabStr.Add(item2.MATNR.TrimStart('0'));
                    }
                    armadoCuerpoTabStr.Add(item2.DESCRIPCION);
                    armadoCuerpoTabStr.Add(item2.MAKTX);

                    if ((cd!=null && cd.costoun_x) || (cv != null && cv.costoun_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(docmod.MONTO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.apoyo_x) || (cv != null && cv.apoyo_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShowPorc(Math.Round(docmod.PORC_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.apoyop_x) || (cv != null && cv.apoyop_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(docmod.MONTO_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.costoap_x) || (cv != null && cv.costoap_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round((docmod.MONTO - docmod.MONTO_APOYO), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.precio_x) || (cv != null && cv.precio_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(docmod.PRECIO_SUG, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.volumen_x) || (cv != null && cv.volumen_x))
                    {
                        if (fact)
                        {
                            armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(docmod.VOLUMEN_REAL), 2), decimales)); //B20180730 MGC 2018.07.30 Formatos
                        }
                        else
                        {
                            armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(docmod.VOLUMEN_EST), 2), decimales)); //B20180730 MGC 2018.07.30 Formatos
                        }
                    }

                    //Apoyo
                    //B20180726 MGC 2018.07.26
                    if ((cd != null && cd.apoyototal_x) || (cv != null && cv.apoyototal_x))
                    {
                        if (fact)
                        {
                            armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(docmod.APOYO_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        }
                        else
                        {
                            armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(docmod.APOYO_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        }
                    }
               }
                else
                {
                    armadoCuerpoTabStr.Add(item2.MATNR.TrimStart('0'));
                    armadoCuerpoTabStr.Add(item2.DESCRIPCION);
                    armadoCuerpoTabStr.Add(item2.MAKTX);
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.MONTO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.MONTO_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.RESTA, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.PRECIO_SUG, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                                                                                      //B20180726 MGC 2018.07.26
                                                                                                      //Volumen y apoyo
                    if (fact)
                    {
                        armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    else
                    {
                        armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                }
                contadorTabla++;
            }
        }

        public static void ObtenerCartaProductosHtml(List<DOCUMENTOP_SP> con2, string decimales,string porclass,string totalm,bool fact,
            ref List<listacuerpoc> armadoCuerpoTab, 
            ref int contadorTabla)
        {
            FormatosC format = new FormatosC();
            foreach (var item2 in con2)
            {

                //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                listacuerpoc lc1 = new listacuerpoc
                {
                    val = item2.MATNR.TrimStart('0'),
                    clase = "ni"
                };
                armadoCuerpoTab.Add(lc1);

                listacuerpoc lc2 = new listacuerpoc
                {
                    val = item2.DESCRIPCION,
                    clase = "ni"
                };
                armadoCuerpoTab.Add(lc2);

                listacuerpoc lc3 = new listacuerpoc
                {
                    val = item2.MAKTX,
                    clase = "ni"
                };
                armadoCuerpoTab.Add(lc3);

                //Costo unitario
                listacuerpoc lc4 = new listacuerpoc
                {
                    val = format.toShow(Math.Round(item2.MONTO, 2), decimales), //B20180730 MGC 2018.07.30 Formatos
                    clase = "input_oper numberd input_dc mon" + porclass
                };
                armadoCuerpoTab.Add(lc4);

                //Porcentaje de apoyo
                listacuerpoc lc5 = new listacuerpoc
                {
                    val = format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                    clase = "input_oper numberd porc input_dc" + porclass
                };
                armadoCuerpoTab.Add(lc5);

                //Apoyo por pieza
                listacuerpoc lc6 = new listacuerpoc
                {
                    val = format.toShow(Math.Round(item2.MONTO_APOYO, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                    clase = "input_oper numberd costoa input_dc mon" + porclass
                };
                armadoCuerpoTab.Add(lc6);

                //Costo con apoyo
                listacuerpoc lc7 = new listacuerpoc
                {
                    val = format.toShow(Math.Round(item2.RESTA, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                    clase = "input_oper numberd costoa input_dc mon" + porclass//Importante costoa para validación en vista
                };
                armadoCuerpoTab.Add(lc7);

                //Precio Sugerido
                listacuerpoc lc8 = new listacuerpoc
                {
                    val = format.toShow(Math.Round(item2.PRECIO_SUG, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                    clase = "input_oper numberd input_dc mon" + porclass
                };
                armadoCuerpoTab.Add(lc8);

                //Modificación 9 y 10 dependiendo del campo de factura en tsol
                //Volumen
                listacuerpoc lc9 = new listacuerpoc();
                if (fact)
                {
                    lc9.val = format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_REAL), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                }
                else
                {
                    lc9.val = format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_EST), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                }
                lc9.clase = "input_oper numberd input_dc num" + porclass;
                armadoCuerpoTab.Add(lc9);

                //Apoyo
                listacuerpoc lc10 = new listacuerpoc();
                if (fact)
                {
                    lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                }
                else
                {
                    lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                }
                lc10.clase = "input_oper numberd input_dc mon" + totalm + "" + porclass;
                armadoCuerpoTab.Add(lc10);

                contadorTabla++;
            }
        }

        public  static void ObtenerCartaCategoriasPdf(TruequeEntities db, CartaD cd,CartaV cv, List<DOCUMENTOP_CAT> con3, string decimales, bool fact, bool guardar,
            ref int indexp,
            ref List<string> armadoCuerpoTabStr,
            ref int contadorTabla)
        {
            FormatosC format = new FormatosC();
            foreach (var item2 in con3)
            {
                if (guardar)
                {
                    int pos = db.CARTAs.Where(a => a.NUM_DOC.Equals(cv.num_doc)).OrderByDescending(a => a.POS).First().POS;
                    GuardarCartaPCategorias(db, cv, item2.MATKL, pos, ref indexp, fact, item2.VIGENCIA_DE.Value, item2.VIGENCIA_AL.Value);
                }
                if (cd != null || cv != null)
                {
                    DOCUMENTOP_MOD docmod = new DOCUMENTOP_MOD();
                if (cv != null)
                {
                     docmod = cv.DOCUMENTOP.FirstOrDefault(x => x.MATKL_ID == item2.MATKL);
                }
                if (cd != null)
                {
                    docmod = new DOCUMENTOP_MOD
                    {
                        MONTO=item2.MONTO,
                        PORC_APOYO= item2.PORC_APOYO,
                        MONTO_APOYO = item2.MONTO_APOYO,
                        PRECIO_SUG = item2.PRECIO_SUG,
                        VOLUMEN_REAL = item2.VOLUMEN_REAL,
                        VOLUMEN_EST = item2.VOLUMEN_EST,
                        APOYO_REAL = item2.APOYO_REAL,
                        APOYO_EST = item2.APOYO_EST
                    };
                }
                    if ((cd != null && cd.material_x) || (cv != null && cv.material_x))
                    {
                        armadoCuerpoTabStr.Add("");
                    }
                    armadoCuerpoTabStr.Add(item2.MATKL);
                    MATERIALGP mt = db.MATERIALGPs.Where(x => x.ID == item2.MATKL).FirstOrDefault();//RSG 03.10.2018
                    if (mt != null)
                        armadoCuerpoTabStr.Add(mt.DESCRIPCION);//RSG 03.10.2018
                    else
                        armadoCuerpoTabStr.Add("");

                    if ((cd != null && cd.costoun_x) || (cv != null && cv.costoun_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(docmod.MONTO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.apoyo_x) || (cv != null && cv.apoyo_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShowPorc(Math.Round(docmod.PORC_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.apoyop_x) || (cv != null && cv.apoyop_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(docmod.MONTO_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }

                    if ((cd != null && cd.costoap_x) || (cv != null && cv.costoap_x))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round((docmod.MONTO - docmod.MONTO_APOYO), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    if ((cd != null && cd.precio_x) || (cv != null && cv.precio_x ))
                    {
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(docmod.PRECIO_SUG, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }

                    //Volumen
                    //B20180726 MGC 2018.07.26
                    if ((cd != null && cd.volumen_x) || (cv != null && cv.volumen_x))
                    {
                        if (fact)
                        {
                            armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(docmod.VOLUMEN_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        }
                        else
                        {
                            armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(docmod.VOLUMEN_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        }
                    }

                    //Apoyo
                    //B20180726 MGC 2018.07.26
                    if ((cd != null && cd.apoyototal_x) || (cv != null && cv.apoyototal_x))
                    {
                        if (fact)
                        {
                            armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(docmod.APOYO_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        }
                        else
                        {
                            armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(docmod.APOYO_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        }
                    }
               }
                else
                {
                    armadoCuerpoTabStr.Add("");
                    armadoCuerpoTabStr.Add(item2.MATKL);
                    armadoCuerpoTabStr.Add(item2.TXT50);
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.MONTO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.MONTO_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.RESTA, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    armadoCuerpoTabStr.Add(format.toShow(Math.Round(item2.PRECIO_SUG, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                                                                                      //B20180726 MGC 2018.07.26
                    if (fact)
                    {
                        armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }
                    else
                    {
                        armadoCuerpoTabStr.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                        armadoCuerpoTabStr.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                    }

                }

                contadorTabla++;
            }

        }
        public static void ObtenerCartaCategoriasHtml(List<DOCUMENTOP_CAT> con3, string decimales, bool fact, string tipo_tecnico,
            ref List<listacuerpoc> armadoCuerpoTab,
            ref int contadorTabla)
        {
            FormatosC format = new FormatosC();
            foreach (var item2 in con3)
            {
                //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                listacuerpoc lc1 = new listacuerpoc();
                lc1.val = "";
                lc1.clase = "ni";
                armadoCuerpoTab.Add(lc1);

                listacuerpoc lc2 = new listacuerpoc();
                lc2.val = item2.MATKL;
                lc2.clase = "ni";
                armadoCuerpoTab.Add(lc2);

                listacuerpoc lc3 = new listacuerpoc();
                lc3.val = item2.TXT50;
                lc3.clase = "ni";
                armadoCuerpoTab.Add(lc3);

                //Costo unitario
                listacuerpoc lc4 = new listacuerpoc();
                lc4.val = format.toShow(0, decimales); //B20180730 MGC 2018.07.30 Formatos
                lc4.clase = "ni";
                armadoCuerpoTab.Add(lc4);

                //Porcentaje de apoyo
                listacuerpoc lc5 = new listacuerpoc();
                //Definición si la distribución es monto o porcentaje
                if (tipo_tecnico == "M")
                {
                    lc5.val = format.toShowPorc(0, decimales); //B20180730 MGC 2018.07.30 Formatos
                }
                else if (tipo_tecnico == "P")
                {
                    lc5.val = format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales); //B20180730 MGC 2018.07.30 Formatos
                }

                lc5.clase = "ni";
                armadoCuerpoTab.Add(lc5);

                //Apoyo por pieza
                listacuerpoc lc6 = new listacuerpoc();
                lc6.val = format.toShow(0, decimales); //B20180730 MGC 2018.07.30 Formatos
                lc6.clase = "ni";
                armadoCuerpoTab.Add(lc6);

                //Costo con apoyo
                listacuerpoc lc7 = new listacuerpoc();
                lc7.val = format.toShow(0, decimales); //B20180730 MGC 2018.07.30 Formatos
                lc7.clase = "ni";
                armadoCuerpoTab.Add(lc7);

                //Precio Sugerido
                listacuerpoc lc8 = new listacuerpoc();
                lc8.val = format.toShow(0, decimales); //B20180730 MGC 2018.07.30 Formatos
                lc8.clase = "ni";
                armadoCuerpoTab.Add(lc8);
                //Modificación 9 y 10 dependiendo del campo de factura en tsol
                //fact = true es real

                //Volumen
                listacuerpoc lc9 = new listacuerpoc();
                lc9.val = format.toShowNum(0, decimales); //B20180730 MGC 2018.07.30 Formatos

                lc9.clase = "ni";
                armadoCuerpoTab.Add(lc9);

                //Apoyo
                listacuerpoc lc10 = new listacuerpoc();
                if (fact)
                {
                    lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales); //B20180730 MGC 2018.07.30 Formatos
                }
                else
                {
                    lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales); //B20180730 MGC 2018.07.30 Formatos
                }
                //Definición si la distribución es monto o porcentaje
                if (tipo_tecnico == "M")
                {
                    lc10.clase = "input_oper numberd input_dc total cat mon";
                }
                else if (tipo_tecnico == "P")
                {
                    lc10.clase = "ni";
                }

                armadoCuerpoTab.Add(lc10);

                contadorTabla++;
            }

        }

        static  void GuardarCartaPMateriales(TruequeEntities db,CartaV v,string MATNR,int pos,ref int indexp,bool fact,DateTime VIGENCIA_DE,DateTime VIGENCIA_AL)
        {
            try
            {
                DOCUMENTOP_MOD docmod = v.DOCUMENTOP.FirstOrDefault(x => x.MATNR == MATNR.TrimStart('0'));
                if (docmod != null )
                {
                    CARTAP carp = new CARTAP
                    {
                        //Armado para registro en bd
                        NUM_DOC = v.num_doc,
                        POS_ID = pos,
                        POS = indexp,
                        MATNR = MATNR,
                        MATKL = "",
                        CANTIDAD = 1,
                        MONTO = docmod.MONTO,
                        PORC_APOYO = docmod.PORC_APOYO,
                        MONTO_APOYO = docmod.MONTO_APOYO,
                        PRECIO_SUG = docmod.PRECIO_SUG
                    };

                    //Volumen
                    //B20180726 MGC 2018.07.26
                    if (v.volumen_x )
                    {
                        if (fact)
                        {
                            carp.VOLUMEN_REAL = (docmod.VOLUMEN_REAL==null?0.0M: docmod.VOLUMEN_REAL);
                            carp.VOLUMEN_EST = 0;
                        }
                        else
                        {
                            carp.VOLUMEN_EST = docmod.VOLUMEN_EST;
                            carp.VOLUMEN_REAL = 0;
                        }
                    }

                    //Apoyo
                    //B20180726 MGC 2018.07.26
                    if (v.apoyototal_x)
                    {
                        if (fact)
                        {
                            carp.APOYO_REAL = (docmod.APOYO_REAL == null ? 0.0M : docmod.APOYO_REAL);
                            carp.APOYO_EST = 0;
                        }
                        else
                        {
                            carp.APOYO_EST = (docmod.APOYO_EST == null ? 0.0M : docmod.APOYO_EST);
                            carp.APOYO_REAL = 0;
                        }
                    }

                    //Fechas
                    carp.VIGENCIA_DE = VIGENCIA_DE;
                    carp.VIGENCIA_AL = VIGENCIA_AL;

                    try
                    {
                        //Guardar en CARPETAP
                        db.CARTAPs.Add(carp);
                        db.SaveChanges();
                        indexp++;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Carta", "Create");
                    }
                }


            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e, "Carta", "Create");
            }
        }

      static void GuardarCartaPCategorias(TruequeEntities db, CartaV v, string MATKL, int pos, ref int indexp, bool fact, DateTime VIGENCIA_DE, DateTime VIGENCIA_AL)
        {

            try
            {
                DOCUMENTOP_MOD docmod = v.DOCUMENTOP.FirstOrDefault(x => x.MATKL_ID == MATKL);

                if (docmod != null)
                {
                    CARTAP carp = new CARTAP
                    {
                        //Armado para registro en bd
                        NUM_DOC = v.num_doc,
                        POS_ID = pos,
                        POS = indexp,
                        MATNR = "",
                        MATKL = MATKL,
                        CANTIDAD = 1,
                        MONTO = docmod.MONTO,
                        PORC_APOYO = docmod.PORC_APOYO,
                        MONTO_APOYO = docmod.MONTO_APOYO,
                        PRECIO_SUG = docmod.PRECIO_SUG
                    };

                    //Volumen
                    //B20180726 MGC 2018.07.26
                    if (v.volumen_x)
                    {
                        if (fact)
                        {
                            carp.VOLUMEN_REAL = (docmod.VOLUMEN_REAL == null ? 0.0M : docmod.VOLUMEN_REAL);
                            carp.VOLUMEN_EST = 0;
                        }
                        else
                        {
                            carp.VOLUMEN_EST = docmod.VOLUMEN_EST;
                            carp.VOLUMEN_REAL = 0;
                        }
                    }

                    //Apoyo
                    //B20180726 MGC 2018.07.26
                    if (v.apoyototal_x)
                    {
                        if (fact)
                        {
                            carp.APOYO_REAL = (docmod.APOYO_REAL == null ? 0.0M : docmod.APOYO_REAL);
                            carp.APOYO_EST = 0;
                        }
                        else
                        {
                            carp.APOYO_REAL = 0;
                            carp.APOYO_EST = (docmod.APOYO_EST == null ? 0.0M : docmod.APOYO_EST);
                        }
                    }

                    //Fechas
                    carp.VIGENCIA_DE = VIGENCIA_DE;
                    carp.VIGENCIA_AL = VIGENCIA_AL;

                    try
                    {
                        //Guardar en CARPETAP
                        db.CARTAPs.Add(carp);
                        db.SaveChanges();
                        indexp++;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Carta", "Create");
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e, "Carta", "Create");
            }
        }
      public static void ObtenerCartaRecurrentes(TruequeEntities db, DOCUMENTO d, string spras_id,
                 ref List<string> cabeza2,
                 ref List<string> armadoCuerpoTab2,
                 ref int rowsRecs,
                 ref List<string> cabeza3,
                 ref List<string> armadoCuerpoTab3,
                 ref int rowsObjQs,
                 bool esPdf, string monto)
        {
            FormatosC format = new FormatosC();
            string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos
            bool varligada = Convert.ToBoolean(d.LIGADA);
            bool conRan = db.DOCUMENTORANs.Any(x => x.LIN == 1 && x.NUM_DOC == d.NUM_DOC);
            int rowRan = 0;
            cabeza2.Add(ObtenerTexto(db, spras_id, "posC2"));
            cabeza2.Add(ObtenerTexto(db, spras_id, "periodoC"));
            cabeza2.Add(ObtenerTexto(db, spras_id, "tipoC2"));
            if (varligada)
            {
                cabeza2.Add(ObtenerTexto(db, spras_id, "porcentajeC2"));
               if (conRan){cabeza2.Add(ObtenerTexto(db, spras_id, "objetivo")); }
                else if (!esPdf) { cabeza2.Add(""); }
                
            }
            else
            {
                if (d.TIPO_TECNICO == "P")
                {
                    cabeza2.Add(ObtenerTexto(db, spras_id, "porcentajeC2"));
                    if (!esPdf){ cabeza2.Add("");}
                }
                else
                {
                    cabeza2.Add(ObtenerTexto(db, spras_id, "montoC2"));
                    if (!esPdf) { cabeza2.Add(""); }
                }
            }

            var con4 = db.DOCUMENTORECs
                                    .Where(x => x.NUM_DOC.Equals(d.NUM_DOC))
                                    .Join(db.DOCUMENTOes, x => x.NUM_DOC, y => y.NUM_DOC, (x, y) => new { x.POS, y.TSOL_ID, x.MONTO_BASE, x.PORC,x.PERIODO,x.EJERCICIO })
                                    .ToList();
          
            foreach (var item in con4)
            {
                string periodo = "P" + item.PERIODO + "-" + item.EJERCICIO;
                string tsol = tiposSolicitudesDao.ListaTiposSolicitudes( spras_id, item.TSOL_ID).First().TXT50;
                string posStr = item.POS.ToString() + "/" + con4.Count.ToString();

                armadoCuerpoTab2.Add(posStr);
                armadoCuerpoTab2.Add(periodo);
                armadoCuerpoTab2.Add(tsol);
                if (varligada)
                {
                    if (conRan) {
                        DOCUMENTORAN docRan = db.DOCUMENTORANs.First(x => x.LIN == 1 && x.NUM_DOC == d.NUM_DOC && x.POS == item.POS);

                        armadoCuerpoTab2.Add(format.toShowPorc(docRan.PORCENTAJE.Value, decimales));
                        armadoCuerpoTab2.Add(format.toShow(Math.Round(docRan.OBJETIVOI.Value, 2), decimales));

                        foreach (DOCUMENTORAN itemTan in db.DOCUMENTORANs.Where(x => x.LIN != 1 && x.NUM_DOC == d.NUM_DOC && x.POS == item.POS))
                        {
                            armadoCuerpoTab2.AddRange(new List<string>{ "","",""});
                            armadoCuerpoTab2.Add(format.toShowPorc(itemTan.PORCENTAJE.Value, decimales));
                            armadoCuerpoTab2.Add(format.toShow(Math.Round(itemTan.OBJETIVOI.Value, 2), decimales));
                            rowRan++;
                        }
                    }
                    else
                    {
                        armadoCuerpoTab2.Add(format.toShowPorc(item.PORC.Value, decimales));
                        if (!esPdf){ armadoCuerpoTab2.Add("");}
                    }
                   
                }
                else
                {
                    if (d.TIPO_TECNICO == "P")
                    {
                        armadoCuerpoTab2.Add(format.toShowPorc(item.PORC.Value,decimales));
                        if (!esPdf){ armadoCuerpoTab2.Add(""); }
                    }
                    else
                    {
                        ////armadoCuerpoTab2.Add(format.toShow(Math.Round(item.MONTO_BASE.Value, 2), decimales));
                        armadoCuerpoTab2.Add(monto);//ADD RSG 27.12.2018
                        if (!esPdf)
                        {
                            armadoCuerpoTab2.Add("");
                        }
                    }
                }
            }
            rowsRecs = (con4.Count+ rowRan);


            bool conObjetivoQ = (d.OBJETIVOQ != null && d.OBJETIVOQ.Value);
            ///TABLA OBJETIVO Q
            if (conObjetivoQ)
            {
                cabeza3.Add(ObtenerTexto(db, spras_id, "posC2"));
                cabeza3.Add(ObtenerTexto(db, spras_id, "periodoC"));
                cabeza3.Add(ObtenerTexto(db, spras_id, "tipoC2"));


                cabeza3.Add(ObtenerTexto(db, spras_id, "porcentajeC2"));
                if (conRan)
                { cabeza3.Add(ObtenerTexto(db, spras_id, "objetivo")); }
                else if (!esPdf) { cabeza3.Add(""); }

                foreach (var item in con4)
                {
                    string periodo = "P" + item.PERIODO + "-" + item.EJERCICIO;
                    string tsol = tiposSolicitudesDao.ListaTiposSolicitudes( spras_id, item.TSOL_ID).First().TXT50;
                    string posStr = item.POS.ToString() + "/" + con4.Count.ToString();
                    armadoCuerpoTab3.Add(posStr);
                    armadoCuerpoTab3.Add(periodo);
                    armadoCuerpoTab3.Add(tsol);

                    if (conRan)
                    {
                        DOCUMENTORAN docRan = db.DOCUMENTORANs.First(x => x.LIN == 1 && x.NUM_DOC == d.NUM_DOC && x.POS == item.POS);

                        armadoCuerpoTab3.Add(format.toShowPorc(d.OBJQ_PORC.Value, decimales));
                        armadoCuerpoTab3.Add(format.toShow(Math.Round(docRan.OBJETIVOI.Value, 2), decimales));
                    }
                    else
                    {
                        armadoCuerpoTab3.Add(format.toShowPorc(d.OBJQ_PORC.Value, decimales));
                        if (!esPdf) { armadoCuerpoTab2.Add(""); }
                    }


                }
                rowsObjQs = con4.Count;
            }
        }
    }
}