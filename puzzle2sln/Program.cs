using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Xml;

namespace puzzle2sln
{
    internal class Program
    {
        static readonly Random rnd = new Random();

        class Side
        {
            public Piece piece;
            public Side origOpp;
            public Side actOpp;
            public int isUsed = 0;
            public short outputNumber = 0;

            public Side(Piece piece)
            {
                this.piece = piece;
            }
        }

        class Piece
        {
            public Side[] sides = new Side[0];

            public Piece[] permGroup; //corner, edge or center
            public int permGroupPos; //index of this piece in group

            public void RollSides(int repeat)
            {
                for (int r = 0; r < repeat; ++r)
                {
                    Side h = sides[0];
                    for (int i = 1; i < sides.Length; ++i)
                    {
                        sides[i - 1] = sides[i];
                    }
                    sides[^1] = h;
                }
            }

            public void RollSideRefs()
            {
                Side h = sides[0].actOpp;
                for (int i = 1; i < sides.Length; ++i)
                {
                    sides[i - 1].actOpp = sides[i].actOpp;
                }
                sides[^1].actOpp = h;
                ReconnectSides();
            }

            public void UnrollSideRefs()
            {
                Side h = sides[^1].actOpp;
                for (int i = sides.Length - 1; i > 0; --i)
                {
                    sides[i].actOpp = sides[i - 1].actOpp;
                }
                sides[0].actOpp = h;
                ReconnectSides();
            }

            public void ReconnectSides()
            {
                foreach (var h in sides)
                {
                    h.actOpp.actOpp = h;
                }
            }
        }


        private static void InitPermGroups(Piece[,] matrix, int size)
        {
            {
                List<Piece> centerPermGroupList = new List<Piece>();
                for (int y = 1; y < size - 1; y++)
                {
                    for (int x = 1; x < size - 1; ++x)
                    {
                        Piece p = matrix[y, x];
                        p.permGroupPos = centerPermGroupList.Count;
                        centerPermGroupList.Add(p);
                    }
                }
                var centerPermGroup = centerPermGroupList.ToArray();
                for (int y = 1; y < size - 1; y++)
                {
                    for (int x = 1; x < size - 1; ++x)
                    {
                        Piece p = matrix[y, x];
                        p.permGroup = centerPermGroup;
                    }
                }
            }

            {
                List<Piece> cornerPermGroupList = new List<Piece>();
                for (int y = 0; y < size; y += size - 1)
                {
                    for (int x = 0; x < size; x += size - 1)
                    {
                        Piece p = matrix[y, x];
                        p.permGroupPos = cornerPermGroupList.Count;
                        cornerPermGroupList.Add(p);
                    }
                }
                var cornerPermGroup = cornerPermGroupList.ToArray();
                for (int y = 0; y < size; y += size - 1)
                {
                    for (int x = 0; x < size; x += size - 1)
                    {
                        Piece p = matrix[y, x];
                        p.permGroup = cornerPermGroup;
                    }
                }
            }

            {
                List<Piece> edgePermGroupList = new List<Piece>();
                for (int i = 1; i < size - 1; ++i)
                {
                    Piece p;

                    p = matrix[0, i];
                    p.permGroupPos = edgePermGroupList.Count;
                    edgePermGroupList.Add(p);

                    p = matrix[i, size - 1];
                    p.permGroupPos = edgePermGroupList.Count;
                    edgePermGroupList.Add(p);

                    p = matrix[size - 1, i];
                    p.permGroupPos = edgePermGroupList.Count;
                    edgePermGroupList.Add(p);

                    p = matrix[i, 0];
                    p.permGroupPos = edgePermGroupList.Count;
                    edgePermGroupList.Add(p);
                }
                var edgePermGroup = edgePermGroupList.ToArray();
                for (int i = 1; i < size - 1; ++i)
                {
                    Piece p;

                    p = matrix[0, i];
                    p.permGroup = edgePermGroup;

                    p = matrix[i, size - 1];
                    p.permGroup = edgePermGroup;

                    p = matrix[size - 1, i];
                    p.permGroup = edgePermGroup;

                    p = matrix[i, 0];
                    p.permGroup = edgePermGroup;
                }
            }
        }

        private static void InitRotateEdgePieces(Piece[,] matrix, int size)
        {
            //make orientation of corners and edge pieces the same so we can easily swap them
            matrix[0, 0].RollSides(0);
            matrix[0, size - 1].RollSides(1);
            matrix[size - 1, size - 1].RollSides(0);
            matrix[size - 1, 0].RollSides(0);

            for (int i = 1; i < size - 1; ++i)
            {
                matrix[0, i].RollSides(1);
                matrix[i, size - 1].RollSides(2);
                matrix[size - 1, i].RollSides(0);
                matrix[i, 0].RollSides(0);
            }
        }

        private static Side[] InitAllSides(int size)
        {
            Piece[,] matrix = new Piece[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; ++x)
                {
                    matrix[y, x] = new Piece();
                }
            }
            List<Side> allSides = new List<Side>();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; ++x)
                {
                    if (x > 0)
                    {
                        Piece p1 = matrix[y, x - 1];
                        Piece p2 = matrix[y, x];
                        Side h1 = new Side(p1);
                        Side h2 = new Side(p2);
                        h1.origOpp = h2;
                        h1.actOpp = h2;
                        h2.origOpp = h1;
                        h2.actOpp = h1;

                        int len = p1.sides.Length;
                        Array.Resize(ref p1.sides, len + 1);
                        p1.sides[len] = h1;

                        len = p2.sides.Length;
                        Array.Resize(ref p2.sides, len + 1);
                        p2.sides[len] = h2;

                        allSides.Add(h1);
                        allSides.Add(h2);
                    }

                    if (y > 0)
                    {
                        Piece p3 = matrix[y - 1, x];
                        Piece p4 = matrix[y, x];
                        Side h3 = new Side(p3);
                        Side h4 = new Side(p4);
                        h3.origOpp = h4;
                        h3.actOpp = h4;
                        h4.origOpp = h3;
                        h4.actOpp = h3;

                        int len = p3.sides.Length;
                        Array.Resize(ref p3.sides, len + 1);
                        p3.sides[len] = h3;

                        len = p4.sides.Length;
                        Array.Resize(ref p4.sides, len + 1);
                        p4.sides[len] = h4;

                        allSides.Add(h3);
                        allSides.Add(h4);
                    }
                }
            }

            InitRotateEdgePieces(matrix, size);
            InitPermGroups(matrix, size);

            //if (rnd.Next(100) < 50)
            //{
            //    Console.Write("NONORDERED ");
            //    return allSides.ToArray();
            //}
            //else
            //{
            //    Console.Write("ORDERED ");
            return allSides.OrderBy(s => s.piece.sides.Count()).ToArray();
            //}
        }

        private static void Swap(Piece a, Piece b)
        {
            Side selfRefa = null;
            Side selfRefb = null;

            for (int i = 0; i < a.sides.Length; i++)
            {
                var ha = a.sides[i];
                var hb = b.sides[i];
                (hb.actOpp, ha.actOpp) = (ha.actOpp, hb.actOpp);

                if (a == ha.actOpp.piece) { selfRefa = ha; }
                if (b == hb.actOpp.piece) { selfRefb = hb; }
            }

            //one piece next to other
            if (selfRefa != null)
            {
                selfRefa.actOpp = selfRefb;
                selfRefb.actOpp = selfRefa;
            }

            a.ReconnectSides();
            b.ReconnectSides();
        }

        private static bool RandomSwapPiece(Piece piece, int cycleLimit)
        {
            if (piece.permGroup.Length == 1) { return false; } //for size 3
            int nextPieceIndex = rnd.Next(piece.permGroup.Length - 1);
            if (nextPieceIndex >= piece.permGroupPos) { nextPieceIndex++; }
            var nextPiece = piece.permGroup[nextPieceIndex];
            Swap(piece, nextPiece);

            foreach (var side in piece.sides)
            {
                var actSide = side;
                int cnt = 0;
                do
                {
                    cnt += 2;
                    actSide = actSide.actOpp.origOpp;
                } while (actSide != side);

                if (cnt > cycleLimit)
                {
                    Swap(piece, nextPiece);
                    return false;
                }
            }

            foreach (var side in nextPiece.sides)
            {
                var actSide = side;
                int cnt = 0;
                do
                {
                    cnt += 2;
                    actSide = actSide.actOpp.origOpp;
                } while (actSide != side);

                if (cnt > cycleLimit)
                {
                    Swap(piece, nextPiece);
                    return false;
                }
            }

            return true;
        }

        private static bool RotatePiece(Piece piece, int cycleLimit)
        {
            piece.RollSideRefs();

            foreach (var side in piece.sides)
            {
                var actSide = side;
                int cnt = 0;
                do
                {
                    cnt += 2;
                    actSide = actSide.actOpp.origOpp;
                } while (actSide != side);

                if (cnt > cycleLimit)
                {
                    piece.UnrollSideRefs();
                    return false;
                }
            }
            return true;
        }

        private static void Fix(Side side, int cycleLimit)
        {
            Piece p = side.piece;
            int range = p.sides.Length == 4 ? 3 : 2;
            do
            {
                switch (rnd.Next(range))
                {
                    case 0: if (RandomSwapPiece(p, cycleLimit)) { return; }; break;
                    case 1: if (RandomSwapPiece(side.actOpp.piece, cycleLimit)) { return; }; break;
                    case 2: if (RotatePiece(p, cycleLimit)) { return; }; break; //only for center pieces
                                                                                //case 3: RotatePiece2(p); break; //only for center pieces
                                                                                //case 4: RotatePiece3(p); break; //only for center pieces
                }
                cycleLimit += 1 + (cycleLimit >> 6);
            } while (true);
        }

        private static void Fix(Piece p, int cycleLimit)
        {
            int range = p.sides.Length == 4 ? 2 : 1;
            do
            {
                switch (rnd.Next(range))
                {
                    case 0: if (RandomSwapPiece(p, cycleLimit)) { return; }; break;
                    case 1: if (RotatePiece(p, cycleLimit)) { return; }; break; //only for center pieces
                }
                cycleLimit += 1 + (cycleLimit >> 6);
            } while (true);
        }

        private static void Validate(Side[] allSides, int cycleLimit)
        {
            bool ok;
            do
            {
                ok = true;
                foreach (var side in allSides)
                {
                    //some pieces are still connected the same way
                    if (side.actOpp == side.origOpp)
                    {
                        Fix(side, cycleLimit);
                        ok = false;
                    }
                }
            } while (!ok);
        }

        private static void Output(SolutionClass SC, int cycleLength, int metric)
        {
            using (StreamWriter sw = new StreamWriter("outp.txt", true))
            {
                sw.Write("Solution for ");
                sw.Write(SC.size);
                sw.Write(" cycleLen/metric ");
                sw.Write(cycleLength);
                sw.Write("/");
                sw.Write(metric);
                string separator1 = "";
                int ip = 0;
                foreach (var piece in SC.allPieces)
                {
                    sw.Write(separator1);
                    if (ip % SC.size == 0) { sw.WriteLine(); }
                    sw.Write("(");
                    string separator2 = "";
                    //int ih = 0;
                    foreach (var h in piece.sides)
                    {
                        sw.Write(separator2);
                        sw.Write(h.outputNumber);
                        separator2 = ", ";
                        //solution[ip, ih++] = h.outputNumber;
                    }
                    for (int i = 0; i < 4 - piece.sides.Length; ++i)
                    {
                        sw.Write(separator2);
                        sw.Write("0");
                    }
                    sw.Write(")");
                    separator1 = ", ";
                    ip++;
                }

                Console.WriteLine();
                Console.Write(DateTime.Now + " - " + cycleLength + "/" + metric);
                bool skipSolving = false;

                SC.timer.Restart();

                int solCount = skipSolving ? 0 : Solver.CountSolutions(SC.solution, SC.size, SC.SOLMIN, SC.PRETESTSEC);

                SC.timer.Stop();
                Console.Write(" ELAPSED " + SC.timer.ElapsedMilliseconds + "  ");               

                sw.WriteLine();
                sw.Write("SolCount ");
                sw.Write(solCount);

                if (!skipSolving)
                {
                    sw.Write("(");
                    sw.Write(SC.timer.ElapsedMilliseconds);
                    sw.Write("ms)");
                }

                if (skipSolving)
                {
                    sw.WriteLine(" (skipped)");
                    SC.bestCycle = Math.Min(SC.bestCycle, metric);
                }
                else if (solCount == 2)
                {
                    sw.WriteLine("!!!");
                }
                else
                {
                    sw.WriteLine();
                }
                sw.WriteLine();

                Console.WriteLine(" " + solCount + "                " + DateTime.Now);
            }
        }

        class SolutionClass
        {
            public readonly Piece[] allPieces;
            public readonly short[,] solution;
            private readonly HashSet<ulong> seen = new HashSet<ulong>();
            public readonly int size;
            public readonly Side[] allSides;
            public readonly List<Side>[] sideCycles;
            public int bestCycle;
            public int counter = 0;

            public readonly int SOLMIN;
            public readonly int PRETESTSEC;
            public readonly System.Diagnostics.Stopwatch timer = new Stopwatch();

            public (int, List<Side>) UpdateSideCycles(int usedNo)
            {
                int actCycleIndex = 0;
                List<Side> longestCycle = sideCycles[0];

                foreach (var side in allSides)
                {
                    if (side.isUsed == usedNo) { continue; }
                    var actSide = side;

                    do
                    {
                        sideCycles[actCycleIndex].Add(actSide);
                        actSide.isUsed = usedNo;
                        actSide = actSide.actOpp;

                        sideCycles[actCycleIndex].Add(actSide);
                        actSide.isUsed = usedNo;
                        actSide = actSide.origOpp;

                    } while (actSide != side);

                    if (longestCycle.Count < sideCycles[actCycleIndex].Count)
                    {
                        longestCycle = sideCycles[actCycleIndex];
                    }
                    actCycleIndex++;
                }

                return (actCycleIndex, longestCycle);
            }

            public bool UpdateSideCyclesFast(int usedNo)
            {
                foreach (var side in allSides)
                {
                    if (side.isUsed == usedNo) { continue; }
                    var actSide = side;
                    int cnt = 0;
                    do
                    {
                        cnt += 2;
                        actSide.isUsed = usedNo;
                        actSide = actSide.actOpp;
                        actSide.isUsed = usedNo;
                        actSide = actSide.origOpp;

                    } while (actSide != side);

                    if (cnt > bestCycle)
                    {
                        //for (int i = 0; i < cnt; ++i)
                        //{
                        //    actSide = actSide.actOpp.origOpp;
                        //}

                        cnt = rnd.Next(cnt);
                        int h = cnt >> 1;
                        for (int i = 0; i < h; ++i)
                        {
                            actSide = actSide.actOpp.origOpp;
                        }
                        if ((cnt & 1) == 1)
                        {
                            actSide = actSide.actOpp;
                        }

                        Fix(actSide, bestCycle);
                        return true;
                    }
                }

                return false;
            }

            private static ulong ROR(ulong x, int n)
            {
                return (((x) >> (n)) | ((x) << (64 - (n))));
            }

            public bool UpdateSolutionIsDuplicit()
            {
                //AssignNumbersToPieces
                for (int c = 0; c < sideCycles.Length;)
                {
                    var cycle = sideCycles[c++];
                    for (int i = 0; i < cycle.Count; ++i)
                    {
                        cycle[i].outputNumber = (short)((i & 1) == 0 ? c : -c);
                    }
                }

                //UpdateSolution
                seen.Clear();
                for (int ip = 0; ip < allPieces.Length; ++ip)
                {
                    var piece = allPieces[ip];
                    var sides = piece.sides;
                    ulong bin = 0;
                    for (int iside = 0; iside < sides.Length; ++iside)
                    {
                        var side = sides[iside];
                        bin |= ((ulong)side.outputNumber & 0xFFFF) << (iside << 4);
                        solution[ip, iside] = side.outputNumber;
                    }

                    //IsDuplicit                   
                    if (!seen.Add(bin)
                     || !seen.Add(ROR(bin, 16))
                     || !seen.Add(ROR(bin, 32))
                     || !seen.Add(ROR(bin, 48))
                        )
                    {
                        Fix(piece, bestCycle);
                        return true;
                    }
                }

                return false;
            }

            public SolutionClass(Side[] allSides, int size, int SOLMIN, int PRETESTSEC, int bestCycle)
            {
                this.allSides = allSides;
                this.size = size;
                this.SOLMIN = SOLMIN;
                this.PRETESTSEC = PRETESTSEC;
                this.bestCycle = bestCycle;

                allPieces = allSides
                    .Select(h => h.piece)
                    .Distinct()
                    .ToArray();
                solution = new short[size * size, 4];
                sideCycles = new List<Side>[allSides.Length / 2];
                for (int i = 0; i < sideCycles.Length; ++i)
                {
                    sideCycles[i] = new List<Side>();
                }
            }
        }

        private static void VerifyCycles(SolutionClass SC, int usedNo)
        {
            (int maxCycleIndex, var longestCycle) = SC.UpdateSideCycles(usedNo);
            int longestCycleLength = longestCycle.Count;

            //var metric = sideCycles.Take(actCycle).Sum(c => c.Count * c.Count);
            var metric = longestCycleLength;
            //bool duplicit = false;

            //if (metric <= bestCycle)
            //{
            bool duplicit = SC.UpdateSolutionIsDuplicit();
            if (!duplicit)
            {
                Output(SC, longestCycleLength, metric);

                /*
                Stopwatch st = new Stopwatch();
                st.Start();
                var cnt = CountSolutions10(SC.solution, SC.size, SC.SOLMIN, SC.PRETESTSEC);
                COMPARE_totalSolveMiliseconds += st.ElapsedMilliseconds;
                if (cnt == 2)
                {
                    COMPARE_sumSolutionsLen += longestCycleLength;
                    COMPARE_sqrSumSolutionsLen += longestCycleLength * longestCycleLength;
                    COMPARE_totalSolutions++;

                    var elapsed = (DateTime.Now - COMPARE_start).TotalMilliseconds;
                    if (elapsed > 60000)
                    {
                        Console.WriteLine(COMPARE_totalSolutions + "  " + (int)(100 * (elapsed - COMPARE_totalSolveMiliseconds) / elapsed)
                            + "%  persec: " + 1000 * (COMPARE_totalSolutions) / (elapsed - COMPARE_totalSolveMiliseconds)
                            + "  avg:" + (double)COMPARE_sumSolutionsLen / COMPARE_totalSolutions
                            + "  sqravg:" + (double)COMPARE_sqrSumSolutionsLen / COMPARE_totalSolutions
                            );

                        COMPARE_sumSolutionsLen = 0;
                        COMPARE_sqrSumSolutionsLen = 0;
                        COMPARE_totalSolutions = 0;
                        COMPARE_totalSolveMiliseconds = 0;
                        COMPARE_start = DateTime.Now;
                    }
                }*/
            }
            //}

            if (!duplicit || ((usedNo & 0xFFFF) == 0)) //for case it was duplicit, but anti duplicit fixing got trapped in a loop, we do regular fixing once in a while
            {
                var breakSide = longestCycle[rnd.Next(longestCycleLength)];
                Fix(breakSide, SC.bestCycle);
            }

            for (int i = 0; i < maxCycleIndex; ++i)
            {
                SC.sideCycles[i].Clear();
            }
        }

        enum ReadState
        {
            unknown,
            data,
        }

        /*public static long COMPARE_totalSolveMiliseconds = 0;
        public static long COMPARE_totalSolutions = 0;
        public static long COMPARE_sumSolutionsLen = 0;
        public static long COMPARE_sqrSumSolutionsLen = 0;
        public static DateTime COMPARE_start = DateTime.Now;*/

        static void Main(string[] args)
        {
            /*
            //solve solutions again with longer time limit
            using (StreamWriter sw = new StreamWriter("outp2.txt", true))
            using (StreamReader sr = new StreamReader("outp.txt"))
            {
                StringBuilder buffer = new StringBuilder();
                bool data = false;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (data)
                    {
                        if (line.StartsWith("SolCount"))
                        {
                            sw.Write(line);
                            data = false;

                            buffer.Replace("(", "").Replace(" ", "");
                            var pieces = buffer.ToString().Split(")", StringSplitOptions.RemoveEmptyEntries);
                            int[,] solution = new int[pieces.Length, 4];
                            for (int i = 0; i < pieces.Length; i++)
                            {
                                var piece = pieces[i];
                                var edges = piece.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
                                solution[i, 0] = edges[0];
                                solution[i, 1] = edges[1];
                                solution[i, 2] = edges[2];
                                solution[i, 3] = edges[3];
                            }
                            Stopwatch timer = Stopwatch.StartNew();
                            int cnt = Solver.CountSolutions(solution, (int)Math.Sqrt(solution.GetLength(0)), 400, 60);
                            sw.WriteLine(" " + cnt + "(" + timer.ElapsedMilliseconds + "ms)");
                        }
                        else
                        {
                            buffer.Append(line);
                            sw.WriteLine(line);
                        }
                    }
                    else
                    {
                        if (line.StartsWith("Solution"))
                        {
                            data = true;
                            buffer.Clear();
                        }
                        sw.WriteLine(line);
                    }
                }
            }
            Console.WriteLine("KONEC ");// + CNT);
            Console.ReadKey();
            return;*/


            int size = 5;
            int bestCycle = 100;

            int PRETESTSEC = 10;
            int SOLMIN = 20;

            var allSides = InitAllSides(size);
            SolutionClass SC = new SolutionClass(allSides, size, SOLMIN, PRETESTSEC, bestCycle);
            Console.WriteLine("Size " + size + " maxlen " + bestCycle + " solving time " + PRETESTSEC + "s/" + SOLMIN + "min " + DateTime.Now);
            DateTime? endTime = null;
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out int res))
                {
                    endTime = DateTime.Now.AddHours(res);
                    Console.WriteLine("Ends at " + endTime);
                }
            }

            int counter = 0;
            while (true)
            {
                Validate(allSides, SC.bestCycle);
                if (!SC.UpdateSideCyclesFast(++counter))
                {
                    VerifyCycles(SC, ++counter);
                }

                if (counter > 1000000)//randomize all
                {
                    counter = 0;
                    foreach (var side in allSides)
                    {
                        Fix(side, SC.bestCycle);
                        side.isUsed = 0;
                    }
                    Console.Write("@");
                    if (endTime != null && endTime < DateTime.Now) { return; }
                }
            };
        }
    }
}
