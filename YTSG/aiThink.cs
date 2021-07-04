using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YTSG {

    // AI思考部分メイン
    class aiThink {

        System.Random r = new System.Random();
        public int maxDepth; // 最大読み深さ

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
        //            teList = ban.MochiKo[teban][rnd].baninfo(ban);  //動かす駒の移動可能位置一覧
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
        //        teList = ban.OkiKo[teban][rnd].baninfo(ban);  //動かす駒の移動可能位置一覧
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
        public koPos thinkMove(int teban, BanInfo ban, int depth) {
            List<koPos> teAllList = new List<koPos>();
            int teCnt = 0; //手の進捗
            Object lockObj = new Object();

            ban.renewNifList(teban);  //二歩リスト更新


            //int[,,] IdouList = ban.idouList();
            //ban.kikiList();
            //Form1.Form1Instance.addMsg("[SENTE]");
            //for (int i = 0; i < 9; i++)
            //{
            //    Form1.Form1Instance.addMsg("[" + ban.IdouList[0,8,i] + "," + ban.IdouList[0, 7, i] + "," + ban.IdouList[0, 6, i] + "," + ban.IdouList[0, 5, i] + "," + ban.IdouList[0, 4, i] + "," + ban.IdouList[0, 3, i] + "," + ban.IdouList[0, 2, i] + "," + ban.IdouList[0, 1, i] + "," + ban.IdouList[0, 0, i] + "]");
            //}
            //Form1.Form1Instance.addMsg("[GOTE]");
            //for (int i = 0; i < 9; i++)
            //{
            //    Form1.Form1Instance.addMsg("[" + ban.IdouList[1, 8, i] + "," + ban.IdouList[1, 7, i] + "," + ban.IdouList[1, 6, i] + "," + ban.IdouList[1, 5, i] + "," + ban.IdouList[1, 4, i] + "," + ban.IdouList[1, 3, i] + "," + ban.IdouList[1, 2, i] + "," + ban.IdouList[1, 1, i] + "," + ban.IdouList[1, 0, i] + "]");
            //}


            //指せる手を全てリスト追加
            foreach (koma km in ban.OkiKo[teban]) {
                teAllList.AddRange(km.baninfo(ban));
                //List<koPos> poslist = km.baninfo(ban);
                //
                //foreach (koPos pos in poslist)
                //{
                //    if (IdouList[teban, pos.x,pos.y]>= IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, pos.x, pos.y]) teAllList.Add(pos);
                //}
            }

            for (int i = 0; i < 7; i++) {
                if (ban.MochiKo[teban, i]?.Count > 0) teAllList.AddRange(ban.MochiKo[teban, i][0].baninfo(ban));
                //if (ban.MochiKo[teban, i]?.Count > 0)
                //{
                //    List<koPos> poslist = ban.MochiKo[teban, i][0].baninfo(ban);
                //    foreach (koPos pos in poslist)
                //    {
                //        if (IdouList[teban, pos.x, pos.y] >= IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, pos.x, pos.y]) teAllList.Add(pos);
                //    }
                //}
            }


            // 降順にソート
            teAllList.Sort((a, b) => b.val - a.val);

            //thread同時数
            int workMin;
            int ioMin;
            ThreadPool.GetMinThreads(out workMin, out ioMin);
            Form1.Form1Instance.addMsg("MinThreads work= " + workMin + ", i/o= " + ioMin + ", teAllList=" + teAllList.Count);

            // 並行処理
            Parallel.For(0, workMin, id => {
                while (true) {
                    BanInfo ban_local = new BanInfo(ban);


                    koma ko_local;
                    int score = -99999;
                    int cnt_local;

                    lock (lockObj) {
                        if (teAllList.Count <= teCnt) break;
                        cnt_local = teCnt;
                        teCnt++;
                    }

                    if (teAllList[cnt_local].ko.x == 9) {
                        ko_local = ban_local.MochiKo[teban, (int)teAllList[cnt_local].ko.type - 1].Find(k => k.type == teAllList[cnt_local].ko.type);
                    } else {
                        ko_local = ban_local.OkiKo[teban].Find(k => k.x == teAllList[cnt_local].ko.x && k.y == teAllList[cnt_local].ko.y);
                    }

                    //1手動かしてみる
                    ban_local.moveKoma(ko_local, teAllList[cnt_local], teAllList[cnt_local].nari, false);

                    List<koPos> nexTe = think(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth - 1, score, teAllList[cnt_local].val);
                    if (nexTe.Count > 0) {
                        teAllList[cnt_local].val -= nexTe[0].val;
                    }
                    

                    //詰み発見のため残り処理スキップ
                    if (teAllList[cnt_local].val > 90000) {
                        lock (lockObj) {
                            teCnt = 999;
                        }
                    }
                    string aaa = "";
                    foreach (var n in nexTe) {
                        aaa += "->[" + n.val + "](" + (n.x + 1) + "," + (n.y + 1) + ")" + n.ko.type;
                    }
                    
                    Form1.Form1Instance.addMsg("[" + Task.CurrentId + "]teAll[" + cnt_local + "].val = [" + teAllList[cnt_local].val + "](" + (teAllList[cnt_local].x + 1) + "," + (teAllList[cnt_local].y + 1) + ")" + teAllList[cnt_local].ko.type  + aaa);

                    if (score < teAllList[cnt_local].val) score = teAllList[cnt_local].val;

                }
            });

            // リストをランダムに並べ替える
            teAllList = teAllList.OrderBy(a => Guid.NewGuid()).ToList();

            // 降順にソート
            //teAllList.Sort((a, b) => b.val - a.val);

            //return teAllList.Find(s => s.val == teAllList.Max(p => p.val));
            return teAllList.Max();
            //return teAllList;
        }


        List<koPos> think(int teban, BanInfo ban, int depth, int abscore, int up_score) {
            List<koPos> retList = new List<koPos>();
            int score = -99999;
            koPos kp = null;
            //Form1.Form1Instance.addMsg("think MochiKo= " + ban.OkiKo[teban].Count + ", " + ban.OkiKo[teban].Count + ":" + teban);
            List<koPos> teAllList = new List<koPos>();

            ban.renewNifList(teban);  //二歩リスト更新
            ban.renewIdouList();

            //int tecount = 0;

            //int[,,] IdouList = ban.idouList();

            //if (depth < 1) {
            //    ban.kikiList();
            //}

            //指せる手を全てリスト追加
            foreach (koma km in ban.OkiKo[teban]) {
                List<koPos> poslist = km.baninfo(ban);

                foreach (koPos pos in poslist) {
                    //if (ban.IdouList[teban, pos.x, pos.y] < ban.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, pos.x, pos.y]) {
                    //    if (ban.BanKo[pos.x, pos.y] ==null) {
                    //        continue;
                    //    }
                    //}
                    teAllList.Add(pos);
                }
            }
            if (depth > 1) {  //最下層+1では無視

                for (int i = 0; i < 7; i++) {
                    if (ban.MochiKo[teban, i]?.Count > 0) {
                        List<koPos> poslist = ban.MochiKo[teban, i][0].baninfo(ban);
                        foreach (koPos pos in poslist) {
                            if (ban.IdouList[teban, pos.x, pos.y] >= ban.IdouList[teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, pos.x, pos.y]) teAllList.Add(pos);
                        }
                    }
                }

            }

            // 降順にソート
            teAllList.Sort((a, b) => b.val - a.val);

            if (depth > 0) {

                foreach (koPos te in teAllList) {
                    if (te.val > 5000) {
                        retList.Clear();
                        retList.Add(te);
                        break;
                    }
                    BanInfo ban_local = new BanInfo(ban);
                    koma ko_local;
                    if (te.ko.x == 9) {
                        ko_local = ban_local.MochiKo[teban, (int)te.ko.type - 1].Find(k => k.type == te.ko.type);
                    } else {
                        ko_local = ban_local.OkiKo[teban].Find(k => k.x == te.ko.x && k.y == te.ko.y);
                    }
                    ban_local.moveKoma(ko_local, te, te.nari, false);
                    List<koPos> childList = think(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth - 1, score, te.val);
                    //te.val -= think(teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE, ban_local, depth - 1, score, te.val).val;
                    //tecount++;
                    //if (tecount> depth*20+10) break;
                    //if (childList.Count > 1) Form1.Form1Instance.addMsg("[" + childList.Count + "]");
                    if (childList.Count > 0) {
                        te.val -= childList[0].val;
                    }

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

    }
}
