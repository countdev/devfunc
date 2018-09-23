using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
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
    public class PublicationDAO : ContextAsyncDAO<Publication>
    {
        public PublicationDAO() : base()
        {

        }

        public async Task<List<Publication>> ListPublications()
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false);
            var sort = SortBuilder.Descending(a => a.DateCreation);
            var projection = ProjectionBuilder.Slice(a => a.Comments, 0, 3);


            return await Collection
                .Find(filter)
                .Sort(sort)
                .Project<Publication>(projection)
                .ToListAsync();
        }

        /// <summary>
        /// Lista todas as publicações
        /// </summary>
        /// <returns>Lista de publicações</returns>
        public async Task<List<Publication>> ListPublications(int page)
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false);
            var sort = SortBuilder.Descending(a => a.DateCreation);
            var projection = ProjectionBuilder.Slice(a => a.Comments, 0, 3);


            return await Collection
                .Find(filter)
                .Sort(sort)
                .Skip((page - 1) * UtilBO.QUANTIDADE_PAGINAS)
                .Limit(UtilBO.QUANTIDADE_PAGINAS)
                .Project<Publication>(projection)
                .ToListAsync();
        }

        /// <summary>
        /// Busca publicações com base em uma palavra chave
        /// </summary>
        /// <param name="text">palavra chave</param>
        /// <param name="techiesId">Lista de tecnologias que a palavra chave se encaixa no nome</param>
        /// <param name="page">numero da pagina para paginação</param>
        /// <returns></returns>
        public async Task<List<Publication>> SearchPublications(string text, List<ObjectId> techiesId, int page)
        {
            var filter = FilterBuilder.Regex(a => a.Title, BsonRegularExpression.Create(new Regex(text, RegexOptions.IgnoreCase)))
                | FilterBuilder.Regex(a => a.Description, BsonRegularExpression.Create(new Regex(text, RegexOptions.IgnoreCase)))
                | FilterBuilder.AnyIn(a => a.Techies, techiesId)
                & FilterBuilder.Exists(a => a.DeletedAt, false);

            var sort = SortBuilder.Descending(a => a.DateCreation);
            var projection = ProjectionBuilder.Slice(a => a.Comments, 0, 3);

            return await Collection
                .Find(filter)
                .Sort(sort)
                .Skip((page - 1) * UtilBO.QUANTIDADE_PAGINAS)
                .Limit(UtilBO.QUANTIDADE_PAGINAS)
                .Project<Publication>(projection)
                .ToListAsync();
        }

        /// <summary>
        /// Sugere uma publicação para o usuario com base em uma palavra chave
        /// </summary>
        /// <param name="text">palavra chave</param>
        /// <param name="techiesId">lista de tecnologias que se encaixam nessa palavra chave</param>
        /// <returns></returns>
        public async Task<List<ObjectId>> SugestPublication(string text, List<ObjectId> techiesId)
        {
            var filter = FilterBuilder.Regex(a => a.Title, BsonRegularExpression.Create(new Regex(text, RegexOptions.IgnoreCase)))
                | FilterBuilder.Regex(a => a.Description, BsonRegularExpression.Create(new Regex(text, RegexOptions.IgnoreCase)))
                | FilterBuilder.AnyIn(a => a.Techies, techiesId)
                & FilterBuilder.Exists(a => a.DeletedAt, false);

            var project = new BsonDocument()
                .Add( "item", 1 )
                .Add("points", new BsonDocument{
                    { "$subtract", new BsonArray{
                        new BsonDocument{
                            { "$size", "$likes" }
                        },
                        new BsonDocument{
                            { "$size", "$dislikes" }
                        }
                    }}
                }
                );
            
            
            var result = await Collection
                .Aggregate()
                .Match(filter)
                .Project(project)
                .Sort("{points : -1 }")
                .Limit(7)
                .ToListAsync();

            List<ObjectId> publicationsId = new List<ObjectId>();
            foreach (var item in result)
            {
                publicationsId.Add(item["_id"].ToString().ToObjectId());
            }
            return publicationsId;
        }
        
        /// <summary>
        /// Cria uma publicação
        /// </summary>
        /// <param name="publication">Publicação a ser criada</param>
        /// <returns></returns>
        public async Task CreatePublication(Publication publication)
        {
            await Collection.InsertOneAsync(publication);
        }

        /// <summary>
        /// Curti uma publicação
        /// </summary>
        /// <param name="userId">Usuario que curtiu</param>
        /// <param name="publicationId">publicação curtida</param>
        public async Task<bool> Like(ObjectId userId, ObjectId publicationId)
        {
            var filter = FilterBuilder.Eq(a => a.Id, publicationId) & FilterBuilder.Exists(a => a.DeletedAt, false);
            var update = UpdateBuilder.Push(a => a.Likes, userId).Pull(a => a.Dislikes, userId);

            var resultado = await Collection.UpdateOneAsync(filter, update);

            return resultado.IsModifiedCountAvailable && resultado.IsAcknowledged && resultado.ModifiedCount == 1;
        }

        /// <summary>
        /// Da dislike em uma publicação
        /// </summary>
        /// <param name="userId">Usuario que deu dislike</param>
        /// <param name="publicationId">publicação que recebeu dislike</param>
        public async Task<bool> Dislike(ObjectId userId, ObjectId publicationId)
        {
            var filter = FilterBuilder.Eq(a => a.Id, publicationId) & FilterBuilder.Exists(a => a.DeletedAt, false);
            var update = UpdateBuilder.Push(a => a.Dislikes, userId).Pull(a => a.Likes, userId);

            var resultado = await Collection.UpdateOneAsync(filter, update);
            return resultado.IsModifiedCountAvailable && resultado.IsAcknowledged && resultado.ModifiedCount == 1;
        }

        /// <summary>
        /// Remove a like or a dislike from a publication
        /// </summary>
        /// <param name="userId">UserId to be removed</param>
        /// <param name="publicationId">id of the publication</param>
        /// <returns></returns>
        public async Task<bool> RemoveLikeAndDislike(ObjectId userId, ObjectId publicationId)
        {
            var filter = FilterBuilder.Eq(a => a.Id, publicationId) & FilterBuilder.Exists(a => a.DeletedAt, false);
            var update = UpdateBuilder.Pull(a => a.Likes, userId).Pull(a => a.Dislikes, userId);

            var resultado = await Collection.UpdateOneAsync(filter, update);

            return resultado.IsModifiedCountAvailable && resultado.IsAcknowledged && resultado.ModifiedCount == 1;
        }

        /// <summary>
        /// Busca uma publicação baseado no ID
        /// </summary>
        /// <param name="publicationId">ID da publicação a ser buscada</param>
        /// <returns></returns>
        public async Task<Publication> SearchPublicationById(ObjectId publicationId)
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false) & FilterBuilder.Eq(a => a.Id, publicationId);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Busca uma lista de publicações baseado nos id's
        /// </summary>
        /// <param name="ids">id's das publicações a serem buscadas</param>
        /// <returns></returns>
        public async Task<List<Publication>> SearchPublicationsByIds(List<ObjectId> ids)
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false) & FilterBuilder.In(a => a.Id, ids);
            return await Collection.Find(filter).ToListAsync();
        }
        
        /// <summary>
        /// Adiciona um comentario a uma publicação
        /// </summary>
        /// <param name="comment">Comentario a ser adicionado</param>
        /// <param name="publicationId">publicação que vai receber o comentario</param>
        public async Task<bool> AddComment(Comment comment, ObjectId publicationId)
        {
            var filter = FilterBuilder.Eq(a => a.Id, publicationId) & FilterBuilder.Exists(a => a.DeletedAt, false);
            var update = UpdateBuilder.Push(a => a.Comments, comment);

            var resultado = await Collection.UpdateOneAsync(filter, update);

            return resultado.IsModifiedCountAvailable && resultado.IsAcknowledged && resultado.ModifiedCount == 1;

        }

        /// <summary>
        /// Retorna mais comentarios de uma publicação especifica
        /// </summary>
        /// <param name="publicationId">Id da publicação</param>
        /// <param name="lastCommentId">Id do comentario a ser usado de base para listagem dos proximos</param>
        /// <param name="limit">quantidade a ser carregada</param>
        /// <returns></returns>
        public async Task<List<Comment>> SeeMoreComments(ObjectId publicationId, ObjectId lastCommentId, int limit)
        {

            var match = new BsonDocument
            {
                {
                    "deletedAt",
                    new BsonDocument {
                        { "$exists", false}
                    }
                },
                {
                    "_id",
                    publicationId
                }
            };

            var projection = new BsonDocument
            {
                {"comments", 1 },
                {"_id", 0 }
            };

            var mathComments = new BsonDocument
            {
                {
                    "comments._id",
                    new BsonDocument
                    {
                        {"$gt", lastCommentId }
                    }
                }
            };

            var result = await
                Collection
                .Aggregate()
                .Match(match)
                .Unwind(a => a.Comments)
                .Project(projection)
                .Match(mathComments)
                .Limit(limit)
                .ToListAsync();

            List<Comment> comentarios = new List<Comment>();
            foreach (var item in result)
            {
                comentarios.Add(BsonSerializer.Deserialize<Comment>(item["comments"].AsBsonDocument));
            }
            return comentarios.ToList();
        }

        /// <summary>
        /// Lista publicações com base em uma tecnologia
        /// </summary>
        /// <param name="techieId">id da tecnologia a ser usada como base</param>
        public async Task<List<Publication>> ListPublicationsPerTechieId(ObjectId techieId)
        {
            var filter = FilterBuilder.Exists(a => a.DeletedAt, false) & FilterBuilder.AnyEq(a => a.Techies, techieId);
            var projection = ProjectionBuilder.Slice(a => a.Comments, 0, 1); //Projeta 0 comentarios pois não vai mostrar a publicação, vai apenas calcular os dados dela então carregar os comentarios é desnecessario
            return await Collection
                .Find(filter)
                .Project<Publication>(projection)
                .ToListAsync();
        }
        
    }
}