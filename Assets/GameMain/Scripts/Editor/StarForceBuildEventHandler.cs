//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace StarForce.Editor
{
    public sealed class StarForceBuildEventHandler : IBuildEventHandler
    {
        public bool ContinueOnFailure
        {
            get
            {
                return false;
            }
        }

        public string m_outputDirectory = "";
        public int m_internalResourceVersion = 0;

        public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            m_outputDirectory = outputDirectory;
            m_internalResourceVersion = internalResourceVersion;
            string streamingAssetsPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            string[] fileNames = Directory.GetFiles(streamingAssetsPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(".gitkeep"))
                {
                    continue;
                }

                File.Delete(fileName);
            }

            //Utility.Path.RemoveEmptyDirectory(streamingAssetsPath);
        }

        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
        }

        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
        {
        }

        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, AssetBundleManifest assetBundleManifest)
        {
        }

        public string GetPlatformVersion(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return "Android";
            }
            else if (platform == Platform.IOS)
            {
                return "IOS";
            }
            else if (platform == Platform.Windows)
            {
                return "Windows";
            }
            else if (platform == Platform.MacOS)
            {
                return "MacOS";
            }
            return "";
        }

        [Serializable]
        public class EditorVersion
        {
            public bool ForceUpdateGame;
            public string LatestGameVersion;
            public int InternalGameVersion;
            public int InternalResourceVersion;
            public string UpdatePrefixUri;
            public int VersionListLength;
            public int VersionListHashCode;
            public int VersionListZipLength;
            public int VersionListZipHashCode;
            public string END_OF_JSON;
        }

        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
        {
            if (string.IsNullOrEmpty(m_outputDirectory))
                return;
            string platformName = GetPlatformVersion(platform) + "Version.txt";
            string versionInfoPath = Path.Combine(m_outputDirectory,platformName);
            if (File.Exists(versionInfoPath))
                File.Delete(versionInfoPath);

            EditorVersion editorVersion = new EditorVersion();
            editorVersion.VersionListLength = versionListLength;
            editorVersion.VersionListHashCode = versionListHashCode;
            editorVersion.VersionListZipLength = versionListZipLength;
            editorVersion.VersionListZipHashCode = versionListZipHashCode;
            editorVersion.ForceUpdateGame = false;
            editorVersion.InternalResourceVersion = m_internalResourceVersion;
            editorVersion.LatestGameVersion = "0.1.0";
            editorVersion.UpdatePrefixUri = "http://192.168.8.157/RemoteResource/{0}";
            editorVersion.END_OF_JSON = "";
            File.WriteAllText(versionInfoPath, EditorJsonUtility.ToJson(editorVersion,true));
        }

        public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
        {
            if (!outputPackageSelected)
            {
                return;
            }

            if (platform != Platform.Windows)
            {
                return;
            }

            string streamingAssetsPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            string[] fileNames = Directory.GetFiles(outputPackagePath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetRegularPath(Path.Combine(streamingAssetsPath, fileName.Substring(outputPackagePath.Length)));
                FileInfo destFileInfo = new FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }

                File.Copy(fileName, destFileName);
            }
        }
    }
}
