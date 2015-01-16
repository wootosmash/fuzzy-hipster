using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    class Utils
    {
        public static string GetFolderPath()
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            var Result = Dialog.ShowDialog();

            if (Result == DialogResult.OK)
            {
                return Dialog.SelectedPath;
            }


            return null;
            
        }


        public static Channel FolderChannel()
        {
            return null;
            /*var catalog = new Catalog.Catalog();
            var torrent = new MoustacheLayer(catalog);
            catalog.BasePath = Environment.CurrentDirectory;
            catalog.Namespace = "Base Programs";
            catalog.Description = "Test Catalog for Base Programs";

            Channel faxes = new Channel()
            {
                Id = Guid.NewGuid(),
                Name = "Faxes Shit",
                Description = "All fax wangs shit",
                PublicKey = "..."
            };
            FileWad blazingSaddles = new FileWad() { ChannelId = faxes.Id, BlockSize = 1024, Name = "My Program", Description = "A hilarious Program" };
            blazingSaddles.BuildFromPath(@".");

            faxes.Wads.Add(blazingSaddles);

            catalog.Channels.Add(faxes);
            catalog.Save();*/
        }
    }
}
