using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace puzzle2sln
{
    internal class Solver
    {
        private const uint EMPTY = 0x80808080;

        private static uint ROR(uint x, int n)
        {
            return (((x) >> (n)) | ((x) << (32 - (n))));
        }

        //private static uint ROL(uint x, int n)
        //{
        //    return (((x) << (n)) | ((x) >> (32 - (n))));
        //}

        private static int UnByte(uint b)
        {
            if (b >= 0x80)
            {
                return (int)(b | 0xFFFFFF00);
            }
            return (int)b;
        }
       
        private static IEnumerable<uint> GenerateEmpty(uint piece)
        {
            yield return piece;

            //if ((piece & 0xFFFF0000) == piece
            // || (piece & 0xFF0000FF) == piece
            // || (piece & 0x0000FFFF) == piece
            // || (piece & 0x00FFFF00) == piece
            //    )
            //{
            //    yield return piece;
            //}

            if ((piece & 0xFFFFFF00) != piece) { yield return (piece & 0xFFFFFF00) | 0x00000080; }
            if ((piece & 0xFFFF00FF) != piece) { yield return (piece & 0xFFFF00FF) | 0x00008000; }
            if ((piece & 0xFF00FFFF) != piece) { yield return (piece & 0xFF00FFFF) | 0x00800000; }
            if ((piece & 0x00FFFFFF) != piece) { yield return (piece & 0x00FFFFFF) | 0x80000000; }

            if ((piece & 0xFFFFFF00) != piece && (piece & 0xFFFF00FF) != piece) { yield return (piece & 0xFFFF0000) | 0x00008080; }
            if ((piece & 0xFFFF00FF) != piece && (piece & 0xFF00FFFF) != piece) { yield return (piece & 0xFF0000FF) | 0x00808000; }
            if ((piece & 0xFF00FFFF) != piece && (piece & 0x00FFFFFF) != piece) { yield return (piece & 0x0000FFFF) | 0x80800000; }
            if ((piece & 0x00FFFFFF) != piece && (piece & 0xFFFFFF00) != piece) { yield return (piece & 0x00FFFF00) | 0x80000080; }

            //not used in this order of filling
            //if ((piece & 0xFFFFFF00) != piece && (piece & 0xFF00FFFF) != piece) { yield return (piece & 0xFF00FF00) | 0x00800080; }
            //if ((piece & 0xFFFF00FF) != piece && (piece & 0x00FFFFFF) != piece) { yield return (piece & 0x00FF00FF) | 0x80008000; }

            if ((piece & 0xFFFFFF00) != piece && (piece & 0xFFFF00FF) != piece && (piece & 0xFF00FFFF) != piece) { yield return (piece & 0xFF000000) | 0x00808080; }
            if ((piece & 0xFFFF00FF) != piece && (piece & 0xFF00FFFF) != piece && (piece & 0x00FFFFFF) != piece) { yield return (piece & 0x000000FF) | 0x80808000; }
            if ((piece & 0xFF00FFFF) != piece && (piece & 0x00FFFFFF) != piece && (piece & 0xFFFFFF00) != piece) { yield return (piece & 0x0000FF00) | 0x80800080; }
            if ((piece & 0x00FFFFFF) != piece && (piece & 0xFFFFFF00) != piece && (piece & 0xFFFF00FF) != piece) { yield return (piece & 0x00FF0000) | 0x80008080; }
        }

        private static uint Invert(uint piece)
        {
            uint a0 = ((~(piece)) + 1) & 0xFF;
            uint a1 = ((~(piece >> 8)) + 1) & 0xFF;
            uint a2 = ((~(piece >> 16)) + 1) & 0xFF;
            uint a3 = ((~(piece >> 24)) + 1) & 0xFF;

            return a2 | (a3 << 8) | (a0 << 16) | (a1 << 24);
        }

        private static IEnumerable<uint> GenerateRotate(uint piece)
        {
            yield return piece;
            yield return ROR(piece, 8);
            yield return ROR(piece, 16);
            yield return ROR(piece, 24);
        }

        private static (Dictionary<uint, short>, short[]) BuildDictionary(uint[] pieceList)
        {
            var ret = new Dictionary<uint, short>();
            var MatchPieces = new List<short>();

            foreach (var group in pieceList
                .SelectMany(p => GenerateRotate(p))
                .Select((p, i) => (p, i))
                .SelectMany(x => GenerateEmpty(x.p).Select(p => (p, x.i)))
                .GroupBy(x => Invert(x.p)))
            {
                ret[group.Key] = (short)MatchPieces.Count;

                MatchPieces.AddRange(group.Select(x => (short)(x.i << 3)));
                MatchPieces.Add(-1);
            }
            return (ret, MatchPieces.ToArray());
        }

        private static uint[] GetPieces(int[,] solution)
        {
            List<uint> Pieces = new List<uint>();

            for (int i = 0; i < solution.GetLength(0); ++i)
            {
                uint p0 = (uint)(solution[i, 0] & 0xFF);
                uint p1 = (uint)(solution[i, 1] & 0xFF);
                uint p2 = (uint)(solution[i, 2] & 0xFF);
                uint p3 = (uint)(solution[i, 3] & 0xFF);

                uint p = p0 | (p1 << 8) | (p2 << 16) | (p3 << 24);

                Pieces.Add(p);
            }

            return Pieces.ToArray();
        }

        private static bool IsDuplicit(int[,] solution, int size)
        {
            HashSet<(int, int, int, int)> seen = new HashSet<(int, int, int, int)>();
            for (int i = 0; i < size * size; ++i)
            {
                int a0 = solution[i, 0];
                int a1 = solution[i, 1];
                int a2 = solution[i, 2];
                int a3 = solution[i, 3];

                for (int j = 0; j < 4; ++j)
                {
                    if (!seen.Add((a0, a1, a2, a3))) { return true; }
                    int tmp = a0;
                    a0 = a1;
                    a1 = a2;
                    a2 = a3;
                    a3 = tmp;
                }
            }
            return false;
        }

        private class State
        {
            public uint[] matrix;
            public Dictionary<uint, short> MatchPiecesStart;
            public short[] MatchPieceIndexes;
            public uint[] Pieces;
            public bool[] isPieceUsed;
            public short[] posArr;
            public int finalPos;
            public readonly int size;
            public readonly int lineSize; //size+2
            public int solcnt;
            public int cyclcount;
            public long totalCyclCount;
            public DateTime endTime;

            public State(int size)
            {
                this.size = size;
                this.lineSize = size + 2;
                GetPosArray();
            }

            enum Smer { dolu, doleva, doprava, nahoru };

            private void GetPosArray()
            {
                int atx = 0;
                int aty = 0;
                Smer smer = Smer.doleva;
                posArr = new short[size * size];
                for (int i = 0; i < posArr.Length; ++i)
                {
                    posArr[i] = (short)Index2D(aty + 1, atx + 1);

                    switch (smer)
                    {
                        case Smer.doleva:
                            if (--atx < 0)
                            {
                                atx = 0;
                                aty++;
                                smer = Smer.doprava;
                            }
                            break;
                        case Smer.doprava:
                            if (++atx == aty)
                            {
                                smer = Smer.nahoru;
                            }
                            break;
                        case Smer.nahoru:
                            if (--aty < 0)
                            {
                                aty = 0;
                                atx++;
                                smer = Smer.dolu;
                            }
                            break;
                        case Smer.dolu:
                            if (++aty == atx)
                            {
                                smer = Smer.doleva;
                            }
                            break;
                    }
                }
            }

            public int Index2D(int y, int x)
            {
                return y * lineSize + x;
            }

            public void PrintState(int atX, int atY)
            {
                string[,] outp = new string[size * 4, size * 4];

                for (int y = 0; y < size * 4; ++y)
                {
                    for (int x = 0; x < size * 4; ++x)
                    {
                        outp[y, x] = "";
                    }
                }

                for (int y = 0; y < size; ++y)
                {
                    for (int x = 0; x < size; ++x)
                    {
                        outp[y * 4 + 1, x * 4 + 0] = (UnByte(matrix[Index2D(y + 1, x + 1)] & 0xFF)).ToString();
                        outp[y * 4 + 0, x * 4 + 1] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 8) & 0xFF)).ToString();
                        outp[y * 4 + 1, x * 4 + 2] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 16) & 0xFF)).ToString();
                        outp[y * 4 + 2, x * 4 + 1] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 24) & 0xFF)).ToString();


                        outp[y * 4 + 1, x * 4 + 1] = (x == atX - 1 && y == atY - 1) ? "@@@@" : "X";
                        //outp[y * 4 + 1, x * 4 + 1] = (S.matrix[y, x]).ToString(); ;
                    }
                }

                for (int y = 0; y < size * 4; ++y)
                {
                    for (int x = 0; x < size * 4; ++x)
                    {
                        Console.Write("\t");
                        Console.Write(outp[y, x]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            public void PrintItem(short pieceI, int pI)
            {
                var pc = ROR(Pieces[pI], pieceI & 0x1F);
                Console.Write("(");
                Console.Write((UnByte(pc & 0xFF)).ToString());
                Console.Write(", ");
                Console.Write((UnByte((pc >> 8) & 0xFF)).ToString());
                Console.Write(", ");
                Console.Write((UnByte((pc >> 16) & 0xFF)).ToString());
                Console.Write(", ");
                Console.Write((UnByte((pc >> 24) & 0xFF)).ToString());
                Console.Write(") ");
                if (isPieceUsed[pI])
                {
                    Console.WriteLine("USED");
                }
                else
                {
                    Console.WriteLine("GO");
                }

            }

            public void SaveState()
            {
                using (StreamWriter sw2 = new StreamWriter("outpDetail-" + solcnt + ".txt", true))
                {
                    string[,] outp = new string[size * 4, size * 4];

                    for (int y = 0; y < size * 4; ++y)
                    {
                        for (int x = 0; x < size * 4; ++x)
                        {
                            outp[y, x] = "";
                        }
                    }

                    for (int y = 0; y < size; ++y)
                    {
                        for (int x = 0; x < size; ++x)
                        {
                            outp[y * 4 + 1, x * 4 + 0] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 0) & 0xFF)).ToString();
                            outp[y * 4 + 0, x * 4 + 1] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 8) & 0xFF)).ToString();
                            outp[y * 4 + 1, x * 4 + 2] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 16) & 0xFF)).ToString();
                            outp[y * 4 + 2, x * 4 + 1] = (UnByte((matrix[Index2D(y + 1, x + 1)] >> 24) & 0xFF)).ToString();


                            outp[y * 4 + 1, x * 4 + 1] = "X";
                            //outp[y * 4 + 1, x * 4 + 1] = (S.matrix[y, x]).ToString(); ;
                        }
                    }

                    for (int y = 0; y < size * 4; ++y)
                    {
                        for (int x = 0; x < size * 4; ++x)
                        {
                            sw2.Write("\t");
                            sw2.Write(outp[y, x]);
                        }
                        sw2.WriteLine();
                    }
                    sw2.WriteLine();
                }
            }

            public void CountSolutions(int posI)
            {
                if (posI >= finalPos)
                {
                    if (posI == posArr.Length)
                    {
                        SaveState();
                        if (++solcnt == 3)
                        {
                            solcnt = 777; //more than 2 solutions
                            finalPos = 0; //quit testing
                        }
                    }
                    return;
                }

                if (++cyclcount > 50000000)//200000000   25M=1sec
                {
                    totalCyclCount += cyclcount;
                    cyclcount = 0;
                    if (DateTime.Now > endTime)
                    {
                        solcnt = 999; //timed out
                        finalPos = 0; //quit testing
                        return;
                    }
                }

                int at = posArr[posI];
                uint s0 = (matrix[at - 1]) & 0x00FF0000;
                uint s1 = (matrix[at - lineSize]) & 0xFF000000;
                uint s2 = (matrix[at + 1]) & 0x000000FF;
                uint s3 = (matrix[at + lineSize]) & 0x0000FF00;
                uint s = s0 | s1 | s2 | s3;

                //PrintState(S, atX,atY);

                if (MatchPiecesStart.TryGetValue(s, out short index))
                {
                    var pieceI = MatchPieceIndexes[index];
                    do
                    {
                        int pI = pieceI >> 5;
                        //PrintItem(S, pieceI, pI);                    

                        if (!isPieceUsed[pI])
                        {
                            isPieceUsed[pI] = true;
                            matrix[at] = ROR(Pieces[pI], pieceI & 0x1F);
                            CountSolutions(posI + 1);
                            isPieceUsed[pI] = false;
                        }
                        pieceI = MatchPieceIndexes[++index];
                        //PrintState(S, atX, atY);                   

                    } while (pieceI >= 0);
                    matrix[at] = EMPTY;
                }

                //Console.WriteLine("NO PIECE");            
            }
        }

        public static int CountSolutions(int[,] solution, int size, int minutesLimit, int secondsPretestLimit)
        {
            foreach (int e in solution)
            {
                if (e < -127 || e > 127) { return -1; } // out of range for this solver
            }
            if (IsDuplicit(solution, size)) { return 888; } // duplicit pieces -> more than 2 solutions
            var totalEndTime = DateTime.Now.AddMinutes(minutesLimit);

            var S = new State(size)
            {
                solcnt = 0,
                cyclcount = 0,
                totalCyclCount = 0,
                Pieces = GetPieces(solution)
            };

            (S.MatchPiecesStart, S.MatchPieceIndexes) = BuildDictionary(S.Pieces);
            S.isPieceUsed = new bool[S.Pieces.Length];

            S.matrix = new uint[S.lineSize * S.lineSize];
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    S.matrix[S.Index2D(y + 1, x + 1)] = EMPTY;
                }
            }

            int at = S.Index2D(1, 1);
            uint s0 = (S.matrix[at - 1]) & 0x00FF0000;
            uint s1 = (S.matrix[at - S.lineSize]) & 0xFF000000;
            uint s2 = (S.matrix[at + 1]) & 0x000000FF;
            uint s3 = (S.matrix[at + S.lineSize]) & 0x0000FF00;
            uint s = s0 | s1 | s2 | s3;

            if (S.MatchPiecesStart.TryGetValue(s, out short index))
            {
                int bestCorner = -1;
                long bestCornerCount = long.MaxValue;
                if (size > 6)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        var pieceI = S.MatchPieceIndexes[index + i];
                        int pI = pieceI >> 5;
                        S.isPieceUsed[pI] = true;
                        S.matrix[at] = ROR(S.Pieces[pI], pieceI & 0x1F);
                        S.finalPos = 30;
                        S.totalCyclCount = 0;
                        S.cyclcount = 0;
                        S.solcnt = 0;
                        S.endTime = DateTime.Now.AddSeconds(secondsPretestLimit);

                        //var st = new System.Diagnostics.Stopwatch();
                        //st.Start();
                        S.CountSolutions(1);
                        S.isPieceUsed[pI] = false;
                        S.totalCyclCount += S.cyclcount;
                        //Console.WriteLine("Totalcnt " + S.totalCyclCount + " elapsed " + st.ElapsedMilliseconds + " solcnt " + S.solcnt);

                        if (S.totalCyclCount < bestCornerCount && S.solcnt < 999)
                        {
                            bestCornerCount = S.totalCyclCount;
                            bestCorner = i;
                        }
                    }
                }
                else
                {
                    bestCorner = 0;//just any
                }

                if (bestCorner == -1) { return 666; } //all corners timed out on quick test

                var pieceI2 = S.MatchPieceIndexes[index + bestCorner];
                int pI2 = pieceI2 >> 5;
                S.isPieceUsed[pI2] = true;
                S.matrix[at] = ROR(S.Pieces[pI2], pieceI2 & 0x1F);
                S.finalPos = S.posArr.Length;
                S.totalCyclCount = 0;
                S.cyclcount = 0;
                S.solcnt = 0;
                S.endTime = totalEndTime;
                S.CountSolutions(1);
            }
            else
            {
                return -1; //no corner piece
            }

            //Console.Write(" CYCLCOUNT " + S.cyclcount + " ");

            return S.solcnt;
        }       
    }
}
