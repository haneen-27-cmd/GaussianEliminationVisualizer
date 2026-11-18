using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace GaussianEliminationVisualizer
{
    // ============================================================
    //  1) Exact fractions (so 1/3 stays 1/3, and no "-0" problems)
    // ============================================================
    public readonly struct Frac
    {
        private readonly BigInteger n, d;               // d == 0 only for default(Frac) -> treated as 0/1
        public BigInteger Num => n;
        public BigInteger Den => d.IsZero ? BigInteger.One : d;

        public Frac(BigInteger num, BigInteger den)
        {
            if (den.IsZero) throw new DivideByZeroException();
            if (den.Sign < 0) { num = -num; den = -den; }
            var g = BigInteger.GreatestCommonDivisor(BigInteger.Abs(num), den);
            if (!g.IsZero && !g.IsOne) { num /= g; den /= g; }
            n = num;
            d = den;
        }

        public static readonly Frac Zero = new Frac(0, 1);

        public bool IsZero => n.IsZero;
        public bool IsOne => Num.IsOne && Den.IsOne;

        public static Frac operator +(Frac a, Frac b) => new Frac(a.Num * b.Den + b.Num * a.Den, a.Den * b.Den);
        public static Frac operator -(Frac a, Frac b) => new Frac(a.Num * b.Den - b.Num * a.Den, a.Den * b.Den);
        public static Frac operator *(Frac a, Frac b) => new Frac(a.Num * b.Num, a.Den * b.Den);
        public static Frac operator /(Frac a, Frac b) => new Frac(a.Num * b.Den, a.Den * b.Num);

        public override string ToString() => Den.IsOne ? Num.ToString() : Num + "/" + Den;

        // accepts: 5   -3   2.5   .5   1/2   -3/4   1.5/2
        public static bool TryParse(string s, out Frac result)
        {
            result = Zero;
            s = s.Trim();
            if (s.Length == 0) return false;

            int slash = s.IndexOf('/');
            if (slash >= 0)
            {
                if (!TryParseDecimal(s.Substring(0, slash), out var a) ||
                    !TryParseDecimal(s.Substring(slash + 1), out var b) ||
                    b.IsZero)
                    return false;
                result = a / b;
                return true;
            }
            return TryParseDecimal(s, out result);
        }

        private static bool TryParseDecimal(string s, out Frac result)
        {
            result = Zero;
            s = s.Trim().Replace(',', '.');
            if (s.Length == 0) return false;

            bool neg = false;
            if (s[0] == '-' || s[0] == '+')
            {
                neg = s[0] == '-';
                s = s.Substring(1);
            }

            int dot = s.IndexOf('.');
            string digits = dot >= 0 ? s.Remove(dot, 1) : s;
            int decimals = dot >= 0 ? s.Length - dot - 1 : 0;
            if (digits.Length == 0) return false;
            foreach (char ch in digits)
                if (ch < '0' || ch > '9') return false;

            var num = BigInteger.Parse(digits);
            if (neg) num = -num;
            result = new Frac(num, BigInteger.Pow(10, decimals));
            return true;
        }
    }

    // ============================================================
    //  2) One step = a snapshot of the matrix + what to highlight
    // ============================================================
    public class Step
    {
        public Frac[,] M = new Frac[0, 0];
        public string Text = "";
        public int PivotRow = -1;    // row of the pivot (blue)
        public int PivotCol = -1;    // pivot column (green + arrow)
        public int TargetRow = -1;   // row being changed (yellow)
        public int SwapRow = -1;     // second row in a swap (orange)
    }

    // ============================================================
    //  3) Gauss / Gauss-Jordan: builds the list of steps
    // ============================================================
    public static class Gauss
    {
        public static List<Step> Solve(Frac[,] input, int vars, bool rref)
        {
            int m = input.GetLength(0);
            var M = (Frac[,])input.Clone();
            var steps = new List<Step>();

            void Add(string text, int pr = -1, int pc = -1, int tr = -1, int sr = -1)
            {
                steps.Add(new Step
                {
                    M = (Frac[,])M.Clone(),
                    Text = text,
                    PivotRow = pr,
                    PivotCol = pc,
                    TargetRow = tr,
                    SwapRow = sr
                });
            }

            Add("Original augmented matrix.");

            var pivots = new List<(int row, int col)>();
            int row = 0;

            // ---------- Forward phase (REF) ----------
            for (int col = 0; col < vars && row < m; col++)
            {
                int p = -1;
                for (int r = row; r < m; r++)
                {
                    if (!M[r, col].IsZero) { p = r; break; }
                }

                if (p == -1)
                {
                    Add($"Column x{col + 1}: all entries from R{row + 1} down are zero, so there is no pivot here. Move to the next column.",
                        -1, col);
                    continue;
                }

                if (p != row)
                {
                    for (int c = 0; c <= vars; c++)
                    {
                        var t = M[row, c];
                        M[row, c] = M[p, c];
                        M[p, c] = t;
                    }
                    Add($"Swap R{row + 1} and R{p + 1} to get a non-zero pivot in column x{col + 1}.",
                        row, col, row, p);
                }

                pivots.Add((row, col));
                Add($"Pivot: R{row + 1}, column x{col + 1} = {M[row, col]}. Clear the entries below it.", row, col);

                for (int r = row + 1; r < m; r++)
                {
                    if (M[r, col].IsZero) continue;

                    var f = M[r, col] / M[row, col];
                    for (int c = col; c <= vars; c++)
                        M[r, c] = M[r, c] - f * M[row, c];

                    Add($"R{r + 1} -> R{r + 1} - ({f}) * R{row + 1}", row, col, r);
                }

                row++;
            }

            // ---------- Backward phase (RREF) ----------
            if (rref)
            {
                for (int k = pivots.Count - 1; k >= 0; k--)
                {
                    var (pr, pc) = pivots[k];
                    var lead = M[pr, pc];

                    if (!lead.IsOne)
                    {
                        for (int c = pc; c <= vars; c++)
                            M[pr, c] = M[pr, c] / lead;
                        Add($"R{pr + 1} -> R{pr + 1} / ({lead})   (make the pivot equal 1)", pr, pc, pr);
                    }

                    for (int r = pr - 1; r >= 0; r--)
                    {
                        if (M[r, pc].IsZero) continue;

                        var f = M[r, pc];
                        for (int c = pc; c <= vars; c++)
                            M[r, c] = M[r, c] - f * M[pr, c];

                        Add($"R{r + 1} -> R{r + 1} - ({f}) * R{pr + 1}   (clear above the pivot)", pr, pc, r);
                    }
                }
            }

            // ---------- Type of solution ----------
            bool inconsistent = false;
            for (int r = 0; r < m && !inconsistent; r++)
            {
                bool allZero = true;
                for (int c = 0; c < vars; c++)
                {
                    if (!M[r, c].IsZero) { allZero = false; break; }
                }
                if (allZero && !M[r, vars].IsZero) inconsistent = true;
            }

            int rank = pivots.Count;
            var sb = new StringBuilder();
            string kind = rref ? "RREF" : "REF";
            sb.Append("Final " + kind + " matrix.\n");

            if (inconsistent)
            {
                sb.Append("No solution (inconsistent system): one row says 0 = non-zero.");
            }
            else if (rank == vars)
            {
                var x = new Frac[vars];
                for (int k = pivots.Count - 1; k >= 0; k--)
                {
                    var (pr, pc) = pivots[k];
                    var s = M[pr, vars];
                    for (int c = pc + 1; c < vars; c++)
                        s = s - M[pr, c] * x[c];
                    x[pc] = s / M[pr, pc];
                }
                sb.Append("Unique solution:  ");
                sb.Append(string.Join(",  ", x.Select((v, i) => "x" + (i + 1) + " = " + v)));
            }
            else
            {
                var free = new List<string>();
                for (int c = 0; c < vars; c++)
                {
                    int cc = c;
                    if (!pivots.Any(pv => pv.col == cc)) free.Add("x" + (c + 1));
                }
                sb.Append("Infinitely many solutions.  Rank = " + rank + ",  free variables: " + string.Join(", ", free));
            }

            Add(sb.ToString());
            return steps;
        }
    }

    // ============================================================
    //  4) The panel that DRAWS the matrix (cells, highlight, arrow)
    // ============================================================
    public class MatrixView : Panel
    {
        private const int CW = 90;       // cell width
        private const int CH = 44;       // cell height
        private const int LeftPad = 80;
        private const int TopPad = 60;

        private Frac[,]? matrix;
        private int vars;
        private Step? step;

        public MatrixView()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            AutoScroll = true;
            BackColor = Color.White;
        }

        public void SetContent(Frac[,]? m, int variables, Step? s)
        {
            matrix = m;
            vars = variables;
            step = s;

            if (m != null)
            {
                int rows = m.GetLength(0);
                AutoScrollMinSize = new Size(LeftPad + (vars + 1) * CW + 190, TopPad + rows * CH + 140);
            }
            else
            {
                AutoScrollMinSize = Size.Empty;
            }
            Invalidate();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        private Color? RowColor(int r)
        {
            if (step == null) return null;
            if (step.SwapRow >= 0 && (r == step.TargetRow || r == step.SwapRow)) return Color.Orange;
            if (r == step.TargetRow) return Color.Gold;
            if (r == step.PivotRow) return Color.DeepSkyBlue;
            return null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (matrix == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

            int rows = matrix.GetLength(0);
            int cols = vars + 1;
            int x0 = LeftPad, y0 = TopPad;
            int w = cols * CW, h = rows * CH;

            using var font = new Font("Segoe UI", 14f);
            using var fontSmall = new Font("Segoe UI", 10f);
            using var fontBold = new Font("Segoe UI", 14f, FontStyle.Bold);
            using var head = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var gridPen = new Pen(Color.Gainsboro, 1);
            using var bracketPen = new Pen(Color.Black, 2.5f);

            // column headers: x1 x2 ... b
            for (int c = 0; c < cols; c++)
            {
                string title = c < vars ? "x" + (c + 1) : "b";
                g.DrawString(title, head, Brushes.DimGray, new RectangleF(x0 + c * CW, y0 - 34, CW, 28), sf);
            }

            // highlight the pivot column
            if (step != null && step.PivotCol >= 0 && step.PivotCol < cols && rows > 0)
            {
                using var br = new SolidBrush(Color.FromArgb(200, 230, 201));
                g.FillRectangle(br, x0 + step.PivotCol * CW, y0, CW, h);
            }

            // highlight rows + row labels
            for (int r = 0; r < rows; r++)
            {
                var rc = RowColor(r);
                if (rc.HasValue)
                {
                    using var br = new SolidBrush(Color.FromArgb(170, rc.Value));
                    g.FillRectangle(br, x0, y0 + r * CH, w, CH);
                }
                g.DrawString("R" + (r + 1), head, Brushes.DimGray, new RectangleF(x0 - 66, y0 + r * CH, 50, CH), sf);
            }

            // grid
            for (int r = 0; r <= rows; r++)
                g.DrawLine(gridPen, x0, y0 + r * CH, x0 + w, y0 + r * CH);
            for (int c = 0; c <= cols; c++)
                g.DrawLine(gridPen, x0 + c * CW, y0, x0 + c * CW, y0 + h);

            // augmented bar before the constants column
            using (var bar = new Pen(Color.Black, 2) { DashStyle = DashStyle.Dash })
                g.DrawLine(bar, x0 + vars * CW, y0 - 4, x0 + vars * CW, y0 + h + 4);

            // brackets [ ]
            g.DrawLines(bracketPen, new[]
            {
                new Point(x0 - 2, y0 - 4), new Point(x0 - 8, y0 - 4),
                new Point(x0 - 8, y0 + h + 4), new Point(x0 - 2, y0 + h + 4)
            });
            g.DrawLines(bracketPen, new[]
            {
                new Point(x0 + w + 2, y0 - 4), new Point(x0 + w + 8, y0 - 4),
                new Point(x0 + w + 8, y0 + h + 4), new Point(x0 + w + 2, y0 + h + 4)
            });

            // numbers
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    bool isPivot = step != null && r == step.PivotRow && c == step.PivotCol;
                    string t = matrix[r, c].ToString();
                    var f = isPivot ? fontBold : (t.Length > 9 ? fontSmall : font);
                    g.DrawString(t, f, isPivot ? Brushes.Crimson : Brushes.Black,
                        new RectangleF(x0 + c * CW, y0 + r * CH, CW, CH), sf);
                }
            }

            if (step != null)
            {
                // box around the pivot cell
                if (step.PivotRow >= 0 && step.PivotCol >= 0 && step.PivotRow < rows)
                {
                    using var pen = new Pen(Color.Crimson, 3);
                    g.DrawRectangle(pen, x0 + step.PivotCol * CW + 2, y0 + step.PivotRow * CH + 2, CW - 4, CH - 4);
                }

                // "<-- changed" next to modified rows
                string tag = step.SwapRow >= 0 ? "<-- swapped" : "<-- changed";
                foreach (int r in new[] { step.TargetRow, step.SwapRow })
                {
                    if (r >= 0 && r < rows)
                        g.DrawString(tag, head, Brushes.DarkOrange, x0 + w + 20, y0 + r * CH + 10);
                }

                // arrow under the pivot column
                if (step.PivotCol >= 0 && step.PivotCol < cols)
                {
                    int cx = x0 + step.PivotCol * CW + CW / 2;
                    int yb = y0 + h + 12;
                    using var pen = new Pen(Color.Crimson, 3) { CustomEndCap = new AdjustableArrowCap(5, 5) };
                    g.DrawLine(pen, cx, yb + 42, cx, yb);
                    string label = step.PivotRow >= 0 ? "pivot column" : "no pivot here";
                    g.DrawString(label, head, Brushes.Crimson, new RectangleF(cx - 70, yb + 44, 140, 24), sf);
                }
            }
        }
    }

    // ============================================================
    //  5) The main window (the whole UI is built in code)
    // ============================================================
    public class MainForm : Form
    {
        private readonly NumericUpDown numEq, numVar;
        private readonly Button btnStart, btnAdd, btnEdit, btnSolve, btnBack, btnNext;
        private readonly TextBox txtEq;
        private readonly ComboBox cmbMode;
        private readonly Label lblPrompt, lblError, lblStep, lblDesc;
        private readonly MatrixView view;

        private int m, n;
        private bool started;
        private readonly List<Frac[]> rows = new List<Frac[]>();
        private readonly List<string> rowTexts = new List<string>();
        private List<Step> steps = new List<Step>();
        private int cur;

        public MainForm()
        {
            Text = "Gaussian Elimination Visualizer";
            ClientSize = new Size(1050, 700);
            MinimumSize = new Size(820, 520);
            Font = new Font("Segoe UI", 10f);

            numEq = new NumericUpDown { Minimum = 1, Maximum = 12, Value = 3, Width = 55 };
            numVar = new NumericUpDown { Minimum = 1, Maximum = 10, Value = 3, Width = 55 };
            btnStart = Btn("Start", 90, (s, e) => ToggleStart());

            lblPrompt = new Label { AutoSize = true, Margin = new Padding(6, 8, 2, 2), Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };

            txtEq = new TextBox { Width = 420, Font = new Font("Consolas", 12f) };
            txtEq.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    AddEquation();
                }
            };
            btnAdd = Btn("Add equation", 120, (s, e) => AddEquation());
            btnEdit = Btn("Edit last equation", 150, (s, e) => EditLast());

            lblError = new Label { AutoSize = true, ForeColor = Color.Crimson, Margin = new Padding(6, 2, 2, 2) };

            cmbMode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80 };
            cmbMode.Items.AddRange(new object[] { "REF", "RREF" });
            cmbMode.SelectedIndex = 1;
            btnSolve = Btn("Solve", 90, (s, e) => SolveClick());
            btnBack = Btn("<  Back", 90, (s, e) => { if (cur > 0) { cur--; ShowStep(); } });
            btnNext = Btn("Next  >", 90, (s, e) => { if (cur < steps.Count - 1) { cur++; ShowStep(); } });
            lblStep = new Label { AutoSize = true, Margin = new Padding(10, 8, 2, 0) };

            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(8)
            };

            top.Controls.Add(L("Equations (m):"));
            top.Controls.Add(numEq);
            top.Controls.Add(L("Variables (n):"));
            top.Controls.Add(numVar);
            top.Controls.Add(btnStart);
            top.SetFlowBreak(btnStart, true);

            top.Controls.Add(lblPrompt);
            top.SetFlowBreak(lblPrompt, true);

            top.Controls.Add(txtEq);
            top.Controls.Add(btnAdd);
            top.Controls.Add(btnEdit);
            top.SetFlowBreak(btnEdit, true);

            top.Controls.Add(lblError);
            top.SetFlowBreak(lblError, true);

            top.Controls.Add(L("Method:"));
            top.Controls.Add(cmbMode);
            top.Controls.Add(btnSolve);
            top.Controls.Add(btnBack);
            top.Controls.Add(btnNext);
            top.Controls.Add(lblStep);

            view = new MatrixView { Dock = DockStyle.Fill };

            lblDesc = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 110,
                Font = new Font("Segoe UI", 12f),
                Padding = new Padding(12, 8, 12, 8),
                BackColor = Color.FromArgb(255, 253, 231)
            };
            // order matters for docking: Fill first, then Bottom, then Top
            Controls.Add(view);
            Controls.Add(lblDesc);
            Controls.Add(top);

            UpdateInputState();
        }

        // ---------- small UI helpers ----------
        private Label L(string text) =>
            new Label { Text = text, AutoSize = true, Margin = new Padding(6, 8, 2, 0) };

        private Button Btn(string text, int width, EventHandler click)
        {
            var b = new Button { Text = text, Width = width, Height = 30 };
            b.Click += click;
            return b;
        }

        private void ShowError(string msg) => lblError.Text = msg;
        private void ClearError() => lblError.Text = "";

        // ---------- Start / Reset ----------
        private void ToggleStart()
        {
            if (!started)
            {
                m = (int)numEq.Value;
                n = (int)numVar.Value;
                started = true;
                btnStart.Text = "Reset";
                numEq.Enabled = numVar.Enabled = false;
            }
            else
            {
                started = false;
                btnStart.Text = "Start";
                numEq.Enabled = numVar.Enabled = true;
            }

            rows.Clear();
            rowTexts.Clear();
            txtEq.Clear();
            ClearError();
            UpdateInputState();
            if (started) txtEq.Focus();
        }

        // ---------- Input: one equation at a time ----------
        private void AddEquation()
        {
            if (!started || rows.Count >= m) return;

            var tokens = txtEq.Text.Split(new[] { ' ', '\t', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int k = rows.Count + 1;

            if (tokens.Length != n + 1)
            {
                ShowError($"Equation {k}: expected {n + 1} numbers ({n} coefficients + the constant) but got {tokens.Length}.");
                return;
            }

            var row = new Frac[n + 1];
            for (int j = 0; j < tokens.Length; j++)
            {
                if (!Frac.TryParse(tokens[j], out row[j]))
                {
                    ShowError($"Equation {k}: '{tokens[j]}' (number {j + 1}) is not a valid number.");
                    return;
                }
            }

            rows.Add(row);
            rowTexts.Add(string.Join(" ", tokens));
            txtEq.Clear();
            ClearError();
            UpdateInputState();
            if (rows.Count < m) txtEq.Focus();
        }

        // Takes the last equation back into the text box so it can be rewritten
        private void EditLast()
        {
            if (rows.Count == 0) return;

            int last = rows.Count - 1;
            txtEq.Text = rowTexts[last];
            rows.RemoveAt(last);
            rowTexts.RemoveAt(last);
            ClearError();
            UpdateInputState();
            txtEq.Focus();
            txtEq.SelectAll();
        }

        private Frac[,] ToMatrix()
        {
            var M = new Frac[rows.Count, n + 1];
            for (int i = 0; i < rows.Count; i++)
                for (int j = 0; j <= n; j++)
                    M[i, j] = rows[i][j];
            return M;
        }

        private void ResetSteps()
        {
            steps = new List<Step>();
            cur = 0;
            btnBack.Enabled = btnNext.Enabled = false;
            lblStep.Text = "";
        }

        private void UpdateInputState()
        {
            ResetSteps();

            bool more = started && rows.Count < m;
            txtEq.Enabled = btnAdd.Enabled = more;
            btnEdit.Enabled = started && rows.Count > 0;
            btnSolve.Enabled = started && rows.Count == m;
            cmbMode.Enabled = started;

            if (!started)
            {
                lblPrompt.Text = "Choose m (equations) and n (variables), then press Start.";/
                view.SetContent(null, 0, null);
                lblDesc.Text = "";
                return;
            }

            if (more)
            {
                var names = new List<string>();
                for (int i = 1; i <= n; i++) names.Add("a" + i);
                string example = string.Join(" ", names) + " b";
                lblPrompt.Text = $"Equation {rows.Count + 1} of {m}:  type {n} coefficients then the constant, separated by spaces  (e.g. {example}).  Fractions like 1/2 are allowed.";
            }
            else
            {
                lblPrompt.Text = "All equations entered. Press Solve, or 'Edit last equation' to change the last one.";
            }

            view.SetContent(ToMatrix(), n, null);
            lblDesc.Text = $"Equations entered: {rows.Count} / {m}";
        }

        // ---------- Solve + step navigation ----------
        private void SolveClick()
        {
            if (!started || rows.Count != m) return;

            steps = Gauss.Solve(ToMatrix(), n, cmbMode.SelectedIndex == 1);
            cur = 0;
            ShowStep();
        }

        private void ShowStep()
        {
            var s = steps[cur];
            view.SetContent(s.M, n, s);
            lblDesc.Text = s.Text;
            lblStep.Text = $"Step {cur} / {steps.Count - 1}";
            btnBack.Enabled = cur > 0;
            btnNext.Enabled = cur < steps.Count - 1;
        }
    }
}