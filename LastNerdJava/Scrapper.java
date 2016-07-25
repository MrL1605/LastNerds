import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.net.URL;
import java.util.List;
import java.util.Scanner;

public class Scrapper {

    public static final String TAG = "UnivJava Log";
    public static final String HOST_URL = "http://www.lastnerdsonearth.com";
    public static final String PATH_NAME = "./Comics/";
	public static final String ANSI_GREEN = "\u001B[32m";
	public static final String ANSI_RESET = "\u001B[0m";
    int seq_num = 0;

	public static void main(String args[]){
	
		Scanner sc = new Scanner(System.in);
		checkAndCreateDir();
		System.out.println("Do you want to start the download? Y or n");
		String resp = sc.nextLine();
		if (resp.equals("Y")){
		
		    String first_comic = "/ch1p01";
		    CheckAndDownload comic_ = new CheckAndDownload();
		    comic_.setCurrentComicPage(first_comic);
		    comic_.run();
		    // new CheckAndDownload().execute(first_comic);
		    
		}
		
	}
	
	public static void checkAndCreateDir(){

        // Create dir if it does not exists
        File dir = new File(PATH_NAME);
        if (!dir.exists()) {
            dir.mkdirs();
        }
	}
	
}
