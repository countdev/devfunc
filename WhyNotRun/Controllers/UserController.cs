using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WhyNotRun.BO;
using WhyNotRun.Filters;
using WhyNotRun.Models;
using WhyNotRun.Models.UserViewModel;

namespace WhyNotRun.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private const string Container = "images";

        private UserBO _userBo;

        public UserController()
        {
            _userBo = new UserBO();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("");
            }
            var user = await _userBo.Login(loginViewModel.Email, loginViewModel.Password);
            if (user != null)
            {
                return Ok(new AuthenticatedViewModel(user));
            }
            return NotFound();
        }

        [HttpPost]
        [Route("users")]
        /*[ FAZER VALIDAÇÃO ]*/
        public async Task<IHttpActionResult> CreateUser(CreateUserViewModel model)
        { //rever
            if (!ModelState.IsValid)
            {
                return BadRequest("");//passar a msg de erro
            }

            
            var result = await _userBo.CreateUser(model.ToUser());
            if (result != null)
            {
                return Ok(new VisualizationUserViewModel(result));
            }
            return InternalServerError();
        }

        [HttpPatch]
        [Route("users/{id}/picture")]
        [WhyNotRunJwtAuth]
        public async Task<IHttpActionResult> SavePicture(string id)
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var accountName = ConfigurationManager.AppSettings["storage:account:name"];
            var accountKey = ConfigurationManager.AppSettings["storage:account:key"];
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference(Container);
            var provider = new AzureStorageMultipartFormDataStreamProvider(imagesContainer);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }

            // Retrieve the filename of the file you have uploaded
            var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
            if (string.IsNullOrEmpty(filename) || !await _userBo.SaveImage(id.ToObjectId(), filename))
            {
                return BadRequest("An error has occured while uploading your file. Please try again.");
            }

            return Ok($"File: {filename} has successfully uploaded");
        }


        
    }
}
