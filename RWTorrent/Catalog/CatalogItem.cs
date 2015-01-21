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
using System.Runtime.Serialization.Formatters.Binary;
using FuzzyHipster.Crypto;
namespace FuzzyHipster.Catalog
{
  [Serializable()]
  public abstract class CatalogItem
  {
    public Guid Id { get; set; }
    public long LastUpdated { get; set; }
    public byte[] Signature { get; set; }
    public AsymmetricKey PublicKey { get; set; }

    /// <summary>
    /// A date/time to not advertise this item until. It can still be requested and blocks etc can go through
    /// </summary>
    public DateTime AdvertisementMoratorium
    {
      get { return advertisementMoratorium; }
      set { advertisementMoratorium = value; }
    }
    
    [NonSerialized()]
    DateTime advertisementMoratorium;
    
    public abstract void Validate();
    
    public void UpdateLastUpdated( long lastUpdated )
    {
      if ( lastUpdated > LastUpdated )
        LastUpdated = lastUpdated;
    }
    
    public void Sign( AsymmetricKey key)
    {
      // this could all go horribly wrong if its not syncro'd
      lock(this)
      {
        PublicKey = null;
        Signature = null;
        
        Signature = key.SignData(this);
        PublicKey = key.GetPublicKey();
      }
    }
    
    public bool VerifySignature()
    {
      // this could all go horribly wrong if its not syncro'd
      lock (this)
      {
        AsymmetricKey publicKey = PublicKey;
        byte[] signature = Signature;
        
        PublicKey = null;
        Signature = null;
        
        bool result = publicKey.VerifySignature(signature, this);
        
        PublicKey = publicKey;
        Signature = signature;
        
        return result;
      }
    }
  }
}





