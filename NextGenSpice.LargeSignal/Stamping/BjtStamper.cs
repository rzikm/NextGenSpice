using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    public class BjtTransistorStamper
    {
        private ConductanceStamper gbc_;
        private ConductanceStamper gbe_;

        private CurrentStamper ibe;
        private CurrentStamper ibc;
        private CurrentStamper ice;

        private VccsStamper gmf_;
        private ConductanceStamper gmr_;
//        private VccsStamper gmr_;

        public BjtTransistorStamper()
        {
            gbc_ = new ConductanceStamper();
            gbe_ = new ConductanceStamper();

            ibe = new CurrentStamper();
            ibc = new CurrentStamper();
            ice = new CurrentStamper();

            gmf_ = new VccsStamper();
            gmr_ = new ConductanceStamper();
//            gmr_ = new VccsStamper();
        }

        /// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
        /// <param name="adapter">The equation system adapter.</param>
        /// <param name="nBase">Index of base terminal.</param>
        /// <param name="nCollector">Index of collector terminal.</param>
        /// <param name="nEmitter">Index of emitter terminal.</param>
        public void Register(IEquationSystemAdapter adapter, int nBase, int nCollector, int nEmitter)
        {
            gbc_.Register(adapter, nBase, nCollector);
            gbe_.Register(adapter, nBase, nEmitter);

            gmf_.Register(adapter, nCollector, nEmitter, nBase, nEmitter);
            gmr_.Register(adapter, nEmitter, nCollector);

            ibe.Register(adapter, nBase, nEmitter);
            ibc.Register(adapter, nBase, nCollector);
            ice.Register(adapter, nCollector, nEmitter);
        }

        /// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
//        public void Stamp(double gBe, double gBc, double gmf, double gmr, double iB, double iC, double iE)
        public void Stamp(double gBe, double gBc, double gm, double go, double iB, double iC, double iE)
        {
            gbe_.Stamp(gBe);
            gbc_.Stamp(gBc);

            gmf_.Stamp(gm);

            gmr_.Stamp(go);

            ibe.Stamp(iB);
            ibc.Stamp(iC);
            ice.Stamp(iE);
        }
    }


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
        public void Stamp(double gBe, double gBc, double gitr, double gitf, double iB, double iC, double iE)
        {
            nbb.Add(gBc + gBe);
            nbc.Add(-gBc);
            nbe.Add(-gBe);

            ncb.Add(-gBc + gitf + gitr);
            ncc.Add(gBc + gitr);
            nce.Add(-gitf);

            neb.Add(-gBe -gitf - gitr);
            nec.Add(gitr);
            nee.Add(gBe + gitf);

            nb.Add(iB);
            nc.Add(iC);
            ne.Add(iE);
        }
    }
}