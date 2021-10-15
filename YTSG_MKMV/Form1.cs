using System;
using System.Windows.Forms;

namespace YTSG_MKMV {
    public partial class Form1 : Form {

        //Form1オブジェクトを保持するためのフィールド
        private static Form1 _form1Instance;

        //Form1オブジェクトを取得、設定するためのプロパティ
        public static Form1 Form1Instance {
            get {
                return _form1Instance;
            }
            set {
                _form1Instance = value;
            }
        }

        public Form1() {
            InitializeComponent();
        }

        delegate void delegate1(String text1);
        delegate void delegate2();
        public void addMsg(string msg) {
            Invoke(new delegate1(FormAddMsg), msg);
        }

        public void resetMsg() {
            Invoke(new delegate2(FormResetMsg));
        }

        public void FormAddMsg(string msg) {
            textBox1.AppendText(msg + "\r\n");
            System.Diagnostics.Debug.WriteLine(msg);
        }

        public void FormResetMsg() {
            textBox1.ResetText();
        }

        public string[] getHyokaText(int teban, out int len) {
            if (teban == 0) {
                len = hyokaBox1.Lines.Length;
                return hyokaBox1.Lines;
            } else {
                len = hyokaBox2.Lines.Length;
                return hyokaBox2.Lines;
            }
        }
        
        // 保存する手数を返す (0なら制限なし)
        public int getSaveNum() {
            return (int)numericUpDown1.Value;
        }


        private void StartButton_Click(object sender, EventArgs e) {
            //暫定処理
            Console.WriteLine("readyok");
            StartButton.Enabled = false;
        }

        private void hyokaBox2_TextChanged(object sender, EventArgs e) {

        }
        [STAThread]
        private void button1_Click(object sender, EventArgs e) {
            //mvSetForm mvForm = new mvSetForm();
            //mvForm.Show();
            //Application.Run(mvForm);
            this.Hide();
            mvSetForm f = new mvSetForm();
            f.ShowDialog();
            f.Dispose();
        }
    }
}
