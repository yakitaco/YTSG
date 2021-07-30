using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSG_MKMV {

	[Serializable]
	public class kmove {
		public int ox; //(0-9) * 9=持ち駒
		public int oy; //(0-8(ox<9の場合) or 1-7 (ox=9の場合)
		public int nx; //(0-9)
		public int ny; //(0-9)
		public bool nari;
		public int val;//この移動手の評価値(>0だと移動候補 <0は対象外)
		public int weight;//重み(この値が大きいほど更新値が小さくなる) val/weight,weight++
		public List<kmove> nxMove = new List<kmove>(); //次の移動手
		public int nxSum; //次の移動手の合計値(次のvalが0以上の値のみ加算)

		public kmove(int _ox, int _oy, int _nx, int _ny, bool _nari, int _val, int _weight) {
			ox = _ox;
			oy = _oy;
			nx = _nx;
			ny = _ny;
			nari = _nari;
			val = _val;
			weight = _weight;
        }

        //ファイルから読み取り
        public static kmove load() {
            //開くファイルを選択するダイアログを開く
            kmove loadedData = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "YTSG定跡データ(*.ytj)|*.ytj";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + @"\Userdata";
            if (ofd.ShowDialog() == DialogResult.OK) {
                //ファイルを読込
                Stream fileStream = ofd.OpenFile();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                loadedData = (kmove)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
            return loadedData;
        }

        //ファイルへセーブ
        public bool save() {
            //保存先を指定するダイアログを開く
            System.IO.Directory.CreateDirectory(@"Userdata");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "チームデータ(*.ytj)|*.ytj";
            sfd.InitialDirectory = Directory.GetCurrentDirectory() + @"\Userdata";
            sfd.FileName = "dat" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".tdt";
            if (sfd.ShowDialog() == DialogResult.OK) {
                //指定したパスにファイルを保存する
                Stream fileStream = sfd.OpenFile();
                BinaryFormatter bF = new BinaryFormatter();
                bF.Serialize(fileStream, this);
                fileStream.Close();
            }
            return true;
        }

    }
}
