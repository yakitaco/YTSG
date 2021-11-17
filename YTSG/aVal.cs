using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG {
    // 動的評価値計算
    class aVal {

        // 指定評価値を取得
        public static int get(KomaType type, int nx, int ny, int ox, int oy, int turn, BanInfo ban) {
            int val = 0;
            switch (type) {
                case KomaType.Fuhyou:
                    /* 飛車先を突くほうが評価値が高い */

                    /* 相手の角桂をつくほうが評価値が高い */


                    break;
                case KomaType.Kyousha:
                    break;
                case KomaType.Keima:
                    break;
                case KomaType.Ginsyou:
                    break;
                case KomaType.Hisya:
                    break;
                case KomaType.Kakugyou:
                    break;
                case KomaType.Kinsyou:
                    break;
                case KomaType.Ousyou:
                    break;
                case KomaType.Tokin:
                case KomaType.Narikyou:
                case KomaType.Narikei:
                case KomaType.Narigin:
                case KomaType.Ryuuou:
                case KomaType.Ryuuma:
                    /* 相手の玉に近いほうが評価値が高い */
                    var ak = ban.KingKo[turn == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE];
                    if (ak != null) val = Math.Abs(ak.x - nx) + Math.Abs(ak.y - nx) - Math.Abs(ak.x - ox) - Math.Abs(ak.y - ox);
                    break;
                default:
                    /* 何もしない */

                    break;
            }




            return 0;
        }
    }
}
