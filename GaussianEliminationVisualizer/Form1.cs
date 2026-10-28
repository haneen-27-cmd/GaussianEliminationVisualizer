using System.Drawing;
namespace GaussianEliminationVisualizer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int equations = (int)numericUpDown1.Value;
            int variables = (int)numericUpDown2.Value;

            matrixPanel.Controls.Clear();

            int columns = variables + 1;

            for (int i = 0; i < equations; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TextBox cell = new TextBox();

                    cell.Width = 60;
                    cell.Location = new Point(20 + j * 70, 40 + i * 35);
                    cell.Name = $"cell_{i}_{j}";
                    
                    matrixPanel.Controls.Add(cell);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Form initialization will go here later.
        }

        private void solveButton_Click(object sender, EventArgs e)
        {
            int equations = (int)numericUpDown1.Value;
            int variables = (int)numericUpDown2.Value;
            double[,] matrix = new double[equations, variables + 1];
            for (int i = 0; i < equations; i++)
            {
                for (int j = 0; j < variables + 1; j++)
                {
                    TextBox cell = (TextBox)matrixPanel.Controls[$"cell_{i}_{j}"];
                   
                    if (!double.TryParse(cell.Text, out matrix[i,j]))
                    {
                        MessageBox.Show($"Please enter a valid number in row {i + 1}, column {j + 1}.");
                        return;
                    }


                }
            }

            stepsRichTextBox.Clear();

            stepsRichTextBox.AppendText("Original Augmented Matrix:\n");

            int pivotRow = 0;

            for (int col = 0; col < variables && pivotRow < equations; col++)
            {
                int swapRow = -1;

                for (int r = pivotRow; r < equations; r++)
                {
                    if (Math.Abs(matrix[r, col]) > 0.000001)
                    {
                        swapRow = r;
                        break;
                    }
                }

                // Skip this column if all remaining entries are zero
                if (swapRow == -1)
                {
                    continue;
                }

                // Swap rows if needed
                if (swapRow != pivotRow)
                {
                    stepsRichTextBox.AppendText(
                        $"R{pivotRow + 1} <-> R{swapRow + 1}\n"
                    );

                    for (int c = 0; c < variables + 1; c++)
                    {
                        double temp = matrix[pivotRow, c];
                        matrix[pivotRow, c] = matrix[swapRow, c];
                        matrix[swapRow, c] = temp;
                    }
                }

                // Eliminate entries below the pivot
                for (int r = pivotRow + 1; r < equations; r++)
                {
                    double factor = matrix[r, col] / matrix[pivotRow, col];

                    if (Math.Abs(factor) > 0.000001)
                    {
                        stepsRichTextBox.AppendText(
                            $"R{r + 1} -> R{r + 1} - ({factor:0.##}) R{pivotRow + 1}\n"
                        );

                        for (int c = col; c < variables + 1; c++)
                        {
                            matrix[r, c] -= factor * matrix[pivotRow, c];

                            if (Math.Abs(matrix[r, c]) < 0.000001)
                            {
                                matrix[r, c] = 0;
                            }
                        }
                    }
                }

                // Display matrix after processing this pivot
                for (int r = 0; r < equations; r++)
                {
                    for (int c = 0; c < variables + 1; c++)
                    {
                        stepsRichTextBox.AppendText($"{matrix[r, c]:0.##}\t");
                    }

                    stepsRichTextBox.AppendText("\n");
                }

                stepsRichTextBox.AppendText("\n");

                pivotRow++;
            }

            stepsRichTextBox.AppendText("REF (Row Echelon Form) steps completed.\n");

            stepsRichTextBox.AppendText("\nFinal REF Matrix:\n");

            for (int i = 0; i < equations; i++)
            {
                for (int j = 0; j < variables + 1; j++)
                {
                    stepsRichTextBox.AppendText(
                        $"{matrix[i, j]:0.##}\t"
                    );
                }

                stepsRichTextBox.AppendText("\n");
            }

            stepsRichTextBox.AppendText("\n");

            // Convert REF to RREF
            stepsRichTextBox.AppendText("\nConverting REF to RREF:\n");

            for (int i = 0; i < equations; i++)
            {
                int leadingColumn = -1;

                for (int j = 0; j < variables; j++)
                {
                    if (Math.Abs(matrix[i, j]) > 0.000001)
                    {
                        leadingColumn = j;
                        break;
                    }
                }

                if (leadingColumn == -1)
                    continue;

                double leadingEntry = matrix[i, leadingColumn];

                if (Math.Abs(leadingEntry - 1) > 0.000001)
                {
                    stepsRichTextBox.AppendText(
                        $"R{i + 1} -> R{i + 1} / ({leadingEntry:0.##})\n"
                    );

                    for (int j = leadingColumn; j < variables + 1; j++)
                    {
                        matrix[i, j] /= leadingEntry;
                    }
                }

                for (int r = 0; r < equations; r++)
                {
                    for (int c = 0; c < variables + 1; c++)
                    {
                        stepsRichTextBox.AppendText($"{matrix[r, c]:0.##}\t");
                    }

                    stepsRichTextBox.AppendText("\n");
                }

                stepsRichTextBox.AppendText("\n");

                
                           
            }

            stepsRichTextBox.AppendText(
    "Eliminating entries above Leading 1s:\n"
);

            for (int i = equations - 1; i >= 0; i--)
            {
                int leadingColumn = -1;

                for (int j = 0; j < variables; j++)
                {
                    if (Math.Abs(matrix[i, j]) > 0.000001)
                    {
                        leadingColumn = j;
                        break;
                    }
                }

                if (leadingColumn == -1)
                    continue;

                for (int r = 0; r < i; r++)
                {
                    double factor = matrix[r, leadingColumn];

                    if (Math.Abs(factor) > 0.000001)
                    {
                        stepsRichTextBox.AppendText(
                            $"R{r + 1} -> R{r + 1} - ({factor:0.##}) R{i + 1}\n"
                        );

                        for (int c = 0; c < variables + 1; c++)
                        {
                            matrix[r, c] -= factor * matrix[i, c];
                        }

                        for (int a = 0; a < equations; a++)
                        {
                            for (int b = 0; b < variables + 1; b++)
                            {
                                stepsRichTextBox.AppendText(
                                    $"{matrix[a, b]:0.##}\t"
                                );
                            }

                            stepsRichTextBox.AppendText("\n");
                        }

                        stepsRichTextBox.AppendText("\n");
                    }
                }
            }

            stepsRichTextBox.AppendText("Final RREF Matrix:\n");

          


            for (int i = 0; i < equations; i++)
            {
                for (int j = 0; j < variables + 1; j++)
                {
                    stepsRichTextBox.AppendText($"{matrix[i, j]:0.##}\t");
                }

                stepsRichTextBox.AppendText("\n");
            }

            // Check the type of solution
            bool noSolution = false;
            int rank = 0;

            for (int i = 0; i < equations; i++)
            {
                bool allVariablesZero = true;

                for (int j = 0; j < variables; j++)
                {
                    if (Math.Abs(matrix[i, j]) > 0.000001)
                    {
                        allVariablesZero = false;
                        break;
                    }
                }

                if (allVariablesZero &&
                    Math.Abs(matrix[i, variables]) > 0.000001)
                {
                    noSolution = true;
                    break;
                }

                if (!allVariablesZero)
                {
                    rank++;
                }
            }

            stepsRichTextBox.AppendText("\nSolution Type:\n");

            if (noSolution)
            {
                stepsRichTextBox.AppendText(
                    "No Solution (Inconsistent System)\n");
            }
            else if (rank == variables)
            {
                stepsRichTextBox.AppendText(
                    "Unique Solution\n");
            }
            else
            {
                stepsRichTextBox.AppendText(
                    "Infinitely Many Solutions\n");
            }

        }
    }

    
}

