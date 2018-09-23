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
    public class TechieBO
    {
        private TechieDAO _techieDao;

        public TechieBO()
        {
            _techieDao = new TechieDAO();
        }

        public async Task<Techie> CreateTechie(Techie techie)
        {
            techie.Id = ObjectId.GenerateNewId();
            await _techieDao.CreateTechie(techie);
            return techie;
        }

        public async Task<Techie> SearchTechie(ObjectId id)
        {
            return await _techieDao.SearchTechiePerId(id);
        }

        protected async Task<List<Techie>> SearchTechies(List<ObjectId> ids)
        {
            return await _techieDao.SearchTechies(ids);
        }

        public async Task<Techie> SearchTechiePerName(string name)
        {
            return await _techieDao.SearchTechiePerName(name);
        }

        public async Task<List<Techie>> SearchTechiesPerName(string name)
        {
            return await _techieDao.SearchTechiesPerName(name);
        }
        
        public async Task<List<Techie>> ListTechie(int page, string order)
        {
            if (order == "name")
            {
                return await _techieDao.ListTechies(page);
            }
            else if (order == "posts")
            {
                var result = await _techieDao.ListTechiesPerPosts(page);
                if (result != null)
                {
                    return await SearchTechies(result);
                }
                return null;
            }
            else
            {
                return await _techieDao.List(); //mudar para o de pontos
            }
        }
        
        public async Task<List<Techie>> SugestTechie(string text)
        {
            if (text == null)
            {
                return (await ListTechie(1, "posts")).Take(5).ToList();
            }
            var result = await _techieDao.SugestTechie(text);

            if (!result.Any())
            {
                return (await ListTechie(1, "posts")).Take(5).ToList();
            }

            return result; 
        }

        public async Task<List<Techie>> ListAllTechies()
        {
            return await _techieDao.List();
        }

    }
}