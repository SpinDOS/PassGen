using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(
    typeof(PassGen.Xamarin.Control.ShowHidePasswordEntryPrivate.PassEntry), 
    typeof(PassGen.Xamarin.iOS.Renderer.PassEntryRenderer))]

namespace PassGen.Xamarin.iOS.Renderer
{
    public class PassEntryRenderer: EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
                Control.TextContentType = UITextContentType.OneTimeCode;
        }
    }
}