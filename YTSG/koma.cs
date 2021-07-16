using System.Collections.Generic;

namespace YTSG {
    //駒情報
    class koma {

        //駒の動き情報
        static uint[] KoMove =
        {
            0b0000000000000000,	 //なし：0
            0b0000000000000001,	 //歩兵：1
            0b0000000001000000,  //香車：7
            0b0000000000100000,  //桂馬：6
            0b0000000000010101,  //銀将：1 + 3 + 5
            0b0000000010000000,  //飛車：8
            0b0000000100000000,  //角行：9
            0b0000000000001111,  //金将：1 + 2 + 3 + 4
            0b0000000000011111,  //王将：1 + 2 + 3 + 4 + 5
            0b0000000000001111,	 //と金(成歩兵)：1 + 2 + 3 + 4
            0b0000000000001111,  //成香：1 + 2 + 3 + 4
            0b0000000000001111,  //成桂：1 + 2 + 3 + 4
            0b0000000000001111,  //成銀：1 + 2 + 3 + 4
            0b0000000010010100,  //竜王：3 + 5 + 8
            0b0000000100001011,  //竜馬：1 + 2 + 4 + 9
        };
        // 1:前 2:後 3:斜め前 4:横 5:斜め後ろ 6:桂馬 7:前ずっと 8:右左後ろずっと 9: 斜めずっと

        static int[] KoScore =
        {
            0,	 //なし：0
            100,	 //歩兵：1
            500,  //香車：7
            600,  //桂馬：6
            800,  //銀将：1 + 3 + 5
            1500,  //飛車：7 + 8
            1200,  //角行：9
            900,  //金将：1 + 2 + 3 + 4
            99999,  //王将：1 + 2 + 3 + 4 + 5
            200,	 //と金(成歩兵)：1 + 2 + 3 + 4
            550,  //成香：1 + 2 + 3 + 4
            650,  //成桂：1 + 2 + 3 + 4
            900,  //成銀：1 + 2 + 3 + 4
            1600,  //竜王：1 + 2 + 3 + 4 + 5 + 7 + 8
            1300,  //竜馬：1 + 2 + 3 + 4 + 5 + 9
        };

        static List<mateMval>[] mateMove = new List<mateMval>[15];

        class mateMval {
            int x;
            int y;

        }

        public int p; //所持プレイヤー (0:先手/1:後手)
        public KomaType type;
        public int x; //筋 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]
        public int y; //段 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]

        // プレイヤーから見た位置(0,0 左上)
        public int px {
            get {
                if ((x < 9) && (x > -1)) {
                    return p == TEIGI.TEBAN_SENTE ? x : TEIGI.SIZE_SUZI - x - 1;
                } else {
                    return x;
                }
            }
        }
        public int py {
            get {
                if ((y < 9) && (y > -1)) {
                    return p == TEIGI.TEBAN_SENTE ? y : TEIGI.SIZE_DAN - y - 1;
                } else {
                    return y;
                }
            }
        }
        
        // 相手のプレイヤー(0:先手/1:後手)
        public int ap {
            get {
                return p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE;
            }
        }

        // 初期化関数
        static koma() {
            for (int i = 0 ; i < 15 ; i++) {
                mateMove[i] = new List<mateMval>();
            }

        }


        //駒が成れるか
        public bool chkNari() {
            bool chk = false;
            switch (type) {
                case KomaType.None:	    //なし
                case KomaType.Fuhyou:	//歩兵
                case KomaType.Kyousha:  //香車
                case KomaType.Keima:    //桂馬
                case KomaType.Ginsyou:  //銀将
                case KomaType.Hisya:    //飛車
                case KomaType.Kakugyou: //角行
                    chk = true;
                    break;
            }
            return chk;
        }

        //駒を裏にする(駒成)
        public int doKNari() {
            int ret = 0;
            switch (type) {
                case KomaType.Fuhyou:	//歩兵
                    type = KomaType.Tokin;
                    break;
                case KomaType.Kyousha:  //香車
                    type = KomaType.Narikyou;
                    break;
                case KomaType.Keima:    //桂馬
                    type = KomaType.Narikei;
                    break;
                case KomaType.Ginsyou:  //銀将
                    type = KomaType.Narigin;
                    break;
                case KomaType.Hisya:    //飛車
                    type = KomaType.Ryuuou;
                    break;
                case KomaType.Kakugyou: //角行
                    type = KomaType.Ryuuma;
                    break;
                default:        //裏返せない場合
                    ret = -1;
                    break;
            }
            return ret;
        }

        //駒を表にする(駒取)
        public int doKModori() {
            int ret = 0;
            switch (type) {
                case KomaType.Tokin:	//と金(成歩兵)
                    type = KomaType.Fuhyou;
                    break;
                case KomaType.Narikyou: //成香
                    type = KomaType.Kyousha;
                    break;
                case KomaType.Narikei:  //成桂
                    type = KomaType.Keima;
                    break;
                case KomaType.Narigin:  //成銀
                    type = KomaType.Ginsyou;
                    break;
                case KomaType.Ryuuou:   //竜王
                    type = KomaType.Hisya;
                    break;
                case KomaType.Ryuuma:   //竜馬
                    type = KomaType.Kakugyou;
                    break;
                default:        //裏返せない場合
                    ret = -1;
                    break;
            }
            return ret;
        }

        //駒配置設定
        public koma(int _p, KomaType _type, int _x, int _y) {
            p = _p;
            type = _type;
            x = _x;
            y = _y;
        }

        public koma(koma ko) {
            p = ko.p;
            type = ko.type;
            x = ko.x;
            y = ko.y;
        }

        //駒の移動チェック(0:移動OK, -1:移動NG(駒が存在しない,移動できない 等))
        public int chkMove(koma ko, koPos dstPos, BanInfo ban) {
            if ((dstPos.y < 0) || (dstPos.x < 0) || (dstPos.y >= TEIGI.SIZE_SUZI) || (dstPos.x >= TEIGI.SIZE_DAN)) return -1;
            //指定した移動位置が移動可能リストに存在するかチェック
            if (baninfo(ban).Contains(new koPos(dstPos.x, dstPos.y)) == true) {
                return 0;
            } else {
                return -1;
            }

        }

        int chkSenGo(int val) {
            if (p == TEIGI.TEBAN_SENTE) {
                return val;
            } else {
                return -val;
            }
        }

        public int[,,] checkKomaKiki(BanInfo ban) {
            int[,,] teList = ban.IdouList;

            //先手基準で計算(前:y-1/右:x-1)
            if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                //koSet(new koPos(0, -1), ban.BanKo, ref teList);
                if ((y + chkSenGo(-1) > -1) && (y + chkSenGo(-1) < 9)) {
                    teList[p, x, y + chkSenGo(-1)]++;
                }
            }
            if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                //koSet(new koPos(0, 1), ban.BanKo, ref teList);
                if ((y + chkSenGo(1) > -1) && (y + chkSenGo(1) < 9)) {
                    teList[p, x, y + chkSenGo(1)]++;
                }
            }
            if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                if ((y + chkSenGo(-1) > -1) && (y + chkSenGo(-1) < 9)) {
                    if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9)) {
                        //koSet(new koPos(-1, -1), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(-1), y + chkSenGo(-1)]++;
                    }
                    if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9)) {
                        //koSet(new koPos(1, -1), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(1), y + chkSenGo(-1)]++;
                    }
                }
            }
            if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9)) {
                    //koSet(new koPos(-1, 0), ban.BanKo, ref teList);
                    teList[p, x + chkSenGo(-1), y]++;
                }
                if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9)) {
                    //koSet(new koPos(1, 0), ban.BanKo, ref teList);
                    teList[p, x + chkSenGo(1), y]++;
                }
            }
            if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                if ((y + chkSenGo(1) > -1) && (y + chkSenGo(1) < 9)) {
                    if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9)) {
                        //koSet(new koPos(-1, 1), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(-1), y + chkSenGo(1)]++;
                    }
                    if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9)) {
                        //koSet(new koPos(1, 1), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(1), y + chkSenGo(1)]++;
                    }
                }
            }
            if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                if ((y + chkSenGo(-2) > -1) && (y + chkSenGo(-2) < 9)) {
                    if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9)) {
                        //koSet(new koPos(-1, -2), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(-1), y + chkSenGo(-2)]++;
                    }
                    if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9)) {
                        //koSet(new koPos(1, -2), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(1), y + chkSenGo(-2)]++;
                    }
                }
            }
            if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車(前に敵がいる時しか動かない)
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    teList[p, x, y + chkSenGo(-i)]++;
                    if (ban.BanKo[x, y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(0, -i), ban.BanKo, ref teList) > 0) break;
                }
            }

            if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    teList[p, x, y + chkSenGo(-i)]++;
                    if (ban.BanKo[x, y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(0, -i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if ((x + chkSenGo(-i) < 0) || (x + chkSenGo(-i) > 8)) break;
                    teList[p, x + chkSenGo(-i), y]++;
                    if (ban.BanKo[x + chkSenGo(-i), y] != null) break;
                    //if (koSet(new koPos(-i, 0), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if ((x + chkSenGo(i) < 0) || (x + chkSenGo(i) > 8)) break;
                    teList[p, x + chkSenGo(i), y]++;
                    if (ban.BanKo[x + chkSenGo(i), y] != null) break;
                    //if (koSet(new koPos(i, 0), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    if ((y + chkSenGo(i) < 0) || (y + chkSenGo(i) > 8)) break;
                    teList[p, x, y + chkSenGo(i)]++;
                    if (ban.BanKo[x, y + chkSenGo(i)] != null) break;
                    //if (koSet(new koPos(0, i), ban.BanKo, ref teList) > 0) break;
                }
            }

            if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    if ((x + chkSenGo(-i) < 0) || (x + chkSenGo(-i) > 8)) break;
                    teList[p, x + chkSenGo(-i), y + chkSenGo(-i)]++;
                    if (ban.BanKo[x + chkSenGo(-i), y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(-i, -i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if ((y + chkSenGo(i) < 0) || (y + chkSenGo(i) > 8)) break;
                    if ((x + chkSenGo(-i) < 0) || (x + chkSenGo(-i) > 8)) break;
                    teList[p, x + chkSenGo(-i), y + chkSenGo(i)]++;
                    if (ban.BanKo[x + chkSenGo(-i), y + chkSenGo(i)] != null) break;
                    //if (koSet(new koPos(-i, i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    if ((x + chkSenGo(i) < 0) || (x + chkSenGo(i) > 8)) break;
                    teList[p, x + chkSenGo(i), y + chkSenGo(-i)]++;
                    if (ban.BanKo[x + chkSenGo(i), y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(i, -i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if ((y + chkSenGo(i) < 0) || (y + chkSenGo(i) > 8)) break;
                    if ((x + chkSenGo(i) < 0) || (x + chkSenGo(i) > 8)) break;
                    teList[p, x + chkSenGo(i), y + chkSenGo(i)]++;
                    if (ban.BanKo[x + chkSenGo(i), y + chkSenGo(i)] != null) break;
                    //if (koSet(new koPos(i, i), ban.BanKo, ref teList) > 0) break;
                }
            }

            return teList;
        }

        // 指定した駒の盤上移動可能リスト
        // 戻り値
        public List<koPos> baninfo(BanInfo ban, bool cont = true) {
            //banSaki Mban = new banSaki();

            //次の手の位置リスト
            List<koPos> teList = new List<koPos>();

            /* 持ち駒の場合 */
            if (this.x == 9) {

                for (int i = 0; i < TEIGI.SIZE_DAN; i++) {　　//段(Y)
                    if ((this.type == KomaType.Fuhyou) || (this.type == KomaType.Kyousha))  //歩or香
                    {
                        if (((this.p == TEIGI.TEBAN_SENTE) && (i == 0)) || ((this.p == TEIGI.TEBAN_GOTE) && (i == TEIGI.SIZE_DAN - 1)))  //先手で1段目 or 後手で9段目
                        {
                            continue;
                        }

                    } else if (this.type == KomaType.Keima)  //桂
                      {
                        if (((this.p == TEIGI.TEBAN_SENTE) && (i < 2)) || ((this.p == TEIGI.TEBAN_GOTE) && (i >= TEIGI.SIZE_DAN - 2)))  //先手で<3段目 or 後手で>7段目
                        {
                            continue;
                        }
                    }

                    for (int j = 0; j < TEIGI.SIZE_SUZI; j++)　　//筋(X)
                    {
                        if (this.type == KomaType.Fuhyou) {
                            //TODO: 2歩チェック
                            if (ban.nifList[this.p, j] == 1) continue;
                        }

                        //指定した盤に駒が置いていない場合
                        if (ban.BanKo[j, i] == null) {
                            // 歩飛角以外の駒で、移動先に敵味方の駒がない場合、無駄なため置かない
                            if ((this.type != KomaType.Fuhyou) && (this.type != KomaType.Hisya) && (this.type != KomaType.Kakugyou) && (new koma(this.p, this.type, j, i).kikiCheck(ban) == 0)) {
                                continue;
                            }

                            teList.Add(new koPos(j, i, 0, this, false));
                        }
                    }
                }
                //盤上の場合
            } else {
                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    //teList.Add(new koPos(this.x, this.y, 0, this, false));
                    koSet(new koPos(this).rMV(0, -1), ban, ref teList, cont);
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    koSet(new koPos(this).rMV(0, 1), ban, ref teList, cont);
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    koSet(new koPos(this).rMV(-1, -1), ban, ref teList, cont);
                    koSet(new koPos(this).rMV(1, -1), ban, ref teList, cont);
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    koSet(new koPos(this).rMV(-1, 0), ban, ref teList, cont);
                    koSet(new koPos(this).rMV(1, 0), ban, ref teList, cont);
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    koSet(new koPos(this).rMV(-1, 1), ban, ref teList, cont);
                    koSet(new koPos(this).rMV(1, 1), ban, ref teList, cont);
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    koSet(new koPos(this).rMV(-1, -2), ban, ref teList, cont);
                    koSet(new koPos(this).rMV(1, -2), ban, ref teList, cont);
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        // 敵の移動先になっていない
                        if (ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE,this.x, this.y] == 0) {
                            if (koSet_Kyousya(new koPos(this).rMV(0, -i), ban, ref teList, cont) != 0) break;
                        } else {
                            if (koSet(new koPos(this).rMV(0, -i), ban, ref teList, cont) != 0) break;
                        }
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSet(new koPos(this).rMV(0, -i), ban, ref teList, cont) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(this).rMV(-i, 0), ban, ref teList, cont) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(this).rMV(i, 0), ban, ref teList, cont) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSet(new koPos(this).rMV(0, i), ban, ref teList, cont) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(this).rMV(-i, -i), ban, ref teList, cont) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(this).rMV(-i, i), ban, ref teList, cont) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(this).rMV(i, -i), ban, ref teList, cont) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(this).rMV(i, i), ban, ref teList, cont) != 0) break;
                    }
                }
            }

            return teList;
        }

        // 指定位置(tgt)へ次の手で移動可能な駒の移動リスト
        public List<koPos> baninfoPos(BanInfo ban,int tgt_x,int tgt_y) {
            //次の手の位置リスト
            List<koPos> teList = new List<koPos>();

            /* 持ち駒の場合 */
            if (this.x == 9) {
                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 0,  1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 0, -1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 1,  1), ban, ref teList);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1,  1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 1,  0), ban, ref teList);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1,  0), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 1, -1), ban, ref teList);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1, -1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 1,  2), ban, ref teList);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1,  2), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(0,  i), ban, ref teList) != 0) break;
                    }
                }
                
                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-i, 0), ban, ref teList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV( 0,-i), ban, ref teList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV( i, 0), ban, ref teList) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-i, -i), ban, ref teList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-i,  i), ban, ref teList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV( i, -i), ban, ref teList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV( i,  i), ban, ref teList) != 0) break;
                    }
                }

            /* 盤上の場合 */
            } else {
                // 一時移動リスト
                List<koPos> tmpList = new List<koPos>();
                
                // 移動可能な位置を一時移動リストに追加
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    koSetPos(new koPos(0, -1), ban, ref tmpList);
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    koSet(new koPos(0, 1), ban, ref tmpList);
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    koSet(new koPos(-1, -1), ban, ref tmpList);
                    koSet(new koPos(1, -1), ban, ref tmpList);
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    koSet(new koPos(-1, 0), ban, ref tmpList);
                    koSet(new koPos(1, 0), ban, ref tmpList);
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    koSet(new koPos(-1, 1), ban, ref tmpList);
                    koSet(new koPos(1, 1), ban, ref tmpList);
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    koSet(new koPos(-1, -2), ban, ref tmpList);
                    koSet(new koPos(1, -2), ban, ref tmpList);
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSet(new koPos(0, -i), ban, ref tmpList) != 0) break;
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSet(new koPos(0, -i), ban, ref tmpList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(-i, 0), ban, ref tmpList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(i, 0), ban, ref tmpList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSet(new koPos(0, i), ban, ref tmpList) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(-i, -i), ban, ref tmpList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(-i, i), ban, ref tmpList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(i, -i), ban, ref tmpList) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSet(new koPos(i, i), ban, ref tmpList) != 0) break;
                    }
                }
            }
            
            // 一時移動リストから次の手で指定位置へ移動可能かチェック

            return teList;
        }

        //posへ置けるかチェック
        int koSetPos(koPos pos, BanInfo ban, ref List<koPos> teList) {
            if (this.p == TEIGI.TEBAN_GOTE) {
                pos.x = -pos.x;
                pos.y = -pos.y;
            }
            
            pPos tmpPpos = new pPos();
            // 移動先(絶対位置) new削除による高速化
            tmpPpos.set(this.p, this.x + pos.x, this.y + pos.y);
            
            // 置き場所が範囲外
            if ((tmpPpos.y < 0) || (tmpPpos.x < 0) || (tmpPpos.y >= TEIGI.SIZE_SUZI) || (tmpPpos.x >= TEIGI.SIZE_DAN)) return -1;
            return 0;
        }

        //香車専用 移動チェック
        int koSet_Kyousya(koPos pos, BanInfo ban, ref List<koPos> teList, bool cont = false) {

            // 置き場所が範囲外
            if ((pos.y < 0) || (pos.x < 0) || (pos.y >= TEIGI.SIZE_SUZI) || (pos.x >= TEIGI.SIZE_DAN)) return -1;

            /* 駒が存在しない */
            if (ban.BanKo[pos.x, pos.y] == null) {
                return 0;
                
            /* 敵の駒がある */
            } else if (ban.BanKo[pos.x, pos.y].p != this.p) {
                // 移動元(this)or移動先(p)が自分基準位置で奥3段の場合は成り考慮
                if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {

                    if ((cont) ||(ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 50).setNari(true));
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 50 - KoScore[(int)this.type]).setNari(true)); // 劣勢の場所
                    }

                    // 香(2段まで)は不成もあり
                    if ((this.type == KomaType.Kyousha) && (pos.py > 1)) {
                        if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type]));
                        } else {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type])); // 劣勢の場所
                        }

                    }
                } else {
                    if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] /* + tekouho.GetKouho(this, pos.x, pos.y) */));
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type])); // 劣勢の場所
                    }
                }
                return 2;
            }

            /* 味方がいると置けない */
            return 1;
        }

        //指定位置に置くことができるかチェック(-1:範囲外で置けない/0:置ける/:1置けない(味方)/2:置ける(敵))
        int koSet(koPos pos, BanInfo ban, ref List<koPos> teList, bool cont = false) {

            // 置き場所が範囲外
            if ((pos.y < 0) || (pos.x < 0) || (pos.y >= TEIGI.SIZE_SUZI) || (pos.x >= TEIGI.SIZE_DAN)) return -1;

            /* 駒が存在しない */
            if (ban.BanKo[pos.x, pos.y] == null) {

                // 移動元(this)or移動先(p)が自分基準位置で奥3段の場合は成り考慮
                if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {
                    if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(250).setNari(true));
                    } else {
                        teList.Add(pos.setVal(50 - KoScore[(int)this.type]).setNari(true)); // 劣勢の場所
                    }

                    // 銀桂香(2段まで)は不成もあり
                    if ((this.type == KomaType.Ginsyou) || ((this.type == KomaType.Keima) && (pos.py > 1))) {

                        if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                            teList.Add(pos.setVal(0));
                        } else {
                            teList.Add(pos.setVal(-KoScore[(int)this.type])); // 劣勢の場所
                        }
                    }
                } else {
                    if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(0 /* + tekouho.GetKouho(this, pos.x, pos.y) */));
                    } else {
                        teList.Add(pos.setVal(-KoScore[(int)this.type]));
                    }
                }
                return 0;

            /* 敵の駒がある */
            } else if (ban.BanKo[pos.x, pos.y].p != this.p) {

                // 移動元(this)or移動先(p)が自分基準位置で奥3段の場合は成り考慮
                if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {

                    if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 150).setNari(true));
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 50 - KoScore[(int)this.type]).setNari(true)); // 劣勢の場所
                    }

                    // 銀桂香(2段まで)は不成もあり
                    if ((this.type == KomaType.Ginsyou) || (((this.type == KomaType.Keima) || (this.type == KomaType.Kyousha)) && (pos.py > 1))) {
                        if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type]));
                        } else {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type]));
                        }

                    }
                } else {
                    if ((cont) || (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] /* + tekouho.GetKouho(this, pos.x, pos.y) */));
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type])); // 劣勢の場所
                    }
                }
                return 2;
            }

            /* 味方がいると置けない */
            return 1;
        }

        //指定位置に敵味方がいるかチェック
        //(-1:範囲外で置けない/0:置ける/:1味方あり/2:置ける(敵))
        // 加算値(BanInfo.IdouList反映用)
        int koCheck(int x, int y, BanInfo ban, int kasan = 0) {

            if (this.p == TEIGI.TEBAN_GOTE) {
                x = -x;
                y = -y;
            }

            pPos tmpPpos = new pPos();
            // 移動先(絶対位置) new削除による高速化
            tmpPpos.set(this.p, this.x + x, this.y + y);

            // 置き場所が範囲外
            if ((tmpPpos.y < 0) || (tmpPpos.x < 0) || (tmpPpos.y >= TEIGI.SIZE_SUZI) || (tmpPpos.x >= TEIGI.SIZE_DAN)) return -1;

            ban.IdouList[p, tmpPpos.x, tmpPpos.y] += kasan; //BanInfo.IdouListに加算(駒有り無し関係ないため)

            if (ban.BanKo[tmpPpos.x, tmpPpos.y] == null) return 0;     // 駒なし
            if (ban.BanKo[tmpPpos.x, tmpPpos.y].p == this.p) return 1; // 味方駒あり


            return 1; // 敵駒あり
        }

        // 自分の駒が別の駒に効いているかチェック
        // 0:効いていない 1:敵味方に効いている
        int kikiCheck(BanInfo ban) {
            int ret = 0;

            //先手基準で計算(前:y-1/右:x-1)
            if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                if (koCheck(0, -1, ban) > 0) return 1;
            }
            if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                if (koCheck(0, 1, ban) > 0) return 1;
            }
            if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                if (koCheck(-1, -1, ban) > 0) return 1;
                if (koCheck(1, -1, ban) > 0) return 1;
            }
            if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                if (koCheck(-1, 0, ban) > 0) return 1;
                if (koCheck(1, 0, ban) > 0) return 1;
            }
            if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                if (koCheck(-1, 1, ban) > 0) return 1;
                if (koCheck(1, 1, ban) > 0) return 1;
            }
            if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                if (koCheck(-1, -2, ban) > 0) return 1;
                if (koCheck(1, -2, ban) > 0) return 1;
            }
            if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 飛車前・香車
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    ret = koCheck(0, -i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
            }

            if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車左右後
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    ret = koCheck(0, -i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    ret = koCheck(-i, 0, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    ret = koCheck(i, 0, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    ret = koCheck(0, i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
            }
            if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    ret = koCheck(-i, -i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    ret = koCheck(-i, i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    ret = koCheck(i, -i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    ret = koCheck(i, i, ban);
                    if (ret > 0) { return 1; } else if (ret < 0) break;
                }
            }
            return 0;
        }

        // 自分の駒が別の駒に効いているかチェック
        // 加算値(BanInfo.IdouList反映用)
        public void kikiRenew(BanInfo ban, int kasan) {

            if (x == 9) return; // 持ち駒は対象外

            //先手基準で計算(前:y-1/右:x-1)
            if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                koCheck(0, -1, ban, kasan);
            }
            if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                koCheck(0, 1, ban, kasan);
            }
            if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                koCheck(-1, -1, ban, kasan);
                koCheck(1, -1, ban, kasan);
            }
            if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                koCheck(-1, 0, ban, kasan);
                koCheck(1, 0, ban, kasan);
            }
            if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                koCheck(-1, 1, ban, kasan);
                koCheck(1, 1, ban, kasan);
            }
            if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                koCheck(-1, -2, ban, kasan);
                koCheck(1, -2, ban, kasan);
            }
            if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 飛車前・香車
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    if (koCheck(0, -i, ban, kasan) != 0) break;
                }
            }

            if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車左右後
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    if (koCheck(0, -i, ban, kasan) != 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if (koCheck(-i, 0, ban, kasan) != 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if (koCheck(i, 0, ban, kasan) != 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                    if (koCheck(0, i, ban, kasan) != 0) break;
                }
            }
            if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if (koCheck(-i, -i, ban, kasan) != 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if (koCheck(-i, i, ban, kasan) != 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if (koCheck(i, -i, ban, kasan) != 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                    if (koCheck(i, i, ban, kasan) != 0) break;
                }
            }
        }

    }
}
