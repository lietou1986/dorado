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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CKFinder
{
	[Designer( typeof( CKFinder.FileBrowserDesigner ) )]
	[ToolboxData( "<{0}:FileBrowser runat=server></{0}:FileBrowser>" )]
	public class FileBrowser : System.Web.UI.Control
	{
		private const string CKFINDER_DEFAULT_BASEPATH = "/ckfinder/";
		public FileBrowser()
		{ }

		#region Properties

		[DefaultValue( "/ckfinder/" )]
		public string BasePath
		{
			get
			{
				object o = ViewState[ "BasePath" ];

				if ( o == null )
					o = System.Configuration.ConfigurationSettings.AppSettings[ "CKFinder:BasePath" ];

				return ( o == null ? "/ckfinder/" : (string)o );
			}
			set { ViewState[ "BasePath" ] = value; }
		}

		[Category( "Appearence" )]
		[DefaultValue( "100%" )]
		public Unit Width
		{
			get { object o = ViewState[ "Width" ]; return ( o == null ? Unit.Percentage( 100 ) : (Unit)o ); }
			set { ViewState[ "Width" ] = value; }
		}

		[Category( "Appearence" )]
		[DefaultValue( "400px" )]
		public Unit Height
		{
			get { object o = ViewState[ "Height" ]; return ( o == null ? Unit.Pixel( 400 ) : (Unit)o ); }
			set { ViewState[ "Height" ] = value; }
		}

		public string SelectFunction
		{
			get { return ViewState[ "SelectFunction" ] as string; }
			set { ViewState[ "SelectFunction" ] = value; }
		}

		public string SelectFunctionData
		{
			get { return ViewState["SelectFunctionData"] as string; }
			set { ViewState["SelectFunctionData"] = value; }
		}

		public string SelectThumbnailFunction
		{
			get { return ViewState["SelectThumbnailFunction"] as string; }
			set { ViewState["SelectThumbnailFunction"] = value; }
		}

		public string SelectThumbnailFunctionData
		{
			get { return ViewState["SelectThumbnailFunctionData"] as string; }
			set { ViewState["SelectThumbnailFunctionData"] = value; }
		}

		public bool DisableThumbnailSelection
		{
			get { return ViewState["DisableThumbnailSelection"] == null ? false : (bool)ViewState["DisableThumbnailSelection"]; }
			set { ViewState["DisableThumbnailSelection"] = value; }
		}

		public string ClassName
		{
			get { return ViewState[ "ClassName" ] as string; }
			set { ViewState[ "ClassName" ] = value; }
		}

		public string StartupPath
		{
			get { return ViewState["StartupPath"] as string; }
			set { ViewState["StartupPath"] = value; }
		}

		public string ResourceType
		{
			get { return ViewState["ResourceType"] as string; }
			set { ViewState["ResourceType"] = value; }
		}

		public bool RememberLastFolder
		{
			get { return ViewState["RememberLastFolder"] == null ? true : (bool)ViewState["RememberLastFolder"]; }
			set { ViewState["RememberLastFolder"] = value; }
		}

		public bool StartupFolderExpanded
		{
			get { return ViewState["StartupFolderExpanded"] == null ? false : (bool)ViewState["StartupFolderExpanded"]; }
			set { ViewState["StartupFolderExpanded"] = value; }
		}

		public string CKFinderId
		{
			get { return ViewState["CKFinderId"] as string; }
			set { ViewState["CKFinderId"] = value; }
		}
		#endregion

		#region Rendering

		public string CreateHtml()
		{

			string _ClassName = this.ClassName;
			string _Id = this.CKFinderId;

			if ( _ClassName != null && _ClassName.Length > 0 )
				_ClassName = " class=\"" + _ClassName + "\"";

			if ( _Id != null && _Id.Length > 0 )
				_Id = " id=\"" + _Id + "\"";

			return "<iframe src=\"" + this.BuildUrl() + "\" width=\"" + this.Width + "\" " +
				"height=\"" + this.Height + "\"" + _ClassName + _Id + " frameborder=\"0\" scrolling=\"no\"></iframe>";
		}

		private string BuildCKFinderDirUrl()
		{
			string _Url = this.BasePath;

			if ( _Url == null || _Url.Length == 0 )
				_Url = CKFINDER_DEFAULT_BASEPATH;

			if ( !_Url.EndsWith( "/" ) )
				_Url = _Url + "/";

			return _Url;
		}

		private string BuildUrl()
		{
			string _Url = this.BuildCKFinderDirUrl();
			string _Qs = "";

			_Url += "ckfinder.html";

			if ( this.SelectFunction != null && this.SelectFunction.Length > 0 )
			{
				_Qs += "?action=js&amp;func=" + this.SelectFunction;
				if ( !string.IsNullOrEmpty( this.SelectFunctionData ) )
					_Qs += "&amp;data=" + this.SelectFunctionData;
			}

			if ( !this.DisableThumbnailSelection )
			{
				if ( this.SelectThumbnailFunction != null && this.SelectThumbnailFunction.Length > 0 )
				{
					_Qs += ( _Qs.Length > 0 ? "&amp;" : "?" );
					_Qs += "thumbFunc=" + this.SelectThumbnailFunction;

					if ( !string.IsNullOrEmpty( this.SelectThumbnailFunctionData ) )
						_Qs += "&amp;tdata=" + this.SelectThumbnailFunctionData;
				}
				else if ( this.SelectFunction != null && this.SelectFunction.Length > 0 )
				{
					_Qs += "&amp;thumbFunc=" + this.SelectFunction;
					if ( !string.IsNullOrEmpty( this.SelectFunctionData ) )
						_Qs += "&amp;tdata=" + this.SelectFunctionData;
				}
			}
			else
			{
				_Qs += ( _Qs.Length > 0 ? "&amp;" : "?" );
				_Qs += "dts=1";
			}

			if ( this.StartupPath != null && this.StartupPath.Length > 0 )
			{
				_Qs += ( _Qs.Length > 0 ? "&amp;" : "?" );
				_Qs += "start=" + System.Web.HttpContext.Current.Server.UrlPathEncode( 
						this.StartupPath + ( this.StartupFolderExpanded ? ":1" : ":0" ) );
			}

			if (this.ResourceType != null && this.ResourceType.Length > 0)
			{
				_Qs += (_Qs.Length > 0 ? "&amp;" : "?");
				_Qs += "type=" + System.Web.HttpContext.Current.Server.UrlPathEncode(this.ResourceType);
			}

			if (!this.RememberLastFolder)
			{
				_Qs += ( _Qs.Length > 0 ? "&amp;" : "?" );
				_Qs += "rlf=0";
			}

			if ( this.CKFinderId != null && this.CKFinderId.Length > 0 )
			{
				_Qs += ( _Qs.Length > 0 ? "&amp;" : "?" );
				_Qs += "id=" + System.Web.HttpContext.Current.Server.UrlPathEncode( this.CKFinderId );
			}

			return _Url + _Qs;
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			writer.Write( this.CreateHtml() );
		}

		#endregion

		#region SetupFCKeditor

		public void SetupFCKeditor( object fckeditorInstance )
		{
			string _OriginalBasePath = this.BasePath;

			// If it is a path relative to the current page.
			if ( !this.BasePath.StartsWith("/") && this.BasePath.IndexOf("://") == -1 )
			{
				string _RequestUri = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
				this.BasePath = _RequestUri.Substring( 0, _RequestUri.LastIndexOf( "/" ) + 1 ) +
					this.BasePath;
			}

			string _QuickUploadUrl = this.BuildCKFinderDirUrl() + "core/connector/aspx/connector.aspx?command=QuickUpload";
			string _Url = this.BuildUrl();
			string _Qs = ( _Url.IndexOf( "?" ) == -1 ) ? "?" : "&amp;";

			// We are doing it through reflection to avoid creating a
			// dependency to the FCKeditor assembly.
			try
			{
				System.Reflection.PropertyInfo _ConfigProp = fckeditorInstance.GetType().GetProperty( "Config" );

				object _Config = _ConfigProp.GetValue( fckeditorInstance, null );

				// Get the default property.
				System.Reflection.PropertyInfo _DefaultProp = _Config.GetType().GetProperty(
					( (System.Reflection.DefaultMemberAttribute)_Config.GetType().GetCustomAttributes( typeof( System.Reflection.DefaultMemberAttribute ), true )[ 0 ] ).MemberName );

				_DefaultProp.SetValue( _Config, _Url, new object[] { "LinkBrowserURL" } );
				_DefaultProp.SetValue( _Config, _Url + _Qs + "type=Images", new object[] { "ImageBrowserURL" } );
				_DefaultProp.SetValue( _Config, _Url + _Qs + "type=Flash", new object[] { "FlashBrowserURL" } );

				_DefaultProp.SetValue( _Config, _QuickUploadUrl, new object[] { "LinkUploadURL" } );
				_DefaultProp.SetValue( _Config, _QuickUploadUrl + "&type=Images", new object[] { "ImageUploadURL" } );
				_DefaultProp.SetValue( _Config, _QuickUploadUrl + "&type=Flash", new object[] { "FlashUploadURL" } );
			}
			catch
			{
				// If the above reflection procedure didn't work, we probably
				// didn't received the apropriate FCKeditor type object.
				throw ( new ApplicationException( "SetupFCKeditor expects an FCKeditor instance object." ) );
			}
		}

		public void SetupCKEditor(object ckeditorInstance)
		{
			try
			{
				string _Url = this.BuildCKFinderDirUrl();
				Type ckeditorInstanceType = ckeditorInstance.GetType();
				ckeditorInstanceType.GetProperty("FilebrowserBrowseUrl").SetValue(ckeditorInstance, _Url + "ckfinder.html", null);
				ckeditorInstanceType.GetProperty("FilebrowserImageBrowseUrl").SetValue(ckeditorInstance, _Url + "ckfinder.html?type=Images", null);
				ckeditorInstanceType.GetProperty("FilebrowserFlashBrowseUrl").SetValue(ckeditorInstance, _Url + "ckfinder.html?type=Flash", null);
				ckeditorInstanceType.GetProperty("FilebrowserUploadUrl").SetValue(ckeditorInstance, _Url + "core/connector/aspx/connector.aspx?command=QuickUpload&type=Files", null);
				ckeditorInstanceType.GetProperty("FilebrowserImageUploadUrl").SetValue(ckeditorInstance, _Url + "core/connector/aspx/connector.aspx?command=QuickUpload&type=Images", null);
				ckeditorInstanceType.GetProperty("FilebrowserFlashUploadUrl").SetValue(ckeditorInstance, _Url + "core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash", null);
			}
			catch
			{
				// If the above reflection procedure didn't work, we probably
				// didn't received the apropriate CKEditor type object.
				throw (new ApplicationException("SetupCKEditor expects an CKEditor instance object."));
			}
		}

		#endregion
	}
}
