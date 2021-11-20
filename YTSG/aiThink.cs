using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YTSG {

    // AI思考部分メイン
    class aiThink {

        // thread同時数
        static int workMin;
        static int ioMin;
        public bool stopFlg = false;

        static aiThink() {
            // thread同時数取得
            ThreadPool.GetMinThreads(out workMin, out ioMin);
        }

        System.Random r = new System.Random();
        public int maxDepth; // 最大読み深さ
        public int maxMateDepth; // [詰将棋]最大読み深さ

        //ランダムに一手動かす
        //public void RandomeMove(int teban, ref BanInfo ban)
        //{
        //
        //    //Form1 form1 = new Form1();
        //    while (true){
        //
        //        int rnd;
        //        List<koPos> teList;
        //
        //        ban.renewNifList(teban);  //二歩リスト更新
        //
        //        //持ち駒を先に選択
        //        if (ban.MochiKo[teban].Count > 0)
        //        {
        //            rnd = r.Next(ban.MochiKo[teban].Count);  //動かす駒を選択
        //            teList = ban.MochiKo[teban][rnd].listUpMoveable(ban);  //動かす駒の移動可能位置一覧
        //            if (teList.Count > 0)
        //            {
        //                int rnd2 = r.Next(teList.Count);  //動かす先を選択
        //                //Console.WriteLine("bestmove " + usio.pos2usi(ban.MochiKo[teban][rnd], teList[rnd2], false));  //標準出力
        //                Form1.Form1Instance.addMsg("[SEND]MOVE:" + ban.MochiKo[teban][rnd].type + ":(" + (ban.MochiKo[teban][rnd].x + 1) + "," + (ban.MochiKo[teban][rnd].y + 1) + ")->(" + (teList[rnd2].x + 1) + "," + (teList[rnd2].y + 1) + ")");
        //                ban.moveKoma(ban.MochiKo[teban][rnd], teList[rnd2], false, false);  //動かす
        //                break;
        //            }
        //        }
        //
        //        rnd = r.Next(ban.OkiKo[teban].Count);  //動かす駒を選択
        //        teList = ban.OkiKo[teban][rnd].listUpMoveable(ban);  //動かす駒の移動可能位置一覧
        //        if (teList.Count > 0)
        //        {
        //            int rnd2 = r.Next(teList.Count);  //動かす先を選択
        //            bool nari = false;
        //            if (((teList[rnd2].y < 3) && (teban == TEIGI.TEBAN_SENTE)) || ((teList[rnd2].y > 5) && (teban == TEIGI.TEBAN_GOTE))) {
        //                nari = ban.OkiKo[teban][rnd].chkNari();
        //            }
        //            //Console.WriteLine("bestmove " + usio.pos2usi(ban.OkiKo[teban][rnd], teList[rnd2], nari));  //標準出力
        //            Form1.Form1Instance.addMsg("[SEND]MOVE:" + ban.OkiKo[teban][rnd].type +":("+ (ban.OkiKo[teban][rnd].x+1) + ","+ (ban.OkiKo[teban][rnd].y+1) + ")->("+ (teList[rnd2].x+1) +","+ (teList[rnd2].y+1) + ")" + (nari==true? "<NARI>" : "" ) + "\n");
        //            ban.moveKoma(ban.OkiKo[teban][rnd], teList[rnd2], nari, false);  //動かす
        //            break;
        //        }
        //    }
        //}

        //思考して一手動かす
        public List<koPos> thinkMove(int teban, BanInfo ban, int depth, int mateDepth, int deepThinkNum, int deepThinkDepth) {
            maxDepth = depth;
            List<koPos> teAllList = new List<koPos>();
            List<koPos> retList = new List<koPos>();
            int maxScore = -9999999;

            /* 詰み */
            if (mateDepth > 0) {
                retList = thinkMateMove(teban, ban, mateDepth);
                if (retList?.Count > 0) return retList;
            }

            int teCnt = 0; //手の進捗
            Object lockObj = new Object();

            //ban.renewNifList(teban);  //二歩リスト更新

            //指せる手を全てリスト追加
            foreach (koma km in ban.OkiKo[teban]) {
                List<koPos> poslist = new List<koPos>();
                km.listUpMoveable(ref poslist, ban);

                foreach (koPos pos in poslist) {
                    //if (ban.BanKo[pos.x, pos.y] == null) pos.val += tekouho.GetKouho(pos);
                    for (int i = Program.kifu.Count - 1; i>=0 && i > Program.kifu.Count - 6; i -= 2) {
                        if ((pos.x == Program.kifu[i].x) && (pos.y == Program.kifu[i].y) && (pos.ko.type == Program.kifu[i].ko.type)){
                            pos.val -= 500;
                        }
                    }

                    pos.val += mVal.get(pos.ko.type, pos.x, pos.y, pos.ko.x, pos.ko.y, teban);
                    //int val = mVal.get(pos.ko.type, pos.x, pos.y, pos.ko.x, pos.ko.y, teban);
                    //Form1.Form1Instance.addMsg("[" + (pos.ko.x + 1) + "," + (pos.ko.y + 1) + ")" + pos.ko.type + " ->" + (pos.x + 1) + "," + (pos.y + 1) + "val=" + val);

                }
                teAllList.AddRange(poslist);
            }

            for (int i = 0; i < 7; i++) {
                if (ban.MochiKo[teban, i]?.Count > 0) {
                    List<koPos> poslist = new List<koPos>();
                    ban.MochiKo[teban, i][0].listUpMoveable(ref poslist, ban);
                    foreach (koPos pos in poslist) {
                        if (ban.IdouList[teban, pos.x, pos.y] == 0) {
                            pos.val -= 100;  // 味方連携無し
                        } else {
                            pos.val -= 20;  // 味方連携有り
                        }
                        for (int ii = Program.kifu.Count - 1; ii >= 0 && ii > Program.kifu.Count - 6; ii -= 2) {
                            if ((pos.x == Program.kifu[ii].x) && (pos.y == Program.kifu[ii].y) && (pos.ko.type == Program.kifu[ii].ko.type)) {
                                pos.val -= 500;
                            }
                        }
                    }
                    teAllList.AddRange(poslist);
                }
            }

            //リストをランダムに並べ替える
            teAllList = teAllList.OrderBy(a => Guid.NewGuid()).ToList();

            // 降順にソート
            teAllList.Sort((a, b) => b.val - a.val);

            //thread同時数
            Form1.Form1Instance.addMsg("MinThreads work= " + workMin + ", i/o= " + ioMin + ", teAllList=" + teAllList.Count);

            if ((teAllList.Count > 150)&&(depth > 4)) {
                maxDepth = 4;
                deepThinkNum = 5;
                deepThinkDepth = 4;
            }

            // 並行処理
            Parallel.For(0, workMin, id => {
                while (true) {
                    BanInfo ban_local = new BanInfo(ban);
                    bool uchifuzume = false;  // 打ち歩詰めチェック

                    koma ko_local;
                    //int score = -99999;
                    int cnt_local;
                    List<koPos> nexTe;

                    lock (lockObj) {
                        if (stopFlg) break;
                        if (teAllList.Count <= teCnt) break;
                        cnt_local = teCnt;
                        teCnt++;
                    }

                    if (teAllList[cnt_local].ko.x == 9) {
                        ko_local = ban_local.MochiKo[teban, (int)teAllList[cnt_local].ko.type - 1][0];
                        if (teAllList[cnt_local].ko.type == KomaType.Fuhyou) uchifuzume = true;
                    } else {
                        ko_local = ban_local.BanKo[teAllList[cnt_local].ko.x, teAllList[cnt_local].ko.y];
                    }

                    // 1手動かしてみる
                    ban_local.moveKoma(ko_local, teAllList[cnt_local], teAllList[cnt_local].nari, false);

                    teAllList[cnt_local].tval = teAllList[cnt_local].val; //次の手の評価値を退避

                    // 王手は即スキップ
                    if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) {
                        teAllList[cnt_local].val = -99999;
                        nexTe = new List<koPos>();  // 未使用
                    } else {
                        nexTe = think(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, 1, maxScore, teAllList[cnt_local].val, teAllList[cnt_local].ko.type, teAllList[cnt_local].x, teAllList[cnt_local].y);

                        if (nexTe?.Count > 0) {
                            if ((uchifuzume == true) && (nexTe[0].val < -5000)) nexTe[0].val = 99999;
                            teAllList[cnt_local].val -= nexTe[0].val;// - tekouho.GetKouho(teAllList[cnt_local]);
                        }
                    }
                    StringBuilder aaa = new StringBuilder("");
                    //string bbb = "";
                    foreach (var n in nexTe ?? new List<koPos>()) {
                        aaa.Append("->[" + n.val + "](" + (n.x + 1) + "," + (n.y + 1) + ")" + n.ko.type);
                        //bbb += " " + usiIO.pos2usi(n.ko, n);
                    }

                    Form1.Form1Instance.addMsg("[" + Task.CurrentId + "]teAll[" + cnt_local + "].val = [" + teAllList[cnt_local].val + "](" + (teAllList[cnt_local].x + 1) + "," + (teAllList[cnt_local].y + 1) + ")" + teAllList[cnt_local].ko.type + aaa);
                    //Console.WriteLine("info score cp " + (ban.chkScore(teban) + teAllList[cnt_local].val) + " pv " + usiIO.pos2usi(teAllList[cnt_local].ko, teAllList[cnt_local]) + bbb);  //標準出力

                    lock (lockObj) {
                        if (maxScore < teAllList[cnt_local].val) {
                            maxScore = teAllList[cnt_local].val;
                            retList = nexTe;
                            retList.Insert(0, teAllList[cnt_local]);
                        }
                    }
                }
            });

            // リストをランダムに並べ替える
            //teAllList = teAllList.OrderBy(a => Guid.NewGuid()).ToList();

            /* 深読み有効 */
            if ((deepThinkDepth > 0) && (!stopFlg) && (maxScore < 5000) && (maxScore > -5000)) {
                maxScore = -9999999;
                List<koPos> nexTe;

                Form1.Form1Instance.addMsg("Deep Think !!");
                // 降順にソート
                teAllList.Sort((a, b) => b.val - a.val);

                for (int cnt_local = 0; cnt_local < deepThinkNum && !stopFlg; cnt_local++) {
                    if ((teAllList[cnt_local].val > 5000) || (teAllList[cnt_local].val < -5000)) continue;

                    BanInfo ban_local = new BanInfo(ban);

                    koma ko_local;
                    if (teAllList[cnt_local].ko.x == 9) {
                        ko_local = ban_local.MochiKo[teban, (int)teAllList[cnt_local].ko.type - 1][0];
                    } else {
                        ko_local = ban_local.BanKo[teAllList[cnt_local].ko.x, teAllList[cnt_local].ko.y];
                    }

                    // 1手動かしてみる
                    ban_local.moveKoma(ko_local, teAllList[cnt_local], teAllList[cnt_local].nari, false);

                    // 王手は即スキップ
                    if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) {
                        teAllList[cnt_local].val = -99999;
                        nexTe = new List<koPos>();  // 未使用
                    } else {

                        /* 盤情報表示 */
                        //string b = ban_local.showBanInfo();
                        //Form1.Form1Instance.addMsg("" + b);
                        nexTe = thinkMove(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, deepThinkDepth, 0, 0, 0);
                        if (nexTe?.Count > 0) {
                            teAllList[cnt_local].val = teAllList[cnt_local].tval - nexTe[0].val;
                        }
                    }

                    if (maxScore < teAllList[cnt_local].val) {
                        maxScore = teAllList[cnt_local].val;
                        retList = nexTe;
                        retList.Insert(0, teAllList[cnt_local]);
                    }

                }

            }



            //return teAllList.Find(s => s.val == teAllList.Max(p => p.val));
            return retList;
            //return teAllList;
        }

        // x y ひとつ前の移動先位置
        List<koPos> think(int teban, BanInfo ban, int depth, int abscore, int up_score, KomaType pre_type, int pre_x, int pre_y) {
            List<koPos> retList = new List<koPos>();
            int score = -999999;
            koPos kp = null;
            bool check = false; // 王手フラグ
            //Form1.Form1Instance.addMsg("think MochiKo= " + ban.OkiKo[teban].Count + ", " + ban.OkiKo[teban].Count + ":" + teban);
            List<koPos> teAllList = new List<koPos>();
            //ban.renewNifList(teban);  //二歩リスト更新

            if (stopFlg) return retList;

            ///* 盤情報表示 */
            //if (maxDepth - depth < 2) {
            //    string bb = ban.showBanInfo();
            //    Form1.Form1Instance.addMsg("" + bb);
            //}

            // 王手
            //if (ban.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban.KingKo[teban].x, ban.KingKo[teban].y] > 0) {
            //    check = true;
            //}

            //指せる手を全てリスト追加
            foreach (koma km in ban.OkiKo[teban]) {
                if (depth < maxDepth) {
                    km.listUpMoveable(ref teAllList, ban);
                } else {
                    km.listUpMoveable(ref teAllList, ban, 1);
                }
            }
            //最下層または王手でない場合は駒打ちを無視
            if ((depth < maxDepth) || (check == true)) {
                for (int i = 0; i < 7; i++) {
                    if (ban.MochiKo[teban, i]?.Count > 0) {
                        ban.MochiKo[teban, i][0].listUpMoveable(ref teAllList, ban);
                        //List<koPos> poslist = ban.MochiKo[teban, i][0].listUpMoveable(ban);
                        //foreach (koPos pos in poslist) {
                        //    if (ban.IdouList[teban, pos.x, pos.y] >= ban.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, pos.x, pos.y]) teAllList.Add(pos);
                        //}
                    }
                }

            }

            // 降順にソート
            teAllList.Sort((a, b) => b.val - a.val);

            if ((depth < maxDepth) || (check == true)) {

                foreach (koPos te in teAllList) {
                    if (te.val > 5000) {
                        retList.Clear();
                        retList.Add(te);
                        break;
                    }

                    // 駒移動の対象を前回移動した駒の前後左右2マス以内に限定
                    //if ((maxDepth - depth > 4) && (pre_type != KomaType.Hisya) && (pre_type != KomaType.Kakugyou) && (pre_type != KomaType.Ryuuma) && (pre_type != KomaType.Ryuuou) && (pre_type != KomaType.Kyousha)) {
                    //    if ((te.ko.type != KomaType.Hisya) && (te.ko.type != KomaType.Kakugyou) && (te.ko.type != KomaType.Ryuuma) && (te.ko.type != KomaType.Ryuuou) && (te.ko.type != KomaType.Kyousha)) {
                    //        if (((te.x - pre_x > 2) || (te.x - pre_x < -2) || (te.y - pre_y > 2) || (te.y - pre_y < -2)) && (ban.IdouList[te.ko.p == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, te.x, te.y] == 0)) {
                    //            continue;
                    //        }
                    //    }
                    //}

                    BanInfo ban_local = new BanInfo(ban);
                    koma ko_local;
                    if (te.ko.x == 9) {
                        ko_local = ban_local.MochiKo[teban, (int)te.ko.type - 1][0];
                    } else {
                        ko_local = ban_local.BanKo[te.ko.x, te.ko.y];
                    }
                    ban_local.moveKoma(ko_local, te, te.nari, false);

                    // 王手は即スキップ
                    if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) {
                        if (score < -999999 + depth * 10000) {
                            retList.Clear();
                            retList.Add(te);
                            te.val = -999999 + depth * 10000;
                        }
                        continue;
                    }

                    List<koPos> childList = think(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth + 1, score, te.val, te.ko.type, te.x, te.y);
                    //te.val -= think(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth + 1, score, te.val).val;
                    //tecount++;
                    //if (tecount> depth*20+10) break;
                    //if (childList.Count > 1) Form1.Form1Instance.addMsg("[" + childList.Count + "]");
                    if (childList?.Count > 0) {
                        te.val -= childList[0].val;
                    }

                    //if (maxDepth - depth < 2) {
                    //    string aaa = "";
                    //    foreach (var n in childList) {
                    //        aaa += "->[" + n.val + "](" + (n.x + 1) + "," + (n.y + 1) + ")" + n.ko.type;
                    //    }
                    //
                    //    Form1.Form1Instance.addMsg("*" + pre_x + "," + pre_y + "/" + pre_type + ":" + +te.val + "](" + (te.x + 1) + "," + (te.y + 1) + ")" + te.ko.type + aaa);
                    //    Thread.Sleep(10);
                    //}

                    if (score < te.val) {
                        score = te.val;
                        retList = childList;
                        retList.Insert(0, te);
                    }

                    // ネガα法による足切り
                    if (abscore > up_score - te.val) break;

                }
            } else {
                // 最下層
                retList.Add(teAllList.Max());
            }

            return retList;
        }

        // 簡易詰将棋アルゴリズムメイン
        // ★空き王手や中合いを考慮しない
        public List<koPos> thinkMateMove(int teban, BanInfo ban, int _maxDepth) {
            maxMateDepth = _maxDepth;
            List<koPos> retList = new List<koPos>();
            List<koPos> teAllList = new List<koPos>();
            int teCnt = 0; //手の進捗
            Object lockObj = new Object();
            //ban.renewNifList(teban);  //二歩リスト更新
            int maxScore = -9999999;

            //[攻め方]王手を指せる手を全てリスト追加
            foreach (koma km in ban.OkiKo[teban]) {
                teAllList.AddRange(km.baninfoPosNext(ban, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].x, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].y));

                //空き王手チェック
                if ((km.type == KomaType.Hisya) || (km.type == KomaType.Ryuuou)) {
                    teAllList.AddRange(km.discoverCheck_hisya(ban));
                } else if ((km.type == KomaType.Kakugyou) || (km.type == KomaType.Ryuuma)) {
                    teAllList.AddRange(km.discoverCheck_Kakugyou(ban));
                } else if (km.type == KomaType.Kyousha) {
                    teAllList.AddRange(km.discoverCheck_Kyousya(ban));
                }

            }

            for (int i = 0; i < 7; i++) {
                if (ban.MochiKo[teban, i]?.Count > 0) teAllList.AddRange(ban.MochiKo[teban, i][0].baninfoPosNext(ban, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].x, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].y));
            }

            //thread同時数
            Form1.Form1Instance.addMsg("[TUME]MinThreads work= " + workMin + ", i/o= " + ioMin + ", teAllList=" + teAllList.Count);
            //Thread.Sleep(2000);
            // 並行処理
            Parallel.For(0, workMin, id => {
                while (true) {
                    int cnt_local;
                    BanInfo ban_local = new BanInfo(ban);
                    koma ko_local;

                    lock (lockObj) {
                        if (teAllList.Count <= teCnt) break;
                        cnt_local = teCnt;
                        teCnt++;
                    }

                    if (teAllList[cnt_local].ko.x == 9) {
                        ko_local = ban_local.MochiKo[teban, (int)teAllList[cnt_local].ko.type - 1][0];
                    } else {
                        ko_local = ban_local.BanKo[teAllList[cnt_local].ko.x, teAllList[cnt_local].ko.y];
                    }

                    // 1手動かしてみる
                    ban_local.moveKoma(ko_local, teAllList[cnt_local], teAllList[cnt_local].nari, false);

                    if (ban_local.KingKo[teban] != null) {
                        if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) continue;
                    }

                    List<koPos> nexTe = new List<koPos>();
                    nexTe = thinkMatedef(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, 1, teAllList[cnt_local]);

                    if (nexTe?.Count > 0) {
                        teAllList[cnt_local].val -= nexTe[0].val;

                        // 打ち歩詰め防止
                        if ((nexTe?.Count == 1) && (nexTe[0].val < -5000) && (teAllList[cnt_local].ko.x == 9) && (teAllList[cnt_local].ko.type == KomaType.Fuhyou)) {
                            continue;
                        }

                    }

                    StringBuilder aaa = new StringBuilder("");
                    foreach (var n in nexTe ?? new List<koPos>()) {
                        aaa.Append("->(" + (n.x + 1) + "," + (n.y + 1) + ")" + n.ko.type);
                    }

                    Form1.Form1Instance.addMsg("[" + Task.CurrentId + "]teAll[" + cnt_local + "].val = [" + teAllList[cnt_local].val + "](" + (teAllList[cnt_local].ko.x + 1) + "," + (teAllList[cnt_local].ko.y + 1) + ")->(" + (teAllList[cnt_local].x + 1) + "," + (teAllList[cnt_local].y + 1) + ")" + teAllList[cnt_local].ko.type + aaa);

                    lock (lockObj) {
                        if ((nexTe?.Count > 0) && (maxScore < teAllList[cnt_local].val)) {
                            maxScore = teAllList[cnt_local].val;
                            retList = nexTe;
                            retList.Insert(0, teAllList[cnt_local]);

                            //それより長い詰みは検索不要
                            if (nexTe.Count < maxMateDepth) {
                                maxMateDepth = nexTe.Count;
                            }

                        }


                    }


                }
            });





            return retList;
        }

        //[守り方]詰将棋1手移動
        List<koPos> thinkMatedef(int teban, BanInfo ban, int depth, koPos pre) {
            List<koPos> retList = new List<koPos>();
            int score = -9999999;

            List<koPos> teAllList = new List<koPos>();
            //ban.renewNifList(teban);  //二歩リスト更新

            // 王の移動(王手の駒を取るも含む)
            ban.KingKo[teban].listUpMoveable(ref teAllList, ban);

            // (王以外の駒で)王手の駒を取る
            foreach (koma km in ban.OkiKo[teban]) {
                if (km.type != KomaType.Ousyou) {
                    teAllList.AddRange(km.baninfoPos(ban, pre.x, pre.y));
                }
            }

            // (王以外の駒で)効きを止める(飛車角香の効きがある場合のみ)
            List<koPos> kikiList = ban.KingKo[teban].kikiList(ban);

            foreach (koPos kiki in kikiList ?? new List<koPos>()) {
                foreach (koma km in ban.OkiKo[teban]) {
                    if (km.type != KomaType.Ousyou) {
                        teAllList.AddRange(km.baninfoPos(ban, kiki.x, kiki.y));
                    }
                }
                for (int i = 0; i < 7; i++) {
                    if (ban.MochiKo[teban, i]?.Count > 0) {
                        teAllList.AddRange(ban.MochiKo[teban, i][0].baninfoPos(ban, kiki.x, kiki.y));
                    }
                }
            }

            //if (depth == 1) {
            //    /* 盤情報表示 */
            //    string bb = ban.showBanInfo();
            //
            //    string aaa = "";
            //    foreach (var n in teAllList ?? new List<koPos>()) {
            //        aaa += "/" + (n.x + 1) + "," + (n.y + 1) + ":" + n.ko.type;
            //    }
            //
            //    Form1.Form1Instance.addMsg(bb + Environment.NewLine + aaa);
            //}

            //一時的に停止
            //if ((pre.ko.type == KomaType.Hisya) || (pre.ko.type == KomaType.Kakugyou) || (pre.ko.type == KomaType.Ryuuma) || (pre.ko.type == KomaType.Ryuuou) || (pre.ko.type == KomaType.Kyousha)) {
            //    for (int i = 0; i < 7; i++) {
            //
            //        if (ban.MochiKo[teban, i]?.Count > 0) {
            //            teAllList.AddRange(ban.MochiKo[teban, i][0].listUpMoveable(ban));
            //        }
            //    }
            //}

            // 降順にソート
            teAllList.Sort((a, b) => b.val - a.val);

            foreach (koPos te in teAllList) {

                BanInfo ban_local = new BanInfo(ban);
                koma ko_local;
                if (te.ko.x == 9) {
                    ko_local = ban_local.MochiKo[teban, (int)te.ko.type - 1][0];
                } else {
                    ko_local = ban_local.BanKo[te.ko.x, te.ko.y];
                }
                ban_local.moveKoma(ko_local, te, te.nari, false);

                if (depth < maxMateDepth) {

                    // 王手は即スキップ
                    if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) {
                        if (score < -999999 + depth * 10000) {
                            retList.Clear();
                            retList.Add(te);
                            te.val = -999999 + depth * 10000;
                        }
                        continue;
                    }

                    List<koPos> childList = thinkMateatk(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth + 1);

                    //詰みがある
                    if (childList?.Count > 0) {
                        te.val -= childList[0].val;
                        if (score < te.val) {
                            score = te.val;
                            retList = childList;
                            retList.Insert(0, te);
                        }

                        //詰みはない
                    } else {
                        retList = null;
                        break;
                    }

                } else {
                    // 王手は即スキップ
                    if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) {
                        retList.Clear();
                        retList.Add(te);
                        te.val = -999999 + depth * 10000;
                    } else {
                        retList = null;
                        break;
                    }

                }


            }

            return retList;
        }

        //[攻め方]詰将棋1手移動
        List<koPos> thinkMateatk(int teban, BanInfo ban, int depth) {
            List<koPos> teAllList = new List<koPos>();
            List<koPos> retList = new List<koPos>();

            int score = -9999999;
            //ban.renewNifList(teban);  //二歩リスト更新

            //[攻め方]王手を指せる手を全てリスト追加
            foreach (koma km in ban.OkiKo[teban]) {
                teAllList.AddRange(km.baninfoPosNext(ban, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].x, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].y));

                //空き王手チェック
                if ((km.type == KomaType.Hisya) || (km.type == KomaType.Ryuuou)) {
                    teAllList.AddRange(km.discoverCheck_hisya(ban));
                } else if ((km.type == KomaType.Kakugyou) || (km.type == KomaType.Ryuuma)) {
                    teAllList.AddRange(km.discoverCheck_Kakugyou(ban));
                } else if (km.type == KomaType.Kyousha) {
                    teAllList.AddRange(km.discoverCheck_Kyousya(ban));
                }

            }
            for (int i = 0; i < 7; i++) {
                if (ban.MochiKo[teban, i]?.Count > 0) teAllList.AddRange(ban.MochiKo[teban, i][0].baninfoPosNext(ban, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].x, ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].y));
            }

            //if (depth == 999) {
            //
            //    /* 盤情報表示 */
            //    string b = ban.showBanInfo();
            //
            //    string aaa = "";
            //    foreach (var n in teAllList ?? new List<koPos>()) {
            //        aaa += "/" + (n.x + 1) + "," + (n.y + 1) + ":" + n.ko.type;
            //    }
            //
            //    Form1.Form1Instance.addMsg(b + Environment.NewLine + ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].x + "," + ban.KingKo[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE].y + "->" + aaa);
            //}

            //foreach (var n in teAllList) {
            //    Form1.Form1Instance.addMsg("<" + depth \+ ">[" + n.val + "](" + (n.ko.x + 1) + "," + (n.ko.y + 1) + ")->(" + (n.x + 1) + "," + (n.y + 1) + ")" + n.ko.type);
            //}

            foreach (koPos te in teAllList) {

                BanInfo ban_local = new BanInfo(ban);
                koma ko_local;


                if (te.ko.x == 9) {
                    ko_local = ban_local.MochiKo[teban, (int)te.ko.type - 1][0];
                } else {
                    ko_local = ban_local.BanKo[te.ko.x, te.ko.y];
                }

                ban_local.moveKoma(ko_local, te, te.nari, false);

                // 王手は即スキップ
                if (ban_local.KingKo[teban] != null) {
                    if (ban_local.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local.KingKo[teban].x, ban_local.KingKo[teban].y] > 0) {
                        continue;
                    }
                }

                List<koPos> childList = thinkMatedef(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth + 1, te);

                if (childList?.Count > 0) {

                    // 打ち歩詰め防止
                    if ((childList?.Count == 1) && (childList[0].val < -5000) && (te.ko.x == 9) && (te.ko.type == KomaType.Fuhyou)) {
                        continue;
                    }

                    te.val -= childList[0].val;
                    if (score < te.val) {
                        score = te.val;
                        retList = childList;
                        retList.Insert(0, te);
                    }

                }

                if (depth > maxMateDepth) {
                    retList = null;
                    break;
                }

            }

            return retList;
        }

    }
}
