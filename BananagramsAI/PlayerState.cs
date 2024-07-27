using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BananagramsAI {
    class PlayerState {
        public Bank Bank { get; private set;  }
        public Grid Grid { get; private set;  }

        public PlayerState(Bank bank, Grid grid) {
            this.Bank = bank;
            this.Grid = grid;
        }

        public PlayerState(PlayerState state) {
            this.Bank = new Bank(state.Bank);
            this.Grid = new Grid(state.Grid);
        }

        /*
        public void PlaceWord(string word, int x, int y, bool vertical) {
            Grid.PlaceWordAt(word, x, y, vertical);
            Bank.TakeWord(word);
        }*/

        public void PlaceWord(Placement placement) {
            Grid.PlaceWordAt(placement.word, placement.x, placement.y, placement.vertical);
            Bank = placement.bankAfter;
        }
        
        public IEnumerable<Placement> FindPlacements(Gaddag words) {
            if (Grid.IsEmpty()) {
                foreach (Gaddag word in words.FindAllWords(Bank)) {
                    Bank after = new Bank(Bank);
                    after.TakeWord(word.word);
                    yield return new Placement(word.word, 0, 0, false, after);
                    yield return new Placement(word.word, 0, 0, true, after);
                }

                yield break;
            }

            IEnumerable<Placement> FindPlacementsOriented(bool vertical) {
                int start = (vertical ? Grid.LeftmostColumnIndex : Grid.TopRowIndex);
                int end = (vertical ? Grid.RightmostColumnIndex : Grid.BottomRowIndex);

                for (int i = start; i <= end; ++i) {
                    if (Grid.IsEmptyLine(i, vertical)) continue;

                    foreach (Placement placement in Grid.FindLinePlacements(i, vertical, Bank, words)) {
                        yield return placement;
                    }
                }
            }
            
            foreach (Placement placement in FindPlacementsOriented(false)) yield return placement;
            foreach (Placement placement in FindPlacementsOriented(true)) yield return placement;
        }
    }
}
