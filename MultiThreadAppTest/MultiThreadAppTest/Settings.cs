using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace MultiThreadAppTest
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    [ProtoContract]
    internal class Settings
    {
        [ProtoMember(1)]
        public int MaxItemCount { get; set; }

        [ProtoMember(2)]
        public int ClipboardClearLimit { get; set; }

        [ProtoMember(3)]
        public int AppLockLimit { get; set; }

        [ProtoMember(4)]
        public bool UseMigemo { get; set; }
    }
}
