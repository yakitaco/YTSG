using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG {
    // USIプロトコルのインタフェース
    class usiIO {
        //USI座標→内部座標変換
        public int usi2pos(string usi, out koPos src, out koPos dst, out KomaType type, out bool nari) {

            //駒打ち
            if (usi.Substring(1, 1) == "*") {
                src = new koPos(9, 9);
                dst = new koPos(Convert.ToInt32(usi.Substring(2, 1)) - 1, dafb2int(usi.Substring(3, 1)));
                type = kafb2Ktype(usi.Substring(0, 1));
                nari = false;
            } else {
                //駒移動
                src = new koPos(Convert.ToInt32(usi.Substring(0, 1)) - 1, dafb2int(usi.Substring(1, 1)));
                dst = new koPos(Convert.ToInt32(usi.Substring(2, 1)) - 1, dafb2int(usi.Substring(3, 1)));
                type = KomaType.None;
                if ((usi.Length == 5) && (usi.Substring(4, 1) == "+"))  //駒成り
                {
                    nari = true;
                } else {
                    nari = false;
                }
            }
            return 0;
        }

        //内部座標→USI座標変換
        public string pos2usi(koma ko, koPos pos) {
            string usiStr = "";
            if (ko.x == 9) {
                usiStr = ktype2Kafb(ko.type) + "*" + (pos.x + 1).ToString() + int2Dafb(pos.y + 1);
            } else {
                if (pos.nari == true) {
                    usiStr = (ko.x + 1).ToString() + int2Dafb(ko.y + 1) + (pos.x + 1).ToString() + int2Dafb(pos.y + 1) + "+"; //成有り
                } else {
                    usiStr = (ko.x + 1).ToString() + int2Dafb(ko.y + 1) + (pos.x + 1).ToString() + int2Dafb(pos.y + 1);
                }
            }
            return usiStr;
        }

        //駒打ち用
        public string ktype2Kafb(KomaType type) {
            string usiStr = "";
            switch (type) {
                case KomaType.Fuhyou:
                    usiStr = "P";
                    break;
                case KomaType.Kyousha:
                    usiStr = "L";
                    break;
                case KomaType.Keima:
                    usiStr = "N";
                    break;
                case KomaType.Ginsyou:
                    usiStr = "S";
                    break;
                case KomaType.Hisya:
                    usiStr = "R";
                    break;
                case KomaType.Kakugyou:
                    usiStr = "B";
                    break;
                case KomaType.Kinsyou:
                    usiStr = "G";
                    break;
                case KomaType.Ousyou:  //ありえないが
                    usiStr = "K";
                    break;
                default:
                    break;
            }

            return usiStr;
        }


        //駒打ち用
        public KomaType kafb2Ktype(string usiStr) {
            KomaType type = KomaType.None;
            switch (usiStr) {
                case "P":
                    type = KomaType.Fuhyou;
                    break;
                case "L":
                    type = KomaType.Kyousha;
                    break;
                case "N":
                    type = KomaType.Keima;
                    break;
                case "S":
                    type = KomaType.Ginsyou;
                    break;
                case "R":
                    type = KomaType.Hisya;
                    break;
                case "B":
                    type = KomaType.Kakugyou;
                    break;
                case "G":
                    type = KomaType.Kinsyou;
                    break;
                case "K":  //ありえないが
                    type = KomaType.Ousyou;
                    break;
                default:
                    break;
            }

            return type;
        }

        public string int2Dafb(int val) {
            string usiStr = "";
            switch (val) {
                case 1:
                    usiStr = "a";
                    break;
                case 2:
                    usiStr = "b";
                    break;
                case 3:
                    usiStr = "c";
                    break;
                case 4:
                    usiStr = "d";
                    break;
                case 5:
                    usiStr = "e";
                    break;
                case 6:
                    usiStr = "f";
                    break;
                case 7:
                    usiStr = "g";
                    break;
                case 8:
                    usiStr = "h";
                    break;
                case 9:
                    usiStr = "i";
                    break;
                default:
                    break;
            }

            return usiStr;
        }

        public int dafb2int(string str) {
            int val = 0; ;
            switch (str) {
                case "a":
                    val = 0;
                    break;
                case "b":
                    val = 1;
                    break;
                case "c":
                    val = 2;
                    break;
                case "d":
                    val = 3;
                    break;
                case "e":
                    val = 4;
                    break;
                case "f":
                    val = 5;
                    break;
                case "g":
                    val = 6;
                    break;
                case "h":
                    val = 7;
                    break;
                case "i":
                    val = 8;
                    break;
                default:
                    break;
            }

            return val;
        }
    }
}
