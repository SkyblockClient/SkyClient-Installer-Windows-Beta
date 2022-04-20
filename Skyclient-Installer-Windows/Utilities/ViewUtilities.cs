using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Skyclient.JsonParts;

namespace Skyclient.Utilities
{
    public class ViewUtilities
    {
        public static string ApplicationName = Assembly.GetExecutingAssembly().GetName().Name ?? "Skyclient-Installer-Windows";
        public static string[] ImageResourceNames = GetImageResourceNames();
        public static void SetImage(ImageBrush img, RepoItem mod)
        {
            img.ImageSource = GetBitmapIcon("icons/" + mod.IconName);
        }

        public static void SetImage(Image img, RepoItem mod)
        {
            img.Source = GetBitmapIcon("icons/" + mod.IconName);
        }

        public static void SetImage(Image img, RepoItemAction action)
        {
            img.Source = GetBitmapIcon("icons/" + action.IconName);
        }

        private static BitmapImage GetBitmapIcon(string image)
        {
            var localimage = image.Replace(" ", "%20"); 
            var found = false;
            var uri = new Uri($"pack://application:,,,/{ApplicationName};component/Images/icons/invalid.png", UriKind.Absolute);
            foreach (string resourceName in ImageResourceNames)
            {
                if (resourceName == "images/" + localimage.ToLower())
                {
                    uri = new Uri($"pack://application:,,,/{ApplicationName};component/images/" + localimage, UriKind.Absolute);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                uri = new Uri(RepoUtils.GetQualifiedHost(image), UriKind.Absolute);
                found = true;
            }
            var bitmap = new BitmapImage(uri);
            return bitmap;
        }
        public static string[] GetImageResourceNames()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            {
                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).Where(entry => entry.StartsWith("images/")).ToArray();
                }
            }
        }
    }
}

