using System.Linq;
using System.Runtime.Caching;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using storeme.Data;
using storeme.Data.Encryption;
using storeme.Code;
using storeme.Data.Model;
using storeme.ViewModels;

namespace storeme.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardRepository repository = new DashboardRepository();

        private async Task<Dashboard> GetDashboard(string passCode)
        {
            if (string.IsNullOrEmpty(passCode))
            {
                throw new InvalidCredentialException();
            }

            return await repository.FindByAccessCode(passCode);
        }

        /// <summary>
        /// Uploads this instance.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [HttpPost]
        public async Task<JsonResult> Upload(HttpPostedFileBase file)
        {
            var dashboard = await GetDashboard(Request.Headers["accessCode"]);
            string uploadPath = Request.Headers["uploadPath"];
            var encryptor = new DashboardEncryptor(dashboard);

            var filename = file.FileName;
            var buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, file.ContentLength);

            var encryptedFile = encryptor.CreateEncryptedItem(filename, uploadPath, buffer, file.ContentType);

            if (dashboard.DashboardItems.Any(f => f.Path == encryptedFile.Path && f.Name == encryptedFile.Name))
            {
                return new FormattedJsonResult(new { result = "error", message = "File with the same name already exists." });
            }

            dashboard.DashboardItems.Add(encryptedFile);

            await repository.Update(dashboard);
            return new FormattedJsonResult(new { result = "success" });
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FormattedJsonResult> New()
        {
            var encryptor = new DashboardEncryptor();
            await repository.Insert(encryptor.Dashboard);

            return new FormattedJsonResult(new { AccessCode = encryptor.PassCode });
        }

        /// <summary>
        /// Existses the specified access code.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FormattedJsonResult> Exists(string accessCode)
        {
            if (string.IsNullOrEmpty(accessCode))
            {
                return new FormattedJsonResult(new { Exists = false });
            }

            var dashboard = await this.GetDashboard(accessCode);
            var exists = dashboard != null;

            return new FormattedJsonResult(new { Exists = exists });
        }

        /// <summary>
        /// Files the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="accessCode">The access code.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileResult> Download(string path, string name, string accessCode)
        {
            path = path ?? string.Empty;
            var toDownload = await repository.GetItem(accessCode, path, name);
            if (toDownload?.File == null)
            {
                throw new HttpException(404, "File not found");
            }

            Response.Headers["x-filename"] = toDownload.Name;
            return File(toDownload.File.Content, toDownload.File.MediaType, toDownload.Name);
        }

        /// <summary>
        /// Gets the files from the specified folder path
        /// </summary>
        /// <param name="onlyFolders">The only folders.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FormattedJsonResult> Files(bool? onlyFolders, string path)
        {
            path = path ?? string.Empty;
            var accessCode = Request.Headers["accessCode"];
            var dashboardItems = await repository.GetItemsByPath(accessCode, path);

            var files = dashboardItems.Select(f => FileViewModel.FromDashboardItem(f, true));
            if (onlyFolders != null && onlyFolders.Value)
            {
                files = files.Where(f => f.Type == FileType.Dir);
            }

            return new FormattedJsonResult(new { result = files });
        }

        /// <summary>
        /// News the folder.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FormattedJsonResult> NewFolder(string name, string path)
        {
            path = path ?? string.Empty;
            var accessCode = Request.Headers["accessCode"];
            var dashboard = await repository.FindByAccessCode(accessCode);
            var encryptor = new DashboardEncryptor(dashboard);

            dashboard.DashboardItems.Add(encryptor.CreateEncryptedItem(name, path));
            await this.repository.Update(dashboard);

            return new FormattedJsonResult(new { Result = "success" });
        }

        /// <summary>
        /// Deletes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task<FormattedJsonResult> Delete(string name, string path)
        {
            path = path ?? string.Empty;
            var accessCode = Request.Headers["accessCode"];
            await this.repository.DeleteItem(accessCode, path, name);

            return new FormattedJsonResult(new { Result = "success" });
        }
    }
}