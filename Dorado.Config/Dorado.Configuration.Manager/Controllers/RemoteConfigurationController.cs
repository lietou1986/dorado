using Dorado.Utils;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Dorado.Configuration.Manager.Controllers
{
    public class RemoteConfigurationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SimpleIndex()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public void Create(string application, string sectionName)
        {
            CreateVersion(application, sectionName);
        }

        public ActionResult CreateVersion()
        {
            return View();
        }

        [HttpPost]
        public void CreateVersion(string application, string sectionName, int major = 1)
        {
            HttpFileCollectionBase files = Request.Files;

            HttpPostedFileBase currentFile = files["fileName"];

            byte[] fileBytes = new byte[currentFile.ContentLength];
            currentFile.InputStream.Read(fileBytes, 0, fileBytes.Length);

            int result = RawConfigurationManager.Instance.CreateMinor(application, sectionName, major, fileBytes) ? 1 : 0; //添加创建人ID
            Response.Write(string.Format("<script>location.reload();parent.CreateCallBack({0});</script>", result));
        }

        public ActionResult EditVersion(string downloadUrl)
        {
            WebClient webClient = new WebClient {Credentials = CredentialCache.DefaultCredentials};
            byte[] buffer = webClient.DownloadData(downloadUrl);
            ViewBag.FileContent = Encoding.UTF8.GetString(buffer);
            return View();
        }

        public ActionResult GetConfigContent(string downloadUrl)
        {
            WebClient webClient = new WebClient {Credentials = CredentialCache.DefaultCredentials};
            byte[] buffer = webClient.DownloadData(downloadUrl);
            return Content(Encoding.UTF8.GetString(buffer));
        }

        [HttpPost]
        public ActionResult EditVersion(string application, string sectionName, int major, string fileContent)
        {
            return EditConfig(application, sectionName, major, fileContent);
        }

        [HttpPost]
        public ActionResult EditConfig(string application, string sectionName, int major, string fileContent)
        {
            fileContent = Server.UrlDecode(fileContent).Trim();
            int result = RawConfigurationManager.Instance.CreateMinor(application, sectionName, major, fileContent) ? 1 : 0;
            return Content(result.ToString());
        }

        public ActionResult ViewHistory()
        {
            return View();
        }

        public ActionResult ViewConfig(string downloadUrl)
        {
            WebClient webClient = new WebClient {Credentials = CredentialCache.DefaultCredentials};
            byte[] buffer = webClient.DownloadData(downloadUrl);
            ViewBag.FileContent = Encoding.UTF8.GetString(buffer);
            return View();
        }

        public ActionResult DeleteConfig(string application, string sectionName)
        {
            application = Server.UrlDecode(application);
            sectionName = Server.UrlDecode(sectionName);

            bool result = RawConfigurationManager.Instance.DeleteConfig(application, sectionName);
            return Content(result.ToString());
        }

        public ActionResult GetHistory(string application, string sectionName, int major=1)
        {
            application = Server.UrlDecode(application);
            sectionName = Server.UrlDecode(sectionName);

            DataTable dt = RawConfigurationManager.Instance.GetMinors(application, sectionName, major);
            string json = JsonUtility.DataTableToJson("rows", dt);
            return Content(json);
        }

        public ActionResult GetAllLastVersion(string application)
        {
            application = Server.UrlDecode(application);

            DataTable dt = RawConfigurationManager.Instance.GetAllLastVersion(application);
            string json = JsonUtility.DataTableToJson("rows", dt);
            return Content(json);
        }

        public ActionResult GetApplications()
        {
            DataTable dt = RawConfigurationManager.Instance.GetApplications();
            string json = JsonUtility.DataTableToJson(dt);
            return Content(json);
        }

        public ActionResult GetAllConfigs(string application)
        {
            application = Server.UrlDecode(application);

            DataTable dt = RawConfigurationManager.Instance.GetAllConfigs(application);
            string json = JsonUtility.DataTableToJson("rows", dt);
            return Content(json);
        }
    }
}