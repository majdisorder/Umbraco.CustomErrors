<!DOCTYPE html>
<% 
    var httpCodeRaw = Request["code"];
    int httpCode;
    if (int.TryParse(httpCodeRaw, out httpCode))
    {
        Response.StatusCode = httpCode;
    }
    else
    {
        httpCode = 500;
       Response.StatusCode = httpCode;
    }

    try
    {
        Server.TransferRequest("/" + httpCode, true);
    }
    catch(Exception e)
    {
        //do the logging boogie
    }
%>

<html>
<head>
    <meta charset='utf-8' />
    <title>Oops</title>
</head>
<body>
    Something went terribly wrong. 
    <!-- 
        This is an ultimate fallback, in case umbraco can't process the request to the actual error content.
        Feel free to style this as you see fit. Of course you can always intentionally leave it ugly ;)    
    -->
</body>
</html>
