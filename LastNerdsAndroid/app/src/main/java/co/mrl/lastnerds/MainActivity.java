package co.mrl.lastnerds;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.net.URL;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    public static final String TAG = "MainActivity Log";
    public static final String HOST_URL = "http://www.lastnerdsonearth.com";
    public static final String PATH_NAME = "/LastNerds/";
    public TextView logs_tv;
    public ProgressBar pro_bar;
    public Button run_bt;
    int seq_num = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        logs_tv = (TextView) findViewById(R.id.logs_tv);
        run_bt = (Button) findViewById(R.id.run_bt);
        pro_bar = (ProgressBar) findViewById(R.id.progress_bar);
        pro_bar.setMax(100);
        pro_bar.setVisibility(View.INVISIBLE);

        // Create dir if it does not exists
        File root = android.os.Environment.getExternalStorageDirectory();
        File dir = new File(root.getAbsolutePath() + PATH_NAME);
        if (!dir.exists()) {
            dir.mkdirs();
        }

        findViewById(R.id.view_bt).setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) {

                startActivity(new Intent(getBaseContext(), ViewComics.class));

            }
        });

        run_bt.setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) {

                seq_num = 1;
                String first_comic = "/ch1p01";
                run_bt.setEnabled(false);
                pro_bar.setVisibility(View.VISIBLE);
                new CheckAndDownload().execute(first_comic);
            }
        });

    }

    public class CheckAndDownload extends AsyncTask<String, String, String> {

        String curr_comic_page = "";

        @Override protected void onProgressUpdate(String... values) {
            super.onProgressUpdate(values);
            if (values[0].equals("62")) {
                append_text(" File Exists, Skips Download ");
            }

            pro_bar.setProgress(Integer.parseInt(values[0]));
            append_text(values[0] + "%, ");
        }

        @Override protected void onPostExecute(String s) {
            super.onPostExecute(s);
            if (s.equals("Down")) {
                append_text("\nWebsite Down or Network Connection Problem\n");
                pro_bar.setVisibility(View.INVISIBLE);
                run_bt.setEnabled(true);
            } else {
                if (!s.equals("Last")) {
                    append_text(" - Complete - " + curr_comic_page + "\n");
                    seq_num += 1;
                    new CheckAndDownload().execute(s);
                } else {
                    run_bt.setEnabled(true);
                    append_text("\n\nLatest Released Comic " + curr_comic_page + "\nDownload Fully Completed.");
                }
            }
        }

        @Override protected String doInBackground(String... params) {

            String href_val, img_download_url;
            curr_comic_page = params[0];

            try {

                Document doc = Jsoup.connect(HOST_URL + curr_comic_page)
                        .userAgent("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:39.0) Gecko/20100101 Firefox/39.0")
                                // Extra headers
                        .header("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
                        .header("Accept-Encoding", "gzip, deflate")
                        .header("Accept-Language", "en-US,en;q=0.7,mr;q=0.3")
                        .header("Host", "lastnerdsonearth.com")
                        .header("Connection", "keep-alive")
                        .header("DNT", "1")
                        .followRedirects(true)
                        .timeout(30000)
                        .get();

                publishProgress("30");
                Element content_ele = doc.getElementById("content");

                // Check if last Image
                if (content_ele.child(0).attr("src").equals("you-could-be-reading.png")) {

                    // Yes then substitute None ot curr url to stop proceeding
                    href_val = "Last";
                    //img_download_url = "http://lastnerdsonearth.com/patreon/you-could-be-reading.png";

                    publishProgress("100");
                    return href_val;

                } else {

                    // If no extract next page url and current image url.
                    Element a_tag = content_ele.child(2);
                    Element img_tag = a_tag.select("img").first();
                    href_val = a_tag.attr("href");
                    img_download_url = img_tag.attr("src");
                }

                publishProgress("60");
                // Check if comic file exists
                // and download if not
                String img_file_name = UrlToFileName(img_download_url);
                Log.i(TAG, "File Name  " + img_file_name);
                File root = android.os.Environment.getExternalStorageDirectory();
                File file = new File(root.getAbsolutePath() + PATH_NAME + "/" + img_file_name);

                if (!file.exists()) {

                    //Download Image File
                    URL url = new URL(img_download_url);
                    InputStream is = (InputStream) url.getContent();
                    ByteArrayOutputStream output = new ByteArrayOutputStream();
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    publishProgress("80");

                    while ((bytesRead = is.read(buffer)) != -1) {
                        output.write(buffer, 0, bytesRead);
                    }

                    //region Save downloaded image to file in specified Location
                    file.createNewFile();
                    FileOutputStream fos = new FileOutputStream(file);
                    fos.write(output.toByteArray());
                    fos.close();

                    List<ImagesTable> first_row = ImagesTable.find(ImagesTable.class, "seq=?", seq_num + "");
                    if (first_row.size() <= 0){
                        int count = Integer.parseInt(ImagesTable.count(ImagesTable.class) + "") + 1;
                        ImagesTable new_image = new ImagesTable(img_file_name, count);
                        new_image.save();
                    }


                } else {
                    // File Exists
                    publishProgress("62");
                }
                publishProgress("90");


            } catch (Exception e) {
                Log.e(TAG, "Exception - " + e.toString());
                return "Down";
            }

            publishProgress("100");
            return href_val;
        }
    }

    private void append_text(String s) {

        String curr = logs_tv.getText().toString();
        curr += s;
        logs_tv.setText(curr);

    }

    public String UrlToFileName(String url) {

        String[] splits = url.split("/");
        String server_file_name = splits[splits.length - 1];
        String[] splits_ = server_file_name.split("_");
        return splits_[0] + ".png";
    }

}
