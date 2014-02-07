using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CaptureWebPage
{
   public class WebSite
    {
        public string Url;
        public string SearchText;
        public string SaveFolder;
        public string MaxWidth;
        public string ShortName;


        public void Write()
        {
            // Create and XmlSerializer to serialize the data to a file 
            XmlSerializer xs = new XmlSerializer(typeof(WebSite));
            using (FileStream fs = new FileStream(this.ShortName + "_Data.xml", FileMode.Create))
            {
                xs.Serialize(fs, this);
            }
        } // end write

       // load existing profile
        public WebSite Load()
        {
            XmlSerializer xs = new XmlSerializer(typeof(WebSite));
            WebSite w = new WebSite();
            try
            {


                string myDataFile = "";

                if (this.ShortName.Length > 0)
                {
                    myDataFile = this.ShortName + "_Data.xml";

                }
                else
                {
                    myDataFile = "Data.xml";
                }
                 using (FileStream fs = new FileStream(myDataFile, FileMode.Open))
                 {
                     w = xs.Deserialize(fs) as WebSite;
                 }

                
            } catch (Exception e) {



            }

            if (w != null)
            {

            }
            else
            {
                w = new WebSite();
            }
            return w;
        }

    } //end class
}
