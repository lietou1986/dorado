namespace CKFinder
{
    public class FileManager : CKFinderPlugin
    {
        public string JavascriptPlugins
        {
            get { return string.Empty; }
        }

        public void Init(Connector.CKFinderEvent ckFinderEvent)
        {
            ckFinderEvent.BeforeExecuteCommand += new Connector.CKFinderEvent.Hook(CKFinderEvent_BeforeExecuteCommand);
            ckFinderEvent.InitCommand += new Connector.CKFinderEvent.Hook(CKFinderEvent_InitCommand);
            ckFinderEvent.AfterFileUpload += new Connector.CKFinderEvent.Hook(CKFinderEvent_AfterFileUpload);
            ckFinderEvent.AfterFileDelete += new Connector.CKFinderEvent.Hook(CKFinderEvent_AfterFileDelete);
            ckFinderEvent.BeforeDownloadFile += new Connector.CKFinderEvent.Hook(CKFinderEvent_BeforeDownloadFile);
        }

        private void CKFinderEvent_BeforeExecuteCommand(object sender, Connector.CKFinderEventArgs e)
        {
            var args = e as Connector.CKFinderBeforeExecuteCommandEventArgs;
        }

        private void CKFinderEvent_InitCommand(object sender, Connector.CKFinderEventArgs e)
        {
            var args = e as Connector.CKFinderInitCommandEventArgs;
        }

        private void CKFinderEvent_AfterFileUpload(object sender, Connector.CKFinderEventArgs e)
        {
            var args = e as Connector.CKFinderAfterFileUploadEventArgs;
        }

        private void CKFinderEvent_AfterFileDelete(object sender, Connector.CKFinderEventArgs e)
        {
            var args = e as Connector.CKFinderAfterFileDeleteEventArgs;
        }

        private void CKFinderEvent_BeforeDownloadFile(object sender, Connector.CKFinderEventArgs e)
        {
            var args = e as Connector.CKFinderBeforeDownloadFileEventArgs;
        }
    }
}