using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    public static class DynamicCardTextureCompression
    {
        public const int Quality = 80;

        public static bool Configure(string path)
        {
            if ((!path.StartsWith(DynamicCardLibrary.ContentRoot + "Old/", StringComparison.Ordinal) &&
                 !path.StartsWith(DynamicCardLibrary.ContentRoot + "Latest/", StringComparison.Ordinal)) ||
                !path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return false;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null || importer.textureType != TextureImporterType.Default) return false;
            // DXT blocks are 4x4. Keep tiny constant-colour lookup textures untouched.
            var header = new byte[24];
            using(var file=File.OpenRead(path))if(file.Read(header,0,header.Length)!=header.Length)return false;
            int width=(header[16]<<24)|(header[17]<<16)|(header[18]<<8)|header[19];
            int height=(header[20]<<24)|(header[21]<<16)|(header[22]<<8)|header[23];
            if(width<4 || height<4)return false;
            var settings = importer.GetPlatformTextureSettings("Standalone");
            if (settings.overridden && settings.format == TextureImporterFormat.DXT5Crunched &&
                settings.crunchedCompression && settings.compressionQuality == Quality) return false;
            // Keep the current size cap, mipmaps, colour space and alpha treatment.
            if (!settings.overridden) settings.maxTextureSize = importer.maxTextureSize;
            settings.name = "Standalone";
            settings.overridden = true;
            settings.format = TextureImporterFormat.DXT5Crunched;
            settings.crunchedCompression = true;
            settings.compressionQuality = Quality;
            importer.SetPlatformTextureSettings(settings);
            importer.SaveAndReimport();
            return true;
        }

        [MenuItem("Tools/Dynamic Cards/Optimize Windows Texture Storage")]
        public static void Apply()
        {
            int count = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var path in Directory.GetFiles(DynamicCardLibrary.ContentRoot, "*.png", SearchOption.AllDirectories))
                    if (Configure(path.Replace('\\', '/'))) count++;
            }
            finally { AssetDatabase.StopAssetEditing(); }
            Debug.Log("Dynamic card Windows texture compression configured: " + count);
        }
    }
}
