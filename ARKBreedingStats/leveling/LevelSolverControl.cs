using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.leveling
{
    public sealed class LevelSolverControl : UserControl
    {
        private static readonly string[] TargetModeNames = { "At least", "Maximize" };

        private sealed class BufferedTableLayoutPanel : TableLayoutPanel
        {
            public BufferedTableLayoutPanel()
            {
                DoubleBuffered = true;
            }
        }

        private sealed class StatRow
        {
            public int StatIndex;
            public int RowIndex;
            public bool Available;
            public Label Name;
            public ComboBox Mode;
            public Nud Target;
            public Nud Priority;
            public Nud Wild;
            public Label Mutation;
            public Label Domestic;
            public Label Result;
        }

        private readonly Label _summary;
        private readonly Button _copyToTester;
        private readonly Nud _levelCap;
        private readonly Nud _domesticCap;
        private readonly Nud _mutationCap;
        private readonly Nud _imprint;
        private readonly TableLayoutPanel _statTable;
        private readonly StatRow[] _rows = new StatRow[Stats.StatsCount];
        private readonly System.Windows.Forms.Timer _solveTimer;
        private Species _species;
        private LevelSolverResult _latestResult;
        private int _serverLevelCap = 450;
        private bool _suspendUpdates;
        private int _solveVersion;
        private CancellationTokenSource _solveCancellation;
        private readonly SemaphoreSlim _solveGate = new SemaphoreSlim(1, 1);

        protected override CreateParams CreateParams
        {
            get
            {
                const int wsExComposited = 0x02000000;
                var createParams = base.CreateParams;
                createParams.ExStyle |= wsExComposited;
                return createParams;
            }
        }

        public LevelSolverControl()
        {
            SuspendLayout();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Dock = DockStyle.Fill;
            AutoScroll = true;

            var content = new FlowLayoutPanel
            {
                AutoScroll = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(8)
            };
            Controls.Add(content);

            var settings = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = true,
                Margin = new Padding(0, 0, 0, 8)
            };
            _levelCap = AddNumericSetting(settings, "Level cap", 1, 5000, 450, 0);
            _domesticCap = AddNumericSetting(settings, "Domestic levels", 0, 500, 88, 0);
            _mutationCap = AddNumericSetting(settings, "Max mutation / stat", 0, 255, 255, 0);
            _imprint = AddNumericSetting(settings, "Imprint %", 0, 500, 100, 1);
            content.Controls.Add(settings);

            _statTable = new BufferedTableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                ColumnCount = 8,
                RowCount = Stats.DisplayOrder.Length,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            _statTable.SuspendLayout();
            for (var columnIndex = 0; columnIndex < _statTable.ColumnCount; columnIndex++)
                _statTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1));
            for (var styleIndex = 0; styleIndex < _statTable.RowCount; styleIndex++)
                _statTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            AddHeader("Stat", 0);
            AddHeader("Mode", 1);
            AddHeader("Target", 2);
            AddHeader("Priority", 3);
            AddHeader("Wild", 4);
            AddHeader("Mutation", 5);
            AddHeader("Domestic", 6);
            AddHeader("Result", 7, ContentAlignment.MiddleRight);

            var rowIndex = 1;
            foreach (var statIndex in Stats.DisplayOrder.Where(s => s != Stats.Torpidity))
                AddStatRow(statIndex, rowIndex++);
            UpdateTableDimensions();
            _statTable.ResumeLayout(false);
            content.Controls.Add(_statTable);

            _summary = new Label
            {
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(0, 8, 0, 0),
                Text = "Select a species and configure at least one target."
            };
            content.Controls.Add(_summary);

            _copyToTester = new Button
            {
                AutoSize = true,
                Enabled = false,
                Margin = new Padding(0, 8, 0, 0),
                Text = "Copy to Stat Testing"
            };
            _copyToTester.Click += (_, _) => CopyLatestResultToTester();
            content.Controls.Add(_copyToTester);

            _solveTimer = new System.Windows.Forms.Timer { Interval = 180 };
            _solveTimer.Tick += SolveTimer_Tick;
            foreach (var input in new[] { _levelCap, _domesticCap, _mutationCap, _imprint })
                input.ValueChanged += InputChanged;
            ResumeLayout(false);
        }

        public event Action<Species, int[], int[], int[], double> CopyToTester;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CreatureCollection CreatureCollection
        {
            set
            {
                _suspendUpdates = true;
                _serverLevelCap = value?.maxServerLevel ?? 450;
                UpdateLevelCap();
                _domesticCap.ValueSave = value?.maxDomLevel ?? CreatureCollection.MaxDomLevelDefault;
                _suspendUpdates = false;
                QueueSolve();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _solveCancellation?.Cancel();
                _solveCancellation?.Dispose();
                _solveTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        public void SetSpecies(Species species)
        {
            _species = species;
            _suspendUpdates = true;
            UpdateLevelCap();
            _statTable.Visible = false;
            _statTable.SuspendLayout();
            try
            {
                for (var statIndex = 0; statIndex < Stats.StatsCount; statIndex++)
                {
                    var row = _rows[statIndex];
                    if (row == null) continue;
                    row.Name.Text = Utils.StatName(statIndex, false, species?.statNames);
                    row.Available = species?.UsesStat(statIndex) == true;
                }
                RebuildVisibleRows();
                UpdateTableDimensions();
            }
            finally
            {
                _statTable.ResumeLayout(false);
                _statTable.Visible = true;
                _suspendUpdates = false;
            }
            QueueSolve();
        }

        private void UpdateLevelCap()
            => _levelCap.ValueSave = _serverLevelCap + (_species?.ServerLevelCapIncrease ?? 0);

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            UpdateTableDimensions();
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            UpdateTableDimensions();
        }

        public void Recalculate() => QueueSolve();

        private void AddHeader(string text, int column, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            _statTable.Controls.Add(new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font(Font, FontStyle.Bold),
                Padding = new Padding(4),
                Text = text,
                TextAlign = alignment
            }, column, 0);
        }

        private void AddStatRow(int statIndex, int rowIndex)
        {
            var mode = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(2)
            };
            mode.Items.AddRange(TargetModeNames);
            mode.SelectedIndex = (int)LevelTargetMode.Target;

            var target = new Nud
            {
                DecimalPlaces = Stats.IsPercentage(statIndex) ? 1 : 1,
                Dock = DockStyle.Fill,
                Maximum = 100000000,
                ThousandsSeparator = true,
                Margin = new Padding(2)
            };
            var priority = new Nud
            {
                Dock = DockStyle.Fill,
                Maximum = 100,
                Value = rowIndex,
                Margin = new Padding(2)
            };
            var wild = new Nud
            {
                Dock = DockStyle.Fill,
                Maximum = 255,
                Margin = new Padding(2)
            };
            var row = new StatRow
            {
                StatIndex = statIndex,
                RowIndex = rowIndex,
                Name = CreateCellLabel(Utils.StatName(statIndex)),
                Mode = mode,
                Target = target,
                Priority = priority,
                Wild = wild,
                Mutation = CreateCellLabel("-", ContentAlignment.MiddleCenter),
                Domestic = CreateCellLabel("-", ContentAlignment.MiddleCenter),
                Result = CreateCellLabel("-", ContentAlignment.MiddleRight)
            };
            _rows[statIndex] = row;
            _statTable.Controls.Add(row.Name, 0, rowIndex);
            _statTable.Controls.Add(row.Mode, 1, rowIndex);
            _statTable.Controls.Add(row.Target, 2, rowIndex);
            _statTable.Controls.Add(row.Priority, 3, rowIndex);
            _statTable.Controls.Add(row.Wild, 4, rowIndex);
            _statTable.Controls.Add(row.Mutation, 5, rowIndex);
            _statTable.Controls.Add(row.Domestic, 6, rowIndex);
            _statTable.Controls.Add(row.Result, 7, rowIndex);

            mode.SelectedIndexChanged += (_, _) =>
            {
                UpdateRowInputState(row);
                QueueSolve();
            };
            target.ValueChanged += InputChanged;
            priority.ValueChanged += InputChanged;
            wild.ValueChanged += InputChanged;
            UpdateRowInputState(row);
        }

        private async void SolveTimer_Tick(object sender, EventArgs e)
        {
            _solveTimer.Stop();
            if (_species == null) return;

            var request = CreateRequest();
            var version = _solveVersion;
            _solveCancellation?.Dispose();
            _solveCancellation = new CancellationTokenSource();
            var cancellationToken = _solveCancellation.Token;
            _summary.Text = "Calculating...";
            var gateEntered = false;
            try
            {
                await _solveGate.WaitAsync(cancellationToken);
                gateEntered = true;
                if (version != _solveVersion) return;
                var result = await Task.Run(() => LevelAllocationSolver.Solve(request, cancellationToken),
                    cancellationToken);
                if (version != _solveVersion || IsDisposed) return;
                DisplayResult(result);
            }
            catch (OperationCanceledException)
            {
                // A newer input superseded this calculation.
            }
            finally
            {
                if (gateEntered)
                    _solveGate.Release();
            }
        }

        private LevelSolverRequest CreateRequest()
        {
            var request = new LevelSolverRequest
            {
                Species = _species,
                LevelCap = (int)_levelCap.Value,
                MaxDomesticLevels = (int)_domesticCap.Value,
                MaxMutationLevelPerStat = (int)_mutationCap.Value,
                ImprintingBonus = (double)_imprint.Value / 100
            };
            for (var statIndex = 0; statIndex < Stats.StatsCount; statIndex++)
            {
                var row = _rows[statIndex];
                if (row == null || !row.Available) continue;
                request.WildLevels[statIndex] = (int)row.Wild.Value;
                request.StatTargets[statIndex] = new LevelStatTarget
                {
                    Mode = (LevelTargetMode)row.Mode.SelectedIndex,
                    TargetValue = Stats.IsPercentage(statIndex)
                        ? (double)row.Target.Value / 100
                        : (double)row.Target.Value,
                    Priority = (int)row.Priority.Value
                };
            }
            return request;
        }

        private void DisplayResult(LevelSolverResult result)
        {
            if (!result.Feasible)
            {
                ClearResults(result.Message);
                return;
            }

            foreach (var row in _rows.Where(r => r != null))
            {
                row.Mutation.Text = result.MutationLevels[row.StatIndex].ToString();
                row.Domestic.Text = result.DomesticLevels[row.StatIndex].ToString();
                var value = result.Values[row.StatIndex] * (Stats.IsPercentage(row.StatIndex) ? 100 : 1);
                row.Result.Text = value.ToString("N1") + (Stats.IsPercentage(row.StatIndex) ? " %" : null);
            }
            _latestResult = result;
            _copyToTester.Enabled = true;
            _summary.Text = $"Total level: {result.TotalLevel:N0}    Domestic levels: {result.TotalDomesticLevels:N0}";
        }

        private void ClearResults(string message)
        {
            _latestResult = null;
            _copyToTester.Enabled = false;
            foreach (var row in _rows.Where(r => r != null))
            {
                row.Mutation.Text = "-";
                row.Domestic.Text = "-";
                row.Result.Text = "-";
            }
            _summary.Text = message;
        }

        private void InputChanged(object sender, EventArgs e) => QueueSolve();

        private void QueueSolve()
        {
            if (_suspendUpdates || _solveTimer == null) return;
            _latestResult = null;
            _copyToTester.Enabled = false;
            _solveVersion++;
            _solveCancellation?.Cancel();
            _solveTimer.Stop();
            _solveTimer.Start();
        }

        private void CopyLatestResultToTester()
        {
            if (_species == null || _latestResult?.Feasible != true) return;

            var wildLevels = _latestResult.WildLevels.ToArray();
            wildLevels[Stats.Torpidity] = _latestResult.TotalLevel - _latestResult.TotalDomesticLevels - 1;
            CopyToTester?.Invoke(_species, wildLevels, _latestResult.MutationLevels.ToArray(),
                _latestResult.DomesticLevels.ToArray(), (double)_imprint.Value / 100);
        }

        private static void UpdateRowInputState(StatRow row)
        {
            var mode = (LevelTargetMode)row.Mode.SelectedIndex;
            row.Target.Enabled = mode == LevelTargetMode.Target;
            row.Priority.Enabled = mode == LevelTargetMode.Maximize;
        }

        private void RebuildVisibleRows()
        {
            foreach (var row in _rows.Where(row => row != null))
            {
                _statTable.Controls.Remove(row.Name);
                _statTable.Controls.Remove(row.Mode);
                _statTable.Controls.Remove(row.Target);
                _statTable.Controls.Remove(row.Priority);
                _statTable.Controls.Remove(row.Wild);
                _statTable.Controls.Remove(row.Mutation);
                _statTable.Controls.Remove(row.Domestic);
                _statTable.Controls.Remove(row.Result);
            }

            var visibleRows = Stats.DisplayOrder
                .Where(statIndex => statIndex != Stats.Torpidity && _rows[statIndex]?.Available == true)
                .Select(statIndex => _rows[statIndex])
                .ToArray();
            _statTable.RowStyles.Clear();
            _statTable.RowCount = visibleRows.Length + 1;
            for (var styleIndex = 0; styleIndex < _statTable.RowCount; styleIndex++)
                _statTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

            for (var rowIndex = 0; rowIndex < visibleRows.Length; rowIndex++)
            {
                var row = visibleRows[rowIndex];
                row.RowIndex = rowIndex + 1;
                _statTable.Controls.Add(row.Name, 0, row.RowIndex);
                _statTable.Controls.Add(row.Mode, 1, row.RowIndex);
                _statTable.Controls.Add(row.Target, 2, row.RowIndex);
                _statTable.Controls.Add(row.Priority, 3, row.RowIndex);
                _statTable.Controls.Add(row.Wild, 4, row.RowIndex);
                _statTable.Controls.Add(row.Mutation, 5, row.RowIndex);
                _statTable.Controls.Add(row.Domestic, 6, row.RowIndex);
                _statTable.Controls.Add(row.Result, 7, row.RowIndex);
            }
        }

        private void UpdateTableDimensions()
        {
            if (_statTable == null || _rows == null || _statTable.ColumnStyles.Count < 8) return;

            var sampleRow = _rows.FirstOrDefault(row => row != null);
            if (sampleRow == null) return;

            var cellAllowance = Math.Max(4, DeviceDpi / 24);
            var spinnerWidth = Math.Max(SystemInformation.VerticalScrollBarWidth, Font.Height);
            var visibleStatNames = _rows
                .Where(row => row?.Available == true)
                .Select(row => row.Name.Text)
                .DefaultIfEmpty("Stat");

            SetColumnWidth(0, Math.Max(HeaderWidth(0), CellWidth(sampleRow.Name, visibleStatNames)) + cellAllowance);
            SetColumnWidth(1, Math.Max(HeaderWidth(1),
                CellWidth(sampleRow.Mode, TargetModeNames, spinnerWidth)) + cellAllowance);
            SetColumnWidth(2, Math.Max(HeaderWidth(2),
                CellWidth(sampleRow.Target, new[] { "100,000,000.0" }, spinnerWidth)) + cellAllowance);
            SetColumnWidth(3, Math.Max(HeaderWidth(3),
                CellWidth(sampleRow.Priority, new[] { "100" }, spinnerWidth)) + cellAllowance);
            SetColumnWidth(4, Math.Max(HeaderWidth(4),
                CellWidth(sampleRow.Wild, new[] { "255" }, spinnerWidth)) + cellAllowance);
            SetColumnWidth(5, Math.Max(HeaderWidth(5), CellWidth(sampleRow.Mutation, new[] { "254" })) + cellAllowance);
            SetColumnWidth(6, Math.Max(HeaderWidth(6), CellWidth(sampleRow.Domestic, new[] { "88" })) + cellAllowance);
            SetColumnWidth(7, Math.Max(HeaderWidth(7),
                CellWidth(sampleRow.Result, new[] { "100,000,000.0 %" })) + cellAllowance);

            var preferredInputHeight = sampleRow.Target.PreferredHeight;
            var rowHeight = Math.Max(preferredInputHeight + 5, Font.Height + 12);
            foreach (RowStyle rowStyle in _statTable.RowStyles)
                rowStyle.Height = rowHeight;
        }

        private int HeaderWidth(int columnIndex)
        {
            var header = _statTable.GetControlFromPosition(columnIndex, 0);
            return header.GetPreferredSize(Size.Empty).Width + header.Margin.Horizontal;
        }

        private static int CellWidth(Control control, IEnumerable<string> values, int adornmentWidth = 0)
            => Measure(values, control.Font) + control.Padding.Horizontal + control.Margin.Horizontal
                + adornmentWidth;

        private void SetColumnWidth(int columnIndex, int width)
            => _statTable.ColumnStyles[columnIndex].Width = width;

        private static int Measure(string value, Font font)
            => TextRenderer.MeasureText(value, font, Size.Empty,
                TextFormatFlags.SingleLine).Width;

        private static int Measure(IEnumerable<string> values, Font font)
            => values.Max(value => Measure(value, font));

        private static Label CreateCellLabel(string text, ContentAlignment alignment = ContentAlignment.MiddleLeft)
            => new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(4),
                Text = text,
                TextAlign = alignment
            };

        private static Nud AddNumericSetting(FlowLayoutPanel panel, string caption, decimal minimum, decimal maximum,
            decimal value, int decimalPlaces)
        {
            panel.Controls.Add(new Label { AutoSize = true, Margin = new Padding(0, 8, 4, 0), Text = caption });
            var input = new Nud
            {
                DecimalPlaces = decimalPlaces,
                Margin = new Padding(0, 4, 12, 0),
                Maximum = maximum,
                Minimum = minimum,
                Size = new Size(72, 23),
                Value = value
            };
            panel.Controls.Add(input);
            return input;
        }
    }
}