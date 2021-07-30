using System;
using System.Windows.Forms;

namespace YTSG_MKMV {

    //駒の種類
    public enum KomaType {
        None,     //なし
        Fuhyou,   //歩兵
        Kyousha,  //香車
        Keima,    //桂馬
        Ginsyou,  //銀将
        Hisya,    //飛車
        Kakugyou, //角行
        Kinsyou,  //金将
        Ousyou,   //王将
        Tokin,    //と金(成歩兵)
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
    }

    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            Console.WriteLine("id name YT-Shogi Make Move 0.1");
            Console.WriteLine("id authoer YAKITACO");
            Console.WriteLine("option name BookFile type string default public.bin");
            Console.WriteLine("option name UseBook type check default true");
            Console.WriteLine("usiok");

            int teban = 0;
            int tesuu = 0;
            kmove baseKmv = null;
            int val = 100;
            usiIO usio = new usiIO();

            while (true) {
                string str = Console.ReadLine();
                Form1.Form1Instance.addMsg("[RECV]" + str);
                if ((str.Length == 7) && (str.Substring(0, 7) == "isready")) {
                    baseKmv = kmove.load();
                    if (baseKmv == null) {  //読み込まれていない場合は新規作成
                        baseKmv = new kmove(0, 0, 0, 0, false, 0, 0);
                    }
                    Console.WriteLine("readyok");

                } else if ((str.Length > 1) && (str.Substring(0, 2) == "go")) {
                    Console.WriteLine("bestmove resign");

                } else if ((str.Length > 7) && (str.Substring(0, 8) == "position")) {
                    //lmv追加
                    string[] arr = str.Split(' ');

                    /* 平手スタートのみ */
                    teban = TEIGI.TEBAN_SENTE;
                    if (arr[1] == "startpos") {

                        kmove tmpKmv = baseKmv;

                        // 手を更新
                        for (tesuu = 0; tesuu + 3 < arr.Length; tesuu++) {

                            KomaType type;
                            koPos src = new koPos(0, 0);
                            koPos dst = new koPos(0, 0);
                            bool nari = false;

                            usio.usi2pos(arr[tesuu + 3], out src, out dst, out type, out nari);
                            Form1.Form1Instance.addMsg("[RECV]AITE:" + arr[tesuu + 3]);

                            //駒打ち
                            if (type != KomaType.None) {
                                src.x = 9;
                                src.y = (int)type;
                                nari = false;
                            }

                            teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);

                            int cnt;
                            for (cnt = 0; cnt < tmpKmv.nxMove.Count; cnt++) {
                                // 一致あり(更新)
                                if ((tmpKmv.nxMove[cnt].ox == src.x) && (tmpKmv.nxMove[cnt].ox == src.y)
                                    && (tmpKmv.nxMove[cnt].nx == dst.x) && (tmpKmv.nxMove[cnt].nx == dst.y) && (tmpKmv.nxMove[cnt].nari == nari)) {

                                    // 評価値の更新
                                    tmpKmv.nxMove[cnt].weight++;
                                    tmpKmv.val -= tmpKmv.nxMove[cnt].val;
                                    tmpKmv.nxMove[cnt].val += val / tmpKmv.nxMove[cnt].weight;
                                    if (tmpKmv.nxMove[cnt].val > 0) tmpKmv.val += tmpKmv.nxMove[cnt].val;

                                    tmpKmv = tmpKmv.nxMove[cnt];

                                    break;
                                }
                            }

                            // 一致なし(新規作成)
                            if (cnt == tmpKmv.nxMove.Count) {
                                kmove nkm = new kmove(src.x, src.y, dst.x, dst.y, nari, val, 1);
                                tmpKmv.nxMove.Add(nkm);
                                if (val > 0) tmpKmv.val += val;

                                tmpKmv = nkm;

                            }

                        }

                    }

                } else if ((str.Length > 8) && (str.Substring(0, 8) == "gameover")) {
                    //kmv 保存
                    baseKmv.save();

                } else {
                    /* 無視 */
                }
            }


        }
    }
}
