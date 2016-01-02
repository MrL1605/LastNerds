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
    private BackgroundWorker AsyncTask;
    private ProgressBar progress_bar;
    private Button browse_bt;
    private Label label1;
    private TextBox folder_path_tb;
    private FolderBrowserDialog folder_dialog;
    private Button run_bt;
    private TextBox logger_tb;
    private Button exit_bt;
    public String folder_path = "";
    

    #region Windows Code
    private void InitializeComponent()
    {
            this.run_bt = new System.Windows.Forms.Button();
            this.AsyncTask = new System.ComponentModel.BackgroundWorker();
            this.progress_bar = new System.Windows.Forms.ProgressBar();
            this.browse_bt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folder_path_tb = new System.Windows.Forms.TextBox();
            this.folder_dialog = new System.Windows.Forms.FolderBrowserDialog();
            this.logger_tb = new System.Windows.Forms.TextBox();
            this.exit_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // run_bt
            // 
            this.run_bt.Location = new System.Drawing.Point(178, 371);
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
            // logger_tb
            // 
            this.logger_tb.Location = new System.Drawing.Point(41, 50);
            this.logger_tb.Multiline = true;
            this.logger_tb.Name = "logger_tb";
            this.logger_tb.ReadOnly = true;
            this.logger_tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logger_tb.Size = new System.Drawing.Size(601, 286);
            this.logger_tb.TabIndex = 6;
            this.logger_tb.Text = "Logs :";
            // 
            // exit_bt
            // 
            this.exit_bt.Location = new System.Drawing.Point(349, 371);
            this.exit_bt.Name = "exit_bt";
            this.exit_bt.Size = new System.Drawing.Size(149, 51);
            this.exit_bt.TabIndex = 7;
            this.exit_bt.Text = "Exit";
            this.exit_bt.UseVisualStyleBackColor = true;
            this.exit_bt.Click += new System.EventHandler(this.exit_bt_Click);
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(680, 474);
            this.Controls.Add(this.exit_bt);
            this.Controls.Add(this.logger_tb);
            this.Controls.Add(this.folder_path_tb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browse_bt);
            this.Controls.Add(this.progress_bar);
            this.Controls.Add(this.run_bt);
            this.Name = "FormMain";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    public static void Main()
    {
        FormMain fm = new FormMain();
        Application.Run(fm);
    }
    #endregion

    public FormMain()
    {
        InitializeComponent();
        progress_bar.Hide();
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

        run_bt.Enabled = false;

        logger_tb.Text += Environment.NewLine;

        if (folder_path.Equals(""))
        {
            show_browse_bt();
        }
        if (folder_path.Equals(""))
            return;
        DirectoryInfo di = new DirectoryInfo(folder_path);
        if (!di.Exists)
        {
            di.Create();
        }

        String first_comic = "/ch1p01";
        // run html parser on first page
        // it will download image file if it does not exists
        // it will also execute next html parser and so on.
        progress_bar.Show();
        AsyncTask.RunWorkerAsync(first_comic);

    }

    String curr_comic_page = "";

    #region AsyncTask HTML decoder with Image Download
    private void AsyncTask_DoWork(object sender, DoWorkEventArgs e)
    {
        String img_download_url, href_val;
        // Partial url of HTML page of comic
        curr_comic_page = e.Argument as String;
        HttpWebRequest req;
        HttpWebResponse res = null;
        try
        {
            #region HTML Download and Parser 

            AsyncTask.ReportProgress(10);
            req = (HttpWebRequest)WebRequest.Create("http://www.lastnerdsonearth.com" + curr_comic_page);
            try
            {
                res = (HttpWebResponse)req.GetResponse();
            }
            catch
            {
                Console.WriteLine("Ended badly");
                e.Result = "Down";
                return;
            }
            Stream stream = res.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            String str_data = sr.ReadToEnd();

            AsyncTask.ReportProgress(30);
            
            // Parse HTML data to extract image
            Document doc = Dcsoup.Parse(str_data);
            Element content_ele = doc.GetElementById("content");
            // Check if last Image
            if (content_ele.Child(0).Attr("src").Equals("you-could-be-reading.png"))
            {
                // Yes then substitute None ot curr url to stop proceeding
                href_val = "None";
                img_download_url = "http://lastnerdsonearth.com/patreon/you-could-be-reading.png";
                res.Close();
                e.Result = href_val;
                AsyncTask.ReportProgress(100);
                return ;
            }
            else
            {
                // If no extract next page url and current image url.
                Element a_tag = content_ele.Child(2);
                Element img_tag = a_tag.Select("img").First;
                href_val = a_tag.Attr("href");
                img_download_url = img_tag.Attr("src");
            }

            #endregion

            AsyncTask.ReportProgress(60);

            // Check if comic file exists
            // and download if not
            String img_file_name = UrlToFile(img_download_url);
            Console.WriteLine("File Name  " + img_file_name);
            if (!(new FileInfo(folder_path + "\\" + img_file_name)).Exists)
            {

                #region Download Image File
                req = (HttpWebRequest)WebRequest.Create(img_download_url);
                try
                {
                    res = (HttpWebResponse)req.GetResponse();
                } catch
                {
                    e.Result = "Down";
                    return;
                }
                stream = res.GetResponseStream();
                #endregion

                AsyncTask.ReportProgress(80);

                #region Save downloaded image to file in specified Location
                var file_path = folder_path + "\\" + UrlToFile(img_download_url);
                using (var fs = File.Create(file_path))
                {
                    AsyncTask.ReportProgress(50);
                    stream.CopyTo(fs);
                }
                stream.Close();
                #endregion

            }
            else
            {
                // File Exists
                AsyncTask.ReportProgress(62);
            }
            AsyncTask.ReportProgress(90);

        }
        catch
        {
            Console.WriteLine("Ended badly");
            e.Result = "Down";
            return;
        }
        finally
        {
            if (res != null)
                res.Close();
        }
        e.Result = href_val;
        AsyncTask.ReportProgress(100);

    }

    private void AsyncTask_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if(e.ProgressPercentage == 62)
        {
            logger_tb.Text += "File Exsits Skipping Download.";
        }
        logger_tb.Text += e.ProgressPercentage + "%, ";
        progress_bar.Value = e.ProgressPercentage;

    }

    private void AsyncTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {

        String href_nxt = e.Result as String;
        if (href_nxt.Equals("Down"))
        {
            logger_tb.Text += "Website Down or Network Connection Problem" + Environment.NewLine;
        }
        else {

            if (!href_nxt.Equals("None"))
            {
                // Call next HTML parser
                logger_tb.Text += " - Complete - " + curr_comic_page + Environment.NewLine;
                AsyncTask.RunWorkerAsync(href_nxt);
            }
            else
            {
                run_bt.Enabled = true;
                logger_tb.Text += Environment.NewLine +  Environment.NewLine + "Latest Released Comic " + curr_comic_page + Environment.NewLine + "Download Fully Completed.";
            }
        }
    }
    #endregion
    
    #region Browse Button
    private void browse_bt_Click(object sender, EventArgs ea)
    {
        show_browse_bt();
    }

    public void show_browse_bt()
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
        String server_file_name = splited[splited.Length - 1];
        String[] splited_ = server_file_name.Split('_');
        return splited_[0]+ ".png";
    }


    #endregion

    private void exit_bt_Click(object sender, EventArgs e)
    {

        Close();
    }
}

