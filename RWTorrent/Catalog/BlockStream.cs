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
    
    Stream CurrentStream;
    long fragmentSize = 0;
    
    public BlockStream( FileWad wad )
    {
      Wad = wad;
      CurrentFile = null;
    }

    public void SeekBlock( int block )
    {
      CurrentBlock = block;
      long fileOffset = 0;
      currentFile = -1;
      
      do
      {
        CurrentFile = GetNext();
        fileOffset = Wad.GetFileOffset(CurrentFile, CurrentBlock);
      }
      while ( CurrentFile != null && fileOffset < 0 );
      
      if ( CurrentStream != null )
        CloseStream();
      
      Console.WriteLine("{0} {1}", CurrentFile, fileOffset);
      
      OpenStream();
      CurrentStream.Seek(fileOffset, SeekOrigin.Begin);
    }
    
    private void CloseStream()
    {
      if ( CurrentStream != null )
      {
        CurrentStream.Close();
        CurrentStream.Dispose();
        CurrentStream = null;
      }
    }
    
    private void OpenStream()
    {
      if ( CurrentFile == null )
        return; 
      
      if ( !File.Exists(CurrentFile.LocalFilepath))
        throw new Exception(string.Format("Local file for getting blocks doesn't exist. File is {0}. Expected local path is {1}",
                                          CurrentFile.CatalogFilepath,
                                          CurrentFile.LocalFilepath));
      
      
      CurrentStream = new FileStream(CurrentFile.LocalFilepath, FileMode.Open);
      fragmentSize = Wad.GetBlockFragmentSize(CurrentFile, CurrentBlock );

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
      
      while ( count > 0 && CurrentFile != null )
      {        
        int bytesToRead = count;
        if ( bytesToRead > fragmentSize )
          bytesToRead = (int)fragmentSize;
        
        //Console.WriteLine("READ {0} {1} {2}", bytesToRead, count, CurrentFile);
                
        int bytesRead = CurrentStream.Read(buffer, offset, (int)bytesToRead);
        
        //Console.WriteLine("BYTESREAD {0} {1}", bytesRead, CurrentStream.Position);
        
        offset += bytesRead;
        totalBytes += bytesRead;
        count -= bytesRead;
        Position += bytesRead;
        
        if ( bytesRead == 0 || CurrentStream.Position == CurrentStream.Length)
        {
          CloseStream();
          CurrentFile = GetNext();
          OpenStream();
        }
        
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
    
    protected override void Dispose(bool disposing)
    {
      CloseStream();
      base.Dispose(disposing);
    }
    
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
