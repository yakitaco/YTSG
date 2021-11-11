using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kmoveDll {
    public enum OPLIST : int
    {
        None = 0,
        IBISYA = 100,            //居飛車
        IBISYA_YAGURA = 110,     //居飛車矢倉
        IBISYA_GENSHI = 111,     //原始棒銀
        IBISYA_KAKUGAWARI = 120, //角換わり
        IBISYA_KOSIKAKE   = 121, //腰掛け銀
        IBISYA_HAYAKURI   = 122, //早繰り銀
        IBISYA_YOKOFUDORI = 130, //横歩取り

        FURIBISYA = 200,   //振り飛車

        NAKBISYA    = 210,//中飛車
        SIKENBISYA  = 220,//四間飛車
        SANKENBISYA = 230,//三間飛車
        MUKAIBISYA  = 240,//向かい飛車
        MIGISIKENBISYA = 250,//右四間飛車
        SODEBISYA   = 260,//袖飛車


        KISYU = 300,          //奇襲・不明
        ONIGOROSHI = 310,     //鬼殺し
        SHINONIGOROSHI = 311, //新鬼殺し
        PACKMAN = 320,        //パックマン
        SUZICHIGAIKAKU = 330, //筋違い角
        HAYAISHIDA     = 340, //筋違い角

        MIGICHIKATETU = 350,//右地下鉄
        HIDARICHIKATETU = 360,//左地下鉄

    }
}
