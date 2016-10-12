using System;
using System.Collections;

namespace DirectShow {
    public class Shot {
        private int startFrame;
        private int endFrame;
        private ArrayList keywords;

        public Shot(int startFrame) {
            this.startFrame = startFrame;
            keywords = new ArrayList();
        }

        public Shot(int startFrame, int endFrame) {
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            keywords = new ArrayList();
        }

        public void addKeyword(String keyword) {
            keywords.Add(keyword);
        }

        public void removeKeyword(String keyword) {
            keywords.Remove(keyword);
        }

        public void emptyKeywords() {
            keywords.Clear();
        }

        public ArrayList Keywords {
            get {
                return keywords;
            }
        }

        public int StartFrame {
            get {
                return startFrame;
            }
            set {
                this.startFrame = value;
            }
        }

        public int EndFrame {
            get {
                return endFrame;
            }
            set {
                this.endFrame = value;
            }
        }
    }
}
