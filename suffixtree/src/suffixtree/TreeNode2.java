package suffixtree;

import java.util.TreeSet;

/**
 * TreeNode met TreeSet
 * @author Bart Middag
 */
public final class TreeNode2 extends AbstractTreeNode implements Comparable {

    private TreeSet<AbstractTreeNode> children;
    private short value;

    public TreeNode2(int begin, int end, int index, short value) {
        super(begin,end,index);
        this.value = value;
    }

    @Override
    public boolean isLeaf() {
        try {
            return (children.isEmpty());
        } catch(NullPointerException ex) {
            return true;
        }
    }

    public short getValue() {
        return value;
    }

    public void setValue(short value) {
        this.value = value;
    }

    @Override
    public TreeSet<AbstractTreeNode> getChildren() {
        return children;
    }

    public void quickAddChild(TreeNode2 child) {
        try {
            children.add(child);
        } catch(NullPointerException ex) {
            //We maken pas de TreeSet aan als we een kind toevoegen, om geheugen te sparen
            children = new TreeSet<AbstractTreeNode>();
            children.add(child);
        }
    }
    
    public void quickRemoveChild(TreeNode2 child) {
        try {
            children.remove(child);
        } catch(NullPointerException ex) {}
    }

    @Override
    public int hashCode() {
        return this.value;
    }

    @Override
    public boolean equals(Object obj) {
        //Vergelijk object op basis van shortwaarde.
        //Handig voor de TreeSet!
        if (obj == null) {
            return false;
        }
        if (getClass() != obj.getClass()) {
            return false;
        }
        if (this.value != ((TreeNode2)obj).getValue()) {
            return false;
        }
        return true;
    }

    @Override
    public int compareTo(Object o) {
        if (o == null) {
            return -1;
        }
        if (getClass() != o.getClass()) {
            return -1;
        }
        final short othervalue = ((TreeNode2)o).getValue();
        if (this.value < othervalue) {
            return -1;
        }
        if (this.value > othervalue) {
            return 1;
        }
        return 0;
    }
}
