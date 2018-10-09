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

namespace CKFinder.Connector
{
    public class CKFinderEvent
    {
        public delegate void Hook(object sender, CKFinderEventArgs e);

        public event Hook BeforeExecuteCommand;

        public event Hook AfterFileUpload;

        public event Hook AfterFileDelete;

        public event Hook BeforeDownloadFile;

        public event Hook InitCommand;

        public enum Hooks
        {
            AfterFileUpload,
            AfterFileDelete,
            BeforeExecuteCommand,
            InitCommand,
            BeforeDownloadFile
        };

        public void ActivateEvent(Hooks hook, params object[] paramlist)
        {
            switch (hook)
            {
                case Hooks.BeforeExecuteCommand:
                    if (BeforeExecuteCommand != null)
                    {
                        BeforeExecuteCommand(this, new CKFinderBeforeExecuteCommandEventArgs(paramlist));
                    }
                    break;

                case Hooks.InitCommand:
                    if (InitCommand != null)
                    {
                        InitCommand(this, new CKFinderInitCommandEventArgs(paramlist));
                    }
                    break;

                case Hooks.AfterFileUpload:
                    if (AfterFileUpload != null)
                    {
                        AfterFileUpload(this, new CKFinderAfterFileUploadEventArgs(paramlist));
                    }
                    break;

                case Hooks.AfterFileDelete:
                    if (AfterFileDelete != null)
                    {
                        AfterFileDelete(this, new CKFinderAfterFileDeleteEventArgs(paramlist));
                    }
                    break;

                case Hooks.BeforeDownloadFile:
                    if (BeforeDownloadFile != null)
                    {
                        BeforeDownloadFile(this, new CKFinderBeforeDownloadFileEventArgs(paramlist));
                    }
                    break;
            }
        }
    }
}