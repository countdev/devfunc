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
    public class PublicationBO
    {
        private PublicationDAO _publicationDao;
        private UserBO _userBo;
        private TechieBO _techieBo;

        public PublicationBO()
        {
            _publicationDao = new PublicationDAO();
            _userBo = new UserBO();
            _techieBo = new TechieBO();
        }

        /// <summary>
        /// Listar publicações
        /// </summary>
        /// <returns>Lista de publicações</returns>
        public async Task<List<Publication>> ListPublications()
        {
            return await _publicationDao.ListPublications();
        }

        /// <summary>
        /// Listar publicações
        /// </summary>
        /// <returns>Lista de publicações</returns>
        public async Task<List<Publication>> ListPublications(int pagina)
        {
            return await _publicationDao.ListPublications(pagina);
        }

        /// <summary>
        /// Cadastra uma publicação
        /// </summary>
        /// <param name="publication"></param>
        /// <returns>Publicação cadastrada</returns>
        public async Task<Publication> CreatePublication(Publication publication)
        {
            publication.Id = ObjectId.GenerateNewId();
            publication.DateCreation = DateTime.Now;

            await _publicationDao.CreatePublication(publication);
            return publication;
        }

        /// <summary>
        /// Busca uma publicação por id
        /// </summary>
        /// <param name="publicationId">publicação a ser buscada</param>
        public async Task<Publication> SearchPublicationById(ObjectId publicationId)
        {
            return await _publicationDao.SearchPublicationById(publicationId);
        }

        /// <summary>
        /// Busca uma lista de publicações baseado nos id's
        /// </summary>
        /// <param name="ids">id's das publicações a serem buscadas</param>
        /// <returns></returns>
        public async Task<List<Publication>> SearchPublicationsByIds(List<ObjectId> ids)
        {
            return await _publicationDao.SearchPublicationsByIds(ids);
        }
        
        /// <summary>
        /// Reage a uma publicação
        /// </summary>
        /// <param name="userId">Usuario que reagiu</param>
        /// <param name="publicationId">publicação reagida</param>
        /// <param name="like">Reação (like = true, dislike = false)</param>
        public async Task<bool> React(ObjectId userId, ObjectId publicationId, bool? like)
        {
            var publicacao = await SearchPublicationById(publicationId);
            if (like == null)
            {
                return await _publicationDao.RemoveLikeAndDislike(userId, publicationId);
            }
            else
            {
                if (like == true)
                {
                    if (publicacao.Likes.Contains(userId))
                    {
                        return false;
                    }
                    return await _publicationDao.Like(userId, publicationId);
                }
                else
                {
                    if (publicacao.Dislikes.Contains(userId))
                    {
                        return false;
                    }
                    return await _publicationDao.Dislike(userId, publicationId);
                }
            }
            
        }

        /// <summary>
        /// Adiciona um comentario a uma publicação
        /// </summary>
        /// <param name="comment">Comentario a ser adicionado</param>
        /// <param name="publicationId">publicação que vai receber o comentario</param>
        public async Task<Comment> AddComment(Comment comment, ObjectId publicationId)
        {
            comment.Id = ObjectId.GenerateNewId();
            comment.DateCreation = DateTime.Now;

            var user = await _userBo.SearchUserPerId(comment.UserId);
            comment.UserName = user.Name;
            comment.UserPicture = user.Picture;
            comment.UserProfession = user.Profession;

            var result = await _publicationDao.AddComment(comment, publicationId);

            if (result)
            {
                return comment;
            }

            return null;

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
            return await _publicationDao.SeeMoreComments(publicationId, lastCommentId, limit);
        }

        /// <summary>
        /// Busca publicações por uma palavra chave
        /// </summary>
        /// <param name="text">palavra chave</param>
        /// <param name="page">pagina para paginação</param>
        /// <returns></returns>
        public async Task<List<Publication>> SearchPublications(string text, int page)
        {
            List<ObjectId> techiesId = new List<ObjectId>();
            foreach (var techie in (await _techieBo.SearchTechiesPerName(text)))
            {
                techiesId.Add(techie.Id);
            }


            return await _publicationDao.SearchPublications(text, techiesId, page);
        }

        /// <summary>
        /// Sugere uma publicação para o usuario com base em uma palavra chave
        /// </summary>
        /// <param name="text">palavra chave</param>
        /// <returns></returns>
        public async Task<List<Publication>> SugestPublication(string text)
        {
            if (text == null || text == "")
            {
                return new List<Publication>();
            }
            List<ObjectId> techiesId = new List<ObjectId>();
            foreach (var techie in (await _techieBo.SearchTechiesPerName(text)))
            {
                techiesId.Add(techie.Id);
            }

            return await SearchPublicationsByIds((await _publicationDao.SugestPublication(text, techiesId)));
        }
        
        public async Task<List<Publication>> ListPublicationsPerTechieId(ObjectId techieId)
        {
            return await _publicationDao.ListPublicationsPerTechieId(techieId);
        }

        

    }
}