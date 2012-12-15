// Eric Bruck
// CS 571 Artificial Intelligence
// November 9, 2011
//
// SUDOKU
//
// Description:
//  This program will solve a game of Sudoku using Arc-consistency algorithm three, Forward Checking, and Back Tracking.
//  At the start of the program we are given some numbers in the Sudoku table.
//  All of the other spaces that are unknown.
//  this means that at the start of the program all of the unknown spaces can be any number ranging from one to nine (1-9).
//  To retain all the information about the Sudoku table a two dimensional array of lists is used.
//  Every list in the array contains all the possible numbers of that location.
//  known numbers of the table will only have one number in the list.
//
// Algorithm:
//  This program is a loop that continues until a solution is found, if one exists.
//  Once in the loop the Arc-consistency algorithm three is applied.
//  If after the Arc-consistency algorithm no new numbers are found then Forward Checking is applied.
//  If after Forward Checking no new numbers are found then Back Tracking is applied.
//  If at any point a new number is found using any of the above methods then Arc-consistency is used.
//  A limit of 10 Back Tracking iterations is set in the program.
//  If this limit is reached it is assumed that there is no solution to this problem.
//
// Arc-consistency algorithm three:
//  The Arc-consistency algorithm three reduces the domain of each unknown location.
//  The domain for the unknown locations is the list of all possible numbers it can be.
//  To reduce the domain for an unknown at location (x,y) we make a list of all the known elements in row x.
//  We then remove all the elements from the domain of (x,y) that are in the list of known elements in row x.
//  After this we make a list of all the known elements in column y.
//  We then remove all the elements from the domain of (x,y) that are in the list of known elements in column y.
//  The next step is to make a list of all the known elements in the three by three box that contains the location (x,y).
//  We then remove all the elements from the domain of (x,y) that are in the list of known elements in the three by three
//      box that contains the location (x,y).
//  The figure below shows the row, column, and box that is used to reduce the domain for the location at the coordinates
//      (x,y).
//  Through the picture one can see that there is some repetition when reducing the domain.
//  For all coordinates there will be a maximum of four repetitions of removing form the domain of (x,y). 
//  The repetitions may be fewer if the possible repeated locations are unknown.
//  If they are unknown then they are skipped, because we are reducing the domain by the known elements.
//  This was not implemented.
//
//
//              =========================================
//              ||   |   |   || X |   |   ||   |   |   ||
//              -----------------------------------------
//              ||   |   |   || X |   |   ||   |   |   ||
//              -----------------------------------------
//              ||   |   |   || X |   |   ||   |   |   ||
//              =========================================
//              ||   |   |   || R | X | X ||   |   |   ||
//              -----------------------------------------
//              ||   |   |   || R | X | X ||   |   |   ||
//              -----------------------------------------
//              || X | X | X || O | R | R || X | X | X ||
//              =========================================
//              ||   |   |   || X |   |   ||   |   |   ||
//              -----------------------------------------
//              ||   |   |   || X |   |   ||   |   |   ||
//              -----------------------------------------
//              ||   |   |   || X |   |   ||   |   |   ||
//              =========================================
//
//      Key:
//          O: The position being checked through with Arc-consistency algorithm three
//          X: The positions used to reduce the domain of position O
//          R: The repeated positions – Always 4
//
//
// Forward Checking:
//  Forward Checking is similar to the Arc-consistency algorithm three.
//  The difference is that instead of comparing the known values we compare the unknown values.
//  To reduce the domain of an unknown location (x,y) through Forward Checking we first copy the list of possibilities 
//      for this location into a temporary list.
//  This is to ensure that the original list is not modified when checking.
//  If nothing can be concluded from Forward Checking then we do not want anything to change.
//  From this temporary list we remove all the numbers that appear in the unknown elements lists of row x.
//  If there is only one element in the temporary list then we replace the original list at location (x,y) with the 
//      temporary list.
//  By doing this we are checking if the location (x,y) has a possible solution that does not appear anywhere else in 
//      the row.
//  This implies that the solution to (x,y) must be the leftover number in the temporary list.
//  We then repeat this with column y if a solution was not found.
//  We first copy the list of possibilities for location (x,y) into a temporary list.
//  Again, from this temporary list we remove all the numbers that appear in the unknown elements lists of column y.
//  If only one element remains in the temporary list then we replace the original list at location (x,y) with the 
//      temporary list.
//  If a solution is not found, this is repeated with the three by three box that contains the location (x,y).
//  If only one element remains in the temporary list then we replace the original list at this location with the 
//      temporary list.
//  If a solution is still not found then the process is duplicated but this time the numbers that appear in the 
//      unknown elements lists of row x and column y are removed.
//  This is checked for a solution, and if a solution to this position is found then replaces the original list with 
//      the temporary list.
//  Otherwise remove all the numbers that appear in the unknown elements lists of the three by three box containing 
//      location (x,y).
//  Replace the original list if a solution is found.
//  This is process is done for all unknown elements.
//
// Back Tracking:
//  The first step in this programs Back Tracking is to find the unknown solution with the fewest possible solutions.
//  This location is chosen because it has a higher probability of finding the correct solution sooner than other positions.
//  The first possible solution to this location is chosen as a guess.
//  Before this position is changed to the guessed solution it saves’s the current state of the Sudoku.
//  Along with this the guessing location and the number in the list of possible solutions that is chosen as the guess 
//      is recorded.
//  After this the guess is applied to the Sudoku table.
//  From here it loops back to the Arc-consistency algorithm three.  
//  If the Back Tracking is implemented and there exists a position in the Sudoku table that contains no elements as its 
//      possible solutions then the previous guess in Back tracking was incorrect.
//  At this point the current Sudoku state is replaced with the previously recorded state, while still keeping the
//      previously recorded state in record.
//  From here we check if the there are anymore possible solutions in the guess location.
//  If there are then the next number in the list is chosen as the guess and increment the saved guess position, and 
//      loop back to the Arc-consistency algorithm three.
//  If there are no more elements in the guess location then remove the previously recorded state along with the 
//      corresponding guess information.
//  Then set the current Sudoku to the next previously stored state, while still keeping this state in record.
//  From here it loops back to check for a state with a guess that has not been traversed.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        // contains the current state of the Sudoku puzzle
        List<int>[][] puzzle = new List<int>[9][];
        // contains all the previous states
        int[][][][] prevStates = new int[15][][][];
        // Information on the previous states and the last guess made at that state
        int[][] back = new int[3][];
        // A null list
        List<int> nullList = new List<int>();

        // holds the initial given numbers
        int[][] Given = new int[9][];

        int backNum;
        int level;
        int iterationNum;

        PerformanceCounter cpuCounter;

        // Initialize all the components at first
        public Form1()
        {
            InitializeComponent();

            List<int> temp = new List<int>();
            nullList = null;
            backNum = -1;
            level = 1;
            iterationNum = 0;

            for (int i = 0; i < 9; i++)
            { puzzle[i] = new List<int>[9]; }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                { puzzle[i][j] = nullList; }
            }

            for (int i = 0; i < 3; i++)
            { back[i] = new int[15]; }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 15; j++)
                { back[i][j] = 0; }
            }

            for (int i = 0; i < 15; i++)
            { prevStates[i] = new int[9][][]; }
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 9; j++)
                { prevStates[i][j] = new int[9][]; }
            }
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int m = 0; m < 9; m++)
                    { prevStates[i][j][m] = new int[9]; }
                }
            }
            initPrevStates();

            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            setLevels(level);
            pushLevel();
            display();

            Start.Select();
        }

        // Set the given array to the desired difficulty
        private void setLevels(int dificulty)
        {
            for (int i = 0; i < Given.Length; i++)
            { Given[i] = new int[9]; }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                { Given[i][j] = 0; }
            }
            switch (dificulty)
            {
                case 1:
                    Given[0][3] = 2; Given[0][4] = 6; Given[0][6] = 7; Given[0][8] = 1;
                    Given[1][0] = 6; Given[1][1] = 8; Given[1][4] = 7; Given[1][7] = 9;
                    Given[2][0] = 1; Given[2][1] = 9; Given[2][5] = 4; Given[2][6] = 5;
                    Given[3][0] = 8; Given[3][1] = 2; Given[3][3] = 1; Given[3][7] = 4;
                    Given[4][2] = 4; Given[4][3] = 6; Given[4][5] = 2; Given[4][6] = 9;
                    Given[5][1] = 5; Given[5][5] = 3; Given[5][7] = 2; Given[5][8] = 8;
                    Given[6][2] = 9; Given[6][3] = 3; Given[6][7] = 7; Given[6][8] = 4;
                    Given[7][1] = 4; Given[7][4] = 5; Given[7][7] = 3; Given[7][8] = 6;
                    Given[8][0] = 7; Given[8][2] = 3; Given[8][4] = 1; Given[8][5] = 8;
                    break;

                case 2:
                    Given[0][1] = 2; Given[0][2] = 1; Given[0][4] = 9; Given[0][6] = 6; Given[0][7] = 7;
                    Given[1][8] = 8;
                    Given[2][1] = 3; Given[2][2] = 4; Given[2][3] = 2; Given[2][4] = 6; Given[2][8] = 5;
                    Given[3][5] = 7; Given[3][6] = 8; Given[3][7] = 6;
                    Given[4][0] = 3; Given[4][1] = 8; Given[4][2] = 5; Given[4][3] = 1; Given[4][5] = 6; Given[4][6] = 9; Given[4][7] = 4; Given[4][8] = 7;
                    Given[5][1] = 7; Given[5][2] = 9; Given[5][3] = 4;
                    Given[6][0] = 9; Given[6][4] = 4; Given[6][5] = 1; Given[6][6] = 7; Given[6][7] = 5;
                    Given[7][0] = 1;
                    Given[8][1] = 4; Given[8][2] = 8; Given[8][4] = 7; Given[8][6] = 3; Given[8][7] = 1;
                    break;
            }
        }

        // Add the given numbers to the puzzle array
        // This is array used to solve the Sudoku
        // For all locations not given, set the value to the list of all possibilities
        // At this point each of the unknowns will have a list of numbers from 1-9
        private void pushLevel()
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Given[i][j] == 0)
                    {
                        puzzle[i][j] = new List<int>(); puzzle[i][j].Add(1); puzzle[i][j].Add(2); puzzle[i][j].Add(3); puzzle[i][j].Add(4); puzzle[i][j].Add(5);
                        puzzle[i][j].Add(6); puzzle[i][j].Add(7); puzzle[i][j].Add(8); puzzle[i][j].Add(9);
                    }
                    else
                    { puzzle[i][j] = new List<int>(); puzzle[i][j].Add(Given[i][j]); }

                    temp = puzzle[i][j];
                }
            }
        }

        // Clear the Sudoku of all entries
        private void clearSudoku()
        {
            foreach (Control c in Box1.Controls)
            { c.Text = ""; }
            foreach (Control c in Box2.Controls)
            { c.Text = ""; }
            foreach (Control c in Box3.Controls)
            { c.Text = ""; }
            foreach (Control c in Box4.Controls)
            { c.Text = ""; }
            foreach (Control c in Box5.Controls)
            { c.Text = ""; }
            foreach (Control c in Box6.Controls)
            { c.Text = ""; }
            foreach (Control c in Box7.Controls)
            { c.Text = ""; }
            foreach (Control c in Box8.Controls)
            { c.Text = ""; }
            foreach (Control c in Box9.Controls)
            { c.Text = ""; }
        }

        // Display the known elements
        private void display()
        {
            List<int> temp = new List<int>();
            int m = 0;
            int rowBox = 0;
            int colBox = 0;
            int R = 0;
            int C = 0;
            string tempNum = "";

            temp = puzzle[0][0];
            foreach (Control c in Box1.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 0;
            colBox = 1;
            foreach (Control c in Box2.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 0;
            colBox = 2;
            foreach (Control c in Box3.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 1;
            colBox = 0;
            foreach (Control c in Box4.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 1;
            colBox = 1;
            foreach (Control c in Box5.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 1;
            colBox = 2;
            foreach (Control c in Box6.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 2;
            colBox = 0;
            foreach (Control c in Box7.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 2;
            colBox = 1;
            foreach (Control c in Box8.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }

            m = 0;
            rowBox = 2;
            colBox = 2;
            foreach (Control c in Box9.Controls)
            {
                R = (2 + (m / 3)) % 3 + (3 * rowBox);
                C = (m % 3) + (colBox * 3);
                temp = puzzle[R][C];
                if (temp.Count == 1)
                { tempNum = temp[0].ToString(); }
                else
                { tempNum = ""; }
                c.Text = tempNum;
                m++;
            }
        }

        // Find the solution to the Sudoku
        private void findSolution()
        {
            int unknown = findUnknown();
            int n = 0;
            DateTime startTime = DateTime.Now;
            while (!checkGoal())
            {
                reduceDomain();
                if (unknown == findUnknown())
                {
                    forwardChecking();
                }

                if (unknown == findUnknown())
                {
                    if (checkError())
                    {
                        reverseBackTracking();
                        n--;
                    }
                    else
                    { backTracking(); n++; }

                    reduceDomain();

                }
                unknown = findUnknown();

                if (n > 10)
                    break;
            }
            System.DateTime endTime = DateTime.Now;
            System.TimeSpan time = endTime.Subtract(startTime);

            display();
        }

        // Get the current CPU usage
        public string getCurrentCpuUsage()
        {
            return cpuCounter.NextValue() + "%";
        }

        // Reduce the domain through AC-3
        private void reduceDomain()
        {
            List<int> current = new List<int>();
            //By Row
            //get all number in each row
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (puzzle[j][i].Count == 1)
                    {
                        current.Add(puzzle[j][i][0]);
                    }
                }

                // delete these numbers from the list of posible solutions of unknown
                for (int i = 0; i < 9; i++)
                {
                    if (puzzle[j][i].Count > 1)
                    {
                        iterationNum++;
                        foreach (int num in current)
                            puzzle[j][i].Remove(num);
                    }
                }

                current.Clear();
            }

            //By Column
            //get all number in each column
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (puzzle[i][j].Count == 1)
                    {
                        current.Add(puzzle[i][j][0]);
                    }
                }

                // delete these numbers from the list of possible solutions of unknown
                for (int i = 0; i < 9; i++)
                {
                    if (puzzle[i][j].Count > 1)
                    {
                        iterationNum++;
                        foreach (int num in current)
                            puzzle[i][j].Remove(num);
                    }
                }

                current.Clear();
            }

            //By Box
            //Get all numbers in each box
            int a;
            int b;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            a = m + (3 * i);
                            b = n + (3 * j);
                            if (puzzle[a][b].Count == 1)
                            {
                                current.Add(puzzle[a][b][0]);
                            }
                        }
                    }

                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            a = m + (3 * i);
                            b = n + (3 * j);
                            if (puzzle[a][b].Count != 1)
                            {
                                iterationNum++;
                                foreach (int num in current)
                                {
                                    puzzle[a][b].Remove(num);
                                }
                            }
                        }
                    }
                    current.Clear();
                }
            }
        }

        // Forward checking method
        // Check for each unknown if there is a number in the domain that appears only once in the row, column, or box
        private void forwardChecking()
        {
            List<int> current = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count() > 1)
                    {
                        foreach (int num in puzzle[i][j])
                            current.Add(num);

                        //compare to column
                        for (int m = 0; m < 9; m++)
                        {
                            //check if its the same column
                            if (m != j)
                            {
                                if (puzzle[i][m].Count() > 1)
                                {
                                    iterationNum++;
                                    foreach (int num in puzzle[i][m])
                                    {
                                        current.Remove(num);
                                    }
                                }
                            }
                        }
                    }
                    if (current.Count() == 1)
                    {
                        puzzle[i][j].Clear();
                        puzzle[i][j].Add(current[0]);
                    }

                    current.Clear();
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count() > 1)
                    {
                        foreach (int num in puzzle[i][j])
                            current.Add(num);
                        //compare to row
                        for (int m = 0; m < 9; m++)
                        {
                            //check if its the same row
                            if (m != i)
                            {
                                if (puzzle[m][j].Count() > 1)
                                {
                                    iterationNum++;
                                    foreach (int num in puzzle[m][j])
                                    {
                                        current.Remove(num);
                                    }
                                }
                            }
                        }
                    }

                    if (current.Count() == 1)
                    {
                        puzzle[i][j].Clear();
                        puzzle[i][j].Add(current[0]);
                    }

                    current.Clear();
                }
            }


            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count() > 1)
                    {
                        foreach (int num in puzzle[i][j])
                            current.Add(num);

                        //compare to row and column
                        for (int m = 0; m < 9; m++)
                        {
                            //check if it’s the same column
                            if (m != j)
                            {
                                if (puzzle[i][m].Count() > 1)
                                {
                                    iterationNum++;
                                    foreach (int num in puzzle[i][m])
                                    {
                                        current.Remove(num);
                                    }
                                }
                            }
                            //check if it’s the same row
                            if (m != i)
                            {
                                if (puzzle[m][j].Count() > 1)
                                {
                                    iterationNum++;
                                    foreach (int num in puzzle[m][j])
                                    {
                                        current.Remove(num);
                                    }
                                }
                            }
                        }
                    }
                    if (current.Count() == 1)
                    {
                        puzzle[i][j].Clear();
                        puzzle[i][j].Add(current[0]);
                    }

                    current.Clear();
                }
            }
        }

        // Find the list with the fewest possible solutions in its domain
        // Guess that the first number in this list is the solution to that location
        private void backTracking()
        {
            int x = -1;
            int y = -1;
            int count = 9;

            //Find list with fewest elements
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count() > 1)
                    {
                        if (puzzle[i][j].Count() < count)
                        {
                            count = puzzle[i][j].Count();
                            x = i; y = j;
                        }
                    }
                }
            }

            //Record current state
            backNum++;
            int m = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    m = 0;
                    foreach (int num in puzzle[i][j])
                    {
                        prevStates[backNum][i][j][m] = num;
                        m++;
                    }
                    while (m < 9)
                    { prevStates[backNum][i][j][m] = 0; m++; }
                }
            }

            //Pick number from smallest list
            //And record info
            back[0][backNum] = x;
            back[1][backNum] = y;
            back[2][backNum] = 0;

            int n = puzzle[x][y][back[2][backNum]];
            puzzle[x][y].Clear();
            puzzle[x][y].Add(n);

            iterationNum++;
        }

        // If a guess was wrong
        // Go to the next number in the domain for the current location and make that the guess
        // If there are no more elements in the list to guess then go to the previous state
        // Then repeat the above steps
        private void reverseBackTracking()
        {
            int x = back[0][backNum];
            int y = back[1][backNum];
            int num1 = 0;
            int num2 = back[2][backNum] + 1;

            for (int i = 0; i < 9; i++)
            {
                if (prevStates[backNum][x][y][i] != 0)
                { num1++; }
            }

            if (num1 >= num2)
            {
                //replace and remove prevState
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        puzzle[i][j].Clear();
                        for (int n = 0; n < 9; n++)//)
                        {
                            if (prevStates[backNum][i][j][n] != 0)
                            { puzzle[i][j].Add(n); }
                            prevStates[backNum][i][j][n] = 0;
                        }
                    }
                }
                //remove info in back array
                back[0][backNum] = 0;
                back[1][backNum] = 0;
                back[2][backNum] = 0;

                backNum--;
            }

            x = back[0][backNum];
            y = back[1][backNum];
            num1 = 0;
            num2 = back[2][backNum] + 1;

            for (int i = 0; i < 9; i++)
            {
                if (prevStates[backNum][x][y][i] != 0)
                { num1++; }
            }
            if (num1 < num2)
            {
                back[2][backNum] += 1;

                puzzle[x][y].Clear();
                puzzle[x][y].Add(prevStates[backNum][x][y][back[2][backNum]]);
            }
            iterationNum++;
        }

        // Check if the current state is the goal state
        private bool checkGoal()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count != 1)
                    { return false; }
                }
            }
            return true;
        }

        // Find the number of unknown elements in the table
        private int findUnknown()
        {
            int unknown = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count() > 1)
                    { unknown++; }
                }
            }

            return unknown;
        }

        // Check if an error has happened
        // This is when a location has a domain containing no elements
        private bool checkError()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i][j].Count() == 0)
                    { return true; }
                }
            }
            return false;
        }

        // Initialize the array containing all the previous states
        private void initPrevStates()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int m = 0; m < 5; m++)
                    {
                        for (int n = 0; n < 9; n++)
                        { prevStates[m][i][j][n] = 0; }
                    }
                }
            }
        }

        // Start solving Sudoku
        private void Start_Click(object sender, EventArgs e)
        {
            findSolution();
        }

        // Change to the easy table
        private void C_Click(object sender, EventArgs e)
        {
            level = 1;
            clearSudoku();
            setLevels(level);
            pushLevel();
            display();

            backNum = -1;
            iterationNum = 0;
        }

        // Change to the medium table
        private void MediumBTN_Click(object sender, EventArgs e)
        {
            level = 2;
            clearSudoku();
            setLevels(level);
            pushLevel();
            display();

            backNum = -1;
            iterationNum = 0;
        }

    }
}

