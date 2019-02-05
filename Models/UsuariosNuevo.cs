using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models
{
    public class UsuarioNuevo
    {
        public IEnumerable<TAT001.Entities.USUARIO> L { get; set; }
        public Usuario N { get; set; }
    }
}