/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 17/01/2015
 * Time: 11:44 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace FuzzyHipster.Network
{
  /// <summary>
  /// Allows for traffic rate control
  /// </summary>
  public class RateLimiter
  {
    public const int UnlimitedRate = -1;
    
    public DateTime CurrentSecond { get; set; }
    public int Length { get; set; }
    public int CurrentRate { get; set; }
    
    /// <summary>
    /// Set the maximum rate per second we should receive at. UnlimitedRate for unlimited rate
    /// </summary>
    public int RateLimit { get; set; }
    
    ManualResetEvent limiter = new ManualResetEvent(true);
    Timer timer = null;
    
    public RateLimiter( int rateLimit )
    {
      timer = new Timer(GoCallback, null, 1000, 1000);
      RateLimit = rateLimit;
    }
    
    public void GotPacket( int length )
    {
      DateTime now = DateTime.Now;
      if ( CurrentSecond.Second != now.Second || CurrentSecond.Minute != now.Minute )
      {
        CurrentRate = Length;
        Length = 0;
      }
      
      CurrentSecond = now;
      Length += length;
    }
    
    public void Limit()
    {
      if ( RateLimit == UnlimitedRate )
        return;
      
      if ( Length > RateLimit )
      {
        Stop();
      }
      
      limiter.WaitOne();
    }
    
    public void Stop()
    {
      limiter.Reset();
    }
    
    void GoCallback( object state )
    {
      limiter.Set();
    }
  }
}
