using kmoveDll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSG_GUI {
    public partial class guiForm : Form {
        int x;
        int y;

        public guiForm(params int[] argumentValues) {
            x = argumentValues[0];
            y = argumentValues[1];

            InitializeComponent();

            //マスの作成
            for (int i = 0; i < x; i++) {
                for (int j = 0; i < y; i++) {

                }
            }



        }

        public void set(kmove kmove) {
        
        }




    }
}
