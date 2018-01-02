using System.IO;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice
{
    public class PrintStatementDeprecated
    {
        public string AnalysisType { get; set; }
        public Token Token { get; set; }
    }

    public abstract class PrintStatement
    {
        public abstract void Initialize(object model);
        public abstract void PrintHeader(TextWriter output);
        public abstract void PrintValue(TextWriter output);
    }

    public abstract class PrintStatement<TModel> : PrintStatement
    {
        protected PrintStatement()
        {
        }

        public override void Initialize(object model)
        {
            Initialize((TModel)model);
        }

        public abstract void Initialize(TModel model);
    }

    public abstract class LsPrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        public string AnalysisType { get; set; }
    }

    class NodeVoltagePrintStatement : LsPrintStatement
    {
        private readonly string nodeName;
        private readonly int index;
        private LargeSignalCircuitModel model;

        public NodeVoltagePrintStatement(string nodeName, int index)
        {
            this.nodeName = nodeName;
            this.index = index;
        }

        public override void PrintHeader(TextWriter output)
        {
            output.Write($"V({nodeName})");
        }

        public override void PrintValue(TextWriter output)
        {
            output.Write(model.NodeVoltages[index]);
        }

        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = model;
        }
    }

    class ElementVoltagePrintStatement : LsPrintStatement
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDeviceModel model;

        public ElementVoltagePrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        public override void PrintHeader(TextWriter output)
        {
            output.Write($"V({elemName})");
        }

        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Voltage);
        }

        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = (ITwoTerminalLargeSignalDeviceModel) model.GetElement(elemName);
        }
    }

    class ElementCurrentPrintStatement : LsPrintStatement
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDeviceModel model;

        public ElementCurrentPrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        public override void PrintHeader(TextWriter output)
        {
            output.Write($"V({elemName})");
        }

        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Current);
        }

        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = (ITwoTerminalLargeSignalDeviceModel)model.GetElement(elemName);
        }
    }
}