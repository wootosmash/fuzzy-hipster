/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using FuzzyHipster.Catalog;
namespace FuzzyHipster.Network
{
	[Serializable()]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class StacksNetMessage : NetMessage
	{
		public Stack[] Stacks {
			get;
			set;
		}

		public StacksNetMessage()
		{
			Type = MessageType.Stacks;
			Stacks = new Stack[0];
		}

		public override string ToString()
		{
			if (Stacks.Length == 0)
				return "[StacksNetMessage Stacks.Count=0]";
			else
				return string.Format("[StacksNetMessage Stacks={0}]", Stacks);
		}
	}
}


