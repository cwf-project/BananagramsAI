using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananagramsAI {
    class Gaddag {
        public bool isEndOfWord;
        public Gaddag[] children;
        public string word;
        public int reversePoint;

        public Gaddag(bool isEndOfWord) {
            children = new Gaddag[27];
            this.isEndOfWord = isEndOfWord;
        }

        public Gaddag this[char c] {
            get {
                return children[c - 'a'];
            }
        }

        public IEnumerable<Gaddag> FindAllWords() {
            if (isEndOfWord)
                yield return this;

            for (int i = 0; i < 27; ++i) {
                if (children[i] == null) continue;
                
                foreach (Gaddag word in children[i].FindAllWords()) {
                    yield return word;
                }
            }
        }
        
        public IEnumerable<Gaddag> FindAllWords(Bank bank) {
            if (isEndOfWord)
                yield return this;

            for (char c = 'a'; c <= 'z'; ++c) {
                if (!bank.HasLetter(c) || this[c] == null) continue;

                Bank after = new Bank(bank);
                after.TakeLetter(c);
                foreach (Gaddag word in this[c].FindAllWords(after)) {
                    yield return word;
                }
            }

            if (this['{'] != null) {
                foreach (Gaddag word in this['{'].FindAllWords(bank)) {
                    yield return word;
                }
            }
        }

        void InsertSplitAt(string word, int split) {
            Gaddag iter = this;

            void InsertLetter(char letter) {
                int index = letter - 'a';

                if (iter.children[index] == null) {
                    iter.children[index] = new Gaddag(false);
                }

                iter = iter.children[index];
            }

            for (int i = 0; i <= split; ++i) {
                int index = split - i;
                InsertLetter(word[index]);
            }
        
            InsertLetter('{'); // Represents a line-reversal. Equal to 'z' + 1
            
            for (int i = split + 1; i < word.Length; ++i) {
                InsertLetter(word[i]);
            }

            iter.isEndOfWord = true;
            iter.word = word;
            iter.reversePoint = split;
        }

        public void InsertWord(string word) {
            for (int i = 0; i < word.Length; ++i) {
                InsertSplitAt(word, i);
            }
        }
    }
}
