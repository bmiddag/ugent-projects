package suffixtree;

import java.util.*;

/**
 * Abstracte SuffixTree
 * Zie verslag voor commentaar!
 * @author Bart Middag
 */
public abstract class AbstractSuffixTree implements IntervalSearcher {
    protected AbstractTreeNode root;
    protected List<Short> values;
    protected List<Short> query;
    
    @Override
    public void construct(List<Short> sequence) {
        //Maak een kopie van de originele lijst en voeg de sentinel toe aan deze kopie.
        values = new ArrayList<Short>(sequence);
        values.add(SENTINEL);
    }
    
    /**
     * search for the pattern query in the reference sequence.
     *
     * @param query: list of shorts
     * @return: true if query is an exact match for at least one
     * sublist/subinterval of the reference sequence. This method is similar to
     * the contains method of a string, but works with lists of shorts instead.
     * @return true for an empty list.
     */
    @Override
    public boolean contains(List<Short> query) {
        this.query = query;
        int i;
        int begin = 0;
        int end = query.size();
        if (query == null || (begin == end)) {
            return true;
        }
        AbstractTreeNode foundNode = quickGetChild(root,query.get(begin));
        while (foundNode != null) {
            int curBegin = foundNode.getBegin();
            int curSize = foundNode.getEnd() - curBegin;
            int minSize = Math.min(curSize, end - begin);
            for (i = 1; i < minSize; i++) {
                if (!(query.get(begin + i).equals(values.get(curBegin + i)))) {
                    return false;
                }
            }
            begin += i;
            if (query == null || (begin == end)) {
                return true;
            }
            foundNode = quickGetChild(foundNode,query.get(begin));
        }
        return false;
    }
    
    /**
     * search for the pattern query in the reference sequence.
     *
     * @param query: list of shorts
     * @return: the number of times query occurs as a sublist/subinterval of the
     * reference sequence. This method is similar to the locate method below,
     * but only returns the number of times the pattern is found.
     * @return for an empty list: the number of suffixes in the reference
     * sequence.
     */
    @Override
    public int count(List<Short> query) {
        this.query = query;
        if (query.isEmpty()) {
            return values.size() - 1;
        }
        return count(root, 0, query.size());
    }

    public int count(AbstractTreeNode node, int begin, int end) {
        int i;
        if (query == null || begin == end) {
            int count = 0;

            if (node.isLeaf()) {
                count++;
            } else {
                for (Iterator<AbstractTreeNode> it = node.getChildren().iterator(); it.hasNext();) {
                    AbstractTreeNode cursor = it.next();
                    count += count(cursor, begin, end);
                }
            }
            return count;
        }

        AbstractTreeNode foundNode = quickGetChild(node,query.get(begin));
        if (foundNode != null) {
            int curBegin = foundNode.getBegin();
            int curSize = foundNode.getEnd() - curBegin;
            int minSize = Math.min(curSize, end - begin);
            for (i = 1; i < minSize; i++) {
                if (!(query.get(begin + i).equals(values.get(curBegin + i)))) {
                    return 0;
                }
            }
            return count(foundNode, begin + i, end);
        }
        return 0;
    }
    
    /**
     * Find all occurrences of the pattern query in the reference sequence.
     * Example: (i) suffix tree for sequence 1,2,50,100,25,25,25,50,100,25,25
     * (ii) query 25,25 (iii) result {4,5,9}
     *
     * @param query the query sequence (list of shorts)
     * @return a set of locations (index positions) in the sequence where the
     * query occurs as a sublist/subinterval of the reference sequence. This
     * method is similar to iteratively using the indexOf method on strings,
     * until all positions are found, but uses lists of shorts instead.
     * @return for an empty list: the indexes of all suffixes in the reference
     * sequence.
     */
    @Override
    public Set<Integer> locate(List<Short> query) {
        this.query = query;
        Set<Integer> output = new HashSet<Integer>();
        if (query.isEmpty()) {
            int size = values.size() - 1;
            for (int i = 0; i < size; i++) {
                output.add(new Integer(i));
            }
        } else {
            locate(root, 0, query.size(), output);
        }
        return output;
    }

    public void locate(AbstractTreeNode node, int begin, int end, Set<Integer> output) {
        int i;

        if (query == null || begin == end) {
            if (node.isLeaf()) {
                output.add(node.getIndex());
            } else {
                for (Iterator<AbstractTreeNode> it = node.getChildren().iterator(); it.hasNext();) {
                    AbstractTreeNode cursor = it.next();
                    locate(cursor, begin, end, output);
                }
            }
        } else {
            AbstractTreeNode foundNode = quickGetChild(node,query.get(begin));
            if (foundNode != null) {
                int curBegin = foundNode.getBegin();
                int curSize = foundNode.getEnd() - curBegin;
                int minSize = Math.min(curSize, end - begin);
                for (i = 1; i < minSize; i++) {
                    if (!(query.get(begin + i).equals(values.get(curBegin + i)))) {
                        return;
                    }
                }
                locate(foundNode, begin + i, end, output);
            }
        }
    }
    
    /**
     * return the root of the suffix tree (only used for suffix tree methods,
     * other alternative methods return null).
     *
     * @return a TreeNode (object from a class implementing TreeNode)
     * representing the root of the suffix tree. If the suffix tree has not been
     * constructed, this method should return null.
     */
    @Override
    public TreeNode getRoot() {
        return root;
    }
    
    
    public void printDetails(AbstractTreeNode node, int nodeLevel) {
        for (int i = 0; i < nodeLevel; i++) {
            System.out.print("    ");
        }
        System.out.print("Values:");
        for (int i = node.getBegin(); i < node.getEnd(); i++) {
            System.out.print(" " + values.get(i));
        }
        System.out.print(" - " + node.getIndex());
        System.out.println();
        if (node.getChildren() != null) {
            for (AbstractTreeNode child : node.getChildren()) {
                printDetails(child, nodeLevel + 1);
            }
        }
    }
    
    public void printDetails() {
        printDetails(root, 0);
    }
    
    public abstract AbstractTreeNode quickGetChild(AbstractTreeNode parent, Short key);
}
