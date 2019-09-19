package cn.com.sinosure.dsp.wps;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;

import javax.servlet.ServletInputStream;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class FileInfoController {

    @RequestMapping("/v1/3rd/fileinfo")
    public FileInfo getFileInfo(@RequestParam String fname) {
        FileInfo result = new FileInfo();
        result.setFname(fname);
        //        result.setUrl("Z:\\Y2018\\N9\\" + fname);
        //        result.setUrl("http://10.1.97.119:9001/dsp/dspfile/download?moduleKey=test123&fileId=230088&userId=zhaojh&token=b3d3a962e8e4e8138954d6a9c52595a8f414f1adcb16d654815cb21a2ca2ed91");
        result.setUrl("http://10.1.97.119:9001/dsp/uploader/new");

        //        try {
        result.setUniqueId(java.util.UUID.randomUUID().toString());
        //        result.setUniqueId("5993db79-3104-41e7-a883-aff9215b0734");
        //        } catch (IOException ex) {
        //            ex.printStackTrace();
        //        }

        result.setCode(200);
        result.setGetFileWay("download");
        result.setEnableCopy(true);
        result.setWatermarkType(1);
        result.setWatermark("Kingsoft WPS test");
        result.setDetail(new FileInfo().getFileInfoDetail());
        result.setWatermarkSetting(new FileInfo().getWatermarkSetting());
        result.setMsg("");
        System.out.println(result);
        return result;
    }

    @RequestMapping("/callback")
    public void callback(HttpServletRequest request, HttpServletResponse response) {

        try {
            InputStreamReader reader;
            reader = new InputStreamReader(request.getInputStream(), "UTF-8");

            char[] buff = new char[1024];
            int length = 0;
            System.out.println(reader.read(buff));
            while ((length = reader.read(buff)) != -1) {
                String x = new String(buff, 0, length);
                System.out.println(x);
                //docx-----{"dur":89,"error":"","fp1CostTime":0,"resultCode":0,"scode":0,"sha1":"4d21abef0df6616312a73b4958b57c7f286bacd3","sheetCount":0,"totalPages":1,"totalTime":89}
                //xlsx-----{"dur":589,"error":"","fp1CostTime":0,"resultCode":0,"scode":0,"sha1":"4d21abef0df6616312a73b4958b57c7f286bacd3","sheetCount":3,"totalPages":12,"totalTime":589}
            }
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static String getContent(InputStreamReader isr, String charset) {
        String pageString = null;
        BufferedReader br = null;
        StringBuffer sb = null;
        try {
            br = new BufferedReader(isr);
            sb = new StringBuffer();
            String line = null;
            while ((line = br.readLine()) != null) {
                sb.append(line + "\n");
            }
            pageString = sb.toString();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            try {
                if (isr != null) {
                    isr.close();
                }
                if (br != null) {
                    br.close();
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
            sb = null;
        }
        return pageString;
    }

    public static void charReader(HttpServletRequest request) {
        BufferedReader br;
        try {
            br = request.getReader();
            String str, wholeStr = "";
            while ((str = br.readLine()) != null) {
                wholeStr += str;
            }
            System.out.println(wholeStr);
        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }

    }

    public static String getRequestPayload(HttpServletRequest req) {
        StringBuilder sb = new StringBuilder();
        try (BufferedReader reader = req.getReader();) {
            char[] buff = new char[1024];
            int len;
            while ((len = reader.read(buff)) != -1) {
                sb.append(buff, 0, len);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return sb.toString();

    }

    //二进制读取

    public static void binaryReader(HttpServletRequest request) {
        int len = request.getContentLength();
        System.out.println(request.getRequestURI());
        ServletInputStream iii;
        try {
            iii = request.getInputStream();
            byte[] buffer = new byte[len];
            iii.read(buffer, 0, len);
            String s = new String(buffer);
            System.out.println(s);
        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
    }

    @RequestMapping("/v1/3rd/file/callback")
    public String fileCallback(HttpServletRequest request, HttpServletResponse response) {
        try {
            InputStreamReader reader;
            reader = new InputStreamReader(request.getInputStream(), "UTF-8");
            char[] buff = new char[1024];
            int length = 0;
            while ((length = reader.read(buff)) != -1) {
                String x = new String(buff, 0, length);
                System.out.println(x);
                //{"dur":89,"error":"","fp1CostTime":0,"resultCode":0,"scode":0,"sha1":"4d21abef0df6616312a73b4958b57c7f286bacd3","sheetCount":0,"totalPages":1,"totalTime":89}
            }
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return null;
    }

    @RequestMapping("/v1/3rd/notify")
    public String notify(HttpServletRequest request, HttpServletResponse response) {
        try {
            InputStreamReader reader;
            reader = new InputStreamReader(request.getInputStream(), "UTF-8");
            char[] buff = new char[1024];
            int length = 0;
            while ((length = reader.read(buff)) != -1) {
                String x = new String(buff, 0, length);
                System.out.println(x);
                /*
                 * /view
                 * 下载的消息通知------{"cmd":"downloadFileNotify","data":{"duration":79,"fname":"2018淇濆瘑闂嵎.xlsx","uniqueId":
                 * "a9c693e9-8a53-476f-bea0-21a03997122e","url":
                 * "http://10.1.97.119:9001/dsp/dspfile/download?moduleKey=test\u0026fileId=230088\u0026userId=zhaojh\u0026token=b3d3a962e8e4e8138954d6a9c52595a8f414f1adcb16d654815cb21a2ca2ed91"
                 * }}
                 * 转换的消息通知------{"cmd":"convertNotify","data":{"duration":331,"error":"","fname":"2018淇濆瘑闂嵎.xlsx",
                 * "fp1CostTime"
                 * :93,"resultCode":0,"scode":0,"totalTime":346,"uniqueId":"a9c693e9-8a53-476f-bea0-21a03997122e"}}
                 */

                /*
                 * /view/preview
                 * 预览的消息通知------{"cmd":"viewNotify","data":{"fname":"2018淇濆瘑闂嵎.xlsx","uniqueId":
                 * "5993db79-3104-41e7-a883-aff9215b0734"}}
                 * 下载的消息通知------{"cmd":"downloadFileNotify","data":{"duration":49,"fname":"2018淇濆瘑闂嵎.xlsx","uniqueId":
                 * "5993db79-3104-41e7-a883-aff9215b0734","url":
                 * "http://10.1.97.119:9001/dsp/dspfile/download?moduleKey=test\u0026fileId=230088\u0026userId=zhaojh\u0026token=b3d3a962e8e4e8138954d6a9c52595a8f414f1adcb16d654815cb21a2ca2ed91"
                 * }}
                 * 转换的消息通知------{"cmd":"convertNotify","data":{"duration":323,"error":"","fname":"2018淇濆瘑闂嵎.xlsx",
                 * "fp1CostTime"
                 * :94,"resultCode":0,"scode":0,"totalTime":323,"uniqueId":"5993db79-3104-41e7-a883-aff9215b0734"}}
                 */
            }
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return null;
    }




}