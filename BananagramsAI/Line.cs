using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananagramsAI {
    class Line {
        List<char> behind = new List<char>();
        List<char> ahead = new List<char>();

        public Line() { }

        public Line(Line line) {
            behind = new List<char>(line.behind);
            ahead = new List<char>(line.ahead);
        }

        public int FrontIndex {
            get {
                return ahead.Count - 1;
            }
        }

        public int BackIndex {
            get {
                return -behind.Count;
            }
        }

        public char this[int index] {
            get {
                if (TryGetTile(index, out char tile)) {
                    return tile;
                } else {
                    throw new IndexOutOfRangeException();
                }
            }

            set {
                List<char> line;
                if (index >= 0) {
                    line = ahead;
                } else {
                    line = behind;
                    index = -index - 1;
                }

                if (index >= line.Count) {
                    line.AddRange(Enumerable.Repeat('\0', index - line.Count));
                    line.Add(value);
                } else {
                    line[index] = value;
                }
            }
        }

        public bool IsEmpty() {
            return (behind.Count == 0) && (ahead.Count == 0);
        }

        public bool TryGetTile(int index, out char tile) { 
            List<char> line;
            if (index >= 0) {
                line = ahead;
            } else {
                line = behind;
                index = -index - 1;
            }

            if (index >= line.Count || line[index] == '\0') {
                tile = '\0';
                return false;
            } else {
                tile = line[index];
                return true;
            }
        }
        
        public bool IsEmptyAt(int index) {
            return !TryGetTile(index, out char tile);
        }
    }
}
