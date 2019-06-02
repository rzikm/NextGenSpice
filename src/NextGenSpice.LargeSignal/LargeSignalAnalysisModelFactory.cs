using System.Composition;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Devices;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Class for creating <see cref="LargeSignalCircuitModel" /> from circuit definition.</summary>
    [Export(typeof(IAnalysisModelFactory<LargeSignalCircuitModel>))]
    public class LargeSignalAnalysisModelFactory : AnalysisModelFactory<LargeSignalCircuitModel>
    {
        public LargeSignalAnalysisModelFactory()
        {
            // register default models
            SetModel<Resistor, LargeSignalResistor>(e => new LargeSignalResistor(e));
            SetModel<CurrentSource, LargeSignalCurrentSource>((e, ctx) =>
                new LargeSignalCurrentSource(e));
            SetModel<VoltageSource, LargeSignalVoltageSource>((e, ctx) =>
                new LargeSignalVoltageSource(e));
            SetModel<Capacitor, LargeSignalCapacitor>(e => new LargeSignalCapacitor(e));
            SetModel<Inductor, LargeSignalInductor>(e => new LargeSignalInductor(e));
            SetModel<Diode, LargeSignalDiode>(e => new LargeSignalDiode(e));
            SetModel<Bjt, LargeSignalBjt>(e => new LargeSignalBjt(e));
            SetModel<Vcvs, LargeSignalVcvs>(e => new LargeSignalVcvs(e));
            SetModel<Vccs, LargeSignalVccs>(e => new LargeSignalVccs(e));
            SetModel<Ccvs, LargeSignalCcvs>((e,ctx) => new LargeSignalCcvs(e, (LargeSignalVoltageSource) ctx.GetModel(e.Ampermeter)));
            SetModel<Cccs, LargeSignalCccs>((e,ctx) => new LargeSignalCccs(e, (LargeSignalVoltageSource) ctx.GetModel(e.Ampermeter)));

            SetModel<Subcircuit, LargeSignalSubcircuit>((e, ctx) =>
                new LargeSignalSubcircuit(e,
                    e.Devices.Select(ctx.CreateSubcontext().GetModel).Cast<ILargeSignalDevice>()));
        }


        /// <summary>Factory method for creating the actual instance of the analysis model.</summary>
        /// <param name="context">Current instantiation context.</param>
        /// <returns></returns>
        protected override LargeSignalCircuitModel NewInstance(
            IModelInstantiationContext<LargeSignalCircuitModel> context)
        {
            var devices = context.CircuitDefinition.Devices
                .Select(context.GetModel).Cast<ILargeSignalDevice>().ToList();

            return new LargeSignalCircuitModel(context.CircuitDefinition.InitialVoltages, devices);
        }
    }
}