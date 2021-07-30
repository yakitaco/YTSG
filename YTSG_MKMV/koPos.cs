using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG_MKMV {

    //駒の位置
    class koPos : System.IComparable<koPos> {
        public int x;
        public int y;
        public int val;
        public bool nari = false;

        public koPos(int _x, int _y) {
            x = _x;
            y = _y;
            val = 0;
        }

        public koPos(int _x, int _y, int _val) {
            x = _x;
            y = _y;
            val = _val;
        }

        // 比較用の処理
        //  0 なら同じ
        // -1 なら比べた相手の方が大きい
        //  1 なら比べた相手の方が小さい
        public int CompareTo(koPos i_other) {
            // Ageの数と名前の文字列の数を足したものを、比較用の数値する
            int thisNum = this.val;
            int otherNum = i_other.val;

            if (thisNum == otherNum) {
                return 0;
            }

            // 数値が"低い"方が偉い(大きい)扱いにする！
            if (thisNum > otherNum) {
                return 1;
            }
            return -1;
        }

        // 移動先の評価値を設定
        public koPos setVal(int _val) {
            val = _val;
            return this;
        }

        public koPos setNari(bool _nari) {
            nari = _nari;
            return this;
        }

        public koPos Copy() {
            return (koPos)MemberwiseClone();
        }

    }

}
