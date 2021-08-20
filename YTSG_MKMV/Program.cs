using kmoveDll;
using System;
using System.Threading;
using System.Threading.Tasks;
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

        public static kmove baseKmv = null;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form1 = new Form1();
            int[,,] hyouka = new int[2, 99, 2];
            int[] currentHyouka = new int[2];
            bool autoScore = false;
            int autoStartWin = 0;
            int autoStartLose = 0;
            int autoEndWin = 0;
            int autoEndLose = 0;

            int teban = 0;
            int tesuu = 0;
            //int val = 100;
            usiIO usio = new usiIO();

            while (true) {
                string str = Console.ReadLine();

                if (Form1.Form1Instance != null) {
                    Form1.Form1Instance.addMsg("[RECV]" + str);
                }
                // usi 起動
                if ((str.Length == 3) && (str.Substring(0, 3) == "usi")) {

                    Console.WriteLine("id name YT-Shogi Make Move 0.1");
                    Console.WriteLine("id authoer YAKITACO");
                    Console.WriteLine("option name BookFile type string default public.bin");
                    Console.WriteLine("option name UseBook type check default true");
                    Console.WriteLine("option name AutoScore type check default false"); //自動評価(連続棋譜読み取り用)
                    Console.WriteLine("option name AutoStartWin type spin default 10");
                    Console.WriteLine("option name AutoStartLose type spin default -10");
                    Console.WriteLine("option name AutoEndWin type spin default 100");
                    Console.WriteLine("option name AutoEndLose type spin default -100");

                    Console.WriteLine("usiok");

                // isready 対局開始前
                } else if ((str.Length == 7) && (str.Substring(0, 7) == "isready")) {

                    Task.Run(() => {
                        Application.Run(form1); // デバッグフォーム
                        Console.WriteLine("bestmove resign");
                    });
                    Form1.Form1Instance = form1; //Form1Instanceに代入
                    Thread.Sleep(1000);

                    baseKmv = kmove.load();
                    if (baseKmv == null) {  //読み込まれていない場合は新規作成
                        baseKmv = new kmove(0, 0, 0, 0, false, 0, 0);
                    }

                } else if ((str.Length > 1) && (str.Substring(0, 2) == "go")) {
                    Console.WriteLine("bestmove resign");

                } else if ((str.Length > 7) && (str.Substring(0, 8) == "position")) {

                    /* 評価値情報をロード(先手・後手) */
                    for (int i = 0; i < 2; i++) {
                        string[] hyoStr = Form1.Form1Instance.getHyokaText(i, out int len);

                        for (int j = 0; j < hyoStr.Length; j++) {
                            Form1.Form1Instance.addMsg("[" + i + "," + j + "] " + hyoStr[j]);
                            string[] arrs = hyoStr[j].Split(',');
                            if (arrs.Length > 1) {
                                if ((Int32.TryParse(arrs[0], out int tmp1)) && (Int32.TryParse(arrs[1], out int tmp2))) {
                                    hyouka[i, j, 0] = tmp1;
                                    hyouka[i, j, 1] = tmp2;
                                }
                            }
                        }
                    }

                    //lmv追加
                    string[] arr = str.Split(' ');

                    /* 平手スタートのみ */
                    teban = TEIGI.TEBAN_SENTE;
                    if (arr[1] == "startpos") {

                        kmove tmpKmv = baseKmv;

                        // 手を更新
                        for (tesuu = 0; tesuu + 3 < arr.Length; tesuu++) {

                            if ((hyouka[teban, currentHyouka[teban] + 1, 0] <= tesuu) && (hyouka[teban, currentHyouka[teban] + 1, 0] > hyouka[teban, currentHyouka[teban], 0])) {
                                currentHyouka[teban]++;
                            }

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

                            int cnt;
                            kmove tmps = tmpKmv;
                            for (cnt = 0; cnt < tmpKmv.nxMove.Count; cnt++) {
                                tmps = tmpKmv;
                                // 一致あり(更新)
                                if ((tmpKmv.nxMove[cnt].ox == src.x) && (tmpKmv.nxMove[cnt].oy == src.y)
                                    && (tmpKmv.nxMove[cnt].nx == dst.x) && (tmpKmv.nxMove[cnt].ny == dst.y) && (tmpKmv.nxMove[cnt].nari == nari)) {

                                    // 評価値の更新
                                    tmpKmv.nxMove[cnt].weight++;
                                    tmpKmv.nxSum -= tmpKmv.nxMove[cnt].val;
                                    tmpKmv.nxMove[cnt].val += hyouka[teban, currentHyouka[teban], 1] / tmpKmv.nxMove[cnt].weight;
                                    if (tmpKmv.nxMove[cnt].val > 0) tmpKmv.nxSum += tmpKmv.nxMove[cnt].val;
                                    Form1.Form1Instance.addMsg("ADD: (" + src.x + "," + src.y + ")->(" + dst.x + "," + dst.y + ") val= " + hyouka[teban, currentHyouka[teban], 1]);
                                    tmpKmv = tmpKmv.nxMove[cnt];

                                    break;
                                }
                            }

                            // 一致なし(新規作成)
                            if (cnt == tmps.nxMove.Count) {
                                kmove nkm = new kmove(src.x, src.y, dst.x, dst.y, nari, hyouka[teban, currentHyouka[teban], 1], 1);
                                tmps.nxMove.Add(nkm);
                                nkm.val = hyouka[teban, currentHyouka[teban], 1];
                                if (nkm.val > 0) tmps.nxSum += nkm.val;
                                Form1.Form1Instance.addMsg("NEW: (" + src.x + "," + src.y + ")->(" + dst.x + "," + dst.y + ") val=" + nkm.val);
                                tmps = nkm;

                            }

                            teban = (teban == TEIGI.TEBAN_SENTE ? TEIGI.TEBAN_GOTE : TEIGI.TEBAN_SENTE);


                        }

                    }

                } else if ((str.Length > 8) && (str.Substring(0, 8) == "gameover")) {
                    //kmv 保存
                    baseKmv.save();

                } else if ((str.Length > 3) && (str.Substring(0, 4) == "open")) {
                    mvSetForm f = new mvSetForm();
                    Application.Run(f);
                    //kmv 保存
                    //baseKmv.save();

                } else if ((str.Length > 8) && (str.Substring(0, 9) == "setoption")) {
                    string[] arr = str.Split(' ');

                    if (arr[2] == "AutoScore") {
                        if (arr[4] == "True") {
                            autoScore = true;
                        } else {
                            autoScore = false;
                        }
                    } else if (arr[2] == "AutoStartWin") {
                        autoStartWin = int.Parse(arr[4]);
                    } else if (arr[2] == "AutoStartLose") {
                        autoStartLose = int.Parse(arr[4]);
                    } else if (arr[2] == "AutoEndWin") {
                        autoEndWin = int.Parse(arr[4]);
                    } else if (arr[2] == "AutoEndLose") {
                        autoEndLose = int.Parse(arr[4]);
                    }

                } else {
                    /* 無視 */
                }
            }


        }



    }
}
