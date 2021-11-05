using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSG_MKMV {
    public partial class csaReadForm : Form {
        public csaReadForm() {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e) {
            string[] names = Directory.GetFiles(textBox3.Text, "*.csa");
            foreach (string name in names) {
                Console.WriteLine(name);
            }
        }

        private void CsaReadForm_Load(object sender, EventArgs e) {

        }

        private void Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
