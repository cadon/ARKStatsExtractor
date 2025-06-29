namespace ARKBreedingStats.multiplierTesting
{
    /// <summary>
    /// Calculates multipliers depending on other given values with the stat value equation.
    /// </summary>
    public static class CalculateMultipliers
    {
        /// <summary>
        /// Calculates the Increase per wild level multiplier (IwM) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? IwM(double statValue, double baseValue, int wildLevel, double iw, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer, bool domesticated, double te, int domLevel, double id, double idM, double idSingleplayer, double ib, double ibm, double sIBM)
        {
            if (wildLevel == 0 || iw == 0) return null;

            return ((statValue / (domesticated ? (1 + te * tm * (tm > 0 ? tmM * tmSingleplayer : 1)) * (1 + domLevel * id * idSingleplayer * idM) : 1) - (domesticated ? ta * (ta > 0 ? taM * taSingleplayer : 1) : 0)) / (baseValue * (domesticated ? tbhm : 1) * (domesticated ? 1 + ib * ibm * sIBM : 1)) - 1) / (wildLevel * iw * iwSingleplayer);
        }

        /// <summary>
        /// Calculates the Increase per wild level (Iw) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? Iw(double statValue, double baseValue, int wildLevel, double iwM, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer, bool domesticated, double te, int domLevel, double id, double idM, double idSingleplayer, double ib, double ibm, double sIBM)
        {
            if (wildLevel == 0 || iwM == 0) return null;

            return ((statValue / (domesticated ? (1 + te * tm * (tm > 0 ? tmM * tmSingleplayer : 1)) * (1 + domLevel * id * idSingleplayer * idM) : 1) - (domesticated ? ta * (ta > 0 ? taM * taSingleplayer : 1) : 0)) / (baseValue * (domesticated ? tbhm : 1) * (domesticated ? 1 + ib * ibm * sIBM : 1)) - 1) / (wildLevel * iwM * iwSingleplayer);
        }

        /// <summary>
        /// Stat value after taming with bonus, without dom levels.
        /// </summary>
        private static double ValueDomWithoutDomLevel(double baseValue, int wildLevel, double iw, double iwM, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer, bool domesticated, double te, double ib, double ibm, double sIBM)
        {
            // ValueWild
            double valueWild = baseValue * (1 + wildLevel * iw * iwSingleplayer * iwM);

            if (domesticated)
            {
                // ValueDom
                return (valueWild * tbhm * (1 + ib * ibm * sIBM) + ta * (ta > 0 ? taM * taSingleplayer : 1))
                     * (1 + (tm > 0 ? te * tm * tmM * tmSingleplayer : tm));
            }
            return valueWild;
        }

        /// <summary>
        /// Stat value after taming with bonus, with dom levels.
        /// </summary>
        private static double ValueDomWithDomLevel(double baseValue, int wildLevel, double iw, double iwM, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer,
            bool domesticated, double te, int domLevel, double id, double idM, double idSingleplayer, double ib, double ibm, double sIBM, out double valueDomWithoutDomLevels)
        {
            valueDomWithoutDomLevels = ValueDomWithoutDomLevel(baseValue, wildLevel, iw, iwM, iwSingleplayer, tbhm, ta, taM, taSingleplayer, tm, tmM, tmSingleplayer, domesticated, te, ib, ibm, sIBM);

            if (domesticated)
            {
                return valueDomWithoutDomLevels * (1 + domLevel * id * idSingleplayer * idM);
            }
            return valueDomWithoutDomLevels;
        }

        /// <summary>
        /// Calculates the Increase per domesticated level multiplier (IdM) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? IdM(double statValue, double baseValue, int wildLevel, double iw, double iwM, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer, bool domesticated, double te, int domLevel, double id, double idM, double idSingleplayer, double ib, double ibm, double sIBM)
        {
            return IdM(statValue, ValueDomWithoutDomLevel(baseValue, wildLevel, iw, iwM, iwSingleplayer, tbhm, ta, taM, taSingleplayer, tm, tmM, tmSingleplayer, domesticated, te, ib, ibm, sIBM), domLevel, id, idSingleplayer);
        }

        /// <summary>
        /// Calculates the Increase per domesticated level multiplier (IdM) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? IdM(double statValue, double valueDom, int domLevel, double id, double idSingleplayer)
        {
            if (valueDom == 0 || domLevel == 0 || id == 0) return null;

            return (statValue / valueDom - 1) / (domLevel * id * idSingleplayer);
        }

        /// <summary>
        /// Calculates the Increase per domesticated level (Id) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? Id(double statValue, double valueDom, int domLevel, double idM, double idSingleplayer)
        {
            if (valueDom == 0 || domLevel == 0 || idM == 0) return null;

            return (statValue / valueDom - 1) / (domLevel * idM * idSingleplayer);
        }

        /// <summary>
        /// Calculates the Tame additive bonus multiplier (TaM) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? TaM(double statValue, double baseValue, int wildLevel, double iw, double iwM, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer, bool domesticated, double te, int domLevel, double id, double idM, double idSingleplayer, double ib, double ibm, double sIBM)
        {
            if (ta == 0) return null;
            var valueDomLeveled = ValueDomWithDomLevel(baseValue, wildLevel, iw, iwM, iwSingleplayer, tbhm, ta, taM, taSingleplayer, tm, tmM, tmSingleplayer, domesticated, te, domLevel, id, idM, idSingleplayer, ib, ibm, sIBM, out double valueDomWithoutDomLevels);
            if (valueDomLeveled == 0) return null;

            return (statValue * valueDomWithoutDomLevels / (valueDomLeveled * (1 + te *
                    tm * (tm > 0 ? tmM * tmSingleplayer : 1))) -
                baseValue *
                (1 + wildLevel * iw * iwSingleplayer * iwM) *
                tbhm * (1 + ib * ibm * sIBM)) / (ta * taSingleplayer);
        }

        /// <summary>
        /// Calculates the Tame multiplicative bonus multiplier (TmM) to the value that solves the equation, assuming all other values are correct.
        /// </summary>
        public static double? TmM(double statValue, double baseValue, int wildLevel, double iw, double iwM, double iwSingleplayer, double tbhm, double ta, double taM, double taSingleplayer, double tm, double tmM, double tmSingleplayer, bool domesticated, double te, int domLevel, double id, double idM, double idSingleplayer, double ib, double ibm, double sIBM)
        {
            if (te <= 0 || tm == 0) return null;
            var valueDomLeveled = ValueDomWithDomLevel(baseValue, wildLevel, iw, iwM, iwSingleplayer, tbhm, ta, taM, taSingleplayer, tm, tmM, tmSingleplayer, domesticated, te, domLevel, id, idM, idSingleplayer, ib, ibm, sIBM, out double valueDomWithoutDomLevels);
            if (valueDomWithoutDomLevels == 0) return null;

            return (statValue * valueDomWithoutDomLevels / (valueDomLeveled * (baseValue * (1 + wildLevel * iw * iwSingleplayer * iwM) * tbhm * (1 + ib * ibm * sIBM) + ta * (ta > 0 ? taM * taSingleplayer : 1))) - 1) / (te * tm * tmSingleplayer);
        }
    }
}
