using kmoveDll;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YTSG {
    class mVal {
        OPLIST type;
        int move;
        public int[,,] val;

        static List<mVal> mV = new List<mVal>();
        static int senTeNum = 0;
        static int goTeNum = 0;
        static int stage = 0;  //0:序盤(型作成) 1:中盤(攻防開始) 2:終盤(詰め重視) 

        static mVal() {

            // 評価値情報ファイル読み取り
            loadFile();

        }

        static void loadFile() {
            string[] files = Directory.GetFiles(@"./mList/", "*.mvl");
            Form1.Form1Instance.addMsg("sss" + files.Length);
            foreach (string cFile in files) {
                Form1.Form1Instance.addMsg(cFile);
                int count = 0;
                mVal tmp = new mVal(0, 0);
                tmp.val = new int[14, 9, 9];
                foreach (string line in System.IO.File.ReadLines(@cFile)) {
                    if (line[0] == '#') continue; // コメント行はスキップ
                    if (count == 0) tmp.type = (OPLIST)int.Parse(line);
                    if (count == 1) tmp.move = int.Parse(line);
                    if (count > 1) {
                        string[] arr = line.Split(',');
                        for (int i = 0; i < 9; i++) {
                            tmp.val[(count - 2) / 9, (count - 2) % 9, i] = int.Parse(arr[i]);
                        }
                    }
                    mV.Add(tmp);
                    count++;
                }
            }
        }

        mVal(OPLIST _type, int _move) {
            type = _type;
            move = _move;
        }

        public static void reset() {
            senTeNum = 0;
            goTeNum = 0;
            stage = 0;
        }

        public static void setType(OPLIST _type, int turn, int count) {
            int tmpCount = -1;
            int cnt = 0;
            for (cnt = 0; cnt < mV.Count; cnt++) {
                if ((mV[cnt].type == _type) && (mV[cnt].move <= count) && (tmpCount < mV[cnt].move)) {
                    if (turn == 0) {
                        senTeNum = cnt;
                    } else {
                        goTeNum = cnt;
                    }
                    tmpCount = mV[cnt].move;
                }


            }
            if (tmpCount == -1) {
                if (turn == 0) {
                    senTeNum = 0;
                } else {
                    goTeNum = 0;
                }
            }
        }


        // 局面のチェック★暫定版
        public static void tmpChk(BanInfo ban) {
            //序盤のみ
            if (stage == 0) {
                //先手
                koma hisya = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Hisya);
                koma fuhyou;
                if (hisya != null) {
                    switch (hisya.x) {
                        case 0:    // 1筋 (右地下鉄？)
                            setType(OPLIST.MIGICHIKATETU, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 1:    // 2筋 (居飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 1);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.IBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 2:    // 3筋 (袖飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 2);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.SODEBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 3:    // 4筋 (右四間飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 3);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.MIGISIKENBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 4:    // 5筋 (中飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 4);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.NAKBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 5:    // 6筋 (四間飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 5);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.SIKENBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 6:    // 7筋 (三間飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 6);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.SANKENBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 7:    // 8筋 (向かい飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_SENTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 6);
                            if ((fuhyou == null) || (fuhyou.y < 6)) setType(OPLIST.MUKAIBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 8:    // 9筋 (左地下鉄？)
                            setType(OPLIST.HIDARICHIKATETU, TEIGI.TEBAN_SENTE, 0);
                            break;
                    }
                }

                hisya = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Hisya);
                if (hisya != null) {
                    switch (hisya.x) {
                        case 8:    // 1筋 (右地下鉄？)
                            setType(OPLIST.MIGICHIKATETU, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 7:    // 2筋 (居飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 7);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.IBISYA, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 6:    // 3筋 (袖飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 6);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.SODEBISYA, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 5:    // 4筋 (右四間飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 5);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.MIGISIKENBISYA, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 4:    // 5筋 (中飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 4);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.NAKBISYA, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 3:    // 6筋 (四間飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 3);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.SIKENBISYA, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 2:    // 7筋 (三間飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 2);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.SANKENBISYA, TEIGI.TEBAN_GOTE, 0);
                            break;
                        case 1:    // 8筋 (向かい飛車？)
                            fuhyou = ban.OkiKo[TEIGI.TEBAN_GOTE].FirstOrDefault(k => k.type == KomaType.Fuhyou && k.x == 2);
                            if ((fuhyou == null) || (fuhyou.y > 2)) setType(OPLIST.MUKAIBISYA, TEIGI.TEBAN_SENTE, 0);
                            break;
                        case 0:    // 9筋 (左地下鉄？)
                            setType(OPLIST.HIDARICHIKATETU, TEIGI.TEBAN_GOTE, 0);
                            break;
                    }
                }
                Form1.Form1Instance.addMsg("SENKEI[" + senTeNum + ":" + mV[senTeNum].type + "]-[" + goTeNum + ":" + mV[goTeNum].type + "]");
            }

        }

        //public static void countUp(int count) {
        //    int tmpCount = 0;
        //    for (int cnt = 0; cnt < mV.Count; cnt++) {
        //        if ((mV[cnt].type == _type) && (mV[cnt].move <= count) && (tmpCount < mV[cnt].move)) {
        //            if (turn == 0) {
        //                senTeNum = cnt;
        //            } else {
        //                goTeNum = cnt;
        //            }
        //            tmpCount = mV[cnt].move;
        //        }
        //
        //
        //    }
        //
        //}

        // 指定評価値テーブルを取得
        public int[,,] getTbl() {
            return mV[0].val;
        }

        public static void setStage(int _stage) {
            if (stage < _stage) {
                stage = _stage;
            }
        }

        // 指定評価値を取得
        public static int get(KomaType type, int nx, int ny, int ox, int oy, int turn) {
            if (stage == 0) {
                if (turn == 0) {
                    return mV[0].val[(int)type - 1, ny, 8 - nx] - mV[0].val[(int)type - 1, oy, 8 - ox];
                } else {
                    return mV[0].val[(int)type - 1, 8 - ny, nx] - mV[0].val[(int)type - 1, 8 - oy, ox];
                }
            } else {
                return 0;
            }

        }


    }
}
