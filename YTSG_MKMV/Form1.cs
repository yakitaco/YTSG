using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string getText() {
            return textBox1.Text;
        }

    }
}
