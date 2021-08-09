using kmoveDll;
using System;
using System.Windows.Forms;

namespace YTSG_MKMV {
    public partial class mvSetForm : Form {
        kmove baseKmv = new kmove(0, 0, 0, 0, false, 0, 0);
        int cnt = 0;

        public mvSetForm() {
            InitializeComponent();
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
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                cnt++;
                baseKmv = baseKmv.nxMove[listBox1.SelectedIndex];

                if (baseKmv.nari == true) {
                    label7.Text = "[" + cnt + "] (" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ")* : " + baseKmv.val + " / " + baseKmv.weight;
                } else {
                    label7.Text = "[" + cnt + "] (" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ") : " + baseKmv.val + " / " + baseKmv.weight;
                }

                if (baseKmv != null) {
                    listBox1.Items.Clear();
                    foreach (kmove k in baseKmv.nxMove) {
                        if (k.nari == true) {
                            listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ")* : " + k.val + " / " + k.weight);
                        } else {
                            listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ") : " + k.val + " / " + k.weight);
                        }
                    }
                }
                if (baseKmv.nxMove.Count > 0) {
                    listBox1.SelectedIndex = 0;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                numericUpDown1.Value = baseKmv.nxMove[listBox1.SelectedIndex].ox + 1;
                numericUpDown2.Value = baseKmv.nxMove[listBox1.SelectedIndex].oy + 1;
                numericUpDown3.Value = baseKmv.nxMove[listBox1.SelectedIndex].nx + 1;
                numericUpDown4.Value = baseKmv.nxMove[listBox1.SelectedIndex].ny + 1;
                if (baseKmv.nxMove[listBox1.SelectedIndex].nari == true) {
                    checkBox1.Checked = true;
                } else {
                    checkBox1.Checked = false;
                }
                numericUpDown5.Value = baseKmv.nxMove[listBox1.SelectedIndex].val;
                numericUpDown6.Value = baseKmv.nxMove[listBox1.SelectedIndex].weight;
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                baseKmv.nxMove[listBox1.SelectedIndex].ox = (int)numericUpDown1.Value - 1;
                baseKmv.nxMove[listBox1.SelectedIndex].oy = (int)numericUpDown2.Value - 1;
                baseKmv.nxMove[listBox1.SelectedIndex].nx = (int)numericUpDown3.Value - 1;
                baseKmv.nxMove[listBox1.SelectedIndex].ny = (int)numericUpDown4.Value - 1;
                if (checkBox1.Checked == true) {
                    baseKmv.nxMove[listBox1.SelectedIndex].nari = true;
                } else {
                    baseKmv.nxMove[listBox1.SelectedIndex].nari = false;
                }
                baseKmv.nxMove[listBox1.SelectedIndex].val = (int)numericUpDown5.Value;
                baseKmv.nxMove[listBox1.SelectedIndex].weight = (int)numericUpDown6.Value;
                if (baseKmv.nxMove[listBox1.SelectedIndex].nari == true) {
                    listBox1.Items[listBox1.SelectedIndex] = "(" + (baseKmv.nxMove[listBox1.SelectedIndex].ox + 1) + "," + (baseKmv.nxMove[listBox1.SelectedIndex].oy + 1) + ")->(" +
                        (baseKmv.nxMove[listBox1.SelectedIndex].nx + 1) + "," + (baseKmv.nxMove[listBox1.SelectedIndex].ny + 1) + ")* : " +
                        baseKmv.nxMove[listBox1.SelectedIndex].val + " / " + baseKmv.nxMove[listBox1.SelectedIndex].weight;
                } else {
                    listBox1.Items[listBox1.SelectedIndex] = "(" + (baseKmv.nxMove[listBox1.SelectedIndex].ox + 1) + "," + (baseKmv.nxMove[listBox1.SelectedIndex].oy + 1) + ")->(" +
                        (baseKmv.nxMove[listBox1.SelectedIndex].nx + 1) + "," + (baseKmv.nxMove[listBox1.SelectedIndex].ny + 1) + ") : " +
                        baseKmv.nxMove[listBox1.SelectedIndex].val + " / " + baseKmv.nxMove[listBox1.SelectedIndex].weight;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            baseKmv.nxMove.Add(new kmove((int)numericUpDown1.Value - 1, (int)numericUpDown2.Value - 1, (int)numericUpDown3.Value - 1,
                (int)numericUpDown4.Value - 1, checkBox1.Checked, (int)numericUpDown5.Value, (int)numericUpDown6.Value));
            if (checkBox1.Checked == true) {
                listBox1.Items.Add("(" + (int)numericUpDown1.Value + "," + (int)numericUpDown2.Value + ")->(" +
                    (int)numericUpDown3.Value + "," + (int)numericUpDown4.Value + ")* : " +
                    (int)numericUpDown5.Value + " / " + (int)numericUpDown6.Value);
            } else {
                listBox1.Items.Add("(" + (int)numericUpDown1.Value + "," + (int)numericUpDown2.Value + ")->(" +
                    (int)numericUpDown3.Value + "," + (int)numericUpDown4.Value + ") : " +
                    (int)numericUpDown5.Value + " / " + (int)numericUpDown6.Value);
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                baseKmv.nxMove.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            }
        }

        /* Next */
        private void button6_Click(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                cnt++;
                baseKmv = baseKmv.nxMove[listBox1.SelectedIndex];

                if (baseKmv.nari == true) {
                    label7.Text = "[" + cnt + "] (" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ")* : " + baseKmv.val + " / " + baseKmv.weight;
                } else {
                    label7.Text = "[" + cnt + "] (" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ") : " + baseKmv.val + " / " + baseKmv.weight;
                }

                if (baseKmv != null) {
                    listBox1.Items.Clear();
                    foreach (kmove k in baseKmv.nxMove) {
                        if (k.nari == true) {
                            listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ")* : " + k.val + " / " + k.weight);
                        } else {
                            listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ") : " + k.val + " / " + k.weight);
                        }
                    }
                }

                if (baseKmv.nxMove.Count > 0) {
                    listBox1.SelectedIndex = 0;
                }
            }
        }
    }
}
