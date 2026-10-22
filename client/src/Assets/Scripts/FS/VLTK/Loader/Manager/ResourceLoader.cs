using FS.GameEngine.Logic;
using FS.VLTK.Utilities;
using Server.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;

namespace FS.VLTK.Loader
{
    /// <summary>
    /// Thư viện tương tác với tài nguyên
    /// </summary>
    public class ResourceLoader
    {
        /// <summary>
        /// Tải File AssetBundle tại đường dẫn tương ứng
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string path)
        {
            string localPath = PathUtils.NormalizeLocalPath(path);
            if (!File.Exists(localPath))
            {
                return null;
            }
            /// Yêu cầu tương ứng
            AssetBundle bundle = AssetBundle.LoadFromFile(localPath);
            /// Trả về kết quả
            return bundle;
        }

        /// <summary>
        /// Tải File AssetBundle tại đường dẫn tương ứng
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isEncrypted"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string path, bool isEncrypted)
        {
            byte[] byteData = null;
            string localPath = PathUtils.NormalizeLocalPath(path);
            if (File.Exists(localPath))
            {
                byteData = File.ReadAllBytes(localPath);
            }
            else
            {
                /// Yêu cầu tương ứng
                UnityWebRequest request = UnityWebRequest.Get(path);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SendWebRequest();

                /// Đợi đến khi tải xong
                while (!request.isDone) ;

                /// Nếu có lỗi gì đó
                if (!string.IsNullOrEmpty(request.error))
                {
                    return null;
                }

                /// Chuỗi Bytes tương ứng
                byteData = request.downloadHandler.data;

                /// Xóa yêu cầu
                request.downloadHandler.Dispose();
                request.Dispose();
            }

            if (byteData == null || byteData.Length == 0)
            {
                return null;
            }

            /// Nếu mã hóa thì giải mã
            if (isEncrypted)
            {
                try
                {
                    byteData = KTResourceCrypto.Decrypt(byteData);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// Chuyển về AssetBundle
            return AssetBundle.LoadFromMemory(byteData);
        }

        /// <summary>
        /// Tải File AssetBundle tại đường dẫn tương ứng
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public static void LoadAssetBundleAsync(string path, bool isEncrypted, Action<AssetBundle> done, Action<string> onError)
        {
            /// Yêu cầu tương ứng
            UnityWebRequest request = UnityWebRequest.Get(path);
            request.downloadHandler = new DownloadHandlerBuffer();
            AsyncOperation op = request.SendWebRequest();
            op.completed += (o) => {
                /// Nếu có lỗi gì đó
                if (!string.IsNullOrEmpty(request.error))
                {
                    onError?.Invoke(request.error);
                    return;
                }

                /// Chuỗi Bytes tương ứng
                byte[] byteData = request.downloadHandler.data;

                /// Xóa yêu cầu
                request.downloadHandler.Dispose();
                request.Dispose();

                /// Nếu mã hóa thì giải mã
                if (isEncrypted)
                {
                    try
                    {
                        byteData = KTResourceCrypto.Decrypt(byteData);
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke(ex.ToString());
                        return;
                    }
                }

                /// Chuyển về AssetBundle
                AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(byteData);
                req.completed += (oo) => {
                    /// Đối tượng AssetBundle tương ứng
                    AssetBundle bundle = req.assetBundle;
                    /// Nếu có lỗi gì đó
                    if (bundle == null)
                    {
                        onError?.Invoke("AssetBundle is NULL");
                    }
                    else
                    {
                        done?.Invoke(bundle);
                    }
                };
            };
        }

        /// <summary>
        /// Tìm file trên ổ đĩa từ tên bundle asset
        /// </summary>
        private static string FindLocalFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            string rel = fileName.StartsWith("Assets/") ? fileName.Substring(7) : (fileName.StartsWith("Assets\\") ? fileName.Substring(7) : fileName);
            string direct = Path.Combine(Application.dataPath, rel);
            if (File.Exists(direct)) return direct;

            string justFile = Path.GetFileName(fileName);
            string configRoot = Path.Combine(Application.dataPath, "Config");
            if (Directory.Exists(configRoot))
            {
                string[] files = Directory.GetFiles(configRoot, justFile, SearchOption.AllDirectories);
                if (files.Length > 0) return files[0];
            }

            return null;
        }

        /// <summary>
        /// Tải XML từ Bundle
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XElement LoadXMLFromBundle(AssetBundle bundle, string fileName)
        {
            if (bundle != null && bundle.Contains(fileName))
            {
                TextAsset textAsset = bundle.LoadAsset(fileName) as TextAsset;
                if (textAsset != null)
                {
                    string text = textAsset.text;
                    Resources.UnloadAsset(textAsset);
                    GameObject.DestroyImmediate(textAsset, true);
                    return XElement.Parse(text);
                }
            }

            string local = FindLocalFile(fileName);
            if (local != null)
            {
                try
                {
                    return XElement.Load(local);
                }
                catch (Exception ex)
                {
                    KTDebug.LogError("Error loading XML from file " + local + ": " + ex.Message);
                }
            }

            KTDebug.LogWarning("XML not found in bundle or disk: " + fileName);
            return new XElement("Empty");
        }

        /// <summary>
        /// Tải text file từ Bundle
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string LoadTextFileFromBundle(AssetBundle bundle, string fileName)
        {
            if (bundle != null && bundle.Contains(fileName))
            {
                TextAsset textAsset = bundle.LoadAsset(fileName) as TextAsset;
                if (textAsset != null)
                {
                    string text = textAsset.text;
                    Resources.UnloadAsset(textAsset);
                    GameObject.DestroyImmediate(textAsset, true);
                    return text;
                }
            }

            string local = FindLocalFile(fileName);
            if (local != null)
            {
                try
                {
                    return File.ReadAllText(local);
                }
                catch (Exception ex)
                {
                    KTDebug.LogError("Error loading text from file " + local + ": " + ex.Message);
                }
            }

            return "";
        }

        /// <summary>
        /// Tải text file từ Bundle
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] LoadBytesFromBundle(AssetBundle bundle, string fileName)
        {
            if (bundle != null && bundle.Contains(fileName))
            {
                TextAsset textAsset = bundle.LoadAsset(fileName) as TextAsset;
                if (textAsset != null)
                {
                    byte[] bytes = textAsset.bytes;
                    Resources.UnloadAsset(textAsset);
                    GameObject.DestroyImmediate(textAsset, true);
                    return bytes;
                }
            }

            string local = FindLocalFile(fileName);
            if (local != null)
            {
                try
                {
                    return File.ReadAllBytes(local);
                }
                catch (Exception ex)
                {
                    KTDebug.LogError("Error loading bytes from file " + local + ": " + ex.Message);
                }
            }

            return new byte[0];
        }

        /// <summary>
        /// Tải Prefab từ Resources
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefabDir"></param>
        /// <returns></returns>
        public static GameObject LoadPrefabFromResources(string prefabDir)
        {
            return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(prefabDir));
        }
    }
}
