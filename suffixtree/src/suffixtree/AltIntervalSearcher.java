package suffixtree;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

//////////////////////////////////////
// Do not alter this file in any way!
// This file was altered by Michael Vyverman on 19 Oktober 2012
// Changes: the intervalSearcher should now return the correct answer for empty queries.
//////////////////////////////////////


/**
 * Class representing an IntervalSearcher that does not require an 
 * index for the (reference) sequence.
 * 
 * The implementation of IntervalSearcher uses a simple algorithm, matching
 * the query until a mismatch is found, for every position in the reference
 * sequence.
 * 
 */
public class AltIntervalSearcher implements IntervalSearcher{
    
    //the reference string of shorts.
    protected List<Short> sequence;
    
    /**
     * Dummy constructor
     * @param sequence 
     */
    @Override
    public void construct(List<Short> sequence){
        this.sequence = sequence;
    }
    
    /**
     * Implementation of contains function of IntervalSearcher
     * @param query: the pattern to be searched
     * @return @see IntervalSearcher/contains
     */
    @Override
    public boolean contains(List<Short> query){
        return indexOf(query, 0) > -1;
    }

    /**
     * Implementation of count function of IntervalSearcher
     * @param query: the pattern to be searched
     * @return @see IntervalSearcher/contains
     */
    @Override
    public int count(List<Short> query){
        int count = 0;
        int offset = indexOf(query, 0);
        while(offset > -1){
            count++;
            offset = indexOf(query, offset+1);
        }
        return count;
    }

    /**
     * Implementation of locate function of IntervalSearcher
     * @param query: the pattern to be searched
     * @return @see IntervalSearcher/contains
     */
    @Override
    public Set<Integer> locate(List<Short> query){
        Set<Integer> result = new HashSet<Integer>();
        int offset = indexOf(query, 0);
        while(offset > -1){
            result.add(offset);
            offset = indexOf(query, offset+1);
        }
        return result;
    }
    
    /**
     * find the next occurrence of the given pattern, starting from a given position
     * in the (reference) sequence.
     * @param query: the pattern to be found.
     * @param offset: the starting index (0-based). The first occurrence of 
     * query will be searched in sequence[offset..|sequence|-1]
     * @return the first index Idx >= offset, where query can be found. If query
     * cannot be found, return -1.
     */
    public int indexOf(List<Short> query, int offset){
        if(offset < sequence.size()){
            int index = offset;
            boolean found = false;
            while(!found && index+query.size() <= sequence.size() ){
                int qIndex = 0;
                while(qIndex < query.size() && query.get(qIndex).equals(sequence.get(index+qIndex))){
                    qIndex++;
                }
                if(qIndex == query.size())
                    found = true;
                else
                    index++;
            }
            if(!found)
                index = -1;
            return index;
        }
        else
            return -1;
    }
    
    /**
     * Method used for suffix tree method. Return null for this alternative method.
     */
    @Override
    public TreeNode getRoot(){
        return null;
    }
}