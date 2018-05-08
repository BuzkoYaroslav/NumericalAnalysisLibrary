using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public class RozenbrokMethod : NoConditionExtremumMethod
    {
        private int methodPower;
        private const double leftBound = -5;
        private const double rightBound = 5;

        public RozenbrokMethod(int n): base()
        {
            methodPower = n;
            tableFormat = "{0,5}{1,20}{2,10:F4}{3,5}{4,20}{5,10:F4}{6,10:F4}{7,20}{8,20}{9,10:F4}\n";
        }

        protected override Vector[] FindExtremum(MultiMathFunction func, ExtremumType type, Vector startPoint)
        {
            int multiplier = type == ExtremumType.Maximum ? -1 : 1;

            Vector[] p = new Vector[methodPower];
            Vector alpha = new double[methodPower];
            for (int i = 0; i < methodPower; i++)
                p[i] = InitialCoordinatDirection(i);

            Vector xCurrent = startPoint,
                xPrev,
                yCurrent = startPoint,
                yPrev;
            List<Vector> points = new List<Vector>();
            points.Add(startPoint);
                
            IExtremumSolver solver = new GoldenRatioExtremum();
            
            do
            {
                xPrev = xCurrent;

                for (int j = 0; j < methodPower; j++)
                {
                    yPrev = yCurrent;

                    MathFunction f = func.TransformToSimpleFunction(TransformDictionary(yPrev + new XFunction(1.0) * p[j]));
                    alpha[j] = solver.FindExtremum(multiplier * f, leftBound, rightBound, ExtremumType.Minimum) ?? 0;

                    yCurrent = yPrev + alpha[j] * p[j];

                    points.Add(yCurrent);
                }

                xCurrent = yCurrent;

                if ((xCurrent - xPrev).EuclideanNorm < Epsilon)
                    break;

                ChangeCoordinateDirections(ref p, alpha);
            } while (true);

            return points.ToArray();
        }

        protected override Vector[] FindExtremumWithInfo(MultiMathFunction func, ExtremumType type, Vector startPoint, string debugInfoFile)
        {
            StreamWriter writer = new StreamWriter(debugInfoFile);
            int multiplier = type == ExtremumType.Maximum ? -1 : 1;

            Vector[] p = new Vector[methodPower];
            Vector alpha = new double[methodPower];
            for (int i = 0; i < methodPower; i++)
                p[i] = InitialCoordinatDirection(i);

            Vector xCurrent = startPoint,
                xPrev,
                yCurrent = startPoint,
                yPrev;
            List<Vector> points = new List<Vector>();
            points.Add(startPoint);

            writer.Write(GapForTable(func, type));

            IExtremumSolver solver = new GoldenRatioExtremum();
            int k = 0;
            do
            {
                xPrev = xCurrent;
                double fPrev = func.Calculate(xPrev),
                    fYCurrent = fPrev, fYPrev;

                for (int j = 0; j < methodPower; j++)
                {
                    yPrev = yCurrent;
                    fYPrev = fYCurrent;

                    MathFunction f = func.TransformToSimpleFunction(TransformDictionary(yPrev + new XFunction(1.0) * p[j]));
                    alpha[j] = solver.FindExtremum(multiplier * f, leftBound, rightBound, ExtremumType.Minimum) ?? 0;

                    yCurrent = yPrev + alpha[j] * p[j];
                    fYCurrent = func.Calculate(yCurrent);

                    points.Add(yCurrent);

                    writer.Write(RowForIteration(k, xPrev, fPrev, j, yPrev, fYPrev, alpha[j], p[j], yCurrent, fYCurrent));
                }

                xCurrent = yCurrent;

                if ((xCurrent - xPrev).EuclideanNorm < Epsilon)
                    break;

                ChangeCoordinateDirections(ref p, alpha);
                k++;
            } while (true);

            writer.Close();

            return points.ToArray();
        }

        private Vector InitialCoordinatDirection(int index)
        {
            double[] p = new double[methodPower];
            p[index] = 1;

            return p;
        }

        private Dictionary<uint, MathFunction> TransformDictionary(MathFunction[] f)
        {
            Dictionary<uint, MathFunction> transform = new Dictionary<uint, MathFunction>();

            for (int i = 0; i < f.Length; i++)
                transform.Add((uint)i, f[i]);

            return transform;
        }

        private void ChangeCoordinateDirections(ref Vector[] p, Vector alpha)
        {
            Vector[] a = new Vector[p.Length];
            Vector[] b = new Vector[p.Length];

            for (int j = 0; j < p.Length; j++)
            {
                a[j] = new double[p.Length];

                if (alpha[j] == 0)
                    a[j] = p[j];
                else
                    for (int k = j; k < p.Length; k++)
                        a[j] += alpha[k] * p[k];

                b[j] = a[j];
                for (int k = 0; k < j; k++)
                    b[j] -= (a[j] * (b[k] * (1.0 / b[k].EuclideanNorm))) * b[k] * (1.0 / b[k].EuclideanNorm);
             }

            p = b.Select(x => x * (1 / x.EuclideanNorm)).ToArray();
        }

        protected override string GapForTable(MultiMathFunction func, ExtremumType type)
        {
            string result = string.Format("Function info:\nF(X) = {0}\nMethod Info:\n{1}\nTask: F(X) -> {2}\n", 
                func, this, type == ExtremumType.Minimum ? "min" : "max");

            result += string.Format(tableFormat, 
                "k", "x^k", "f(x^k)", "j", "y_j^k", "f(y_j^k)", "alpha_j^k", "p_j^k", "y_j+1^k", "f(y_j+1^k)");

            return result;
        }
        protected override string RowForIteration(params object[] args)
        {
            return string.Format(tableFormat,
                args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
        }
    }
}
