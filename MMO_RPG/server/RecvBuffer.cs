using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;

        //커서의 역할을 하게 됨
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        //데이터가 얼마나 쌓여있는지 확인
        public int DataSize { get { return _writePos - _readPos; } }
        //버퍼에 남은 공간
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        //현재까지 받은 데이터의 범위
        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }
        
        //사용 하능한 유효 범위
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        //중간중간 정리해주는 역할, 버퍼 끝까지 가지 않도록 당겨줌
        public void Clean()
        {
            int dataSize = DataSize;
            //readPos와 writePos가 겹치는 상태, 모든 데이터를 처리한 상태
            if(dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            else
            {
                //남은 데이터가 있는 상태, 시작 위치로 복사 _writePos는 _readPos 와의 거리만큼 떨어진 곳으로 이동
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        //데이터 가공을 성공적으로 끝냈는지를 확인
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;
            
            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;
            
            _writePos += numOfBytes;
            return true;
        }
    }
}
