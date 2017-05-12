using System;
using System.IO;

namespace Dorado.Spider
{
    /// <summary>
    /// SpiderInputStream: This class is used by the spider to
    /// both parse and save an InputStream.
    /// </summary>
    public class SpiderInputStream : Stream
    {
        /// <summary>
        /// Reports that this Stream can read.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Reports that this Stream can not seek.
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Reports that this Stream can not write.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Flush the underlying output stream.
        /// </summary>
        public override void Flush()
        {
            _ostream.Flush();
        }

        /// <summary>
        /// Getting the length is not supported.  This will
        /// throw an exception.
        /// </summary>
        public override long Length
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Getting or setting the position is not supported, this
        /// will throw an exception.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// The OutputStream that this class will send all output to.
        /// </summary>
        public Stream OutputStream
        {
            get { return _ostream; }
            set { this._ostream = value; }
        }

        /// <summary>
        /// The input stream to read from.
        /// </summary>
        private readonly Stream _istream;

        /// <summary>
        /// The output stream to write to.
        /// </summary>
        private Stream _ostream;

        /// <summary>
        /// Construct a SpiderInputStream object.
        /// </summary>
        /// <param name="istream">The input stream.</param>
        /// <param name="ostream">The output stream.</param>
        public SpiderInputStream(Stream istream, Stream ostream)
        {
            this._istream = istream;
            this._ostream = ostream;
        }

        /// <summary>
        /// Read bytes from the underlying input stream and also
        /// write them to the output stream.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="offset">The offset into the buffer to begin reading at.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = _istream.Read(buffer, offset, count);
            if (_ostream != null)
            {
                _ostream.Write(buffer, offset, count);
            }
            return result;
        }

        /// <summary>
        /// This operation is not supported, and will throw an
        /// exception.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The origin.</param>
        /// <returns>How many bytes were read.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// This operation is not supported, and will throw an
        /// exception.
        /// </summary>
        /// <param name="value">The length</param>
        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// This operation is not supported, and will throw an
        /// exception.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}