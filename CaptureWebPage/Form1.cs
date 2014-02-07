using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Security;


namespace CaptureWebPage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            string mySaveLoc = txtSaveLoc.Text;

            listBox1.Items.Clear();
           
            //instantiate web browser obect
            WebBrowser wb = new WebBrowser();
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;


            //save source web page to image
            Bitmap thumbnail = SaveWebPage2Image(wb, url);
            
            pictureBox1.Image = thumbnail;

            WebClient client = new WebClient();
           // client.Credentials = new NetworkCredential(@"domain\username", "password");
            client.UseDefaultCredentials = true;
            string html = client.DownloadString(url);
            string srchString = txtRegEx.Text;

            string myUrl = "";
            string myFullPicNm = "";
           


            //Loop links we found
            foreach (LinkItem i in LinkFinder.Find(html))
            {

                try
                {
                    //Debug.WriteLine(i);
                    if (i.Href.IndexOf(srchString) > -1 && i.Href.Substring(0, 4) != "java")
                    {
                        listBox1.Items.Add(i.Href);


                        try
                        {
                            //check if absolute path or not
                            if (i.Href.Substring(0, 4) == "http")
                            {
                                myUrl = i.Href;

                            }
                            else
                            {
                                myUrl = url + "/" + i.Href;
                            }

                        }
                        catch (Exception myEx)
                        {

                        }

                        

                        thumbnail = getImg(SaveWebPage2Image(wb,myUrl));

                        myFullPicNm = mySaveLoc + getPicName(myUrl) + ".png";

                        if (System.IO.File.Exists(myFullPicNm))
                        {
                            System.IO.File.Delete(myFullPicNm);
                        }
                        thumbnail.Save(myFullPicNm, System.Drawing.Imaging.ImageFormat.Png);


                        pictureBox1.Image = thumbnail;
                    }
                }
                catch (Exception myEx)
                {

                    //MessageBox.Show("Error:" + myEx);

                }

            }
            //getLinks(html);
            //specify where image will be save
            //thumbnail.Save("C:/web-shot.png", System.Drawing.Imaging.ImageFormat.Png);
        }


        //get the file name for the web page picture
        public string getPicName(string rawNm)
        {
            string myOut = "";

            myOut = rawNm.Replace("http://", "");
            myOut = myOut.Replace("https://", "");
            myOut = myOut.Replace("/", "_");

            myOut = RemoveSpecialCharacters(myOut);

            return myOut;
            
        }


        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        } 

    /// <summary>
    /// THIS DOES NOT WORK THAT WELL
    /// </summary>
    /// <param name="message"></param>
       public void getLinks (string message)
        {
          
            
        // GET REGEX Pattern
           string anchorPattern = txtRegEx.Text;
           if (anchorPattern.Length == 0)
           {
                anchorPattern = "<a[\\s]+[^>]*?href[\\s]?=[\\s\\\"\']+(?<href>.*?)[\\\"\\']+.*?>(?<fileName>[^<]+|.*?)?<\\/a>";
           }


            MatchCollection matches = Regex.Matches(message, anchorPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            if (matches.Count > 0)
            {
               // List<Uri> uris = new List<Uri>();
                int cnt = 0;
                foreach (Match m in matches)
                {
                    string url = m.Groups[0].Value;
                    Uri testUri = null;
                    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out testUri))
                    {
                       // uris.Add(testUri);
                        listBox1.Items.Add("test:" + url);
                        //MessageBox.Show("item" + url);
                        cnt = cnt + 1;
                    }
                    listBox1.Refresh();
                }

                MessageBox.Show("Count:" + cnt);
            }
            
        } 



        //
        //
        /// <summary>
        /// this used the html parser classes
        /// </summary>
        /// <param name="url"></param>
        protected void ScanLinks(string url)
        {
            // Download page
            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            // Scan links on this page
            HtmlTag tag;
            HtmlParser parse = new HtmlParser(html);
            while (parse.ParseNext("a", out tag))
            {
                // See if this anchor links to us
               string value;
                listBox1.Items.Add(tag.Attributes.TryGetValue("href", out value));

               // if (tag.Attributes.TryGetValue("href", out value))
                //{
                    // value contains URL referenced by this link
                //}
            }
        }


        /// <summary>
        /// THIS SAVES THE WEB PAGE TO A bitmap
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Bitmap SaveWebPage2Image(WebBrowser wb, string url)
        {
            // Load the webpage into a WebBrowser control
            // WebBrowser wb = new WebBrowser();
            // wb.ScrollBarsEnabled = false;
            //wb.ScriptErrorsSuppressed = true;
            wb.Navigate(url);

            while (wb.ReadyState != WebBrowserReadyState.Complete) { System.Windows.Forms.Application.DoEvents(); }

            // Take Screenshot of the web pages full width
            wb.Width = wb.Document.Body.ScrollRectangle.Width;

            // Take Screenshot of the web pages full height
            wb.Height = wb.Document.Body.ScrollRectangle.Height;

            // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
            Bitmap bitmap = new Bitmap(wb.Width, wb.Height);
            wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));
            //wb.Dispose();

            return bitmap;
        }


        //get a resized image
        public Bitmap getImg(Bitmap myBitMap)
        {
            int maxPixels;

            if (txtImgWidth.Text.Length > 0)
            {
                maxPixels = int.Parse(txtImgWidth.Text);
            } else
            {
                //default width if none is specified
                maxPixels = 200;
            }
            
            

            int myWidth;
            int myHeight;

            

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (myBitMap.Width > maxPixels)
            {
                factor = (double)maxPixels / myBitMap.Width;


                myWidth = (int)(myBitMap.Width * factor);
                 myHeight = (int)(myBitMap.Height * factor);
            }
            else
            {


                myWidth = myBitMap.Width;
                myHeight = myBitMap.Height;


            }
            Bitmap myOut = new Bitmap(myWidth, myHeight);
            try
            {
                myOut = ResizeBitmap(myBitMap, myWidth, myHeight);
            }
            catch
            {
                MessageBox.Show("Error resizing Bitmap");
            }

            return myOut;

        } // end getImg

        //do the resizing
        public Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }


        //save profile
        private void button2_Click(object sender, EventArgs e)
        {

            WebSite myWS = new WebSite();

            myWS.ShortName = txtShortName.Text;
            myWS.Url = textBox1.Text;
            myWS.MaxWidth = txtImgWidth.Text;
            myWS.SaveFolder = txtSaveLoc.Text;
            myWS.SearchText = txtRegEx.Text;

            myWS.Write();
            MessageBox.Show("Saved!");

        }

        //load profile
        private void btnLoad_Click(object sender, EventArgs e)
        {
            WebSite myWS = new WebSite();
            myWS.ShortName = txtShortName.Text;

            myWS = myWS.Load();

           txtShortName.Text = myWS.ShortName;
           textBox1.Text = myWS.Url;
           txtImgWidth.Text = myWS.MaxWidth;
           txtSaveLoc.Text = myWS.SaveFolder;
           txtRegEx.Text = myWS.SearchText;
        

        }


    }
}
