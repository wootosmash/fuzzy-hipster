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
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
namespace FuzzyHipster.Catalog
{
	public class StackCollection : List<Stack>
	{
	  
		public Stack this[Guid stackGuid] {
			get {
				return Find(x => x.Id == stackGuid);
			}
		}

		public void RefreshStack(Stack stack)
		{
			if (Contains(stack))
				Remove(stack);
			Add(stack);
			stack.Save();
		}
	}
}





