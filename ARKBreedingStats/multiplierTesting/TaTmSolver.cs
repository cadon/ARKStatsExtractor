using System;

namespace ARKBreedingStats.multiplierTesting
{
    /// <summary>
    /// Solves Ta*TaM and Tm*TmM or Ta*TaM and TBHM with two given equations.
    /// </summary>
    internal class TaTmSolver
    {
        // The general stat equation is
        // V = (B * ( 1 + Lw * Iw * IwM) * TBHM * (1 + IB * IBs * IBM) + Ta * TaM) * (1 + TE * Tm * TmM) * (1 + Ld * Id * IdM)
        // Assuming all values are known except the products Ta * TaM and Tm * TmM
        // this is the case for server multiplier determination or species multiplier determination
        // Using variables
        // x = B * ( 1 + Lw * Iw * IwM) * (1 + IB * IBs * IBM),
        // h = TBHM
        // y = 1 + Ld * Id * IdM,
        // a = Ta * TaM,
        // m = Tm * TmM,
        // W = V/y
        // the formula is
        // W = (x * h + a) * (1 + TE * m)
        // for solver W = (x * h + a) * (1 + t * m), U = (x_2 * h + a) * (1 + t_2 * m) for a, m
        // for solver W = x * h + a, U = x_2 * h + a for h, a

        // f like first equation, s like second equation
        private double _fW;
        private double _fX;
        private double _fXWithoutTbhm;
        private double _fTe;

        /// <summary>
        /// Define first equation with distinct taming effectiveness.
        /// </summary>
        /// <param name="statValue"></param>
        /// <param name="baseValue"></param>
        /// <param name="lw">wild levels</param>
        /// <param name="iw">increase per wild level of this species</param>
        /// <param name="iwm">increase per wild level server multiplier</param>
        /// <param name="tbhm">tamed bonus health multiplier of species</param>
        /// <param name="ib">imprinting bonus of creature</param>
        /// <param name="ibs">imprinting bonus species stat multiplier</param>
        /// <param name="ibm">imprinting bonus multiplier of server</param>
        /// <param name="te">taming effectiveness</param>
        /// <param name="ld">domestic levels</param>
        /// <param name="id">increase per domestic level of species</param>
        /// <param name="idm">increase per domestic level server multiplier</param>
        public void SetFirstEquation(double statValue, double baseValue, double lw, double iw, double iwm, double tbhm,
            double ib, double ibs, double ibm, double te, double ld, double id, double idm)
        {
            SetValues(statValue, baseValue, lw, iw, iwm, tbhm, ib, ibs, ibm, te, ld, id, idm, out _fW, out _fX, out _fXWithoutTbhm, out _fTe);
        }

        private void SetValues(double statValue, double baseValue, double lw, double iw, double iwm, double tbhm,
            double ib, double ibs, double ibm, double te, double ld, double id, double idm, out double w,
            out double x, out double xWithoutTbhm, out double teOut)
        {
            w = statValue / (1 + ld * id * idm);
            xWithoutTbhm = baseValue * (1 + lw * iw * iwm) * (1 + ib * ibs * ibm);
            x = xWithoutTbhm * tbhm;
            teOut = te;
        }

        /// <summary>
        /// Calculate the products of Ta * TaM and Tm * TmM with a second equation.
        /// Returns error text or null on success.
        /// </summary>
        /// <param name="statValue"></param>
        /// <param name="baseValue"></param>
        /// <param name="lw">wild levels</param>
        /// <param name="iw">increase per wild level of this species</param>
        /// <param name="iwm">increase per wild level server multiplier</param>
        /// <param name="tbhm">tamed bonus health multiplier of species</param>
        /// <param name="ib">imprinting bonus of creature</param>
        /// <param name="ibs">imprinting bonus species stat multiplier</param>
        /// <param name="ibm">imprinting bonus multiplier of server</param>
        /// <param name="te">taming effectiveness</param>
        /// <param name="ld">domestic levels</param>
        /// <param name="id">increase per domestic level of species</param>
        /// <param name="idm">increase per domestic level server multiplier</param>
        /// <param name="taTaM">product of taming additive bonus of species and server multiplier</param>
        /// <param name="tmTmM">product of taming multiplicative bonus of species and server multiplier</param>
        public string CalculateTaTm(double statValue, double baseValue, double lw, double iw, double iwm, double tbhm,
            double ib, double ibs, double ibm, double te, double ld, double id, double idm, out double taTaM, out double tmTmM)
        {
            SetValues(statValue, baseValue, lw, iw, iwm, tbhm, ib, ibs, ibm, te, ld, id, idm, out var sW, out var sX, out _, out var sTe);

            taTaM = 0;
            tmTmM = 0;

            if (Math.Abs(_fTe - sTe) < 0.005)
            {
                return $"The taming effectiveness (TE) values need to be more different (given values are {_fTe:p1} and {sTe:p1})";
            }

            var squareRootPart = Math.Sqrt(
                Math.Pow(_fX * _fTe - _fX * sTe + sX * _fTe - sX * sTe - _fTe * sW + sTe * _fW, 2) - 4 * (_fTe - sTe)
                * (_fX * sX * _fTe - _fX * sX * sTe - _fX * _fTe * sW + sX * sTe * _fW));
            var secondPart = -_fX * _fTe + _fX * sTe;
            var thirdPart = -sX * _fTe + sX * sTe;
            var dividend = _fTe * (+squareRootPart + _fX * _fTe - _fX * sTe - sX * _fTe + sX * sTe + _fTe * sW - sTe * _fW);
            var useSecondOption = dividend == 0;

            // there are multiple possible solutions.
            // taTaM can be negative, assume tmTmM is always positive
            // sometimes both options are possible, by a few tests the second option results in odd values unlikely to be correct, but this might be false

            if (!useSecondOption)
            {
                // first option
                taTaM = (squareRootPart + secondPart + thirdPart + _fTe * sW - sTe * _fW) / (2 * (_fTe - sTe));
                tmTmM = (-squareRootPart + secondPart - thirdPart + 2 * _fTe * _fW - _fTe * sW - sTe * _fW) / dividend;
                if (tmTmM >= 0) return null; // no error
            }

            // second option
            dividend = _fTe * (-squareRootPart + _fX * _fTe - _fX * sTe - sX * _fTe + sX * sTe + _fTe * sW - sTe * _fW);
            if (dividend == 0)
            {
                return "div by 0 error";
            }

            taTaM = (-squareRootPart + secondPart + thirdPart + _fTe * sW - sTe * _fW) / (2 * (_fTe - sTe));
            tmTmM = (squareRootPart + secondPart - thirdPart + 2 * _fTe * _fW - _fTe * sW - sTe * _fW) / dividend;

            return null; // no error
        }

        /// <summary>
        /// Calculate the products of Ta * TaM and TBHM with a second equation, assuming Tm == 0.
        /// Returns error text or null on success.
        /// </summary>
        /// <param name="statValue"></param>
        /// <param name="baseValue"></param>
        /// <param name="lw">wild levels</param>
        /// <param name="iw">increase per wild level of this species</param>
        /// <param name="iwm">increase per wild level server multiplier</param>
        /// <param name="ib">imprinting bonus of creature</param>
        /// <param name="ibs">imprinting bonus species stat multiplier</param>
        /// <param name="ibm">imprinting bonus multiplier of server</param>
        /// <param name="te">taming effectiveness</param>
        /// <param name="ld">domestic levels</param>
        /// <param name="id">increase per domestic level of species</param>
        /// <param name="idm">increase per domestic level server multiplier</param>
        /// <param name="taTaM">product of taming additive bonus of species and server multiplier</param>
        /// <param name="tbhm">tamed bonus health multiplier of species</param>
        public string CalculateTaTbhm(double statValue, double baseValue, double lw, double iw, double iwm,
            double ib, double ibs, double ibm, double te, double ld, double id, double idm, out double taTaM,
            out double tbhm)
        {
            SetValues(statValue, baseValue, lw, iw, iwm, 1, ib, ibs, ibm, te, ld, id, idm, out var sW, out _, out var sXWithoutTbhm, out _);

            taTaM = 0;
            tbhm = 0;

            var dividend = sXWithoutTbhm - _fXWithoutTbhm;

            if (Math.Abs(dividend) < 0.005)
            {
                return "The wild levels need to be more different to calculate TBHM";
            }

            taTaM = (_fW * sXWithoutTbhm - sW * _fXWithoutTbhm) / dividend;
            tbhm = (sW - _fW) / dividend;
            return null; // no error
        }
    }
}
