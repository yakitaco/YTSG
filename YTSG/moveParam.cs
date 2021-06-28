using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG {
    class moveParam {
        int move = 0;
        int pos = 0;

        // 手筋情報 [現在の位置][手筋パラメータ一覧]
        private int[][] param= new int[100][];

        // ★手筋パラメータ一覧
        // 0 : 何手目 (0オリジン)
        // 1 : なんて先まで読むか (1オリジン)
        // 2 : 定跡情報を使用するか (1 使用する)
        // 3 : 重みづけ情報ID (0: 使用しない 1～ID)
        // 4 : 高速モードを使用するか (1 使用する)


        // 現在の手筋情報を返す
        public int[] prm {
            get {
                return param[pos];
            }
        }

        // 手筋情報ファイル(CSV形式)を読み取る
        // ret=0 読み取り成功 -1 読み取り失敗(デフォルトを設定)
        public int readParam(string fname) {

            for (int i = 0;i< 100; i++) {
                param[i] = new int[10];
            }


            return 0;
        }


        // デフォルトパラメータ設定
        public void setDefParam() {


        }

        // 手をカウントアップ
        public void incParamMove() {
            move++;
            // 次の手筋情報に更新要の場合
            if (move == param[pos+1][0]) {
                pos++;
            }
        }

    }
}
