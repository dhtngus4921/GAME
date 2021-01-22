﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class PlayerInfoReq
    {
        public long playerId; //8
        public string name; //가변적, 크기 예상 x

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += sizeof(ushort);
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += sizeof(ushort);
            //유효 범위 이외에도 충분한 공간이 있는지, 데이터가 있는지 확인해야 한다 
            //범위를 지정해주는 형태
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            //skill list
            //skill이 몇개 들어가 있는지 먼저 추출
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            skills.Clear();
            for(int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            //BitConverter.getBytes: 넣어준 크기 만큼 생성해준다는 이야기
            //&연산으로 받아서 한번이라도 실패하면 success = false;
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq;);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);
            //패킷 헤더에 count 대신 임의의 byte를 넣으면 그 값으로 count가 바껴서 출력
            success &= BitConverter.TryWriteBytes(s, count);

            //UTF-16, string
            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            //success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            //count += sizeof(ushort);
            //Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            //count += nameLen;

            //UTF-16, string, 최적화한 코드 위의 내용과 같음
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            //skills
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            //roof를 돌면서 skill 하나씩 span으로 집어준 영역에 write하게 됨
            foreach (SkillInfo skill in skills)
                success &= skill.Write(s, ref count);

            success &= BitConverter.TryWriteBytes(s, count);

            if(success == false)
                return null;

            return SendBufferHelper.Close(count);
        }
    }
      
    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() {playerId = 1001, name = "ABCD"};
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 102, level = 2, duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 103, level = 3, duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 104, level = 4, duration = 6.0f });

            //for (int i = 0; i < 5; i++)
            {
                //버퍼를 꺼내와 써주는 작업
                ArraySegment<byte> s = packet.Write();

                if(s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected bytes: {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            //받아 온 데이터를 긁어 recvData에 저장해줌
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[from Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBuffer)
        {
            Console.WriteLine($"Transferred bytes: {numOfBuffer}");
        }
    }
}