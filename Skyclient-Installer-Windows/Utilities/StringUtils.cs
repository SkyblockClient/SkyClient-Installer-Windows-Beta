using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyclient.Utilities
{
    public class StringUtils
    {
        public class NeedlemanWunsch
        {
            private int Match { get; set; }
            private int Mismatch { get; set; }

            private int Gap { get; set; }

            // Row sequence
            private string FirstSequence { get; set; }

            // Column sequence
            private string SecondSequence { get; set; }
            private int[,] Matrix { get; set; }

            // All possible back traces
            private List<List<Trace>> BackTraces { get; set; }
            private List<AlignedSequencePair> AlignedSequencePairs { get; set; }

            /// <summary>
            /// Constructor for initializing a NeedlemanWunsch instance.
            /// Sets given values for alignment.
            /// </summary>
            /// <param name="match">Match reward</param>
            /// <param name="mismatch">Mismatch penalty</param>
            /// <param name="gap">Gap penalty</param>
            /// <param name="firstSequence">First sequence</param>
            /// <param name="secondSequence">Second sequence</param>
            public NeedlemanWunsch(int match, int mismatch, int gap, string firstSequence, string secondSequence)
            {
                Match = match;
                Mismatch = mismatch;
                Gap = gap;
                FirstSequence = firstSequence;
                SecondSequence = secondSequence;
                Matrix = new int[firstSequence.Length + 1, secondSequence.Length + 1];
                BackTraces = new List<List<Trace>>();
                AlignedSequencePairs = new List<AlignedSequencePair>();
            }

            /// <summary>
            /// Fill in solution matrix based on match, mismatch and gap values
            /// </summary>
            public void FillMatrix()
            {
                // Fill in first row and first col with gap values
                for (int i = 0; i < Matrix.GetLength(0); i++)
                {
                    Matrix[i, 0] = i * Gap;
                }

                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Matrix[0, j] = j * Gap;
                }

                // Traverse matrix and calculate all values

                for (int i = 1; i < Matrix.GetLength(0); i++)
                {
                    for (int j = 1; j < Matrix.GetLength(1); j++)
                    {
                        int topValue = Matrix[i - 1, j] + Gap;
                        int leftValue = Matrix[i, j - 1] + Gap;
                        int diagonalValue = Matrix[i - 1, j - 1] +
                                            (FirstSequence[i - 1] == SecondSequence[j - 1] ? Match : Mismatch);
                        Matrix[i, j] = Math.Max(Math.Max(topValue, leftValue), diagonalValue);
                    }
                }
            }

            /// <summary>
            /// Recursively trace back
            /// </summary>
            /// <param name="traces"></param>
            public void TraceBack(List<Trace> traces)
            {
                // Continue tracing until Matrix[1,1], Matrix[1,0] or Matrix[0,1]
                while (!((traces.Last().RowIndex == 1 && traces.Last().ColIndex == 1) ||
                         (traces.Last().RowIndex == 1 && traces.Last().ColIndex == 0) ||
                         (traces.Last().RowIndex == 0 && traces.Last().ColIndex == 1)))
                {
                    bool isSourceTop = false;
                    bool isSourceLeft = false;
                    bool isSourceDiagonal = false;

                    int topValue = Matrix[traces.Last().RowIndex - 1, traces.Last().ColIndex] + Gap;
                    int leftValue = Matrix[traces.Last().RowIndex, traces.Last().ColIndex - 1] + Gap;
                    int diagonalValue = Matrix[traces.Last().RowIndex - 1, traces.Last().ColIndex - 1] +
                                        (FirstSequence[traces.Last().RowIndex - 1] ==
                                         SecondSequence[traces.Last().ColIndex - 1]
                                            ? Match
                                            : Mismatch);

                    // Set flags
                    if (topValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                    {
                        isSourceTop = true;
                    }

                    if (leftValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                    {
                        isSourceLeft = true;
                    }

                    if (diagonalValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                    {
                        isSourceDiagonal = true;
                    }

                    // Handle all possibilities, there might be alternative traces
                    // If there such trace exists, handle it as different traceback recursively.
                    if (isSourceTop && isSourceLeft && isSourceDiagonal)
                    {
                        var tempTrace = new List<Trace>(traces);

                        //top condition
                        tempTrace.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex
                        });
                        TraceBack(tempTrace); //recursive call

                        //left condition
                        tempTrace = new List<Trace>(traces);
                        tempTrace.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                        TraceBack(tempTrace);
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                    }
                    else if (isSourceTop && isSourceLeft)
                    {
                        var tempTrace = new List<Trace>(traces);

                        //top condition
                        tempTrace.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex
                        });
                        TraceBack(tempTrace);

                        //left condition
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                    }
                    else if (isSourceTop && isSourceDiagonal)
                    {
                        var tempTrace = new List<Trace>(traces);

                        //top condition
                        tempTrace.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex
                        });
                        TraceBack(tempTrace);

                        //diagonal condition
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                    }
                    else if (isSourceLeft && isSourceDiagonal)
                    {
                        var tempTrace = new List<Trace>(traces);

                        //left condition
                        tempTrace.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                        TraceBack(tempTrace);

                        //diagonal condition
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                    }
                    else if (isSourceTop)
                    {
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex
                        });
                    }
                    else if (isSourceLeft)
                    {
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                    }
                    else
                    {
                        traces.Add(new Trace
                        {
                            RowIndex = traces.Last().RowIndex - 1,
                            ColIndex = traces.Last().ColIndex - 1
                        });
                    }
                }

                traces.Add(new Trace { RowIndex = 0, ColIndex = 0 });
                BackTraces.Add(traces);
            }


            /// <summary>
            /// Based on back traces, find all possible sequences.
            /// </summary>
            public void AlignSequences()
            {
                for (int i = 0; i < BackTraces.Count; i++)
                {
                    string firstAlignedSequence = "";
                    string secondAlignedSequence = "";
                    for (int j = 0; j < BackTraces[i].Count - 1; j++)
                    {
                        if (BackTraces[i][j].RowIndex - 1 == BackTraces[i][j + 1].RowIndex &&
                            BackTraces[i][j].ColIndex - 1 == BackTraces[i][j + 1].ColIndex)
                        {
                            firstAlignedSequence += FirstSequence[BackTraces[i][j].RowIndex - 1];
                            secondAlignedSequence += SecondSequence[BackTraces[i][j].ColIndex - 1];
                        }
                        else if (BackTraces[i][j].RowIndex - 1 == BackTraces[i][j + 1].RowIndex &&
                                 BackTraces[i][j].ColIndex == BackTraces[i][j + 1].ColIndex)
                        {
                            firstAlignedSequence += FirstSequence[BackTraces[i][j].RowIndex - 1];
                            secondAlignedSequence += "-";
                        }
                        else
                        {
                            firstAlignedSequence += "-";
                            secondAlignedSequence += SecondSequence[BackTraces[i][j].ColIndex - 1];
                        }
                    }

                    AlignedSequencePairs.Add(new AlignedSequencePair
                    {
                        FirstAlignedSequence = String.Concat(firstAlignedSequence.Reverse()),
                        SecondAlignedSequence = String.Concat(secondAlignedSequence.Reverse())
                    });
                }
            }

            /// <summary>
            /// Print sequence alignment results along with other info obtained.
            /// </summary>
            public void PrintResults()
            {
                // Print solution matrix

                Console.WriteLine("Solution matrix:");

                for (int i = 0; i < Matrix.GetLength(0); i++)
                {
                    if (i == 0)
                    {
                        Console.Write("\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;

                        for (int j = 0; j < Matrix.GetLength(1); j++)
                        {
                            if (j != 0)
                            {
                                Console.Write(SecondSequence[j - 1] + "\t");
                            }
                        }

                        Console.ResetColor();
                        Console.WriteLine();
                    }

                    if (i != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(FirstSequence[i - 1]);
                        Console.ResetColor();
                    }

                    Console.Write("\t");


                    for (int j = 0; j < Matrix.GetLength(1); j++)
                    {
                        Console.Write(Matrix[i, j] + "\t");
                    }

                    Console.WriteLine();
                }

                // Print score
                Console.WriteLine("\nObtained score: " + Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1]);

                // Print back traces info
                Console.WriteLine("Number of backtraces: " + BackTraces.Count);

                // Print all possible alignments
                Console.WriteLine("\nAll possible alignments:");

                foreach (var pair in AlignedSequencePairs)
                {
                    Console.WriteLine("----------");
                    Console.WriteLine(pair.FirstAlignedSequence);
                    Console.WriteLine(pair.SecondAlignedSequence);
                }

                Console.WriteLine("----------");
            }

            /// <summary>
            /// Run alignment process based on sequence info.
            /// </summary>
            public void Run()
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                FillMatrix();
                var traces = new List<Trace>();
                traces.Add(new Trace
                {
                    RowIndex = Matrix.GetLength(0) - 1,
                    ColIndex = Matrix.GetLength(1) - 1
                });
                TraceBack(traces);
                AlignSequences();
                watch.Stop();

                PrintResults();
                Console.WriteLine("\nExecution time: " + watch.ElapsedMilliseconds + " milliseconds\n");
            }
        }


        public class AlignedSequencePair
        {
            public string FirstAlignedSequence { get; set; }
            public string SecondAlignedSequence { get; set; }
        }

        public class Trace
        {
            public int RowIndex { get; set; }
            public int ColIndex { get; set; }
        }

        public static LR Hirschberg(string X, string Y)
        {
            var wunsch1 = new NeedlemanWunsch(1, -1, -1, Y, X);
            wunsch1.Run();

            var Z = "";
            var W = "";
            /*
            if (X.Length == 0)
            {
                for (int i = 1; i < Y.Length; i++)
                {
                    Z += "-";
                    W += Y[i];
                }
            }
            else if (Y.Length == 0)
            {
                for (int i = 1; i < X.Length; i++)
                {
                    Z += X[i];
                    W += "-";
                }
            }
            else if (X.Length == 1 || Y.Length == 1)
            {
                var wunsch = new NeedlemanWunsch(10, -2, -5, X, Y);
                wunsch.Run();
                //wunsch.PrintResults();
                //throw new NotImplementedException(); // NeedlemanWunsch(X, Y)
            }
            else
            {
                var xlen = X.Length;
                var xmid = X.Length / 2;
                var ylen = Y.Length;

                var ScoreL = NWScore(substring(X, 1, xmid), Y);
                var ScoreR = NWScore(rev(substring(X, xmid + 1, xlen)), rev(Y));

                // ymid = arg max ScoreL + rev(ScoreR)
                var ymid = ScoreL.Zip(rev(ScoreR), (x, y) => x + y).ToArray().Max();

                // (Z,W) = Hirschberg(X1:xmid, y1:ymid) +
                //      Hirschberg(Xxmid+1:xlen, Yymid+1:ylen)
                return Hirschberg(substring(X, 1, xmid), substring(Y, 1, ymid)) + 
                    Hirschberg(substring(X, xmid + 1, xlen), substring(Y, ymid + 1, ylen));
            }
            */
            return new LR(Z, W);
        }

        public struct LR
        {
            public string L, R;
            public LR(string l, string r)
            {
                this.L = l;
                this.R = r;
            }
            public static LR operator +(LR a, LR b)
            {
                return new LR(a.L + b.L, a.R + b.R);
            }
        }

        private static string substring(string s, int i, int j)
        {
            var newstring = "";
            for (int index = 0; index < s.Length; index++)
            {
                if (index >= i && index <= j)
                {
                    newstring += s[index];
                }
            }
            return newstring;
        }
        private static string rev(string s)
        {
            return s.Reverse().ToString();
        }
        private static int[] rev(int[] s)
        {
            return s.Reverse().ToArray();
        }

        private static int[] NWScore(string X, string Y)
        {
            //int[,] Score = new int[X.Length, Y.Length];
            int[,] Score = new int[2, 2 * (Y.Length + 1)];
            // Score(0, 0) = 0 // 2 * (length(Y) + 1) array
            Score[0, 0] = 0;
            for (int j = 1; j < Y.Length; j++)
            {
                // Score(0, j) = Score(0, j - 1) + Ins(Yj)
                Score[0, j] = Score[0, j - 1] + Ins(Y[j]);
            }
            for (int i = 1; i < X.Length; i++)
            {
                Score[1, 0] = Score[0, 0] + Del(X[i]);
                for (int j = 1; j < Y.Length; j++)
                {
                    var scoreSub = Score[0, j - 1] + Sub(X[i], Y[j]);
                    var scoreDel = Score[0, j] + Del(X[i]);
                    var scoreIns = Score[1, j - 1] + Ins(Y[j]);
                    Score[1, j] = Math.Max(Math.Max(scoreSub, scoreDel), scoreIns);
                }
                // copy Score[1] to Score [0]
                for (int ij = 0; ij < Score.GetLength(1); ij++)
                {
                    Score[0, ij] = Score[1, ij];
                }
            }

            int[] LastLine = new int[Y.Length];
            for (int j = 0; j < Y.Length; j++)
            {
                LastLine[j] = Score[1, j];
            }
            return LastLine;
        }

        private static int Del(char s)
        {
            return 1;
        }
        private static int Ins(char s)
        {
            return 1;
        }
        private static int Sub(char s1, char s2)
        {
            return s1 == s2 ? 0 : 1;
        }

        public static int CalcLevenshteinDistance(string a, string b)
        {
            if (String.IsNullOrEmpty(a) && String.IsNullOrEmpty(b))
            {
                return 0;
            }
            if (String.IsNullOrEmpty(a))
            {
                return b.Length;
            }
            if (String.IsNullOrEmpty(b))
            {
                return a.Length;
            }
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    int dist =
                        Math.Min
                        (
                            Math.Min(
                                distances[i - 1, j] + 1, // deletion
                                distances[i, j - 1] + 1 // insertion
                                ),
                            distances[i - 1, j - 1] + cost // substitution
                        );
                    distances[i, j] = dist;
                }
            }
            return distances[lengthA, lengthB];
        }
    }
}
