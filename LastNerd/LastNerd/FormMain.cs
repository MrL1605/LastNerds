using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Supremes;
using Supremes.Nodes;

public class FormMain : Form
{
    private TextBox logger_tb;
    private BackgroundWorker AsyncTask;
    private ProgressBar progress_bar;
    private BackgroundWorker AsyncFileDownload;
    private Button browse_bt;
    private Label label1;
    private TextBox folder_path_tb;
    private FolderBrowserDialog folder_dialog;
    private Button run_bt;
    String folder_path = "";

    #region Windows Code
    private void InitializeComponent()
    {
            this.logger_tb = new System.Windows.Forms.TextBox();
            this.run_bt = new System.Windows.Forms.Button();
            this.AsyncTask = new System.ComponentModel.BackgroundWorker();
            this.progress_bar = new System.Windows.Forms.ProgressBar();
            this.AsyncFileDownload = new System.ComponentModel.BackgroundWorker();
            this.browse_bt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folder_path_tb = new System.Windows.Forms.TextBox();
            this.folder_dialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // logger_tb
            // 
            this.logger_tb.Location = new System.Drawing.Point(41, 54);
            this.logger_tb.Multiline = true;
            this.logger_tb.Name = "logger_tb";
            this.logger_tb.ReadOnly = true;
            this.logger_tb.Size = new System.Drawing.Size(601, 266);
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
            // AsyncTask
            // 
            this.AsyncTask.WorkerReportsProgress = true;
            this.AsyncTask.DoWork += new System.ComponentModel.DoWorkEventHandler(this.AsyncTask_DoWork);
            this.AsyncTask.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.AsyncTask_ProgressChanged);
            this.AsyncTask.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.AsyncTask_RunWorkerCompleted);
            // 
            // progress_bar
            // 
            this.progress_bar.Location = new System.Drawing.Point(41, 342);
            this.progress_bar.Name = "progress_bar";
            this.progress_bar.Size = new System.Drawing.Size(601, 23);
            this.progress_bar.TabIndex = 2;
            // 
            // AsyncFileDownload
            // 
            this.AsyncFileDownload.WorkerReportsProgress = true;
            this.AsyncFileDownload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.AsyncFileDownload_DoWork);
            this.AsyncFileDownload.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.AsyncFileDownload_ProgressChanged);
            this.AsyncFileDownload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.AsyncFileDownload_RunWorkerCompleted);
            // 
            // browse_bt
            // 
            this.browse_bt.Location = new System.Drawing.Point(504, 21);
            this.browse_bt.Name = "browse_bt";
            this.browse_bt.Size = new System.Drawing.Size(138, 21);
            this.browse_bt.TabIndex = 3;
            this.browse_bt.Text = "Browse";
            this.browse_bt.UseVisualStyleBackColor = true;
            this.browse_bt.Click += new System.EventHandler(this.browse_bt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select folder to Download Comics Images";
            // 
            // folder_path_tb
            // 
            this.folder_path_tb.Location = new System.Drawing.Point(267, 21);
            this.folder_path_tb.Name = "folder_path_tb";
            this.folder_path_tb.Size = new System.Drawing.Size(231, 20);
            this.folder_path_tb.TabIndex = 5;
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(680, 503);
            this.Controls.Add(this.folder_path_tb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browse_bt);
            this.Controls.Add(this.progress_bar);
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
        progress_bar.Hide();
    }

    public static void Main()
    {
        FormMain fm = new FormMain();
        Application.Run(fm);
    
    }

    private void run_bt_Click(object sender, EventArgs e)
    {

        /**
         * Algo -
        1. Check if Dir exists if no create and download all photos in it.  
        2. Get the first comic and check corresponding image file in folder.
        3. If exists check next one from html and corresponding image file in folder.
        4. If any file is not found download it.
        5. Continue checking and downloading until last released comic is found.
         */

        String page_addr = "/ch3p50";
        String address = "http://www.lastnerdsonearth.com/img/ch1/ch1p01_i-stole-that-line-from-myself.png";
        if (folder_path.Equals(""))
        {
            var t = new Thread((ThreadStart)(() =>
            {
                DialogResult dialog_res = folder_dialog.ShowDialog();
                if (dialog_res.Equals(DialogResult.OK))
                {
                    folder_path = folder_dialog.SelectedPath;
                }
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            folder_path_tb.Text = folder_path;
        }
        DirectoryInfo di = new DirectoryInfo(folder_path);
        if (!di.Exists)
        {
            di.Create();
        }
        //AsyncTask.RunWorkerAsync(page_addr);
        //progress_bar.Show();
        AsyncFileDownload.RunWorkerAsync(address);
        /*
        FileInfo fi = new FileInfo("F:\\Ch01P01.png");
        logger_tb.Text += Environment.NewLine + "";
        if (fi.Exists)
        {
            logger_tb.Text += "Exists";
        }
        else
        {
            logger_tb.Text += "Nope";
        }
        */

    }

    #region Async using Task
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
    #endregion

    #region AsyncTask HTML decoder
    private void AsyncTask_DoWork(object sender, DoWorkEventArgs e)
    {
        String href_val = "href", img_url = "imgUrl", address = e.Argument as String;
        HttpWebRequest req;
        HttpWebResponse res = null;
        try
        {
            AsyncTask.ReportProgress(10);

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

            AsyncTask.ReportProgress(50);

            Document doc = Dcsoup.Parse(str_data);
            Element content_ele = doc.GetElementById("content");
            // Might be last
            if (content_ele.Child(0).Attr("src").Equals("you-could-be-reading.png"))
            {
                // Yes last :P
                href_val = "None";
                img_url = "http://lastnerdsonearth.com/patreon/you-could-be-reading.png";
                res.Close();
                e.Result = href_val;

                AsyncTask.ReportProgress(100);
                return ;
            }
            else
            {
                Element a_tag = content_ele.Child(2);
                Element img_tag = a_tag.Select("img").First;
                href_val = a_tag.Attr("href");
                img_url = img_tag.Attr("src");
                String img_file_name = UrlToFile(img_url);
                if (!(new FileInfo(img_file_name)).Exists)
                {
                    AsyncFileDownload.RunWorkerAsync(img_url);
                }

            }

            AsyncTask.ReportProgress(80);

        }
        finally
        {
            if (res != null)
                res.Close();
        }
        address = href_val;
        e.Result = href_val;
        AsyncTask.ReportProgress(100);

    }

    private void AsyncTask_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        
        logger_tb.Text += e.ProgressPercentage + "%  , ";
        progress_bar.Value = e.ProgressPercentage;

    }

    private void AsyncTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        logger_tb.Text += "Complete" + Environment.NewLine;
        
        String href_nxt = e.Result as String;
        if (!href_nxt.Equals("None"))
        {
            AsyncTask.RunWorkerAsync(href_nxt);
        }
    }
    #endregion

    #region Async File Download
    private void AsyncFileDownload_DoWork(object sender, DoWorkEventArgs e)
    {
        String address = e.Argument as String;
        HttpWebRequest req;
        HttpWebResponse res = null;
        try
        {
            AsyncFileDownload.ReportProgress(10);

            req = (HttpWebRequest)WebRequest.Create(address);
            res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();

            var file_path = folder_path + "\\" + UrlToFile(address);
            using (var fs = File.Create(file_path))
            {
                AsyncFileDownload.ReportProgress(50);
                stream.CopyTo(fs);
            }
            stream.Close();
            AsyncFileDownload.ReportProgress(80);

        }
        finally
        {
            if (res != null)
                res.Close();
            
        }
        e.Result = "Done";
        AsyncFileDownload.ReportProgress(100);

    }

    private void AsyncFileDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        logger_tb.Text += e.ProgressPercentage + "%, ";

    }

    private void AsyncFileDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        logger_tb.Text += "Completed " + Environment.NewLine;
    }

    #endregion

    #region Browse Button
    private void browse_bt_Click(object sender, EventArgs ea)
    {
        var t = new Thread((ThreadStart)(() =>
        {
            DialogResult dialog_res = folder_dialog.ShowDialog();
            if (dialog_res.Equals(DialogResult.OK))
            {
                folder_path = folder_dialog.SelectedPath;
            }
        }));

        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();
        folder_path_tb.Text = folder_path;

    }
    #endregion

    #region Url to File Namer
    public String UrlToFile(String Url)
    {

        String[] splited = Url.Split('/');
        return splited[splited.Length - 1];
    }
    #endregion
}

