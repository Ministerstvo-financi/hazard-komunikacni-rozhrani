package cz.com.spcss.utilities;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.Validate;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.*;

public class CryptoUtilities {
    public static Map<String, String> splitQuery(String query) {
        Validate.notBlank(query, "query can't be blank");

        Map<String, String> query_pairs = new LinkedHashMap<>();
        String[] pairs = query.split("&");
        for (String pair : pairs) {
            int idx = pair.indexOf("=");
            query_pairs.put(pair.substring(0, idx), pair.substring(idx + 1));
        }
        return query_pairs;
    }

    public static String getInputOrInputFromFile(String input) throws IOException {
        if(StringUtils.isNotBlank(input) && input.charAt(0) == '@'){
            return new String(Files.readAllBytes(Paths.get(input.substring(1))));
        }
        return input;
    }

    public static String[] getInputOrInputFromFile(String[] inputs) throws IOException {
        List<String> finalInputs = new ArrayList<>();
        for(String input: inputs){
            if(StringUtils.isNotBlank(input) && input.charAt(0) == '@'){
                finalInputs.add(new String(Files.readAllBytes(Paths.get(input.substring(1)))));
            }
            else{
                finalInputs.add(input);
            }
        }
        return finalInputs.toArray(new String[inputs.length]);
    }
}
