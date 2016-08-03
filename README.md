#Introduction
This code sample demonstrates how to use Okta's OpenID Connect implementation with an ASP.NET Single Page Application calling an ASP.NET Web API.

##Solution architecture
The Visual Studio solution is made of 2 projects:  
- **SinglePageWebApp** is a single page web application where most (if not all) of the action occurs in the Views/Home/Index.cshtml page (along with the Scripts/single-page.js JavaScript file)  
- **WebApi** is an ASP.NET Web API project that exposes 2 very simple endpoints:  **/hello** and **/secure/hello**.


##Pre-requisites
- The SinglePageWebApp project _**MUST**_ be run over an HTTPS url. I personally use NGrok (www.ngrok.com) but you are obviously free to use whichever similar tool you want or use a real SSL certificate. Note that self-signed certificates are **NOT** supported (though they might work - test at your own risks!).

- This sample was built on Windows Server 2012 and Visual Studio 2015 Community Edition. Although we don't make Visual Studio 2015 a requirement, we highly recommend that you use it to test this application.

#How to run this sample
1. First of all, please download the source code above and make sure you can compile it in Visual Studio 2015 (Community Edition is fine)
2. Open the **web.dev.release.config** in both the SinglePageWebApp and the WebApi projects and make sure to properly fill out the 3 last parameters.  
You can also duplicate that file and create a new **web.dev.debug.config** file if you'd rather run the projects in debug mode. 
3. If you haven't done so yet, please make sure to create an OpenID Connect client. Use the UI of your Okta organization's administration site by going to Applications -> Add Application -> Create New App and selecting the "OpenID Connect" option (OpenID Connect is still in Early Access, so please contact Okta at developers at okta dot com to have OpenID Connect enabled on your Okta development organization).
4. The default OpenID Connect app that Okta currently creates is for a traditional web application (using the OpenID Connect/OAuth Code Flow). To create a Single Page Web Application and use the OpenID Connect/OAuth Implicit Flow, edit the url of the page that just opened (and that should end with (applicationType=WEB) and update it to (applicationType=BROWSER). Then submit the page.
5. Set the Redirect URIs field (don't press "Add another") to the HTTPS url of your SinglePageWebApp application (such as https://abcd.ngrok.io). I recommend that you use ngrok (www.ngrok.com) to get a publicly available, SSL-enabled web url but you can use any other technique you might like. [if you do use Ngrok, I recommend you read the following article to see how to use with the Visual Studio and IIS Express: https://gist.github.com/nsbingham/9548754 ]
6. Presh "Finish" and on the General tab, copy the Client ID value and paste it into both **web.dev.release.config** config files (of both Visual Studio projects)
7. Assign test users to this new application (these are the users you'll use to test the application)
8. Go to Settings -> Customization and scroll to the bottom. Enable "Allow Iframe embedding"
9. Go to Security -> API and generate a new API Token. Copy the value of that token into the web.dev[config|release].config file of the WebApi project
10. Last but not least, go to the CORS tab (close to the API tab) and press the Edit button. Then add the https:// url of your SinglePageWebApp project and press Save.
11. You should be good to go and ready to test your SPA/ASP.NET Web API application!
12. If you want to look more closely at the code, you should check the Views/Home/Register.cshtml page as well the Scrips/single-page.js script file