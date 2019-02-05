using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class CartaV
    {
        [Key, Column(Order = 0)]
        public decimal num_doc { get; set; }
        [Key, Column(Order = 1)]
        public int pos { get; set; }
        public string company { get; set; }
        public bool company_x { get; set; }

        public string taxid { get; set; }
        public bool taxid_x { get; set; }

        public string concepto { get; set; }
        public bool concepto_x { get; set; }

        public string cliente { get; set; }
        public bool cliente_x { get; set; }

        public string puesto { get; set; }
        public bool puesto_x { get; set; }

        public string direccion { get; set; }
        public bool direccion_x { get; set; }

        public string folio { get; set; }
        public bool folio_x { get; set; }
        public string lugarFech { get; set; }
        public bool lugarFech_x { get; set; }
        public string lugar { get; set; }
        public bool lugar_x { get; set; }
        public string payerNom { get; set; }
        public bool payerNom_x { get; set; }
        public string payerId { get; set; }
        public bool payerId_x { get; set; }
        public string estimado { get; set; }
        public bool estimado_x { get; set; }

        public string mecanica { get; set; }
        public bool mecanica_x { get; set; }
        public string monto { get; set; }
        public bool monto_x { get; set; }

        public string nombreE { get; set; }
        public bool nombreE_x { get; set; }

        public string puestoE { get; set; }
        public bool puestoE_x { get; set; }

        public string companyC { get; set; }
        public bool companyC_x { get; set; }

        public string nombreC { get; set; }
        public bool nombreC_x { get; set; }

        public string puestoC { get; set; }
        public bool puestoC_x { get; set; }

        public string companyCC { get; set; }
        public bool companyCC_x { get; set; }

        public string legal { get; set; }
        public bool legal_x { get; set; }

        public string mail { get; set; }
        public bool mail_x { get; set; }

        public string comentarios { get; set; }
        public bool comentarios_x { get; set; }

        public string compromisoK { get; set; }
        public bool compromisoK_x { get; set; }

        public string compromisoC { get; set; }
        public bool compromisoC_x { get; set; }
        public string moneda { get; set; }
        ///////////////////////////////////////////////DATOS PARA LA TABLA DE MATERIALES////////////////////////////////////////////////////////////
        public List<string> listaFechas { get; set; }//RANGO DE FECHAS QUE DETERMINAN EL NUMERO DE TABLAS
        public List<int> numfilasTabla { get; set; }//NUMERO FILAS POR TABLA CALCULADA
        public List<string> listaCuerpo { get; set; }//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE QUE POSTERIORMENTE ES DISTRIBUIDA EN LAS TABLAS
        public List<listacuerpoc> listaCuerpom { get; set; }//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE QUE POSTERIORMENTE ES DISTRIBUIDA EN LAS TABLAS
        public List<DOCUMENTOP_MOD> DOCUMENTOP { get; set; } //B20180710 MGC 2018.07.13 Modificaciones para editar los campos de distribución
        public List<string> numColEncabezado { get; set; }//NUMERO DE COLUMNAS PARA EL ENCABEZADO
        public string material { get; set; }//TEXTO ENCABEZADO
        public bool material_x { get; set; }
        public string categoria { get; set; }//TEXTO ENCABEZADO
        public string descripcion { get; set; }//TEXTO ENCABEZADO
        public string costoun { get; set; }//TEXTO ENCABEZADO
        public bool costoun_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string apoyo { get; set; }//TEXTO ENCABEZADO
        public bool apoyo_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string apoyop { get; set; }//TEXTO ENCABEZADO
        public bool apoyop_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string costoap { get; set; }//TEXTO ENCABEZADO
        public bool costoap_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string precio { get; set; }//TEXTO ENCABEZADO
        public bool precio_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        //B20180726 MGC 2018.07.26
        //public string apoyoEst { get; set; }//TEXTO ENCABEZADO
        //public bool apoyoEst_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        //public string apoyoRea { get; set; }//TEXTO ENCABEZADO
        //public bool apoyoRea_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        //B20180726 MGC 2018.07.26
        public string volumen { get; set; }//TEXTO ENCABEZADO
        public bool volumen_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string apoyototal { get; set; }//TEXTO ENCABEZADO
        public bool apoyototal_x { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////DATOS PARA LA TABLA DE RECURRENCIAS DOCUMENTOREC///////////////////////////////////////////////
        public List<string> numColEncabezado2 { get; set; }//NUMERO DE COLUMNAS PARA EL ENCABEZADO
        public int numfilasTabla2 { get; set; }//NUMERO DE FILAS TOTAL DE LA TABLA
        public List<string> listaCuerpoRec { get; set; }//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
        public bool secondTab_x { get; set; }//PARA MOSTRAR O NO LA TABLA
        public string tipoDoc { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string fechaF { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string montoR { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string porcentajeR { get; set; }//PARA MOSTRAR O NO LA COLUMNA
        public string aplicado { get; set; }//PARA MOSTRAR O NO LA COLUMNA
                                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                            //[Key, Column(Order = 2)]

        /////////////////////////////TABLA OBJETIVOS Q/////////////////////////////////////
        public List<string> numColEncabezado3 { get; set; }//NUMERO DE COLUMNAS PARA EL ENCABEZADO
        public int numfilasTabla3 { get; set; }//NUMERO DE FILAS TOTAL DE LA TABLA
        public List<string> listaCuerpoObjQ { get; set; }//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
        public bool tercerTab_x { get; set; }
    }
}