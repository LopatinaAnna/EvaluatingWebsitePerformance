# EVALUATING WEBSITE PERFORMANCE
ASP.NET MVC web application for evaluating website performance. <br/>
Used technologies: .NET Framework/C#, ASP.NET MVC, MS SQL, Entity Framework, LINQ, Ninject, HTML, CSS, Bootstrap.

## Home
Contain form with inputs for: <br/>
URL, number of sitemap’s URLs, number requests to each URL.<br/>
Unregistered users can get this page, but their request history will not be stored.<br/>
Registered user can get their tested websites list on 'History' page

## Results
On the top of the page display pages speed graphic that contain min and max values for each page.<br/>
On the bottom of the page display page speed as a table that contain requested pages and their min and max response values.<br/>
Slowest requests is on top.
All values presented in (ms).


## History
Available only for registered users.<br/>
Containt tested websites list with request url and creation date.<br/>
History can be removed entirely, or it can be removed as a separate item.

## Register
For register used email address, password and confirm password.<br/>
Password must be at least 6 characters.

## Login
For login used email address and password.

## Test account
email: test@gmail.com<br/>
password: qwerty
