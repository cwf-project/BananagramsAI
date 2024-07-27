using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace BananagramsAI {
    class SearchNode {
        public PlayerState State { get; }
        public int MovesToReach { get; set; }
        //public SimplePriorityQueue<SearchNode> Children { get; private set;  } = new SimplePriorityQueue<SearchNode>();

        public SearchNode(PlayerState state, int movesToReach) {
            this.State = state;
            this.MovesToReach = movesToReach;
        }

        public IEnumerable<SearchNode> FindChildren(Gaddag words) {
            var placements = State.FindPlacements(words);
            foreach (Placement placement in placements) {
                PlayerState next = new PlayerState(State);
                next.PlaceWord(placement);
                yield return new SearchNode(next, MovesToReach + 1);
            }
        }
    }
}
