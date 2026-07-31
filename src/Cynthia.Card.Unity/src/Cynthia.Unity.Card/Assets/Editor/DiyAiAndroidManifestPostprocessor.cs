#if UNITY_ANDROID
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Android;

public sealed class DiyAiAndroidManifestPostprocessor : IPostGenerateGradleAndroidProject
{
    private static readonly XNamespace AndroidNamespace = "http://schemas.android.com/apk/res/android";

    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        var manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");
        var document = XDocument.Load(manifestPath);
        var manifest = document.Root;
        if (manifest == null)
            throw new InvalidDataException("Generated AndroidManifest.xml has no root element.");

        var hasInternetPermission = manifest.Elements()
            .Any(element => element.Name.LocalName == "uses-permission"
                && (string)element.Attribute(AndroidNamespace + "name") == "android.permission.INTERNET");
        if (!hasInternetPermission)
        {
            manifest.AddFirst(new XElement("uses-permission",
                new XAttribute(AndroidNamespace + "name", "android.permission.INTERNET")));
        }

        var application = manifest.Elements().FirstOrDefault(element => element.Name.LocalName == "application");
        if (application == null)
            throw new InvalidDataException("Generated AndroidManifest.xml has no application element.");

        // DIY-AI currently listens on plain HTTP port 5010. Remove this when it moves behind TLS.
        application.SetAttributeValue(AndroidNamespace + "usesCleartextTraffic", "true");
        document.Save(manifestPath);
    }
}
#endif
