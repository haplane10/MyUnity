using System;
using System.Collections.Generic;
using System.Text;

namespace LogicServer
{
    class MessageTags
    {
        public const ushort START_TRAINING = 1;

        // CLIENT CUSTOM PACKETS < 1000
        // sendtype:0 SEND TO ALL
        // sendtype:1 SEND TO SQUAD PLAYERS
        // 명칭 등 자유롭게 사용
        public const ushort INIT = 9;
        public const ushort ANIMATOR = 10;
        public const ushort TRANSFORM = 11;         // CS{sendtype:byte ...} SC{checkflag:uint32 ...} type:0 ALL
        public const ushort CHARACTER_ACT = 12;          // CS{sendtype:byte ...} SC{checkflag:uint32 ...}
        public const ushort LIGHTMAP_SWITCH = 13;
        public const ushort VIDEO_STREAMING = 14;
        public const ushort VIDEO_RESOLUTION = 15;
        // 클라이언트 - 서버 통신패킷 정의
        // RESERVED PACKETS >= 1000
        // LOGIN 1000 ~ 1999
        // LOGIC 2000 ~

        // LOGIN
        // 로그인
        public const ushort LOGIN_ACCOUNT = 1001;        // <-> CS{loginid:string loginpw:string} SC{result:short uid:ulong name:string department:string armycode:string rank:ushort servercount:ushort [ip:string port:ushort type:string]}

                          
        // LOGIC
        // 서버입장
        public const ushort LOGIC_ACCOUNT_ASSIGN = 2101; // <-> CS{uid:ulong} SC{result:byte}

        // 훈련 시작
        public const ushort TRAINING_START = 2001;       // <-> CS{type:ushort} SC{result:byte type:ushort}

        //Prob 테스트
        public const ushort Prob_START = 3030;          

    }
}
