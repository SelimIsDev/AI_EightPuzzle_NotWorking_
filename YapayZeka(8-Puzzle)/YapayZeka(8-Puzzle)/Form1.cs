using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class PuzzleSolver
{
    private class Node
    {
        public int[,] State;
        public int G;
        public int H;
        public int F => G + H;
        public Node Parent;

        public Node(int[,] state, int g, int h, Node parent)
        {
            State = (int[,])state.Clone();
            G = g;
            H = h;
            Parent = parent;
        }
    }

    private readonly int[,] goalState =
    {
        { 1, 2, 3 },
        { 4, 5, 6 },
        { 7, 8, 0 }
    };

    public List<int[,]> Solve(int[,] startState)
    {
        var openList = new List<Node>();
        var closedSet = new HashSet<string>();

        Node startNode = new Node(startState, 0, CalculateH(startState), null);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            openList = openList.OrderBy(n => n.F).ToList();
            Node currentNode = openList.First();
            openList.RemoveAt(0);

            if (IsGoalState(currentNode.State))
                return ConstructPath(currentNode);

            closedSet.Add(ConvertToString(currentNode.State));

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!closedSet.Contains(ConvertToString(neighbor.State)))
                    openList.Add(neighbor);
            }
        }

        return null;
    }

    private List<int[,]> ConstructPath(Node node)
    {
        List<int[,]> path = new List<int[,]>();
        while (node != null)
        {
            path.Add(node.State);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    private bool IsGoalState(int[,] state)
    {
        return ConvertToString(state) == ConvertToString(goalState);
    }

    private int CalculateH(int[,] state)
    {
        int h = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (state[i, j] == 0) continue;
                int targetX = (state[i, j] - 1) / 3;
                int targetY = (state[i, j] - 1) % 3;
                h += Math.Abs(i - targetX) + Math.Abs(j - targetY);
            }
        }
        return h;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        (int x, int y) emptyTile = FindEmptyTile(node.State);

        int[][] moves = { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };

        foreach (var move in moves)
        {
            int newX = emptyTile.x + move[0];
            int newY = emptyTile.y + move[1];

            if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
            {
                int[,] newState = (int[,])node.State.Clone();
                newState[emptyTile.x, emptyTile.y] = newState[newX, newY];
                newState[newX, newY] = 0;

                Node newNode = new Node(newState, node.G + 1, CalculateH(newState), node);
                neighbors.Add(newNode);
            }
        }

        return neighbors;
    }

    private (int x, int y) FindEmptyTile(int[,] state)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (state[i, j] == 0)
                    return (i, j);

        return (-1, -1);
    }

    private string ConvertToString(int[,] state)
    {
        return string.Join(",", state.Cast<int>());
    }
}

public class Form1 : Form
{
    private Button solveButton;
    private PuzzleSolver solver = new PuzzleSolver();
    private int[,] initialGrid =
    {
        { 1, 2, 3 },
        { 4, 0, 5 },
        { 6, 7, 8 }
    };

    public Form1()
    {
        Text = "8 Puzzle A* Çözücü";
        Size = new System.Drawing.Size(320, 400);
        StartPosition = FormStartPosition.CenterScreen;

        solveButton = new Button
        {
            Text = "Çöz",
            Size = new System.Drawing.Size(100, 30),
            Location = new System.Drawing.Point(100, 300)
        };
        solveButton.Click += SolveButton_Click;

        Controls.Add(solveButton);
    }

    private void SolveButton_Click(object sender, EventArgs e)
    {
        List<int[,]> solution = solver.Solve(initialGrid);
        if (solution != null)
        {
            foreach (var state in solution)
            {
                PrintGrid(state);
                System.Threading.Thread.Sleep(500);
            }
        }
        else
        {
            MessageBox.Show("Çözüm bulunamadı.");
        }
    }

    private void PrintGrid(int[,] grid)
    {
        Console.Clear();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(grid[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("--------------");
    }
}