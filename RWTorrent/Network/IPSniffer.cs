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

namespace FuzzyHipster.Network
{
  /// <summary>
  /// Class for determining your public IP Address - useful when behind NAT
  /// </summary>
  public class IPSniffer
  {
    public static int MaxTries = 4;
    public const string DynDNSAddress =  "http://checkip.dyndns.org/";
    public const string ICanHazIPAddress = "http://icanhazip.com/";
    public const string CurlMyIPAddress = "http://curlmyip.com/";
    public const string IPEchoAddress = "http://ipecho.net/plain";
    
    public IPSniffer()
    {
    }
    
    public static IPAddress GetPublicIP()
    {
      
      IPAddress ip = null;
      int tries = 0;
      
      while (tries < MaxTries && ip == null )
      {
        try {
          if ( tries % 4 == 0 )
            ip = GetPublicIPFromDynDNS();
          else if ( tries % 4 == 1 )
            ip = GetPublicIPFromICanHazIP();
          else if ( tries % 4 == 2 )
            ip = GetPublicIPFromCurlMyIP();
          else 
            ip = GetPublicIPFromIPEchoAddress();

        }
        catch( Exception ex )
        {
          
        }
        
        tries++;
      }
      
      return ip;
    }
    
    public static IPAddress GetPublicIPFromDynDNS()
    {
      System.Net.WebRequest req = System.Net.WebRequest.Create(DynDNSAddress);
      System.Net.WebResponse resp = req.GetResponse();
      System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
      string response = sr.ReadToEnd().Trim();
      string[] a = response.Split(':');
      string a2 = a[1].Substring(1);
      string[] a3 = a2.Split('<');
      string a4 = a3[0];
      return IPAddress.Parse(a4);
    }
    
    public static IPAddress GetPublicIPFromICanHazIP()
    {
      return GetPublicIPFromPlainTextSource(ICanHazIPAddress);
    }
    
    public static IPAddress GetPublicIPFromCurlMyIP()
    {
      return GetPublicIPFromPlainTextSource(CurlMyIPAddress);
    }
    
    public static IPAddress GetPublicIPFromIPEchoAddress()
    {
      return GetPublicIPFromPlainTextSource(IPEchoAddress);
    }
    
    private static IPAddress GetPublicIPFromPlainTextSource( string url )
    {
      System.Net.WebRequest req = System.Net.WebRequest.Create(url);
      System.Net.WebResponse resp = req.GetResponse();
      System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
      string response = sr.ReadToEnd().Trim();
      return IPAddress.Parse(response);
    }
  }
}
