/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 16/01/2015
 * Time: 11:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace FuzzyHipster.Catalog
{
  /// <summary>
  /// Streams a block from a multitude of files
  /// </summary>
  public class BlockStream : Stream
  {
    public FileWad Wad { get; set; }
    public int CurrentBlock { get; protected set; }
    public FileDescriptor CurrentFile { get; protected set; }
    
    public BlockStream( FileWad wad )
    {
      Wad = wad;
      CurrentFile = null;
    }

    public void SeekBlock( int block )
    {
      CurrentBlock = block;
    }
    
    public FileDescriptor GetNext()
    {
      currentFile++;
      if ( currentFile < Wad.Files.Count )
        CurrentFile = Wad.Files[currentFile];
      else
        CurrentFile = null;
      return CurrentFile;
    }
    int currentFile = -1;
    
    
    #region implemented abstract members of Stream

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int totalBytes = 0;
      FileDescriptor file = GetNext();
      
      while ( file != null && count > 0 )
      {
        long startOffset = Wad.GetBlockStartOffset( file, CurrentBlock );
        long maxSize = Wad.GetBlockSize(file, CurrentBlock);
        
        if ( startOffset >= 0 )
        {
          using ( var stream = new FileStream(file.LocalFilepath, FileMode.Open))
          {
            stream.Seek(Wad.GetBlockStartOffset(file, CurrentBlock), SeekOrigin.Begin);
            int bytes = stream.Read(buffer, offset, count);
            totalBytes += bytes;

            if ( count > bytes )
              count -= bytes;
            
            Position += bytes;
          }
        }
        
        file = GetNext();
      }
      
      return totalBytes;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override bool CanRead {
      get {
        return true;
      }
    }

    public override bool CanSeek {
      get {
        return false;
      }
    }

    public override bool CanWrite {
      get {
        return false;
      }
    }

    public override long Length {
      get {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Position in the current block
    /// </summary>
    public override long Position { get; set; }

    #endregion
  }
}
