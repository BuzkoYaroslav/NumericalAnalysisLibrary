using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class SLAE
    {
        public delegate Vector Solver(SLAE system);

        private Matrix AInitial, ACurrent;
        private Vector fInitial, fCurrent;

        private Vector solution = null;

        private int[] order;

        public SLAE(Matrix A, Vector f)
        {
            if (f.Count != A.RowsCount)
                throw new Exception(Constants.SLAEConstants.incorrectMatrixAndVectorSizesString);

            AInitial = (double[,])A;
            ACurrent = (double[,])A;

            fInitial = (double[])f;
            fCurrent = (double[])f;

            InitOrder();
        }

        public double this[int index]
        {
            get
            {
                return fCurrent[index];
            }
        }
        public double this[int i, int j]
        {
            get
            {
                return ACurrent[i, j];
            }
        }

        public Matrix SystemMatrixCurrent
        {
            get
            {
                return ACurrent;
            }
        }
        public Matrix RightCurrent
        {
            get
            {
                return fCurrent;
            }
        }

        public Matrix SystemMatrix
        {
            get
            {
                return AInitial;
            }
        }
        public Vector RightPart
        {
            get
            {
                return fInitial;
            }
        }

        public int UnknownVariablesCount
        {
            get
            {
                return AInitial.ColumnsCount;
            }
        }
        public int EquasionsCount
        {
            get
            {
                return AInitial.RowsCount;
            }
        }

        public void Solve(Solver method)
        {
            solution = method(this);
        }

        public double Soltion(int index)
        {
            if (solution == null)
                solution = RetrieveSolution(false);

            return solution[order[index]];
        }
        private void InitOrder()
        {
            order = new int[AInitial.ColumnsCount];
            for (int i = 0; i < order.Length; i++)
                order[i] = i;
        }

        public int FindNoZeroColumn(int row, int startColumn)
        {
            for (int i = startColumn; i < ACurrent.ColumnsCount; i++)
            {
                if (ACurrent[row, i] != 0)
                    return i;
            }

            return -1;
        }
        public void MakeCoeficientEqualToOne(int row, int column)
        {
            if (ACurrent[row, column] == 1)
                return;

            double coef = 1.0 / ACurrent[row, column];

            for (int i = 0; i < ACurrent.ColumnsCount; i++)
                ACurrent[row, i] *= coef;

            fCurrent[row] *= coef;
        }
        public void MakeDownEqualToZero(int row, int column)
        {
            for (int i = row + 1; i < ACurrent.RowsCount; i++)
            {
                double coef = -ACurrent[i, column];

                for (int j = row; j < ACurrent.ColumnsCount; j++)
                    ACurrent[i, j] += ACurrent[row, j] * coef;

                fCurrent[i] += fCurrent[row] * coef;
            }
        }

        public void SwapColumns(int col1, int col2)
        {
            ACurrent.SwapColumns(col1, col2);

            int tmp = order[col1];
            order[col1] = order[col2];
            order[col2] = tmp;
        }

        private Vector RetrieveSolution(bool upDown)
        {
            double[] solution = new double[ACurrent.ColumnsCount];
            bool[] isInitialize = new bool[ACurrent.ColumnsCount];

            for (int i = 0; i < solution.Length; i++)
            {
                solution[i] = 1;
                isInitialize[i] = false;
            }

            for (int i = upDown ? 0 : ACurrent.RowsCount - 1; (upDown && i < ACurrent.RowsCount) || 
                                                              (!upDown && i >= 0); 
                                                              i += upDown ? 1 : -1)
            {
                int index = FindNoZeroColumn(i, i);

                double newSolution = 0;

                for (int j = upDown ? 0 : index + 1; (upDown && j < index) || (!upDown && j < ACurrent.ColumnsCount); j++)
                {
                    newSolution -= ACurrent[i, j] * solution[j];
                }

                newSolution += fCurrent[i];
                newSolution /= ACurrent[i, index];

                if (newSolution != solution[index] && isInitialize[i])
                    throw new Exception(ResponseAboutExceptionalSolution(false));

                solution[index] = newSolution;
                isInitialize[i] = true;
            }

            return solution;
        }
        public Vector InitialOrderSolution(bool upDown)
        {
            if (this.solution == null)
                this.solution = RetrieveSolution(upDown);

            Vector solution = (double[])this.solution;

            for (int i = 0; i < UnknownVariablesCount; i++)
            {
                double tmp = solution[i];
                solution[i] = solution[order[i]];
                solution[order[i]] = tmp;
            }

            return solution;
        }

        public Vector Deviation()
        {
            return AInitial * solution - fInitial;
        }

        public static string ResponseAboutExceptionalSolution(bool hasZeroRow)
        {
            return hasZeroRow ? Constants.SLAEConstants.infinitCountOfSolutionsString 
                : Constants.SLAEConstants.noSolutionsString;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < EquasionsCount; i++)
            {
                for (int j = 0; j < UnknownVariablesCount; j++)
                {
                    if (AInitial[i, j] == 1)
                        result += "X" + j.ToString();
                    else
                        result += AInitial[i, j] + " * X" + j.ToString();

                    if (j == UnknownVariablesCount - 1)
                        result += " = ";
                    else
                        result += " + ";
                }

                result += fInitial[i] + "\n"; 
            }

            return result;
        }
        public string SolutionString()
        {
            string result = "";

            for (int i = 0; i < solution.Count; i++)
                result += "X" + i.ToString() + " = " + Math.Round(solution[i], 2) + "\n";

            return result;
        }

    }
}
