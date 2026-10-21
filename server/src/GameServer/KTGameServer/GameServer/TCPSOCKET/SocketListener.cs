using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using Server.Tools;
using Server.Protocol;
using GameServer.Logic;
using GameServer.Server;
using GameServer.Tools;
using GameServer.Core.Executor;

namespace Server.TCP
{
    /*鑷悕鏄憲鐨勬寚浠や贡搴忓垎鏋�:
     * 1. 濡傛灉A socket鐨勬帴鏀朵簨浠跺湪澶勭悊杩囩▼涓紝琚叾浠栫嚎绋嬪彂閫佹椂閿欒锛屽叧闂璼ocket锛屾鏃禩CPInPacket涓殑socket鍙ユ焺浼氳娓呯┖锛屽鑷撮敊璇�備絾鏄叾浠栫嚎绋嬬敱浜庨攣鐨勪綔鐢紝鏃犳硶娓呯┖鍏�
     * 姝ｅ湪澶勭悊鐨凾CPInPacket鍐呴儴鏁版嵁锛屾墍浠ヤ笉浼氬鑷村叾鎸囦护涔卞簭锛屼絾鏄綋鍏舵帴鏀跺紓甯革紝鍐嶆澶勭悊鍏抽棴鏃讹紝鐢变簬鏃犳硶鍦ㄥ瓧鍏镐腑鎵惧埌TCPInPacket鎵�浠ワ紝鏃犳硶瀵筎CPInPacket涓嶅啀澶勭悊锛孴CPInPacket
     * 琚叾浠栫嚎绋嬪鐞嗘竻绌烘帀锛屾鏃堕�昏緫鏄纭殑锛屽彧鏄細鍐嶄笉鍔犲垽鏂殑鎯呭喌涓嬪鑷村娆″噺灏憇ocket璁℃暟銆�
     * 2. 鍙戦�佹椂濡傛灉澶勭悊鍏抽棴鎿嶄綔锛屼細瀵艰嚧褰撳鏂瑰叧闂紝鑷繁鍙戦�佸け璐ユ椂锛屽娆℃墽琛屽叧闂搷浣溿�備絾鏄浜庢寚浠ゆ祦涔卞簭娌℃湁褰卞搷銆�
     * 3. 濡傛灉A socket鐨勬帴鏀舵椂闂村湪澶勭悊杩囩▼涓紝鎵ц鍙戦�佸嚭閿欙紝浼氬叧闂璼ocket锛屾墽琛屾竻绌哄唴瀛樼殑鎿嶄綔锛屾鏃禩CPInPacket涓殑socket鍙ユ焺浼氳娓呯┖锛屽鑷撮敊璇�傚苟涓旀病鏈夌粓姝㈡搷浣滐紝缁х画閫掑綊寰幆澶勭悊銆�
     * 鍚屾椂鐢变簬鑷繁灏員CPInPacket褰掕繕缁欎簡闃熷垪涓紝鍏朵粬鎺ユ敹灏变細鏈夋満浼氬彇鍑轰娇鐢紝褰撳叾浠杝ocket 鎺ユ敹鍙栧嚭浣跨敤鏃讹紝浼氶噸鏂板浜巗ocket璧嬪�硷紝瀵艰嚧A socket鐨勬帴鏀朵腑鐨勫彂閫佸鐞嗛�昏緫鍑洪敊锛屽彂閫佺粰浜�
     * 閿欒鐨勫鎴风瀵硅薄銆傚悓鏃讹紝鐢变簬姝ゆ椂鑷繁鐨勯�掑綊寰幆閫�鍑烘椂鍙兘浼氶仐鐣欏崐鎴寚浠ゆ暟鎹�傚叾浠栦娇鐢ㄨ繖涓猅CPInPacket鐨剆ocket鎺ユ敹鍒扮殑鏁版嵁锛岃繘鍘诲叾缁х画杩涜澶勭悊鍒欎細瀵艰嚧涔卞簭鐨勬寚浠ゆ祦鍑虹幇銆�
     * 灏辨槸A 鐨勯仐鐣欏崐鎴寚浠わ紝鍔犱笂鍏朵粬渚嬪B鐨勬柊鐨勬寚浠ゆ暟鎹紝瀵艰嚧B鍚庣画鎺ユ敹鍏ㄩ儴鍑洪敊銆傚懙鍛点�傜粓浜庢悶鎳備簡锛孻ES銆�
     * 瑙ｅ喅鏂规:
     * a. 鍙戦�佹椂涓嶈繘琛屽叧闂鐞嗭紝浜ょ粰鎺ユ敹鍘诲仛銆�
     * b. 鎺ユ敹鏃讹紝濡傛灉鍑虹幇閿欒锛岀粓姝㈤�掑綊寰幆锛岄��鍑恒��
     * c. 鎺ユ敹閮ㄥ垎锛屽彂閫佹暟鎹鐞嗗嚭閿欙紝鍒欑珛鍒绘墽琛屽叧闂璗MSKSocket鎿嶄綔銆�
     * d. 灏辨槸鍘熷垯涓婏紝鍏抽棴鎿嶄綔浜ょ粰鎺ユ敹鍘诲鐞嗐��
     */

    /// <summary>
    /// 鏈嶅姟鍣ㄤ睛鍚被
    /// </summary>
    public sealed class SocketListener
    {
        public int GTotalSendCount = 0;

        /// <summary>
        /// 鏄惁寮�鍚痠p鐧藉悕鍗曡繃婊�
        /// </summary>
        private bool EnabledIPListFilter = false;

        /// <summary>
        /// IP鐧藉悕鍗曞垪琛�
        /// </summary>
        private Dictionary<string, bool> IPWhiteList = new Dictionary<string, bool>();

        /// <summary>
        /// 缂撳啿鍖虹殑澶у皬
        /// </summary>
        Int32 ReceiveBufferSize;

        /// <summary>
        /// Represents a large reusable set of buffers for all socket operations.
        /// </summary>
        private BufferManager bufferManager;

        /// <summary>
        /// The socket used to listen for incoming connection requests.
        /// </summary>
        private TMSKSocket listenSocket;

        /// <summary>
        /// The total number of clients connected to the server.
        /// </summary>
        //private Int32 numConnectedSockets;

        /// <summary>
        /// 宸茬粡杩炴帴鐨凾MSKSocket璇嶅吀瀵硅薄
        /// </summary>
        private Dictionary<TMSKSocket, bool> ConnectedSocketsDict;

        /// <summary>
        /// 澶栭儴鑾峰彇鎬荤殑杩炴帴涓暟
        /// </summary>
        public Int32 ConnectedSocketsCount
        {
            get
            {
                Int32 n = 0;
                //Interlocked.Exchange(ref n, this.numConnectedSockets);
                lock (this.ConnectedSocketsDict)
                {
                    n = this.ConnectedSocketsDict.Count;
                }
                return n;
            }
        }

        /// <summary>
        /// the maximum number of connections the sample is designed to handle simultaneously.
        /// </summary>
        public Int32 numConnections;

        /// <summary>
        /// Read, write (don't alloc buffer space for accepts).
        /// </summary>
        private const Int32 opsToPreAlloc = 2;

        /// <summary>
        /// Pool of reusable SocketAsyncEventArgs objects for read socket operations.
        /// </summary>
        public SocketAsyncEventArgsPool readPool; //绾跨▼瀹夊叏

        public int ReadPoolCount
        {
            get { return readPool.Count; }
        }

        /// <summary>
        /// Pool of reusable SocketAsyncEventArgs objects for write socket operations.
        /// </summary>
        public SocketAsyncEventArgsPool writePool; //绾跨▼瀹夊叏

        public int WritePoolCount
        {
            get { return writePool.Count; }
        }

        /// <summary>
        /// Controls the total number of clients connected to the server.
        /// </summary>
        //private Semaphore semaphoreAcceptedClients;

        /// <summary>
        /// Total # bytes counter received by the server.
        /// </summary>
        private long totalBytesRead;

        /// <summary>
        /// 鑾峰彇鎬荤殑鎺ユ敹鐨勫瓧鑺傛暟
        /// </summary>
        public long TotalBytesReadSize
        {
            get
            {
                long n = 0;
                Interlocked.Exchange(ref n, this.totalBytesRead);
                return n;
            }
        }

        /// <summary>
        /// Total # bytes counter received by the server.
        /// </summary>
        private long totalBytesWrite;

        /// <summary>
        /// 鑾峰彇鎬荤殑鍙戦�佺殑瀛楄妭鏁�
        /// </summary>
        public long TotalBytesWriteSize
        {
            get
            {
                long n = 0;
                Interlocked.Exchange(ref n, this.totalBytesWrite);
                return n;
            }
        }

        //鏄惁涓嶅啀鎺ュ彈鏂扮殑鐢ㄦ埛(榛樿涓嶆帴鍙�)
        private bool _DontAccept = true;

        /// <summary>
        /// 鏄惁涓嶅啀鎺ュ彈鏂扮殑鐢ㄦ埛
        /// </summary>
        public bool DontAccept
        {
            get { return _DontAccept; }
            set { _DontAccept = value; }
        }

        /// 杩炴帴鎴愬姛閫氱煡鍑芥暟
        public event SocketConnectedEvnetHandler SocketConnected = null;

        /// 鏂紑鎴愬姛閫氱煡鍑芥暟
        public event SocketClosedEventHandler SocketClosed = null;

        /// 鎺ユ敹鏁版嵁閫氱煡鍑芥暟
        public event SocketReceivedEventHandler SocketReceived = null;

        /// 鍙戦�佹暟鎹�氱煡鍑芥暟
        public event SocketSendedEventHandler SocketSended = null;

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        /// </summary>
        /// <param name="numConnections">Maximum number of connections to be handled simultaneously.</param>
        /// <param name="receiveBufferSize">Buffer size to use for each socket I/O operation.</param>
        internal SocketListener(Int32 numConnections, Int32 receiveBufferSize)
        {
            this.totalBytesRead = 0;
            this.totalBytesWrite = 0;
            //this.numConnectedSockets = 0;
            this.numConnections = numConnections;
            this.ReceiveBufferSize = receiveBufferSize;

            int readBuffNum = numConnections * 3;

            // Allocate buffers such that the maximum number of sockets can have one outstanding read and 
            // write posted to the socket simultaneously .
            this.bufferManager = new BufferManager(receiveBufferSize * readBuffNum,
                receiveBufferSize);

            // 宸茬粡杩炴帴鐨凾MSKSocket璇嶅吀瀵硅薄
            this.ConnectedSocketsDict = new Dictionary<TMSKSocket, bool>(readBuffNum);

            this.readPool = new SocketAsyncEventArgsPool(readBuffNum);
            this.writePool = new SocketAsyncEventArgsPool(readBuffNum);
            //this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        /// <summary>
        /// 娣诲姞socket瀵硅薄
        /// </summary>
        /// <param name="socket"></param>
        private void AddSocket(TMSKSocket socket)
        {
            lock (this.ConnectedSocketsDict)
            {
                this.ConnectedSocketsDict.Add(socket, true);
            }
        }

        /// <summary>
        /// 鍒犻櫎socket瀵硅薄
        /// </summary>
        /// <param name="socket"></param>
        private void RemoveSocket(TMSKSocket socket)
        {
            lock (this.ConnectedSocketsDict)
            {
                this.ConnectedSocketsDict.Remove(socket);
            }
        }

        /// <summary>
        /// 鏌ユ壘TMSKSocket瀵硅薄
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private bool FindSocket(TMSKSocket socket)
        {
            bool ret = false;
            lock (this.ConnectedSocketsDict)
            {
                ret = this.ConnectedSocketsDict.ContainsKey(socket);
            }

            return ret;
        }

        /// <summary>
        /// Close the socket associated with the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private void CloseClientSocket(SocketAsyncEventArgs e, string reason)
        {
            AsyncUserToken aut = e.UserToken as AsyncUserToken;

            TMSKSocket s = null;
            try
            {
                s = aut.CurrentSocket;

                string ip = "鏈煡";

                try
                {
                    ip = string.Format("{0}", s.RemoteEndPoint);
                }
                catch (System.Exception)
                {
                }

                //LogManager.WriteLog(LogTypes.Error, string.Format("Close socket: {0}, Total sockets: {1}, Result 1: {2}, Result 2: {3}", ip, ConnectedSocketsCount, reason, s.CloseReason));
                CloseSocket(s);
            }
            finally
            {
                aut.CurrentSocket = null; //閲婃斁
                aut.Tag = null; //閲婃斁

                // Free the SocketAsyncEventArg so they can be reused by another client.
                if (e.LastOperation == SocketAsyncOperation.Send)
                {
                    e.SetBuffer(null, 0, 0); //鍥炴敹鍐呭瓨
                    if (GameManager.FlagOptimizeThreadPool3)
                    {
                        //TMSKThreadStaticClass.GetInstance().PushReadSocketAsyncEventArgs(e);
                        if (null != s) s.PushWriteSocketAsyncEventArgs(e);
                    }
                    else
                    {
                        this.writePool.Push(e);
                    }
                }
                else if (e.LastOperation == SocketAsyncOperation.Receive)
                {
                    if (GameManager.FlagOptimizeThreadPool3)
                    {
                        //TMSKThreadStaticClass.GetInstance().PushReadSocketAsyncEventArgs(e);
                        if (null != s) s.PushReadSocketAsyncEventArgs(e);
                    }
                    else
                    {
                        this.readPool.Push(e);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the server by preallocating reusable buffers and 
        /// context objects.  These objects do not need to be preallocated 
        /// or reused, but it is done this way to illustrate how the API can 
        /// easily be used to create reusable objects to increase server performance.
        /// </summary>
        internal void Init()
        {
            // Allocates one large Byte buffer which all I/O operations use a piece of. This guards 
            // against memory fragmentation.
            this.bufferManager.InitBuffer();

            // Preallocate pool of SocketAsyncEventArgs objects.
            SocketAsyncEventArgs readWriteEventArg;

            int readBuffNum = numConnections * 3;
            for (Int32 i = 0; i < readBuffNum; i++)
            {
                //for (int j = 0; j < opsToPreAlloc; j++)
                {
                    // Preallocate a set of reusable SocketAsyncEventArgs.
                    readWriteEventArg = new SocketAsyncEventArgs();
                    readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                    readWriteEventArg.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };

                    // Assign a Byte buffer from the buffer pool to the SocketAsyncEventArg object.
                    this.bufferManager.SetBuffer(readWriteEventArg);

                    // Add SocketAsyncEventArg to the pool.
                    this.readPool.Push(readWriteEventArg);
                }
            }

            for (Int32 i = 0; i < readBuffNum; i++)
            {
                //for (Int32 j = 0; j < 5; j++)
                //{
                    readWriteEventArg = new SocketAsyncEventArgs();
                    readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                    readWriteEventArg.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };
                    this.writePool.Push(readWriteEventArg);
                //}
            }
        }

        /// <summary>
        /// Callback method associated with TMSKSocket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (null == this.listenSocket) return; //宸茬粡涓嶅湪渚﹀惉

                this.ProcessAccept(e);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("Exception at SocketListener::OnAcceptCompleted.\n{0}", ex.ToString()));
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    //throw ex;
                //});
                DataHelper.WriteFormatExceptionLog(ex, "OnAcceptCompleted", false);
            }
        }

        /// <summary>
        /// Callback called whenever a receive or send operation is completed on a socket.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                // Determine which type of operation just completed and call the associated handler.
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        this.ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        this.ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("Exception at SocketListener::OnIOCompleted.\n{0}", ex.ToString()));
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    //throw ex;
                //});
                DataHelper.WriteFormatExceptionLog(ex, "OnIOCompleted", false);
            }
        }

        /// <summary>
        /// 浠巗ocket鎺ユ敹鏁版嵁
        /// </summary>
        /// <param name="s"></param>
        /// <param name="readEventArgs"></param>
        private bool _ReceiveAsync(SocketAsyncEventArgs readEventArgs)
        {
            try
            {
                TMSKSocket s = (readEventArgs.UserToken as AsyncUserToken).CurrentSocket;
                return s.ReceiveAsync(readEventArgs);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("Exception at SocketListener::_ReceiveAsync.\n{0}", ex.ToString()));
                string str = ex.Message.ToString();
                this.CloseClientSocket(readEventArgs, str.Replace('\n', ' '));
                return true;
            }
        }

        /// <summary>
        /// 鍚慣MSKSocket鍙戦�佹暟鎹�
        /// </summary>
        /// <param name="writeEventArgs"></param>
        /// <returns></returns>
        private bool _SendAsync(SocketAsyncEventArgs writeEventArgs, out bool exception)
        {
            exception = false;

            try
            {
                TMSKSocket s = (writeEventArgs.UserToken as AsyncUserToken).CurrentSocket;
                return s.SendAsync(writeEventArgs);
            }
            catch (Exception ex) //姝ゅ鏈夊彲鑳芥槸瀵硅薄闈炴硶绛夊紓甯�, 渚嬪TMSKSocket瀵硅薄宸茬粡鏃犳晥
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("Exception at SocketListener::_SendAsync.\n{0}", ex.ToString()));
                exception = true;
                //this.CloseClientSocket(writeEventArgs);
                return true;
            }
        }

        /// <summary>
        /// 鍚戝鎴风鍙戦�佹暟鎹�
        /// </summary>
        /// <param name="data"></param>
        internal bool SendData(TMSKSocket s, TCPOutPacket tcpOutPacket, bool pushBack = true)
        {
            if (s != null && tcpOutPacket != null)
            {
                // 骞挎挱鐨勬椂鍊欏涓鎴风鍚屾椂浣跨敤鍚屼竴涓猙yte[]锛岃鐗瑰埆娉ㄦ剰
                UInt16 SendPacketId = tcpOutPacket.PacketCmdID;
                Array.Copy(BitConverter.GetBytes(SendPacketId), 0, tcpOutPacket.GetPacketBytes(), 4, 2);
            }

#if true //鍘熸潵鐨勭洿鎺ュ彂閫侊紝鏀逛负寮傛绾跨▼鍙戦�侊紝鍑忓皯閿佸畾

            bool bRet = false;
            if (null != s)
            {
                // 鍙湁杩炴帴鐘舵�佹墠鍙戦�佹秷鎭� ChenXiaojun
                if ((s as TMSKSocket).Connected)
                {
                    //缂撳啿鏁版嵁鍖�
                    if (GameManager.FlagSkipSendDataCall)
                    {
                        bRet = true;
                    }
                    else
                    {
                        bRet = Global._SendBufferManager.AddOutPacket(s, tcpOutPacket);
                    }
                }
            }

            //杩樺洖tcpoutpacket
            if (pushBack)
            {
                Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
            }

            return bRet;
#else

            //灏嗗彂閫佺殑鎸囦护閫佸埌寮傛闃熷垪
            SendPacketManager.getInstance().addSendPacketWrapper(new SendPacketWrapper()
                {
                    socket = s,
                    tcpOutPacket = tcpOutPacket,
                });

            return true;
#endif //
        }

        /// <summary>
        /// 鍚戝鎴风鍙戦�佹暟鎹�
        /// </summary>
        /// <param name="data"></param>
        public bool SendData(TMSKSocket s, Byte[] buffer, int offset, int count, MemoryBlock item)
        {
            GTotalSendCount++;
            SocketAsyncEventArgs writeEventArgs;
            if (GameManager.FlagOptimizeThreadPool3)
            {
                writeEventArgs = s.PopWriteSocketAsyncEventArgs(); //绾跨▼瀹夊叏鐨勬搷浣�
            }
            else
            {
                writeEventArgs = this.writePool.Pop(); //绾跨▼瀹夊叏鐨勬搷浣�
            }
            if (null == writeEventArgs)
            {
                writeEventArgs = new SocketAsyncEventArgs();
                writeEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                writeEventArgs.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };
            }

            writeEventArgs.SetBuffer(buffer, offset, count);
            AsyncUserToken userToken = (writeEventArgs.UserToken as AsyncUserToken);
            userToken.CurrentSocket = s;
            userToken.Tag = item;

            bool exception = false;
            if (GameManager.FlagSkipSocketSend)
            {
                writeEventArgs.SocketError = SocketError.Success;
                this.ProcessSend(writeEventArgs);
            }
            else
            {
                Boolean willRaiseEvent = _SendAsync(writeEventArgs, out exception);
                if (!willRaiseEvent)
                {
                    this.ProcessSend(writeEventArgs);
                }
            }

            if (exception) //姝ゅ涓嶅鐞嗕細瀵艰嚧鍐呭瓨娉勯湶
            {
                /// 鍙戦�佹暟鎹�氱煡鍑芥暟
                if (null != SocketSended)
                {
                    SocketSended(this, writeEventArgs);
                }

                //浠�涔堜簨鎯呴兘涓嶅仛, 鏀跺洖浣跨敤鐨別鍜宐uffer
                // Free the SocketAsyncEventArg so they can be reused by another client.
                writeEventArgs.SetBuffer(null, 0, 0); //鍥炴敹鍐呭瓨
                userToken.CurrentSocket = null; //閲婃斁
                userToken.Tag = null; //閲婃斁
                if (GameManager.FlagOptimizeThreadPool3)
                {
                    //TMSKThreadStaticClass.GetInstance().PushSocketAsyncEventArgs(writeEventArgs);
                    s.PushWriteSocketAsyncEventArgs(writeEventArgs);
                }
                else
                {
                    this.writePool.Push(writeEventArgs);
                }
            }

            return (!exception);
        }

        /// <summary>
        /// 鍚戝鎴风鍙戦�佹暟鎹�
        /// </summary>
        /// <param name="data"></param>
        public bool SendData(TMSKSocket s, Byte[] buffer, int offset, int count, MemoryBlock item, SendBuffer sendBuffer)
        {
            GTotalSendCount++;
            SocketAsyncEventArgs writeEventArgs;
            if (GameManager.FlagOptimizeThreadPool3)
            {
                writeEventArgs = s.PopWriteSocketAsyncEventArgs(); //绾跨▼瀹夊叏鐨勬搷浣�
            }
            else
            {
                writeEventArgs = this.writePool.Pop(); //绾跨▼瀹夊叏鐨勬搷浣�
            }
            if (null == writeEventArgs)
            {
                writeEventArgs = new SocketAsyncEventArgs();
                writeEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                writeEventArgs.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };
            }

            writeEventArgs.SetBuffer(buffer, offset, count);
            AsyncUserToken userToken = (writeEventArgs.UserToken as AsyncUserToken);
            userToken.CurrentSocket = s;
            userToken.Tag = item;
            userToken._SendBuffer = sendBuffer;

            bool exception = false;
            if (GameManager.FlagSkipSocketSend)
            {
                writeEventArgs.SocketError = SocketError.Success;
                this.ProcessSend(writeEventArgs);
            }
            else
            {
                Boolean willRaiseEvent = _SendAsync(writeEventArgs, out exception);
                if (!willRaiseEvent)
                {
                    this.ProcessSend(writeEventArgs);
                }
            }

            if (exception) //姝ゅ涓嶅鐞嗕細瀵艰嚧鍐呭瓨娉勯湶
            {
                /// 鍙戦�佹暟鎹�氱煡鍑芥暟
                if (null != SocketSended)
                {
                    SocketSended(this, writeEventArgs);
                }

                //浠�涔堜簨鎯呴兘涓嶅仛, 鏀跺洖浣跨敤鐨別鍜宐uffer
                // Free the SocketAsyncEventArg so they can be reused by another client.
                writeEventArgs.SetBuffer(null, 0, 0); //鍥炴敹鍐呭瓨
                userToken.CurrentSocket = null; //閲婃斁
                userToken.Tag = null; //閲婃斁
                if (GameManager.FlagOptimizeThreadPool3)
                {
                    s.PushWriteSocketAsyncEventArgs(writeEventArgs);
                }
                else
                {
                    this.writePool.Push(writeEventArgs);
                }
            }

            return (!exception);
        }

        /// <summary>
        /// Process the accept for the socket listener.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            //Socket s = e.AcceptSocket;
            TMSKSocket s = new TMSKSocket(e.AcceptSocket);

            //妫�鏌P鍚嶅崟
            bool disableConnect = false;
            bool? inIpWriteList = null;
            if (EnabledIPListFilter)
            {
                lock (IPWhiteList)
                {
                    if (EnabledIPListFilter && null != s && null != s.RemoteEndPoint)
                    {
                        IPEndPoint remoteIPEndPoint = (s.RemoteEndPoint as IPEndPoint);
                        if (null != remoteIPEndPoint && null != remoteIPEndPoint.Address)
                        {
                            string remoteIP = remoteIPEndPoint.Address.ToString();
                            if (!string.IsNullOrEmpty(remoteIP) && !IPWhiteList.ContainsKey(remoteIP))
                            {
                                disableConnect = true;
                                inIpWriteList = false;
                            }
                            else
                            {
                                inIpWriteList = true;
                            }
                        }
                    }
                }
            }

            //鏄惁涓嶅啀鎺ュ彈鏂扮殑鐢ㄦ埛
            if (DontAccept || disableConnect)
            {
                try
                {
                    if (disableConnect)
                    {
                        //LogManager.WriteLog(LogTypes.Error, string.Format("New connection: {0}, but IP address was banned锛孎orce close: {1}", s.RemoteEndPoint, ConnectedSocketsCount));
                    }
                    else if(DontAccept)
                    {
                        //LogManager.WriteLog(LogTypes.Error, string.Format("New connection: {0}, but server don't accept锛孎orce close: {1}", s.RemoteEndPoint, ConnectedSocketsCount));
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    s.Shutdown(SocketShutdown.Receive);
                }
                catch (Exception)
                {
                    // Throws if client process has already closed.
                }

                try
                {
                    s.Close(30);
                }
                catch (Exception)
                {
                }

                // Accept the next connection request.
                this.StartAccept(e);
                return;
            }

            byte[] inOptionValues = new byte[sizeof(uint) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)120000).CopyTo(inOptionValues, sizeof(uint));
            BitConverter.GetBytes((uint)5000).CopyTo(inOptionValues, sizeof(uint) * 2);
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    (s as TMSKSocket).IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
                }
                catch { }
            }
            else
            {
                try
                {
                    (s as TMSKSocket).SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, true);
                    (s as TMSKSocket).SetSocketOption(System.Net.Sockets.SocketOptionLevel.Tcp, System.Net.Sockets.SocketOptionName.TcpKeepAliveTime, 120);
                    (s as TMSKSocket).SetSocketOption(System.Net.Sockets.SocketOptionLevel.Tcp, System.Net.Sockets.SocketOptionName.TcpKeepAliveInterval, 5);
                }
                catch { }
            }

            SocketAsyncEventArgs readEventArgs = null;
            if (GameManager.FlagOptimizeThreadPool3)
            {
                readEventArgs = s.PopReadSocketAsyncEventArgs(); //绾跨▼瀹夊叏鐨勬搷浣�
            }
            else
            {
                readEventArgs = this.readPool.Pop();
            }
            if (null == readEventArgs)
            {
                try
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("New connect: {0}, but pool has already full锛宖orce close: {1}", s.RemoteEndPoint, ConnectedSocketsCount));
                }
                catch (Exception)
                {
                }

                try
                {
                    s.Shutdown(SocketShutdown.Receive);
                }
                catch (Exception)
                {
                    // Throws if client process has already closed.
                }

                try
                {
                    s.Close(30);
                }
                catch (Exception)
                {
                }

                // Accept the next connection request.
                this.StartAccept(e);
                return;
            }

            //澧炲姞璁℃暟鍣�
            //Interlocked.Increment(ref this.numConnectedSockets);

            // Get the socket for the accepted client connection and put it into the 
            // ReadEventArg object user token.
            (readEventArgs.UserToken as AsyncUserToken).CurrentSocket = s;

            Global._SendBufferManager.Add(s);

            AddSocket(s);

           // try
           // {
           //     //LogManager.WriteLog(LogTypes.Error, string.Format("New connection: {0}, Total sockets: {1}", s.RemoteEndPoint, ConnectedSocketsCount));
           // }
            //catch (Exception)
           // {
           // }

            /// 杩炴帴鎴愬姛閫氱煡鍑芥暟
            if (null != SocketConnected)
            {
                SocketConnected(this, readEventArgs);
            }

            s.session.InIpWhiteList = inIpWriteList;
            // As soon as the client is connected, post a receive to the connection.
            Boolean willRaiseEvent = _ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                this.ProcessReceive(readEventArgs);
            }

            // Accept the next connection request.
            this.StartAccept(e);
        }

        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation.</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = (e.UserToken as AsyncUserToken);
            TMSKSocket s = userToken.CurrentSocket;

            // Check if the remote host closed the connection.
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // Increment the count of the total bytes receive by the server.
                Interlocked.Add(ref this.totalBytesRead, e.BytesTransferred);

                bool recvReturn = true;

                // Get the message received from the listener.
                //e.Buffer, e.Offset, e.bytesTransferred);
                //閫氱煡澶栭儴鏈夋柊鐨剆ocket鍒拌揪
                if (null != SocketReceived)
                {
                    //瀛楄妭鎺掑簭
                    if (GameManager.FlagUseWin32Decrypt)
                    {
                        //浣跨敤鍔ㄦ�佸簱瑙ｅ瘑
                        int length = e.BytesTransferred;
                        unsafe
                        {
                            fixed (byte* p = e.Buffer)
                            {
                               Win32API.SortBytes(p, e.Offset, e.BytesTransferred, s.SortKey64);
                            }
                        }
                    }
                    else
                    {
                       // Win32API.SortBytes(e.Buffer, e.Offset, e.BytesTransferred, s.SortKey64);
                       DataHelper.SortBytes(e.Buffer, e.Offset, e.BytesTransferred, s.SortKey64);
                    }

                    try
                    {
                        recvReturn = SocketReceived(this, e);
                    }
                    catch (System.Exception ex)
                    {
                        //鎵�鏈夊紓甯稿簲褰撳湪涓嬪眰鎹曡幏锛屼笉搴旀姏鍒拌繖灞傛潵
                        LogManager.WriteException(ex.ToString());
                        recvReturn = false;
                    }
                }

                if (recvReturn)
                {
                    //缁х画鎺ユ敹娴佺▼(浠ユ暟鎹祦涓洪┍鍔�)
                    Boolean willRaiseEvent = _ReceiveAsync(e);
                    if (!willRaiseEvent)
                    {
                        this.ProcessReceive(e);
                    }
                }
                else
                {
                    UInt16 lastPacketCmd = TCPManager.getInstance().LastPacketCmdID(s);
                    string reason = string.Format("CMD={0}", ((TCPGameServerCmds)lastPacketCmd).ToString());
                    this.CloseClientSocket(e, reason);
                }
            }
            else
            {
                string reason = string.Format("[{0}]{1}", (int)e.SocketError, e.SocketError.ToString());
                this.CloseClientSocket(e, reason);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send operation.</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
             /// 鍙戦�佹暟鎹�氱煡鍑芥暟
            if (null != SocketSended)
            {
                SocketSended(this, e);
            }

            if (e.SocketError == SocketError.Success)
            {
                Interlocked.Add(ref this.totalBytesWrite, e.BytesTransferred);
            }
            else
            {
                //this.CloseClientSocket(e);
            }

            //浠�涔堜簨鎯呴兘涓嶅仛, 鏀跺洖浣跨敤鐨別鍜宐uffer
            // Free the SocketAsyncEventArg so they can be reused by another client.
            e.SetBuffer(null, 0, 0); //鍥炴敹鍐呭瓨
            TMSKSocket s = (e.UserToken as AsyncUserToken).CurrentSocket;
            (e.UserToken as AsyncUserToken).CurrentSocket = null; //閲婃斁
            (e.UserToken as AsyncUserToken).Tag = null; //閲婃斁
            if (GameManager.FlagOptimizeThreadPool3)
            {
                if(null != s) s.PushWriteSocketAsyncEventArgs(e);
            }
            else
            {
                this.writePool.Push(e);
            }
        }

        /// <summary>
        /// Starts the server such that it is listening for incoming connection requests.    
        /// </summary>
        /// <param name="localEndPoint">The endpoint which the server will listening for connection requests on.</param>
        internal void Start(string ip, int port)
        {
            if ("" == ip) ip = "0.0.0.0"; //闃叉IP鏃犳晥

            // Get endpoint for the listener.
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            // Create the socket which listens for incoming connections.
            this.listenSocket = new TMSKSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Associate the socket with the local endpoint.
            this.listenSocket.Bind(localEndPoint);

            // Start the server with a listen backlog of 100 connections.
            this.listenSocket.Listen(100);

            // Post accepts on the listening socket.
            this.StartAccept(null);
        }
        
        /// <summary>
        /// 鍏抽棴渚﹀惉鐨凾MSKSocket
        /// </summary>
        public void Stop()
        {
            TMSKSocket s = this.listenSocket;
            this.listenSocket = null;
            s.Close();
        }

        /// <summary>
        /// 鍏抽棴鎸囧畾鐨凾MSKSocket杩炴帴
        /// </summary>
        /// <param name="s"></param>
        /// <param name="reason">寮哄埗鍏抽棴鐨勫師鍥�</param>
        public bool CloseSocket(TMSKSocket s, string reason = "")
        {
            //try
            //{               
            //    s.Shutdown(SocketShutdown.Both);

            //    // 闇�瑕丆lose 涓嶇劧涓嶄細鏀跺埌濂楁帴瀛楀叧闂� ChenXiaojun
            //    s.Close();
            //}
            //catch (Exception ex)
            //{
            //    // Throws if client process has already closed.
            //    try
            //    {
            //        LogManager.WriteLog(LogTypes.Info, string.Format("CloseSocket 寮傚父: {0}, {1}", s.RemoteEndPoint, ex.Message));
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            if (!FindSocket(s)) //宸茬粡鍏抽棴浜�
            {
                //宸茬粡鍏抽棴涔嬪悗鍐嶈皟鐢ㄤ竴娆�
                Global._SendBufferManager.Remove(s);
                return false;
            }

            if (!string.IsNullOrEmpty(reason))
            {
                s.CloseReason = reason;
            }
            RemoveSocket(s);

            /// 鏂紑鎴愬姛閫氱煡鍑芥暟
            if (null != SocketClosed)
            {
                SocketClosed(this, s);
            }

            //鍦ㄨ繖涓綅缃皟鐢紝閬垮厤閬楁紡,閲嶅璋冪敤涓嶄細鍑洪敊
            Global._SendBufferManager.Remove(s);

            try
            {
                s.Shutdown(SocketShutdown.Receive);
            }
            catch (Exception ex)
            {
                // Throws if client process has already closed.
                try
                {
                    LogManager.WriteLog(LogTypes.Info, string.Format("CloseSocket s.Shutdown() error: {0}, {1}", s.RemoteEndPoint, ex.Message));
                }
                catch (Exception)
                {
                }
            }

            try
            {
                s.Close(30);
            }
            catch (Exception ex)
            {
                // Throws if client process has already closed.
                try
                {
                    LogManager.WriteLog(LogTypes.Info, string.Format("CloseSocket s.Close() error: {0}, {1}", s.RemoteEndPoint, ex.Message));
                }
                catch (Exception)
                {
                }
            }

            //鐢辨帴鏀朵簨浠跺幓閲婃斁澶勭悊
            return true;
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client.
        /// </summary>
        /// <param name="acceptEventArg">The context object to use when issuing 
        /// the accept operation on the server's listening socket.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                // TMSKSocket must be cleared since the context object is being reused.
                acceptEventArg.AcceptSocket = null;
            }

            //this.semaphoreAcceptedClients.WaitOne(); //鎺у埗鎬荤殑杩炴帴鏁�
            Boolean willRaiseEvent = this.listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                this.ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 鍒濆鍖朓P鐧藉悕鍗曞垪琛�
        /// </summary>
        /// <param name="ipList"></param>
        public List<string> InitIPWhiteList(string[] ipList, bool enabeld = true)
        {
            List<string> resultList = new List<string>();
            lock (IPWhiteList)
            {
                EnabledIPListFilter = false;
                IPWhiteList.Clear();
                if (null != ipList && ipList.Length > 0)
                {
                    foreach (var ipStr in ipList)
                    {
                        IPAddress ipAddress;
                        if (IPAddress.TryParse(ipStr, out ipAddress))
                        {
                            resultList.Add(ipAddress.ToString());
                            IPWhiteList[ipAddress.ToString()] = true;
                        }
                    }

                    if (IPWhiteList.Count > 0)
                    {
                        EnabledIPListFilter = enabeld;
                    }
                }

                return resultList;
            }
        }

        public void ClearTimeoutSocket()
        {
            bool ret = false;
            long nowTicks = TimeUtil.NOW();
            lock (this.ConnectedSocketsDict)
            {
                foreach (var socket in this.ConnectedSocketsDict.Keys)
                {
                    if (socket.session.SocketTime[1] == 0 && socket.Connected)
                    {
                        if (Math.Abs(nowTicks - socket.session.SocketTime[0]) > 30 * 1000)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(socket.CloseReason))
                                {
                                    socket.CloseReason = "ClearTimeoutSocket";
                                }

                                socket.Shutdown(SocketShutdown.Receive);
                            }
                            catch { }
                            try
                            {
                                socket.Close(30);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }
}
