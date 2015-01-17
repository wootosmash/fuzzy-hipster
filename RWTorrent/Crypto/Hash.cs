/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 16/01/2015
 * Time: 11:35 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security.Cryptography;

namespace FuzzyHipster.Crypto
{
  /// <summary>
  /// Description of Hash.
  /// </summary>
  public class Hash
  {
    public Hash()
    {
    }
    
    public static byte[] GetHash( Stream stream, long length )
    {
      using ( var md5 = MD5.Create())
      {
        using ( var partial = new PartialStream(stream, 0, length))
        {
          return md5.ComputeHash(stream);
        }
      }
    }
    
    public static byte[] GetHash( string filename )
    {
      using ( var md5 = MD5.Create())
      {
        using (var stream = File.OpenRead(filename))
        {
          return md5.ComputeHash(stream);
        }
      }
    }
    
    public static byte[] GetHash( byte[] data )
    {
      using ( var md5 = MD5.Create())
      {
        using ( var stream = new MemoryStream(data))
        {
          return md5.ComputeHash(stream);
        }
      }
    }
    
    public static bool Compare( byte[] hash1, byte[] hash2 )
    {
      if ( hash1.Length != hash2.Length )
        return false;
      
      for ( int i=0;i<hash1.Length;i++)
        if ( hash2[i] != hash1[i] )
          return false;
      
      return true;
    }
    

    
  }
  
  public class PartialStream : Stream
  {
    private readonly Stream _UnderlyingStream;
    private readonly long _Position;
    private readonly long _Length;

    public PartialStream(Stream underlyingStream, long position, long length)
    {
      if (!underlyingStream.CanRead ) //|| !underlyingStream.CanSeek)
        throw new ArgumentException(string.Format("underlyingStream CanRead={0} CanSeek={1}", underlyingStream.CanRead, underlyingStream.CanSeek));

      _UnderlyingStream = underlyingStream;
      _Position = position;
      _Length = length;
      _UnderlyingStream.Position = position;
    }

    public override bool CanRead
    {
      get
      {
        return _UnderlyingStream.CanRead;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return true;
      }
    }

    public override long Length
    {
      get
      {
        return _Length;
      }
    }

    public override long Position
    {
      get
      {
        return _UnderlyingStream.Position - _Position;
      }

      set
      {
        _UnderlyingStream.Position = value + _Position;
      }
    }

    public override void Flush()
    {
      throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          return _UnderlyingStream.Seek(_Position + offset, SeekOrigin.Begin) - _Position;

        case SeekOrigin.End:
          return _UnderlyingStream.Seek(_Length + offset, SeekOrigin.Begin) - _Position;

        case SeekOrigin.Current:
          return _UnderlyingStream.Seek(offset, SeekOrigin.Current) - _Position;

        default:
          throw new ArgumentException("origin");
      }
    }

    public override void SetLength(long length)
    {
      throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      long left = _Length - Position;
      if (left < count)
        count = (int)left;
      return _UnderlyingStream.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }
  }
  
}
