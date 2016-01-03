package co.mrl.lastnerds;

import android.app.Activity;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.Toast;

import java.io.File;
import java.util.List;

/**
 * Created by MrL on 3/1/16.
 */
public class ViewComics extends Activity {

    public static final String PATH_NAME = "/LastNerds/";
    public static final String TAG = "ViewComics TAG";
    public int curr_seq = 1;
    TouchImageView touch_iv;

    @Override protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.view_comic);

        touch_iv = (TouchImageView) findViewById(R.id.touch_iv);
        new ShowImage().execute("" + curr_seq);

        findViewById(R.id.nxt_bt).setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) {

                curr_seq++;
                new ShowImage().execute("" + curr_seq);
            }
        });

        findViewById(R.id.prev_bt).setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) {

                curr_seq--;
                new ShowImage().execute("" + curr_seq);
            }
        });
    }

    public class ShowImage extends AsyncTask<String,String,String>{

        @Override protected String doInBackground(String... params) {

            List<ImagesTable> first_row = ImagesTable.find(ImagesTable.class, "seq=?", params[0]);
            if (first_row.size() != 1){
                return "No";
            }

            Log.i(TAG, "size - " + first_row.size());
            String file_name = first_row.get(0).file_name;

            File root = android.os.Environment.getExternalStorageDirectory();
            File file = new File(root.getAbsolutePath() + PATH_NAME + "/" + file_name);

            if(file.exists()){
                return file_name;
            }else{
                return "No";
            }
        }

        @Override protected void onPostExecute(String s) {
            super.onPostExecute(s);

            if (s.equals("No")){
                Toast.makeText(ViewComics.this, "No Files left", Toast.LENGTH_LONG).show();
                finish();
            }else {

                File root = android.os.Environment.getExternalStorageDirectory();
                File file = new File(root.getAbsolutePath() + PATH_NAME + "/" + s);
                touch_iv.setImageURI(Uri.parse(file.getAbsolutePath()));

            }
        }
    }
}
