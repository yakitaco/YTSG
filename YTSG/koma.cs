using System.Collections.Generic;
using System.Linq;

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
            1800,  //竜王：1 + 2 + 3 + 4 + 5 + 7 + 8
            1400,  //竜馬：1 + 2 + 3 + 4 + 5 + 9
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

        // プレイヤーから見た位置(0,0 左下)
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

        public int kScore {
            get {
                return KoScore[(int)type];
            }
        }

        // 初期化関数
        static koma() {
            for (int i = 0; i < 15; i++) {
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
            List<koPos> retList = new List<koPos>();
            //指定した移動位置が移動可能リストに存在するかチェック
            this.listUpMoveable(ref retList, ban);
            if (retList.Contains(new koPos(dstPos.x, dstPos.y)) == true) {
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
        // moveableList 移動可能リスト(参照型)
        // chkType = 0:通常 / 1:[移動]評価値に取られた場合の減算あり / 2:[駒打]
        // 戻り値 指定条件で移動可能なリスト
        public void listUpMoveable(ref List<koPos> moveableList, BanInfo ban, int chkType = 0) {

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

                        int val = 0;

                        //指定した盤に駒が置いていない場合
                        if (ban.BanKo[j, i] == null) {
                            // 歩飛角以外の駒で、移動先に敵味方の駒がない場合、無駄なため置かない
                            if ((this.type != KomaType.Fuhyou) && (this.type != KomaType.Hisya) && (this.type != KomaType.Kakugyou) && (new koma(this.p, this.type, j, i).kikiCheck(ban) == 0)) {
                                continue;
                            }

                            if (((ban.IdouList[this.p, j, i] == 0) || (this.type == KomaType.Hisya) || (this.type == KomaType.Kakugyou)) && (ban.IdouList[this.ap, j, i] > 0)) {
                                if (this.type != KomaType.Fuhyou) {
                                    val -= 500;
                                } else if ((i > 0) && (ban.IdouList[this.p, j, i - 1] == 0) && (ban.IdouList[this.ap, j, i - 1] > 0)) {
                                    val -= 500;
                                } else if ((i < TEIGI.SIZE_DAN - 1) && (ban.IdouList[this.p, j, i + 1] == 0) && (ban.IdouList[this.ap, j, i + 1] > 0)) {
                                    val -= 500;
                                }

                            }

                            // 敵飛車前の歩は高評価
                            if ((this.type == KomaType.Fuhyou) && (ban.IdouList[this.p, j, i] > 0)) {
                                if (this.p == TEIGI.TEBAN_SENTE) {
                                    for (int k = 1; k < TEIGI.SIZE_DAN; k++) {
                                        if (i - k < 0)  break;
                                        if (ban.BanKo[j, i-k] == null) continue;
                                        if (((ban.BanKo[j, i - k].type == KomaType.Hisya) || (ban.BanKo[j, i - k].type == KomaType.Ryuuou))&&(ban.BanKo[j, i - k].p == this.ap)) val += 500;
                                        break;
                                    }
                                } else {
                                    for (int k = 1; k < TEIGI.SIZE_DAN; k++) {
                                        if (i + k > 8) break;
                                        if (ban.BanKo[j, i + k] == null) continue;
                                        if (((ban.BanKo[j, i + k].type == KomaType.Hisya) || (ban.BanKo[j, i + k].type == KomaType.Ryuuou)) && (ban.BanKo[j, i + k].p == this.ap)) val += 500;
                                        break;
                                    }
                                }
                            }

                            moveableList.Add(new koPos(j, i, val, this, false));
                        }
                    }
                }
                //盤上の場合
            } else {
                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    checkMoveable(new koPos(this).rMV(0, -1), ban, ref moveableList, chkType);
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    checkMoveable(new koPos(this).rMV(0, 1), ban, ref moveableList, chkType);
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    checkMoveable(new koPos(this).rMV(-1, -1), ban, ref moveableList, chkType);
                    checkMoveable(new koPos(this).rMV(1, -1), ban, ref moveableList, chkType);
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    checkMoveable(new koPos(this).rMV(-1, 0), ban, ref moveableList, chkType);
                    checkMoveable(new koPos(this).rMV(1, 0), ban, ref moveableList, chkType);
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    checkMoveable(new koPos(this).rMV(-1, 1), ban, ref moveableList, chkType);
                    checkMoveable(new koPos(this).rMV(1, 1), ban, ref moveableList, chkType);
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    checkMoveable(new koPos(this).rMV(-1, -2), ban, ref moveableList, chkType);
                    checkMoveable(new koPos(this).rMV(1, -2), ban, ref moveableList, chkType);
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        // 敵の移動先になっていない
                        if (ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, this.x, this.y] == 0) {
                            if (koSet_Kyousya(new koPos(this).rMV(0, -i), ban, ref moveableList, chkType) != 0) break;
                        } else {
                            if (checkMoveable(new koPos(this).rMV(0, -i), ban, ref moveableList, chkType) != 0) break;
                        }
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (checkMoveable(new koPos(this).rMV(0, -i), ban, ref moveableList, chkType) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (checkMoveable(new koPos(this).rMV(-i, 0), ban, ref moveableList, chkType) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (checkMoveable(new koPos(this).rMV(i, 0), ban, ref moveableList, chkType) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (checkMoveable(new koPos(this).rMV(0, i), ban, ref moveableList, chkType) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (checkMoveable(new koPos(this).rMV(-i, -i), ban, ref moveableList, chkType) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (checkMoveable(new koPos(this).rMV(-i, i), ban, ref moveableList, chkType) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (checkMoveable(new koPos(this).rMV(i, -i), ban, ref moveableList, chkType) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (checkMoveable(new koPos(this).rMV(i, i), ban, ref moveableList, chkType) != 0) break;
                    }
                }
            }
        }


        // 指定位置(tgt)へ移動可能な駒の移動リスト
        public List<koPos> baninfoPos(BanInfo ban, int tgt_x, int tgt_y) {
            //次の手の位置リスト
            List<koPos> teList = new List<koPos>();

            /* 持ち駒の場合 */
            if (this.x == 9) {
                if (this.type == KomaType.Fuhyou) {
                    //TODO: 2歩チェック
                    if (ban.nifList[this.p, tgt_x] == 1) return teList;
                }
                // 駒がない場合のみOK(koSetPos内でチェック)
                koSetPos(new koPos(this, tgt_x, tgt_y), ban, ref teList, 1);
            } else {
                /* 盤上の場合 */

                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    if (koSetPosP(new koPos(this).rMV(0, -1), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    if (koSetPosP(new koPos(this).rMV(0, 1), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    if (koSetPosP(new koPos(this).rMV(-1, -1), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                    if (koSetPosP(new koPos(this).rMV(1, -1), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    if (koSetPosP(new koPos(this).rMV(-1, 0), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                    if (koSetPosP(new koPos(this).rMV(1, 0), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    if (koSetPosP(new koPos(this).rMV(-1, 1), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                    if (koSetPosP(new koPos(this).rMV(1, 1), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    if (koSetPosP(new koPos(this).rMV(-1, -2), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                    if (koSetPosP(new koPos(this).rMV(1, -2), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPosP(new koPos(this).rMV(0, -i), ban, ref teList, tgt_x, tgt_y) != 0) return teList;
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPosP(new koPos(this).rMV(0, -i), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(0, -i), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPosP(new koPos(this).rMV(-i, 0), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(-i, 0), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPosP(new koPos(this).rMV(i, 0), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(i, 0), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPosP(new koPos(this).rMV(0, i), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(0, i), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPosP(new koPos(this).rMV(-i, -i), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(-i, -i), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPosP(new koPos(this).rMV(-i, i), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(-i, i), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPosP(new koPos(this).rMV(i, -i), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(i, -i), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPosP(new koPos(this).rMV(i, i), ban, ref teList, tgt_x, tgt_y) == 3) return teList;
                        if (koSetPosP(new koPos(this).rMV(i, i), ban, ref teList, tgt_x, tgt_y) != 0) break;
                    }
                }
            }

            return teList;
        }

        // 指定位置(tgt)へ次の手で移動可能な駒の移動リスト
        public List<koPos> baninfoPosNext(BanInfo ban, int tgt_x, int tgt_y) {
            //次の手の位置リスト
            List<koPos> teList = new List<koPos>();

            /* 持ち駒の場合 */
            if (this.x == 9) {
                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    if (this.type == KomaType.Fuhyou) {
                        //TODO: 2歩チェック
                        if (ban.nifList[this.p, tgt_x] == 1) return teList;
                    }
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(0, 1), ban, ref teList, 1);
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(0, -1), ban, ref teList, 1);
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(1, 1), ban, ref teList, 1);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1, 1), ban, ref teList, 1);
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(1, 0), ban, ref teList, 1);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1, 0), ban, ref teList, 1);
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(1, -1), ban, ref teList, 1);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1, -1), ban, ref teList, 1);
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(1, 2), ban, ref teList, 1);
                    koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-1, 2), ban, ref teList, 1);
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(0, i), ban, ref teList, 1) != 0) break;
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(0, i), ban, ref teList, 1) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-i, 0), ban, ref teList, 1) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(0, -i), ban, ref teList, 1) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(i, 0), ban, ref teList, 1) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-i, -i), ban, ref teList, 1) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(-i, i), ban, ref teList, 1) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(i, -i), ban, ref teList, 1) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this, tgt_x, tgt_y).rMV(i, i), ban, ref teList, 1) != 0) break;
                    }
                }

                /* 盤上の場合 */
            } else {
                // 一時移動リスト
                List<koPos> tmpList = new List<koPos>();

                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0) { //1 : 前
                    koSetPos(new koPos(this).rMV(0, -1), ban, ref tmpList, 0);
                }
                if ((KoMove[(uint)this.type] & 2) > 0) { //2 : 後
                    koSetPos(new koPos(this).rMV(0, 1), ban, ref tmpList, 0);
                }
                if ((KoMove[(uint)this.type] & 4) > 0) { //3 : 左右前
                    koSetPos(new koPos(this).rMV(-1, -1), ban, ref tmpList, 0);
                    koSetPos(new koPos(this).rMV(1, -1), ban, ref tmpList, 0);
                }
                if ((KoMove[(uint)this.type] & 8) > 0) { //4 : 左右横
                    koSetPos(new koPos(this).rMV(-1, 0), ban, ref tmpList, 0);
                    koSetPos(new koPos(this).rMV(1, 0), ban, ref tmpList, 0);
                }
                if ((KoMove[(uint)this.type] & 16) > 0) { //5 : 左右後
                    koSetPos(new koPos(this).rMV(-1, 1), ban, ref tmpList, 0);
                    koSetPos(new koPos(this).rMV(1, 1), ban, ref tmpList, 0);
                }
                if ((KoMove[(uint)this.type] & 32) > 0) { //6 : 桂馬
                    koSetPos(new koPos(this).rMV(-1, -2), ban, ref tmpList, 0);
                    koSetPos(new koPos(this).rMV(1, -2), ban, ref tmpList, 0);
                }
                if ((KoMove[(uint)this.type] & 64) > 0) { //7 : 香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this).rMV(0, -i), ban, ref tmpList, 0) != 0) break;
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0) { //8 : 飛車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this).rMV(0, -i), ban, ref tmpList, 0) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this).rMV(-i, 0), ban, ref tmpList, 0) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this).rMV(i, 0), ban, ref tmpList, 0) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                        if (koSetPos(new koPos(this).rMV(0, i), ban, ref tmpList, 0) != 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0) { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this).rMV(-i, -i), ban, ref tmpList, 0) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this).rMV(-i, i), ban, ref tmpList, 0) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this).rMV(i, -i), ban, ref tmpList, 0) != 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                        if (koSetPos(new koPos(this).rMV(i, i), ban, ref tmpList, 0) != 0) break;
                    }
                }

                // 一時移動リストから次の手で指定位置へ移動可能かチェック
                foreach (koPos te in tmpList) {

                    koma tmpKo = new koma(te.ko);
                    if (te.nari == true) tmpKo.doKNari();

                    //先手基準で計算(前:y-1/右:x-1)
                    if ((KoMove[(uint)tmpKo.type] & 1) > 0) { //1 : 前
                        if (koSetPosPos(te, 0, -1, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                    }
                    if ((KoMove[(uint)tmpKo.type] & 2) > 0) { //2 : 後
                        if (koSetPosPos(te, 0, 1, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                    }
                    if ((KoMove[(uint)tmpKo.type] & 4) > 0) { //3 : 左右前
                        if (koSetPosPos(te, -1, -1, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                        if (koSetPosPos(te, 1, -1, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                    }
                    if ((KoMove[(uint)tmpKo.type] & 8) > 0) { //4 : 左右横
                        if (koSetPosPos(te, -1, 0, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                        if (koSetPosPos(te, 1, 0, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                    }
                    if ((KoMove[(uint)tmpKo.type] & 16) > 0) { //5 : 左右後
                        if (koSetPosPos(te, -1, 1, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                        if (koSetPosPos(te, 1, 1, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                    }
                    if ((KoMove[(uint)tmpKo.type] & 32) > 0) { //6 : 桂馬
                        if (koSetPosPos(te, -1, -2, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                        if (koSetPosPos(te, 1, -2, ban, ref teList, tgt_x, tgt_y) == 3) continue;
                    }
                    if ((KoMove[(uint)tmpKo.type] & 64) > 0) { //7 : 香車
                        for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                            if (koSetPosPos(te, 0, -i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                    }

                    if ((KoMove[(uint)tmpKo.type] & 128) > 0) { //8 : 飛車
                        for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                            if (koSetPosPos(te, 0, -i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                        for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                            if (koSetPosPos(te, -i, 0, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                        for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                            if (koSetPosPos(te, i, 0, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                        for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                            if (koSetPosPos(te, 0, i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                    }
                    if ((KoMove[(uint)tmpKo.type] & 256) > 0) { //9 : 角行
                        for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                            if (koSetPosPos(te, -i, -i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                        for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                            if (koSetPosPos(te, -i, i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                        for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                            if (koSetPosPos(te, i, -i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                        for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                            if (koSetPosPos(te, i, i, ban, ref teList, tgt_x, tgt_y) != 0) break;
                        }
                    }
                }

            }

            return teList;
        }

        //posがtgtかチェック
        int koSetPosP(koPos pos, BanInfo ban, ref List<koPos> teList, int tgt_x, int tgt_y) {
            List<koPos> a = new List<koPos>();
            int ret = koSetPos(pos, ban, ref a, 0);
            if ((pos.x == tgt_x) && (pos.y == tgt_y)) {
                teList.AddRange(a);
                ret = 3;
            }
            return ret;
        }

        //posが次の手でtgtかチェック
        int koSetPosPos(koPos pos, int mv_x, int mv_y, BanInfo ban, ref List<koPos> teList, int tgt_x, int tgt_y) {
            List<koPos> a = new List<koPos>();
            koPos poss = pos.Copy().rMV(mv_x, mv_y);
            int ret = koSetPos(poss, ban, ref a, 0);
            if ((poss.x == tgt_x) && (poss.y == tgt_y)) {
                teList.Add(pos);
                ret = 3;
            }
            return ret;
        }

        //posへ置けるかチェック(mType=0:移動/1:置き)
        int koSetPos(koPos pos, BanInfo ban, ref List<koPos> teList, int type) {
            // 置き場所が範囲外
            if ((pos.y < 0) || (pos.x < 0) || (pos.y >= TEIGI.SIZE_SUZI) || (pos.x >= TEIGI.SIZE_DAN)) return -1;

            /* 駒が存在しない */
            if (ban.BanKo[pos.x, pos.y] == null) {
                // 駒成り
                if ((type == 0) && ((py < 3) || (pos.py < 3)) && (this.chkNari())) {

                    // 銀桂香(2段まで)は不成もあり(そのまま入れると次で成になってしまうためコピー)
                    if ((this.type == KomaType.Ginsyou) || (((this.type == KomaType.Keima) || (this.type == KomaType.Kyousha)) && (pos.py > 1))) {
                        teList.Add(pos.Copy());
                    }

                    teList.Add(pos.setNari(true));

                } else {
                    teList.Add(pos);
                }
                return 0;
                /* 敵の駒がある(移動のみ置ける) */
            } else if (ban.BanKo[pos.x, pos.y].p != this.p) {
                if (type == 0) {
                    // 駒成り
                    if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {
                        teList.Add(pos.setNari(true));
                        // 銀桂香(2段まで)は不成もあり
                        if ((this.type == KomaType.Ginsyou) || (((this.type == KomaType.Keima) || (this.type == KomaType.Kyousha)) && (pos.py > 1))) {
                            teList.Add(pos);
                        }

                    } else {
                        teList.Add(pos);
                    }
                }
                return 2;
            }
            /* 味方がいると置けない */
            return 1;
        }

        //香車専用 移動チェック
        int koSet_Kyousya(koPos pos, BanInfo ban, ref List<koPos> teList, int chkType) {

            // 置き場所が範囲外
            if ((pos.y < 0) || (pos.x < 0) || (pos.y >= TEIGI.SIZE_SUZI) || (pos.x >= TEIGI.SIZE_DAN)) return -1;

            /* 駒が存在しない */
            if (ban.BanKo[pos.x, pos.y] == null) {
                return 0;

                /* 敵の駒がある */
            } else if (ban.BanKo[pos.x, pos.y].p != this.p) {
                // 移動元(this)or移動先(p)が自分基準位置で奥3段の場合は成り考慮
                if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {

                    // 香(2段まで)は不成もあり
                    if ((this.type == KomaType.Kyousha) && (pos.py > 1)) {
                        if ((chkType == 1) && (ban.IdouList[this.p, pos.x, pos.y] <= ban.IdouList[this.ap, pos.x, pos.y])) {
                            teList.Add(pos.Copy().setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type])); // 劣勢の場所
                        } else {
                            teList.Add(pos.Copy().setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type]));
                        }

                    }

                    if ((chkType == 1) && (ban.IdouList[this.p, pos.x, pos.y] <= ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 50 - KoScore[(int)this.type]).setNari(true)); // 劣勢の場所
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 50).setNari(true));
                    }

                } else {
                    if ((chkType == 1) && (ban.IdouList[this.p, pos.x, pos.y] <= ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type])); // 劣勢の場所
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] /* + tekouho.GetKouho(this, pos.x, pos.y) */));
                    }
                }
                return 2;
            }

            /* 味方がいると置けない */
            return 1;
        }

        //指定位置に置くことができるかチェック(-1:範囲外で置けない/0:置ける/:1置けない(味方)/2:置ける(敵))
        int checkMoveable(koPos pos, BanInfo ban, ref List<koPos> teList, int chkType) {

            // 置き場所が範囲外
            if ((pos.y < 0) || (pos.x < 0) || (pos.y >= TEIGI.SIZE_SUZI) || (pos.x >= TEIGI.SIZE_DAN)) return -1;

            /* 駒が存在しない */
            if (ban.BanKo[pos.x, pos.y] == null) {

                // 移動元(this)or移動先(p)が自分基準位置で奥3段の場合は成り考慮
                if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {


                    // 銀桂香(2段まで)は不成もあり(そのまま入れると次で成になってしまうためコピー)
                    if ((this.type == KomaType.Ginsyou) || ((this.type == KomaType.Keima) && (pos.py > 1))) {

                        if ((chkType == 1) && (ban.IdouList[this.p, pos.x, pos.y] <= ban.IdouList[this.ap, pos.x, pos.y])) {
                            teList.Add(pos.Copy().setVal(-KoScore[(int)this.type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p))); // 劣勢の場所
                        } else {
                            teList.Add(pos.Copy().setVal(0 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)));
                        }
                    }

                    if ((chkType == 1) && (ban.IdouList[this.p, pos.x, pos.y] <= ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(50 - KoScore[(int)this.type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)).setNari(true)); // 劣勢の場所
                    } else {
                        teList.Add(pos.setVal(250 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)).setNari(true));
                    }

                } else {
                    if ((chkType == 1) && (ban.IdouList[this.p, pos.x, pos.y] <= ban.IdouList[this.ap, pos.x, pos.y])) {
                        teList.Add(pos.setVal(-KoScore[(int)this.type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p))); // 劣勢の場所
                    } else {
                        teList.Add(pos.setVal(0 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p) /* + tekouho.GetKouho(this, pos.x, pos.y) */));
                    }
                }
                return 0;

                /* 敵の駒がある */
            } else if (ban.BanKo[pos.x, pos.y].p != this.p) {

                // 移動元(this)or移動先(p)が自分基準位置で奥3段の場合は成り考慮
                if (((py < 3) || (pos.py < 3)) && (this.chkNari())) {

                    // 銀桂香(2段まで)は不成もあり(そのまま入れると次で成になってしまうためコピー)
                    if ((this.type == KomaType.Ginsyou) || (((this.type == KomaType.Keima) || (this.type == KomaType.Kyousha)) && (pos.py > 1))) {
                        if (chkType == 1) {
                            if (ban.IdouList[this.p, pos.x, pos.y] < ban.IdouList[this.ap, pos.x, pos.y]) {
                                teList.Add(pos.Copy().setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] / 2 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)));
                            } else {
                                teList.Add(pos.Copy().setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)));
                            }
                        } else {
                            teList.Add(pos.Copy().setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)));
                        }

                    }

                    if (chkType == 1) {
                        if (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y]) {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] / 2 + 150 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)).setNari(true));
                        } else {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 50 - KoScore[(int)this.type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)).setNari(true)); // 劣勢の場所
                        }
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + 150 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)).setNari(true));
                    }

                } else {
                    if (chkType == 1) {
                        if (ban.IdouList[this.p, pos.x, pos.y] > ban.IdouList[this.ap, pos.x, pos.y]) {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] / 2 + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)));
                        } else {
                            teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] - KoScore[(int)this.type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p))); // 劣勢の場所
                        }
                    } else {
                        teList.Add(pos.setVal(KoScore[(int)ban.BanKo[pos.x, pos.y].type] + mVal.get(this.type, pos.x, pos.y, this.x, this.y, this.p)));
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

        public List<koPos> discoverCheck_hisya(BanInfo ban) {
            List<koPos> retList = new List<koPos>();
            koma tmp_koma = null;
            if (ban.KingKo[this.ap].x == this.x) {
                if (ban.KingKo[this.ap].y > this.y) {
                    for (int tmp_y = this.y + 1; tmp_y < ban.KingKo[this.ap].y; tmp_y++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[this.x, tmp_y] != null) {
                            if ((ban.BanKo[this.x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[this.x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }
                } else {
                    for (int tmp_y = ban.KingKo[this.ap].y + 1; tmp_y < this.y; tmp_y++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[this.x, tmp_y] != null) {
                            if ((ban.BanKo[this.x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[this.x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }

                }
                if (tmp_koma != null) {
                    tmp_koma.listUpMoveable(ref retList, ban);
                    retList = retList.Where(o => o.x != this.x).ToList(); //さえぎる場所は削除
                }
            } else if (ban.KingKo[this.ap].y == this.y) {
                if (ban.KingKo[this.ap].x > this.x) {
                    for (int tmp_x = this.x + 1; tmp_x < ban.KingKo[this.ap].x; tmp_x++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[tmp_x, this.y] != null) {
                            if ((ban.BanKo[tmp_x, this.y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[tmp_x, this.y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }
                } else {
                    for (int tmp_x = ban.KingKo[this.ap].x + 1; tmp_x < this.x; tmp_x++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[tmp_x, this.y] != null) {
                            if ((ban.BanKo[tmp_x, this.y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[tmp_x, this.y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }

                }
                if (tmp_koma != null) {
                    tmp_koma.listUpMoveable(ref retList, ban);
                    retList = retList.Where(o => o.y != this.y).ToList(); //さえぎる場所は削除
                }
            }

            return retList;
        }

        public List<koPos> discoverCheck_Kakugyou(BanInfo ban) {
            List<koPos> retList = new List<koPos>();
            int tmp_x, tmp_y;
            koma tmp_koma = null;
            if (ban.KingKo[this.ap].x - ban.KingKo[this.ap].y == this.x - this.y) {
                if (ban.KingKo[this.ap].y > this.y) {
                    for (tmp_x = this.x + 1, tmp_y = this.y + 1; tmp_x < ban.KingKo[this.ap].x; tmp_x++, tmp_y++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[tmp_x, tmp_y] != null) {
                            if ((ban.BanKo[tmp_x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[tmp_x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }
                } else {
                    for (tmp_x = ban.KingKo[this.ap].x + 1, tmp_y = ban.KingKo[this.ap].y + 1; tmp_y < this.y; tmp_x++, tmp_y++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[tmp_x, tmp_y] != null) {
                            if ((ban.BanKo[tmp_x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[tmp_x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }

                }
                if (tmp_koma != null) {
                    tmp_koma.listUpMoveable(ref retList, ban);
                    retList = retList.Where(o => o.x - o.y != this.x - this.y).ToList(); //さえぎる場所は削除
                }
            } else if (ban.KingKo[this.ap].x + ban.KingKo[this.ap].y == this.x + this.y) {
                if (ban.KingKo[this.ap].x > this.x) {
                    for (tmp_x = this.x + 1, tmp_y = this.y - 1; tmp_x < ban.KingKo[this.ap].x; tmp_x++, tmp_y--) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[tmp_x, tmp_y] != null) {
                            if ((ban.BanKo[tmp_x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[tmp_x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }
                } else {
                    for (tmp_x = ban.KingKo[this.ap].x + 1, tmp_y = ban.KingKo[this.ap].y - 1; tmp_x < this.x; tmp_x++, tmp_y--) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[tmp_x, tmp_y] != null) {
                            if ((ban.BanKo[tmp_x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[tmp_x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }

                }
                if (tmp_koma != null) {
                    tmp_koma.listUpMoveable(ref retList, ban);
                    retList = retList.Where(o => o.x + o.y != this.x + this.y).ToList(); //さえぎる場所は削除
                }
            }

            return retList;
        }

        public List<koPos> discoverCheck_Kyousya(BanInfo ban) {
            List<koPos> retList = new List<koPos>();
            koma tmp_koma = null;
            if (ban.KingKo[this.ap].x == this.x) {
                if ((ban.KingKo[this.ap].y > this.y) && (this.p == TEIGI.TEBAN_GOTE)) {
                    for (int tmp_y = this.y + 1; tmp_y < ban.KingKo[this.ap].y; tmp_y++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[this.x, tmp_y] != null) {
                            if ((ban.BanKo[this.x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[this.x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }
                } else if ((ban.KingKo[this.ap].y < this.y) && (this.p == TEIGI.TEBAN_SENTE)) {
                    for (int tmp_y = ban.KingKo[this.ap].y + 1; tmp_y < this.y; tmp_y++) {
                        // 味方が一人だけいる場合のみ
                        if (ban.BanKo[this.x, tmp_y] != null) {
                            if ((ban.BanKo[this.x, tmp_y].p == this.p) && (tmp_koma == null)) {
                                tmp_koma = ban.BanKo[this.x, tmp_y];
                            } else {
                                tmp_koma = null;
                                break;
                            }
                        }
                    }

                }
                if (tmp_koma != null) {
                    tmp_koma.listUpMoveable(ref retList, ban);
                    retList = retList.Where(o => o.x != this.x).ToList(); //さえぎる場所は削除
                }
            }

            return retList;
        }

        //この駒に飛角香が効いている場所をリスト作成
        public List<koPos> kikiList(BanInfo ban) {
            List<koPos> retList = new List<koPos>();
            List<koPos> tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                if (kikiListPos(new koPos(this).rMV(0, -i), ban, retList, tmpList, 1) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                if (kikiListPos(new koPos(this).rMV(-i, 0), ban, retList, tmpList, 0) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                if (kikiListPos(new koPos(this).rMV(i, 0), ban, retList, tmpList, 0) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_DAN; i++) {
                if (kikiListPos(new koPos(this).rMV(0, i), ban, retList, tmpList, 0) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                if (kikiListPos(new koPos(this).rMV(-i, -i), ban, retList, tmpList, 2) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                if (kikiListPos(new koPos(this).rMV(-i, i), ban, retList, tmpList, 2) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                if (kikiListPos(new koPos(this).rMV(i, -i), ban, retList, tmpList, 2) != 0) break;
            }
            tmpList = new List<koPos>();    // 一時移動リスト
            for (int i = 1; i < TEIGI.SIZE_SUZI; i++) {
                if (kikiListPos(new koPos(this).rMV(i, i), ban, retList, tmpList, 2) != 0) break;
            }

            return retList;
        }

        // type = 0 飛車 1=飛車香車 2= 角
        int kikiListPos(koPos pos, BanInfo ban, List<koPos> retList, List<koPos> tmpList, int type) {

            // 置き場所が範囲外
            if ((pos.y < 0) || (pos.x < 0) || (pos.y >= TEIGI.SIZE_SUZI) || (pos.x >= TEIGI.SIZE_DAN)) return -1;

            if (ban.BanKo[pos.x, pos.y] == null) {
                if (ban.IdouList[this.p, pos.x, pos.y] > 0) {
                    tmpList.Add(pos);
                }
                // 効き対象の駒である
            } else if (ban.BanKo[pos.x, pos.y].p != this.p) {
                if ((type == 0) && ((ban.BanKo[pos.x, pos.y].type == KomaType.Hisya) || (ban.BanKo[pos.x, pos.y].type == KomaType.Ryuuou))) {
                    retList.AddRange(tmpList);
                } else if ((type == 1) && ((ban.BanKo[pos.x, pos.y].type == KomaType.Hisya) || (ban.BanKo[pos.x, pos.y].type == KomaType.Ryuuou) || (ban.BanKo[pos.x, pos.y].type == KomaType.Kyousha))) {
                    retList.AddRange(tmpList);
                } else if ((type == 2) && ((ban.BanKo[pos.x, pos.y].type == KomaType.Kakugyou) || (ban.BanKo[pos.x, pos.y].type == KomaType.Ryuuma))) {
                    retList.AddRange(tmpList);
                }
                return 1;
                // その他の駒は対象外
            } else {
                return 1;
            }


            return 0;
        }

    }


}
