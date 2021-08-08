using kmoveDll;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSG_MKMV {
    public partial class mvSetForm : Form {
        kmove baseKmv = new kmove(0, 0, 0, 0, false, 0, 0);
        int cnt = 0;

        public mvSetForm() {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void mvSetForm_Load(object sender, EventArgs e) {

        }

        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "YTSG定跡データ(*.ytj)|*.ytj";
            //ofd.InitialDirectory = Directory.GetCurrentDirectory() + @"\Userdata";

            //ofd.ShowDialog();
            //Task.Run(() => {
                baseKmv = kmove.load();
            //});
            if (baseKmv != null) {
                cnt = 0;
                label7.Text = "[0]startpos";
                listBox1.Items.Clear();
                foreach (kmove k in baseKmv.nxMove) {
                    listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ") : " + k.val + " / " + k.weight);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            baseKmv.save();
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)){
                cnt++;
                baseKmv = baseKmv.nxMove[listBox1.SelectedIndex];
                label7.Text = "[" + cnt + "](" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ") : " + baseKmv.val + " / " + baseKmv.weight;
                if (baseKmv != null) {
                    listBox1.Items.Clear();
                    foreach (kmove k in baseKmv.nxMove) {
                        listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ") : " + k.val + " / " + k.weight);
                    }
                }
            }
        }
    }
}
