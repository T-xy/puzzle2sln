        private static void RotatePiece2(Piece piece)
        {
            piece.RollSideRefs();
            piece.RollSideRefs();
            piece.ReconnectSides();
        }

        private static void RotatePiece3(Piece piece)
        {
            piece.RollSideRefs();
            piece.RollSideRefs();
            piece.RollSideRefs();
            piece.ReconnectSides();
        }


            /*private void GetPosArray()
            {
                int atx = 0;
                int aty = 0;
                Smer smer = Smer.doprava;
                posArr = new short[size * size];
                for (int i = 0; i < posArr.Length; ++i)
                {
                    posArr[i] = (short)index2D(aty + 1, atx + 1);

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
                            if (++atx == size)
                            {
                                atx--;
                                aty++;
                                smer = Smer.doleva;
                            }
                            break;                        
                    }
                }
            }*/

        private static int CountSolutions(int[,,] matrix, int[,] solution, bool[] used, int size, int atX, int atY, int atP)
        {
            if (atY == size)
            {
                using (StreamWriter sw2 = new StreamWriter("outpDetail.txt", true))
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
                            outp[y * 4 + 1, x * 4 + 0] = (-matrix[y, x, 0]).ToString();
                            outp[y * 4 + 0, x * 4 + 1] = (-matrix[y, x, 1]).ToString();
                            outp[y * 4 + 1, x * 4 + 2] = (matrix[y, x + 1, 0]).ToString();
                            outp[y * 4 + 2, x * 4 + 1] = (matrix[y + 1, x, 1]).ToString();

                            outp[y * 4 + 1, x * 4 + 1] = "X";
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

                return 1;
            }

            int ret = 0;

            for (int i = atP; i < size * size; ++i)
            {
                if (used[i]) { continue; }

                int m0 = matrix[atY, atX, 0];
                int m1 = matrix[atY, atX, 1];
                int m2 = matrix[atY, atX + 1, 0];
                int m3 = matrix[atY + 1, atX, 1];

                for (int rot = 0; rot < 4; ++rot)
                {
                    int s0 = solution[i, 0];
                    int s1 = solution[i, 1];
                    int s2 = solution[i, 2];
                    int s3 = solution[i, 3];

                    if (rot == 0 &&
                        (m1 != -s0 && m1 != -s1 && m1 != -s2 && m1 != -s3
                      || m0 != -s0 && m0 != -s1 && m0 != -s2 && m0 != -s3
                        ))
                    {
                        break;
                    }

                    if ((m0 == -s0)
                     && (m1 == -s1)
                     && (m2 == int.MinValue || m2 == -s2)
                     && (m3 == int.MinValue || m3 == -s3))
                    {
                        matrix[atY, atX + 1, 0] = s2;
                        matrix[atY + 1, atX, 1] = s3;
                        used[i] = true;

                        int atX2 = atX + 1;
                        int atY2 = atY;
                        if (atX2 == size)
                        {
                            atX2 = 0;
                            atY2++;
                        }

                        ret += CountSolutions(matrix, solution, used, size, atX2, atY2, 0);

                        used[i] = false;
                        matrix[atY, atX + 1, 0] = m2;
                        matrix[atY + 1, atX, 1] = m3;
                    }

                    solution[i, 0] = s1;
                    solution[i, 1] = s2;
                    solution[i, 2] = s3;
                    solution[i, 3] = s0;
                }
            }
            return ret;
        }

        private static int CountSolutions(int[,] solution, int size)
        {
            int[,,] matrix = new int[size + 1, size + 1, 2];
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    if (x > 0)
                    {
                        matrix[y, x, 0] = int.MinValue; //empty
                    }
                    if (y > 0)
                    {
                        matrix[y, x, 1] = int.MinValue; //empty
                    }
                }
            }

            bool[] used = new bool[size * size];

            return CountSolutions(matrix, solution, used, size, 0, 0, size * size - 1);
        }














        private static int CountSolutions2(int[,,] matrix,
            int[,] cornerSolution, int[,] edgeSolution, int[,] insideSolution,
            int size, int atX, int atY, int atP)
        {
            if (atY == size)
            {
                using (StreamWriter sw2 = new StreamWriter("outpDetail.txt", true))
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
                            outp[y * 4 + 1, x * 4 + 0] = (-matrix[y, x, 0]).ToString();
                            outp[y * 4 + 0, x * 4 + 1] = (-matrix[y, x, 1]).ToString();
                            outp[y * 4 + 1, x * 4 + 2] = (matrix[y, x + 1, 0]).ToString();
                            outp[y * 4 + 2, x * 4 + 1] = (matrix[y + 1, x, 1]).ToString();

                            outp[y * 4 + 1, x * 4 + 1] = "X";
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

                return 1;
            }

            int ret = 0;

            int[,] solution;
            bool sY = atX == 0 || atX == size - 1;
            bool sX = atY == 0 || atY == size - 1;
            if (sX)
            {
                solution = sY ? cornerSolution : edgeSolution;
            }
            else
            {
                solution = sY ? edgeSolution : insideSolution;
            }

            int len = solution.GetLength(0);
            for (int i = atP; i < len; ++i)
            {
                if (solution[i, 4] > 0) { continue; }

                int m0 = matrix[atY, atX, 0];
                int m1 = matrix[atY, atX, 1];
                int m2 = matrix[atY, atX + 1, 0];
                int m3 = matrix[atY + 1, atX, 1];

                for (int rot = 0; rot < 4; ++rot)
                {
                    int s0 = solution[i, 0];
                    int s1 = solution[i, 1];
                    int s2 = solution[i, 2];
                    int s3 = solution[i, 3];

                    if (rot == 0 &&
                        (m1 != -s0 && m1 != -s1 && m1 != -s2 && m1 != -s3
                      || m0 != -s0 && m0 != -s1 && m0 != -s2 && m0 != -s3
                        ))
                    {
                        break;
                    }

                    if ((m0 == -s0)
                     && (m1 == -s1)
                     && (m2 == int.MinValue || m2 == -s2)
                     && (m3 == int.MinValue || m3 == -s3))
                    {
                        matrix[atY, atX + 1, 0] = s2;
                        matrix[atY + 1, atX, 1] = s3;
                        solution[i, 4] = 1;

                        int atX2 = atX + 1;
                        int atY2 = atY;
                        if (atX2 == size)
                        {
                            atX2 = 0;
                            atY2++;
                        }

                        ret += CountSolutions2(matrix, cornerSolution, edgeSolution, insideSolution, size, atX2, atY2, 0);

                        solution[i, 4] = 0;
                        matrix[atY, atX + 1, 0] = m2;
                        matrix[atY + 1, atX, 1] = m3;
                    }

                    solution[i, 0] = s1;
                    solution[i, 1] = s2;
                    solution[i, 2] = s3;
                    solution[i, 3] = s0;
                }
            }
            return ret;
        }

        private static int CountSolutions2(int[,] solution, int size)
        {
            int[,,] matrix = new int[size + 1, size + 1, 2];

            int[,] cornerSolution = new int[4, 5];
            int[,] edgeSolution = new int[(size - 2) * 4, 5];
            int[,] insideSolution = new int[(size - 2) * (size - 2), 5];

            int ic = 0;
            int ie = 0;
            int ii = 0;
            for (int s = 0; s < size * size; ++s)
            {
                int count = (solution[s, 0] == 0 ? 1 : 0)
                    + (solution[s, 1] == 0 ? 1 : 0)
                    + (solution[s, 2] == 0 ? 1 : 0)
                    + (solution[s, 3] == 0 ? 1 : 0);

                if (count == 0)
                {
                    insideSolution[ii, 0] = solution[s, 0];
                    insideSolution[ii, 1] = solution[s, 1];
                    insideSolution[ii, 2] = solution[s, 2];
                    insideSolution[ii++, 3] = solution[s, 3];
                }
                else if (count == 1)
                {
                    edgeSolution[ie, 0] = solution[s, 0];
                    edgeSolution[ie, 1] = solution[s, 1];
                    edgeSolution[ie, 2] = solution[s, 2];
                    edgeSolution[ie++, 3] = solution[s, 3];
                }
                else
                {
                    cornerSolution[ic, 0] = solution[s, 0];
                    cornerSolution[ic, 1] = solution[s, 1];
                    cornerSolution[ic, 2] = solution[s, 2];
                    cornerSolution[ic++, 3] = solution[s, 3];
                }
            }

            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    if (x > 0)
                    {
                        matrix[y, x, 0] = int.MinValue; //empty
                    }
                    if (y > 0)
                    {
                        matrix[y, x, 1] = int.MinValue; //empty
                    }
                }
            }


            return CountSolutions2(matrix, cornerSolution, edgeSolution, insideSolution, size, 0, 0, 3);
        }








        private static int CountSolutions3(int[,,] matrix,
           int[,] cornerSolution, int[,] edgeSolution, int[,] insideSolution,
           int size, int atX, int atY, int atP)
        {
            if (atX == size - 1)
            {
                return 1;
            }

            int ret = 0;

            int[,] solution;
            bool sY = atX == 0 || atX == size - 1;
            bool sX = atY == 0 || atY == size - 1;
            if (sX)
            {
                solution = sY ? cornerSolution : edgeSolution;
            }
            else
            {
                solution = sY ? edgeSolution : insideSolution;
            }

            int len = solution.GetLength(0);
            for (int i = atP; i < len; ++i)
            {
                if (solution[i, 4] > 0) { continue; }

                int m0 = matrix[atY, atX, 0];
                int m1 = matrix[atY, atX, 1];
                int m2 = matrix[atY, atX + 1, 0];
                int m3 = matrix[atY + 1, atX, 1];

                for (int rot = 0; rot < 4; ++rot)
                {
                    int s0 = solution[i, 0];
                    int s1 = solution[i, 1];
                    int s2 = solution[i, 2];
                    int s3 = solution[i, 3];

                    if ((m0 == int.MinValue || m0 == -s0)
                     && (m1 == int.MinValue || m1 == -s1)
                     && (m2 == int.MinValue || m2 == -s2)
                     && (m3 == int.MinValue || m3 == -s3))
                    {
                        matrix[atY, atX + 1, 0] = s2;
                        matrix[atY + 1, atX, 1] = s3;
                        solution[i, 4] = 1;

                        int atX2 = atX;
                        int atY2 = atY;

                        if (atY2 == 0)
                        {
                            atX2++;
                            if (atX2 == size)
                            {
                                atX2 = 0;
                                atY2 = 1;
                            }
                        }
                        else if (atX2 == 0)
                        {
                            atY2++;
                            if (atY2 == size)
                            {
                                atX2 = 1;
                                atY2 = size - 1;
                            }
                        }
                        else if (atY2 == size - 1)
                        {
                            atX2++;
                            if (atX2 == size)
                            {
                                atX2 = size - 1;
                                atY2 = 1;
                            }
                        }
                        else
                        {
                            atY2++;
                            if (atY2 == size - 1)
                            {
                                atY2 = size;
                            }
                        }


                        ret += CountSolutions3(matrix, cornerSolution, edgeSolution, insideSolution, size, atX2, atY2, 0);

                        solution[i, 4] = 0;
                        matrix[atY, atX + 1, 0] = m2;
                        matrix[atY + 1, atX, 1] = m3;
                    }

                    solution[i, 0] = s1;
                    solution[i, 1] = s2;
                    solution[i, 2] = s3;
                    solution[i, 3] = s0;
                }
            }
            return ret;
        }

        private static int CountSolutions3(int[,] solution, int size)
        {
            int[,,] matrix = new int[size + 1, size + 1, 2];

            int[,] cornerSolution = new int[4, 5];
            int[,] edgeSolution = new int[(size - 2) * 4, 5];
            int[,] insideSolution = new int[(size - 2) * (size - 2), 5];

            int ic = 0;
            int ie = 0;
            int ii = 0;
            for (int s = 0; s < size * size; ++s)
            {
                int count = (solution[s, 0] == 0 ? 1 : 0)
                    + (solution[s, 1] == 0 ? 1 : 0)
                    + (solution[s, 2] == 0 ? 1 : 0)
                    + (solution[s, 3] == 0 ? 1 : 0);

                if (count == 0)
                {
                    insideSolution[ii, 0] = solution[s, 0];
                    insideSolution[ii, 1] = solution[s, 1];
                    insideSolution[ii, 2] = solution[s, 2];
                    insideSolution[ii++, 3] = solution[s, 3];
                }
                else if (count == 1)
                {
                    edgeSolution[ie, 0] = solution[s, 0];
                    edgeSolution[ie, 1] = solution[s, 1];
                    edgeSolution[ie, 2] = solution[s, 2];
                    edgeSolution[ie++, 3] = solution[s, 3];
                }
                else
                {
                    cornerSolution[ic, 0] = solution[s, 0];
                    cornerSolution[ic, 1] = solution[s, 1];
                    cornerSolution[ic, 2] = solution[s, 2];
                    cornerSolution[ic++, 3] = solution[s, 3];
                }
            }

            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    if (x > 0)
                    {
                        matrix[y, x, 0] = int.MinValue; //empty
                    }
                    if (y > 0)
                    {
                        matrix[y, x, 1] = int.MinValue; //empty
                    }
                }
            }


            return CountSolutions3(matrix, cornerSolution, edgeSolution, insideSolution, size, 0, 0, 3);
        }







        private static void CountSolutions4(int[,,] matrix,
          int[,] cornerSolution, int[,] edgeSolution, int[,] insideSolution,
          int size, int atX, int atY, int atP, ref int solcnt, ref int cyclcount, DateTime endTime)
        {
            if (atY == size)
            {
                using (StreamWriter sw2 = new StreamWriter("outpDetail4.txt", true))
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
                            outp[y * 4 + 1, x * 4 + 0] = (-matrix[y, x, 0]).ToString();
                            outp[y * 4 + 0, x * 4 + 1] = (-matrix[y, x, 1]).ToString();
                            outp[y * 4 + 1, x * 4 + 2] = (matrix[y, x + 1, 0]).ToString();
                            outp[y * 4 + 2, x * 4 + 1] = (matrix[y + 1, x, 1]).ToString();

                            outp[y * 4 + 1, x * 4 + 1] = "X";
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

                solcnt++;
                return;
            }

            if (solcnt > 2)
            {
                if (solcnt < 777) { solcnt = 777; }
                return;
            }
            if (++cyclcount > 20000000)//200000000
            {
                cyclcount = 0;
                if (DateTime.Now > endTime)
                {
                    solcnt = 999;
                    return;
                }
            }

            int[,] solution;
            bool sY = atX == 0 || atX == size - 1;
            bool sX = atY == 0 || atY == size - 1;
            if (sX)
            {
                solution = sY ? cornerSolution : edgeSolution;
            }
            else
            {
                solution = sY ? edgeSolution : insideSolution;
            }

            int len = solution.GetLength(0);
            for (int i = atP; i < len; ++i)
            {
                if (solution[i, 4] > 0) { continue; }

                int m0 = -matrix[atY, atX, 0];
                int m1 = -matrix[atY, atX, 1];
                int m2 = -matrix[atY, atX + 1, 0];
                int m3 = -matrix[atY + 1, atX, 1];

                int s0 = solution[i, 0];
                int s1 = solution[i, 1];
                int s2 = solution[i, 2];
                int s3 = solution[i, 3];

                if ((m1 == s0 || m1 == s1 || m1 == s2 || m1 == s3)
                 && (m0 == s0 || m0 == s1 || m0 == s2 || m0 == s3))
                {
                    for (int rot = 0; rot < 4; ++rot)
                    {
                        if ((m0 == s0)
                         && (m1 == s1)
                         && (m2 == int.MinValue || m2 == s2)
                         && (m3 == int.MinValue || m3 == s3))
                        {
                            matrix[atY, atX + 1, 0] = s2;
                            matrix[atY + 1, atX, 1] = s3;
                            solution[i, 4] = 1;

                            int atX2 = atX + 1;
                            int atY2 = atY;
                            if (atX2 == size)
                            {
                                atX2 = 0;
                                atY2++;
                            }

                            CountSolutions4(matrix, cornerSolution, edgeSolution, insideSolution, size, atX2, atY2, 0, ref solcnt, ref cyclcount, endTime);

                            solution[i, 4] = 0;
                            matrix[atY, atX + 1, 0] = m2;
                            matrix[atY + 1, atX, 1] = m3;
                        }

                        var tmp = s0;
                        s0 = s1;
                        s1 = s2;
                        s2 = s3;
                        s3 = tmp;
                    }
                }
            }
        }

        private static int CountSolutions4(int[,] solution, int size)
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
                    if (!seen.Add((a0, a1, a2, a3))) { return 888; }
                    int tmp = a0;
                    a0 = a1;
                    a1 = a2;
                    a2 = a3;
                    a3 = tmp;
                }
            }

            int[,,] matrix = new int[size + 1, size + 1, 2];

            int[,] cornerSolution = new int[4, 5];
            int[,] edgeSolution = new int[(size - 2) * 4, 5];
            int[,] insideSolution = new int[(size - 2) * (size - 2), 5];

            int ic = 0;
            int ie = 0;
            int ii = 0;
            for (int s = 0; s < size * size; ++s)
            {
                int count = (solution[s, 0] == 0 ? 1 : 0)
                    + (solution[s, 1] == 0 ? 1 : 0)
                    + (solution[s, 2] == 0 ? 1 : 0)
                    + (solution[s, 3] == 0 ? 1 : 0);

                if (count == 0)
                {
                    insideSolution[ii, 0] = solution[s, 0];
                    insideSolution[ii, 1] = solution[s, 1];
                    insideSolution[ii, 2] = solution[s, 2];
                    insideSolution[ii++, 3] = solution[s, 3];
                }
                else if (count == 1)
                {
                    edgeSolution[ie, 0] = solution[s, 0];
                    edgeSolution[ie, 1] = solution[s, 1];
                    edgeSolution[ie, 2] = solution[s, 2];
                    edgeSolution[ie++, 3] = solution[s, 3];
                }
                else
                {
                    cornerSolution[ic, 0] = solution[s, 0];
                    cornerSolution[ic, 1] = solution[s, 1];
                    cornerSolution[ic, 2] = solution[s, 2];
                    cornerSolution[ic++, 3] = solution[s, 3];
                }
            }

            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    if (x > 0)
                    {
                        matrix[y, x, 0] = int.MinValue; //empty
                    }
                    if (y > 0)
                    {
                        matrix[y, x, 1] = int.MinValue; //empty
                    }
                }
            }

            int solcnt = 0;
            int cyclcount = 0;
            DateTime endTime = DateTime.Now;//.AddMinutes(15);

            CountSolutions4(matrix, cornerSolution, edgeSolution, insideSolution, size, 0, 0, 3, ref solcnt, ref cyclcount, endTime);

            //Console.Write(" CYCLCOUNT " + cyclcount + " ");

            return solcnt;
        }







        //static int CNT = 0;

        enum Smer { dolu, doleva, doprava, nahoru };

        private static bool isDuplicit(int[,] solution, int size)
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


        private static void CountSolutions7(int[,,] matrix,
         int[,] cornerSolution, int[,] edgeSolution, int[,] insideSolution,
         int size, int[,] posArr, int posI, int atP, ref int solcnt, ref int cyclcount, DateTime endTime)
        {
            //if (posI == 25)
            //{
            //    CNT++;
            //    return;
            //}

            if (posI == posArr.GetLength(0))
            {
                using (StreamWriter sw2 = new StreamWriter("outpDetail7-" + solcnt + ".txt", true))
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
                            outp[y * 4 + 1, x * 4 + 0] = (matrix[y, x, 0]).ToString();
                            outp[y * 4 + 0, x * 4 + 1] = (matrix[y, x, 1]).ToString();
                            outp[y * 4 + 1, x * 4 + 2] = (matrix[y, x + 1, 0]).ToString();
                            outp[y * 4 + 2, x * 4 + 1] = (matrix[y + 1, x, 1]).ToString();

                            outp[y * 4 + 1, x * 4 + 1] = "X";
                            //outp[y * 4 + 1, x * 4 + 1] = (matrix[y, x, 2]).ToString(); ;
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

                solcnt++;
                return;
            }

            if (solcnt > 2)
            {
                if (solcnt < 777) { solcnt = 777; }
                return;
            }
            if (++cyclcount > 20000000)//200000000
            {
                cyclcount = 0;
                if (DateTime.Now > endTime)
                {
                    solcnt = 999;
                    return;
                }
            }

            int atX = posArr[posI, 0];
            int atY = posArr[posI, 1];

            int[,] solution;
            bool sY = atX == 0 || atX == size - 1;
            bool sX = atY == 0 || atY == size - 1;
            if (sX)
            {
                solution = sY ? cornerSolution : edgeSolution;
            }
            else
            {
                solution = sY ? edgeSolution : insideSolution;
            }

            int len = solution.GetLength(0);
            for (int i = atP; i < len; ++i)
            {
                if (solution[i, 4] > 0) { continue; }

                int m0 = -matrix[atY, atX, 0];
                int m1 = -matrix[atY, atX, 1];
                int m2 = -matrix[atY, atX + 1, 0];
                int m3 = -matrix[atY + 1, atX, 1];

                int s0 = solution[i, 0];
                int s1 = solution[i, 1];
                int s2 = solution[i, 2];
                int s3 = solution[i, 3];

                //if ((m1 == s0 || m1 == s1 || m1 == s2 || m1 == s3)
                // && (m0 == s0 || m0 == s1 || m0 == s2 || m0 == s3))
                {
                    for (int rot = 0; rot < 4; ++rot)
                    {
                        if ((m0 == int.MinValue || m0 == s0)
                         && (m1 == int.MinValue || m1 == s1)
                         && (m2 == int.MinValue || m2 == s2)
                         && (m3 == int.MinValue || m3 == s3))
                        {
                            matrix[atY, atX, 0] = s0;
                            matrix[atY, atX, 1] = s1;
                            matrix[atY, atX + 1, 0] = s2;
                            matrix[atY + 1, atX, 1] = s3;
                            matrix[atY, atX, 2] = i;
                            solution[i, 4] = 1;

                            CountSolutions7(matrix, cornerSolution, edgeSolution, insideSolution, size, posArr, posI + 1, 0, ref solcnt, ref cyclcount, endTime);

                            solution[i, 4] = 0;
                            matrix[atY, atX, 0] = -m0;
                            matrix[atY, atX, 1] = -m1;
                            matrix[atY, atX + 1, 0] = -m2;
                            matrix[atY + 1, atX, 1] = -m3;
                        }

                        var tmp = s0;
                        s0 = s1;
                        s1 = s2;
                        s2 = s3;
                        s3 = tmp;
                    }
                }
            }
        }


        private static int CountSolutions7(int[,] solution, int size)
        {
            if (isDuplicit(solution, size)) { return 888; }

            int[,,] matrix = new int[size + 1, size + 1, 3];

            int[,] cornerSolution = new int[4, 5];
            int[,] edgeSolution = new int[(size - 2) * 4, 5];
            int[,] insideSolution = new int[(size - 2) * (size - 2), 5];

            int ic = 0;
            int ie = 0;
            int ii = 0;
            for (int s = 0; s < size * size; ++s)
            {
                int count = (solution[s, 0] == 0 ? 1 : 0)
                    + (solution[s, 1] == 0 ? 1 : 0)
                    + (solution[s, 2] == 0 ? 1 : 0)
                    + (solution[s, 3] == 0 ? 1 : 0);

                if (count == 0)
                {
                    insideSolution[ii, 0] = solution[s, 0];
                    insideSolution[ii, 1] = solution[s, 1];
                    insideSolution[ii, 2] = solution[s, 2];
                    insideSolution[ii++, 3] = solution[s, 3];
                }
                else if (count == 1)
                {
                    edgeSolution[ie, 0] = solution[s, 0];
                    edgeSolution[ie, 1] = solution[s, 1];
                    edgeSolution[ie, 2] = solution[s, 2];
                    edgeSolution[ie++, 3] = solution[s, 3];
                }
                else
                {
                    cornerSolution[ic, 0] = solution[s, 0];
                    cornerSolution[ic, 1] = solution[s, 1];
                    cornerSolution[ic, 2] = solution[s, 2];
                    cornerSolution[ic++, 3] = solution[s, 3];
                }
            }

            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    if (x > 0)
                    {
                        matrix[y, x, 0] = int.MinValue; //empty
                    }
                    if (y > 0)
                    {
                        matrix[y, x, 1] = int.MinValue; //empty
                    }
                }
            }

            int[,] posArr = new int[size * size, 2];

            int atx = 0;
            int aty = 0;

            Smer smer = Smer.doleva;

            for (int i = 0; i < posArr.GetLength(0); ++i)
            {
                posArr[i, 0] = atx;
                posArr[i, 1] = aty;

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


            int solcnt = 0;
            int cyclcount = 0;
            DateTime endTime = DateTime.Now.AddMinutes(2);

            CountSolutions7(matrix, cornerSolution, edgeSolution, insideSolution, size, posArr, 0, 3, ref solcnt, ref cyclcount, endTime);

            //Console.Write(" CYCLCOUNT " + cyclcount + " ");

            return solcnt;
        }


        /*       private static void CountSolutions8(int[,,] matrix,
         int[,] cornerSolution, int[,] edgeSolution, int[,] insideSolution,
         int size, int[,] posArr, int posI, int atP, ref int solcnt, ref int cyclcount, DateTime endTime)
               {
                   if (posI == posArr.GetLength(0))
                   {
                       using (StreamWriter sw2 = new StreamWriter("outpDetail7-" + solcnt + ".txt", true))
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
                                   outp[y * 4 + 1, x * 4 + 0] = (matrix[y, x, 0]).ToString();
                                   outp[y * 4 + 0, x * 4 + 1] = (matrix[y, x, 1]).ToString();
                                   outp[y * 4 + 1, x * 4 + 2] = (matrix[y, x + 1, 0]).ToString();
                                   outp[y * 4 + 2, x * 4 + 1] = (matrix[y + 1, x, 1]).ToString();

                                   //outp[y * 4 + 1, x * 4 + 1] = "X";
                                   outp[y * 4 + 1, x * 4 + 1] = (matrix[y, x, 2]).ToString(); ;
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

                       solcnt++;
                       return;
                   }

                   if (solcnt > 2)
                   {
                       if (solcnt < 777) { solcnt = 777; }
                       return;
                   }
                   if (++cyclcount > 2000000)//200000000
                   {
                       cyclcount = 0;
                       if (DateTime.Now > endTime)
                       {
                           solcnt = 999;
                           return;
                       }
                   }

                   int atX = posArr[posI, 0];
                   int atY = posArr[posI, 1];

                   int[,] solution;
                   bool sY = atX == 0 || atX == size - 1;
                   bool sX = atY == 0 || atY == size - 1;
                   if (sX)
                   {
                       solution = sY ? cornerSolution : edgeSolution;
                   }
                   else
                   {
                       solution = sY ? edgeSolution : insideSolution;
                   }

                   int len = solution.GetLength(0);
                   for (int i = atP; i < len; ++i)
                   {
                       if (solution[i, 4] > 0) { continue; }

                       int m0 = -matrix[atY, atX, 0];
                       int m1 = -matrix[atY, atX, 1];
                       int m2 = -matrix[atY, atX + 1, 0];
                       int m3 = -matrix[atY + 1, atX, 1];

                       int s0 = solution[i, 0];
                       int s1 = solution[i, 1];
                       int s2 = solution[i, 2];
                       int s3 = solution[i, 3];

                       //if ((m1 == s0 || m1 == s1 || m1 == s2 || m1 == s3)
                       // && (m0 == s0 || m0 == s1 || m0 == s2 || m0 == s3))
                       {
                           for (int rot = 0; rot < 4; ++rot)
                           {
                               if ((m0 == int.MinValue || m0 == s0)
                                && (m1 == int.MinValue || m1 == s1)
                                && (m2 == int.MinValue || m2 == s2)
                                && (m3 == int.MinValue || m3 == s3))
                               {
                                   matrix[atY, atX, 0] = s0;
                                   matrix[atY, atX, 1] = s1;
                                   matrix[atY, atX + 1, 0] = s2;
                                   matrix[atY + 1, atX, 1] = s3;
                                   matrix[atY, atX, 2] = i;
                                   solution[i, 4] = 1;

                                   CountSolutions8(matrix, cornerSolution, edgeSolution, insideSolution, size, posArr, posI + 1, 0, ref solcnt, ref cyclcount, endTime);

                                   solution[i, 4] = 0;
                                   matrix[atY, atX, 0] = -m0;
                                   matrix[atY, atX, 1] = -m1;
                                   matrix[atY, atX + 1, 0] = -m2;
                                   matrix[atY + 1, atX, 1] = -m3;
                               }

                               var tmp = s0;
                               s0 = s1;
                               s1 = s2;
                               s2 = s3;
                               s3 = tmp;
                           }
                       }
                   }
               }


               private static int CountSolutions8(int[,] solution, int size)
               {
                   if (isDuplicit(solution, size)) { return 888; }

                   int[,,] matrix = new int[size + 1, size + 1, 3];

                   int[,] cornerSolution = new int[4, 5];
                   int[,] edgeSolution = new int[(size - 2) * 4, 5];
                   int[,] insideSolution = new int[(size - 2) * (size - 2), 5];

                   int ic = 0;
                   int ie = 0;
                   int ii = 0;
                   for (int s = 0; s < size * size; ++s)
                   {
                       int count = (solution[s, 0] == 0 ? 1 : 0)
                           + (solution[s, 1] == 0 ? 1 : 0)
                           + (solution[s, 2] == 0 ? 1 : 0)
                           + (solution[s, 3] == 0 ? 1 : 0);

                       if (count == 0)
                       {
                           insideSolution[ii, 0] = solution[s, 0];
                           insideSolution[ii, 1] = solution[s, 1];
                           insideSolution[ii, 2] = solution[s, 2];
                           insideSolution[ii++, 3] = solution[s, 3];
                       }
                       else if (count == 1)
                       {
                           edgeSolution[ie, 0] = solution[s, 0];
                           edgeSolution[ie, 1] = solution[s, 1];
                           edgeSolution[ie, 2] = solution[s, 2];
                           edgeSolution[ie++, 3] = solution[s, 3];
                       }
                       else
                       {
                           cornerSolution[ic, 0] = solution[s, 0];
                           cornerSolution[ic, 1] = solution[s, 1];
                           cornerSolution[ic, 2] = solution[s, 2];
                           cornerSolution[ic++, 3] = solution[s, 3];
                       }
                   }

                   for (int y = 0; y < size; ++y)
                   {
                       for (int x = 0; x < size; ++x)
                       {
                           if (x > 0)
                           {
                               matrix[y, x, 0] = int.MinValue; //empty
                           }
                           if (y > 0)
                           {
                               matrix[y, x, 1] = int.MinValue; //empty
                           }
                       }
                   }

                   int[,] posArr = new int[size * size, 2];

                   int[,] hand = null;

                   if (size == 12)
                   {
                       hand = new int[,] {
                       {0,1,8,9,24,25,48,49,50,51,52,53              },
                       {3,2,7,10,23,26,47,58,57,56,55,54             },
                       {4,5,6,11,22,27,46,59,60,61,62,63             },
                       {15,14,13,12,21,28,45,68,67,66,65,64          },
                       {16,17,18,19,20,29,44,69,70,71,72,73          },
                       {35,34,33,32,31,30,43,78,77,76,75,74          },
                       {36,37,38,39,40,41,42,79,80,81,82,83          },
                       {135,134,133,124,123,114,113,104,103,94,93,84 },
                       {136,137,132,125,122,115,112,105,102,95,92,85 },
                       {139,138,131,126,121,116,111,106,101,96,91,86 },
                       {140,141,130,127,120,117,110,107,100,97,90,87 },
                       { 143,142,129,128,119,118,109,108,99,98,89,88 }
                   };
                   }

                   if (size == 8)
                   {
                       //hand = new int[,] {
                       //{0,1,8,9,24,25,26,27 }    ,
                       //{3,2,7,10,23,30,29,28    },
                       //{4,5,6,11,22,31,32,33    },
                       //{15,14,13,12,21,36,35,34 },
                       //{16,17,18,19,20,37,38,39 },
                       //{59,58,57,52,51,46,45,40 },
                       //{60,61,56,53,50,47,44,41 },
                       //{ 63,62,55,54,49,48,43,42}
                       //};
                       hand = new int[,] {
       {0,1,2,3,4,5,6,7 }        ,
       {15,14,13,12,11,10,9,8   },
       {16,17,18,19,20,21,22,23 },
       {31,30,29,28,27,26,25,24 },
       {32,33,34,35,36,37,38,39 },
       {47,46,45,44,43,42,41,40 },
       {48,49,50,51,52,53,54,55 },
       { 63,62,61,60,59,58,57,56},
                       };
                   }

                   for (int i = 0; i < posArr.GetLength(0); ++i)
                   {
                       for (int y = 0; y < size; ++y)
                       {
                           for (int x = 0; x < size; ++x)
                           {
                               if (hand[y, x] == i)
                               {
                                   posArr[i, 0] = x;
                                   posArr[i, 1] = y;
                               }
                           }
                       }
                   }

                   int solcnt = 0;
                   int cyclcount = 0;
                   DateTime endTime = DateTime.Now.AddMinutes(2);

                   CountSolutions8(matrix, cornerSolution, edgeSolution, insideSolution, size, posArr, 0, 3, ref solcnt, ref cyclcount, endTime);

                   //Console.Write(" CYCLCOUNT " + cyclcount + " ");

                   return solcnt;
               }
        */



        public class State9
        {
            public uint[,] matrix;
            public Dictionary<uint, short> MatchPiecesStart;
            public short[] MatchPieceIndexes;
            public uint[] Pieces;
            public bool[] isPieceUsed;
            public byte[,] posArr;
            public int finalPos;
            public readonly int size;
            public int solcnt;
            public int cyclcount;
            public long totalCyclCount;
            public DateTime endTime;

            public State9(int size)
            {
                this.size = size;
                GetPosArray();
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
                        outp[y * 4 + 1, x * 4 + 0] = (unByte(matrix[y + 1, x + 1] & 0xFF)).ToString();
                        outp[y * 4 + 0, x * 4 + 1] = (unByte((matrix[y + 1, x + 1] >> 8) & 0xFF)).ToString();
                        outp[y * 4 + 1, x * 4 + 2] = (unByte((matrix[y + 1, x + 1] >> 16) & 0xFF)).ToString();
                        outp[y * 4 + 2, x * 4 + 1] = (unByte((matrix[y + 1, x + 1] >> 24) & 0xFF)).ToString();


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
                Console.Write((unByte(pc & 0xFF)).ToString());
                Console.Write(", ");
                Console.Write((unByte((pc >> 8) & 0xFF)).ToString());
                Console.Write(", ");
                Console.Write((unByte((pc >> 16) & 0xFF)).ToString());
                Console.Write(", ");
                Console.Write((unByte((pc >> 24) & 0xFF)).ToString());
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
                using (StreamWriter sw2 = new StreamWriter("outpDetail9-" + solcnt + ".txt", true))
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
                            outp[y * 4 + 1, x * 4 + 0] = (unByte(matrix[y + 1, x + 1] & 0xFF)).ToString();
                            outp[y * 4 + 0, x * 4 + 1] = (unByte((matrix[y + 1, x + 1] >> 8) & 0xFF)).ToString();
                            outp[y * 4 + 1, x * 4 + 2] = (unByte((matrix[y + 1, x + 1] >> 16) & 0xFF)).ToString();
                            outp[y * 4 + 2, x * 4 + 1] = (unByte((matrix[y + 1, x + 1] >> 24) & 0xFF)).ToString();


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

            private void GetPosArray()
            {
                int atx = 0;
                int aty = 0;
                Smer smer = Smer.doleva;
                posArr = new byte[size * size, 2];
                for (int i = 0; i < posArr.GetLength(0); ++i)
                {
                    posArr[i, 0] = (byte)(atx + 1);
                    posArr[i, 1] = (byte)(aty + 1);

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

            public void CountSolutions9(int posI)
            {
                if (posI == finalPos)
                {
                    if (posI == posArr.GetLength(0))
                    {
                        SaveState();
                        solcnt++;
                    }
                    return;
                }

                if (solcnt > 2)
                {
                    if (solcnt < 777) { solcnt = 777; }
                    return;
                }
                if (++cyclcount > 200000000)//200000000   20M=1sec
                {
                    totalCyclCount += cyclcount;
                    cyclcount = 0;
                    if (DateTime.Now > endTime)
                    {
                        solcnt = 999;
                        return;
                    }
                }

                int atX = posArr[posI, 0];
                int atY = posArr[posI, 1];
                uint s0 = (matrix[atY, atX - 1]) & 0x00FF0000;
                uint s1 = (matrix[atY - 1, atX]) & 0xFF000000;
                uint s2 = (matrix[atY, atX + 1]) & 0x000000FF;
                uint s3 = (matrix[atY + 1, atX]) & 0x0000FF00;
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
                            matrix[atY, atX] = ROR(Pieces[pI], pieceI & 0x1F);
                            CountSolutions9(posI + 1);
                            matrix[atY, atX] = EMPTY;
                            isPieceUsed[pI] = false;
                        }
                        pieceI = MatchPieceIndexes[++index];
                        //PrintState(S, atX, atY);                   
                    } while (pieceI >= 0);
                }

                //Console.WriteLine("NO PIECE");            
            }
        }



        private static int CountSolutions9(int[,] solution, int size, int minutesLimit)
        {
            foreach (int e in solution)
            {
                if (e < -127 || e > 127) { return -1; } // out of range for this solver
            }
            if (isDuplicit(solution, size)) { return 888; } // duplicit pieces -> more than 2 solutions
            var totalEndTime = DateTime.Now.AddMinutes(minutesLimit);

            State9 S = new State9(size)
            {
                solcnt = 0,
                cyclcount = 0,
                totalCyclCount = 0,
                Pieces = GetPieces(solution)
            };

            (S.MatchPiecesStart, S.MatchPieceIndexes) = buildDictionary(S.Pieces);
            S.isPieceUsed = new bool[S.Pieces.Length];

            S.matrix = new uint[size + 2, size + 2];
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    S.matrix[y + 1, x + 1] = EMPTY;
                }
            }

            int atX = 1;
            int atY = 1;
            uint s0 = (S.matrix[atY, atX - 1]) & 0x00FF0000;
            uint s1 = (S.matrix[atY - 1, atX]) & 0xFF000000;
            uint s2 = (S.matrix[atY, atX + 1]) & 0x000000FF;
            uint s3 = (S.matrix[atY + 1, atX]) & 0x0000FF00;
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
                        S.matrix[atY, atX] = ROR(S.Pieces[pI], pieceI & 0x1F);
                        S.finalPos = 30;
                        S.totalCyclCount = 0;
                        S.cyclcount = 0;
                        S.solcnt = 0;
                        S.endTime = DateTime.Now.AddMinutes(1);

                        //System.Diagnostics.Stopwatch st = new Stopwatch();
                        //st.Start();
                        S.CountSolutions9(1);
                        S.matrix[atY, atX] = EMPTY;
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
                    bestCorner = 0; //just any
                }

                if (bestCorner == -1) { return 999; }

                var pieceI2 = S.MatchPieceIndexes[index + bestCorner];
                int pI2 = pieceI2 >> 5;
                S.isPieceUsed[pI2] = true;
                S.matrix[atY, atX] = ROR(S.Pieces[pI2], pieceI2 & 0x1F);
                S.finalPos = S.posArr.GetLength(0);
                S.totalCyclCount = 0;
                S.cyclcount = 0;
                S.solcnt = 0;
                S.endTime = totalEndTime;
                S.CountSolutions9(1);
            }
            else
            {
                return -1;
            }

            //Console.Write(" CYCLCOUNT " + S.cyclcount + " ");

            return S.solcnt;
        }





