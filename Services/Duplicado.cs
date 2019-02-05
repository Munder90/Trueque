using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class Duplicado
    {
        public DOCUMENTBORR llenaDuplicado(TruequeEntities db1, decimal num_doc, string user)
        {
            DOCUMENTBORR docb = new DOCUMENTBORR();
            DOCUMENTO docPadre = db1.DOCUMENTOes.Find(num_doc);
            docb.AGENTE_ACTUAL = docPadre.AGENTE_ACTUAL;
            docb.CANAL_ID = docPadre.CANAL_ID;
            docb.CANTIDAD_EV = docPadre.CANTIDAD_EV;
            docb.CIUDAD = docPadre.CIUDAD;
            docb.CONCEPTO = docPadre.CONCEPTO;
            docb.DOCUMENTO_REF = docPadre.DOCUMENTO_REF;
            docb.EJERCICIO = docPadre.EJERCICIO;
            docb.ESTADO = docPadre.ESTADO;
            docb.ESTATUS = docPadre.ESTATUS;
            docb.ESTATUS_C = docPadre.ESTATUS_C;
            docb.ESTATUS_EXT = docPadre.ESTATUS_EXT;
            docb.ESTATUS_SAP = docPadre.ESTATUS_SAP;
            docb.ESTATUS_WF = docPadre.ESTATUS_WF;
            docb.FECHAC = docPadre.FECHAC;
            docb.FECHAC_PLAN = docPadre.FECHAC_PLAN;
            docb.FECHAC_USER = docPadre.FECHAC_USER;
            docb.FECHAD = docPadre.FECHAD;
            docb.FECHAD_SOPORTE = docPadre.FECHAD_SOPORTE;
            docb.FECHAF_VIG = docPadre.FECHAF_VIG;
            docb.FECHAI_VIG = docPadre.FECHAI_VIG;
            docb.FECHA_PASO_ACTUAL = docPadre.FECHA_PASO_ACTUAL;
            docb.GRUPO_CTE_ID = docPadre.GRUPO_CTE_ID;
            docb.HORAC = docPadre.HORAC;
            docb.HORAC_USER = docPadre.HORAC_USER;
            docb.IMPUESTO = docPadre.IMPUESTO;
            docb.LIGADA = "";
            if (docPadre.LIGADA == true)
                docb.LIGADA = "X";
            docb.METODO_PAGO = docPadre.METODO_PAGO;
            docb.MONEDAL2_ID = docPadre.MONEDAL2_ID;
            docb.MONEDAL_ID = docPadre.MONEDAL_ID;
            docb.MONEDA_ID = docPadre.MONEDA_ID;
            docb.MONEDA_DIS = docPadre.MONEDA_ID;
            docb.MONTO_BASE_GS_PCT_MD = docPadre.MONTO_BASE_GS_PCT_MD;
            docb.MONTO_BASE_GS_PCT_ML = docPadre.MONTO_BASE_GS_PCT_ML;
            docb.MONTO_BASE_GS_PCT_ML2 = docPadre.MONTO_BASE_GS_PCT_ML2;
            docb.MONTO_BASE_NS_PCT_MD = docPadre.MONTO_BASE_NS_PCT_MD;
            docb.MONTO_BASE_NS_PCT_ML = docPadre.MONTO_BASE_NS_PCT_ML;
            docb.MONTO_BASE_NS_PCT_ML2 = docPadre.MONTO_BASE_NS_PCT_ML2;
            docb.MONTO_DOC_MD = docPadre.MONTO_DOC_MD;
            docb.MONTO_DOC_ML = docPadre.MONTO_DOC_ML;
            docb.MONTO_DOC_ML2 = docPadre.MONTO_DOC_ML2;
            docb.MONTO_FIJO_MD = docPadre.MONTO_FIJO_MD;
            docb.MONTO_FIJO_ML = docPadre.MONTO_FIJO_ML;
            docb.MONTO_FIJO_ML2 = docPadre.MONTO_FIJO_ML2;
            docb.NOTAS = docPadre.NOTAS;
            docb.NO_FACTURA = docPadre.NO_FACTURA;
            docb.NO_PROVEEDOR = docPadre.NO_PROVEEDOR;
            docb.PAIS_ID = docPadre.PAIS_ID;
            docb.PASO_ACTUAL = docPadre.PASO_ACTUAL;
            docb.PAYER_EMAIL = docPadre.PAYER_EMAIL;
            docb.PAYER_ID = docPadre.PAYER_ID;
            docb.PAYER_NOMBRE = docPadre.PAYER_NOMBRE;
            docb.PERIODO = docPadre.PERIODO;
            docb.PORC_ADICIONAL = docPadre.PORC_ADICIONAL;
            docb.PORC_APOYO = docPadre.PORC_APOYO;
            docb.SOCIEDAD_ID = docPadre.SOCIEDAD_ID;
            docb.SOLD_TO_ID = docPadre.SOLD_TO_ID;
            docb.SPART = docPadre.SPART;
            docb.TALL_ID = docPadre.TALL_ID;
            docb.TIPO_CAMBIO = docPadre.TIPO_CAMBIO;
            docb.TIPO_CAMBIOL = docPadre.TIPO_CAMBIOL;
            docb.TIPO_CAMBIOL2 = docPadre.TIPO_CAMBIOL2;
            docb.TIPO_RECURRENTE = docPadre.TIPO_RECURRENTE;
            docb.TIPO_TECNICO = docPadre.TIPO_TECNICO;

            TSOL tsol = db1.TSOLs.Where(x => x.TSOLM == docPadre.TSOL_ID).FirstOrDefault();
            if (tsol == null)
                docb.TSOL_ID = docPadre.TSOL_ID;
            else
                docb.TSOL_ID = tsol.ID;
            docb.USUARIOC_ID = docPadre.USUARIOC_ID;
            docb.VKORG = docPadre.VKORG;
            docb.VTWEG = docPadre.VTWEG;

            if (docPadre.DOCUMENTOPs != null)
                foreach (DOCUMENTOP dp in docPadre.DOCUMENTOPs.ToList())
                {
                    DOCUMENTOBORRP docBp = new DOCUMENTOBORRP();
                    docBp.APOYO_EST = dp.APOYO_EST;
                    docBp.APOYO_REAL = dp.APOYO_REAL;
                    docBp.CANTIDAD = dp.CANTIDAD;
                    docBp.MATKL = dp.MATKL;
                    docBp.MATNR = dp.MATNR.TrimStart('0');
                    docBp.MONTO = dp.MONTO;
                    docBp.MONTO_APOYO = dp.MONTO_APOYO;
                    docBp.PORC_APOYO = dp.PORC_APOYO;
                    docBp.POS = dp.POS;
                    docBp.PRECIO_SUG = dp.PRECIO_SUG;
                    docBp.VIGENCIA_AL = dp.VIGENCIA_AL;
                    docBp.VIGENCIA_DE = dp.VIGENCIA_DE;
                    docBp.VOLUMEN_EST = dp.VOLUMEN_EST;
                    docBp.VOLUMEN_REAL = dp.VOLUMEN_REAL;

                    docb.DOCUMENTOBORRPs.Add(docBp);
                }

            if (docPadre.DOCUMENTOFs != null)
                foreach (DOCUMENTOF df in docPadre.DOCUMENTOFs)
                {
                    DOCUMENTOBORRF docBf = new DOCUMENTOBORRF();
                    docBf.ACTIVO = true;
                    docBf.AUTORIZACION = df.AUTORIZACION;
                    docBf.BELNR = df.BELNR;
                    docBf.BILL_DOC = df.BILL_DOC;
                    docBf.CONTROL = df.CONTROL;
                    docBf.EJERCICIOK = df.EJERCICIOK;
                    docBf.FACTURA = df.FACTURA;
                    docBf.FACTURAK = df.FACTURAK;
                    docBf.FECHA = df.FECHA;
                    docBf.IMPORTE_FAC = df.IMPORTE_FAC;
                    docBf.PAYER = df.PAYER;
                    docBf.POS = df.POS;
                    docBf.PROVEEDOR = df.PROVEEDOR;
                    docBf.SOCIEDAD = df.SOCIEDAD;
                    docBf.VENCIMIENTO = df.VENCIMIENTO;

                    docb.DOCUMENTOBORRFs.Add(docBf);
                }

            if (docPadre.DOCUMENTORECs != null)
                foreach (DOCUMENTOREC dre in docPadre.DOCUMENTORECs)
                {
                    DOCUMENTOBORRREC docRe = new DOCUMENTOBORRREC();
                    docRe.DOC_REF = dre.DOC_REF;
                    docRe.EJERCICIO = dre.EJERCICIO;
                    docRe.ESTATUS = dre.ESTATUS;
                    docRe.FECHAF = dre.FECHAV;
                    docRe.FECHAV = dre.FECHAV;
                    docRe.MONTO_BASE = dre.MONTO_BASE;
                    docRe.MONTO_FIJO = dre.MONTO_FIJO;
                    docRe.MONTO_GRS = dre.MONTO_GRS;
                    docRe.MONTO_NET = dre.MONTO_NET;
                    docRe.PERIODO = dre.PERIODO;
                    docRe.PORC = dre.PORC;
                    docRe.POS = dre.POS;

                    docb.DOCUMENTOBORRRECs.Add(docRe);
                }
            return docb;
        }
    }
}