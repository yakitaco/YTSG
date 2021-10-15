using kmoveDll;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YTSG_MKMV {
    public partial class mvSetForm : Form {
        kmove rootKmv = new kmove(0, 0, 0, 0, false, 0, 0);
        kmove baseKmv = null;
        int cnt = 0;
        List<kmove> selList = new List<kmove>();

        List<kmove> copyList = new List<kmove>();

        public mvSetForm() {
            InitializeComponent();
        }

        private void mvSetForm_Load(object sender, EventArgs e) {
            baseKmv = rootKmv;
            showCurrentPos();
        }

        /* load */
        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "YTSG定跡データ(*.ytj)|*.ytj";
            //ofd.InitialDirectory = Directory.GetCurrentDirectory() + @"\Userdata";

            //ofd.ShowDialog();
            //Task.Run(() => {
            kmove tmpKmv = kmove.load();
            //});
            if (tmpKmv != null) {
                rootKmv = tmpKmv;
                baseKmv = rootKmv;
                cnt = 0;
                selList.Clear();
                showCurrentPos();
                listBox1.Items.Clear();
                foreach (kmove k in baseKmv.nxMove) {
                    listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ") : " + k.val + " / " + k.weight);
                }
            }
        }

        /* save */
        private void button3_Click(object sender, EventArgs e) {
            if (rootKmv != null) {
                rootKmv.save();
            }
        }

        /* exit */
        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        /* next */
        private void listBox1_DoubleClick(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                cnt++;
                selList.Add(baseKmv);
                baseKmv = baseKmv.nxMove[listBox1.SelectedIndex];

                showCurrentPos();

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

        // 変更
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
                baseKmv.calcNxSum(); // nxSum更新
                showCurrentPos();
            }
        }

        // 追加
        private void button4_Click(object sender, EventArgs e) {

            /* defaultが存在する */
            if ((baseKmv.nxMove.Count > 0) &&
                (baseKmv.nxMove[baseKmv.nxMove.Count - 1].ox == 0) && (baseKmv.nxMove[baseKmv.nxMove.Count - 1].oy == 0) &&
                (baseKmv.nxMove[baseKmv.nxMove.Count - 1].nx == 0) && (baseKmv.nxMove[baseKmv.nxMove.Count - 1].ny == 0)) {

                baseKmv.nxMove.Insert(baseKmv.nxMove.Count - 1, new kmove((int)numericUpDown1.Value - 1, (int)numericUpDown2.Value - 1, (int)numericUpDown3.Value - 1,
                (int)numericUpDown4.Value - 1, checkBox1.Checked, (int)numericUpDown5.Value, (int)numericUpDown6.Value));

                if (checkBox1.Checked == true) {
                    listBox1.Items.Insert(baseKmv.nxMove.Count - 2, "(" + (int)numericUpDown1.Value + "," + (int)numericUpDown2.Value + ")->(" +
                        (int)numericUpDown3.Value + "," + (int)numericUpDown4.Value + ")* : " +
                        (int)numericUpDown5.Value + " / " + (int)numericUpDown6.Value);
                } else {
                    listBox1.Items.Insert(baseKmv.nxMove.Count - 2, "(" + (int)numericUpDown1.Value + "," + (int)numericUpDown2.Value + ")->(" +
                        (int)numericUpDown3.Value + "," + (int)numericUpDown4.Value + ") : " +
                        (int)numericUpDown5.Value + " / " + (int)numericUpDown6.Value);
                }

                baseKmv.calcNxSum(); // nxSum更新
                showCurrentPos();

            } else {

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

                baseKmv.calcNxSum(); // nxSum更新
                showCurrentPos();
            }
        }

        //削除
        private void button7_Click(object sender, EventArgs e) {
            if ((listBox1.SelectedIndex > -1) && (listBox1.SelectedIndex < listBox1.Items.Count)) {
                baseKmv.nxMove.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                baseKmv.calcNxSum(); // nxSum更新
                showCurrentPos();
            }

        }

        /* Back */
        private void button6_Click(object sender, EventArgs e) {
            if (cnt > 0) {
                cnt--;
                baseKmv = selList[cnt];
                selList.RemoveAt(cnt);
                showCurrentPos();

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

        //先頭へ戻る
        private void button8_Click(object sender, EventArgs e) {
            baseKmv = rootKmv;
            cnt = 0;
            selList.Clear();
            showCurrentPos();
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

        private void showCurrentPos() {
            if (baseKmv != null) {
                if (cnt == 0) {
                    label7.Text = "[" + cnt + "] startpos <" + baseKmv.nxSum + ">";
                    button6.Enabled = false;
                    button8.Enabled = false;
                } else if (baseKmv.nari == true) {
                    label7.Text = "[" + cnt + "] (" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ")* : " + baseKmv.val + " / " + baseKmv.weight + " <" + baseKmv.nxSum + ">";
                    button6.Enabled = true;
                    button8.Enabled = true;
                } else {
                    label7.Text = "[" + cnt + "] (" + (baseKmv.ox + 1) + "," + (baseKmv.oy + 1) + ")->(" + (baseKmv.nx + 1) + "," + (baseKmv.ny + 1) + ") : " + baseKmv.val + " / " + baseKmv.weight + " <" + baseKmv.nxSum + ">";
                    button6.Enabled = true;
                    button8.Enabled = true;
                }
            } else {
                label7.Text = "<ERROR> null";
                button6.Enabled = false;
                button8.Enabled = false;
            }
        }

        /* Copy */
        private void button9_Click(object sender, EventArgs e) {
            copyList = baseKmv.nxMove;
            button10.Enabled = true;

        }

        /* Paste */
        private void button10_Click(object sender, EventArgs e) {
            baseKmv.nxMove = copyList;

            listBox1.Items.Clear();
            foreach (kmove k in baseKmv.nxMove) {
                if (k.nari == true) {
                    listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ")* : " + k.val + " / " + k.weight);
                } else {
                    listBox1.Items.Add("(" + (k.ox + 1) + "," + (k.oy + 1) + ")->(" + (k.nx + 1) + "," + (k.ny + 1) + ") : " + k.val + " / " + k.weight);
                }
            }

            showCurrentPos();
        }
    }
}
