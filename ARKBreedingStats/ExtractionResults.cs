using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    class ExtractionResults
    {
        public List<StatResult>[] results = new List<StatResult>[8]; // stores the possible results of all stats as array (wildlevel, domlevel, tamingEff)
        public int[] chosenResults = new int[8];
        public bool[] fixedResults = new bool[8];
        public List<int> statsWithEff = new List<int>();
        public bool validResults;
        public bool postTamed;
        public int[] levelDomFromTorporAndTotalRange = new int[] { 0, 0 }, levelWildFromTorporRange = new int[] { 0, 0 }; // 0: min, 1: max
        public int[] lowerBoundWilds = new int[8], lowerBoundDoms = new int[8], upperBoundDoms = new int[8];
        public int wildFreeMax = 0, domFreeMin = 0, domFreeMax = 0; // unassigned levels

        public ExtractionResults()
        {
            for (int s = 0; s < 8; s++)
            {
                chosenResults[s] = 0;
                fixedResults[s] = false;
                results[s] = new List<StatResult>();
            }
        }

        public void Clear()
        {
            for (int s = 0; s < 8; s++)
            {
                chosenResults[s] = 0;
                fixedResults[s] = false;
                results[s].Clear();
            }
            validResults = false;
            statsWithEff.Clear();
        }

        public double uniqueTE()
        {
            double eff = -2;
            if (statsWithEff.Count > 0 && results[statsWithEff[0]].Count > chosenResults[statsWithEff[0]])
            {
                eff = results[statsWithEff[0]][chosenResults[statsWithEff[0]]].TE;
                for (int s = 1; s < statsWithEff.Count; s++)
                {
                    // efficiency-calculation can be a bit off due to ingame-rounding
                    if (results[statsWithEff[s]].Count <= chosenResults[statsWithEff[s]] || Math.Abs(results[statsWithEff[s]][chosenResults[statsWithEff[s]]].TE - eff) > 0.0025)
                    {
                        return -1; // no unique TE
                    }
                }
            }
            return eff;
        }

        /// <summary>
        /// Marks the results as invalid that violate the given bounds assuming the fixedResults are true
        /// </summary>
        public int filterResultsByFixed()
        {
            int[] lowBoundWs = (int[])lowerBoundWilds.Clone();
            int[] lowBoundDs = (int[])lowerBoundDoms.Clone();
            int[] uppBoundDs = (int[])upperBoundDoms.Clone();
            int wildMax = wildFreeMax, domMin = domFreeMin, domMax = domFreeMax;

            // set all results to non-valid that are in a fixed stat and not the chosen one
            for (int s = 0; s < 7; s++)
            {
                for (int r = 0; r < results[s].Count; r++)
                {
                    results[s][r].currentlyNotValid = (fixedResults[s] && r != chosenResults[s]);
                }
                // subtract fixed stat-levels
                if (fixedResults[s])
                {
                    wildMax -= results[s][chosenResults[s]].levelWild;
                    domMin -= results[s][chosenResults[s]].levelDom;
                    domMax -= results[s][chosenResults[s]].levelDom;
                    lowBoundWs[s] = 0;
                    lowBoundDs[s] = 0;
                    uppBoundDs[s] = 0;
                }
            }


            // mark all results as invalid that are not possible with the current fixed chosen results
            // loop as many times as necessary to remove results that depends on the invalidation of results in a later stat
            bool loopAgain = true;
            int validResults, validR1, validR2, uniqueR;
            while (loopAgain)
            {
                loopAgain = false;
                for (int s = 0; s < 7; s++)
                {
                    validResults = 0;
                    validR1 = -1;
                    validR2 = -1;
                    uniqueR = -1;
                    for (int r = 0; r < results[s].Count; r++)
                    {
                        if (!results[s][r].currentlyNotValid)
                            validResults++;
                    }
                    if (validResults > 1)
                    {
                        for (int r = 0; r < results[s].Count; r++)
                        {
                            if (!results[s][r].currentlyNotValid && (results[s][r].levelWild > wildMax - lowBoundWs.Sum() + lowBoundWs[s] || results[s][r].levelDom > domMax - lowBoundDs.Sum() + lowBoundDs[s] || results[s][r].levelDom < domMin - uppBoundDs.Sum() + uppBoundDs[s]))
                            {
                                results[s][r].currentlyNotValid = true;
                                validResults--;
                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (validResults == 1)
                                {
                                    // find unique valid result
                                    for (int rr = 0; rr < results[s].Count; rr++)
                                    {
                                        if (!results[s][rr].currentlyNotValid)
                                        {
                                            uniqueR = rr;
                                            break;
                                        }
                                    }
                                    loopAgain = true;
                                    wildMax -= results[s][uniqueR].levelWild;
                                    domMin -= results[s][uniqueR].levelDom;
                                    domMax -= results[s][uniqueR].levelDom;
                                    lowBoundWs[s] = 0;
                                    lowBoundDs[s] = 0;
                                    uppBoundDs[s] = 0;
                                    if (wildMax < 0 || domMax < 0)
                                    {
                                        return s;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return -1; // -1 is good for this function. A value >=0 means that stat is faulty
        }
    }
}
