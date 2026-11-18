# Gaussian Elimination Visualizer

A Windows Forms app (C#) that solves a system of linear equations
with Gaussian elimination and shows every step visually.

## Features
- Enter equations one at a time, with instant validation
- Edit the last equation if you made a mistake
- The matrix is drawn on screen, not printed as text
- Step-by-step navigation (Back / Next)
- REF or RREF
- Highlighted rows, pivot column, and an arrow under the pivot
- Exact fractions (no rounding errors)
- Detects: unique solution, infinitely many solutions, no solution

## How to use
1. Choose the number of equations (m) and variables (n), then press Start.
2. Type each equation as coefficients followed by the constant.
   Example: 2x1 + x2 - x3 = 8  ->  2 1 -1 8
3. Choose REF or RREF, then press Solve.
4. Use Next and Back to follow the steps.

## Colors
- Green: pivot column
- Blue: pivot row
- Yellow: row being changed
- Orange: rows being swapped

## Built with
C# and Windows Forms (.NET)
