using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;
using WhyNotRun.Models;

namespace WhyNotRun.DAO
{
    public class TechieDAO : ContextAsyncDAO<Techie>
    {
        public TechieDAO() : base()
        {

        }

        /// <summary>
        /// Cria uma nova Techie
        /// </summary>
        /// <param name="techie"></param>
        /// <returns></returns>
        public async Task CreateTechie(Techie techie)
        {
            await Collection.InsertOneAsync(techie);
        }

        /// <summary>
        /// Faz a busca de uma Techie pelo Id
        /// </summary>
        /// <param name="id"> Id da Techie</param>
        /// <returns> retorna uma Techie </returns>
        public async Task<Techie> SearchTechiePerId(ObjectId id)
        {
            var filter = FilterBuilder.Eq(a => a.Id, id)
                & FilterBuilder.Exists(a => a.DeletedAt, false);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Faz a busca de uma lista de techies baseado nos id's
        /// </summary>
        /// <param name="ids">Ids para busca</param>
        /// <returns></returns>
        public async Task<List<Techie>> SearchTechies(List<ObjectId> ids)
        {
            var filter = FilterBuilder.In(a => a.Id, ids) & FilterBuilder.Exists(a => a.DeletedAt, false);
            return await Collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Busca uma tecnologia por nome
        /// </summary>
        /// <param name="name">texto a ser procurado</param>
        /// <returns></returns>
        public async Task<Techie> SearchTechiePerName(string name)
        {
            var filter = FilterBuilder.Where(a => a.Name.ToLower() == name.ToLower())
                & FilterBuilder.Exists(a => a.DeletedAt, false);

            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Busca uma lista de tecnologias que o nome seja parecido com o texto digitado 
        /// </summary>
        /// <param name="name">texto a ser procurado</param>
        /// <returns></returns>
        public async Task<List<Techie>> SearchTechiesPerName(string name)
        {
            var filter = FilterBuilder.Regex(a => a.Name, BsonRegularExpression.Create(new Regex(name, RegexOptions.IgnoreCase)))
                & FilterBuilder.Exists(a => a.DeletedAt, false);
            return await Collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Lista as Techies não deletadas
        /// </summary>
        /// <returns> Retorna uma lista de Techies</returns>
        public async Task<List<Techie>> ListTechies(int page)
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false);

            return await Collection
                .Find(filter)
                .Skip((page - 1) * UtilBO.QUANTIDADE_PAGINAS)
                .Limit(UtilBO.QUANTIDADE_PAGINAS)
                .ToListAsync();
        }

        public async Task<List<Techie>> List()
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false);

            return await Collection
                .Find(filter)
                .ToListAsync();
        }



        /// <summary>
        /// Sugere uma tecnologia com base em uma palavra chave
        /// </summary>
        /// <param name="text">palavra chave</param>
        /// <returns></returns>
        public async Task<List<Techie>> SugestTechie(string text)
        {
            var filter = FilterBuilder.Regex(a => a.Name, BsonRegularExpression.Create(new Regex(text, RegexOptions.IgnoreCase)))
                & FilterBuilder.Exists(a => a.DeletedAt, false);
            var sort = SortBuilder.Ascending(a => a.Name);

            return await Collection
                .Find(filter)
                .Sort(sort)
                .Limit(5)
                .ToListAsync();
        }

        /// <summary>
        /// Lista tecnologias ordenando por maior quantidade de postagens
        /// </summary>
        /// <param name="page">pagina da paginação</param>
        /// <returns></returns>
        public async Task<List<ObjectId>> ListTechiesPerPosts(int page)
        {
            var project = new BsonDocument()
                 .Add("item", 1)
                 .Add("posts", new BsonDocument{
                    { "$size", "$publications"}}
                 );
            
            var result = await Collection
                .Aggregate()
                .Lookup("publication", "_id", "techies", "publications")
                .Project(project)
                .Sort("{posts : -1 }")
                .Skip((page - 1) * UtilBO.QUANTIDADE_PAGINAS)
                .Limit(UtilBO.QUANTIDADE_PAGINAS)
                .ToListAsync();

            List<ObjectId> techiesId = new List<ObjectId>();
            foreach (var item in result)
            {
                techiesId.Add(item["_id"].ToString().ToObjectId());
            }
            return techiesId;
            
        }

        /// <summary>
        /// Lista tecnologias por maior quantidade de pontos
        /// </summary>
        /// <param name="page">Pagina da paginação</param>
        /// <returns></returns>
        //public async Task<List<ObjectId>> ListTechiesPerPoints(int page)
        //{
        //    var project = new BsonDocument()
        //         .Add("item", 1)
        //         .Add("posts", new BsonDocument{
        //            { "$size", "$publications"}}
        //         );

        //    var result = await Collection
        //        .Aggregate()
        //        .Lookup("publication", "_id", "techies", "publications")
        //        .Project(project)
        //        .Sort("{posts : -1 }")
        //        .Skip((page - 1) * UtilBO.QUANTIDADE_PAGINAS)
        //        .Limit(UtilBO.QUANTIDADE_PAGINAS)
        //        .ToListAsync();

        //    List<ObjectId> techiesId = new List<ObjectId>();
        //    foreach (var item in result)
        //    {
        //        techiesId.Add(item["_id"].ToString().ToObjectId());
        //    }
        //    return techiesId;

        //}

    }
}