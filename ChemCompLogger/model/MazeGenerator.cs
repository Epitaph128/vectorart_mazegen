using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ChemCompLogger.model
{
    class MazeGenerator
    {
        private static Random randomGen = new Random();
        private AppState appState;
        private MazePrinter mazePrinter;

        internal class Maze
        {
            public MazeNode[] mazeNodes;
            public int w;
            public int h;
            public int sx;
            public int sy;
            public int fx;
            public int fy;
            public int seed;
            public HashSet<Tuple<int, int>> edgeNodes;
            public string mazeShape;

            public Maze(int seed, string mazeShape, int w, int h)
            {
                this.seed = seed;
                this.w = w;
                this.h = h;
                this.mazeNodes = new MazeNode[w * h];
                this.edgeNodes = new HashSet<Tuple<int, int>>();
                this.mazeShape = mazeShape;
            }
        }

        internal class MazeNode
        {
            public int x;
            public int y;
            public bool[] pconn;
            public bool[] conn;
            public bool outsideMaze;

            public MazeNode(int x, int y)
            {
                this.x = x;
                this.y = y;
                this.conn = new bool[8];
                this.pconn = new bool[8];
                this.outsideMaze = false;
            }
        }

        // assuming nodes designated are in bounds
        internal void ConnectNodes(Maze maze, int x, int y, int conn, bool connect)
        {
            switch(conn)
            {
                case 0: // up
                    maze.mazeNodes[y * maze.w + x].conn[0] = connect;
                    maze.mazeNodes[(y - 1) * maze.w + x].conn[4] = connect;
                    break;
                case 1: // up-right
                    maze.mazeNodes[y * maze.w + x].conn[1] = connect;
                    maze.mazeNodes[(y - 1) * maze.w + x + 1].conn[5] = connect;
                    break;
                case 2: // right
                    maze.mazeNodes[y * maze.w + x].conn[2] = connect;
                    maze.mazeNodes[y * maze.w + x + 1].conn[6] = connect;
                    break;
                case 3: // right-down
                    maze.mazeNodes[y * maze.w + x].conn[3] = connect;
                    maze.mazeNodes[(y + 1) * maze.w + x + 1].conn[7] = connect;
                    break;
                case 4: // down
                    maze.mazeNodes[y * maze.w + x].conn[4] = connect;
                    maze.mazeNodes[(y + 1) * maze.w + x].conn[0] = connect;
                    break;
                case 5: // down-left
                    maze.mazeNodes[y * maze.w + x].conn[5] = connect;
                    maze.mazeNodes[(y + 1) * maze.w + x - 1].conn[1] = connect;
                    break;
                case 6: // left
                    maze.mazeNodes[y * maze.w + x].conn[6] = connect;
                    maze.mazeNodes[y * maze.w + x - 1].conn[2] = connect;
                    break;
                case 7: // up-left
                    maze.mazeNodes[y * maze.w + x].conn[7] = connect;
                    maze.mazeNodes[(y - 1) * maze.w + x - 1].conn[3] = connect;
                    break;
            }
        }

        internal void RemoveNodeFromMaze(Maze maze, int x, int y, bool horzMazeGate = false, bool vertMazeGate = false)
        {
            if (horzMazeGate)
            {
                if (y != 0)
                {
                    ConnectNodes(maze, x, y, 0, false);
                    if (x < maze.w - 1)
                    {
                        ConnectNodes(maze, x, y, 1, false);
                    }
                    if (x != 0)
                    {
                        ConnectNodes(maze, x, y, 7, false);
                    }
                }
                return;
            }
            if (vertMazeGate)
            {
                if (y != 0)
                {
                    if (x < maze.w - 1)
                    {
                        ConnectNodes(maze, x, y, 1, false);
                    }
                }
                if (x < maze.w - 1)
                {
                    ConnectNodes(maze, x, y, 2, false);
                }
                if (y < maze.h - 1)
                {
                    if (x < maze.w - 1)
                    {
                        ConnectNodes(maze, x, y, 3, false);
                    }
                }
            }
            if (!horzMazeGate && !vertMazeGate)
            {
                maze.mazeNodes[y * maze.w + x].outsideMaze = true;
                if (y != 0)
                {
                    ConnectNodes(maze, x, y, 0, false);
                    if (x < maze.w - 1)
                    {
                        ConnectNodes(maze, x, y, 1, false);
                    }
                    if (x != 0)
                    {
                        ConnectNodes(maze, x, y, 7, false);
                    }
                }
                if (y < maze.h - 1)
                {
                    ConnectNodes(maze, x, y, 4, false);
                    if (x != 0)
                    {
                        ConnectNodes(maze, x, y, 5, false);
                    }
                    if (x < maze.w - 1)
                    {
                        ConnectNodes(maze, x, y, 3, false);
                    }
                }
                if (x < maze.w - 1)
                {
                    ConnectNodes(maze, x, y, 2, false);
                }
                if (x != 0)
                {
                    ConnectNodes(maze, x, y, 6, false);
                }
            }
        }

        internal bool NodeHasConnection(Maze maze, int x, int y)
        {
            var node = maze.mazeNodes[y * maze.w + x];
            for (int i = 0; i < 8; i++)
            {
                if (node.conn[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal Maze GenerateMaze(int width = 36, int height = 44, bool mazeWithinMaze = false)
        {
            if (appState.MazeStyle != "Normal" && !mazeWithinMaze)
            {
                width = 71;
                height = 88;
            }
            var maze = new Maze(randomGen.Next(), appState.MazeShape, width, height);
            randomGen = new Random(maze.seed);

            bool[] diagonalDown = null;
            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    var mazeNode = new MazeNode(x, y);
                    maze.mazeNodes[y * maze.w + x] = mazeNode;
                }
            }
            bool connectDiagonal = false;
            bool leftSideConnection = randomGen.Next(2) == 0;
            var angleOfOneX = Math.PI / maze.w;
            for (int y = 0; y < maze.h; y++)
            {
                diagonalDown = new bool[maze.w];
                for (int i = 0; i < maze.w; i++)
                {
                    diagonalDown[i] = randomGen.NextDouble() < 0.5D;
                }
                for (int x = 0; x < maze.w; x++)
                {
                    // restrict connections outside of grid
                    if (x < maze.w - 1)
                    {
                        ConnectNodes(maze, x, y, 2, true);
                    }
                    if (y < maze.h - 1)
                    {
                        ConnectNodes(maze, x, y, 4, true);
                    }
                    if (appState.UseDiagonals && !mazeWithinMaze)
                    {
                        if (connectDiagonal)
                        {
                            if (x > 0 && y < maze.h - 1)
                            {
                                ConnectNodes(maze, x, y, 5, true);
                            }
                            connectDiagonal = false;
                        }
                        if (diagonalDown[x])
                        {
                            if (x < maze.w - 1 && y < maze.h - 1)
                            {
                                ConnectNodes(maze, x, y, 3, true);
                            }
                        }
                        else
                        {
                            connectDiagonal = true;
                        }
                    }
                }
            }

            if (!mazeWithinMaze)
            {
                bool[] safeNodes = new bool[maze.w * maze.h];
                switch (appState.MazeShape)
                {
                    case "Rectangle":
                        break;
                    case "Lemon":
                        for (int x = 1; x < maze.w; x++)
                        {
                            var nodesInCol = Math.Round(Math.Sin(x * angleOfOneX) * (maze.w - 6)) / 2D;

                            for (int y = (int)(maze.h / 2D - nodesInCol); y <= (int)(maze.h / 2D + nodesInCol); y++)
                            {
                                safeNodes[y * maze.w + x] = true;
                            }
                        }
                        for (int x = 0; x < maze.w; x++)
                        {
                            for (int y = 0; y < maze.h; y++)
                            {
                                if (!safeNodes[y * maze.w + x])
                                {
                                    RemoveNodeFromMaze(maze, x, y);
                                }
                            }
                        }
                        break;
                    case "Cross":
                        int wf = (int)Math.Ceiling(maze.w / 3D);
                        int hf = (int)Math.Ceiling(maze.h / 3D);
                        int hf2 = (int)Math.Ceiling(maze.h / 6D);
                        for (int y = 0; y < maze.h; y++)
                        {
                            for (int x = wf; x < wf * 2; x++)
                            {
                                safeNodes[y * maze.w + x] = true;
                            }
                        }
                        for (int y = hf - hf2; y < hf * 2 - hf2; y++)
                        {
                            for (int x = 0; x < maze.w; x++)
                            {
                                safeNodes[y * maze.w + x] = true;
                            }
                        }
                        for (int x = 0; x < maze.w; x++)
                        {
                            for (int y = 0; y < maze.h; y++)
                            {
                                if (!safeNodes[y * maze.w + x])
                                {
                                    RemoveNodeFromMaze(maze, x, y);
                                }
                            }
                        }
                        break;
                    case "Pyramid":
                        int nc = 0;
                        int heightDiffCol = maze.h / (maze.w / 2);
                        for (int x = 0; x < maze.w; x++)
                        {
                            if (x < maze.w / 2)
                            {
                                nc += heightDiffCol;
                                if (nc > maze.h)
                                {
                                    nc = maze.h;
                                }
                            }
                            else
                            {
                                nc -= heightDiffCol;
                                if (nc < 0)
                                {
                                    nc = 0;
                                }
                            }
                            for (int y = maze.h - 1; y > maze.h - 1 - nc; y--)
                            {
                                safeNodes[y * maze.w + x] = true;
                            }
                        }
                        for (int x = 0; x < maze.w; x++)
                        {
                            for (int y = 0; y < maze.h; y++)
                            {
                                if (!safeNodes[y * maze.w + x])
                                {
                                    RemoveNodeFromMaze(maze, x, y);
                                }
                            }
                        }
                        break;
                    case "U":
                        wf = (int)Math.Ceiling(maze.w / 3D);
                        hf = (int)Math.Ceiling(maze.h / 3D);
                        for (int x = wf; x < wf * 2; x++)
                        {
                            for (int y = 0; y < hf * 2; y++)
                            {
                                RemoveNodeFromMaze(maze, x, y);
                            }
                        }
                        break;
                    case "Diamond":
                        wf = (int)Math.Floor(maze.w / 2D);
                        hf = (int)Math.Floor(maze.h / 2D);
                        double m = -(double)hf / (double)wf;
                        for (int x = 0; x < maze.w; x++)
                        {
                            for (int y = 0; y < maze.h; y++)
                            {
                                if (y <= (double)x * m + hf)
                                {
                                    RemoveNodeFromMaze(maze, x, y);
                                }
                                if (y >= (double)x * -m + hf)
                                {
                                    RemoveNodeFromMaze(maze, x, y);
                                }
                                if (x > wf)
                                {
                                    if (y <= (double)(x - wf) * -m)
                                    {
                                        RemoveNodeFromMaze(maze, x, y);
                                    }
                                    if (y >= (double)(x - wf) * m + hf * 2)
                                    {
                                        RemoveNodeFromMaze(maze, x, y);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("Shape: \"" + appState.MazeShape + "\" doesn't exist");
                }
            }

            if (appState.DifficultyEnhancer == "Maze Within Maze" && !mazeWithinMaze)
            {
                var divFactor = 8;
                var mazeLayout = GenerateMaze(width / divFactor, height / divFactor, true);
                for (int y = 0; y < mazeLayout.h; y++)
                {
                    for (int x = 0; x < mazeLayout.w; x++)
                    {
                        // down
                        if (!mazeLayout.mazeNodes[y * mazeLayout.w + x].conn[4])
                        {
                            var y2 = (y + 1) * divFactor;
                            int x2 = x * divFactor;
                            if (x2 - 1 >= 0 && y2 < maze.h)
                            {
                                ConnectNodes(maze, x2 - 1, y2, 1, false);
                            }
                            for (; x2 < (x + 1) * divFactor; x2++)
                            {
                                if (y2 < maze.h)
                                {
                                    RemoveNodeFromMaze(maze, x2, y2, true);
                                }
                            }
                            if (x2 < maze.w && y2 < maze.h)
                            {
                                ConnectNodes(maze, x2, y2, 7, false);
                            }
                        }
                        // right
                        if (!mazeLayout.mazeNodes[y * mazeLayout.w + x].conn[2])
                        {
                            var x2 = (x + 1) * divFactor - 1;
                            int y2 = y * divFactor;
                            if (y2 - 1 >= 0)
                            {
                                ConnectNodes(maze, x2, y2 - 1, 3, false);
                            }
                            for (; y2 < (y + 1) * divFactor; y2++)
                            {
                                if (x2 < maze.w)
                                {
                                    RemoveNodeFromMaze(maze, x2, y2, false, true);
                                }
                            }
                            if (x2 < maze.w && y2 < maze.h)
                            {
                                ConnectNodes(maze, x2, y2, 1, false);
                            }
                        }
                    }
                }
                for (int x = 0; x < maze.w; x++)
                {
                    for (int y = 0; y < maze.h; y++)
                    {
                        if (x >= (width / divFactor) * divFactor || y >= (height / divFactor) * divFactor)
                        {
                            RemoveNodeFromMaze(maze, x, y, false);
                        }
                    }
                }
            }

            if (appState.DifficultyEnhancer == "Maze Gates" && !mazeWithinMaze)
            {
                switch (appState.MazeShape)
                {
                    case "Rectangle":
                    case "Cross":
                        int distY = randomGen.Next(8) + 6;
                        while (distY < maze.h - 5)
                        {
                            // TODO: currently all inside nodes must appear
                            // in a single row (no gaps) for this to work
                            int nodesInsideMazeForDistYRow = 0;
                            int firstNodeInsideMaze = -1;
                            for (int x = 0; x < maze.w; x++)
                            {
                                if (firstNodeInsideMaze == -1 && !maze.mazeNodes[distY * maze.w + x].outsideMaze)
                                {
                                    firstNodeInsideMaze = x;
                                }
                                nodesInsideMazeForDistYRow +=
                                    maze.mazeNodes[distY * maze.w + x].outsideMaze ? 0 : 1;
                            }
                            if (firstNodeInsideMaze == -1 || nodesInsideMazeForDistYRow >= 3)
                            {
                                int holeAt = randomGen.Next(nodesInsideMazeForDistYRow - 2) + firstNodeInsideMaze + 1;
                                for (int x = 0; x < maze.w; x++)
                                {
                                    if (Math.Abs(x - holeAt) > 2)
                                    {
                                        RemoveNodeFromMaze(maze, x, distY, true);
                                    }
                                }

                            }
                            distY += randomGen.Next(8) + 6;
                        }
                        break;
                    case "Pyramid":
                        int nodesIntoMaze = 0;
                        for (int y = 0; y < maze.h; y++)
                        {
                            if (nodesIntoMaze > 4)
                            {
                                RemoveNodeFromMaze(maze, maze.w / 2 - 1, y, false);
                                continue;
                            }
                            if (!maze.mazeNodes[y * maze.w + maze.w / 2 - 1].outsideMaze)
                            {
                                nodesIntoMaze++;
                            }
                        }
                        break;
                }
            }

            // swap the connections set to the pconn storage
            // (indicating possible connections, not actual)
            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    var tempConn = maze.mazeNodes[y * maze.w + x].pconn;
                    maze.mazeNodes[y * maze.w + x].pconn = maze.mazeNodes[y * maze.w + x].conn;
                    maze.mazeNodes[y * maze.w + x].conn = tempConn;
                }
            }

            // detect edge nodes
            for (int y = 0; y < maze.h; y++)
            {
                bool firstNodeInRow = false;
                bool lastNodeInRow = false;
                for (int x = 0; x < maze.w; x++)
                {
                    if (!firstNodeInRow && !maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        maze.edgeNodes.Add(new Tuple<int, int>(x, y));
                        firstNodeInRow = true;
                    }
                }
                for (int x = maze.w - 1; x >= 0; x--)
                {
                    if (!lastNodeInRow && !maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        maze.edgeNodes.Add(new Tuple<int, int>(x, y));
                        lastNodeInRow = true;
                    }
                }
            }
            for (int x = 0; x < maze.w; x++)
            {
                bool firstNodeInCol = false;
                bool lastNodeInCol = false;
                for (int y = 0; y < maze.h; y++)
                {
                    if (!firstNodeInCol && !maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        maze.edgeNodes.Add(new Tuple<int, int>(x, y));
                        firstNodeInCol = true;
                    }
                }
                for (int y = maze.h - 1; y >= 0; y--)
                {
                    if (!lastNodeInCol && !maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        maze.edgeNodes.Add(new Tuple<int, int>(x, y));
                        lastNodeInCol = true;
                    }
                }
            }

            // sets the start node
            CreateMazePaths(maze);

            // test maze w/ flood fill
            int[] distances = FloodFillMaze(maze, maze.sx, maze.sy);

            // sets the finish node
            int furthestNodeDist = 0;
            foreach (var node in maze.edgeNodes)
            {
                if (distances[node.Item2 * maze.w + node.Item1] > furthestNodeDist)
                {
                    furthestNodeDist = distances[node.Item2 * maze.w + node.Item1];
                    maze.fx = node.Item1;
                    maze.fy = node.Item2;
                }
            }

            // adjusts start node further from finish node if possible
            distances = FloodFillMaze(maze, maze.fx, maze.fy);
            furthestNodeDist = 0;
            foreach (var node in maze.edgeNodes)
            {
                if (distances[node.Item2 * maze.w + node.Item1] > furthestNodeDist)
                {
                    furthestNodeDist = distances[node.Item2 * maze.w + node.Item1];
                    maze.sx = node.Item1;
                    maze.sy = node.Item2;
                }
            }

            return maze;
        }

        private void CreateMazePaths(Maze maze)
        {
            var allEdgeNodes = maze.edgeNodes.ToList<Tuple<int, int>>();

            var startNode = allEdgeNodes[randomGen.Next(maze.edgeNodes.Count)];
            maze.sx = startNode.Item1;
            maze.sy = startNode.Item2;

            // generate the maze using a backtracking algo
            int psx = maze.sx;
            int psy = maze.sy;
            bool[] nodesConnected = new bool[maze.w * maze.h];
            var nodeStack = new Stack<Tuple<int, int>>();
            int nodesToConnect = 0;
            for (int i = 0; i < maze.w * maze.h; i++)
            {
                if (!maze.mazeNodes[i].outsideMaze)
                {
                    nodesToConnect++;
                }
            }
            nodesToConnect--;

            nodesConnected[psy * maze.w + psx] = true;
            nodeStack.Push(new Tuple<int, int>(psx, psy));

            var pathOrder = new List<Tuple<int, int>>();
            pathOrder.Add(new Tuple<int, int>(psx, psy));

            var jumpedNodes = new List<Tuple<int, int>>();

            bool nodesLeftUnconnected = false;

            while (nodesToConnect > 0)
            {
                // generate the random sequence of connections to attempt
                List<int> connectionTrialSequence = new List<int>();
                for (int c = 0; c < 8; c++)
                {
                    if (maze.mazeNodes[psy * maze.w + psx].pconn[c])
                    {
                        connectionTrialSequence.Add(c);
                    }
                }
                var shuffles = 16;
                for (int n = 0; n < shuffles; n++)
                {
                    var p1 = randomGen.Next(connectionTrialSequence.Count);
                    var p2 = randomGen.Next(connectionTrialSequence.Count);
                    if (p1 != p2)
                    {
                        var t = connectionTrialSequence[p1];
                        connectionTrialSequence[p1] = connectionTrialSequence[p2];
                        connectionTrialSequence[p2] = t;
                    }
                }
                // in order, trial the connections to attempt until a suitable is found
                bool connectionMade = false;
                for (int c = 0; c < connectionTrialSequence.Count; c++)
                {
                    if (connectionMade) break;
                    switch (connectionTrialSequence[c])
                    {
                        case 0: // up
                            if (psy == 0) continue;
                            if (!nodesConnected[(psy - 1) * maze.w + psx])
                            {
                                ConnectNodes(maze, psx, psy, 0, true);
                                psy--;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 1: // up-right
                            if (psy == 0 || psx == maze.w - 1) continue;
                            if (!nodesConnected[(psy - 1) * maze.w + psx + 1])
                            {
                                ConnectNodes(maze, psx, psy, 1, true);
                                psy--;
                                psx++;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 2: // right
                            if (psx == maze.w - 1) continue;
                            if (!nodesConnected[psy * maze.w + psx + 1])
                            {
                                ConnectNodes(maze, psx, psy, 2, true);
                                psx++;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 3: // right-down
                            if (psx == maze.w - 1 || psy == maze.h - 1) continue;
                            if (!nodesConnected[(psy + 1) * maze.w + psx + 1])
                            {
                                ConnectNodes(maze, psx, psy, 3, true);
                                psx++;
                                psy++;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 4: // down
                            if (psy == maze.h - 1) continue;
                            if (!nodesConnected[(psy + 1) * maze.w + psx])
                            {
                                ConnectNodes(maze, psx, psy, 4, true);
                                psy++;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 5: // down-left
                            if (psx == 0 || psy == maze.h - 1) continue;
                            if (!nodesConnected[(psy + 1) * maze.w + psx - 1])
                            {
                                ConnectNodes(maze, psx, psy, 5, true);
                                psx--;
                                psy++;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 6: // left
                            if (psx == 0) continue;
                            if (!nodesConnected[psy * maze.w + psx - 1])
                            {
                                ConnectNodes(maze, psx, psy, 6, true);
                                psx--;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                        case 7: // up-left
                            if (psx == 0 || psy == 0) continue;
                            if (!nodesConnected[(psy - 1) * maze.w + psx - 1])
                            {
                                ConnectNodes(maze, psx, psy, 7, true);
                                psx--;
                                psy--;
                                nodesConnected[psy * maze.w + psx] = true;
                                connectionMade = true;
                            }
                            break;
                    }
                }
                if (connectionMade)
                {
                    nodesToConnect--;
                    nodeStack.Push(new Tuple<int, int>(psx, psy));
                    pathOrder.Add(new Tuple<int, int>(psx, psy));
                }
                else
                {
                    if (nodeStack.Count == 0)
                    {
                        if (jumpedNodes.Count != 0)
                        {
                            var ni = randomGen.Next(jumpedNodes.Count);
                            var p = jumpedNodes[ni];
                            psx = p.Item1;
                            psy = p.Item2;
                            jumpedNodes.RemoveAt(ni);
                        }
                        else
                        {
                            nodesLeftUnconnected = true;
                            break;
                        }
                    }
                    else
                    {
                        var nodesToBackTrack = randomGen.Next(nodeStack.Count) + 1;
                        while (nodesToBackTrack > 0)
                        {
                            if (nodesToBackTrack > 1)
                            {
                                jumpedNodes.Add(nodeStack.Pop());
                            }
                            else
                            {
                                var p = nodeStack.Pop();
                                psx = p.Item1;
                                psy = p.Item2;
                            }
                            nodesToBackTrack--;
                        }
                    }
                }
            }

            // assure all nodes are connected into maze
            // after main backtracking/path-creation phase
            if (nodesLeftUnconnected)
            {
                // TODO: functionalize backtracking logic to allow
                // recursive backtracking from different starting nodes
                // (i.e. if some are unconnected, then start connecting from
                // the unconnected nodes until none remain
                for (int y = 0; y < maze.h; y++)
                {
                    for (int x = 0; x < maze.w; x++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].outsideMaze && !nodesConnected[y * maze.w + x])
                        {
                            if (y != 0)
                            {
                                if (nodesConnected[(y - 1) * maze.w + x])
                                {
                                    ConnectNodes(maze, x, y, 0, true);
                                    continue;
                                }
                            }
                            if (y != maze.h - 1)
                            {
                                if (nodesConnected[(y + 1) * maze.w + x])
                                {
                                    ConnectNodes(maze, x, y, 4, true);
                                    continue;
                                }
                            }
                            if (x != 0)
                            {
                                if (nodesConnected[y * maze.w + x - 1])
                                {
                                    ConnectNodes(maze, x, y, 6, true);
                                    continue;
                                }
                            }
                            if (x != maze.w - 1)
                            {
                                if (nodesConnected[y * maze.w + x + 1])
                                {
                                    ConnectNodes(maze, x, y, 2, true);
                                    continue;
                                }
                            }

                            throw new Exception("Node (" + x + ", " + y + ") unconnectable to maze.");
                        }
                    }
                }
            }
        }

        private int[] FloodFillMaze(Maze maze, int sx, int sy)
        {
            int[] floodDistance = new int[maze.w * maze.h];

            for (int i = 0; i < maze.w * maze.h; i++)
            {
                floodDistance[i] = int.MaxValue;
            }

            int currentDistance = 0;
            int x = sx;
            int y = sy;

            var fillCurrent = new List<Tuple<int, int>>();
            fillCurrent.Add(new Tuple<int, int>(x, y));
            var fillFromNext = new List<Tuple<int, int>>();

            while (fillCurrent.Count > 0)
            {
                currentDistance++;
                foreach (var pt in fillCurrent)
                {
                    x = pt.Item1;
                    y = pt.Item2;
                    for (int c = 0; c < 8; c++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].conn[c]) continue;
                        switch (c)
                        {
                            case 0: // up
                                if (currentDistance >= floodDistance[(y - 1) * maze.w + x]) continue;
                                floodDistance[(y - 1) * maze.w + x] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x, y - 1));
                                break;
                            case 1: // up-right
                                if (currentDistance >= floodDistance[(y - 1) * maze.w + x + 1]) continue;
                                floodDistance[(y - 1) * maze.w + x + 1] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x + 1, y - 1));
                                break;
                            case 2: // right
                                if (currentDistance >= floodDistance[y * maze.w + x + 1]) continue;
                                floodDistance[y * maze.w + x + 1] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x + 1, y));
                                break;
                            case 3: // right-down
                                if (currentDistance >= floodDistance[(y + 1) * maze.w + x + 1]) continue;
                                floodDistance[(y + 1) * maze.w + x + 1] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x + 1, y + 1));
                                break;
                            case 4: // down
                                if (currentDistance >= floodDistance[(y + 1) * maze.w + x]) continue;
                                floodDistance[(y + 1) * maze.w + x] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x, y + 1));
                                break;
                            case 5: // down-left
                                if (currentDistance >= floodDistance[(y + 1) * maze.w + x - 1]) continue;
                                floodDistance[(y + 1) * maze.w + x - 1] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x - 1, y + 1));
                                break;
                            case 6: // left
                                if (currentDistance >= floodDistance[y * maze.w + x - 1]) continue;
                                floodDistance[y * maze.w + x - 1] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x - 1, y));
                                break;
                            case 7: // up-left
                                if (currentDistance >= floodDistance[(y - 1) * maze.w + x - 1]) continue;
                                floodDistance[(y - 1) * maze.w + x - 1] = currentDistance;
                                fillFromNext.Add(new Tuple<int, int>(x - 1, y - 1));
                                break;
                        }
                    }
                }
                fillCurrent.Clear();
                fillCurrent.AddRange(fillFromNext);
                fillFromNext.Clear();
            }

            for (int i = 0; i < maze.w * maze.h; i++)
            {
                if (!maze.mazeNodes[i].outsideMaze && floodDistance[i] == int.MaxValue)
                {
                    throw new Exception("Unreachable node in maze: " + i);
                }
            }

            return floodDistance;
        }

        internal void CreateMazes(AppState appState)
        {
            this.appState = appState;
            if (this.mazePrinter == null)
            {
                this.mazePrinter = new MazePrinter();
            }
            var mazes = new List<Maze>();
            for (int i = 0; i < appState.UniqueMazes; i++)
            {
                mazes.Add(GenerateMaze());
            }
            mazePrinter.PrintMazes(appState, mazes);
        }
    }
}
