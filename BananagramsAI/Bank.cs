using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananagramsAI {
    class Bank {
        readonly public int[] letters = new int[26];
        readonly static float ExactWeight = 0.5f;

        public Bank() {
            //Letters = new int[26];
        }

        public Bank(int[] letters) {
            letters.CopyTo(this.letters, 0);
        }

        public Bank(Bank bank) {
            // This drastically improves performance when calculating Bank value.
            Buffer.BlockCopy(bank.letters, 0, letters, 0, 26 * sizeof(int));
            //bank.Letters.CopyTo(Letters, 0);
        }
        
        public int this[char letter] {
            get {
                return letters[(int)letter - (int)'a'];
            }

            set {
                letters[(int)letter - (int)'a'] = value;
            }
        }

        public int Size {
            get {
                return letters.Sum();
            }
        }

        public bool HasLetter(char letter) {
            return this[letter] > 0;
        }

        public void TakeLetter(char letter) {
            if (this[letter] == 0) {
                throw new Exception();
            } else {
                this[letter] -= 1;
            }
        }

        public bool TryTakeLetter(char letter) {
            if (this[letter] <= 0) {
                return false;
            } else {
                this[letter] -= 1;
                return true;
            }
        }

        public bool IsAvailableWord(string word) {
            Bank after = new Bank(this);
            foreach (char c in word) {
                if (!after.TryTakeLetter(c)) {
                    return false;
                }
            }

            return true;
        }

        public void TakeWord(string word) {
            foreach (char c in word) {
                TakeLetter(c);
            }
        }

        public bool TryTakeWord(string word) {
            foreach (char letter in word) {
                if (this[letter] <= 0) {
                    return false;
                } else {
                    this[letter] -= 1;
                }
            }

            return true;
        }

        public float CalculateValue(List<string> words) {
            int exacts = 0;
            int exactCandidates = 0;
            int inexacts = 0;
            int inexactCandidates = 0;
            // The Size getter is actually quite expensive so we cache the size here.
            int size = Size;
            
            // I would much prefer a foreach here, but for actually gives better performance.
            for (int i = 0; i < words.Count; ++i) {
                string word = words[i];
                if (word.Length > size + 1) {
                    continue;
                } else if (word.Length == size + 1) {
                    exactCandidates += 1;
                } else {
                    inexactCandidates += 1;
                }

                Bank bank = new Bank(this);

                bool oneExtra = false;
                foreach (char c in word) {
                    if (!bank.HasLetter(c)) {
                        if (oneExtra) {
                            // This word doesn't match, so skip it.
                            oneExtra = false;
                            break;
                        } else {
                            oneExtra = true;
                        }
                    } else {
                        bank.TakeLetter(c);
                    }
                }
                
                if (oneExtra) {
                    if (word.Length == size + 1) {
                        exacts += 1;
                    }  else {
                        inexacts += 1;
                    }
                }
            }

            float exactFreq = exactCandidates == 0 ? 0 : (float)exacts / exactCandidates;
            float inexactFreq = inexactCandidates == 0 ? 0 : (float)inexacts / inexactCandidates;
            float value = exactFreq * ExactWeight + inexactFreq * (1 - ExactWeight);
            return value;
        }
    }
}
