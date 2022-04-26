using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServiceFabric.Remoting.Json
{
    internal class SegmentedPoolMemoryStream : Stream
    {
        private readonly IBufferPoolManager _bufferPoolManager;
        private List<IPooledBuffer> _writeBuffers;
        private bool _canRead;
        private bool _canSeek;
        private bool _canWrite;
        private long _position;
        private int _currentBufferOffset;
        private IPooledBuffer _currentBuffer;

        public SegmentedPoolMemoryStream(IBufferPoolManager bufferPoolManager)
        {
            _bufferPoolManager = bufferPoolManager;
            Initialize();
        }

        private void Initialize()
        {
            _canWrite = true;
            _canRead = false;
            _canSeek = false;
            _position = 0;
            _writeBuffers = new List<IPooledBuffer>(1);
            _currentBuffer = _bufferPoolManager.TakeBuffer();
            _writeBuffers.Add(_currentBuffer);
            BufferSize = _writeBuffers[0].Value.Count;
            _currentBufferOffset = 0;
        }

        public override bool CanRead => _canRead;

        public override bool CanSeek => _canSeek;

        public override bool CanWrite => _canWrite;

        public override long Length => _position;

        public override long Position
        {
            get => _position;
            set => _position = value;
        }
        public int BufferSize { get; set; }

        public override void Flush()
        {
            //no-op
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            _position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if ((offset + count) > buffer.Length)
            {
                throw new ArgumentException("buffer too small", nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentException("offset must be >= 0", nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentException("count must be >= 0", nameof(count));
            }

            var i = _currentBufferOffset + count;

            if (i <= BufferSize)
            {
                Buffer.BlockCopy(buffer, offset, _currentBuffer.Value.Array, _currentBufferOffset,
                    count);
                _currentBuffer.ContentLength += count;
                _currentBufferOffset += count;
                _position += count;
                return;
            }

            var bytesLeft = count;

            while (bytesLeft > 0)
            {
                //check for buffer full
                if (BufferSize <= _currentBufferOffset)
                {
                    //Create new buffer and Add to buffer

                    _currentBuffer = _bufferPoolManager.TakeBuffer();

                    _writeBuffers.Add(_currentBuffer);
                    _currentBufferOffset = 0;
                }

                var bytesToCopy = (_currentBufferOffset + bytesLeft) <= BufferSize
                    ? bytesLeft
                    : BufferSize - _currentBufferOffset;

                Buffer.BlockCopy(buffer, offset, _currentBuffer.Value.Array, _currentBufferOffset,
                    bytesToCopy);

                _currentBuffer.ContentLength += bytesToCopy;

                _position += bytesToCopy;
                offset += bytesToCopy;
                bytesLeft -= bytesToCopy;
                _currentBufferOffset += bytesToCopy;
            }
        }

        public override void WriteByte(byte value)
        {
            var i = _currentBufferOffset + 1;

            if (i > BufferSize)
            {
                _currentBuffer = _bufferPoolManager.TakeBuffer();
                _writeBuffers.Add(_currentBuffer);
                _currentBufferOffset = 0;
            }

            _currentBuffer.Value.Array[_currentBufferOffset] = value;
            _currentBuffer.ContentLength += 1;
            _position += 1;
        }

#pragma warning disable CA1024 // Use properties where appropriate
        public IEnumerable<IPooledBuffer> GetBuffers()
#pragma warning restore CA1024 // Use properties where appropriate
        {
            if (!CanWrite)
            {
                throw new NotImplementedException();
            }

            return _writeBuffers;
        }
    }
}
