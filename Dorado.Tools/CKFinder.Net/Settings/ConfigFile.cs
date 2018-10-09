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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CKFinder.Settings
{
	public class ConfigFile : System.Web.UI.UserControl
	{
		public string LicenseName;
		public string LicenseKey;
		public string BaseUrl;
		public string BaseDir;

		public bool SecureImageUploads;
		public bool ForceSingleExtension;
		public bool CheckDoubleExtension;
		public bool CheckSizeAfterScaling;
		public bool DisallowUnsafeCharacters;
		public bool EnableCsrfProtection;
		public string[] HtmlExtensions;
		public string[] Plugins;
		public Hashtable PluginSettings;

		public string DefaultResourceTypes;

		private Thumbnails _Thumbnails;
		private Images _Images;
		private AccessControlManager _AccessControl;
		private ResourceTypeManager _ResourceType;

		private string[] _HideFolders;
		private string[] _HideFiles;

		internal Regex HideFoldersRegex;
		internal Regex HideFilesRegex;

		public string RoleSessionVar;

		private static ConfigFile _Current;

		public ConfigFile()
		{
			_Thumbnails = new Thumbnails();
			_Images = new Images();
			_AccessControl = new AccessControlManager();
			_ResourceType = new ResourceTypeManager();

			this.HideFolders = new string[ 0 ];
			this.HideFiles = new string[ 0 ];

			LicenseName = "";
			LicenseKey = "";
			BaseUrl = "/ckfinder/userfiles/";
			BaseDir = "";
			ForceSingleExtension = true;
			CheckSizeAfterScaling = true;
			DisallowUnsafeCharacters = true;
			EnableCsrfProtection = true;
			CheckDoubleExtension = true;
			DefaultResourceTypes = "";
			HtmlExtensions = new string[ 0 ];
			Plugins = new string[ 0 ];
			PluginSettings = new Hashtable();
			RoleSessionVar = "";
		}

		internal static ConfigFile Current
		{
			get
			{
				if ( _Current == null )
				{
					Connector.Connector connector = System.Web.HttpContext.Current.Handler as Connector.Connector;

					if ( connector == null )
						_Current = new ConfigFile();
					else
						_Current = connector.ConfigFile;
				}
				return _Current;
			}

		}

		internal void SetCurrent()
		{
			_Current = this;
		}

		public virtual void SetConfig()
		{ }

		public virtual bool CheckAuthentication()
		{
			return false;
		}

		public Thumbnails Thumbnails
		{
			get { return _Thumbnails; }
		}

		public Images Images
		{
			get { return _Images; }
		}

		public AccessControlManager AccessControl
		{
			get { return _AccessControl; }
		}

		public ResourceTypeManager ResourceType
		{
			get { return _ResourceType; }
		}

		private static Regex BuildHideRegex( string[] entries )
		{
			string pattern = string.Join( "<PIP>", entries );
			pattern = pattern.Replace( "*", "<AST>" ).Replace( "?", "<QMK>" );
			pattern = Regex.Escape( pattern );
			pattern = pattern.Replace( "<AST>", ".*" ).Replace( "<QMK>", "." ).Replace( "<PIP>", "|" );
			pattern = "^(?:" + pattern + ")$";

			return new Regex( pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase );
		}

		public string[] HideFolders
		{
			get { return _HideFolders; }
			set
			{
				_HideFolders = value;
				HideFoldersRegex = BuildHideRegex( value );
			}
		}

		public string[] HideFiles
		{
			get { return _HideFiles; }
			set
			{
				_HideFiles = value;
				HideFilesRegex = BuildHideRegex( value );
			}
		}
	}
}
