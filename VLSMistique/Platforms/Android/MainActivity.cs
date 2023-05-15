using Android.App;
using Android.Content.PM;
using Android.OS;

namespace VLSMistique;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape)] // <--- Orientation moved to the end of the line and set to landscape
public class MainActivity : MauiAppCompatActivity
{
}
