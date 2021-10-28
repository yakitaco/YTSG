
using kmoveDll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//駒の種類
public enum KomaType {
    None,	  //なし
    Fuhyou,	  //歩兵
    Kyousha,  //香車
    Keima,    //桂馬
    Ginsyou,  //銀将
    Hisya,    //飛車
    Kakugyou, //角行
    Kinsyou,  //金将
    Ousyou,   //王将
    Tokin,	  //と金(成歩兵)
    Narikyou, //成香
    Narikei,  //成桂
    Narigin,  //成銀
    Ryuuou,   //竜王
    Ryuuma,   //竜馬
}

//固定値定義
static class TEIGI {
    public const int TEBAN_SENTE = 0; //先手
    public const int TEBAN_GOTE = 1; //後手
    public const int SIZE_SUZI = 9; //筋(x軸)
    public const int SIZE_DAN = 9; //段(y軸)

    public const int yomi4te = 9; //読みを4手先にする手数
    public const int yomi5te = 9; //読みを5手先にする手数
    public const int yomi6te = 9; //読みを6手先にする手数
    public const int yomi5ms = 9; //読みを4手先にする残りミリ秒    
    public const int yomi4ms = 9; //読みを4手先にする残りミリ秒
    public const int yomi3ms = 9; //読みを3手先にする残りミリ秒
}

namespace YTSG {

    //駒の位置
    class koPos : System.IComparable<koPos> {
        public int x;
        public int y;
        public int val;
        public int tval; //現局面での値
        public koma ko;
        public bool nari = false;

        public koPos(int _x, int _y, bool _nari = false) {
            x = _x;
            y = _y;
            val = 0;
            nari = _nari;
        }

        public koPos(int _x, int _y, int _val) {
            x = _x;
            y = _y;
            val = _val;
        }

        public koPos(koma _ko) {
            x = _ko.x;
            y = _ko.y;
            ko = _ko;
        }

        public koPos(koma _ko, int _x, int _y) {
            ko = _ko;
            x = _x;
            y = _y;
        }

        public koPos(int _x, int _y, int _val, koma _ko, bool _nari) {
            x = _x;
            y = _y;
            val = _val;
            ko = _ko;
            nari = _nari;
        }

        //先手後手目線での現位置から相対移動(Relative Move)
        public koPos rMV(int _x, int _y) {
            if (ko == null) {
                return this;
            }
            if (ko.p == TEIGI.TEBAN_SENTE) {
                x += _x;
                y += _y;
            } else {
                x -= _x;
                y -= _y;
            }
            return this;
        }

        // プレイヤーから見た位置(0,0 左上)
        public int px {
            get {
                if ((x < 9) && (x > -1)) {
                    return ko.p == TEIGI.TEBAN_SENTE ? x : TEIGI.SIZE_SUZI - x - 1;
                } else {
                    return x;
                }
            }
        }
        public int py {
            get {
                if ((y < 9) && (y > -1)) {
                    return ko.p == TEIGI.TEBAN_SENTE ? y : TEIGI.SIZE_DAN - y - 1;
                } else {
                    return y;
                }
            }
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




    // プレイヤー視点ありの位置情報
    class pPos {
        public int p = 0; //所持プレイヤー (0:先手/1:後手)
        public int x = 0; //筋 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]
        public int y = 0; //段 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]

        public void set(int _p, int _x, int _y) {
            x = _x;
            y = _y;
            p = _p;
        }

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

    }

    class joseki {



        public void readJoseki(string file) {


        }
    }

    class Program {
        // 手数によるパラメータ
        static moveParam mPar = new moveParam();
        private static System.Timers.Timer aTimer;
        public static List<koPos> kifu = new List<koPos>();

        [STAThread]
        static void Main(string[] args) {
            int myTeban = TEIGI.TEBAN_SENTE;
            aiThink cpu = new aiThink();
            int tesuu = 0;
            Task<List<koPos>> aiTaskMain = null;
            List<koPos> mateMove = null; // 詰みの手筋

            Random rnd = new System.Random();

            int rets = mPar.readParam("");
            rets = mPar.prm[0];
            kmove baseKmv = null;
            kmove tmpKmv = null;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form1 = new Form1();
            Form1.Form1Instance = form1; //Form1Instanceに代入

            //form1.Show();
            Task.Run(() => {
                Application.Run(form1); // デバッグフォーム
                Console.WriteLine("bestmove resign");
            });
            Thread.Sleep(1000);

            BanInfo ban = new BanInfo();

            usiIO usio = new usiIO();

            Process thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            Form1.Form1Instance.resetMsg();

            int startStrPos = 0;
            int teban = 0;

            //Application.Run(form1);
            while (true) {
                int score = 0;
                string str = Console.ReadLine();
                Form1.Form1Instance.addMsg("[RECV]" + str);

                // usi 起動
                if ((str.Length == 3) && (str.Substring(0, 3) == "usi")) {
                    Console.WriteLine("id name YT-Shogi 0.1");
                    Console.WriteLine("id authoer YAKITACO");
                    Console.WriteLine("option name BookFile type string default default.ytj");
                    Console.WriteLine("option name UseBook type check default true");
                    Console.WriteLine("usiok");

                    // isready 対局開始前
                } else if ((str.Length == 7) && (str.Substring(0, 7) == "isready")) {

                    //Form1.Form1Instance.addMsg("[RECV]" + str);

                    /* (再)初期化処理 */
                    //tekouho.ReadJoseki00("");
                    tesuu = 0;

                    mVal.reset(); //評価値のリセット
                    score = 0;

                    //baseKmv = kmove.load();

                    Thread.Sleep(1000);
                    Console.WriteLine("readyok");
                    Form1.Form1Instance.addMsg("[SEND]readyok");

                } else if ((str.Length > 1) && (str.Substring(0, 2) == "go")) {

                    //Form1.Form1Instance.addMsg("[RECV]" + str);
                    //cpu.RandomeMove(myTeban, ref ban);

                    string[] arr = str.Split(' ');

                    //通常読み
                    if (arr[1] == "btime") {

                        int nokori = Convert.ToInt32(myTeban == TEIGI.TEBAN_SENTE ? arr[2] : arr[4]);

                        // タイマー設定
                        //if (nokori > )
                        aTimer = new System.Timers.Timer(2000);


                        Form1.Form1Instance.addMsg("[NOKORI]" + nokori);

                        //定跡あり
                        if ((tmpKmv != null) && (tmpKmv.nxSum > 0)) {
                            int rVal = rnd.Next(0, tmpKmv.nxSum);
                            int i;
                            for (i = 0; i < tmpKmv.nxMove.Count; i++) {
                                if (tmpKmv.nxMove[i].val < 0) continue;
                                if (rVal > tmpKmv.nxMove[i].val) {
                                    rVal -= tmpKmv.nxMove[i].val;
                                    continue;
                                }
                                Form1.Form1Instance.addMsg("Hit Move : (" + tmpKmv.nxMove[i].ox + "," + tmpKmv.nxMove[i].oy + ")->(" + tmpKmv.nxMove[i].nx + "," + tmpKmv.nxMove[i].ny + "):" + tmpKmv.nxMove[i].val);

                                // 移動チェック
                                BanInfo tmpBan = new BanInfo(ban);
                                int ret;
                                
                                if (tmpKmv.nxMove[i].ox == 9) { // 駒打ち
                                    koPos dst = new koPos(0, 0);
                                    dst.ko = tmpBan.MochiKo[teban, tmpKmv.nxMove[i].oy - 1][0];
                                    ret = tmpBan.moveKoma(teban, (KomaType)tmpKmv.nxMove[i].oy, dst, true);
                                } else { // 移動
                                    koPos src = new koPos(tmpKmv.nxMove[i].ox, tmpKmv.nxMove[i].oy);
                                    koPos dst = new koPos(tmpKmv.nxMove[i].nx, tmpKmv.nxMove[i].ny);
                                    ret = tmpBan.moveKoma(src, dst, tmpKmv.nxMove[i].nari, true);
                                }

                                if (ret == 0) {  //移動OK
                                    if (tmpKmv.nxMove[i].ox == 9) {
                                        Console.WriteLine("bestmove " + usio.pos2usi(new koma(0, (KomaType)tmpKmv.nxMove[i].oy, 9, 9), new koPos(tmpKmv.nxMove[i].nx, tmpKmv.nxMove[i].ny)));  //標準出力
                                    } else {
                                        Console.WriteLine("bestmove " + usio.pos2usi(new koma(0, (KomaType)tmpKmv.nxMove[i].oy, tmpKmv.nxMove[i].ox, tmpKmv.nxMove[i].oy), new koPos(tmpKmv.nxMove[i].nx, tmpKmv.nxMove[i].ny, tmpKmv.nxMove[i].nari)));  //標準出力
                                    }
                                } else {  //移動NG
                                    Form1.Form1Instance.addMsg("Move chk NG!!");
                                    i = tmpKmv.nxMove.Count;
                                }
                                break;
                            }
                            if (i < tmpKmv.nxMove.Count) continue; // 定跡ありのためスキップ

                        }

                        thisProcess.PriorityClass = ProcessPriorityClass.RealTime; //優先度高
                        //thisProcess.PriorityClass = ProcessPriorityClass.BelowNormal; //優先度普通

                        //if ((tesuu == 9) || (tesuu == 10)) tekouho.ReadJoseki03("");
                        //if ((tesuu == 39) || (tesuu == 40)) tekouho.ResetJoseki();

                        if ((tesuu < 20) || (nokori < 60000)) {
                            //cpu.maxDepth = 3;
                            //ret = cpu.thinkMove(myTeban, ban, 3, 0)[0]; //コンピュータ思考

                            aiTaskMain = Task.Run(() => {
                                return cpu.thinkMove(myTeban, ban, 4, 0, 0, 0);
                            });

                        } else if ((tesuu < 50) || (nokori < 300000)) {
                            //cpu.maxDepth = 4;
                            //ret = cpu.thinkMove(myTeban, ban, 4)[0]; //コンピュータ思考

                            aiTaskMain = Task.Run(() => {
                                return cpu.thinkMove(myTeban, ban, 4, 5, 3, 0);
                            });

                        } else {
                            //cpu.maxDepth = 5;
                            //ret = cpu.thinkMove(myTeban, ban, 5)[0]; //コンピュータ思考

                            aiTaskMain = Task.Run(() => {
                                return cpu.thinkMove(myTeban, ban, 4, 10, 7, 4);
                                //return cpu.thinkMove(myTeban, ban, 5, 7, 0, 0);
                            });

                        }

                        //Thread.Sleep(2000);
                        //Form1.Form1Instance.addMsg("TESTTESTETEST");

                        List<koPos> retList;
                        retList = aiTaskMain.Result;

                        thisProcess.PriorityClass = ProcessPriorityClass.AboveNormal; //優先度普通

                        tesuu++;

                        if ((retList?.Count == 0) || (retList[0].val < -5000)) { //投了
                            Console.WriteLine("bestmove resign");

                        } else {
                            if (retList.Count > 1) {
                                Console.WriteLine("bestmove " + usio.pos2usi(retList[0].ko, retList[0]) + " ponder " + usio.pos2usi(retList[1].ko, retList[1]));  //標準出力
                                //Console.WriteLine("bestmove " + usio.pos2usi(retList[0].ko, retList[0]));  //標準出力

                            } else {
                                Console.WriteLine("bestmove " + usio.pos2usi(retList[0].ko, retList[0]));  //標準出力
                            }
                            Form1.Form1Instance.addMsg("[SEND]MOVE:" + retList[0].ko.type + ":(" + (retList[0].ko.x + 1) + "," + (retList[0].ko.y + 1) + ")->(" + (retList[0].x + 1) + "," + (retList[0].y + 1) + ")" + (retList[0].nari == true ? "<NARI>" : "") + "\n");
                            ban.moveKoma(retList[0].ko, retList[0], retList[0].nari, false);  //動かす
                            teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);

                            // 詰みの手筋が見えている(先頭2手は削除)
                            if (retList[0].val > 5000) {
                                mateMove = retList;

                                Console.WriteLine("info score mate " + mateMove.Count);  //標準出力

                                mateMove.RemoveAt(0);
                                mateMove.RemoveAt(0);

                            } else {
                                score += retList[0].tval;
                                Console.WriteLine("info score cp " + score);  //標準出力
                            }

                        }

                        // 先読み
                    } else if (arr[1] == "ponder") {

                        // 詰みが見える場合は何もしない
                        if (mateMove?.Count > 0) {
                            Form1.Form1Instance.addMsg("Think Ponder. <<mate>>" + mateMove.Count);

                            // 詰みが見えてない場合のみ先読み実施
                        } else {
                            Form1.Form1Instance.addMsg("Think Ponder.");

                            thisProcess.PriorityClass = ProcessPriorityClass.RealTime; //優先度高
                            int nokori = Convert.ToInt32(myTeban == TEIGI.TEBAN_SENTE ? arr[3] : arr[5]);

                            //if ((tesuu == 9) || (tesuu == 10)) tekouho.ReadJoseki03("");
                            //if ((tesuu == 39) || (tesuu == 40)) tekouho.ResetJoseki();

                            if ((tesuu < 20) || (nokori < 60000)) {
                                //cpu.maxDepth = 4;
                                //ret = cpu.thinkMove(myTeban, ban, 3)[0]; //コンピュータ思考

                                aiTaskMain = Task.Run(() => {
                                    return cpu.thinkMove(myTeban, ban, 4, 0, 0, 0);
                                });

                            } else if ((tesuu < 50) || (nokori < 300000)) {
                                //cpu.maxDepth = 4;
                                //ret = cpu.thinkMove(myTeban, ban, 4)[0]; //コンピュータ思考

                                aiTaskMain = Task.Run(() => {
                                    return cpu.thinkMove(myTeban, ban, 4, 5, 3, 0);
                                });

                            } else {
                                //cpu.maxDepth = 5;
                                //ret = cpu.thinkMove(myTeban, ban, 5)[0]; //コンピュータ思考

                                aiTaskMain = Task.Run(() => {
                                    //return cpu.thinkMove(myTeban, ban, 4, 7, 5, 4);
                                    return cpu.thinkMove(myTeban, ban, 4, 10, 7, 4);
                                });

                            }
                        }

                        // 詰将棋
                    } else if (arr[1] == "mate") {
                        List<koPos> retList;

                        thisProcess.PriorityClass = ProcessPriorityClass.RealTime; //優先度高

                        retList = cpu.thinkMateMove(myTeban, ban, 15);

                        thisProcess.PriorityClass = ProcessPriorityClass.AboveNormal; //優先度普通

                        if (retList?.Count > 0) {
                            retList.RemoveAt(retList.Count - 1);
                            string aaa = "";
                            foreach (var n in retList ?? new List<koPos>()) {
                                aaa += " " + usio.pos2usi(n.ko, n);
                            }
                            Console.WriteLine("checkmate" + aaa);

                            // 詰みなし
                        } else {

                            Console.WriteLine("checkmate nomate");
                        }
                    }

                } else if ((str.Length > 8) && (str.Substring(0, 9) == "ponderhit")) {

                    // 詰みが見えている場合
                    if (mateMove?.Count > 0) {
                        Form1.Form1Instance.addMsg("ponder hit!! <<mate>>");
                        Console.WriteLine("bestmove " + usio.pos2usi(mateMove[0].ko, mateMove[0]) + " ponder " + usio.pos2usi(mateMove[1].ko, mateMove[1]));
                        mateMove.RemoveAt(0);
                        mateMove.RemoveAt(0);
                    } else {
                        Form1.Form1Instance.addMsg("ponder hit!!");
                        List<koPos> retList;
                        //Thread.Sleep(5000);  //早いと先読みthread開始前に読み取ってしまうためwait
                        retList = aiTaskMain.Result;

                        if ((retList?.Count == 0) || (retList[0].val < -5000)) { //投了
                            Console.WriteLine("bestmove resign");

                        } else {
                            if (retList.Count > 1) {
                                Console.WriteLine("bestmove " + usio.pos2usi(retList[0].ko, retList[0]) + " ponder " + usio.pos2usi(retList[1].ko, retList[1]));  //標準出力
                                                                                                                                                                  //Console.WriteLine("bestmove " + usio.pos2usi(retList[0].ko, retList[0]));  //標準出力
                                Form1.Form1Instance.addMsg("[SEND]MOVE:" + retList[0].ko.type + ":(" + (retList[0].ko.x + 1) + "," + (retList[0].ko.y + 1) + ")->(" + (retList[0].x + 1) + "," + (retList[0].y + 1) + ")" + (retList[0].nari == true ? "<NARI>" : "") +
                                    " ponder " + retList[1].ko.type + ":(" + (retList[1].ko.x + 1) + "," + (retList[1].ko.y + 1) + ")->(" + (retList[1].x + 1) + "," + (retList[1].y + 1) + ")" + (retList[1].nari == true ? "<NARI>" : "") + "\n");

                            } else {
                                Console.WriteLine("bestmove " + usio.pos2usi(retList[0].ko, retList[0]));  //標準出力
                                Form1.Form1Instance.addMsg("[SEND]MOVE:" + retList[0].ko.type + ":(" + (retList[0].ko.x + 1) + "," + (retList[0].ko.y + 1) + ")->(" + (retList[0].x + 1) + "," + (retList[0].y + 1) + ")" + (retList[0].nari == true ? "<NARI>" : "") + "\n");

                            }
                            ban.moveKoma(retList[0].ko, retList[0], retList[0].nari, false);  //動かす
                            teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);

                            // 詰みの手筋が見えている(先頭2手は削除)
                            if (retList[0].val > 5000) {
                                mateMove = retList;
                                mateMove.RemoveAt(0);
                                mateMove.RemoveAt(0);
                            }

                        }
                    }

                } else if ((str.Length > 7) && (str.Substring(0, 8) == "position")) {
                    string[] arr = str.Split(' ');

                    teban = TEIGI.TEBAN_SENTE;
                    if (arr[1] == "startpos") {

                        tmpKmv = baseKmv;

                        ban = new BanInfo();
                        kifu = new List<koPos>();
                        startStrPos = 3;

                        // 駒落ち or 途中盤面
                    } else if (arr[1] == "sfen") {

                        ban = new BanInfo(arr[2], arr[4]);
                        startStrPos = 7;
                        tmpKmv = null;

                        if (arr[3] == "b") {
                            teban = TEIGI.TEBAN_SENTE;
                        } else {
                            teban = TEIGI.TEBAN_GOTE;
                        }

                    }

                    // 手を更新(差分のみ)
                    for (tesuu = 0; tesuu + startStrPos < arr.Length; tesuu++) {

                        KomaType type;
                        koPos src = new koPos(0, 0);
                        koPos dst = new koPos(0, 0);
                        bool nari = false;

                        usio.usi2pos(arr[tesuu + startStrPos], out src, out dst, out type, out nari);
                        //Form1.Form1Instance.addMsg("[RECV]AITE:" + arr[tesuu + startStrPos]);
                        //駒打ち
                        if (type != KomaType.None) {
                            dst.ko = ban.MochiKo[teban, (int)type - 1][0];
                            ban.moveKoma(teban, type, dst, false);

                            //駒打ち
                            src.x = 9;
                            src.y = (int)type;
                            nari = false;
                            //駒移動
                        } else {
                            dst.ko = ban.BanKo[src.x, src.y];
                            ban.moveKoma(src, dst, nari, false);
                        }
                        kifu.Add(dst);
                        teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);

                        if (tmpKmv != null) {
                            int cnt;
                            int num = tmpKmv.nxMove.Count;
                            for (cnt = 0; cnt < num; cnt++) {

                                // 一致あり(更新)
                                if ((
                                    (tmpKmv.nxMove[cnt].ox == src.x) && (tmpKmv.nxMove[cnt].oy == src.y)
                                    && (tmpKmv.nxMove[cnt].nx == dst.x) && (tmpKmv.nxMove[cnt].ny == dst.y) && (tmpKmv.nxMove[cnt].nari == nari))||
                                    ((tmpKmv.nxMove[cnt].ox == 0) && (tmpKmv.nxMove[cnt].oy == 0) && (tmpKmv.nxMove[cnt].nx == 0) && (tmpKmv.nxMove[cnt].ny == 0))  //デフォルト
                                    ) {
                                    Form1.Form1Instance.addMsg("[JS][" + cnt + "]" + tmpKmv.nxMove[cnt].ox + "," + tmpKmv.nxMove[cnt].oy + "->" + tmpKmv.nxMove[cnt].nx + "," + tmpKmv.nxMove[cnt].ny + ":" + tmpKmv.nxMove[cnt].nxSum + " Hit!!");
                                    tmpKmv = tmpKmv.nxMove[cnt];
                                    break;
                                }
                                Form1.Form1Instance.addMsg("[JS][" + cnt + "]" + tmpKmv.nxMove[cnt].ox + "," + tmpKmv.nxMove[cnt].oy + "->" + tmpKmv.nxMove[cnt].nx + "," + tmpKmv.nxMove[cnt].ny + ":" + tmpKmv.nxMove[cnt].nxSum);
                            }

                            // 一致なし(新規作成)
                            if (cnt == num) {
                                tmpKmv = null;
                            }
                        }

                    }
                    myTeban = teban;

                    /* 盤情報表示 */
                    string b = ban.showBanInfo();
                    Form1.Form1Instance.addMsg("" + b);

                } else if ((str.Length == 4) && (str.Substring(0, 4) == "stop")) {
                    Form1.Form1Instance.addMsg("ponder miss...");
                    mateMove = null;
                    cpu.stopFlg = true;

                    thisProcess.PriorityClass = ProcessPriorityClass.AboveNormal; //優先度普通

                    List<koPos> retList;
                    retList = aiTaskMain.Result;

                    Console.WriteLine("bestmove 4a3b");  //標準出力

                    cpu.stopFlg = false;

                    // オプション設定
                } else if ((str.Length > 8) && (str.Substring(0, 9) == "setoption")) {
                    string[] arr = str.Split(' ');

                    if (arr[2] == "BookFile") {
                        baseKmv = kmove.load(arr[4]);
                        Form1.Form1Instance.addMsg("[BOOK]" + arr[4]);
                        //baseKmv = kmove.load();

                        //if (baseKmv == null) {  //読み込まれていない場合は新規作成
                        //    baseKmv = new kmove(0, 0, 0, 0, false, 0, 0);
                        //}
                    }

                } else if ((str.Length > 8) && (str.Substring(0, 8) == "gameover")) {

                    //通知
                    lineNotify.doNotify("gameover");

                    //ファイル保存
                    SaveLog.saveNewFile(Form1.Form1Instance.getText(), myTeban);
                    Form1.Form1Instance.resetMsg();
                    tesuu = 0;

                    // アプリケーションの終了
                } else if ((str.Length == 4) && (str.Substring(0, 4) == "quit")) {
                    Form1.Form1Instance.addMsg("[RECV]" + str);
                    //ファイル保存
                    if (tesuu > 0) {
                        SaveLog.saveNewFile(Form1.Form1Instance.getText(), myTeban);
                        Form1.Form1Instance.resetMsg();
                    }

                    //Application.Exit();

                } else {
                    /* 無視 */
                }
            }
        }

        //指定の駒が動けるマス一覧(ban:今の盤面,ko 対象の駒)
        private koPos[] chkMoveArray(BanInfo ban, koma ko) {

            return new koPos[0];
        }

    }
}
