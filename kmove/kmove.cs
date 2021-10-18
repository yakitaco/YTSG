using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace kmoveDll {

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

        [OptionalField]
        public OPLIST[] opening; // 戦型
        public CSLIST[] castle; // 囲い

        public kmove(int _ox, int _oy, int _nx, int _ny, bool _nari, int _val, int _weight, OPLIST _opening, CSLIST _castle) {
            ox = _ox;
            oy = _oy;
            nx = _nx;
            ny = _ny;
            nari = _nari;
            val = _val;
            weight = _weight;
            if (_opening > 0) {
                opening = new OPLIST[1];
                opening[0] = _opening;
            }
            if (_castle > 0) {
                castle = new CSLIST[1];
                castle[0] = _castle;
            }
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

        public static kmove load(string filePath) {
            //開くファイルを選択するダイアログを開く
            kmove loadedData = null;
            if (System.IO.File.Exists(filePath)) {
                //ファイルを読込
                Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                loadedData = (kmove)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            } else {
                MessageBox.Show("'" + filePath + "'は存在しません。");
            }

            return loadedData;
        }

        //ファイルへセーブ
        public bool save() {
            //保存先を指定するダイアログを開く
            System.IO.Directory.CreateDirectory(@"Userdata");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "YTSG定跡データ(*.ytj)|*.ytj";
            sfd.InitialDirectory = Directory.GetCurrentDirectory() + @"\Userdata";
            sfd.FileName = "dat" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            if (sfd.ShowDialog() == DialogResult.OK) {
                //指定したパスにファイルを保存する
                Stream fileStream = sfd.OpenFile();
                BinaryFormatter bF = new BinaryFormatter();
                bF.Serialize(fileStream, this);
                fileStream.Close();
            }
            return true;
        }

        // nxSumの値を更新
        public void calcNxSum() {
            nxSum = 0;
            if (nxMove != null) {
                foreach (kmove k in nxMove) {
                    if (k.val > 0) nxSum += k.val;
                }
            }
        }

        //戦型を更新
        public void setOpening(OPLIST _opening) {
            if (_opening > 0) {
                opening = new OPLIST[1];
                opening[0] = _opening;
            }
        }
        //囲いを更新
        public void setCastle(CSLIST _castle) {
            if (_castle > 0) {
                castle = new CSLIST[1];
                castle[0] = _castle;
            }
        }
    }
}
