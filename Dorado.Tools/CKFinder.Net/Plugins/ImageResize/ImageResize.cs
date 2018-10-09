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
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;
using System.Web.UI;
using ImageManipulation;
using CKFinder;

namespace CKFinder.Connector.CommandHandlers
{
	public class ImageResizeCommandHandler : XmlCommandHandlerBase
	{
		public ImageResizeCommandHandler()
			: base()
		{
		}

		protected override void BuildXml()
		{
			Config _Config = Config.Current;
			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileDelete | AccessControlRules.FileUpload ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			string fileName = Request.Form["FileName"];

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

			int newWidth = 0; 
			int newHeight = 0;

			if ( Request.Form["width"] != null && Request.Form["width"].Length > 0
				&& Request.Form["height"] != null && Request.Form["height"].Length > 0 )
			{
				newWidth = System.Int32.Parse( Request.Form["width"].Trim() );
				newHeight = System.Int32.Parse( Request.Form["height"].Trim() );
			}

			bool resizeOriginal = newWidth > 0 && newHeight > 0;

			if ( resizeOriginal )
			{
				if ( Request.Form["newFileName"] == null || Request.Form["newFileName"].Length == 0 )
				{
					ConnectorException.Throw( Errors.InvalidName );
				}

				string newFileName = Request.Form["newFileName"];

				// Replace dots in the name with underscores (only one dot can be there... security issue).
				if ( Config.Current.ForceSingleExtension )
					newFileName = Regex.Replace( newFileName, @"\.(?![^.]*$)", "_", RegexOptions.None );

				if ( !this.CurrentFolder.ResourceTypeInfo.CheckExtension( System.IO.Path.GetExtension( newFileName ) ) )
				{
					ConnectorException.Throw( Errors.InvalidExtension );
					return;
				}

				if ( !Connector.CheckFileName( newFileName ) || Config.Current.CheckIsHiddenFile( newFileName ) )
				{
					ConnectorException.Throw( Errors.InvalidName );
					return;
				}

				if ( _Config.Images.MaxHeight > 0 && newHeight > _Config.Images.MaxHeight )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
				}
				if ( _Config.Images.MaxWidth > 0 && newWidth > _Config.Images.MaxWidth )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
				}

				string newFilePath = System.IO.Path.Combine( this.CurrentFolder.ServerPath, newFileName );

				if ( Request.Form["overwrite"] != "1" && System.IO.File.Exists( newFilePath ) )
					ConnectorException.Throw( Errors.AlreadyExist );

				CKFinder.Connector.ImageTools.ResizeImage( filePath, newFilePath, newWidth, newHeight, true, Config.Current.Images.Quality );
			}

			string sExtension = System.IO.Path.GetExtension( fileName );
			sExtension = sExtension.TrimStart( '.' );

			// Map the virtual path to the local server path.
			string sServerDir = this.CurrentFolder.ServerPath;
			string sFileNameNoExt = System.IO.Path.GetFileNameWithoutExtension( fileName );
			sFileNameNoExt = Regex.Replace( sFileNameNoExt, @"^(.+)_\d+x\d+$", "$1" );
			List<string> sizes = new List<string>( new string[] { "small", "medium", "large" } );
			Regex sizeRegex = new Regex( @"^(\d+)x(\d+)$" );

			foreach ( string size in sizes )
			{
				if ( Request.Form[size] != null && Request.Form[size] == "1" )
				{
					string thumbName = sFileNameNoExt + "_" + size + "." + sExtension;
					string newFilePath = System.IO.Path.Combine( this.CurrentFolder.ServerPath, thumbName );
					if ( CKFinder.Connector.Config.Current.PluginSettings.ContainsKey( "ImageResize_" + size + "Thumb" ) )
					{
						string thumbSize = CKFinder.Connector.Config.Current.PluginSettings["ImageResize_" + size + "Thumb"].ToString().Trim();

						if ( sizeRegex.IsMatch( thumbSize ) )
						{
							Match m = sizeRegex.Match( thumbSize );
							GroupCollection gc = m.Groups;

							newWidth = Int32.Parse( gc[1].Value );
							newHeight = Int32.Parse( gc[2].Value );

							CKFinder.Connector.ImageTools.ResizeImage( filePath, newFilePath, newWidth, newHeight, true, Config.Current.Images.Quality );
						}
					}
				}
			}
		}
	}

	public class ImageResizeInfoCommandHandler : XmlCommandHandlerBase
	{
		public ImageResizeInfoCommandHandler()
			: base()
		{
		}

		protected override void BuildXml()
		{
			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileView ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			string fileName = Request["FileName"];
			System.Drawing.Image sourceImage;

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
				sourceImage = System.Drawing.Image.FromFile( filePath );

				XmlNode oImageInfo = XmlUtil.AppendElement( this.ConnectorNode, "ImageInfo" );
				XmlUtil.SetAttribute( oImageInfo, "width", sourceImage.Width.ToString() );
				XmlUtil.SetAttribute( oImageInfo, "height", sourceImage.Height.ToString() );

				sourceImage.Dispose();
			}
			catch ( OutOfMemoryException )
			{
				ConnectorException.Throw( Errors.InvalidName );
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
	public class ImageResize : CKFinder.CKFinderPlugin
	{
		public string JavascriptPlugins
		{
			get { return "imageresize"; }
		}

		public void Init( CKFinder.Connector.CKFinderEvent CKFinderEvent )
		{
			CKFinderEvent.BeforeExecuteCommand += new CKFinder.Connector.CKFinderEvent.Hook( this.BeforeExecuteCommand );
			CKFinderEvent.InitCommand += new CKFinder.Connector.CKFinderEvent.Hook( this.InitCommand );
		}

		protected void InitCommand( object sender, CKFinder.Connector.CKFinderEventArgs args )
		{
			XmlNode ConnectorNode = (XmlNode)args.data[0];
			XmlNode oimageresize = CKFinder.Connector.XmlUtil.AppendElement( ConnectorNode.SelectSingleNode("PluginsInfo"), "imageresize" );

			if ( CKFinder.Connector.Config.Current.PluginSettings.ContainsKey( "ImageResize_smallThumb" ) )
			{
				CKFinder.Connector.XmlUtil.SetAttribute( oimageresize, "smallThumb", CKFinder.Connector.Config.Current.PluginSettings["ImageResize_smallThumb"].ToString() );
			}
			if ( CKFinder.Connector.Config.Current.PluginSettings.ContainsKey( "ImageResize_mediumThumb" ) )
			{
				CKFinder.Connector.XmlUtil.SetAttribute( oimageresize, "mediumThumb", CKFinder.Connector.Config.Current.PluginSettings["ImageResize_mediumThumb"].ToString() );
			}
			if ( CKFinder.Connector.Config.Current.PluginSettings.ContainsKey( "ImageResize_largeThumb" ) )
			{
				CKFinder.Connector.XmlUtil.SetAttribute( oimageresize, "largeThumb", CKFinder.Connector.Config.Current.PluginSettings["ImageResize_largeThumb"].ToString() );
			}
		}

		protected void BeforeExecuteCommand( object sender, CKFinder.Connector.CKFinderEventArgs args )
		{
			String command = (String)args.data[0];

			if ( command == "ImageResizeInfo" )
			{
				HttpResponse Response = (HttpResponse)args.data[1];

				CKFinder.Connector.CommandHandlers.CommandHandlerBase commandHandler =
					new CKFinder.Connector.CommandHandlers.ImageResizeInfoCommandHandler();
				commandHandler.SendResponse( Response );
			}
			else if ( command == "ImageResize" )
			{
				HttpResponse Response = (HttpResponse)args.data[1];

				CKFinder.Connector.CommandHandlers.CommandHandlerBase commandHandler =
					new CKFinder.Connector.CommandHandlers.ImageResizeCommandHandler();
				commandHandler.SendResponse( Response );
			}
		}
	}
}
