﻿// <copyright file="NonLinearLeastSquaresTest.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

#if NATIVEMKL

using System;
using MathNet.Numerics.Optimization;
using NUnit.Framework;

namespace MathNet.Numerics.UnitTests.OptimizationTests
{
    [TestFixture]
    public class NonLinearLeastSquaresTest
    {
        [Test]
        public void CurveFit()
        {
            var minimizer = new NonLinearLeastSquaresMinimizer();
            minimizer.Options.MaximumIterations = 1010;
            minimizer.Options.Criterion4 = 1e-8;
            // y = b1*(1-exp[-b2*x])  +  e
            var xin = new double[] { 1, 2, 3, 5, 7, 10 };
            var yin = new double[] { 109, 149, 149, 191, 213, 224 };

            // estimated derivative method: does not find best solution.
            //var popt = minimizer.CurveFit(xin, yin, (x, p) => p[0] * (1 - Math.Exp(-p[1] * x)), new double[] { 1, 1 });

            Func<double, double[], double> function = (x, p) => p[0] * (1 - Math.Exp(-p[1] * x));
            Func<double, double[], double[]> jacobian = (x, p) => new double[] {
                1 - Math.Exp(-p[1] * x),
                p[0] * x * Math.Exp(-p[1] * x) };

            var popt = minimizer.CurveFit(xin, yin, function, new double[] { 1, 1 }, jacobian); // 100, 0.75

            double[] expected = new double[] { 2.1380940889E+02, 5.4723748542E-01 };

            double residual = 0;
            for (int i = 0; i < yin.Length; ++i) residual += (yin[i] - function(xin[i], popt)) * (yin[i] - function(xin[i], popt));

            Assert.AreEqual(expected[0], popt[0], 1e-3);
            Assert.AreEqual(expected[1], popt[1], 1e-3);
        }
    }
}

#endif
