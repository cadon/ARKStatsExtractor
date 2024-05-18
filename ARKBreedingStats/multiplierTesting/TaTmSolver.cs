using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.multiplierTesting
{
    /// <summary>
    /// Solves Ta*TaM and Tm*TmM with two given equations.
    /// </summary>
    internal class TaTmSolver
    {
        // The general stat equation is
        // V = (B * ( 1 + Lw * Iw * IwM) * TBHM * (1 + IB * IBs * IBM) + Ta * TaM) * (1 + TE * Tm * TmM) * (1 + Ld * Id * IdM)
        // Assuming all values are known except the products Ta * TaM and Tm * TmM
        // this is the case for server multiplier determination or species multiplier determination
        // Using variables a = B * ( 1 + Lw * Iw * IwM) * TBHM * (1 + IB * IBs * IBM),
        // b = 1 + Ld * Id * IdM,
        // ta = Ta * TaM,
        // tm = Tm * TmM,
        // W = V/b
        // the formula is
        // W = (a + ta) * (1 + TE * tm)

        // f like first equation, s like second equation
        private double _fW;
        private double _fA;
        private double _fTe;

        public void SetFirstEquation(double statValue, double baseValue, double lw, double iw, double iwm, double tbhm,
            double ib, double ibs, double ibm, double te, double ld, double id, double idm)
        {
            _fW = statValue / (1 + ld * id * idm);
            _fA = baseValue * (1 + lw * iw * iwm) * tbhm * (1 + ib * ibs * ibm);
            _fTe = te;
        }

        /// <summary>
        /// Calculate the products of Ta * TaM and Tm * TmM with a second equation.
        /// Returns error text or null on success.
        /// </summary>
        public string CalculateTaTm(double statValue, double baseValue, double lw, double iw, double iwm, double tbhm,
            double ib, double ibs, double ibm, double te, double ld, double id, double idm, out double taTaM, out double tmTmM)
        {
            var sW = statValue / (1 + ld * id * idm);
            var sA = baseValue * (1 + lw * iw * iwm) * tbhm * (1 + ib * ibs * ibm);
            var sTe = te;

            taTaM = 0;
            tmTmM = 0;

            if (_fTe == sTe)
            {
                return "Both TE values need to be different";
            }

            var squareRootPart = Math.Sqrt(
                Math.Pow(_fA * _fTe - _fA * sTe + sA * _fTe - sA * sTe - _fTe * sW + sTe * _fW, 2) - 4 * (_fTe - sTe)
                * (_fA * sA * _fTe - _fA * sA * sTe - _fA * _fTe * sW + sA * sTe * _fW));
            var secondPart = -_fA * _fTe + _fA * sTe;
            var thirdPart = -sA * _fTe + sA * sTe;
            var dividend = _fTe * (-squareRootPart + _fA * _fTe - _fA * sTe - sA * _fTe + sA * sTe + _fTe * sW - sTe * _fW);

            if (dividend == 0)
            {
                return "div by 0 error";
            }

            taTaM = (-squareRootPart + secondPart + thirdPart + _fTe * sW - sTe * _fW) / (2 * (_fTe - sTe));
            tmTmM = (squareRootPart + secondPart - thirdPart + 2 * _fTe * _fW - _fTe * sW - sTe * _fW) / dividend;

            return null;
        }
    }
}
