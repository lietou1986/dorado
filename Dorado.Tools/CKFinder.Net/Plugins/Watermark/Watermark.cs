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
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI;
using System.Collections;

namespace CKFinder.Plugins
{
	public class Watermark : CKFinder.CKFinderPlugin
	{
		public string JavascriptPlugins
		{
			get { return ""; }
		}

		public void Init(CKFinder.Connector.CKFinderEvent CKFinderEvent)
		{
			CKFinderEvent.AfterFileUpload += new CKFinder.Connector.CKFinderEvent.Hook(CKFinderEvent_AfterFileUpload);

			if (!CKFinder.Connector.Config.Current.PluginSettings.ContainsKey("Watermark_source")) CKFinder.Connector.Config.Current.PluginSettings["Watermark_source"] = "logo.gif";
			if (!CKFinder.Connector.Config.Current.PluginSettings.ContainsKey("Watermark_marginRight")) CKFinder.Connector.Config.Current.PluginSettings["Watermark_marginRight"] = "5";
			if (!CKFinder.Connector.Config.Current.PluginSettings.ContainsKey("Watermark_marginBottom")) CKFinder.Connector.Config.Current.PluginSettings["Watermark_marginBottom"] = "5";
			if (!CKFinder.Connector.Config.Current.PluginSettings.ContainsKey("Watermark_quality")) CKFinder.Connector.Config.Current.PluginSettings["Watermark_quality"] = "90";
			if (!CKFinder.Connector.Config.Current.PluginSettings.ContainsKey("Watermark_transparency")) CKFinder.Connector.Config.Current.PluginSettings["Watermark_transparency"] = "80";
		}

		private void CKFinderEvent_AfterFileUpload(object sender, CKFinder.Connector.CKFinderEventArgs e)
		{
			if (!File.Exists((string)e.data[1]))
				return;
			try
			{
				CreateWatermark((string)e.data[1]);
			}
			catch { }
		}

		private void CreateWatermark(string sourceFile)
		{
			string logoImageName = (string)CKFinder.Connector.Config.Current.PluginSettings["Watermark_source"];
			string logoImageFile = logoImageName;

			//search watermark image
			if (!File.Exists(logoImageFile))
			{
				if (File.Exists(HttpContext.Current.Server.MapPath(logoImageFile)))
					logoImageFile = HttpContext.Current.Server.MapPath(logoImageFile);
				else
				{
					logoImageFile = HttpContext.Current.Server.MapPath(((Page)HttpContext.Current.Handler).ResolveUrl("~/ckfinder/plugins/watermark/" + logoImageName));
					if (!File.Exists(logoImageFile))
					{
						logoImageFile = HttpContext.Current.Server.MapPath(((Page)HttpContext.Current.Handler).ResolveUrl("ckfinder/plugins/waterrmark/" + logoImageName));
						if (!File.Exists(logoImageFile))
							return;
					}
				}
			}

			//load configuration
			int marginBottom = 0, marginRight = 0, transparency = 0;
			int.TryParse((string)CKFinder.Connector.Config.Current.PluginSettings["Watermark_marginBottom"], out marginBottom);
			int.TryParse((string)CKFinder.Connector.Config.Current.PluginSettings["Watermark_marginRight"], out marginRight);
			if (int.TryParse((string)CKFinder.Connector.Config.Current.PluginSettings["Watermark_transparency"], out transparency))
			{
				if (transparency > 100) transparency = 100;
				if (transparency < 0) transparency = 0;
				transparency = transparency * 255 / 100;
			}
			else transparency = 255;

			//load bitmaps
			Image sourceImage = new Bitmap(sourceFile);
			Bitmap destinationBmp = new Bitmap(sourceImage.Width, sourceImage.Height);
			Bitmap logoImage = new Bitmap(logoImageFile);
			Graphics graphics = Graphics.FromImage(destinationBmp);

			//convert to indexes image
			Bitmap bitmapImage = CreateNonIndexedImage(logoImage);

			//set transparency
			for (int i = 0; i < logoImage.Width; i++)
				for (int j = 0; j < logoImage.Height; j++)
				{
					Color logoImagePixel = logoImage.GetPixel(i, j);
					if (logoImagePixel.A != 0)
						bitmapImage.SetPixel(i, j, Color.FromArgb(transparency, logoImagePixel));
				}

			//resize watermark - if is necessary
			if (logoImage.Width > sourceImage.Width || logoImage.Height > sourceImage.Height)
				bitmapImage = resizeImage(logoImage,
					new Size(logoImage.Width > sourceImage.Width ? sourceImage.Width : logoImage.Width,
						logoImage.Height > sourceImage.Height ? sourceImage.Height : logoImage.Height));
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			System.Drawing.Imaging.ImageFormat sourceImageFormat = sourceImage.RawFormat;
			graphics.DrawImage(sourceImage, new PointF(0, 0));
			graphics.DrawImage(bitmapImage, sourceImage.Width - bitmapImage.Width - marginRight, sourceImage.Height - bitmapImage.Height - marginBottom);
			//Dispose objects
			sourceImage.Dispose();
			logoImage.Dispose();
			bitmapImage.Dispose();
			graphics.Dispose();
			destinationBmp.Save(sourceFile, sourceImageFormat);
			sourceImage.Dispose();
		}

		private Bitmap resizeImage(Image imgToResize, Size size)
		{
			int sourceWidth = imgToResize.Width;
			int sourceHeight = imgToResize.Height;
			float nPercent = 0;
			float nPercentW = nPercentW = ((float)size.Width / (float)sourceWidth);
			float nPercentH = nPercentH = ((float)size.Height / (float)sourceHeight);
			nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap bitmap = new Bitmap(destWidth, destHeight);
			Graphics graphics = Graphics.FromImage((Image)bitmap);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
			graphics.Dispose();
			return bitmap;
		}

		private Bitmap CreateNonIndexedImage(Bitmap src)
		{
			Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (Graphics gfx = Graphics.FromImage(newBmp))
				gfx.DrawImage(src, 0, 0);
			return newBmp;
		}
	}
}
