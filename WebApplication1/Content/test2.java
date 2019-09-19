package com.sinosoft.sinobrps.util;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class test2 extends HttpServlet{
	private static final long serialVersionUID = 1L;
	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp)
			throws ServletException, IOException {
		 try {
	            InputStreamReader reader;
	            reader = new InputStreamReader(req.getInputStream(), "UTF-8");
	            BufferedReader in = new BufferedReader(reader);
	            StringBuffer buffer = new StringBuffer();
	            String line = "";
	            while((line = in.readLine()) != null){
	            	buffer.append(line);
	            	System.out.println(buffer.toString());
	            }
	            System.out.println(buffer.toString());
	           /* char[] buff = new char[1024];
	            int length = 0;
	            System.out.println(reader.read(buff));
	            length = reader.read(buff);
	            while (length != -1) {
	                String x = new String(buff, 0, length);
	                System.out.println(x);
	            }*/
	        } catch (IOException e) {
	            e.printStackTrace();
	        }
	}
	
}
