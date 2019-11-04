using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIDI.Models.Commom
{
    public class MongoPictureModel
    {
        public ObjectId _id { get; set; }
        public string FileName { get; set; }
        public string PictureDataAsString { get; set; }
    }
}