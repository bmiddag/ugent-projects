// Bart Middag

package pagerank;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.URL;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Locale;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;
import org.apache.commons.math3.fraction.BigFraction;
import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;

/**
 * Page Processor class
 * - Create Page objects from web documents, compute PageRank, export and import -
 * @author Bart Middag
 */
public class PageProcessor {
    private Set<Page> visitedPages;
    private Map<String, Set<Page>> index;

    public PageProcessor() {
        visitedPages = new HashSet<>();
        index = new HashMap<>();
    }
    
    /**
     * Return page object for URL if it has already been visited
     * @param url   URL to look for locally
     * @return      Local Page object for URL
     */
    private Page getLocalPage(URL url) {
        for(Page page: visitedPages) {
            if(page.getURL().sameFile(url)) return page;
        }
        return null;
    }

    /**
     * Crawls the web and creates Page objects from web documents
     * @param url   URL to start crawling from
     * @return      Page object for the processed page
     * @throws IOException 
     */
    public Page process(URL url) throws IOException {
        // Check if page has already been visited
        Page page = getLocalPage(url);
        if (page == null) {
            try {
                // Allow time between server requests
                Thread.sleep(50);
            } catch(InterruptedException e) {} // No problem if thread is interrupted
            // Connect to page
            Document doc = Jsoup.connect(url.toString()).get();
            
            // Create page
            page = new Page(url);
            visitedPages.add(page);
            
            // Add keywords
            String[] text = doc.body().text().trim().split("\\s+"); // Remove all whitespace
            for (String keyword : text) {
                String newKey = keyword.toLowerCase(Locale.ENGLISH);
                page.addKeyword(newKey);
                if(!index.containsKey(newKey)) index.put(newKey, new HashSet<Page>());
                index.get(newKey).add(page);
            }
            
            // Get all links
            Elements links = doc.select("a[href]");
            for (Element link : links) {
                page.addLink(process(new URL(link.attr("abs:href"))));
            }
        }
        
        return page;
    }
    
    /**
     * Start PageRank process with default parameters
     * @param startPage The start page for the ranking process
     */
    public void rank(Page startPage) {
        rank(startPage, BigFraction.ONE_HALF, 0.0001);
    }
    
    /**
     * Start PageRank process
     * @param startPage The start page for the ranking process
     * @param alpha     The teleportation factor: probability by which one would visit a non-linked page
     * @param threshold The threshold for the difference between PageRanks in two different iterations
     */
    public void rank(Page startPage, BigFraction alpha, double threshold) {
        int numPages = visitedPages.size();
        boolean converged = false;
        BigFraction oneMinusAlpha = BigFraction.ONE.subtract(alpha);
        startPage.setPageRank(new BigFraction(1));
        while(!converged) {
            for(Page page : visitedPages) {
                page.nextStep();
            }
            for(Page page : visitedPages) {
                BigFraction previousRank = page.getPreviousRank();
                if(previousRank.compareTo(BigFraction.ZERO) == 0) continue;
                Set<Page> links = new HashSet<>(page.getLinks()); // Don't count duplicate links twice
                if(links.isEmpty()) {
                    for(Page nextPage : visitedPages) {
                        nextPage.addPageRank(previousRank.divide(numPages));
                    }
                } else {
                    for(Page link : links) {
                        link.addPageRank(previousRank.multiply(oneMinusAlpha).divide(links.size()));
                    }
                    for(Page nextPage : visitedPages) {
                        nextPage.addPageRank(previousRank.multiply(alpha).divide(numPages));
                    }
                }
            }
            converged = true;
            for(Page page : visitedPages) {
                // Calculate difference between pageranks in two consecutive iterations.
                double difference = Math.abs(page.getPageRank().doubleValue() - page.getPreviousRank().doubleValue());
                if(difference > threshold) {
                    converged = false;
                    break;
                }
            }
        }
    }
    
    /**
     * Search for terms in the visited pages
     * @param terms Array of terms a page has to contain
     * @return      A set of pages that contain all terms, sorted by their PageRank
     */
    public TreeSet<Page> search(String[] terms) {
        TreeSet<Page> results = new TreeSet<>(visitedPages);
        for(String term : terms) {
            String lowerTerm = term.toLowerCase(Locale.ENGLISH);
            if(!index.containsKey(lowerTerm)) return new TreeSet<>();
            results.retainAll(index.get(lowerTerm));
            if(results.isEmpty()) return results;
        }
        return results;
    }
    
    /**
     * Export page database to file
     * @param filename  File to export to
     * @throws FileNotFoundException
     * @throws IOException 
     */
    public void exportFile(String filename) throws FileNotFoundException, IOException {
        try (FileOutputStream fileOut = new FileOutputStream(filename);
                ObjectOutputStream out = new ObjectOutputStream(fileOut)) {
            out.writeObject(visitedPages);
            out.writeObject(index);
        }
    }
    
    /**
     * Import page database from file
     * @param filename  File to import from
     * @throws FileNotFoundException
     * @throws IOException
     * @throws ClassNotFoundException 
     */
    public void importFile(String filename) throws FileNotFoundException, IOException, ClassNotFoundException {
        try (FileInputStream fileIn = new FileInputStream(filename);
                ObjectInputStream in = new ObjectInputStream(fileIn)) {
            visitedPages = (HashSet<Page>)in.readObject();
            index = (HashMap<String, Set<Page>>)in.readObject();
        }
    }
    
    /**
     * Print statistics after crawling the web.
     * Should be called after process(URL url);
     */
    public void printStatistics() {
        int pages = visitedPages.size();
        int terms = index.keySet().size();
        double links = 0;
        double incomingLinks = 0;
        for(Page page: visitedPages) {
            links += page.getLinks().size(); // Count duplicate links twice
            incomingLinks += page.getIncomingLinks();
        }
        links /= pages;
        incomingLinks /= pages;
        
        /* Note: incoming and outgoing links will be the same number for the test site,
           as all links on the test site refer to other pages on the test site. However,
           in real-world cases, where we stop the crawler after a while, they might not be.
           That's why they are computed differently here. */
        
        System.out.println("Statistics:");
        System.out.println("* Number of pages crawled: " + pages);
        System.out.println("* Number of distinct terms found: " + terms);
        System.out.println("* Mean number of incoming links per page: " + incomingLinks);
        System.out.println("* Mean number of outgoing links per page: " + links);
    }
    
    /**
     * Print search results after a search operation.
     * Should be called after search(String[] terms).
     * @param terms Terms you have searched for
     * @param pages Sorted set of pages the search operation has returned
     */
    public void printSearchResults(String[] terms, TreeSet<Page> pages) {
        System.out.println("Results:");
        System.out.print("* Terms: ");
        for (int i = 0; i < terms.length; i++) {
            System.out.print(terms[i]);
            if (i != terms.length - 1) {
                System.out.print(", ");
            } else System.out.println("");
        }
        
        System.out.println("* Found " + pages.size() + " result" + (pages.size() != 1 ? "s." : "."));
        if (pages.size() > 0) {
            System.out.println("  Displaying top " + Math.min(pages.size(), 10) + " results:");
            
            // We want to to see the top 10 documents with descending PageRanks.
            Iterator<Page> pageIt = pages.descendingIterator();

            for (int i = 0; i < Math.min(pages.size(), 10); i++) {
                Page page = pageIt.next();
                System.out.println("  " + (i >= 9 ? "" : " ") + (i + 1) + " - " + page.getURL() + " - PageRank: " + page.getPageRank().doubleValue());
            }
        }
    }
}
