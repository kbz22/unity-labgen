using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LabyrinthNamespace
{
    public enum LabyrinthDirection { Up, Down, Left, Right, None = -1 }

    public class Labyrinth
    {
        private LabyrinthDirection[,] labyrinthDirections;
        private Vector2Int entryPoint = new Vector2Int(0, 0);
        private Vector2Int exitPoint = new Vector2Int(0, 0);

        public int Width
        {
            get { return this.labyrinthDirections.GetLength(0); }
        }

        public int Height
        {
            get { return this.labyrinthDirections.GetLength(1); }
        }

        public Vector2Int EntryPoint
        {
            get { return this.entryPoint; }
            set
            {                
                entryPoint = new Vector2Int(Math.Min(labyrinthDirections.GetLength(0), Math.Max(0, value.x)),
                    Math.Min(labyrinthDirections.GetLength(1), Math.Max(0, value.y)));
            }
        }
        public Vector2Int ExitPoint
        {
            get { return exitPoint; }
            set
            {
                exitPoint = new Vector2Int(Math.Min(labyrinthDirections.GetLength(0), Math.Max(0, value.x)), 
                    Math.Min(labyrinthDirections.GetLength(1), Math.Max(0, value.y)));
            }
        }

        public LabyrinthDirection this[int x, int y]
        {
            get
            {
                LabyrinthDirection direction = LabyrinthDirection.None;

                try
                {
                    direction = labyrinthDirections[x, y];
                }
                catch //(Exception e)
                {
                    //Debug.Log("Labyrinth get error at [" + x.ToString() + "," + y.ToString() + "]:\n" + e.Message);
                }

                return direction;
            }

            set
            {
                try
                {
                    labyrinthDirections[x, y] = value;
                }
                catch //(Exception e)
                {
                    //Debug.Log("Labyrinth set error at [" + x.ToString() + "," + y.ToString() + "]:\n" + e.Message);
                }
            }
        }

        public Labyrinth(Vector2Int size)
        {
            this.labyrinthDirections = new LabyrinthDirection[size.x, size.y];

            this.Clear();
            this.Generate(size);
        }

        private void Clear()
        {
            for (int i = 0; i < this.Width; i++)
                for (int k = 0; k < this.Height; k++)
                    this.labyrinthDirections[i, k] = LabyrinthDirection.None;
        }

        private void Generate(Vector2Int size)
        {     
            var rand = new System.Random();
            this.EntryPoint = new Vector2Int(rand.Next(this.Width - 1), 0);
            this.ExitPoint = new Vector2Int(rand.Next(this.Width-1), this.Height-1);

            Stack<Vector2Int> cellStack = new Stack<Vector2Int>();
            List<LabyrinthDirection> availableDirection = new List<LabyrinthDirection>();
            Vector2Int currentCell = entryPoint;
            cellStack.Push(EntryPoint);
            
            // Generate main path
            // random turns
            while (true)
            {
                // Take a cell from the stack if possible - break the loop otherwise
                try { cellStack.Peek(); }
                catch { break; }

                currentCell = cellStack.Pop();

                // Check which cells are unvisited and possible to visit
                try { if (this.labyrinthDirections[currentCell.x + 1, currentCell.y] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Right); }
                catch {}
                try { if (this.labyrinthDirections[currentCell.x - 1, currentCell.y] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Left); }
                catch { }
                try { if (this.labyrinthDirections[currentCell.x, currentCell.y + 1] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Up); }
                catch { }
                try { if (this.labyrinthDirections[currentCell.x, currentCell.y - 1] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Down); }
                catch { }

                if (availableDirection.Count == 0) continue;

                // randomly order available directions, then pick the first one and put it on the stack
                availableDirection = availableDirection.OrderBy(x => rand.Next()).ToList();                

                this.labyrinthDirections[currentCell.x, currentCell.y] = availableDirection[0];
                switch (availableDirection[0])
                {
                    case LabyrinthDirection.Up:
                        cellStack.Push(new Vector2Int(currentCell.x,currentCell.y + 1));
                        break;
                    case LabyrinthDirection.Down:
                        cellStack.Push(new Vector2Int(currentCell.x, currentCell.y - 1));
                        break;
                    case LabyrinthDirection.Left:
                        cellStack.Push(new Vector2Int(currentCell.x -1, currentCell.y));
                        break;
                    case LabyrinthDirection.Right:
                        cellStack.Push(new Vector2Int(currentCell.x + 1, currentCell.y));
                        break;
                }

                availableDirection.Clear();
            }            
           
            bool unvisitedFieldFlag;
            LabyrinthDirection[,] tmp = new LabyrinthDirection[this.Width, this.Height];

            // copy current labyrinth into a tmp
            for (int i = 0; i < Width; i++)
                for (int k = 0; k < Height; k++)
                    tmp[i, k] = this.labyrinthDirections[i, k];

            while (true)
            {
                // look for an unvisited cell, put it on the stack
                unvisitedFieldFlag = false;
                for (int i = 0; i < Width; i++)
                {
                    for (int k = 0; k < Height; k++)
                        if (this.labyrinthDirections[i, k] == LabyrinthDirection.None)
                        {
                            cellStack.Push(new Vector2Int(i, k));
                            unvisitedFieldFlag = true;
                            break;
                        }
                    if (unvisitedFieldFlag) break;
                }

                // if all cells were visited the generation is done
                if (!unvisitedFieldFlag) break;

                availableDirection.Clear();

                // Generate side paths
                while (true)
                {
                    try { cellStack.Peek(); }
                    catch { break; }

                    currentCell = cellStack.Pop();
                                        
                    try { if (tmp[currentCell.x + 1, currentCell.y] != LabyrinthDirection.None || labyrinthDirections[currentCell.x + 1, currentCell.y] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Right); }
                    catch { }
                    try { if (tmp[currentCell.x - 1, currentCell.y] != LabyrinthDirection.None || labyrinthDirections[currentCell.x - 1, currentCell.y] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Left); }
                    catch { }
                    try { if (tmp[currentCell.x, currentCell.y + 1] != LabyrinthDirection.None || labyrinthDirections[currentCell.x, currentCell.y + 1] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Up); }
                    catch { }
                    try { if (tmp[currentCell.x, currentCell.y - 1] != LabyrinthDirection.None || labyrinthDirections[currentCell.x, currentCell.y - 1] == LabyrinthDirection.None) availableDirection.Add(LabyrinthDirection.Down); }
                    catch { }

                    availableDirection = availableDirection.OrderBy(x => rand.Next()).ToList();

                    tmp[currentCell.x, currentCell.y] = availableDirection[0];

                    switch (availableDirection[0])
                    {
                        case LabyrinthDirection.Up:
                            try
                            {
                                if (this.labyrinthDirections[currentCell.x, currentCell.y + 1] == LabyrinthDirection.None)
                                    cellStack.Push(new Vector2Int(currentCell.x, currentCell.y + 1));
                            }
                            catch { }
                            break;
                        case LabyrinthDirection.Down:
                            try
                            {
                                if (this.labyrinthDirections[currentCell.x, currentCell.y - 1] == LabyrinthDirection.None)
                                    cellStack.Push(new Vector2Int(currentCell.x, currentCell.y - 1));
                            }
                            catch { }
                            break;
                        case LabyrinthDirection.Left:
                            try
                            {
                                if(this.labyrinthDirections[currentCell.x - 1, currentCell.y] == LabyrinthDirection.None)
                                    cellStack.Push(new Vector2Int(currentCell.x - 1, currentCell.y));   
                            }
                            catch { }
                            break;
                        case LabyrinthDirection.Right:
                            try
                            {
                                if (this.labyrinthDirections[currentCell.x + 1, currentCell.y] == LabyrinthDirection.None)
                                    cellStack.Push(new Vector2Int(currentCell.x + 1, currentCell.y));
                            }
                            catch { }
                            break;
                    }

                    availableDirection.Clear();
                }

                // Update the labyrinth
                for (int i = 0; i < Width; i++)
                    for (int k = 0; k < Height; k++)
                        labyrinthDirections[i, k] = tmp[i, k];
            }
        }
    }
}