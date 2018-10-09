/*
 * CKFinder
 * ========
 * http://cksource.com/ckfinder
 * Copyright (C) 2007-2015, CKSource - Frederico Knabben. All rights reserved.
 *
 * The software, this file and its contents are subject to the CKFinder
 * License. Please read the license.txt file before using, installing, copying,
 * modifying or distribute this file or part of its contents. The contents of
 * this file is part of the Source Code of CKFinder.
 */

using System;
using System.Web;
using System.Xml;

namespace CKFinder.Connector
{
    public class CKFinderEventArgs : EventArgs
    {
        public CKFinderEventArgs(params object[] paramlist)
        {
            this.data = paramlist;
            this.cancelled = false;
        }

        public object[] data;
        public bool cancelled;
    }

    public class CKFinderBeforeExecuteCommandEventArgs : CKFinderEventArgs
    {
        public string Command;
        public System.Web.HttpResponse Response;

        public CKFinderBeforeExecuteCommandEventArgs(params object[] paramlist)
            : base(paramlist)
        {
            this.Command = paramlist[0] as string;
            this.Response = paramlist[1] as System.Web.HttpResponse;
        }
    }

    public class CKFinderAfterFileUploadEventArgs : CKFinderEventArgs
    {
        public FolderHandler FolderHanlder;
        public string FilePath;
        public string OriginalFileName;
        public string ActualFileName;
        public string Guid;
        public string ContentType;
        public int FileSize;

        public CKFinderAfterFileUploadEventArgs(params object[] paramlist)
            : base(paramlist)
        {
            this.FolderHanlder = paramlist[0] as FolderHandler;
            this.FilePath = paramlist[1] as string;
            this.OriginalFileName = paramlist[2] as string;
            this.ActualFileName = paramlist[3] as string;
            this.Guid = paramlist[4] as string;
            this.ContentType = paramlist[5] as string;
            this.FileSize = (paramlist[6] == null) ? 0 : (int)paramlist[6];
        }
    }

    public class CKFinderInitCommandEventArgs : CKFinderEventArgs
    {
        public XmlNode ConnectorNode;

        public CKFinderInitCommandEventArgs(params object[] paramlist)
        {
            this.ConnectorNode = paramlist[0] as XmlNode;
        }
    }

    public class CKFinderAfterFileDeleteEventArgs : CKFinderEventArgs
    {
        public FolderHandler FolderHandler;
        public string FilePath;
        public string FileName;

        public CKFinderAfterFileDeleteEventArgs(params object[] paramlist)
        {
            this.FolderHandler = paramlist[0] as FolderHandler;
            this.FilePath = paramlist[1] as string;
            this.FileName = paramlist[2] as string;
        }
    }

    public class CKFinderBeforeDownloadFileEventArgs : CKFinderEventArgs
    {
        public FolderHandler FolderHandler;
        public string FilePath;
        public string FileName;
        public HttpResponse Response;

        public CKFinderBeforeDownloadFileEventArgs(params object[] paramlist)
        {
            this.FolderHandler = paramlist[0] as FolderHandler;
            this.FilePath = paramlist[1] as string;
            this.FileName = paramlist[2] as string;
            this.Response = paramlist[3] as HttpResponse;
        }
    }
}