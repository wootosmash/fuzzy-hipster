﻿/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 18/01/2015
 * Time: 7:33 PM
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
  /// <summary>
  /// Description of KeyCollection.
  /// </summary>
  public class KeyCollection : List<Key>
  {
    public static KeyCollection Load( string basePath )
    {
      var col = new KeyCollection();
      string keysPath = Path.Combine(basePath, @"Catalog\");

      if ( !Directory.Exists(keysPath))
        Directory.CreateDirectory(keysPath);

      if ( File.Exists(keysPath + "Keys.xml"))
      {
        var serialiser = new XmlSerializer(typeof(List<Key>));
        using (var reader = new StreamReader(keysPath + "Keys.xml"))
        {
          var list = serialiser.Deserialize(reader) as List<Key>;
          foreach( var key in list )
            if ( !col.Contains(key))
              col.Add(key);
        }
      }

      return col;
    }
    
    
    public void Save()
    {
      lock( this)
      {
        string basePath = MoustacheLayer.Singleton.Catalog.BasePath;
        string keysPath = Path.Combine(basePath, @"Catalog\");
        
        if ( !Directory.Exists(keysPath))
          Directory.CreateDirectory(keysPath);
        
        var list = new List<Key>();
        
        foreach( var key in this )
          list.Add(key);
        
        var serialiser = new XmlSerializer(typeof(List<Key>));
        using (var writer = new StreamWriter(keysPath + "Keys.xml"))
          serialiser.Serialize(writer, list);
      }
    }
  }
}
