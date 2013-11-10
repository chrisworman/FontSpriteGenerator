// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace FontSpriteGenerator
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSPopUpButton FontNameChooser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField FontSizeTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton GenerateButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FontNameChooser != null) {
				FontNameChooser.Dispose ();
				FontNameChooser = null;
			}

			if (FontSizeTextField != null) {
				FontSizeTextField.Dispose ();
				FontSizeTextField = null;
			}

			if (GenerateButton != null) {
				GenerateButton.Dispose ();
				GenerateButton = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
