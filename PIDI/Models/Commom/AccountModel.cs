using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PIDI.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIDI.Models
{
    public class AccountModel
    {

        [Required]
        [Display(Name = "Email")]
        [BsonElement("email")]
        [DataType(DataType.EmailAddress)]

        public string email { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [BsonElement("senha")]
        [DataType(DataType.Password)]
        public string senha { get; set; }

    }
}