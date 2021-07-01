﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//駒の種類
public enum KomaType
{
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
static class TEIGI
{
    public const int TEBAN_SENTE = 0; //先手
    public const int TEBAN_GOTE  = 1; //後手
    public const int SIZE_SUZI   = 9; //筋(x軸)
    public const int SIZE_DAN    = 9; //段(y軸)
    
    public const int yomi4te     = 9; //読みを4手先にする手数
    public const int yomi5te     = 9; //読みを5手先にする手数
    public const int yomi6te     = 9; //読みを6手先にする手数
    public const int yomi5ms     = 9; //読みを4手先にする残りミリ秒    
    public const int yomi4ms     = 9; //読みを4手先にする残りミリ秒
    public const int yomi3ms     = 9; //読みを3手先にする残りミリ秒
}

namespace YTSG {

    //駒の位置
    class koPos : System.IComparable<koPos>
    {
        public int x;
        public int y;
        public int val;
        public koma ko;
        public bool nari = false;

        public koPos(int _x, int _y)
        {
            x = _x;
            y = _y;
            val = 0;
        }

        public koPos(int _x, int _y, int _val)
        {
            x = _x;
            y = _y;
            val = _val;
        }

        public koPos(int _x, int _y, int _val, koma _ko, bool _nari)
        {
            x = _x;
            y = _y;
            val = _val;
            ko = _ko;
            nari = _nari;
        }

        // 比較用の処理
        //  0 なら同じ
        // -1 なら比べた相手の方が大きい
        //  1 なら比べた相手の方が小さい
        public int CompareTo(koPos i_other)
        {
            // Ageの数と名前の文字列の数を足したものを、比較用の数値する
            int thisNum = this.val;
            int otherNum = i_other.val;

            if (thisNum == otherNum)
            {
                return 0;
            }

            // 数値が"低い"方が偉い(大きい)扱いにする！
            if (thisNum > otherNum)
            {
                return 1;
            }
            return -1;
        }

    }
    
    // プレイヤー視点ありの位置情報
    class pPos{
        public int p; //所持プレイヤー (0:先手/1:後手)
        public int x; //筋 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]
        public int y; //段 (0-8) [持ち駒:9 / 盤上無い(コマ落ち等): -1]
    
        public pPos(int _p, int _x, int _y) {
            x = _x;
            y = _y;
            p = _p;
        }
        
        // プレイヤーから見た位置(0,0 左上)
        public int px {
            get {
                if ((x < 9)&&(x > -1)) {
                    return p == TEIGI.TEBAN_SENTE ? x : TEIGI.SIZE_SUZI - x - 1;
                } else {
                    return x;
                }
            }
        }
        public int py {
            get {
                if ((y < 9)&&(y > -1)) {
                    return p == TEIGI.TEBAN_SENTE ? y : TEIGI.SIZE_DAN - y - 1;
                } else {
                    return y;
                }
            }
        }
        
    }

    class joseki
    {



        public void readJoseki(string file)
        {


        }
    }

    static class tekouho
    {
        static int[,,] IdouList = new int[14,9,9];

        public static void ResetJoseki()
        {
            Array.Clear(IdouList, 0, IdouList.Length);
        }

        public static void ReadJoseki(string file)
        {
            IdouList[0, 6, 5] = 100;
            IdouList[0, 1, 5] =  90;
            IdouList[0, 5, 5] = 100;
            IdouList[0, 1, 4] = 150;
            IdouList[0, 1, 3] = 160;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    IdouList[0, i, j] = (3-j)*100+100;
                    IdouList[1, i, j] = (3 - j) * 100 + 100;
                    IdouList[2, i, j] = (3 - j) * 100 + 100;
                    IdouList[3, i, j] = (3 - j) * 100 + 100;
                    IdouList[4, i, j] = (3 - j) * 100 + 100;
                    IdouList[5, i, j] = (3 - j) * 100 + 100;
                }
            }
            IdouList[0, 8, 5] = 10;
            IdouList[0, 0, 5] = 10;
            IdouList[0, 8, 5] = 5;
            IdouList[0, 0, 5] = 5;
            IdouList[0, 2, 4] = -50;
            IdouList[0, 2, 5] = 10;
            IdouList[0, 7, 5] = -50;
            IdouList[2, 2, 6] = 50; //桂

            IdouList[3, 2, 7] = 40; //銀
            IdouList[3, 1, 6] = 70; //銀
            IdouList[3, 2, 6] = 70; //銀
            IdouList[3, 0, 5] = 80; //銀
            IdouList[3, 1, 5] = 80; //銀
            IdouList[3, 5, 7] = 50; //銀
            IdouList[3, 5, 6] = 80; //銀
            IdouList[3, 6, 6] = 100; //銀
            IdouList[3, 6, 6] = 10; //銀
            IdouList[1, 0, 7] = -50; //香
            IdouList[1, 8, 7] = -50; //香
            IdouList[6, 6, 7] = 120; //金
            IdouList[6, 6, 6] = 181; //金
            IdouList[7, 5, 8] = 1; //王
            IdouList[7, 6, 8] = 2; //王
            IdouList[7, 7, 6] = 3; //王
            IdouList[7, 8, 8] = 4; //王
            IdouList[5, 6, 6] = 20; //角
            IdouList[5, 7, 5] = -70; //角
            IdouList[5, 6, 8] = 30; //角
            //IdouList[7, 8, 8] = 2; //王
            //IdouList[7, 8, 8] = 2; //王
        }

        public static int GetKouho(int teban, koma ko, koPos dstPos)
        {
            //後手は逆
            int sX = (teban == TEIGI.TEBAN_SENTE ? ko.x : (TEIGI.SIZE_SUZI - ko.x - 1));
            int sY = (teban == TEIGI.TEBAN_SENTE ? ko.y : (TEIGI.SIZE_SUZI - ko.y - 1));
            int dX = (teban == TEIGI.TEBAN_SENTE ? dstPos.x : (TEIGI.SIZE_SUZI - dstPos.x - 1));
            int dY = (teban == TEIGI.TEBAN_SENTE ? dstPos.y : (TEIGI.SIZE_SUZI - dstPos.y - 1));

            //移動前と移動後の差分
            return IdouList[(int)ko.type - 1, dX, dY] - IdouList[(int)ko.type - 1, sX, sY];
        }

    }

    class Program
    {
        // 手数によるパラメータ
        static moveParam mPar = new moveParam();
        private static System.Timers.Timer aTimer;

        static void Main(string[] args)
        {
            int myTeban = TEIGI.TEBAN_SENTE;
            aiThink cpu = new aiThink();
            bool initFlg = true; //初期フラグ
            int tesuu = 0;

            int rets = mPar.readParam("");
            rets = mPar.prm[0];


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

            Console.WriteLine("id name YT-Shogi");
            Console.WriteLine("id authoer YAKITACO");
            Console.WriteLine("option name BookFile type string default public.bin");
            Console.WriteLine("option name UseBook type check default true");
            Console.WriteLine("usiok");


            string str = Console.ReadLine();
            Form1.Form1Instance.addMsg("[RECV]" + str);

            int startStrPos = 0;
            int teban = 0;

            //Application.Run(form1);
            while (true) {
                str = Console.ReadLine();

                // isready 対局開始前
                if ((str.Length == 7) && (str.Substring(0, 7) == "isready"))
                {
                    Form1.Form1Instance.addMsg("[RECV]" + str);

                    /* (再)初期化処理 */
                    tekouho.ReadJoseki("");
                    tesuu = 0;
                    initFlg = true;

                    Thread.Sleep(1000);
                    Console.WriteLine("readyok");
                    Form1.Form1Instance.addMsg("[SEND]readyok");

                } else if ((str.Length > 1) && (str.Substring(0, 2) == "go")) {
                    Form1.Form1Instance.addMsg("[RECV]" + str);
                    //cpu.RandomeMove(myTeban, ref ban);

                    string[] arr = str.Split(' ');
                    int nokori = Convert.ToInt32(myTeban == TEIGI.TEBAN_SENTE ? arr[2] : arr[4]);

                    // タイマー設定
                    //if (nokori > )
                    aTimer = new System.Timers.Timer(2000);


                    Form1.Form1Instance.addMsg("[NOKORI]" + nokori);
                    
                    thisProcess.PriorityClass = ProcessPriorityClass.RealTime; //優先度高

                    if (tesuu == 30) tekouho.ResetJoseki();

                    koPos ret;
                    if ((tesuu < 20)||(nokori< 60000)) {
                        ret = cpu.thinkMove(myTeban, ban, 3); //コンピュータ思考
                    } else if ((tesuu < 40)||(nokori< 150000)) {
                        ret = cpu.thinkMove(myTeban, ban, 4); //コンピュータ思考
                    } else {
                        ret = cpu.thinkMove(myTeban, ban, 4); //コンピュータ思考
                    }

                    thisProcess.PriorityClass = ProcessPriorityClass.AboveNormal; //優先度普通

                    tesuu++;

                    if (ret.val < -5000)
                    { //投了
                        Console.WriteLine("bestmove resign");

                    }
                    else
                    {
                        Console.WriteLine("bestmove " + usio.pos2usi(ret.ko, ret));  //標準出力
                        Form1.Form1Instance.addMsg("[SEND]MOVE:" + ret.ko.type + ":(" + (ret.ko.x + 1) + "," + (ret.ko.y + 1) + ")->(" + (ret.x + 1) + "," + (ret.y + 1) + ")" + (ret.nari == true ? "<NARI>" : "") + "\n");
                        ban.moveKoma(ret.ko, ret, ret.nari, false);  //動かす
                        teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);

                        //foreach (koma km in ban.OkiKo[0])
                        //{
                        //    Form1.Form1Instance.addMsg("OkiKo[0] :" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                        //}
                        //foreach (koma km in ban.OkiKo[1])
                        //{
                        //    Form1.Form1Instance.addMsg("OkiKo[1] :" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                        //}
                        //for (int i =0; i < 7; i++) {
                        //    foreach (koma km in ban.MochiKo[0,i])
                        //    {
                        //        Form1.Form1Instance.addMsg("MochiKo[0]:" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                        //    }
                        //}
                        //for (int i = 0; i < 7; i++)
                        //{
                        //    foreach (koma km in ban.MochiKo[1,i])
                        //    {
                        //        Form1.Form1Instance.addMsg("MochiKo[1]:" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                        //    }
                        //}
                    }

                } else if ((str.Length > 7)&&(str.Substring(0, 8) == "position")) {
                    string[] arr = str.Split(' ');

                    //初期処理
                    if (initFlg)
                    {
                        teban = TEIGI.TEBAN_SENTE;
                        if (arr[1] == "startpos") {
                            //平手の対応
                            if (arr.Length == 4) {
                                myTeban = TEIGI.TEBAN_GOTE;
                                Form1.Form1Instance.addMsg("GOTE");
                            } else {
                                myTeban = TEIGI.TEBAN_SENTE;
                                Form1.Form1Instance.addMsg("SENTE");
                            }
                            
                            ban = new BanInfo();
                            startStrPos = 3;
                            
                            // 駒落ち or 途中盤面
                        } else if (arr[1] == "sfen") {
                            Form1.Form1Instance.addMsg("[RECV]" + str);
                            
                            ban = new BanInfo(arr[2], arr[4]);
                            startStrPos = 7;
                            
                            if (arr[3] == "b") {
                                teban = TEIGI.TEBAN_SENTE;
                            } else {
                                teban = TEIGI.TEBAN_GOTE;
                            }

                            /* 盤情報表示 */
                            string b = ban.showBanInfo();
                            Form1.Form1Instance.addMsg("" + b);
                            Thread.Sleep(10000);
                        }

                        initFlg = false;

                    }
                    
                    // 手を更新(差分のみ)
                    for (int i = tesuu + startStrPos ; i < arr.Length ; i++, tesuu++){

                        KomaType type;
                        koPos src = new koPos(0, 0);
                        koPos dst = new koPos(0, 0);
                        bool nari = false;
                        
                        usio.usi2pos(arr[i], out src, out dst, out type, out nari);
                        Form1.Form1Instance.addMsg("[RECV]AITE:" + arr[i]);
                        //駒打ち
                        if (type != KomaType.None) {
                            ban.moveKoma(teban, type, dst, false);
                        //駒移動
                        }
                        else {
                            ban.moveKoma(src, dst, nari, false);
                        }
                        teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);

                        /* 盤情報表示 */
                        string b = ban.showBanInfo();
                        Form1.Form1Instance.addMsg("" + b);

                    }
                    myTeban = teban;


                } else if ((str.Length > 8) && (str.Substring(0, 8) == "gameover")) {
                    Form1.Form1Instance.addMsg("[RECV]" + str + "\n");
                    //全駒表示
                    foreach (koma km in ban.OkiKo[0])
                    {
                        Form1.Form1Instance.addMsg("OkiKo[0] :" + km.p +":" + km.type+" ("+ (km.x+1) +","+ (km.y+1) +")");
                    }
                    foreach (koma km in ban.OkiKo[1])
                    {
                        Form1.Form1Instance.addMsg("OkiKo[1] :" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        foreach (koma km in ban.MochiKo[0, i])
                        {
                            Form1.Form1Instance.addMsg("MochiKo[0]:" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                        }
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        foreach (koma km in ban.MochiKo[1, i])
                        {
                            Form1.Form1Instance.addMsg("MochiKo[1]:" + km.p + ":" + km.type + " (" + (km.x + 1) + "," + (km.y + 1) + ")");
                        }
                    }

                    //通知
                    lineNotify.doNotify("gameover");

                    //ファイル保存
                    SaveLog.saveNewFile(Form1.Form1Instance.getText(), myTeban);
                    Form1.Form1Instance.resetMsg();

                } else {
                    Form1.Form1Instance.addMsg("[RECV]" + str);
                }
            }
        }

        //指定の駒が動けるマス一覧(ban:今の盤面,ko 対象の駒)
        private koPos[] chkMoveArray(BanInfo ban, koma ko) {

            return new koPos[0];
        }

    }
}
