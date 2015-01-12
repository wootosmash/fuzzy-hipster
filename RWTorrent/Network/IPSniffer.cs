/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 11/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;

namespace RWTorrent.Network
{
  /// <summary>
  /// Class for determining your public IP Address - useful when behind NAT
  /// </summary>
  public class IPSniffer
  {
    public IPSniffer()
    {
    }
    
    public static IPAddress GetPublicIP()
    {
      string url = "http://checkip.dyndns.org";
      System.Net.WebRequest req = System.Net.WebRequest.Create(url);
      System.Net.WebResponse resp = req.GetResponse();
      System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
      string response = sr.ReadToEnd().Trim();
      string[] a = response.Split(':');
      string a2 = a[1].Substring(1);
      string[] a3 = a2.Split('<');
      string a4 = a3[0];
      return IPAddress.Parse(a4);
    }
  }
}
