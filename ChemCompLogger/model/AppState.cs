using ChemCompLogger.view;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemCompLogger.model
{
    class AppState
    {
        private MenuState menuState;
        private MazeGenerator gameGenerator;
        private string gameTemplate;
        private bool useDiagonals;
        private Queue<string> difficultyEnhancers;
        private Queue<string> mazeStyle;
        private int uniqueMazes;
        private Queue<string> mazeShapes;

        internal AppState(AppConsole appConsole)
        {
            menuState = new MenuState(appConsole);
            gameGenerator = new MazeGenerator();
            uniqueMazes = 1;

            mazeShapes = new Queue<string>();
            mazeShapes.Enqueue("Rectangle");
            mazeShapes.Enqueue("Pyramid");
            mazeShapes.Enqueue("Diamond");
            mazeShapes.Enqueue("Cross");
            mazeShapes.Enqueue("U");
            mazeShapes.Enqueue("Lemon");

            mazeStyle = new Queue<string>();
            mazeStyle.Enqueue("Normal");
            mazeStyle.Enqueue("Compact");
            mazeStyle.Enqueue("Compact Print-Friendly");

            difficultyEnhancers = new Queue<string>();
            difficultyEnhancers.Enqueue("None");
            difficultyEnhancers.Enqueue("Maze Gates");
            difficultyEnhancers.Enqueue("Maze Within Maze");
        }
        internal String GetStringFromBytes(byte[] bytes)
        {
            String s = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    break;
                }
                s += (char)bytes[i];
            }
            return s;
        }

        internal MenuState GetMenuState()
        {
            return menuState;
        }

        public MazeGenerator GameGenerator
        {
            get
            {
                return gameGenerator;
            }
        }

        public int UniqueMazes
        {
            get { return uniqueMazes; }
            set { uniqueMazes = value; }
        }

        public bool UseDiagonals
        {
            get { return useDiagonals; }
            set { useDiagonals = value; }
        }

        public string DifficultyEnhancer
        {
            get { return difficultyEnhancers.Peek(); }
        }

        public void ChangeDifficultyEnhancer()
        {
            var enhancer = difficultyEnhancers.Dequeue();
            difficultyEnhancers.Enqueue(enhancer);
        }

        public string MazeStyle
        {
            get { return mazeStyle.Peek(); }
        }

        public void ChangeMazeStyle()
        {
            var style = mazeStyle.Dequeue();
            mazeStyle.Enqueue(style);
        }

        public string MazeShape
        {
            get { return mazeShapes.Peek(); }
        }

        public void ChangeMazeShape()
        {
            var shape = mazeShapes.Dequeue();
            mazeShapes.Enqueue(shape);
        }
    }
}
