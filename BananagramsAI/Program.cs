using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

using Priority_Queue; // C'mon dude, don't have underscores in namespaces.

namespace BananagramsAI {
    class Program {
        public static void Main(string[] args) {
            var query = from line in File.ReadLines(@"C:\Users\undoall\source\repos\BananagramsAI\BananagramsAI\shortwords.txt")
                        where line.All(c => "abcdefghijklmnopqrstuvwxyz".Contains(c))
                        select line;
            List<string> wordList = query.ToList();

            Gaddag words = new Gaddag(false);

            foreach (string word in wordList) {
                words.InsertWord(word);
            }

            float Heuristic(SearchNode node) {
                return (float)node.State.Bank.Size;
                //return (1 - node.State.Bank.CalculateValue(wordList)) * node.State.Bank.Size;
            }

            List<char> pile = new List<char>();
            for (int i = 0; i < 2; ++i) {
                pile.AddRange("J, K, Q, X, Z".ToLower().Split(',').Select(w => w.Trim()[0]));
            }

            for (int i = 0; i < 3; ++i) {
                pile.AddRange("B, C, F, H, M, P, V, W, Y".ToLower().Split(',').Select(w => w.Trim()[0]));
            }

            pile.AddRange(Enumerable.Repeat('g', 4));
            pile.AddRange(Enumerable.Repeat('l', 5));

            for (int i = 0; i < 6; ++i) {
                pile.AddRange("D, S, U".ToLower().Split(',').Select(w => w.Trim()[0]));
            }

            pile.AddRange(Enumerable.Repeat('n', 8));
            pile.AddRange(Enumerable.Repeat('t', 9));
            pile.AddRange(Enumerable.Repeat('r', 9));
            pile.AddRange(Enumerable.Repeat('o', 11));
            pile.AddRange(Enumerable.Repeat('i', 12));
            pile.AddRange(Enumerable.Repeat('a', 13));
            pile.AddRange(Enumerable.Repeat('e', 18));

            int num_taking = 40;

            Random rng = new Random();

            int[] temp = new int[26];
            for (int i = 0; i < num_taking; ++i) {
                int index = rng.Next(0, pile.Count);
                temp[pile[index] - 'a'] += 1;
                pile.RemoveAt(index);
            }

            Bank bank = new Bank(temp);
            Grid grid = new Grid();

            SimplePriorityQueue<SearchNode> queue = new SimplePriorityQueue<SearchNode>();

            SearchNode root = new SearchNode(new PlayerState(bank, grid), 0);
            queue.Enqueue(root, num_taking);
            int count = 0;

            PlayerState BestFirstSearch(PlayerState start) {
                for (; ; ) {
                    SearchNode node;
                    while (!queue.TryDequeue(out node)) ;
                    //SearchNode node = queue.Dequeue();
                    Interlocked.Increment(ref count);

                    //Console.Clear();
                    //node.State.Grid.Display();
                    if (count % 1000 == 0) {
                        Console.WriteLine("({0}) got node with bank size {1}", count, node.State.Bank.Size);
                    }

                    foreach (SearchNode child in node.FindChildren(words)) {
                        if (child.State.Bank.Size == 0) {
                            Console.WriteLine("Solution found!");
                            child.State.Grid.Display();
                            Console.WriteLine("Total number of nodes examined: {0}", count);
                            Environment.Exit(Environment.ExitCode);
                            return child.State;
                        }

                        queue.Enqueue(child, Heuristic(child));
                    }
                }
            }
            
            Console.Write("Bank: ");
            for (char c = 'a'; c <= 'z'; ++c) {
                for (int i = 0; i < bank.letters[c - 'a']; ++i) {
                    Console.Write(c);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Searching for solution...");

            int num_threads = 1;
            Task[] tasks = new Task[num_threads];
            for (int i = 0; i < num_threads; ++i) {
                tasks[i] = Task.Run(() => BestFirstSearch(new PlayerState(bank, grid)));
            }

            tasks[0].Wait();

            /*
            PlayerState state = new PlayerState(bank, grid);
            List<Placement> placements;
            for (int i = 0; i < 20; ++i) {
                Console.WriteLine();
                placements = state.FindPlacements(wordsBlock);
                Console.Write("From " + placements.Count + " possible moves, placing: ");
                Placement placement = placements[rng.Next(0, placements.Count)];


                Console.WriteLine(placement.word);
                state.PlaceWord(placement);
                state.Grid.Display();
                for (char c = 'a'; c <= 'z'; ++c) {
                    if (state.Bank.HasLetter(c)) {
                        Console.Write(c);
                    }
                }
                Console.WriteLine();
            }
            */
        }
    }
}
