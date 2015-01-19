/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FuzzyHipster.Crypto;
namespace FuzzyHipster.Catalog
{
  [Serializable()]
  public class FileWad : CatalogItem, IEquatable<FileWad>
  {
    public Guid ChannelId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long BlockSize { get; set; }

    public int TotalBlocks { get; set; }
    
    public long TotalSize { get; set; }
    
    
    public FileDescriptorCollection Files { get; set; }

    public BlockIndexItemCollection BlockIndex { get; set; }
    
    [NonSerialized()]
    bool isFullyDownloaded;
    public bool IsFullyDownloaded {
      get {
        return isFullyDownloaded;
      }
      set {
        isFullyDownloaded = value;
      }
    }

    public FileWad()
    {
      Id = Guid.NewGuid();
      Files = new FileDescriptorCollection();
      BlockIndex = new BlockIndexItemCollection();
      LastUpdated = DateTime.Now.ToFileTimeUtc();
      IsFullyDownloaded = true;
    }

    public void CatalogBlock( int block, string tempFile )
    {
      string path = string.Format(@"{0}\Catalog\{1}\{2}\", MoustacheLayer.Singleton.Catalog.BasePath, this.ChannelId, Id);
      if ( Directory.Exists( path ))
        Directory.CreateDirectory(path);

      CheckIfFullyDownloaded();
    }
    
    public void CheckIfFullyDownloaded()
    {
      bool downloaded = true;
      for( int i=0;i<BlockIndex.Count;i++)
      {
        if ( !BlockIndex[i].Downloaded )
        {
          Console.WriteLine("Block {0} is not downloaded", i);
          downloaded = false;
          break;
        }
      }
      IsFullyDownloaded = downloaded;
    }
    
    /// <summary>
    /// For this file, how much of this block does it take up?
    /// </summary>
    /// <param name="file"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public long GetBlockFragmentSize( FileDescriptor file, int block )
    {
      if ( file.StartBlock > block )
        return 0;
      if ( file.EndBlock < block )
        return 0;
      
      if ( file.StartBlock == file.EndBlock && file.StartBlock == block )
        return file.Length;
      if ( file.StartBlock == block )
        return BlockSize - file.StartOffset;
      if ( file.EndBlock == block )
        return file.EndFragmentSize;
      if ( file.StartBlock < block && file.EndBlock > block )
        return BlockSize;
      return 0;
    }
    
    /// <summary>
    /// Determines the offset to start writing a file into this block
    /// </summary>
    /// <param name="file"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public long GetBlockOffset( FileDescriptor file, int block )
    {
      if ( file.EndBlock < block )
        return -1;
      if ( file.StartBlock > block )
        return -1;
      
      if ( file.StartBlock == block )
        return file.StartOffset;
      
      return 0;
    }
    
    /// <summary>
    /// Determines the offset within a file to write/read this block
    /// </summary>
    /// <param name="file"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public long GetFileOffset( FileDescriptor file, int block )
    {
      if ( file.StartBlock > block )
        return -1;
      if ( file.EndBlock < block )
        return -1;
      
      if ( file.StartBlock == block )
        return 0;
      return (BlockSize - file.StartOffset) + ((block - file.StartBlock -1) * BlockSize);
    }

    public string GetBlocksPath()
    {
      string path = Path.Combine(MoustacheLayer.Singleton.Catalog.BasePath, @"Catalog\Blocks\" + Id + @"\");
      return path;
    }
    
    public Stream GetBlockStream( int block )
    {
      string blocksPath = GetBlocksPath();
      string [] blocks = Directory.GetFiles(blocksPath);
      
      foreach( string file in blocks )
      {
        
        if ( Path.GetFileName(file).StartsWith(block + "-"))
          return new FileStream(file, FileMode.Open, FileAccess.Read);
      }
      
      throw new Exception(string.Format("Cant find the file for block {0} in path {1}", block, blocksPath));
    }
    
    public bool[] GetBlockAvailability()
    {
      bool[] availability = new bool[BlockIndex.Count];
      
      for( int i=0;i<BlockIndex.Count;i++)
        availability[i] = BlockIndex[i].Downloaded;
      
      return availability;
    }
    
    public int[] GetAvailabilityNeededIntersection( bool[] availability )
    {
      var list = new List<int>();
      
      for ( int i=0;i<availability.Length;i++)
        if ( availability[i] )
          if ( !BlockIndex[i].Downloaded )
            list.Add(i);
      
      return list.ToArray();
    }
    
    public void SaveFromBlocks( string basePath )
    {
      if ( Files == null )
        throw new Exception("FileWad.Files is not set");
      if ( Files.Count == 0 )
        throw new Exception("FileDescriptors list is empty");
      
      foreach( var file in Files )
        SaveFromBlocks(file, basePath);
    }

    /// <summary>
    /// Save a file from blocks
    /// </summary>
    /// <param name="file"></param>
    /// <param name="basePath"></param>
    public void SaveFromBlocks( FileDescriptor file, string basePath )
    {
      if ( File.Exists(file.LocalFilepath))
        return;
      
      const int bufferLength = 1024;
      var buffer = new byte[bufferLength];
      string blocksPath = GetBlocksPath();
      
      if ( !Directory.Exists(basePath))
        Directory.CreateDirectory(basePath);
      
      file.LocalFilepath = Path.Combine(basePath, file.CatalogFilepath);
      
      if ( !Directory.Exists(Path.GetDirectoryName(file.LocalFilepath)))
        Directory.CreateDirectory(Path.GetDirectoryName(file.LocalFilepath));
      
      using (var writer = new FileStream(file.LocalFilepath, FileMode.CreateNew))
      {
        for ( int block = file.StartBlock; block <= file.EndBlock; block++ )
        {
          using (var reader = GetBlockStream(block))
          {
            long start = GetBlockOffset(file, block);
            long totalLength = GetBlockFragmentSize(file, block);
            
            reader.Seek(start, SeekOrigin.Begin);
            
            while ( totalLength > 0 )
            {
              long count = bufferLength;
              if ( count > totalLength )
                count = totalLength;
              count = reader.Read(buffer, 0, (int)count);
              writer.Write(buffer, 0,  (int)count);
              totalLength -= count;
            }
          }
        }
      }
      
      byte[] hash = Hash.GetHash(file.LocalFilepath);
      if ( !Hash.Compare(file.Hash, hash))
      {
        //File.Delete(filePath);
        throw new Exception(string.Format("Convert file {0} from blocks failed, hash is bad.", file.CatalogFilepath));
      }
    }
    
    public void VerifyBlock( int block, string blockFilePath )
    {
      var info = new FileInfo(blockFilePath);

      BlockIndexItem item = BlockIndex[block];
      
      if ( item.Hash == null )
        throw new Exception(string.Format("BlockIndex item {0} has no hash", block));
      
      // block length doesn't match expected length
      if (info.Length != item.Length)
        throw new Exception(string.Format("Temp file length {0} and BlockIndex lengths {1} do not match", info.Length, item.Length));
      // hashes dont match
      if ( !Hash.Compare(Hash.GetHash(blockFilePath), item.Hash))
        throw new Exception(string.Format("Temp file hash and BlockIndex hashes do not match"));
      // length of data buffer doesn't match block size
      if (info.Length != BlockSize && block != BlockIndex.Count-1)
        throw new Exception(string.Format("Block size {0} does not match the expected block size from our index {1} bytes", info.Length, BlockSize));
    }

    /// <summary>
    /// Build a FileWad from either a File Path or Directory Path
    /// </summary>
    /// <param name="path"></param>
    public void BuildFromPath(string path)
    {
      bool isFile = File.Exists(path);
      long totalLength = 0;
      
      if ( isFile )
        totalLength = AddFile(Path.GetDirectoryName(path), path);
      else
      {
        if (!Directory.Exists(path))
          throw new Exception(string.Format("Path {0} doesn't exist", path));
        
        totalLength = RecursiveBuildFiles(path, path);
      }
      
      if ( BlockSize <= 0 )
        BlockSize = EstimateBlockSize(MoustacheLayer.Singleton.Settings.DefaultBlockQuantity, CalculatePathSize(path));
      
      TotalBlocks = (int)Math.Ceiling((double)totalLength / (double)BlockSize);
      
      int lastBlock = 0;
      long lastOffset = 0;
      bool firstLoop = true;
      
      // build file descs
      foreach (FileDescriptor file in Files)
      {
        if (lastOffset == 0 && !firstLoop )
          lastBlock++;
        
        firstLoop = false;
        
        file.StartBlock = lastBlock;
        
        lastBlock += (int)Math.Floor((double)(file.Length + lastOffset) / (double)BlockSize);

        file.EndBlock = lastBlock;
        file.StartOffset = lastOffset;
        
        
        if (file.StartBlock == file.EndBlock)
        {
          file.EndFragmentSize = file.Length;
          lastOffset += file.EndFragmentSize;
        }
        else
        {
          file.EndFragmentSize = (long)((file.Length - (BlockSize - file.StartOffset)) % BlockSize);
          lastOffset = file.EndFragmentSize;
        }
        
        file.Hash = Hash.GetHash(file.LocalFilepath );
      }
      
      // build block index
      for (int i = 0; i < TotalBlocks; i++)
      {
        var block = new BlockIndexItem();
        block.Length = BlockSize;
        block.Downloaded = true;
        BlockIndex.Add(block);
      }
      
      if (totalLength % BlockSize > 0)
        BlockIndex[BlockIndex.Count - 1].Length = totalLength % BlockSize;
      
      // calculate the hashes
      for(int i=0;i<BlockIndex.Count;i++)
      {
        using ( var stream = new BlockStream( this ))
        {
          stream.SeekBlock(i);
          BlockIndex[i].Hash = Hash.GetHash(stream, BlockIndex[i].Length);
        }
      }
      
      Validate();
    }
    
    public override void Validate()
    {
      CheckIfFullyDownloaded();
      
      foreach( var file in Files )
      {
        long len = (BlockSize - file.StartOffset) + ((file.EndBlock - file.StartBlock - 1) * BlockSize) + file.EndFragmentSize;
        if ( file.StartBlock == file.EndBlock )
        {
          if ( file.StartOffset + file.EndFragmentSize > BlockSize )
            throw new Exception("Offsets and fragment sizes are too big for the block: " + file);
          len = file.EndFragmentSize;
        }
        
        if ( file.Length != len )
          throw new Exception(string.Format("File descriptor length doesn't validate calculated={0}, expected={1}, descriptor={2}",
                                            file.Length, len, file));
      }
      
    }

    public void Save()
    {
      lock (this) {
        string basePath = MoustacheLayer.Singleton.Catalog.BasePath;
        string channelPath = Path.Combine(basePath, string.Format(@"Catalog\Channels\{0}\", ChannelId));
        
        if (!Directory.Exists(channelPath))
          Directory.CreateDirectory(channelPath);
        
        var serialiserChannels = new XmlSerializer(typeof(FileWad));
        using (var writer = new StreamWriter(string.Format("{0}{1}.xml", channelPath, Id)))
          serialiserChannels.Serialize(writer, this);
      }
    }
    
    public static FileWad Load( string filePath )
    {
      if ( !File.Exists(filePath))
        return null;
      
      var serialiser = new XmlSerializer(typeof(FileWad));
      using (var reader = new StreamReader(filePath))
        return serialiser.Deserialize(reader) as FileWad;
    }
    
    private long AddFile(string basePath, string file)
    {
      var info = new FileInfo(file);
      var descriptor = new FileDescriptor()
      {
        Hash = Hash.GetHash(file),
        CatalogFilepath = file.Substring(basePath.Length),
        LocalFilepath = file,
        IsAllocated = true,
        Length = info.Length
      };
      if ( descriptor.CatalogFilepath.StartsWith(@"\"))
        descriptor.CatalogFilepath = descriptor.CatalogFilepath.Substring(1);
      
      Files.Add(descriptor);
      return info.Length;
    }
    
    private long RecursiveBuildFiles(string basePath, string path)
    {
      string[] files = Directory.GetFiles(path);
      long totalLength = 0;
      
      foreach (string file in files)
      {
        totalLength += AddFile(basePath, file);
      }
      
      foreach (string dir in Directory.GetDirectories(path))
      {
        totalLength += RecursiveBuildFiles(basePath, dir);
      }
      return totalLength;
    }

    #region IEquatable implementation
    public bool Equals(FileWad other)
    {
      return other.Id == Id;
    }
    #endregion
    

    public override string ToString()
    {
      return string.Format("[FileWad Id={0}, ChannelId={1}, Name={2}, Description={3}, BlockSize={4}, TotalBlocks={5}, TotalSize={6}, LastUpdate={7}, Files={8}, BlockIndex={9}]", Id, ChannelId, Name, Description, BlockSize, TotalBlocks, TotalSize, LastUpdated, Files, BlockIndex);
    }

    
    public static long CalculatePathSize( string path )
    {
      long size = 0;
      
      if ( File.Exists(path))
        return new FileInfo(path).Length;
      
      if ( !Directory.Exists(path))
        return 0;
      
      foreach( string file in Directory.GetFiles(path))
      {
        var info = new FileInfo(file);
        size += info.Length;
      }
      
      foreach( string dir in Directory.GetDirectories(path))
      {
        size += CalculatePathSize(dir);
      }
      
      return size;
    }
    
    public static long EstimateBlockSize( int quantityDesired, long totalBytes )
    {
      long blockSize = totalBytes / quantityDesired;
      int p = (int)Math.Pow(2, Math.Ceiling(Math.Log(blockSize)/Math.Log(2)));
      
      if ( p < MoustacheLayer.Singleton.Settings.MinBlockSize )
        p = MoustacheLayer.Singleton.Settings.MinBlockSize;
      
      return p;
    }
  }
}



