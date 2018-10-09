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

// To disabled special debugging features, simply uncomment this line.
// #undef DEBUG

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web;

namespace CKFinder.Connector
{
	/// <summary>
	/// The class handles all requests sent to the CKFinder connector file,
	/// connector.aspx.
	/// </summary>
	public class Connector : Page
	{
		private const string CsrfTokenName = "ckCsrfToken";
		private const int MinCsrfTokenLength = 32;

		/// <summary>
		/// The "ConfigFile" object, is the instance of config.ascx, present in
		/// the connector.aspx file. It makes it possible to configure CKFinder
		/// whithout having to compile it.
		/// </summary>
		public Settings.ConfigFile ConfigFile;
		public CKFinderPlugin[] Plugins;
		public List<string> JavascriptPlugins;
		public CKFinderEvent CKFinderEvent = new CKFinderEvent();

		#region Disable ASP.NET features

		/// <summary>
		/// Theming is disabled as it interferes in the connector response data.
		/// </summary>
		public override bool EnableTheming
		{
			get { return false; }
			set { /* Ignore it with no error */ }
		}

		/// <summary>
		/// Master Page is disabled as it interferes in the connector response data.
		/// </summary>
		public override string MasterPageFile
		{
			get { return null; }
			set { /* Ignore it with no error */ }
		}

		/// <summary>
		/// Theming is disabled as it interferes in the connector response data.
		/// </summary>
		public override string Theme
		{
			get { return ""; }
			set { /* Ignore it with no error */ }
		}

		/// <summary>
		/// Theming is disabled as it interferes in the connector response data.
		/// </summary>
		public override string StyleSheetTheme
		{
			get { return ""; }
			set { /* Ignore it with no error */ }
		}

		#endregion

		protected void LoadPlugins()
		{
			Config _Config = Config.Current;

			if ( _Config.Plugins == null || _Config.Plugins.Length == 0 )
				return;

			Plugins = new CKFinderPlugin[_Config.Plugins.Length];
			JavascriptPlugins = new List<string>();

			for ( int i = 0; i < _Config.Plugins.Length; i++ )
			{
				int j = 0;

				if ( _Config.Plugins[i].IndexOf( "," ) != -1 )
				{
					this.Plugins[j] = (CKFinderPlugin)Activator.CreateInstance( Type.GetType( _Config.Plugins[i] ) );
					this.Plugins[j].Init( CKFinderEvent );

					if ( this.Plugins[j].JavascriptPlugins.Length > 0 )
					{
						this.JavascriptPlugins.Add( this.Plugins[j].JavascriptPlugins );
					}
					j++;
				}
				else
				{
					this.JavascriptPlugins.Add( _Config.Plugins[i] );
				}
			}
		}

		protected override void OnLoad( EventArgs e )
		{
			// Set the config file instance as the current one (to avoid singleton issues).
			ConfigFile.SetCurrent();

			// Load the config file settings.
			ConfigFile.SetConfig();

			// Load plugins.
			LoadPlugins();

#if (DEBUG)
			// For testing purposes, we may force the user to get the Admin role.
			// Session[ "CKFinder_UserRole" ] = "Admin";

			// Simulate slow connections.
			// System.Threading.Thread.Sleep( 2000 );
#endif

			CommandHandlers.CommandHandlerBase commandHandler = null;
			
			try
			{
				VerifyRequest();

				// Take the desired command from the querystring.
				string command = Request.QueryString["command"];

				if ( command == null )
					ConnectorException.Throw( Errors.InvalidCommand );
				else
				{
					CKFinderEvent.ActivateEvent( CKFinderEvent.Hooks.BeforeExecuteCommand, command, Response );

					// Create an instance of the class that handles the
					// requested command.
					switch ( command )
					{
						case "Init":
							commandHandler = new CommandHandlers.InitCommandHandler();
							break;

						case "GetFolders":
							commandHandler = new CommandHandlers.GetFoldersCommandHandler();
							break;

						case "GetFiles":
							commandHandler = new CommandHandlers.GetFilesCommandHandler();
							break;

						case "Thumbnail":
							commandHandler = new CommandHandlers.ThumbnailCommandHandler();
							break;

						case "CreateFolder":
							commandHandler = new CommandHandlers.CreateFolderCommandHandler();
							break;

						case "RenameFolder":
							commandHandler = new CommandHandlers.RenameFolderCommandHandler();
							break;

						case "DeleteFolder":
							commandHandler = new CommandHandlers.DeleteFolderCommandHandler();
							break;

						case "FileUpload":
							commandHandler = new CommandHandlers.FileUploadCommandHandler();
							break;

						case "QuickUpload":
							commandHandler = new CommandHandlers.QuickUploadCommandHandler();
							break;

						case "DownloadFile":
							commandHandler = new CommandHandlers.DownloadFileCommandHandler();
							break;

						case "RenameFile":
							commandHandler = new CommandHandlers.RenameFileCommandHandler();
							break;

						case "DeleteFiles":
							commandHandler = new CommandHandlers.DeleteFilesCommandHandler();
							break;

						case "CopyFiles":
							commandHandler = new CommandHandlers.CopyFilesCommandHandler();
							break;

						case "MoveFiles":
							commandHandler = new CommandHandlers.MoveFilesCommandHandler();
							break;

						default:
							ConnectorException.Throw( Errors.InvalidCommand );
							break;
					}
				}

				// Send the appropriate response.
				if ( commandHandler != null )
					commandHandler.SendResponse( Response );
			}
			catch ( ConnectorException connectorException )
			{
#if DEBUG
				// While debugging, throwing the error gives us more useful
				// information.
				throw connectorException;
#else
				commandHandler = new CommandHandlers.ErrorCommandHandler( connectorException );
				commandHandler.SendResponse( Response );
#endif
			}
		}

		public static bool CheckFolderName(string folderName)
		{
			Config _Config = Config.Current;
			if ( _Config.DisallowUnsafeCharacters && folderName.Contains(".") )
				return false;

			return Connector.CheckFileName(folderName);
		}

		public static bool CheckFileName( string fileName )
		{
			if ( fileName == null || fileName.Length == 0 || fileName.StartsWith( "." ) || fileName.EndsWith( "." ) || fileName.Contains( ".." ) )
				return false;

			if ( Regex.IsMatch( fileName, @"[/\\:\*\?""\<\>\|\p{C}]" ) )
				return false;

			Config _Config = Config.Current;
			if ( _Config.DisallowUnsafeCharacters && fileName.Contains(";") )
				return false;

			return true;
		}

		private void VerifyRequest()
		{
			Config _Config = Config.Current;
			if (string.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
			{
				if (_Config.EnableCsrfProtection && !CheckCsrfToken())
				{
					ConnectorException.Throw(Errors.InvalidRequest);
				}
			}
		}

		private bool CheckCsrfToken()
		{
			HttpCookie cookie = Request.Cookies[CsrfTokenName];
			string cookieToken = cookie != null ? cookie.Value : null;
			string paramToken = Request.Form[CsrfTokenName];

			if (cookieToken == null || paramToken == null)
			{
				return false;
			}

			return cookieToken.Length >= MinCsrfTokenLength && paramToken.Length >= MinCsrfTokenLength && cookieToken == paramToken;
		}
	}
}
