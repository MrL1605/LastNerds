package co.mrl.lastnerds;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import java.util.Timer;
import java.util.TimerTask;

/**
 * Created by MrL on 3/1/16.
 */
public class SplashScreen extends Activity {

    @Override protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.splash_screen);

        Timer t = new Timer();
        t.schedule(new TimerTask() {
            @Override public void run() {

                finish();
                startActivity(new Intent(getBaseContext(), MainActivity.class));
            }
        }, 2000);


    }
}
