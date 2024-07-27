using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BananagramsAI {
    class Grid {
        Dictionary<int, Line> rows = new Dictionary<int, Line>();
        Dictionary<int, Line> columns = new Dictionary<int, Line>();

        public Grid() { }

        public Grid(Grid grid) {
            this.rows = grid.rows.ToDictionary(pair => pair.Key, pair => new Line(pair.Value));
            this.columns = grid.columns.ToDictionary(pair => pair.Key, pair => new Line(pair.Value));
        }

        public int TopRowIndex {
            get {
                return rows.Keys.Min();
            }
        }

        public int BottomRowIndex {
            get {
                return rows.Keys.Max();
            }
        }

        public int LeftmostColumnIndex {
            get {
                return columns.Keys.Min();
            }
        }

        public int RightmostColumnIndex {
            get {
                return columns.Keys.Max();
            }
        }

        public bool IsEmpty() {
            return rows.Values.All(row => row.IsEmpty());
        }
        
        public void PlaceWordAt(string word, int x, int y, bool vertical) {
            int alongStart = (vertical ? y : x);
            int acrossStart = (vertical ? y : x);
            int acrossIndex = (vertical ? x : y);

            Dictionary<int, Line> alongAxis = (vertical ? columns : rows);
            int alongIndex = (vertical ? x : y);
            Line along = GetLineLazy(alongAxis, alongIndex);
            Dictionary<int, Line> across = (vertical ? rows : columns);
            
            for (int i = 0; i < word.Length; ++i) {
                along[alongStart + i] = word[i];

                if (!across.ContainsKey(acrossStart + i)) {
                    across[acrossStart + i] = new Line();
                }

                across[acrossStart + i][acrossIndex] = word[i];
            }
        }

        Line GetLineLazy(Dictionary<int, Line> axis, int index) {
            if (axis.TryGetValue(index, out Line line)) {
                return line;
            } else {
                axis[index] = new Line();
                return axis[index];
            }
        }

        public void Display() {
            if (IsEmpty()) return; 

            int leftmost = rows.Values.Select(line => line.BackIndex).Min();

            foreach (int index in rows.Keys.OrderBy(y => y)) {
                Line row = rows[index];

                for (int j = leftmost; j < row.BackIndex; ++j) {
                    Console.Write(' ');
                }

                for (int i = row.BackIndex; i <= row.FrontIndex; ++i) {
                    if (row.TryGetTile(i, out char tile)) {
                        Console.Write(tile);
                    } else {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();
            }
        }

        public bool IsEmptyLine(int lineIndex, bool column) {
            return (column ? columns : rows)[lineIndex].IsEmpty();
        }

        public IEnumerable<Placement> FindLinePlacementsFrom(int lineIndex, int start, int frontLeeway, bool vertical, Bank startingBank, Gaddag words) {
            Dictionary<int, Line> axis = vertical ? columns : rows;
            Line line = axis[lineIndex];

            Line[] neighbors = new Line[2];
            neighbors[0] = GetLineLazy(axis, lineIndex - 1);
            neighbors[1] = GetLineLazy(axis, lineIndex + 1);

            Stack<(Gaddag, int, Bank)> nodes = new Stack<(Gaddag, int, Bank)>();
            nodes.Push((words, start, startingBank));

            while (nodes.Count != 0) {
                (Gaddag node, int index, Bank bank) = nodes.Pop();

                if (line.IsEmptyAt(index)) {
                    if (!(neighbors[0].IsEmptyAt(index) && neighbors[1].IsEmptyAt(index))) continue;
                    for (char c = 'a'; c <= 'z'; ++c) {
                        if (!bank.HasLetter(c) || node[c] == null) {
                            continue;
                        }

                        Bank after = new Bank(bank);
                        after.TakeLetter(c);

                        nodes.Push((node[c], index - 1, after));
                    }

                    if (node['{'] != null) {
                        foreach (Gaddag word in node['{'].FindAllWords(bank)) {
                            if (word.word.Length - word.reversePoint >= frontLeeway) continue;
                            Bank after = new Bank(bank);
                            after.TakeWord(word.word.Substring(word.reversePoint+1));
                            if (Enumerable.SequenceEqual(startingBank.letters, after.letters)) continue;
                            yield return new Placement(word.word, lineIndex, start - word.reversePoint, vertical, after);
                        }
                    }
                } else {
                    char c = line[index];
                    if (node[c] != null) {
                        nodes.Push((node[c], index - 1, bank));
                    }
                }
            }
        }
        
        public IEnumerable<Placement> FindLinePlacements(int lineIndex, bool vertical, Bank bank, Gaddag words) {
            Dictionary<int, Line> axis = vertical ? columns : rows;
            Line line = axis[lineIndex];
            Line[] neighbors = new Line[2];
            neighbors[0] = GetLineLazy(axis, lineIndex - 1);
            neighbors[1] = GetLineLazy(axis, lineIndex + 1);

            int last = int.MaxValue;

            for (int i = line.FrontIndex; i >= line.BackIndex; --i) {
                if (!line.IsEmptyAt(i)) {
                    foreach (Placement placement in FindLinePlacementsFrom(lineIndex, i, last - i, vertical, bank, words)) {
                        yield return placement;
                    }

                    last = i - 1;
                } else if (!(neighbors[0].IsEmptyAt(i) && neighbors[1].IsEmptyAt(i))) {
                    last = i;
                }
            }
        }
    }
}
