/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace FuzzyHipster.Catalog
{
  public class Block : IEquatable<Block>
	{
		public Guid FileWadId {
			get;
			set;
		}

		public int Sequence {
			get;
			set;
		}

		public string Hash {
			get;
			set;
		}

		public long Length {
			get;
			set;
		}

		public byte[] Data {
			get;
			set;
		}

		public void Save()
		{
			FileWad wad = MoustacheLayer.Singleton.Catalog.GetFileWad(FileWadId);
			FileDescriptor[] descriptors = wad.Files.GetDescriptorsByBlock(this);
			foreach (var descriptor in descriptors) 
			{
				if (!descriptor.IsAllocated)
					descriptor.AllocateFile();
				
				using (var writer = new BinaryWriter(new FileStream(descriptor.LocalFilepath, FileMode.OpenOrCreate))) 
				{
					long startOffset = ((long)wad.BlockSize * (long)descriptor.StartBlock) + descriptor.StartOffset;
					writer.Seek((int)startOffset, SeekOrigin.Begin);
					writer.Write(Data, 0, GetLengthOfBytesToWrite(descriptor));
				}
			}
		}

		int GetLengthOfBytesToWrite(FileDescriptor descriptor)
		{
			int lengthOfBytesToWrite = (int)Length;
			if (Sequence == descriptor.StartBlock && Sequence == descriptor.EndBlock)
				lengthOfBytesToWrite = (int)descriptor.StartOffset - (int)descriptor.EndFragmentSize;
			else {
				if (Sequence == descriptor.StartBlock)
				  lengthOfBytesToWrite -= (int)descriptor.StartOffset;
				if (Sequence == descriptor.EndBlock)
				  lengthOfBytesToWrite -= (int)descriptor.EndFragmentSize;
			}
			return lengthOfBytesToWrite;
		}

		public static byte[] GetBytes(Block block)
		{
		  return null;
		}
		
    #region IEquatable implementation
    public bool Equals(Block other)
    {
      return other.FileWadId == FileWadId && other.Sequence == Sequence;
    }
    #endregion
		
	}
}



