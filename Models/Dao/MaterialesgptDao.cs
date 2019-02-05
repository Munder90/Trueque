using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Services;

namespace TAT001.Models.Dao
{
    public class MaterialesgptDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public List<MATERIALGPT> ListaMaterialGroupsCliente(string vkorg, string spart, string kunnr, string soc_id, int aii, int mii, int aff, int mff)
        {
            db.Database.CommandTimeout = 180;
            if (!db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == soc_id))
            {
                return new List<MATERIALGPT>();
            }
            List<MATERIALGPT> materialgp = db.Database.SqlQuery<MATERIALGPT>("CPS_LISTA_MATERIALGP_CLIENTE @SOCIEDAD_ID,@VKORG,@SPART,@KUNNR,@aii,@mii,@aff,@mff",
               new SqlParameter("@SOCIEDAD_ID", soc_id),
              new SqlParameter("@VKORG", vkorg),
              new SqlParameter("@SPART", spart),
              new SqlParameter("@KUNNR", kunnr),
              new SqlParameter("@aii", aii),
              new SqlParameter("@mii", mii),
              new SqlParameter("@aff", aff),
              new SqlParameter("@mff", mff)).ToList();
            return materialgp;
        }
        public List<DOCUMENTOM_MOD> ListaMaterialGroupsMateriales(string vkorg, string spart, string kunnr, string soc_id, int aii, int mii, int aff, int mff, string user_id)
        {
            if (!db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == soc_id))
            {
                return new List<DOCUMENTOM_MOD>();
            }
            db.Database.CommandTimeout = 180;
            string spras_id = FnCommon.ObtenerSprasId(db, user_id);
            List<DOCUMENTOM_MOD> materialgp = db.Database.SqlQuery<DOCUMENTOM_MOD>("CPS_LISTA_MATERIALGP_MATERIALES @SOCIEDAD_ID,@VKORG,@SPART,@KUNNR,@SPRAS_ID,@aii,@mii,@aff,@mff",
              new SqlParameter("@SOCIEDAD_ID", soc_id),
              new SqlParameter("@VKORG", vkorg),
              new SqlParameter("@SPART", spart),
              new SqlParameter("@KUNNR", kunnr),
              new SqlParameter("@SPRAS_ID", spras_id),
              new SqlParameter("@aii", aii),
              new SqlParameter("@mii", mii),
              new SqlParameter("@aff", aff),
              new SqlParameter("@mff", mff)).ToList();
            return materialgp;
        }

        public List<MATERIALGPT> CategoriasCliente(string vkorg, string spart, string kunnr, string soc_id)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);
            if (kunnr == null)
            {
                kunnr = "";
            }
            List<MATERIALGPT> jd = new List<MATERIALGPT>();



            //Validar si hay materiales
            if (db.MATERIALs.Any(x => x.MATERIALGP_ID != null && x.ACTIVO.Value))
            {
                List<CLIENTE> clil = new List<CLIENTE>();

                try
                {
                    CLIENTE cli = db.CLIENTEs.Where(c => c.KUNNR == kunnr && c.VKORG == vkorg && c.SPART == spart).FirstOrDefault();
                    //Saber si el cliente es sold to, payer o un grupo  //Es un soldto
                    if (cli != null && cli.KUNNR != cli.PAYER && cli.KUNNR != cli.BANNER)
                    {
                        clil.Add(cli);
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "Listas", "categoriasCliente");
                }

                var cie = clil;
                //Obtener el numero de periodos para obtener el historial
                int? mesesVenta = (db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == soc_id) ? db.CONFDIST_CAT.First(x => x.SOCIEDAD_ID == soc_id).PERIODOS : null);
                int nummonths = (mesesVenta != null ? mesesVenta.Value : DateTime.Now.Month);
                int imonths = nummonths * -1;
                //Obtener el rango de los periodos incluyendo el año
                DateTime ff = DateTime.Today;
                DateTime fi = ff.AddMonths(imonths);

                string mi = fi.Month.ToString();
                string ai = fi.Year.ToString();

                string mf = ff.Month.ToString();
                string af = ff.Year.ToString();

                int aii = 0;
                int mii = 0;
                int aff = 0;
                int mff = 0;
                try
                {
                    aii = Convert.ToInt32(ai);
                    mii = Convert.ToInt32(mi);
                    aff = Convert.ToInt32(af);
                    mff = Convert.ToInt32(mf);
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "Listas", "categoriasCliente-mesesVenta");
                }

                if (cie != null)
                {
                    jd = ListaMaterialGroupsCliente( vkorg, spart, kunnr, soc_id, aii, mii, aff, mff);
                }
            }

            var list = new List<MATERIALGPT>();
            if (jd.Count > 0)
            {
                MATERIALGPT c = FnCommon.ObtenerTotalProducts(db);
                list.Add(new MATERIALGPT
                {
                    MATERIALGP_ID = c.MATERIALGP_ID,
                    TXT50 = c.TXT50
                });
                list.AddRange(jd);
            }
            return list;
        }
        public List<CategoriaMaterial> GrupoMateriales(string vkorg, string spart, string kunnr, string soc_id,string usuario_id)
        {

            if (kunnr == null)
            {
                kunnr = "";
            }

            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            List<DOCUMENTOM_MOD> jd = new List<DOCUMENTOM_MOD>();


            //Validar si hay materiales
            if (db.MATERIALs.Any(x => x.MATERIALGP_ID != null && x.ACTIVO.Value))
            {
                List<CLIENTE> clil = new List<CLIENTE>();
                try
                {
                    CLIENTE cli = db.CLIENTEs.Where(c => c.KUNNR == kunnr && c.VKORG == vkorg && c.SPART == spart).FirstOrDefault();

                    //Saber si el cliente es sold to, payer o un grupo
                    if (cli != null && cli.KUNNR != cli.PAYER && cli.KUNNR != cli.BANNER)
                    {
                        clil.Add(cli);
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "Listas", "grupoMateriales");
                }
                var cie = clil;
                //Obtener el numero de periodos para obtener el historial
                int? mesesVenta = (db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == soc_id) ? db.CONFDIST_CAT.First(x => x.SOCIEDAD_ID == soc_id).PERIODOS : null);
                int nummonths = (mesesVenta != null ? mesesVenta.Value : DateTime.Now.Month);
                int imonths = nummonths * -1;
                //Obtener el rango de los periodos incluyendo el año
                DateTime ff = DateTime.Today;
                DateTime fi = ff.AddMonths(imonths);

                string mi = fi.Month.ToString();
                string ai = fi.Year.ToString();

                string mf = ff.Month.ToString();
                string af = ff.Year.ToString();

                int aii = 0;
                int mii = 0;
                int aff = 0;
                int mff = 0;
                try
                {
                    aii = Convert.ToInt32(ai);
                    mii = Convert.ToInt32(mi);
                    aff = Convert.ToInt32(af);
                    mff = Convert.ToInt32(mf);
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "Listas", "grupoMateriales-mesesVenta");
                }


                if (cie != null)
                {
                    jd = ListaMaterialGroupsMateriales(vkorg, spart, kunnr, soc_id, aii, mii, aff, mff, usuario_id);
                }
            }

            //Obtener las categorías
            var categorias = jd.GroupBy(c => c.ID_CAT, c => new { ID = c.ID_CAT.ToString(), DESC = c.DESC }).ToList();

            List<CategoriaMaterial> lcatmat = new List<CategoriaMaterial>();

            foreach (var item in categorias)
            {
                CategoriaMaterial cm = new CategoriaMaterial();
                cm.ID = item.Key;
                cm.EXCLUIR = jd.FirstOrDefault(x => x.ID_CAT.Equals(item.Key)).EXCLUIR; //RSG 09.07.2018 ID167

                //Obtener los materiales de la categoría
                List<DOCUMENTOM_MOD> dl;
                List<DOCUMENTOM_MOD> dm = new List<DOCUMENTOM_MOD>();
                dl = jd.Where(c => c.ID_CAT == item.Key).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, DESC = c.DESC }).ToList();//Falta obtener el groupby

                //Obtener la descripción de los materiales
                foreach (DOCUMENTOM_MOD d in dl)
                {
                    DOCUMENTOM_MOD dcl;
                    dcl = dm.Where(z => z.MATNR == d.MATNR).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, DESC = c.DESC }).FirstOrDefault();

                    if (dcl == null)
                    {
                        DOCUMENTOM_MOD dcll = new DOCUMENTOM_MOD();
                        //No se ha agregado
                        decimal val = dl.Where(y => y.MATNR == d.MATNR).Sum(x => x.VAL);
                        dcll.ID_CAT = item.Key;
                        dcll.MATNR = d.MATNR;

                        dcll.DESC = d.DESC;
                        dcll.VAL = val;
                        cm.TOTALCAT += val;//ADD RSG 22.11.2018

                        dm.Add(dcll);
                    }
                }

                cm.MATERIALES = dm;
                //LEJ 18.07.2018-----------------------------------------------------------
                MATERIALGP vv = FnCommon.ObtenerMaterialGroup(db, cm.ID);
                cm.UNICA = vv.UNICA;
                cm.DESCRIPCION = vv.DESCRIPCION;
                lcatmat.Add(cm);
            }

            if (lcatmat.Count > 0)
            {
                CategoriaMaterial nnn = new CategoriaMaterial();
                nnn.ID = "000";
                nnn.DESCRIPCION = FnCommon.ObtenerTotalProducts(db).TXT50;
                nnn.MATERIALES = new List<DOCUMENTOM_MOD>();
                nnn.TOTALCAT = 0;//ADD RSG 22.11.2018
                //foreach (var item in lcatmat)//RSG 09.07.2018 ID167
                foreach (var item in lcatmat.Where(x => !x.EXCLUIR).ToList())
                {
                    foreach (var ii in item.MATERIALES)
                    {
                        DOCUMENTOM_MOD dm = new DOCUMENTOM_MOD();
                        dm.ID_CAT = "000";
                        dm.DESC = ii.DESC;
                        dm.MATNR = ii.MATNR;
                        dm.POR = ii.POR;
                        dm.VAL = ii.VAL;
                        nnn.TOTALCAT += ii.VAL;
                        nnn.MATERIALES.Add(dm);
                    }
                }
                //LEJ 18.07.2018-----------------------------------------------------------
                nnn.UNICA = FnCommon.ObtenerMaterialGroup(db, nnn.ID).UNICA;
                lcatmat.Add(nnn);
            }

            
            return lcatmat;
        }
    }
}