using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using PIDI.App_Start;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PIDI.Models.Admin
{
    public class GalleryController : Controller
    {
        public ActionResult Index()
        {
            var theModel = GetThePictures();
            return View(theModel);
        }

        public ActionResult AddPicture()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPicture(HttpPostedFileBase theFile)
        {
            if (theFile.ContentLength > 0)
            {
                //get the file's name
                string theFileName = Path.GetFileName(theFile.FileName);

                //get the bytes from the content stream of the file
                byte[] thePictureAsBytes = new byte[theFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(theFile.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(theFile.ContentLength);
                }

                //convert the bytes of image data to a string using the Base64 encoding
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);

                //create a new mongo picture model object to insert into the db
                MongoPictureModel thePicture = new MongoPictureModel()
                {
                    FileName = theFileName,
                    PictureDataAsString = thePictureDataAsString
                };

                //insert the picture object
                bool didItInsert = InsertPictureIntoDatabase(thePicture);

                if (didItInsert)
                    ViewBag.Message = "The image was updated successfully";
                else
                    ViewBag.Message = "A database error has occurred";
            }
            else
                ViewBag.Message = "You must upload an image";

            return View();
        }

        public MongoPictureModel TransformImage(HttpPostedFileBase theFile)
        {
            if (theFile.ContentLength > 0)
            {
                //get the file's name 
                string theFileName = Path.GetFileName(theFile.FileName);

                //get the bytes from the content stream of the file
                byte[] thePictureAsBytes = new byte[theFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(theFile.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(theFile.ContentLength);
                }

                //convert the bytes of image data to a string using the Base64 encoding
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);

                //create a new mongo picture model object to insert into the db
                MongoPictureModel thePicture = new MongoPictureModel()
                {
                    FileName = theFileName,
                    PictureDataAsString = thePictureDataAsString
                };

                return thePicture;
            }
            else
                return null;

        }

        /// <summary>
        /// This method will insert the picture into the db
        /// </summary>
        /// <param name="thePicture"></param>
        /// <returns></returns>
        private bool InsertPictureIntoDatabase(MongoPictureModel thePicture)
        {
            var thePictureColleciton = pictureCollection;
            try
            {
                thePictureColleciton.InsertOne(thePicture);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// This method will return just the id's and filenames of the pictures 
        /// to use to retrieve the image from the db
        /// </summary>
        /// <returns></returns>
        private List<MongoPictureModel> GetThePictures()
        {
            var pictures = pictureCollection.AsQueryable<MongoPictureModel>().ToList();
            return pictures;
        }

        /// <summary>
        /// This action will return an image result to render the data from the picture as a jpeg
        /// </summary>
        /// <param name="id">id of the picture as a string</param>
        /// <returns></returns>
        public FileContentResult ShowPicture(string id)
        {
            var thePictureColleciton = pictureCollection;

            //get picture document from db
            var thePicture = thePictureColleciton.Find(x => x._id == new ObjectId(id)).SingleOrDefault();

            //transform the picture's data from string to an array of bytes
            var thePictureDataAsBytes = Convert.FromBase64String(thePicture.PictureDataAsString);

            //return array of bytes as the image's data to action's response. 
            //We set the image's content mime type to image/jpeg
            return new FileContentResult(thePictureDataAsBytes, "image/jpeg");
        }

        /// <summary>
        /// This will return the mongoDB picture collection object to use dor data related actions
        /// </summary>
        /// <returns></returns>

        private MongoDBContext server;
        IMongoCollection<MongoPictureModel> pictureCollection;
        public GalleryController()
        {
            server = new MongoDBContext();
            string databaseName = "PictureApplication";//replace with whatever name you choose
            pictureCollection = server.database.GetCollection<MongoPictureModel>(databaseName);
        }
    }
}
