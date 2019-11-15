using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace PIDI.Models.Commom
{
    public class MongoPictureModel
    {
        public MongoPictureModel()
        {
            id = ObjectId.GenerateNewId();
        }

        public ObjectId id { get; set; }
        public string FileName { get; set; }
        public string PictureDataAsString { get; set; }
    }
}