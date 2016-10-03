#Introduction
This code sample demonstrates how to use Okta's OpenID Connect implementation with an ASP.NET Single Page Application calling an ASP.NET Web API.

##Solution architecture
The Visual Studio solution is made of 2 projects:  
- **SinglePageWebApp** is a single page web application where most (if not all) of the action occurs in the Views/Home/Index.cshtml page (along with the Scripts/single-page.js JavaScript file)  
- **WebApi** is an ASP.NET Web API project that exposes 2 endpoints:  **/unprotected** and **/protected**. The **/unprotected** endpoint can be called by anonymous or non-authorized users, but the **/protected** endpoint can only be accessed by authenticated users who are members of pre-configured Okta groups.


##Pre-requisites

- This sample was built on Windows Server 2012 and Visual Studio 2015 Community Edition. Although we don't make Visual Studio 2015 a requirement, we highly recommend that you use it to test this application.

#How to run this sample
1. First of all, please download the source code above and make sure you can compile it in Visual Studio 2015 (Community Edition is fine)
2. Open the **web.dev.release.config** in both the SinglePageWebApp and the WebApi projects and make sure to properly fill out the 3 last parameters.  
You can also duplicate that file and create a new **web.dev.debug.config** file if you'd rather run the projects in debug mode. 
3. If you haven't done so yet, please make sure to create an OpenID Connect client. Use the UI of your Okta organization's administration site by going to `Applications -> Add Application -> Create New App`.
4. In the `Platform` dropdown list, select the `Single Page App (SPA)` option. The sign-on option should then default to `OpenID Connect`. If you don't see it, please contact Okta at developers at okta dot com to have OpenID Connect enabled on your Okta development organization (Okta's OpenID Connect feature is currently in Early Availability).
4. Press the `Create` button.
5. Fill out the `Application Name` field.
6. Add a Redirect URI to be the url of your SinglePageWebApp application (by default `http://localhost:8080/`).
7. In the `Authorization Server` tab of your OpenID Connect client app in Okta, click `Edit` in the `OpenID Connect ID Token` section.
8. In the `Groups claim` section, select the `Starts with` dropdown box and instead select `Regex`.
9. In the blank text field following `Regex`, enter `.*`. This is the regular expression that includes all the Okta-mastered groups the user is a member of in the ID Token. Press `Save` to save your ID Token settings.
10. Add a Redirect URI to be the url of your SinglePageWebApp application (by default `http://localhost:8080/`).
11. Presh "Finish" and on the General tab, copy the Client ID value and paste it into both **web.dev.release.config** config files (in the `SinglePageWebApp` and `WebApi` Visual Studio projects).
12. Go to the `Security -> API -> CORS` (or `Security -> API -> Trusted Origins`) tab and add the url of your application (such as `http://localhost:8080`) as CORS endpoint. 
13. Assign test users to this new application (these are the users you'll use to test the application).
14. In the **web.dev.release.config** of the `WebApi` project, update the `okta:RequiredGroupMemberships` value to include the names of the possible group(s) your test users are assigned to (comma-separated list). The `OktaGroupMembershipAttribute` custom class in the `WebApi` project will validate a user if he belongs to any of these groups (not all of them).
10. You should be good to go and ready to test your SPA/ASP.NET Web API application!
12. If you want to look more closely at the code, you should check the Views/Home/Register.cshtml page as well the Scrips/single-page.js script file