using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Helper class for stamping BJT transistor devices onto the equation system.</summary>
    public class BjtStamper
    {
        private IEquationSystemCoefficientProxy nb;
        private IEquationSystemCoefficientProxy nbb;
        private IEquationSystemCoefficientProxy nbc;
        private IEquationSystemCoefficientProxy nbe;
        private IEquationSystemCoefficientProxy nc;
        private IEquationSystemCoefficientProxy ncb;
        private IEquationSystemCoefficientProxy ncc;
        private IEquationSystemCoefficientProxy nce;
        private IEquationSystemCoefficientProxy ne;
        private IEquationSystemCoefficientProxy neb;
        private IEquationSystemCoefficientProxy nec;
        private IEquationSystemCoefficientProxy nee;

        /// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
        /// <param name="adapter">The equation system adapter.</param>
        /// <param name="nBase">Index of base terminal.</param>
        /// <param name="nCollector">Index of collector terminal.</param>
        /// <param name="nEmitter">Index of emitter terminal.</param>
        public void Register(IEquationSystemAdapter adapter, int nBase, int nCollector, int nEmitter)
        {
            nbb = adapter.GetMatrixCoefficientProxy(nBase, nBase);
            nbc = adapter.GetMatrixCoefficientProxy(nBase, nCollector);
            nbe = adapter.GetMatrixCoefficientProxy(nBase, nEmitter);
            ncb = adapter.GetMatrixCoefficientProxy(nCollector, nBase);
            ncc = adapter.GetMatrixCoefficientProxy(nCollector, nCollector);
            nce = adapter.GetMatrixCoefficientProxy(nCollector, nEmitter);
            neb = adapter.GetMatrixCoefficientProxy(nEmitter, nBase);
            nec = adapter.GetMatrixCoefficientProxy(nEmitter, nCollector);
            nee = adapter.GetMatrixCoefficientProxy(nEmitter, nEmitter);

            nb = adapter.GetRightHandSideCoefficientProxy(nBase);
            nc = adapter.GetRightHandSideCoefficientProxy(nCollector);
            ne = adapter.GetRightHandSideCoefficientProxy(nEmitter);
        }

        /// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
        public void Stamp(double gBe, double gBc, double gMf, double gMr, double iB, double iC, double iE)
        {
            nbb.Add(gBe + gBc);
            nbc.Add(-gBc);
            nbe.Add(-gBe);

            ncb.Add(-gBc + gMf + gMr);
            ncc.Add(gBc - gMr);
            nce.Add(-gMf);

            neb.Add(-gBe - gMf - gMr);
            nec.Add(+gMr);
            nee.Add(gBe + gMf);

            nb.Add(iB);
            nc.Add(iC);
            ne.Add(iE);
        }
    }
}