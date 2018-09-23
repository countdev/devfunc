using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.DAO;
using WhyNotRun.Models;

namespace WhyNotRun.BO
{
    public class UserBO
    {
        private const string PICTURE_DEFAULT = "picture-pattern.jpg";

        private UserDAO _userDao;

        public UserBO()
        {
            _userDao = new UserDAO();
        }

        public async Task<User> CreateUser(User user)
        {
            user.Id = ObjectId.GenerateNewId();
            user.Password = user.Password.Encript();
            var emailValid = await ValidMailExists(user.Email);
            if (emailValid)
            {
                await _userDao.CreateUser(user);
                return user;
            }
            return null;
        }

        public async Task<bool> ValidMailExists(string email)
        {
            return await _userDao.ValidEmailExists(email);
        }

        public async Task<User> SearchUserPerId(ObjectId id)
        {
            return await _userDao.SearchUserPerId(id);
        }

        public async Task<User> Login(string email, string password)
        {
            var user = await _userDao.Login(email, password.Encript());
            if (user != null)
            {
                return await SearchUserPerId(user.Id);
            }
            return null;
        }

        public async Task<bool> SaveImage(ObjectId userId, string picturePath)
        {
            var newPath = "https://whynotrun.blob.core.windows.net/images/" + picturePath;
            return await _userDao.SaveImage(userId, newPath);
        }
    }
}