# Umbraco.CustomErrors
Sample project demonstrating custom error pages setup with umbraco content.

Setting up custom error pages for Umbraco can be a bit of a pain sometimes. Especially if you want the content to be editable through the CMS, and use the same templates you use for your content pages.

This project demonstrates how you can set up custom error pages in a few easy steps. 
If possible the content and templates for these pages will be loaded from Umbraco. 
Of course there is also a fallback scenario for those cases when an error occurs in Umbraco itself. 

## Features
- Configure content of the error pages in Umbraco 
- Configure templates of the error pages in Umbraco 
- Returns appropriate http status codes
- Works in multi domain setup
- Handle 404's the easy way (no more messing with custom content finders)

## Getting started
### 1. Setting up Umbraco
First we will create a Document Type to hold our error content. Of course you can makes this as complex as you see fit. For this sample I just went for the basic properties: Title, Body and umbracoNaviHide (unless you want your error pages to visible in your navigation :smirk:). 

It doesn't really matter how you set up the doctype, the real important part here is having a doctype alias, as we will be using that later.

After you have set up a doctype and assigned a template, go to the Content section of Umbraco and create your error pages. In this particular setup we will use the Content Name property to hold the error code. This gives us something to check against, as well as an easily accesible url. Of course this means we will need a content item for each error code we want to capture. You can always use a different approach, but I kind of like this, because it's clean and simple.

So, say we want to catch 500, 400 and 404. Create three nodes using the Error Page Document type we just created, and name them... You guessed it: 500, 400 and 404 respectively. 

**Note:** I also set up ModelsBuilder with `ModelsMode="AppData"`. This isn't absolutely necessary, but I do it this way because it allows us to write cleaner code.

### 2. Show me the code
#### Filters/ErrorPageFilter.cs
This ActionFilter will capture requests to our Error Page Document Type and set the correct http status code.

```cs
public override void OnResultExecuted(ResultExecutedContext filterContext)
{
    var content = ((filterContext.Result as ViewResultBase)?.Model as RenderModel)?.Content;

    //feel free to use a magic string instead of ErrorPage.ModelTypeAlias, if you're not using ModelsBuilder 
    if (content?.DocumentTypeAlias == ErrorPage.ModelTypeAlias) 
    {
        if (int.TryParse(content.Name, out var statusCode))
        {
            statusCode = 500; //something is definitely wrong
        }

        filterContext.HttpContext.Response.StatusCode = statusCode;
    }
}
```

#### Events/Application.cs
Hook up the ActionFilter we just created as a global filter.
```cs
public class Application : ApplicationEventHandler
{
    protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
        GlobalFilters.Filters.Add(new ErrorPageFilter());
    }
}
```
#### error.aspx
This page will perform the actual magic. Does it have have to be an aspx page you say? Yes. I'm sorry.
```
<% Response.StatusCode = 500; %> <!-- this is just a fallback really -->
<!DOCTYPE html>
<% 
    var httpCodeRaw = Request["code"];
    int httpCode;
    //determine the status code based on the querystring, or default to 500
    if (int.TryParse(httpCodeRaw, out httpCode))
    {
        Response.StatusCode = httpCode;
    }
    else
    {
        httpCode = 500;
    }

    try
    {
        Server.TransferRequest("/" + httpCode, true); 
        //load the actual content from umbraco. 
        //This is why we set the Name (and hence the url) to the http status code
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
```

### 2. Configuring IIS
#### Web.config
At this point we are ready to configure the custom errors for our application. For this you will need to edit the Web.config. Only the relevant sections are shown.
```xml
<configuration>
    <appSettings>
    <add key="umbracoReservedUrls" value="~/error.aspx,~/config/splashes/booting.aspx,~/install/default.aspx,~/config/splashes/noNodes.aspx,~/VSEnterpriseHelper.axd" />
    <!-- umbracoReservedUrls will already be in the config file, just add ~/error.aspx to the list -->
    </appSettings>
    <system.web>
        <customErrors mode="On" redirectMode="ResponseRewrite" defaultRedirect="~/error.aspx">
          <error statusCode="500" redirect="~/error.aspx"/>
          <error statusCode="400" redirect="~/error.aspx?code=400"/>
          <error statusCode="404" redirect="~/error.aspx?code=404"/>
        </customErrors>
    </system.web>
    <system.webServer>
    <httpErrors  errorMode="Custom">
      <remove statusCode="400"/>
      <error statusCode="400" path="/error.aspx?code=400" responseMode="ExecuteURL"/>
      <remove statusCode="500"/>
      <error statusCode="500" path="/error.aspx" responseMode="ExecuteURL"/>
      <remove statusCode="404"/>
      <error statusCode="404" path="/error.aspx?code=404" responseMode="ExecuteURL"/>
    </httpErrors>
    </system.webServer>
<configuration>
```

And that's it. If all goes well, you should now be able to serve error pages straight from Umbraco.

If you want to test if everything works, I added some links to the master template to trigger 500 and 400 errors. For a 404 just try an unknown url.

