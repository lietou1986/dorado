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
        private static readonly string NoAppPath = "General";

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
        public void Create(string sectionName, string application)
        {
            int result = 1;
            TreeNode node = new TreeNode();
            node.Value = sectionName;
            node.Value = Path.Combine(node.Value, application.Trim());
            node.Value = Path.Combine(node.Value, "1");

            RawConfigurationManager rcm = RawConfigurationManager.Instance;
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase currentFile = files["fileName"];

            byte[] fileBytes = new byte[currentFile.ContentLength];
            currentFile.InputStream.Read(fileBytes, 0, fileBytes.Length);

            //判断是否符合XML要求
            if (!rcm.IsStandardXML(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(fileBytes))))
                result = 0;
            else
                rcm.CreateMinor(node, fileBytes);
            Response.Write(string.Format("<script>parent.CreateCallBack({0});</script>", result));
        }

        public ActionResult CreateVersion()
        {
            return View();
        }

        [HttpPost]
        public void CreateVersion(string sectionName, string application, int major = 1)
        {
            int result = 1;
            TreeNode node = new TreeNode();
            node.Value = sectionName;
            node.Value = Path.Combine(node.Value, application.Trim());
            node.Value = Path.Combine(node.Value, major.ToString());

            RawConfigurationManager rcm = RawConfigurationManager.Instance;
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase currentFile = files["fileName"];

            byte[] fileBytes = new byte[currentFile.ContentLength];
            currentFile.InputStream.Read(fileBytes, 0, fileBytes.Length);

            //判断是否符合XML要求
            if (!rcm.IsStandardXML(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(fileBytes))))
                result = 0;
            else
                rcm.CreateMinor(node, fileBytes);//添加创建人ID
            Response.Write(string.Format("<script>location.reload();parent.CreateCallBack({0});</script>", result));
        }

        public ActionResult EditVersion(string downloadUrl)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = CredentialCache.DefaultCredentials;
            byte[] buffer = webClient.DownloadData(downloadUrl);
            ViewBag.FileContent = Encoding.UTF8.GetString(buffer);
            return View();
        }

        public ActionResult GetConfigContent(string downloadUrl)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = CredentialCache.DefaultCredentials;
            byte[] buffer = webClient.DownloadData(downloadUrl);
            return Content(Encoding.UTF8.GetString(buffer));
        }

        [HttpPost]
        public ActionResult EditVersion(string sectionName, string application, string major, string fileContent)
        {
            int result = 1;
            TreeNode node = new TreeNode();
            node.Value = sectionName;
            node.Value = Path.Combine(node.Value, application);
            node.Value = Path.Combine(node.Value, major);

            RawConfigurationManager rcm = RawConfigurationManager.Instance;
            fileContent = Server.UrlDecode(fileContent).Trim();

            //判断是否符合XML要求
            if (!rcm.IsStandardXML(Encoding.UTF8.GetBytes(fileContent)))
                result = 0;
            else
                rcm.CreateMinor(node, Encoding.UTF8.GetBytes(fileContent));//添加创建人ID
            return Content(result.ToString());
        }

        [HttpPost]
        public ActionResult EditConfig(string sectionName, string application, string major, string fileContent)
        {
            int result = 1;
            TreeNode node = new TreeNode();
            node.Value = sectionName;
            node.Value = Path.Combine(node.Value, application);
            node.Value = Path.Combine(node.Value, major);

            RawConfigurationManager rcm = RawConfigurationManager.Instance;
            fileContent = Server.UrlDecode(fileContent).Trim();

            //判断是否符合XML要求
            if (!rcm.IsStandardXML(Encoding.UTF8.GetBytes(fileContent)))
                result = 0;
            else
                rcm.CreateMinor(node, Encoding.UTF8.GetBytes(fileContent));//添加创建人ID
            return Content(result.ToString());
        }

        public ActionResult ViewHistory()
        {
            return View();
        }

        public ActionResult ViewConfig(string downloadurl)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(downloadurl);
            req.ContentType = "text/xml";
            req.Method = "GET";
            req.Timeout = 30000;
            req.ReadWriteTimeout = 30000;
            req.KeepAlive = false;
            string resultContent = string.Empty;
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            using (Stream stream = rsp.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
                resultContent = sr.ReadToEnd();
                stream.Close();
            }
            rsp.Close();
            ViewBag.FileContent = resultContent;
            return View();
        }

        public ActionResult DeleteSection(string sectionName)
        {
            sectionName = Server.UrlDecode(sectionName);

            bool result = RawConfigurationManager.Instance.DeleteConfig(sectionName);
            return Content(result.ToString());
        }

        public ActionResult DeleteApplication(string application, string sectionName)
        {
            sectionName = Server.UrlDecode(sectionName);
            application = Server.UrlDecode(application);
            if (string.Compare(application, NoAppPath) == 0)
            {
                sectionName = Path.Combine(sectionName, NoAppPath);
            }
            else
            {
                sectionName = Path.Combine(sectionName, application);
            }
            bool result = RawConfigurationManager.Instance.DeleteConfig(sectionName);
            return Content(result.ToString());
        }

        public ActionResult GetHistory(string sectionName, string application, string major)
        {
            string value = sectionName;

            if (string.Compare(application, NoAppPath) == 0)
            {
                value = Path.Combine(value, NoAppPath);
            }
            else
            {
                value = Path.Combine(value, application);
            }
            value = Path.Combine(value, major);

            DataTable dt = RawConfigurationManager.Instance.GetMinors(value);
            string json = JsonUtility.DataTableToJson("rows", dt);
            return Content(json);
        }

        public ActionResult GetAllLastVersion()
        {
            DataTable dt = RawConfigurationManager.Instance.GetAllLastVersion();
            string json = JsonUtility.DataTableToJson("rows", dt);
            return Content(json);
        }

        public ActionResult GetApplications(string sectionName)
        {
            DataTable dt = RawConfigurationManager.Instance.GetApplications(sectionName);
            string json = JsonUtility.DataTableToJson(dt);
            return Content(json);
        }

        public ActionResult GetAllConfigs()
        {
            DataTable dt = RawConfigurationManager.Instance.GetAllConfigs();
            string json = JsonUtility.DataTableToJson("rows", dt);
            return Content(json);
        }
    }
}