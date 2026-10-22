using FS.GameEngine.Logic;
using FS.VLTK.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace FS.VLTK.Loader
{
    /// <summary>
    /// Kết quả sau khi tải File Version.xml
    /// </summary>
    public enum LoadVersionResult
    {
        /// <summary>
        /// Vào trò chơi
        /// </summary>
        PlayGame,
        /// <summary>
        /// Yêu cầu tải lại App mới nhất ở trên chợ
        /// </summary>
        RequireReDownloadApp,
        /// <summary>
        /// Yêu cầu tải tài nguyên
        /// </summary>
        RequireDownloadResource,
    }

    /// <summary>
    /// Tải File Version.XML
    /// </summary>
    public class LoadVersion : TTMonoBehaviour
    {
        #region Define
        /// <summary>
        /// Tên File Version
        /// </summary>
        public const string VersionFile = "Version.xml";
        #endregion

        #region Private fields
        /// <summary>
        /// Thông tin file Version.XML hiện có
        /// </summary>
        private GameInfo localGameInfo = null;

        /// <summary>
        /// Thông tin file Version.XML hiện có trong StreamingAsset
        /// </summary>
        private GameInfo streamingAssetGameInfo = null;

        /// <summary>
        /// Thông tin file Version.XML trên Server
        /// </summary>
        private GameInfo serverGameInfo = null;
        #endregion

        #region Properties
        /// <summary>
        /// Sự kiện tải xuống hoàn tất
        /// </summary>
        public Action<GameInfo, LoadVersionResult> Done { get; set; }

        /// <summary>
        /// Sự kiện tải xuống thất bại
        /// </summary>
        public Action<string> Faild { get; set; }
        #endregion

        #region Core MonoBehaviour
        /// <summary>
        /// Hàm này gọi khi đối tượng được khởi tạo
        /// </summary>
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Hàm này gọi ở Frame đầu tiên
        /// </summary>
        private void Start()
        {
            this.StartCoroutine(this.ReadData());
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Thực hiện đọc dữ liệu
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadData()
        {
            /// Đọc dữ liệu từ File Version trong StreamingAsset
            yield return this.ReadStreamingAssetVersion();
            /// Đọc file Version.XML hiện có
            yield return this.ReadLocalVersionLocal();
            /// Nếu không có thông tin
            if (this.localGameInfo == null)
            {
                yield break;
            }
            /// Đọc file Version.XML trên Server
            yield return this.ReadServerVersion();
            /// Nếu không có thông tin
            if (this.serverGameInfo == null)
            {
                yield break;
            }
            /// Thực hiện so sánh phiên bản
            this.CompareVersion();
            /// Hủy đối tượng
            this.Destroy();
        }

        /// <summary>
        /// Đọc File Version.XML ở StreamingAsset
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadStreamingAssetVersion()
        {
            string diskPath = PathUtils.NormalizeLocalPath(PathUtils.SteamingAssetsPath(LoadVersion.VersionFile));
            if (File.Exists(diskPath))
            {
                try
                {
                    string strText = File.ReadAllText(diskPath);
                    string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (strText.StartsWith(_byteOrderMarkUtf8))
                    {
                        strText = strText.Replace(_byteOrderMarkUtf8, "");
                    }
                    this.streamingAssetGameInfo = GameInfo.Parse(XElement.Parse(strText));
                    yield break;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning("[LoadVersion] ReadStreamingAssetVersion File.ReadAllText error: " + ex);
                }
            }

            string fullPath = PathUtils.SteamingAssetsPath(LoadVersion.VersionFile);
            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                yield return request.SendWebRequest();
                if (!string.IsNullOrEmpty(request.error))
                {
                    yield break;
                }

                try
                {
                    byte[] byteData = request.downloadHandler.data;
                    string strText = new UTF8Encoding().GetString(byteData);
                    string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (strText.StartsWith(_byteOrderMarkUtf8))
                    {
                        strText = strText.Replace(_byteOrderMarkUtf8, "");
                    }
                    this.streamingAssetGameInfo = GameInfo.Parse(XElement.Parse(strText));
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Đọc File Version.XML hiện có
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadLocalVersionLocal()
        {
            string persistentPath = PathUtils.GetPersistentPath(LoadVersion.VersionFile);
            if (File.Exists(persistentPath))
            {
                try
                {
                    string strText = File.ReadAllText(persistentPath);
                    string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (strText.StartsWith(_byteOrderMarkUtf8))
                    {
                        strText = strText.Replace(_byteOrderMarkUtf8, "");
                    }
                    GameInfo gameInfo = GameInfo.Parse(XElement.Parse(strText));
                    if (this.streamingAssetGameInfo != null && int.Parse(this.streamingAssetGameInfo.Version) > int.Parse(gameInfo.Version))
                    {
                        this.localGameInfo = this.streamingAssetGameInfo;
                    }
                    else
                    {
                        this.localGameInfo = gameInfo;
                    }
                    yield break;
                }
                catch (Exception)
                {
                }
            }

            if (this.streamingAssetGameInfo != null)
            {
                this.localGameInfo = this.streamingAssetGameInfo;
                yield break;
            }

            string fullPath = Global.WebPath(LoadVersion.VersionFile);
            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                yield return request.SendWebRequest();

                if (!string.IsNullOrEmpty(request.error))
                {
                    this.Faild?.Invoke(request.error);
                    yield break;
                }

                try
                {
                    byte[] byteData = request.downloadHandler.data;
                    string strText = new UTF8Encoding().GetString(byteData);
                    string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (strText.StartsWith(_byteOrderMarkUtf8))
                    {
                        strText = strText.Replace(_byteOrderMarkUtf8, "");
                    }
                    GameInfo gameInfo = GameInfo.Parse(XElement.Parse(strText));

                    if (this.streamingAssetGameInfo != null && int.Parse(this.streamingAssetGameInfo.Version) > int.Parse(gameInfo.Version))
                    {
                        this.localGameInfo = this.streamingAssetGameInfo;
                    }
                    else
                    {
                        this.localGameInfo = gameInfo;
                    }
                }
                catch (Exception)
                {
                    this.localGameInfo = this.streamingAssetGameInfo;
                }
            }
        }

        /// <summary>
        /// Đọc dữ liệu Version.XML ở Server
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadServerVersion()
        {
            string baseUrl = this.localGameInfo.URL;
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            string device = Global.GetDeviceForWebURL();
            if (!string.IsNullOrEmpty(device)) baseUrl += device + "/";
            string fullPath = baseUrl + LoadVersion.VersionFile;

            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                yield return request.SendWebRequest();

                if (!string.IsNullOrEmpty(request.error))
                {
                    this.Faild?.Invoke(request.error);
                    yield break;
                }

                try
                {
                    byte[] byteData = request.downloadHandler.data;
                    string strText = new UTF8Encoding().GetString(byteData);
                    string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (strText.StartsWith(_byteOrderMarkUtf8))
                    {
                        strText = strText.Replace(_byteOrderMarkUtf8, "");
                    }
                    this.serverGameInfo = GameInfo.Parse(XElement.Parse(strText));
                }
                catch (Exception ex)
                {
                    this.Faild?.Invoke(ex.ToString());
                }
            }
        }
        
        /// <summary>
        /// Hủy đối tượng
        /// </summary>
        private void Destroy()
        {
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Thực hiện so sánh 2 phiên bản của Client và Server để đưa ra kết luận có cần Update không
        /// </summary>
        private void CompareVersion()
        {
            /// Nếu phiên bản ở Client thấp hơn ở Server
            if (int.Parse(this.localGameInfo.Version) < int.Parse(this.serverGameInfo.Version))
            {
                this.Done?.Invoke(this.serverGameInfo, LoadVersionResult.RequireReDownloadApp);
            }
            /// Nếu tài nguyên khác nhau
            else if (int.Parse(this.localGameInfo.ResourceVersion) < int.Parse(this.serverGameInfo.ResourceVersion))
            {
                this.Done?.Invoke(this.serverGameInfo, LoadVersionResult.RequireDownloadResource);
            }
            /// Nếu khớp với Server thì vào Game
            else
            {
                this.Done?.Invoke(this.serverGameInfo, LoadVersionResult.PlayGame);
            }
        }
#endregion
    }
}
