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
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [Display(Name = "Nome")]
        [BsonElement("nome")]
        public string nome { get; set; }

        [Display(Name = "Email")]
        [BsonElement("email")]
        [DataType(DataType.EmailAddress)]

        public string email { get; set; }

        [BsonIgnore]
        [Display(Name = "Confirmar Email")]
        [Compare("email", ErrorMessage = "Os campos de email precisam ser iguais!")]
        [DataType(DataType.Password)]


        public string ConfirmEmail { get; set; }

        [Display(Name = "Data de Nascimento")]
        [BsonElement("dtNascimento")]
        [DataType(DataType.Date)]

        public string dtNascimento { get; set; }

        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        [Display(Name = "CPF")]
        [BsonElement("cpf")]
        public string cpf { get; set; }

        [Display(Name = "Sexo")]
        [BsonElement("sexo")]

        public string sexo { get; set; }

        [Display(Name = "Senha")]
        [BsonElement("senha")]
        [DataType(DataType.Password)]
        public string senha { get; set; }

        [BsonIgnore]
        [Display(Name = "Confirmar Senha")]
        [Compare("senha", ErrorMessage = "Os dois campos de senhas precisam ser iguais!")]
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }

        [BsonElement("Perfil")]
        public string Perfil { get; set; }

    }
}