using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG
{
    //駒情報
    class koma
    {
        //駒の動き情報
        uint[] KoMove =
        {
            0b0000000000000000,	 //なし：0
            0b0000000000000001,	 //歩兵：1
            0b0000000001000000,  //香車：7
            0b0000000000100000,  //桂馬：6
            0b0000000000010101,  //銀将：1 + 3 + 5
            0b0000000011000000,  //飛車：7 + 8
            0b0000000100000000,  //角行：9
            0b0000000000001111,  //金将：1 + 2 + 3 + 4
            0b0000000000011111,  //王将：1 + 2 + 3 + 4 + 5
            0b0000000000001111,	 //と金(成歩兵)：1 + 2 + 3 + 4
            0b0000000000001111,  //成香：1 + 2 + 3 + 4
            0b0000000000001111,  //成桂：1 + 2 + 3 + 4
            0b0000000000001111,  //成銀：1 + 2 + 3 + 4
            0b0000000011010100,  //竜王：3 + 5 + 7 + 8
            0b0000000100001011,  //竜馬：1 + 2 + 4 + 9
        };
        // 1:前 2:後 3:斜め前 4:横 5:斜め後ろ 6:桂馬 7:前ずっと 8:右左後ろずっと 9: 斜めずっと

        int[] KoScore =
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

        public int p; //所持プレイヤー (0:先手/1:後手)
        public KomaType type;
        public int x; //筋 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]
        public int y; //段 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]

        //駒が成れるか
        public bool chkNari()
        {
            bool chk = false;
            switch (type)
            {
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
        public int doKNari()
        {
            int ret = 0;
            switch (type)
            {
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
        public int doKModori()
        {
            int ret = 0;
            switch (type)
            {
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
        public koma(int _p, KomaType _type, int _x, int _y)
        {
            p = _p;
            type = _type;
            x = _x;
            y = _y;
        }

        public koma(koma ko)
        {
            p = ko.p;
            type = ko.type;
            x = ko.x;
            y = ko.y;
        }

        //駒の移動チェック(0:移動OK, -1:移動NG(駒が存在しない,移動できない 等))
        public int chkMove(koma ko, koPos dstPos, BanInfo ban)
        {
            if ((dstPos.y < 0) || (dstPos.x < 0) || (dstPos.y >= TEIGI.SIZE_SUZI) || (dstPos.x >= TEIGI.SIZE_DAN)) return -1;
            //指定した移動位置が移動可能リストに存在するかチェック
            if (baninfo(ban).Contains(new koPos(dstPos.x, dstPos.y)) == true)
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }

        int chkSenGo(int val){
            if (p == TEIGI.TEBAN_SENTE) {
                return val;
            } else {
                return -val;
            }
        }

        public int[,,] checkKomaKiki(BanInfo ban)
        {
            int[,,] teList = ban.IdouList;

            //先手基準で計算(前:y-1/右:x-1)
            if ((KoMove[(uint)this.type] & 1) > 0)
            { //1 : 前
                //koSet(new koPos(0, -1), ban.BanKo, ref teList);
                if ((y + chkSenGo(-1)>-1)&&(y + chkSenGo(-1)<9)) {
                    teList[p, x, y + chkSenGo(-1)]++;
                }
            }
            if ((KoMove[(uint)this.type] & 2) > 0)
            { //2 : 後
                //koSet(new koPos(0, 1), ban.BanKo, ref teList);
                if ((y + chkSenGo(1) > -1) && (y + chkSenGo(1) < 9)) {
                    teList[p, x, y + chkSenGo(1)]++;
                }
            }
            if ((KoMove[(uint)this.type] & 4) > 0)
            { //3 : 左右前
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
            if ((KoMove[(uint)this.type] & 8) > 0)
            { //4 : 左右横
                if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9))
                {
                    //koSet(new koPos(-1, 0), ban.BanKo, ref teList);
                    teList[p, x + chkSenGo(-1), y]++;
                }
                if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9))
                {
                    //koSet(new koPos(1, 0), ban.BanKo, ref teList);
                    teList[p, x + chkSenGo(1), y]++;
                }
            }
            if ((KoMove[(uint)this.type] & 16) > 0)
            { //5 : 左右後
                if ((y + chkSenGo(1) > -1) && (y + chkSenGo(1) < 9))
                {
                    if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9))
                    {
                        //koSet(new koPos(-1, 1), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(-1), y + chkSenGo(1)]++;
                    }
                    if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9))
                    {
                        //koSet(new koPos(1, 1), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(1), y + chkSenGo(1)]++;
                    }
                }
            }
            if ((KoMove[(uint)this.type] & 32) > 0)
            { //6 : 桂馬
                if ((y + chkSenGo(-2) > -1) && (y + chkSenGo(-2) < 9))
                {
                    if ((x + chkSenGo(-1) > -1) && (x + chkSenGo(-1) < 9))
                    {
                        //koSet(new koPos(-1, -2), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(-1), y + chkSenGo(-2)]++;
                    }
                    if ((x + chkSenGo(1) > -1) && (x + chkSenGo(1) < 9))
                    {
                        //koSet(new koPos(1, -2), ban.BanKo, ref teList);
                        teList[p, x + chkSenGo(1), y + chkSenGo(-2)]++;
                    }
                }
            }
            if ((KoMove[(uint)this.type] & 64) > 0)
            { //7 : 飛車前・香車
                for (int i = 1; i < TEIGI.SIZE_DAN; i++)
                {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    teList[p, x, y + chkSenGo(-i)]++;
                    if (ban.BanKo[x, y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(0, -i), ban.BanKo, ref teList) > 0) break;
                }
            }

            if ((KoMove[(uint)this.type] & 128) > 0)
            { //8 : 飛車左右後
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                {
                    if ((x + chkSenGo(-i) < 0) || (x + chkSenGo(-i) > 8)) break;
                    teList[p, x + chkSenGo(-i), y]++;
                    if (ban.BanKo[x + chkSenGo(-i), y] != null) break;
                    //if (koSet(new koPos(-i, 0), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                {
                    if ((x + chkSenGo(i) < 0) || (x + chkSenGo(i) > 8)) break;
                    teList[p, x + chkSenGo(i), y]++;
                    if (ban.BanKo[x + chkSenGo(i), y] != null) break;
                    //if (koSet(new koPos(i, 0), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_DAN; i++)
                {
                    if ((y + chkSenGo(i) < 0) || (y + chkSenGo(i) > 8)) break;
                    teList[p, x, y + chkSenGo(i)]++;
                    if (ban.BanKo[x, y + chkSenGo(i)] != null) break;
                    //if (koSet(new koPos(0, i), ban.BanKo, ref teList) > 0) break;
                }
            }

            if ((KoMove[(uint)this.type] & 256) > 0)
            { //9 : 角行
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    if ((x + chkSenGo(-i) < 0) || (x + chkSenGo(-i) > 8)) break;
                    teList[p, x + chkSenGo(-i), y + chkSenGo(-i)]++;
                    if (ban.BanKo[x + chkSenGo(-i), y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(-i, -i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                {
                    if ((y + chkSenGo(i) < 0) || (y + chkSenGo(i) > 8)) break;
                    if ((x + chkSenGo(-i) < 0) || (x + chkSenGo(-i) > 8)) break;
                    teList[p, x + chkSenGo(-i), y + chkSenGo(i)]++;
                    if (ban.BanKo[x + chkSenGo(-i), y + chkSenGo(i)] != null) break;
                    //if (koSet(new koPos(-i, i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                {
                    if ((y + chkSenGo(-i) < 0) || (y + chkSenGo(-i) > 8)) break;
                    if ((x + chkSenGo(i) < 0) || (x + chkSenGo(i) > 8)) break;
                    teList[p, x + chkSenGo(i), y + chkSenGo(-i)]++;
                    if (ban.BanKo[x + chkSenGo(i), y + chkSenGo(-i)] != null) break;
                    //if (koSet(new koPos(i, -i), ban.BanKo, ref teList) > 0) break;
                }
                for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                {
                    if ((y + chkSenGo(i) < 0) || (y + chkSenGo(i) > 8)) break;
                    if ((x + chkSenGo(i) < 0) || (x + chkSenGo(i) > 8)) break;
                    teList[p, x + chkSenGo(i), y + chkSenGo(i)]++;
                    if (ban.BanKo[x + chkSenGo(i), y + chkSenGo(i)] != null) break;
                    //if (koSet(new koPos(i, i), ban.BanKo, ref teList) > 0) break;
                }
            }

            return teList;
        }

            //駒の盤上移動情報
        public List<koPos> baninfo(BanInfo ban)
        {
            //banSaki Mban = new banSaki();

            //次の手の位置リスト
            List<koPos> teList = new List<koPos>();

            /* 持ち駒の場合 */
            if (this.x == 9)
            {

                for (int i = 0; i < TEIGI.SIZE_DAN; i++)
                {　　//段(Y)
                    if ((this.type == KomaType.Fuhyou) || (this.type == KomaType.Kyousha))  //歩or香
                    {
                        if (((this.p == TEIGI.TEBAN_SENTE) && (i == 0)) || ((this.p == TEIGI.TEBAN_GOTE) && (i == TEIGI.SIZE_DAN - 1)))  //先手で1段目 or 後手で9段目
                        {
                            continue;
                        }

                    }
                    else if (this.type == KomaType.Keima)  //桂
                    {
                        if (((this.p == TEIGI.TEBAN_SENTE) && (i < 2)) || ((this.p == TEIGI.TEBAN_GOTE) && (i >= TEIGI.SIZE_DAN - 2)))  //先手で<3段目 or 後手で>7段目
                        {
                            continue;
                        }
                    }

                    for (int j = 0; j < TEIGI.SIZE_SUZI; j++)　　//筋(X)
                    {
                        if (this.type == KomaType.Fuhyou)
                        {
                            //TODO: 2歩チェック
                            if (ban.nifList[this.p].Contains(j)) continue;
                        }

                        //指定した盤に駒が置いていない場合
                        if (ban.BanKo[j, i] == null)
                        {
                            teList.Add(new koPos(j, i, 0, this, false));
                        }
                    }
                }
                //盤上の場合
            }
            else
            {
                //先手基準で計算(前:y-1/右:x-1)
                if ((KoMove[(uint)this.type] & 1) > 0)
                { //1 : 前
                    //teList.Add(new koPos(this.x, this.y, 0, this, false));
                    koSet(new koPos(0, -1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 2) > 0)
                { //2 : 後
                    koSet(new koPos(0, 1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 4) > 0)
                { //3 : 左右前
                    koSet(new koPos(-1, -1), ban, ref teList);
                    koSet(new koPos(1, -1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 8) > 0)
                { //4 : 左右横
                    koSet(new koPos(-1, 0), ban, ref teList);
                    koSet(new koPos(1, 0), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 16) > 0)
                { //5 : 左右後
                    koSet(new koPos(-1, 1), ban, ref teList);
                    koSet(new koPos(1, 1), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 32) > 0)
                { //6 : 桂馬
                    koSet(new koPos(-1, -2), ban, ref teList);
                    koSet(new koPos(1, -2), ban, ref teList);
                }
                if ((KoMove[(uint)this.type] & 64) > 0)
                { //7 : 飛車前・香車
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++)
                    {
                        if (koSet(new koPos(0, -i), ban, ref teList) > 0) break;
                    }
                }

                if ((KoMove[(uint)this.type] & 128) > 0)
                { //8 : 飛車左右後
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                    {
                        if (koSet(new koPos(-i, 0), ban, ref teList) > 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                    {
                        if (koSet(new koPos(i, 0), ban, ref teList) > 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_DAN; i++)
                    {
                        if (koSet(new koPos(0, i), ban, ref teList) > 0) break;
                    }
                }
                if ((KoMove[(uint)this.type] & 256) > 0)
                { //9 : 角行
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                    {
                        if (koSet(new koPos(-i, -i), ban, ref teList) > 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                    {
                        if (koSet(new koPos(-i, i), ban, ref teList) > 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                    {
                        if (koSet(new koPos(i, -i), ban, ref teList) > 0) break;
                    }
                    for (int i = 1; i < TEIGI.SIZE_SUZI; i++)
                    {
                        if (koSet(new koPos(i, i), ban, ref teList) > 0) break;
                    }
                }
            }

            return teList;
        }

        //指定位置に置くことができるかチェック(0:置けない/1:置ける)
        int koSet(koPos pos, BanInfo ban, ref List<koPos> teList)
        {
            if (this.p == TEIGI.TEBAN_GOTE)
            {
                pos.x = -pos.x;
                pos.y = -pos.y;
            }

            koPos tPos = new koPos(this.x + pos.x, this.y + pos.y);

            //置き場所が範囲外
            if ((tPos.y < 0) || (tPos.x < 0) || (tPos.y >= TEIGI.SIZE_SUZI) || (tPos.x >= TEIGI.SIZE_DAN)) return 1;

            //駒が存在しない
            if (ban.BanKo[tPos.x, tPos.y] == null)
            {

                //成り
                if (((((y<3)||(tPos.y < 3)) && (this.p == TEIGI.TEBAN_SENTE)) || (((y>5)||(tPos.y > 5)) && (this.p == TEIGI.TEBAN_GOTE))) && (this.chkNari()))
                {
                    if (this.chkNari()) {
                        if (ban.IdouList[this.p, tPos.x, tPos.y] >= ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, tPos.x, tPos.y]) {
                            teList.Add(new koPos(tPos.x, tPos.y, 50, this, true));
                        } else {
                            teList.Add(new koPos(tPos.x, tPos.y, 50 - KoScore[(int)this.type], this, true)); // 劣勢の場所
                        }

                    }
                    if ((this.type == KomaType.Ginsyou) || (this.type == KomaType.Keima) || (this.type == KomaType.Kyousha))//銀桂香は不成もあり
                    {
                        
                        if (ban.IdouList[this.p, tPos.x, tPos.y] >= ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, tPos.x, tPos.y])
                        {
                            teList.Add(new koPos(tPos.x, tPos.y, 0, this, false));
                        }
                        else
                        {
                            teList.Add(new koPos(tPos.x, tPos.y, -KoScore[(int)this.type], this, false)); // 劣勢の場所
                        }
                    }
                } else {
                    if (ban.IdouList[this.p, tPos.x, tPos.y] >= ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, tPos.x, tPos.y])
                    {
                        teList.Add(new koPos(tPos.x, tPos.y, 0 + tekouho.GetKouho(this.p, this, new koPos(tPos.x, tPos.y)), this, false));
                    }
                    else
                    {
                        teList.Add(new koPos(tPos.x, tPos.y, -KoScore[(int)this.type], this, false)); // 劣勢の場所
                    }
                }

                return 0;
            }

            //敵の駒がある
            if (ban.BanKo[tPos.x, tPos.y].p != this.p)
            {

                //成り
                if (((((y < 3) || (tPos.y < 3)) && (this.p == TEIGI.TEBAN_SENTE)) || (((y > 5) || (tPos.y > 5)) && (this.p == TEIGI.TEBAN_GOTE))) && (this.chkNari()))
                {
                    
                    if (ban.IdouList[this.p, tPos.x, tPos.y] >= ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, tPos.x, tPos.y])
                    {
                        teList.Add(new koPos(tPos.x, tPos.y, KoScore[(int)ban.BanKo[tPos.x, tPos.y].type] + 50, this, true));
                    }
                    else
                    {
                        teList.Add(new koPos(tPos.x, tPos.y, KoScore[(int)ban.BanKo[tPos.x, tPos.y].type] + 50 - KoScore[(int)this.type], this, true)); // 劣勢の場所
                    }
                    if ((this.type == KomaType.Ginsyou) || (this.type == KomaType.Keima) || (this.type == KomaType.Keima))//銀桂香は不成もあり
                    {
                        if (ban.IdouList[this.p, tPos.x, tPos.y] >= ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, tPos.x, tPos.y])
                        {
                            teList.Add(new koPos(tPos.x, tPos.y, KoScore[(int)ban.BanKo[tPos.x, tPos.y].type], this, false));
                        }
                        else
                        {
                            teList.Add(new koPos(tPos.x, tPos.y, KoScore[(int)ban.BanKo[tPos.x, tPos.y].type] - KoScore[(int)this.type], this, false)); // 劣勢の場所
                        }
                        
                    }
                }
                else
                {
                    if (ban.IdouList[this.p, tPos.x, tPos.y] >= ban.IdouList[this.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, tPos.x, tPos.y])
                    {
                        teList.Add(new koPos(tPos.x, tPos.y, KoScore[(int)ban.BanKo[tPos.x, tPos.y].type] + tekouho.GetKouho(this.p, this, new koPos(tPos.x, tPos.y)) , this, false));
                    }
                    else
                    {
                        teList.Add(new koPos(tPos.x, tPos.y, KoScore[(int)ban.BanKo[tPos.x, tPos.y].type] - KoScore[(int)this.type], this, false)); // 劣勢の場所
                    }
                }

                return 1;
            }

            return 1;
        }

    }
}
