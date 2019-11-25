using MongoDB.Bson;
using MongoDB.Driver;
using PIDI.App_Start;
using PIDI.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PIDI.Models
{
    public class Roles : RoleProvider
    {

        private MongoDBContext dBContext;
        private IMongoCollection<UserModel> userCollection;
        private IMongoCollection<RoleModel> rolesCollection;

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Roles()
        {
            dBContext = new MongoDBContext();
            userCollection = dBContext.database.GetCollection<UserModel>("users");
            rolesCollection = dBContext.database.GetCollection<RoleModel>("roles");
        }

      

        public override void CreateRole(string roleName)
        {
            var role = new RoleModel();
            role.RoleName = roleName;

            var query = rolesCollection.AsQueryable<RoleModel>().FirstOrDefault(x => x.RoleName == roleName);

            if(query == null)
                rolesCollection.InsertOne(role);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {

            throw new NotImplementedException();

        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string id)
        {
            ObjectId userID = new ObjectId(id);
            var sRoles = userCollection.AsQueryable<UserModel>().FirstOrDefault(x => x.Id == userID).Perfil;
            string[] retorno = { sRoles };
            return retorno;
        }
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}