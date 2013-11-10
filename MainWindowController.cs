using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace FontSpriteGenerator
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{

		/// <summary>
		/// The set of supported characters separated by spaces.
		/// </summary>
		public const string Characters = " . , : ; \" ' ! ? - ( ) # @ $ % & * + | / 0 1 2 3 4 5 6 7 8 9 a b c d e f g h i j k l m n o p q r s t u v w x y z A B C D E F G H I J K L M N O P Q R S T U V W X Y Z  ";
		//public const string Characters = ".,:;\"'!?-()#@$%&*+|/0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		#region Constructors
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion
		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			GenerateButton.Activated += (object sender, EventArgs e) => {
				CreateFontAndBoundariesFiles ();
			};

		}

		private enum BoundaryState
		{
			Start ,
			End
		}

		public void CreateFontAndBoundariesFiles()
		{

			string fontName = FontNameChooser.SelectedItem.Title;
			int fontSize = Convert.ToInt32( FontSizeTextField.StringValue );
			string fontFileNameWithoutExtension = string.Format ("{0}{1}", fontName.Replace (" ", ""), fontSize.ToString ()); 
			string fontImagePath = string.Format ("/Users/chrisworman/Desktop/{0}.png", fontFileNameWithoutExtension);
			string fontBoundariesPath = string.Format("/Users/chrisworman/Desktop/{0}.txt", fontFileNameWithoutExtension);

//			using (Bitmap fontBitmap = CreateFontBitmap(fontName, fontSize)) {
//				fontBitmap.Save (fontImagePath, System.Drawing.Imaging.ImageFormat.Png);
//				ComputeFontBoundariesAndSaveToFile (fontBitmap, fontBoundariesPath);
//			}

			string boundariesString;
			using (Bitmap fontBitmap = CreateFontBitmapAndComputeBoundaries(fontName, fontSize, out boundariesString)) {
				fontBitmap.Save (fontImagePath, System.Drawing.Imaging.ImageFormat.Png);
				System.IO.File.WriteAllText (fontBoundariesPath, boundariesString);
			}

		}

//		public Bitmap CreateFontBitmap(string fontName, int fontSize)
//		{
//
//			Font font = new Font (fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
//
//			// Get the size of the Characters when drawn when the specified font
//			Bitmap fontBitmap = new Bitmap (1, 1);
//			Graphics fontGraphics = Graphics.FromImage (fontBitmap);
//			fontGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
//			SizeF fontMeasurement = fontGraphics.MeasureString (Characters, font);
//			fontBitmap.Dispose ();
//
//			fontBitmap = new Bitmap ((int)fontMeasurement.Width + 10, (int)fontMeasurement.Height);
//			fontGraphics = Graphics.FromImage (fontBitmap);
//			fontGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
//			fontGraphics.DrawString (Characters, font, new SolidBrush (Color.White), new PointF (0, 0));
//
//			return fontBitmap;
//
//		}

		public Bitmap CreateFontBitmapAndComputeBoundaries(string fontName, int fontSize, out string boundariesString)
		{

			Font font = new Font (fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

			// Compute the measurements of each character
			Bitmap fontBitmap = new Bitmap (1, 1);
			Graphics fontGraphics = Graphics.FromImage (fontBitmap);
			fontGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			Dictionary<char,SizeF> characterMeasurements = new Dictionary<char,SizeF>();
			int totalCharacterWidth = 0;
			int maxCharacterHeight = 0;
			foreach (char character in Characters.ToCharArray()) {
				SizeF characterMeasurement = fontGraphics.MeasureString (character.ToString(), font);
				if (! characterMeasurements.ContainsKey (character)) {
					characterMeasurements.Add (character, characterMeasurement);
				}
				totalCharacterWidth += (int)characterMeasurement.Width;
				maxCharacterHeight = Math.Max (maxCharacterHeight, (int) characterMeasurement.Height);
			}
			fontBitmap.Dispose ();

			fontBitmap = new Bitmap (totalCharacterWidth, maxCharacterHeight);
			fontGraphics = Graphics.FromImage (fontBitmap);
			fontGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			float xOffSet = 0;
			StringBuilder boundaries = new StringBuilder ();
			foreach (char character in Characters.ToCharArray()) {

				// Draw the current character at the current x offset
				fontGraphics.DrawString (character.ToString(), font, new SolidBrush (Color.White), new PointF (xOffSet, 0));

				// Get width measurements of the current character
				SizeF characterMeasurement;
				characterMeasurements.TryGetValue (character, out characterMeasurement);

				// If the character is not a space, then record it's boundaries
				if (character != ' ') {
					AppendBoundary ((int) (xOffSet - 1), boundaries);
					AppendBoundary ((int) (characterMeasurement.Width + 1), boundaries);
				}

				// Increment the x offset according to the just drawn character
				xOffSet += characterMeasurement.Width;

			}

			boundariesString = boundaries.ToString ();
			return fontBitmap;

		}

//		public void ComputeFontBoundariesAndSaveToFile (Bitmap fontBitmap, string filePath)
//		{
//
//			StringBuilder boundaries = new StringBuilder ();
//			BoundaryState state = BoundaryState.Start;
//
//			for (int x=0; x<fontBitmap.Width; x++) {
//				for (int y=0; y<fontBitmap.Height; y++) {
//
//					Color colorXY = fontBitmap.GetPixel (x,y);
//
//					if (state == BoundaryState.Start && colorXY.A != (byte)0) {
//						AppendBoundary (x, boundaries);
//						state = BoundaryState.End;
//						break;
//					} else if (state == BoundaryState.End && colorXY.A != (byte)0) {
//						break;
//					} else if (state == BoundaryState.End && colorXY.A == (byte)0 && y == fontBitmap.Height - 1) {
//						AppendBoundary (x + 1, boundaries);
//						state = BoundaryState.Start;
//					}
//
//				}
//			}
//
//			System.IO.File.WriteAllText (filePath, boundaries.ToString());
//
//		}

		private void AppendBoundary(int x, StringBuilder boundaries)
		{
			if (boundaries.Length == 0) {
				boundaries.Append (x.ToString ());
			} else {
				boundaries.AppendFormat (",{0}", x.ToString ());
			}
		}

	}
}

