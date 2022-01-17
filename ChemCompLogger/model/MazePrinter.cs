using NGraphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChemCompLogger.model.MazeGenerator;

namespace ChemCompLogger.model
{
    class MazePrinter
    {
        private AppState appState;
        private static MemoryStream graphicStream = new MemoryStream(6400000);
        private static StreamWriter graphicTextWriter = new StreamWriter(graphicStream);
        private static Dictionary<string, GraphicPosition> artDictionary;

        internal class GraphicPosition
        {
            public int startByte;
            public int endByte;

            public GraphicPosition(int startByte, int endByte)
            {
                this.startByte = startByte;
                this.endByte = endByte;
            }
        }

        internal void WriteStringToStream(Stream stream, String text)
        {
            foreach (char c in text)
            {
                stream.WriteByte((byte)c);
            }
        }

        private byte[] streamToStreamBuffer = new byte[32000000];

        internal void WriteStreamToStream(Stream stream, Stream stream2, int startByte, int endByte)
        {
            int bytesToCopy = endByte - startByte;
            stream2.Position = startByte;
            // no seek (assuming setting position seeks)
            stream2.Read(streamToStreamBuffer, 0, bytesToCopy);
            stream.Write(streamToStreamBuffer, 0, bytesToCopy);
        }

        // reading must occur in a single batch of "reads"
        // otherwise the position of the graphic memory stream
        // will not be at the proper positon for setting the
        // start and end byte and may overwrite existing
        // graphics
        internal void ReadSvgFromStream(NGraphics.Graphic graphic, Dictionary<string, GraphicPosition> graphicPositions, string nameOfGraphic)
        {
            var gfxPos = new GraphicPosition((int)graphicStream.Position, 0);

            graphic.WriteSvg(graphicTextWriter);
            graphicTextWriter.Flush();

            gfxPos.endByte = (int)graphicStream.Position;
            graphicPositions.Add(nameOfGraphic, gfxPos);
            // prior was flushing all bytes up to and including the first \n
        }

        internal void RenderMazeToSvg(Maze maze, AppState appState = null, string titleText = null)
        {
            if (appState != null)
            {
                this.appState = appState;
            }
            if (artDictionary == null)
            {
                artDictionary = new Dictionary<string, GraphicPosition>();
            }
            NGraphics.GraphicCanvas canvas = null;
            switch(this.appState.MazeStyle)
            {
                case "Normal":
                    canvas = PrintRegularMaze(maze);
                    break;
                case "Compact":
                    canvas = PrintCompactMaze(maze);
                    break;
                case "Compact Print-Friendly":
                    canvas = PrintCompactPrintFriendlyMaze(maze);
                    break;
            }

            if (!string.IsNullOrEmpty(titleText))
            {
                ReadSvgFromStream(canvas.Graphic, artDictionary, titleText);
                return;
            }
            ReadSvgFromStream(canvas.Graphic, artDictionary, "Maze #" + maze.seed);
        }

        private GraphicCanvas PrintCompactPrintFriendlyMaze(Maze maze)
        {
            var canvas = new NGraphics.GraphicCanvas(new NGraphics.Size((maze.w + 4) * 16D, (maze.h + 4) * 16D));
            int cx, cy;
            NGraphics.Path path;

            var bgColor = new NGraphics.Color();
            bgColor.R = 0;
            bgColor.G = 0;
            bgColor.B = 0;
            bgColor.A = 255;
            var bgBrush = new NGraphics.SolidBrush(bgColor);

            PrintCompactMazeTargets(canvas, maze.sx, maze.sy);
            PrintCompactMazeTargets(canvas, maze.fx, maze.fy);

            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    cx = x * 16 + 32;
                    cy = y * 16 + 32;

                    if (maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        continue;
                    }

                    // draw connections;
                    for (int c = 0; c < 8; c++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].conn[c])
                        {
                            continue;
                        }
                        switch (c)
                        {
                            case 0: // up
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx + 3, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 13, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 13, cy));
                                path.LineTo(new NGraphics.Point(cx + 3, cy));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 1: // up right
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx + 4.65, cy + 4.65));
                                path.LineTo(new NGraphics.Point(cx + 16 + 4.65, cy - 16 + 4.65));
                                path.LineTo(new NGraphics.Point(cx + 16 + 11.35, cy - 16 + 11.35));
                                path.LineTo(new NGraphics.Point(cx + 11.35, cy + 11.35));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 2: // right
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx + 16, cy + 3));
                                path.LineTo(new NGraphics.Point(cx + 16, cy + 13));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 13));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 3));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 4: // down
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx + 3, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 13, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 13, cy + 17));
                                path.LineTo(new NGraphics.Point(cx + 3, cy + 17));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 6: // left
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx, cy + 3));
                                path.LineTo(new NGraphics.Point(cx, cy + 13));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 13));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 3));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 7: // up left
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx + 4.65, cy + 11.35));
                                path.LineTo(new NGraphics.Point(cx - 16 + 4.65, cy - 16 + 11.35));
                                path.LineTo(new NGraphics.Point(cx - 16 + 11.35, cy - 16 + 4.65));
                                path.LineTo(new NGraphics.Point(cx + 11.35, cy + 4.65));
                                path.Close();
                                path.Draw(canvas);
                                break;
                        }
                    }

                    canvas.DrawEllipse(new Rect(new Point(cx + 3, cy + 3), new NGraphics.Size(10D)), null, bgBrush);
                }
            }

            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    cx = x * 16 + 32;
                    cy = y * 16 + 32;

                    if (maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        continue;
                    }

                    // draw connections;
                    for (int c = 0; c < 8; c++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].conn[c])
                        {
                            continue;
                        }
                        switch (c)
                        {
                            case 0: // up
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 4, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy));
                                path.LineTo(new NGraphics.Point(cx + 4, cy));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 1: // up right
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 5.65, cy + 5));
                                path.LineTo(new NGraphics.Point(cx + 16 + 5.65, cy - 16 + 5));
                                path.LineTo(new NGraphics.Point(cx + 16 + 10.65, cy - 16 + 10.65));
                                path.LineTo(new NGraphics.Point(cx + 10.65, cy + 10.65));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 2: // right
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 16, cy + 4));
                                path.LineTo(new NGraphics.Point(cx + 16, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 4));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 4: // down
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 4, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy + 17));
                                path.LineTo(new NGraphics.Point(cx + 4, cy + 17));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 6: // left
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx, cy + 4));
                                path.LineTo(new NGraphics.Point(cx, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 4));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 7: // up left
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 5, cy + 10.35));
                                path.LineTo(new NGraphics.Point(cx - 16 + 5, cy - 16 + 10.35));
                                path.LineTo(new NGraphics.Point(cx - 16 + 11, cy - 16 + 5.65));
                                path.LineTo(new NGraphics.Point(cx + 11, cy + 5.65));
                                path.Close();
                                path.Draw(canvas);
                                break;
                        }
                    }

                    canvas.DrawEllipse(new Rect(new Point(cx + 4, cy + 4), new NGraphics.Size(8D)), null, NGraphics.Brushes.White);
                }
            }

            PrintCompactMazeStartFinish(canvas, maze.sx, maze.sy);
            PrintCompactMazeStartFinish(canvas, maze.fx, maze.fy);

            return canvas;
        }

        private GraphicCanvas PrintCompactMaze(Maze maze)
        {
            var canvas = new NGraphics.GraphicCanvas(new NGraphics.Size((maze.w + 4) * 16D, (maze.h + 4) * 16D));
            int cx, cy;
            NGraphics.Path path;

            var bgColor = new NGraphics.Color();
            bgColor.R = 0;
            bgColor.G = 0;
            bgColor.B = 0;
            bgColor.A = 255;
            var bgBrush = new NGraphics.SolidBrush(bgColor);

            PrintCompactMazeTargets(canvas, maze.sx, maze.sy);
            PrintCompactMazeTargets(canvas, maze.fx, maze.fy);

            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    cx = x * 16 + 32;
                    cy = y * 16 + 32;

                    if (maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        // down/right
                        if (y != maze.h - 1 && x != maze.w - 1)
                        {
                            if (!maze.mazeNodes[(y + 1) * maze.w + x].outsideMaze &&
                                !maze.mazeNodes[y * maze.w + x + 1].outsideMaze)
                            {
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx + 16, cy));
                                path.LineTo(new NGraphics.Point(cx + 16, cy + 16));
                                path.LineTo(new NGraphics.Point(cx, cy + 16));
                                path.Close();
                                path.Draw(canvas);
                            }
                        }
                        // down/left
                        if (y != maze.h - 1 && x != 0)
                        {
                            if (!maze.mazeNodes[(y + 1) * maze.w + x].outsideMaze &&
                                !maze.mazeNodes[y * maze.w + x - 1].outsideMaze)
                            {
                                path = new NGraphics.Path(null,
                                   bgBrush);
                                path.MoveTo(new NGraphics.Point(cx, cy));
                                path.LineTo(new NGraphics.Point(cx + 16, cy + 16));
                                path.LineTo(new NGraphics.Point(cx, cy + 16));
                                path.Close();
                                path.Draw(canvas);
                            }
                        }
                        // up/right
                        if (y != 0 && x != maze.w - 1)
                        {
                            if (!maze.mazeNodes[(y - 1) * maze.w + x].outsideMaze &&
                                !maze.mazeNodes[y * maze.w + x + 1].outsideMaze)
                            {
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx, cy));
                                path.LineTo(new NGraphics.Point(cx + 16, cy));
                                path.LineTo(new NGraphics.Point(cx + 16, cy + 16));
                                path.Close();
                                path.Draw(canvas);
                            }
                        }
                        // up/left
                        if (y != 0 && x != 0)
                        {
                            if (!maze.mazeNodes[(y - 1) * maze.w + x].outsideMaze &&
                                !maze.mazeNodes[y * maze.w + x - 1].outsideMaze)
                            {
                                path = new NGraphics.Path(null,
                                bgBrush);
                                path.MoveTo(new NGraphics.Point(cx, cy));
                                path.LineTo(new NGraphics.Point(cx + 16, cy));
                                path.LineTo(new NGraphics.Point(cx, cy + 16));
                                path.Close();
                                path.Draw(canvas);
                            }
                        }
                    }
                    else // draw squares
                    {
                        path = new NGraphics.Path(null,
                                bgBrush);
                        path.MoveTo(new NGraphics.Point(cx, cy));
                        path.LineTo(new NGraphics.Point(cx + 16.1D, cy));
                        path.LineTo(new NGraphics.Point(cx + 16.1D, cy + 16.1D));
                        path.LineTo(new NGraphics.Point(cx, cy + 16.1D));
                        path.Close();
                        path.Draw(canvas);
                        //path = new NGraphics.Path(null,
                        //            NGraphics.Brushes.Black);
                        //path.MoveTo(new NGraphics.Point(cx + 4, cy));
                        //path.LineTo(new NGraphics.Point(cx + 12, cy));
                        //path.LineTo(new NGraphics.Point(cx + 17, cy + 4));
                        //path.LineTo(new NGraphics.Point(cx + 17, cy + 12));
                        //path.LineTo(new NGraphics.Point(cx + 12, cy + 17));
                        //path.LineTo(new NGraphics.Point(cx + 4, cy + 17));
                        //path.LineTo(new NGraphics.Point(cx, cy + 12));
                        //path.LineTo(new NGraphics.Point(cx, cy + 4));
                        //path.Close();
                        //path.Draw(canvas);
                    }
                }
            }
            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    cx = x * 16 + 32;
                    cy = y * 16 + 32;

                    if (maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        continue;
                    }

                    // draw connections;
                    for (int c = 0; c < 8; c++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].conn[c])
                        {
                            continue;
                        }
                        switch (c)
                        {
                            case 0: // up
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 4, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy));
                                path.LineTo(new NGraphics.Point(cx + 4, cy));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 1: // up right
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 5.65, cy + 5.65));
                                path.LineTo(new NGraphics.Point(cx + 16 + 5.65, cy - 16 + 5.65));
                                path.LineTo(new NGraphics.Point(cx + 16 + 10.35, cy - 16 + 10.35));
                                path.LineTo(new NGraphics.Point(cx + 10.35, cy + 10.35));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 2: // right
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 16, cy + 4));
                                path.LineTo(new NGraphics.Point(cx + 16, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 4));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 4: // down
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 4, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy + 8));
                                path.LineTo(new NGraphics.Point(cx + 12, cy + 17));
                                path.LineTo(new NGraphics.Point(cx + 4, cy + 17));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 6: // left
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx, cy + 4));
                                path.LineTo(new NGraphics.Point(cx, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 12));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 4));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 7: // up left
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 5.65, cy + 10.35));
                                path.LineTo(new NGraphics.Point(cx - 16 + 5.65, cy - 16 + 10.35));
                                path.LineTo(new NGraphics.Point(cx - 16 + 10.35, cy - 16 + 5.65));
                                path.LineTo(new NGraphics.Point(cx + 10.35, cy + 5));
                                path.Close();
                                path.Draw(canvas);
                                break;
                        }
                    }

                    canvas.DrawEllipse(new Rect(new Point(cx + 4, cy + 4), new NGraphics.Size(8D)), null, NGraphics.Brushes.White);
                }
            }

            PrintCompactMazeStartFinish(canvas, maze.sx, maze.sy);
            PrintCompactMazeStartFinish(canvas, maze.fx, maze.fy);

            return canvas;
        }

        private void PrintCompactMazeStartFinish(GraphicCanvas canvas, int nx, int ny)
        {
            canvas.DrawEllipse(new NGraphics.Rect(
                new NGraphics.Point(nx * 16 + 37, ny * 16 + 37),
                new NGraphics.Size(6D)), null, NGraphics.Brushes.Black);
        }

        internal void PrintCompactMazeTargets(NGraphics.GraphicCanvas canvas, int nx, int ny)
        {
            bool drewOpening = false;
            int cx = nx * 16 + 41;
            int cy = ny * 16 + 41;
            canvas.DrawEllipse(new Rect(new NGraphics.Point(cx - 32, cy - 32),
                new NGraphics.Size(64D)), null, NGraphics.Brushes.Black);
            canvas.DrawEllipse(new Rect(new NGraphics.Point(cx - 30, cy - 30),
                new NGraphics.Size(60D)), null, NGraphics.Brushes.White);
        }

        internal NGraphics.GraphicCanvas PrintRegularMaze(Maze maze)
        {
            var canvas = new NGraphics.GraphicCanvas(new NGraphics.Size((maze.w + 2) * 32D, (maze.h + 2) * 32D));

            PrintRegularMazeOpening(maze, canvas, maze.sx, maze.sy);
            PrintRegularMazeOpening(maze, canvas, maze.fx, maze.fy);

            int cx, cy;
            NGraphics.Path path;
            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    if (maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        continue;
                    }
                    cx = x * 32 + 32 + 8;
                    cy = y * 32 + 32 + 8;
                    // draw connections;
                    for (int c = 2; c < 6; c++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].conn[c])
                        {
                            continue;
                        }
                        switch (c)
                        {
                            case 2: // right
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.Black);
                                path.MoveTo(new NGraphics.Point(cx, cy - 8));
                                path.LineTo(new NGraphics.Point(cx + 32, cy - 8));
                                path.LineTo(new NGraphics.Point(cx + 32, cy + 8));
                                path.LineTo(new NGraphics.Point(cx, cy + 8));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 3: // right-down
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.Black);
                                path.MoveTo(new NGraphics.Point(cx - 5.65, cy + 5.65));
                                path.LineTo(new NGraphics.Point(cx + 32 - 5.65, cy + 32 + 5.65));
                                path.LineTo(new NGraphics.Point(cx + 32 + 5.65, cy + 32 - 5.65));
                                path.LineTo(new NGraphics.Point(cx + 5.65, cy - 5.65));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 4: // down
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.Black);
                                path.MoveTo(new NGraphics.Point(cx - 8, cy));
                                path.LineTo(new NGraphics.Point(cx - 8, cy + 32));
                                path.LineTo(new NGraphics.Point(cx + 8, cy + 32));
                                path.LineTo(new NGraphics.Point(cx + 8, cy));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 5: // down-left
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.Black);
                                path.MoveTo(new NGraphics.Point(cx + 5.65, cy + 5.65));
                                path.LineTo(new NGraphics.Point(cx - 32 + 5.65, cy + 32 + 5.65));
                                path.LineTo(new NGraphics.Point(cx - 32 - 5.65, cy + 32 - 5.65));
                                path.LineTo(new NGraphics.Point(cx - 5.65, cy - 5.65));
                                path.Close();
                                path.Draw(canvas);
                                break;
                        }
                    }
                    canvas.DrawEllipse(
                        new NGraphics.Rect(new NGraphics.Point(cx - 8, cy - 8), new NGraphics.Size(16D)), null, NGraphics.Brushes.Black);
                }
            }
            for (int y = 0; y < maze.h; y++)
            {
                for (int x = 0; x < maze.w; x++)
                {
                    if (maze.mazeNodes[y * maze.w + x].outsideMaze)
                    {
                        continue;
                    }
                    cx = x * 32 + 32 + 8;
                    cy = y * 32 + 32 + 8;
                    // draw connections;
                    for (int c = 2; c < 6; c++)
                    {
                        if (!maze.mazeNodes[y * maze.w + x].conn[c])
                        {
                            continue;
                        }
                        switch (c)
                        {
                            case 2: // right
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx, cy - 6));
                                path.LineTo(new NGraphics.Point(cx + 32, cy - 6));
                                path.LineTo(new NGraphics.Point(cx + 32, cy + 6));
                                path.LineTo(new NGraphics.Point(cx, cy + 6));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 3: // right-down
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx - 4.24, cy + 4.24));
                                path.LineTo(new NGraphics.Point(cx + 32 - 4.24, cy + 32 + 4.24));
                                path.LineTo(new NGraphics.Point(cx + 32 + 4.24, cy + 32 - 4.24));
                                path.LineTo(new NGraphics.Point(cx + 4.24, cy - 4.24));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 4: // down
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx - 6, cy));
                                path.LineTo(new NGraphics.Point(cx - 6, cy + 32));
                                path.LineTo(new NGraphics.Point(cx + 6, cy + 32));
                                path.LineTo(new NGraphics.Point(cx + 6, cy));
                                path.Close();
                                path.Draw(canvas);
                                break;
                            case 5: // down-left
                                path = new NGraphics.Path(null,
                                NGraphics.Brushes.White);
                                path.MoveTo(new NGraphics.Point(cx + 4.24, cy + 4.24));
                                path.LineTo(new NGraphics.Point(cx - 32 + 4.24, cy + 32 + 4.24));
                                path.LineTo(new NGraphics.Point(cx - 32 - 4.24, cy + 32 - 4.24));
                                path.LineTo(new NGraphics.Point(cx - 4.24, cy - 4.24));
                                path.Close();
                                path.Draw(canvas);
                                break;
                        }
                    }
                    canvas.DrawEllipse(
                        new NGraphics.Rect(new NGraphics.Point(cx - 6, cy - 6), new NGraphics.Size(12D)), null, NGraphics.Brushes.White);
                }
            }

            PrintRegularMazeStartFinish(maze, canvas);

            return canvas;
        }

        internal void PrintRegularMazeStartFinish(Maze maze, NGraphics.GraphicCanvas canvas)
        {
            canvas.DrawEllipse(new NGraphics.Rect(new NGraphics.Point(maze.sx * 32 + 36, maze.sy * 32 + 36),
                       new NGraphics.Size(8D)), null, NGraphics.Brushes.Black);
            canvas.DrawEllipse(new NGraphics.Rect(new NGraphics.Point(maze.fx * 32 + 36, maze.fy * 32 + 36),
                new NGraphics.Size(8D)), null, NGraphics.Brushes.Black);
        }

        internal void PrintRegularMazeOpening(Maze maze, NGraphics.GraphicCanvas canvas, int nx, int ny)
        {
            bool drewOpening = false;
            int cx = 0;
            int cy = 0;
            NGraphics.Path path;
            if (ny == 0 || maze.mazeNodes[(ny - 1) * maze.w + nx].outsideMaze)
            {
                cx = nx * 32 + 32;
                cy = (ny - 1) * 32 + 32;
                canvas.DrawEllipse(
                    new NGraphics.Rect(new NGraphics.Point(cx, cy),
                    new NGraphics.Size(16D)), null, NGraphics.Brushes.Black);
                path = new NGraphics.Path(null,
                            NGraphics.Brushes.Black);
                path.MoveTo(new NGraphics.Point(cx + 6, cy));
                path.LineTo(new NGraphics.Point(cx + 10, cy));
                path.LineTo(new NGraphics.Point(cx + 10, cy + 32));
                path.LineTo(new NGraphics.Point(cx + 6, cy + 32));
                path.Close();
                path.Draw(canvas);
                drewOpening = true;
            }
            else if (ny == maze.h - 1 || maze.mazeNodes[(ny + 1) * maze.w + nx].outsideMaze)
            {
                cx = nx * 32 + 32;
                cy = (ny + 1) * 32 + 32;
                canvas.DrawEllipse(
                    new NGraphics.Rect(new NGraphics.Point(cx, cy),
                    new NGraphics.Size(16D)), null, NGraphics.Brushes.Black);
                path = new NGraphics.Path(null,
                            NGraphics.Brushes.Black);
                path.MoveTo(new NGraphics.Point(cx + 6, cy));
                path.LineTo(new NGraphics.Point(cx + 10, cy));
                path.LineTo(new NGraphics.Point(cx + 10, cy - 32));
                path.LineTo(new NGraphics.Point(cx + 6, cy - 32));
                path.Close();
                path.Draw(canvas);
                drewOpening = true;
            }
            if (!drewOpening)
            {
                if (nx == 0 || maze.mazeNodes[ny * maze.w + nx - 1].outsideMaze)
                {
                    cx = (nx - 1) * 32 + 32;
                    cy = ny * 32 + 32;
                    canvas.DrawEllipse(
                        new NGraphics.Rect(new NGraphics.Point(cx, cy),
                        new NGraphics.Size(16D)), null, NGraphics.Brushes.Black);
                    path = new NGraphics.Path(null,
                                NGraphics.Brushes.Black);
                    path.MoveTo(new NGraphics.Point(cx + 6, cy + 6));
                    path.LineTo(new NGraphics.Point(cx + 6, cy + 10));
                    path.LineTo(new NGraphics.Point(cx + 6 + 32, cy + 10));
                    path.LineTo(new NGraphics.Point(cx + 6 + 32, cy + 6));
                    path.Close();
                    path.Draw(canvas);
                    drewOpening = true;
                }
                else if (nx == maze.w - 1 || maze.mazeNodes[ny * maze.w + nx + 1].outsideMaze)
                {
                    cx = (nx + 1) * 32 + 32;
                    cy = ny * 32 + 32;
                    canvas.DrawEllipse(
                        new NGraphics.Rect(new NGraphics.Point(cx, cy),
                        new NGraphics.Size(16D)), null, NGraphics.Brushes.Black);
                    path = new NGraphics.Path(null,
                                NGraphics.Brushes.Black);
                    path.MoveTo(new NGraphics.Point(cx + 6, cy + 6));
                    path.LineTo(new NGraphics.Point(cx + 6, cy + 10));
                    path.LineTo(new NGraphics.Point(cx + 6 - 32, cy + 10));
                    path.LineTo(new NGraphics.Point(cx + 6 - 32, cy + 6));
                    path.Close();
                    path.Draw(canvas);
                    drewOpening = true;
                }
            }
        }

        internal void PrintMazes(AppState appState, List<Maze> mazes)
        {
            this.appState = appState;
            artDictionary = new Dictionary<string, GraphicPosition>();
            foreach (Maze maze in mazes)
            {
                RenderMazeToSvg(maze);
            }
            Directory.CreateDirectory("Games");
            var dateStamp = System.DateTime.UtcNow.ToShortDateString() + "_" + System.DateTime.UtcNow.ToShortTimeString() + "-" + System.DateTime.UtcNow.Second;
            dateStamp = dateStamp.Replace("/", "-");
            dateStamp = dateStamp.Replace(":", "-");
            dateStamp = dateStamp.Replace(" ", "");
            FileStream fs;
            fs = File.Create("./Games/maze_" + dateStamp + ".html");

            var pathToExe = Properties.Resources.bootstrap_min;

            Stream contentStream = new MemoryStream(128000000);

            WriteStringToStream(contentStream, "<!DOCTYPE html><html><head>\n<style>" + pathToExe + "</style>\n</head><br/>");

            var custom_style = @"
table
{
    text-align: center;
    margin: 0;
}
td
{
    font-size: 1.5em;
    padding: 8px;
}
.credits
{
    font-size: 0.7em;
    margin-left: 44px;
    text-align: center;
}
h1, h2, h3, h4, h5, h6, p
{
    text-align: center;
}
";
            int printedMazes = 0;
            foreach (var gfxPosMember in artDictionary)
            {
                WriteStringToStream(contentStream, "<h1>" + gfxPosMember.Key + "</h1><br/><center>");
                WriteStreamToStream(contentStream, graphicStream,
                    gfxPosMember.Value.startByte, gfxPosMember.Value.endByte);
                WriteStringToStream(contentStream, "</center>");
                printedMazes++;
                // make sure content fills page
                WriteStringToStream(contentStream, "<p style=\"page-break-before: always\"/>\n");
            }

            WriteStringToStream(contentStream, string.Format("\n<style>{0}</style>\n", custom_style) +
                "</html>");

            // reset stream to head
            contentStream.Seek(0, SeekOrigin.Begin);
            do
            {
                var b = contentStream.ReadByte();
                if (b == -1)
                {
                    break;
                }
                fs.WriteByte((byte)b);
            } while (true);

            fs.Close();
            System.Diagnostics.Process.Start(fs.Name);
        }
    }
}
