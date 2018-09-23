using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.Models;

namespace WhyNotRun.DAO
{

    public class UserDAO : ContextAsyncDAO<User>
    {
        public UserDAO() : base()
        {

        }

        public async Task CreateUser(User user)
        {
            await Collection.InsertOneAsync(user);
        }

        public async Task<bool> ValidEmailExists(string email)
        {
            var filter = FilterBuilder.Eq(a => a.Email, email)
                & FilterBuilder.Exists(a => a.DeletedAt, false);

            return await Collection.Find(filter).FirstOrDefaultAsync() == null;
        }

        public async Task<User> SearchUserPerId(ObjectId id)
        {
            var filter = FilterBuilder.Eq(a => a.Id, id)
                & FilterBuilder.Exists(a => a.DeletedAt, false);

            var result = await Collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<User> Login(string email, string password)
        {
            var filter = FilterBuilder.Eq(a => a.Email, email)
                & FilterBuilder.Eq(a => a.Password, password)
                & FilterBuilder.Exists(a => a.DeletedAt, false);

            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveImage(ObjectId userId, string picturePath)
        {
            var filter = FilterBuilder.Eq(a => a.Id, userId)
                & FilterBuilder.Exists(a => a.DeletedAt, false);
            var update = UpdateBuilder.Set(a => a.Picture, picturePath);

            var resultado = await Collection.UpdateOneAsync(filter, update);

            return resultado.IsModifiedCountAvailable && resultado.IsAcknowledged && resultado.ModifiedCount == 1;
        }
    }

}