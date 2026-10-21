using GameServer.KiemThe.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.KiemThe.Logic
{
    /// <summary>
    /// Đối tượng sự kiện
    /// </summary>
    public class KTActivity
    {
        /// <summary>
        /// Thông tin sự kiên
        /// </summary>
        public ActivityXML Data { get; set; }

        /// <summary>
        /// Đã bắt đầu chưa
        /// </summary>
        public bool IsStarted { get; set; }

        public bool IsOver => !IsStarted;
        public long LifeTime { get; set; } = 0;
        private readonly Dictionary<int, long> _params = new Dictionary<int, long>();
        public long GetParam(int index) => _params.TryGetValue(index, out long val) ? val : 0;
        public void SetParam(int index, long val) => _params[index] = val;
    }
}
