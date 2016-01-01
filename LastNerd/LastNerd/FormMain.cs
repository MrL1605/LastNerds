using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Drawing;
using Supremes;
using Supremes.Nodes;
using Supremes.Parsers;
using System.Windows.Forms;

public class FormMain : Form
{
    private TextBox logger_tb;
    private Button run_bt;
    #region Windows Code
    private void InitializeComponent()
    {
            this.logger_tb = new System.Windows.Forms.TextBox();
            this.run_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logger_tb
            // 
            this.logger_tb.Location = new System.Drawing.Point(41, 28);
            this.logger_tb.Multiline = true;
            this.logger_tb.Name = "logger_tb";
            this.logger_tb.ReadOnly = true;
            this.logger_tb.Size = new System.Drawing.Size(601, 292);
            this.logger_tb.TabIndex = 0;
            this.logger_tb.Text = "Logs :";
            // 
            // run_bt
            // 
            this.run_bt.Location = new System.Drawing.Point(267, 380);
            this.run_bt.Name = "run_bt";
            this.run_bt.Size = new System.Drawing.Size(149, 51);
            this.run_bt.TabIndex = 1;
            this.run_bt.Text = "Run";
            this.run_bt.UseVisualStyleBackColor = true;
            this.run_bt.Click += new System.EventHandler(this.run_bt_Click);
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(680, 503);
            this.Controls.Add(this.run_bt);
            this.Controls.Add(this.logger_tb);
            this.Name = "FormMain";
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    #endregion

    public FormMain()
    {
        InitializeComponent();
    }

    public static void Main()
    {
        FormMain fm = new FormMain();
        Application.Run(fm);

    }

    private void run_bt_Click(object sender, EventArgs e)
    {
        String page_addr = "/ch3p50";
        String address = "http://www.lastnerdsonearth.com/img/ch1/ch1p01_i-stole-that-line-from-myself.png";
        get_resp(page_addr);

    }

    public void get_resp(string address)
    {
            var data = Task.Run(() =>
            {
                string href_val = "href", img_url = "imgUrl";
                HttpWebRequest req;
                HttpWebResponse res = null;
                try
                {

                    req = (HttpWebRequest)WebRequest.Create("http://www.lastnerdsonearth.com" + address);
                    res = (HttpWebResponse)req.GetResponse();
                    Stream stream = res.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    String str_data = sr.ReadToEnd();
                    /*
                    var file_path = "F:\\MaImage.png";
                    using (var fs = File.Create(file_path))
                    {
                        stream.CopyTo(fs);
                    }
                    */

                    Document doc = Dcsoup.Parse(str_data);
                    Element content_ele = doc.GetElementById("content");
                    // Might be last
                    if (content_ele.Child(0).Attr("src").Equals("you-could-be-reading.png"))
                    {
                        // Yes last :P
                        href_val = "None";
                        img_url = "http://lastnerdsonearth.com/patreon/you-could-be-reading.png";
                        res.Close();
                        return href_val;
                    }
                    else
                    {
                        Element a_tag = content_ele.Child(2);
                        Element img_tag = a_tag.Select("img").First;
                        href_val = a_tag.Attr("href");
                        img_url = img_tag.Attr("src");
                    }

                }
                finally
                {
                    if (res != null)
                        res.Close();
                }
                Console.WriteLine(href_val);
                address = href_val;
                return href_val;
            });

        String nxt_href = data.Result;
        logger_tb.Text += Environment.NewLine + "Donwload complete -  " + nxt_href;
        if (!nxt_href.Equals("None")){
            get_resp(nxt_href);
        }
        else
        {
            logger_tb.Text += Environment.NewLine + "X Complete -  ";
        }

    }


}

