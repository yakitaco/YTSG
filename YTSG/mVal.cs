﻿using kmoveDll;
using System.Collections.Generic;

namespace YTSG {
    class mVal {
        OPLIST type;
        int move;
        public int[,,] val;

        static List<mVal> mV;
        static int senTeNum = 0;
        static int goTeNum = 0;

        static int[,] allZero = new int[9,9]
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }};

        static mVal() {
            mV = new List<mVal>();

            //初手～5手
            mVal tmp = new mVal(OPLIST.None, 0);
            tmp.val = new int[14, 9, 9]  {
                { { 0, 0, 0, 0, 0, 0, 0,60, 0 }, // 歩
                  { 0, 0, 0, 0, 0, 0, 0,50, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,40, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,30, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,20, 0 },
                  { 0, 0,10, 5, 3, 0, 0,10, 0 },
                  { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 桂
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 銀
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0,70,70,70 },
                  { 0, 0, 0, 0, 0, 0,60,50,60 },
                  { 0, 0, 0, 0, 0, 0,50,40,50 },
                  { 0, 0, 0, 0, 0, 0, 0,30, 0 },
                  { 0, 0,20, 0, 0, 0,20,20, 0 },
                  { 0, 0, 2,10, 0, 0,10, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { {  0,  0,  0,  0,  0,  0,  0, 70,  0 }, // 飛
                  {  0,  0,  0,  0,  0,  0,  0, 60,  0 },
                  {  0,  0,  0,  0,  0,  0,  0, 50,  0 },
                  {-10,-10,-10,-10,-10,-10,-10,  0,  0 },
                  {-10,-10,-10,-10,-10,-10,-10,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {-10,-10,-10,-10,-10,-10,-10,  0,-10 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 角
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0,10, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0,15, 0, 0, 0, 0, 0 },
                  { 0, 0, 5, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 金
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0,10, 0, 0, 0, 0, 0 },
                  { 0, 0,10, 0, 5, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 王
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0,20, 7, 5, 0, 0, 0, 0, 0 },
                  { 0, 0, 7, 5, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // と
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成桂
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成銀
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 竜王
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 竜馬
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            };
            mV.Add(tmp);

            //居飛車
            tmp = new mVal(OPLIST.None, 0);
            tmp.val = new int[14, 9, 9]  {
                { { 0, 0, 0, 0, 0, 0, 0,60, 0 }, // 歩
                  { 0, 0, 0, 0, 0, 0, 0,50, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,40, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,30, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,20, 0 },
                  { 0, 0,10, 5, 3, 0, 0,10, 0 },
                  { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 桂
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 銀
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0,70,70,70 },
                  { 0, 0, 0, 0, 0, 0,60,50,60 },
                  { 0, 0, 0, 0, 0, 0,50,40,50 },
                  { 0, 0, 0, 0, 0, 0, 0,30, 0 },
                  { 0, 0,20, 0, 0, 0,20,20, 0 },
                  { 0, 0, 2,10, 0, 0,10, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { {  0,  0,  0,  0,  0,  0,  0, 70,  0 }, // 飛
                  {  0,  0,  0,  0,  0,  0,  0, 60,  0 },
                  {  0,  0,  0,  0,  0,  0,  0, 50,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {-10,-10,-10,-10,-10,-10,-10,  0,-10 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 角
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0,10, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0,15, 0, 0, 0, 0, 0 },
                  { 0, 0, 5, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 金
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0,10, 0, 0, 0, 0, 0 },
                  { 0, 0,10, 0, 5, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 王
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0,20, 7, 5, 0, 0, 0, 0, 0 },
                  { 0, 0, 7, 5, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // と
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成桂
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成銀
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 竜王
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 竜馬
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            };
            mV.Add(tmp);

            // 向飛車
            tmp = new mVal(OPLIST.MUKAIBISYA, 0);
            tmp.val = new int[14, 9, 9]  {
                { { 0, 0,60, 0, 0, 0, 0, 0, 0 }, // 歩
                  { 0, 0,50, 0, 0, 0, 0, 0, 0 },
                  { 0, 0,40, 0, 0, 0, 0, 0, 0 },
                  { 0, 0,30, 0, 0, 0, 0, 0, 0 },
                  { 0, 0,20, 0, 0, 0, 0, 0, 0 },
                  { 0, 0,10, 0, 0, 0, 0, 0, 0 },
                  {10,10, 0,10,10,10,10,10,10 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { {  0,  0,  0,  0,  0,  0,  0,  0,  0 }, // 桂
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  { -5,  0, -5,  0,  0,  0,  3,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                  {  0,  0,  0,  0,  0,  0,  0,  0,  0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 銀
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0,70,70,70 },
                  { 0, 0, 0, 0, 0, 0,60,50,60 },
                  { 0, 0, 0, 0, 0, 0,50,40,50 },
                  { 0, 0, 0, 0, 0, 0, 0,30, 0 },
                  { 0, 0, 0, 0, 0, 0,20,20, 0 },
                  { 0, 0, 0, 0, 0, 0,10, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0,50, 0 }, // 飛
                  { 0, 0, 0, 0, 0, 0, 0,50, 0 },
                  { 0, 0, 0, 0, 0, 0, 0,50, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 角
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 金
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 王
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0,10, 0, 0, 0, 0, 0 },
                  { 0, 0, 0,10, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // と
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成香
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成桂
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 成銀
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 竜王
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
                { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 竜馬
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            };
            mV.Add(tmp);

        }

        mVal(OPLIST _type, int _move) {
            type = _type;
            move = _move;
        }

        public static void reset() {
            senTeNum = 0;
            goTeNum = 0;
        }

        public static void setType(OPLIST _type, int turn) {

        }

        public static void countUp(int count) {

        }

        // 指定評価値テーブルを取得
        public int[,,] getTbl() {
            return mV[0].val;
        }

        // 指定評価値を取得
        public static int get(KomaType type, int nx, int ny, int ox, int oy, int turn) {
            if (turn == 0) {
                return mV[0].val[(int)type - 1, ny, 8 - nx] - mV[0].val[(int)type - 1, oy, 8 - ox];
            } else {
                return mV[0].val[(int)type - 1, 8 - ny, nx] - mV[0].val[(int)type - 1, 8 - oy, ox];
            }

        }


    }
}
