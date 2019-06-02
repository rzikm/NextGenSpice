using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	public class BjtTransistorStamper
	{
		private readonly ConductanceStamper gbc_;
		private readonly ConductanceStamper gbe_;

		private readonly VccsStamper gmf_;
		private readonly VccsStamper gmr_;
		private readonly CurrentStamper ibc;

		private readonly CurrentStamper ibe;

		public BjtTransistorStamper()
		{
			gbc_ = new ConductanceStamper();
			gbe_ = new ConductanceStamper();

			ibe = new CurrentStamper();
			ibc = new CurrentStamper();

			gmf_ = new VccsStamper();
			gmr_ = new VccsStamper();
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
			gmr_.Register(adapter, nCollector, nEmitter, nEmitter, nCollector);

			ibe.Register(adapter, nBase, nEmitter);
			ibc.Register(adapter, nBase, nCollector);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double gBe, double gBc, double gmf, double gmr, double ibeeq, double ibceq)
		{
			gbe_.Stamp(gBe);
			gbc_.Stamp(gBc);

			gmf_.Stamp(gmf);
			gmr_.Stamp(gmr);

			ibe.Stamp(ibeeq);
			ibc.Stamp(ibceq);
		}
	}
}