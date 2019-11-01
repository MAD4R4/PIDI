using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIDI.Models.Admin
{
    public class RoleModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [Required]
        [BsonElement("RoleName")]
        [Display(Name = "Nome Perfil")]

        public string RoleName { get; set; }
    }
}