using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WhyNotRun.BO;
using WhyNotRun.Filters;
using WhyNotRun.Models.CommentViewModel;
using WhyNotRun.Models.PublicationViewModel;

namespace WhyNotRun.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PublicationController : ApiController
    {
        private PublicationBO _publicationBo;

        public PublicationController()
        {
            _publicationBo = new PublicationBO();
        }

        /// <summary>
        /// Listar publicacoes
        /// </summary>
        /// <returns>Lista de publicações</returns>
        [HttpGet]
        [Route("publications")] //alterar passando o id do ultimo comentario e trazendo só os seguintes dele
        public async Task<IHttpActionResult> ListPublications(int page)
        {
            var resultado = await _publicationBo.ListPublications(page);
            if (resultado != null)
            {
                return Ok(new ListOfPublicationsViewModel(resultado));
            }
            return NotFound(); //mudar isso
        }
        
        /// <summary>
        /// Cadastrar publicação
        /// </summary>
        /// <param name="model">publicação a ser cadastrada</param>
        /// <returns></returns>
        [HttpPost]
        [Route("publications")]
        [WhyNotRunJwtAuth]
        public async Task<IHttpActionResult> CreatePublication(CreatePublicationViewModel model)
        {
            var resultado = await _publicationBo.CreatePublication(model.ToPublication());
            if (resultado != null)
            {
                return Ok(new ViewPublicationViewModel(resultado));
            }
            return StatusCode((HttpStatusCode)422);
        }

        /// <summary>
        /// Reage a uma publicação
        /// </summary>
        /// <param name="model">dados da publicação reagida</param>
        [HttpPatch]
        [Route("publications/{id}/reactions")]
        [WhyNotRunJwtAuth]
        public async Task<IHttpActionResult> React(string id, ReactPublicationViewModel model)
        {
            var resultado = await _publicationBo.React(model.UserId.ToObjectId(), id.ToObjectId(), model.Like);
            if (resultado)
            {
                return Ok(new ViewPublicationViewModel(await _publicationBo.SearchPublicationById(id.ToObjectId())));
            }
            return InternalServerError();
        }

        /// <summary>
        /// Adiciona um comentario a uma publicação
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("comments")]
        [WhyNotRunJwtAuth]
        public async Task<IHttpActionResult> AddComment(AddCommentViewModel model)
        {
            var resultado = await _publicationBo.AddComment(model.ToComment(), model.PublicationId.ToObjectId());
            if (resultado != null)
            {
                return Ok(new CreatedCommentViewModel(resultado)); //mudar isso
            }
            return StatusCode((HttpStatusCode)422);

        }//teste

        /// <summary>
        /// Retorna mais comentarios de uma publicação especifica
        /// </summary>
        /// <param name="publicationId">Id da publicação</param>
        /// <param name="lastCommentId">Id do comentario a ser usado de base para listagem dos proximos</param>
        /// <param name="limit">quantidade a ser carregada</param>
        /// <returns></returns>
        [HttpGet]
        [Route("comments")]//lembrar de rever o retorno disso
        public async Task<IHttpActionResult> SeeMoreComments(string publicationId, string lastCommentId, int limit)
        {
            var result = await _publicationBo.SeeMoreComments(publicationId.ToObjectId(), lastCommentId.ToObjectId(), limit);
            if (result != null)
            {
                return Ok(CreatedCommentViewModel.ToList(result));
            }
            return NotFound();
        }
        
        /// <summary>
        /// Busca publicações com base em uma palavra chave
        /// </summary>
        /// <param name="text"></param>
        [HttpGet]
        [Route("publications")] 
        public async Task<IHttpActionResult> SearchPublications(string text, int page)
        {
            var result = await _publicationBo.SearchPublications(text, page);
            if (result != null)
            {
                return Ok(ViewPublicationViewModel.ToList(result));
            }
            return NotFound();
        }
        
        /// <summary>
        /// Busca uma publicação por ID
        /// </summary>
        /// <param name="publicationId">ID da publicação a ser buscada</param>
        /// <returns></returns>
        [HttpGet]
        [Route("publications/{publicationId}")]
        public async Task<IHttpActionResult> SearchPublicationById(string publicationId)
        {
            var result = await _publicationBo.SearchPublicationById(publicationId.ToObjectId());
            if (result != null)
            {
                return Ok(new ViewPublicationViewModel(result));
            }
            return NotFound();
        }

        /// <summary>
        /// Sugere uma publicação para o usuario com base em uma palavra chave
        /// </summary>
        /// <param name="text">palavra chave</param>
        /// <returns></returns>
        [HttpGet]
        [Route("publications")]
        public async Task<IHttpActionResult> SugestPublication(string text)
        {
            var result = await _publicationBo.SugestPublication(text);

            if (result != null)
            {
                return Ok(SugestPublicationViewModel.ToList(result));
            }
            return NotFound();
        }
        

    }
}
