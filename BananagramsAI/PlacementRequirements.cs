using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BananagramsAI {
    struct PlacementRequirements {
        public int backLeeway;
        public Regex regex;

        public PlacementRequirements(Regex regex, int backLeeway) {
            this.regex = regex; 
            this.backLeeway = backLeeway;
        }
    }
}
