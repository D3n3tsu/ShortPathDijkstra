using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DijkstraShortPathStandf
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<Vertix> listVertices = new List<Vertix>(200);
            listVertices.Add(new Vertix(0, null, null));
            string fileName = "";
            OpenFileDialog opener = new OpenFileDialog();
            if (opener.ShowDialog() == DialogResult.OK)
            {
                fileName = opener.FileName;
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                using(StreamReader sr = new StreamReader(fileName))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] line = sr.ReadLine().Split(new char[] { ' ', '\t' },StringSplitOptions.RemoveEmptyEntries);
                        int number = int.Parse(line[0]);
                        int[] edges = new int[line.Length - 1];
                        int[] distances = new int[line.Length - 1];
                        for (int i = 1; i < line.Length; i++)
                        {
                            string[] edgeInfo = line[i].Split(',');
                            edges[i - 1] = int.Parse(edgeInfo[0]);
                            distances[i - 1] = int.Parse(edgeInfo[1]);
                        }
                        listVertices.Add(new Vertix(number, edges, distances));
                    }
                }
            }
            string[] strQuestions = Console.ReadLine().Split(',');
            int[] questions = new int[strQuestions.Length];
            for (int i = 0; i < strQuestions.Length; i++)
            {
                questions[i] = int.Parse(strQuestions[i]);
            }

            if (listVertices.Count > 0)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Dictionary<int, int> ShortestPathes = GetShortestPaths(listVertices.ToArray());
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                
                Dictionary<int, int> ShortestPathes2 = GetShortestPaths2(listVertices.ToArray());
                

                int[] answers = new int[questions.Length];
                int[] answers2 = new int[questions.Length];
                for (int i = 0; i < answers.Length; i++)
                {
                    answers[i] = ShortestPathes[questions[i]];
                    answers2[i] = ShortestPathes2[questions[i]];
                    for (int h = 1; h < ShortestPathes.Count - 1; h++)
                    {
                        if (ShortestPathes[h] != ShortestPathes2[h])
                        { throw new Exception("Bad news"); }
                    }
                }
                StringBuilder strToCopy = new StringBuilder();
                for (int i = 0; i < answers.Length; i++)
                {
                    strToCopy.Append(answers[i]);
                    if (i < answers.Length - 1)
                    {
                        strToCopy.Append(",");
                    }
                }
                Clipboard.SetText(strToCopy.ToString());
            }
            
        }

        static Dictionary<int, int> GetShortestPaths2(Vertix[] vertices)
        {
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            Heap heap = new Heap(vertices);
            Dictionary<int, int> results = new Dictionary<int, int>();
            bool doWhile = true;
            while (doWhile ){
                HeapNode temp = heap.Deheapify();
                if (temp != null)
                {
                    results.Add(temp.MyVertix.Number, temp.ShortestDistance);
                }
                else doWhile = false;
            }

            watch2.Stop();
            var elapsedMs2 = watch2.ElapsedMilliseconds;
            return results;
        }

        static Dictionary<int, int> GetShortestPaths(Vertix[] vertices)
        {
            Vertix[] extractedVertices = new Vertix[vertices.Length];
            int[] pathes = new int[vertices.Length];
            extractedVertices[1] = vertices[1];
            vertices[1].Number = 0;
            pathes[1] = 0;
            int count = vertices.Length - 1;

            while (count > 0)
            {
                int shortestPath = int.MaxValue;
                int vertixToExtract = 0;
                for (int i = 1; i < extractedVertices.Length; i++)
                {
                    if (extractedVertices[i].Number != 0)
                    {
                        for (int j = 0; j < extractedVertices[i].Edges.Length; j++)
                        {
                            int edge = extractedVertices[i].Edges[j];
                            if (vertices[edge].Number != 0)
                            {
                                int tempPath = GetShortestPath(vertices[edge], extractedVertices, pathes);
                                if (tempPath < shortestPath)
                                {
                                    shortestPath = tempPath;
                                    vertixToExtract = edge;
                                }
                            }
                        }
                    }
                }
                extractedVertices[vertixToExtract] = vertices[vertixToExtract];
                pathes[vertixToExtract] = shortestPath;
                vertices[vertixToExtract].Number = 0;

                count--;
            }

            Dictionary<int, int> result = new Dictionary<int, int>(vertices.Length);
            for (int i = 0; i < extractedVertices.Length; i++)
            {
                result.Add(i, pathes[i]);
            }
            return result;
        }

        static int GetShortestPath(Vertix vertix, Vertix[] extractedVertices, int[] pathes)
        {
            int shortestPath = int.MaxValue;
            for (int i = 1; i < extractedVertices.Length; i++)
            {
                int temp = 0;
                if (extractedVertices[i].Number!=0)
                {
                    temp = extractedVertices[i].GetDistance(vertix.Number);
                    if (temp != -1)
                    {
                        int newPath = pathes[i] + temp;
                        if (newPath <= shortestPath)
                        {
                            shortestPath = newPath;
                        }
                    }
                }
            }
            
            return shortestPath;
        }
    }

    struct Vertix
    {
        public int Number;
        public int[] Edges;
        public int[] Distances;
        public Vertix(int number, int[] edges, int[] distances)
        {
            Number = number;
            Edges = edges;
            Distances = distances;
        }
        
        public int GetDistance(int vertix)
        {
            int idx = Array.IndexOf(Edges, vertix);
            if (idx != -1) { 
            return Distances[idx];
            }
            else
            {
                return -1;
            }
        }
    }
}
