using System;
using System.Collections.Generic;

namespace YTSG {




    //盤情報
    class BanInfo {
        public koma[,] BanKo = new koma[TEIGI.SIZE_SUZI, TEIGI.SIZE_DAN];   //盤上情報
        public List<koma>[] OkiKo = new List<koma>[2];                      //置き駒情報 (BanKo[0]:先手 / BanKo[1]:後手)
        public List<koma>[,] MochiKo = new List<koma>[2, 7];                //持ち駒情報 (MochiKo[0,]:先手 / MochiKo[1,]:後手 [,0-7]各駒)
        public int[,,] IdouList = new int[2, 9, 9];                         //駒の移動可能リスト
        public List<int>[] nifList = new List<int>[2];                      //二歩リスト

        //平手の盤情報生成(盤情報,置き駒情報,持ち駒情報)
        public BanInfo() {
            OkiKo[TEIGI.TEBAN_SENTE] = new List<koma>();
            OkiKo[TEIGI.TEBAN_GOTE] = new List<koma>();
            for (int i = 0; i < 7; i++) {
                MochiKo[TEIGI.TEBAN_SENTE, i] = new List<koma>();
                MochiKo[TEIGI.TEBAN_GOTE, i] = new List<koma>();
            }
            nifList[TEIGI.TEBAN_SENTE] = new List<int>();
            nifList[TEIGI.TEBAN_GOTE] = new List<int>();
            //王の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Ousyou, 4, 8));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Ousyou, 4, 0));

            //金の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Kinsyou, 3, 8));
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Kinsyou, 5, 8));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Kinsyou, 3, 0));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Kinsyou, 5, 0));

            //銀の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Ginsyou, 2, 8));
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Ginsyou, 6, 8));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Ginsyou, 2, 0));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Ginsyou, 6, 0));

            //桂の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Keima, 1, 8));
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Keima, 7, 8));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Keima, 1, 0));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Keima, 7, 0));

            //香の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Kyousha, 0, 8));
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Kyousha, 8, 8));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Kyousha, 0, 0));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Kyousha, 8, 0));

            //角の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Kakugyou, 7, 7));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Kakugyou, 1, 1));

            //飛の配置
            this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Hisya, 1, 7));
            this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Hisya, 7, 1));

            //歩の配置
            for (int i = 0; i < 9; i++) {
                this.addKoma(new koma(TEIGI.TEBAN_SENTE, KomaType.Fuhyou, i, 6));
                this.addKoma(new koma(TEIGI.TEBAN_GOTE, KomaType.Fuhyou, i, 2));
            }

        }

        //盤情報作成(指定の場面)
        public BanInfo(string oki, string mochi) {
            OkiKo[TEIGI.TEBAN_SENTE] = new List<koma>();
            OkiKo[TEIGI.TEBAN_GOTE] = new List<koma>();
            for (int i = 0; i < 7; i++) {
                MochiKo[TEIGI.TEBAN_SENTE, i] = new List<koma>();
                MochiKo[TEIGI.TEBAN_GOTE, i] = new List<koma>();
            }
            nifList[TEIGI.TEBAN_SENTE] = new List<int>();
            nifList[TEIGI.TEBAN_GOTE] = new List<int>();

            // 場面の設定
            int j = 0;
            for (int i = 0; i < TEIGI.SIZE_DAN; i++) {
                int suzi = TEIGI.SIZE_SUZI - 1;
                while (suzi > -1) {
                    // 数字(空白の数)
                    if (int.TryParse(oki.Substring(j, 1), out var num)) {
                        suzi -= num;
                        j++;
                    } else {
                        // 駒配置
                        this.addKoma(sfen.toKoma(oki, ref j, suzi, i));
                        suzi--;
                    }
                }
                j++;
            }

            // 持ち駒の設定
            j = 0;
            while ((j < mochi.Length) && (mochi[j] != '-')) {
                // 数字(駒の数) ★2桁もアリ(未実装)
                int num = 0;
                if (int.TryParse(mochi.Substring(j, 1), out num)) {
                    j++;
                } else {
                    num = 1;
                }
            
                // 持ち駒追加(複数駒ありを考慮)
                koma k = sfen.toKoma(mochi, ref j, 9, 9);
                for (int i = 0; i < num; i++) {
                    this.addKoma(new koma(k.p, k.type, 9, 9));
                }
            }
        }

        //盤情報作成(コピー)
        public BanInfo(BanInfo ban) {
            OkiKo[TEIGI.TEBAN_SENTE] = new List<koma>();
            OkiKo[TEIGI.TEBAN_GOTE] = new List<koma>();
            for (int i = 0; i < 7; i++) {
                MochiKo[TEIGI.TEBAN_SENTE, i] = new List<koma>();
                foreach (koma km in ban.MochiKo[TEIGI.TEBAN_SENTE, i]) {
                    MochiKo[TEIGI.TEBAN_SENTE, i].Add(new koma(km));
                }

                MochiKo[TEIGI.TEBAN_GOTE, i] = new List<koma>();
                foreach (koma km in ban.MochiKo[TEIGI.TEBAN_GOTE, i]) {
                    MochiKo[TEIGI.TEBAN_GOTE, i].Add(new koma(km));
                }
            }
            nifList[TEIGI.TEBAN_SENTE] = new List<int>();
            nifList[TEIGI.TEBAN_GOTE] = new List<int>();

            foreach (koma km in ban.OkiKo[TEIGI.TEBAN_SENTE]) {
                koma tmp_km = new koma(km);
                OkiKo[TEIGI.TEBAN_SENTE].Add(tmp_km);
                BanKo[km.x, km.y] = tmp_km;
            }

            foreach (koma km in ban.OkiKo[TEIGI.TEBAN_GOTE]) {
                koma tmp_km = new koma(km);
                OkiKo[TEIGI.TEBAN_GOTE].Add(tmp_km);
                BanKo[km.x, km.y] = tmp_km;
            }

        }

        //駒の追加(駒指定:[駒情報]) (0:追加OK, -1:追加NG(すでに駒が存在する))
        public int addKoma(koma ko) {
            if (ko.x == 9) {
                MochiKo[ko.p, (int)ko.type - 1].Add(ko);
            } else {
                if (BanKo[ko.x, ko.y] != null) return -1;  //盤上に既に存在
                BanKo[ko.x, ko.y] = ko;
                OkiKo[ko.p].Add(ko);
            }
            return 0;
        }

        //二歩リスト更新
        public void renewNifList(int teban) {
            nifList[teban].Clear(); //リスト初期化

            //if (OkiKo[teban].Count > 0) {
            for (int i = 0; i < TEIGI.SIZE_SUZI; i++) {  //筋(X)
                if (OkiKo[teban].Exists(k => (k.type == KomaType.Fuhyou && k.x == i)) == true) {  //その筋上に味方の歩があるか
                    nifList[teban].Add(i);
                }
            }
            //}
        }


        //駒の移動(駒指定:[駒情報][移動先位置][成有無][駒移動チェック有無]) (0:移動OK, -1:移動NG(駒が存在しない,移動できない 等))
        //移動先に敵の駒が存在したら取れる
        public int moveKoma(koma ko, koPos dstPos, bool naru, bool chk) {
            if (chk == true) {
                ko.chkMove(ko, dstPos, this);
            }

            /* 敵駒を取る(味方駒でも取れる) */
            if (BanKo[dstPos.x, dstPos.y] != null) {
                koma toriKo = BanKo[dstPos.x, dstPos.y];  //取る駒
                toriKo.x = 9;  //持ち駒状態
                toriKo.y = 9;  //持ち駒状態
                toriKo.doKModori();  //成り状態を戻す
                OkiKo[toriKo.p].Remove(toriKo);
                MochiKo[ko.p, (int)toriKo.type - 1].Add(toriKo);

                toriKo.p = ko.p;  //取った駒の手番(一番最後に変更)
            }

            /* 駒打ち */
            if (ko.x == 9) {
                MochiKo[ko.p, (int)ko.type - 1].Remove(ko);
                OkiKo[ko.p].Add(ko);
            } else {
                /* 駒移動 */
                BanKo[ko.x, ko.y] = null;
                if (naru == true) ko.doKNari(); //駒成り
            }

            BanKo[dstPos.x, dstPos.y] = ko;
            ko.x = dstPos.x;
            ko.y = dstPos.y;

            return 0;
        }

        //駒打ち用
        public int moveKoma(int teban, KomaType type, koPos dstPos, bool chk) {
            koma ko = MochiKo[teban, (int)type - 1].Find(k => k.type == type);

            if (ko != null) {
                moveKoma(ko, dstPos, false, chk);
            } else {
                return -1;
            }

            return 0;
        }

        //駒の移動(座標指定:[移動元位置][移動先位置][駒種(打つのみ)][成有無][駒移動チェック有無]) (0:移動OK, -1:移動NG(駒が存在しない,移動できない 等))
        public int moveKoma(koPos srcPos, koPos dstPos, bool naru, bool chk) {
            koma ko = BanKo[srcPos.x, srcPos.y];

            if (ko != null) {
                moveKoma(ko, dstPos, naru, chk);
            } else {
                return -1;
            }

            return 0;
        }

        //駒の削除(座標指定:[位置][駒種(持ちのみ)]) (0:削除OK, -1:削除NG (駒が存在しない))
        public int delKoma(koPos pos, KomaType tyop) {
            return 0;
        }

        //駒の削除(駒指定:[駒情報]) (0:削除OK, -1:削除NG (駒が存在しない))
        public int delKoma(koma ko) {
            return 0;
        }
        
        //移動可能リスト更新(先手・後手の駒が移動可能場所を加算する)
        public void renewIdouList() {
            IdouList = new int[2, 9, 9];

            //指せる手を全てリスト追加
            foreach (koma km in OkiKo[TEIGI.TEBAN_SENTE]) {
                List<koPos> poslist = km.baninfo(this);

                foreach (koPos pos in poslist) {
                    IdouList[TEIGI.TEBAN_SENTE, pos.x, pos.y]++;
                }
            }
            foreach (koma km in OkiKo[TEIGI.TEBAN_GOTE]) {
                List<koPos> poslist = km.baninfo(this);

                foreach (koPos pos in poslist) {
                    IdouList[TEIGI.TEBAN_GOTE, pos.x, pos.y]++;
                }
            }
            
        }

        //移動リスト一覧作成
        public int[,,] idouList() {
            int[,,] list = new int[2, 9, 9];

            //指せる手を全てリスト追加
            foreach (koma km in OkiKo[TEIGI.TEBAN_SENTE]) {
                List<koPos> poslist = km.baninfo(this);

                foreach (koPos pos in poslist) {
                    list[TEIGI.TEBAN_SENTE, pos.x, pos.y]++;
                }
            }
            foreach (koma km in OkiKo[TEIGI.TEBAN_GOTE]) {
                List<koPos> poslist = km.baninfo(this);

                foreach (koPos pos in poslist) {
                    list[TEIGI.TEBAN_GOTE, pos.x, pos.y]++;
                }
            }

            return list;
        }

        public void kikiList() {
            foreach (koma km in OkiKo[TEIGI.TEBAN_SENTE]) {
                km.checkKomaKiki(this);
            }
            foreach (koma km in OkiKo[TEIGI.TEBAN_GOTE]) {
                km.checkKomaKiki(this);
            }
        }

        //現在の盤情報をASCIIで表示
        public string showBanInfo() {
            string str = "";
            for (int j = 0; j < TEIGI.SIZE_DAN; j++) {
                for (int i = TEIGI.SIZE_SUZI - 1; i >= 0; i--) {
                    // 駒が置いてある
                    if (BanKo[i, j] != null) {
                        // 先手
                        if (BanKo[i, j].p == 0) {
                            switch (BanKo[i, j].type) {
                                case KomaType.Fuhyou:
                                    str += "P_|";
                                    break;
                                case KomaType.Kyousha:
                                    str += "L_|";
                                    break;
                                case KomaType.Keima:
                                    str += "N_|";
                                    break;
                                case KomaType.Ginsyou:
                                    str += "S_|";
                                    break;
                                case KomaType.Hisya:
                                    str += "R_|";
                                    break;
                                case KomaType.Kakugyou:
                                    str += "B_|";
                                    break;
                                case KomaType.Kinsyou:
                                    str += "G_|";
                                    break;
                                case KomaType.Ousyou:
                                    str += "K_|";
                                    break;
                                case KomaType.Tokin:
                                    str += "+P|";
                                    break;
                                case KomaType.Narikyou:
                                    str += "+L|";
                                    break;
                                case KomaType.Narikei:
                                    str += "+N|";
                                    break;
                                case KomaType.Narigin:
                                    str += "+G|";
                                    break;
                                case KomaType.Ryuuou:
                                    str += "+R|";
                                    break;
                                case KomaType.Ryuuma:
                                    str += "+B|";
                                    break;
                                default:
                                    str += "!_|";
                                    break;
                            }

                            // 後手
                        } else {
                            switch (BanKo[i, j].type) {
                                case KomaType.Fuhyou:
                                    str += "p_|";
                                    break;
                                case KomaType.Kyousha:
                                    str += "l_|";
                                    break;
                                case KomaType.Keima:
                                    str += "n_|";
                                    break;
                                case KomaType.Ginsyou:
                                    str += "s_|";
                                    break;
                                case KomaType.Hisya:
                                    str += "r_|";
                                    break;
                                case KomaType.Kakugyou:
                                    str += "b_|";
                                    break;
                                case KomaType.Kinsyou:
                                    str += "g_|";
                                    break;
                                case KomaType.Ousyou:
                                    str += "k_|";
                                    break;
                                case KomaType.Tokin:
                                    str += "+p|";
                                    break;
                                case KomaType.Narikyou:
                                    str += "+l|";
                                    break;
                                case KomaType.Narikei:
                                    str += "+n|";
                                    break;
                                case KomaType.Narigin:
                                    str += "+g|";
                                    break;
                                case KomaType.Ryuuou:
                                    str += "+r|";
                                    break;
                                case KomaType.Ryuuma:
                                    str += "+b|";
                                    break;
                                default:
                                    str += "?_|";
                                    break;
                            }
                        }
                        // 駒がおいてない
                    } else {
                        str += "__|";
                    }




                }
                // 改行
                str += Environment.NewLine;


            }


            // 持ち駒の表示
            if (MochiKo[TEIGI.TEBAN_SENTE, 0]?.Count > 0) str += "P" + MochiKo[TEIGI.TEBAN_SENTE, 0].Count + " ";
            if (MochiKo[TEIGI.TEBAN_SENTE, 1]?.Count > 0) str += "L" + MochiKo[TEIGI.TEBAN_SENTE, 1].Count + " ";
            if (MochiKo[TEIGI.TEBAN_SENTE, 2]?.Count > 0) str += "N" + MochiKo[TEIGI.TEBAN_SENTE, 2].Count + " ";
            if (MochiKo[TEIGI.TEBAN_SENTE, 3]?.Count > 0) str += "S" + MochiKo[TEIGI.TEBAN_SENTE, 3].Count + " ";
            if (MochiKo[TEIGI.TEBAN_SENTE, 4]?.Count > 0) str += "R" + MochiKo[TEIGI.TEBAN_SENTE, 4].Count + " ";
            if (MochiKo[TEIGI.TEBAN_SENTE, 5]?.Count > 0) str += "B" + MochiKo[TEIGI.TEBAN_SENTE, 5].Count + " ";
            if (MochiKo[TEIGI.TEBAN_SENTE, 6]?.Count > 0) str += "G" + MochiKo[TEIGI.TEBAN_SENTE, 6].Count + " ";
            str += Environment.NewLine;
            if (MochiKo[TEIGI.TEBAN_GOTE, 0]?.Count > 0) str += "p" + MochiKo[TEIGI.TEBAN_GOTE, 0].Count + " ";
            if (MochiKo[TEIGI.TEBAN_GOTE, 1]?.Count > 0) str += "l" + MochiKo[TEIGI.TEBAN_GOTE, 1].Count + " ";
            if (MochiKo[TEIGI.TEBAN_GOTE, 2]?.Count > 0) str += "n" + MochiKo[TEIGI.TEBAN_GOTE, 2].Count + " ";
            if (MochiKo[TEIGI.TEBAN_GOTE, 3]?.Count > 0) str += "s" + MochiKo[TEIGI.TEBAN_GOTE, 3].Count + " ";
            if (MochiKo[TEIGI.TEBAN_GOTE, 4]?.Count > 0) str += "r" + MochiKo[TEIGI.TEBAN_GOTE, 4].Count + " ";
            if (MochiKo[TEIGI.TEBAN_GOTE, 5]?.Count > 0) str += "b" + MochiKo[TEIGI.TEBAN_GOTE, 5].Count + " ";
            if (MochiKo[TEIGI.TEBAN_GOTE, 6]?.Count > 0) str += "g" + MochiKo[TEIGI.TEBAN_GOTE, 6].Count + " ";

            return str;
        }

        // pos by player
        // 自分基準の位置 左上(0,0) 右下(9,9)
        //public static koPos posBplayer(koPos){
        //    
        //}


    }
}
