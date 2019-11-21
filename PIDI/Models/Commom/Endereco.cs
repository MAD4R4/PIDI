using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIDI.Models.Commom
{
    public class Endereco
    {
        public string NomeEndereco { get; set; }
        public string cep { get; set; }
        public string rua { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
    }
}