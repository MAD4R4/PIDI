using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PIDI.Models.Commom
{
    public class Endereco
    {
        public string NomeEndereco { get; set; }
        [BsonElement("cep")]
        [Display(Name = "CEP")]
        public string cep { get; set; }
        [BsonElement("rua")]
        [Display(Name = "Rua")]
        public string rua { get; set; }
        [BsonElement("bairro")]
        [Display(Name = "Bairro")]
        public string bairro { get; set; }
        [BsonElement("cidade")]
        [Display(Name = "Cidade")]
        public string cidade { get; set; }
        [BsonElement("uf")]
        [Display(Name = "UF")]
        public string uf { get; set; }
    }
}