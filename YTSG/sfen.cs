using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG {
    // SFEN(Shogi Forsyth-Edwards Notation)表記法
    static class sfen {
        // SFEN 形式からkomaクラスへ(先頭のみ)[cnt 何文字目か]
        public static koma toKoma(string str, ref int cnt, int suzi, int dan) {
            bool nari = false;
            KomaType type = KomaType.None;
            int teban = TEIGI.TEBAN_SENTE;
            // 成り
            if (str[cnt] == '+') {
                cnt++;
                nari = true;
            }
            // 駒
            switch (str[cnt]) {
                case 'P':
                    type = KomaType.Fuhyou;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'L':
                    type = KomaType.Kyousha;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'N':
                    type = KomaType.Keima;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'S':
                    type = KomaType.Ginsyou;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'R':
                    type = KomaType.Hisya;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'B':
                    type = KomaType.Kakugyou;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'G':
                    type = KomaType.Kinsyou;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'K':
                    type = KomaType.Ousyou;
                    teban = TEIGI.TEBAN_SENTE;
                    break;
                case 'p':
                    type = KomaType.Fuhyou;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 'l':
                    type = KomaType.Kyousha;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 'n':
                    type = KomaType.Keima;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 's':
                    type = KomaType.Ginsyou;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 'r':
                    type = KomaType.Hisya;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 'b':
                    type = KomaType.Kakugyou;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 'g':
                    type = KomaType.Kinsyou;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                case 'k':
                    type = KomaType.Ousyou;
                    teban = TEIGI.TEBAN_GOTE;
                    break;
                default:
                    break;
            }
            cnt++;
            koma k = new koma(teban, type, suzi, dan);

            // 成りフラグがONの場合、裏返す
            if (nari == true) k.doKNari();

            return k;
        }

        // komaクラスからSFEN形式へ
        //public string fromKoma(koma k){
        //
        //}

    }
}
