using kmoveDll;
using System;
using System.IO;
using System.Windows.Forms;

namespace YTSG_MKMV {
    public partial class csaReadForm : Form {
        public csaReadForm() {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e) {

            kmove rootKmv = kmove.load(@textBox1.Text);
            if (rootKmv == null) {  //読み込まれていない場合は新規作成
                rootKmv = new kmove(0, 0, 0, 0, false, 0, 0, OPLIST.None, CSLIST.None);
            }

            string[] files = Directory.GetFiles(@textBox3.Text, "*.csa");
            foreach (string cFile in files) {
                Console.WriteLine(cFile);

                kmove tmpKmv = rootKmv;

                int[,] ban = new int[9, 9]; // 疑似駒板
                // FU:1/KY:2/KE:3/GI:4/HI:5/KA:6/KI:7/OU:8/TO:9/NY:10/NK:11/NG:12/RY:13/UM:14
                int count = 0;

                foreach (string line in System.IO.File.ReadLines(@cFile)) {
                    if (((line[0] == '+') || (line[0] == '-')) && (line.Length > 4)) {
                        koPos src = new koPos(0, 0);
                        koPos dst = new koPos(0, 0);
                        bool hitMove = false;
                        bool nari = false;
                        //System.Console.WriteLine(line);
                        var ox = int.Parse(line[1].ToString());
                        var oy = int.Parse(line[2].ToString());
                        var nx = int.Parse(line[3].ToString());
                        var ny = int.Parse(line[4].ToString());
                        var a = csa2num(line.Substring(5, 2));
                        //前と駒タイプが異なる->駒が成った
                        if ((ox>0)&&(oy>0)&&(ban[ox - 1, oy - 1] != a) && (ban[ox - 1, oy - 1] > 0)) nari = true;

                        src.x = ox - 1;
                        src.y = oy - 1;
                        dst.x = nx - 1;
                        dst.y = ny - 1;

                        //駒打ち
                        if ((ox == 0) && (oy== 0)) {
                            src.x = 9;
                            src.y = (int)a;
                            nari = false;
                        }

                        int cnt;

                        for (cnt = 0; cnt < tmpKmv.nxMove.Count; cnt++) {

                            // 一致あり(更新)
                            if ((tmpKmv.nxMove[cnt].ox == src.x) && (tmpKmv.nxMove[cnt].oy == src.y)
                                && (tmpKmv.nxMove[cnt].nx == dst.x) && (tmpKmv.nxMove[cnt].ny == dst.y) && (tmpKmv.nxMove[cnt].nari == nari)) {

                                // 評価値の更新
                                tmpKmv.nxMove[cnt].weight++;
                                tmpKmv.nxMove[cnt].val += 100 / tmpKmv.nxMove[cnt].weight;
                                if (tmpKmv.nxMove[cnt].val > 0) tmpKmv.nxSum += tmpKmv.nxMove[cnt].val;
                                System.Console.WriteLine("ADD: (" + src.x + "," + src.y + ")->(" + dst.x + "," + dst.y + ") val= " + 100);

                                tmpKmv.calcNxSum();
                                tmpKmv = tmpKmv.nxMove[cnt];

                                hitMove = true;

                                break;
                            }
                        }

                        // 一致なし(新規作成)
                        if (hitMove == false) {
                            kmove nkm = new kmove(src.x, src.y, dst.x, dst.y, nari, 100, 1, OPLIST.None, CSLIST.None);

                            //デフォルトがある場合
                            if ((tmpKmv.nxMove.Count > 0) &&
                                (tmpKmv.nxMove[tmpKmv.nxMove.Count - 1].ox == 0) && (tmpKmv.nxMove[tmpKmv.nxMove.Count - 1].oy == 0) &&
                                (tmpKmv.nxMove[tmpKmv.nxMove.Count - 1].nx == 0) && (tmpKmv.nxMove[tmpKmv.nxMove.Count - 1].ny == 0)) {

                                tmpKmv.nxMove.Insert(tmpKmv.nxMove.Count - 1, nkm);
                            } else {
                                tmpKmv.nxMove.Add(nkm);
                            }

                            nkm.val = 100;
                            tmpKmv.calcNxSum();
                            System.Console.WriteLine("NEW: (" + src.x + "," + src.y + ")->(" + dst.x + "," + dst.y + ") val=" + nkm.val);
                            tmpKmv = nkm;

                        }

                        //前の情報を保持(成りチェック用)
                        if (nari == true) {
                            System.Console.WriteLine(a + ":" + ox + "," + oy + "->" + nx + "," + ny + "*");
                        } else {
                            System.Console.WriteLine(a + ":" + ox + "," + oy + "->" + nx + "," + ny);
                        }

                        //前の情報を保持(成りチェック用)
                        ban[nx - 1, ny - 1] = a;

                        count++;

                    }





                    if ((numericUpDown1.Value > 0) && (count > numericUpDown1.Value)) break;
                }
            }

            rootKmv.save(@textBox2.Text);


        }

        int csa2num(string str) {
            int ret;

            switch (str) {
                case "FU":    // 歩兵
                    ret = 1;
                    break;
                case "KY":    // 香車
                    ret = 2;
                    break;
                case "KE":    // 桂馬
                    ret = 3;
                    break;
                case "GI":    // 銀将
                    ret = 4;
                    break;
                case "HI":    // 飛車
                    ret = 5;
                    break;
                case "KA":    // 角行
                    ret = 6;
                    break;
                case "KI":    // 金将
                    ret = 7;
                    break;
                case "OU":    // 王将
                    ret = 8;
                    break;
                case "TO":    // と金
                    ret = 9;
                    break;
                case "NY":    // 成香
                    ret = 10;
                    break;
                case "NK":    // 成桂
                    ret = 11;
                    break;
                case "NG":    // 成銀
                    ret = 12;
                    break;
                case "RY":    // 竜王
                    ret = 13;
                    break;
                case "UM":    // 竜馬
                    ret = 14;
                    break;
                default:
                    ret = -1;
                    break;
            }

            return ret;
        }


        private void CsaReadForm_Load(object sender, EventArgs e) {

        }

        private void Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
