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

    /// <summary>
    /// Reads in one block
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
      int totalBytes = 0;
      FileDescriptor file = GetNext();
      
      while ( file != null && count > 0 )
      {
        long startOffset = Wad.GetFileOffset( file, CurrentBlock );
        long fragmentSize = Wad.GetBlockFragmentSize(file, CurrentBlock );
        
        if ( startOffset >= 0 )
        {
          if ( !File.Exists(file.LocalFilepath))
            throw new Exception(string.Format("Local file for getting blocks doesn't exist. File is {0}. Expected local path is {1}", 
                                              file.CatalogFilepath, 
                                              file.LocalFilepath));
          
          using ( var stream = new FileStream(file.LocalFilepath, FileMode.Open))
          {            
            stream.Seek(startOffset, SeekOrigin.Begin);
            int bytes = stream.Read(buffer, offset, (int)fragmentSize);
            totalBytes += bytes;
            offset += bytes;

            //if ( count > bytes )
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
        
    /// <summary>
    /// Gets a stream for the block/WAD. Figures out where the look in the system
    /// </summary>
    /// <param name="wad"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public static Stream Create( FileWad wad, int block )
    {
      string blockPath = MoustacheLayer.Singleton.Catalog.BasePath + @"Catalog\Blocks\" + wad.Id + @"\";
      if ( Directory.Exists(blockPath))
      {
        string [] files = Directory.GetFiles(blockPath, block + "-*.blk");
        if ( files.Length > 0 )
          return new FileStream( files[0], FileMode.Open );
      }
      
      var stream = new BlockStream(wad);
      stream.SeekBlock(block);
      return stream;
    }
  }
}
