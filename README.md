# EVALUATING WEBSITE PERFORMANCE
ASP.NET MVC web application for evaluating website performance. <br/>
Used technologies: .NET Framework/C#, ASP.NET MVC, MS SQL, Entity Framework, LINQ, Ninject, HTML, CSS, JS, Bootstrap.

## /Home/Index
Contains form with inputs for: <br/>
URL, number of sitemap’s URLs, number requests to each URL.<br/>
Unregistered users can get this page, but their request history will not be stored.<br/>
Registered user can get their tested websites list on 'History' page.

## /Home/Results
On the top of the page display pages speed graphic that contain min and max values for each page.<br/>
On the bottom of the page display page speed as a table.<br/>
The table contains the requested pages and their minimum and maximum response values.<br/>
The table can be sorted in ascending and descending order by clicking the header cell of each column.<br/>
Initially slowest requests is on table top.
All values presented in (ms).

## /Home/HistoryList
Available only for registered users.<br/>
Contains table with tested websites list with request url and creation date.<br/>
The table can be sorted in ascending and descending order by clicking the header cell of each column.<br/>
History can be removed entirely, or it can be removed as a separate item.

## /Home/HistoryResult
Available only for registered users.<br/>
Contains the result of an already tested website and has all the functions of the "/Home/Results" page.

## /Account/Register
For register used email address, password and confirm password.<br/>
Password must be at least 6 characters.

## /Account/Login
For login used email address and password.

## Test account
email: test@gmail.com<br/>
password: qwerty
