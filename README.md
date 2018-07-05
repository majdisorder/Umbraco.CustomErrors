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

After you have set up a doctype and assigned a template, go to the Content section of Umbraco and create your error pages. In this particular setup we will use the Content Name property to hold the error code. This gives us something to check against, as well as an easily accesible url. Of course this means we will need a content item for each error code we want to capture. You can always use a different approach, but I kind of like this, beacause it's clean and simple.

So, say we want to catch 500, 400 and 404. Create three nodes using the Error Page Document type we just created, and name them... You guessed it: 500, 400 and 404 respectively. 

**Note:** I also set up ModelsBuilder with `ModelsMode="AppData"`. This isn't absolutely necessary, but I do it this way because it allows us to write cleaner code.

### 2. Show me the code
#### ErrorPageFilter.cs
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
