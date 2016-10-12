// Bart Middag

package pagerank;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.util.Arrays;
import java.util.TreeSet;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * PageRank application
 * @author Bart Middag
 */
public class PageRank {

    public static void main(String[] args) {
        if (args.length > 1) {
            if(args[0].equals("make")) {
                if(args.length == 2) {
                    make(args[1]);
                    System.exit(0);
                }
            } else if(args[0].equals("query") || args[0].equals("quary")) {
                search(Arrays.copyOfRange(args, 1, args.length));
                System.exit(0);
            }
        }
        
        // This section can only be reached if you did something wrong.
        System.out.println("Usage:");
        System.out.println("* Command 1: pagerank make url");
        System.out.println("  Crawls the web starting from url and exports a file with keyword information.");
        System.out.println("* Command 2: pagerank query term1 term2 term3 ...");
        System.out.println("  Searches the keyword file created with Command 1 for a page containing all terms.");
    }
    
    /**
     * Crawl an URL and create a new auxiliary file
     * @param url The URL to crawl
     */
    public static void make(String url) {
        PageProcessor proc = new PageProcessor();
        try {
            // Possible improvement: check robots.txt first.
            // But the test server doesn't contain that.
            System.out.print("Fetching URLs...");
            Page start = proc.process(new URL(url));
            System.out.println(" complete!");
            System.out.print("Computing PageRank...");
            proc.rank(start);
            System.out.println(" complete!");
            System.out.print("Exporting index...");
            proc.exportFile("crawler_index.ser");
            System.out.println(" complete!");
            proc.printStatistics();
        } catch (UnsupportedEncodingException ex) {
            Logger.getLogger(PageRank.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IOException ex) {
            Logger.getLogger(PageRank.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    /**
     * Use an auxiliary file to search for terms
     * @param terms The keywords to search for
     */
    public static void search(String[] terms) {
        PageProcessor proc = new PageProcessor();
        try {
            System.out.print("Importing index...");
            proc.importFile("crawler_index.ser");
            System.out.println(" complete!");
            System.out.print("Searching...");
            TreeSet<Page> pages = proc.search(terms);
            System.out.println(" complete!");
            proc.printSearchResults(terms, pages);
        } catch (FileNotFoundException ex) {
            System.out.println("Error: Index file not found.");
        } catch (IOException | ClassNotFoundException ex) {
            Logger.getLogger(PageRank.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
