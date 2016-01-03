package co.mrl.lastnerds;

import com.orm.SugarRecord;

/**
 * Created by MrL on 3/1/16.
 */
public class ImagesTable extends SugarRecord<ImagesTable> {

    String file_name;
    int seq;

    public ImagesTable() {
    }

    public ImagesTable(String file_name, int seq) {
        this.file_name = file_name;
        this.seq = seq;
    }

}
