import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.net.URL;
import java.net.HttpURLConnection;
import java.net.URLConnection;
import java.util.List;


public class CheckAndDownload extends Thread{

    public static final String TAG = "UnivJava Log";
    public static final String HOST_URL = "http://www.lastnerdsonearth.com";
    public static final String PATH_NAME = "./Comics/";
	public static final String ANSI_GREEN = "\u001B[32m";
	public static final String ANSI_RESET = "\u001B[0m";
	public static final String ANSI_BLUE = "\u001B[34m";
	public static final String ANSI_RED = "\u001B[31m";
	public static final String ANSI_YELLOW = "\u001B[33m";
	
    int seq_num = 0;
    String curr_comic_page = "";
    
    private void publishProgress(String val){
    	if (val.equals("62")) {
            append_text("                                   " + ANSI_BLUE + "File Exists, Skips Download" + ANSI_RESET);
        }
        int _val = Integer.parseInt(val) / 10;
        String print_str = "\r[";
        for (int i=0; i<10; i++){
        	if (i<_val){
        		print_str += "-";
        	}else if (i == _val){
        		print_str += ">";
        	}else{
        		print_str += " ";
        	}
        }
        print_str += "]    " + val + "%";
        append_text(ANSI_GREEN + print_str + ANSI_RESET);

    }
    
    public void setCurrentComicPage(String name){
    	curr_comic_page = name;
    }

    public void onPostExecute(String s){
    	if (s.equals("Down")) {
            append_text(ANSI_RED + "\nWebsite Down or Network Connection Problem\n" + ANSI_RESET);
        } else {
            if (!s.equals("Last")) {
                append_text(ANSI_GREEN + " - Complete - " + curr_comic_page + "\n" + ANSI_RESET);
                seq_num += 1;
    		    CheckAndDownload _tmp = new CheckAndDownload();
				_tmp.setCurrentComicPage(s);
				_tmp.run();
                // new CheckAndDownload().execute(s);
            } else {
                append_text(ANSI_YELLOW + "\n\nLatest Released Comic " + curr_comic_page + "\nDownload Fully Completed." + ANSI_RESET);
            }
        }
    }
    

    @Override public void run(){
    
        String href_val, img_download_url;
        // curr_comic_page = params[0];

        try {
			
			publishProgress("0");
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
            Thread.sleep(100);
            Element content_ele = doc.getElementById("content");

            // Check if last Image
            if (content_ele.child(0).attr("src").equals("you-could-be-reading.png")) {

                // Yes then substitute None ot curr url to stop proceeding
                href_val = "Last";
                //img_download_url = "http://lastnerdsonearth.com/patreon/you-could-be-reading.png";

                publishProgress("100");
                onPostExecute(href_val);
                return;

            } else {

                // If no extract next page url and current image url.
                Element a_tag = content_ele.child(2);
                Element img_tag = a_tag.select("img").first();
                href_val = a_tag.attr("href");
                img_download_url = img_tag.attr("src");
            }

            publishProgress("60");
            Thread.sleep(100);
            // Check if comic file exists
            // and download if not
            String img_file_name = UrlToFileName(img_download_url);
            
            
            //System.out.println(TAG + ": File Name  " + img_file_name);
            File file = new File(PATH_NAME + img_file_name);

            if (!file.exists()) {

				//Download Image File
				URL url = new URL(img_download_url);
				URLConnection urlConn = url.openConnection();
				urlConn.setRequestProperty("User-Agent","Mozilla/5.0 (Windows NT 6.1; rv:7.0.1) Gecko/20100101 Firefox/7.0.1");
				InputStream in = urlConn.getInputStream();
                file.createNewFile();
                FileOutputStream fos = new FileOutputStream(file);
				int c;
				byte[] b = new byte[1024];
				while ((c = in.read(b)) != -1)
					fos.write(b, 0, c);
			
                //fos.write(output.toByteArray());
                // is.close();
                fos.close();

				if (in != null)
					in.close();
				if (fos != null)
					fos.close();


            } else {
                // File Exists
                publishProgress("62");
            }
            publishProgress("90");


        } catch (Exception e) {
            System.out.println(TAG + ": Exception - " + e.toString());
            onPostExecute("Down");
            return;
        }

        publishProgress("100");
        onPostExecute(href_val);
        return;
    	
    }
    
    public static void append_text(String s) {

		System.out.print(s);
		
		// String curr = logs_tv.getText().toString();
		// curr += s;
		// logs_tv.setText(curr);

	}

	public static String UrlToFileName(String url) {

		String[] splits = url.split("/");
		String server_file_name = splits[splits.length - 1];
		String[] splits_ = server_file_name.split("_");
		return splits_[0] + ".png";
	}

}


// http://www.lastnerdsonearth.com/img/ch1/ch1p01_i-stole-that-line-from-myself.png
