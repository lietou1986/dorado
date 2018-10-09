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
using System.Web.UI;
using System.Globalization;
using CKFinder;

namespace CKFinder.Connector.CommandHandlers
{
	public class SaveFileCommandHandler : XmlCommandHandlerBase
	{
		public SaveFileCommandHandler()
			: base()
		{
		}

		protected override void BuildXml()
		{
			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileDelete ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			string fileName = Request["FileName"];
			string content = Request.Form["content"];

			if ( !Connector.CheckFileName( fileName ) || Config.Current.CheckIsHiddenFile( fileName ) )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
				return;
			}

			if ( !this.CurrentFolder.ResourceTypeInfo.CheckExtension( System.IO.Path.GetExtension( fileName ) ) )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
				return;
			}

			string filePath = System.IO.Path.Combine( this.CurrentFolder.ServerPath, fileName );

			if ( !System.IO.File.Exists( filePath ) )
				ConnectorException.Throw( Errors.FileNotFound );

			try
			{
				System.IO.File.WriteAllText( filePath, content );
			}
			catch ( System.UnauthorizedAccessException )
			{
				ConnectorException.Throw( Errors.AccessDenied );
			}
			catch ( System.Security.SecurityException )
			{
				ConnectorException.Throw( Errors.AccessDenied );
			}
			catch ( System.ArgumentException )
			{
				ConnectorException.Throw( Errors.FileNotFound );
			}
			catch ( System.IO.PathTooLongException )
			{
				ConnectorException.Throw( Errors.FileNotFound );
			}
			catch
			{
#if DEBUG
				throw;
#else
				ConnectorException.Throw( Errors.Unknown );
#endif
			}
		}
	}
}

namespace CKFinder.Plugins
{
	public class FileEditor : CKFinder.CKFinderPlugin
	{
		public string JavascriptPlugins
		{
			get { return "fileeditor"; }
		}

		public void Init( CKFinder.Connector.CKFinderEvent CKFinderEvent )
		{
			CKFinderEvent.BeforeExecuteCommand += new CKFinder.Connector.CKFinderEvent.Hook( this.BeforeExecuteCommand );
		}

		protected void BeforeExecuteCommand( object sender, CKFinder.Connector.CKFinderEventArgs args )
		{
			String command = (String)args.data[0];

			if ( command == "SaveFile" )
			{
				HttpResponse Response = (HttpResponse)args.data[1];

				CKFinder.Connector.CommandHandlers.CommandHandlerBase commandHandler =
					new CKFinder.Connector.CommandHandlers.SaveFileCommandHandler();
				commandHandler.SendResponse( Response );
			}
		}
	}
}
