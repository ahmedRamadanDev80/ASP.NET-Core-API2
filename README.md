## ASP.NET-Core-API2

This is an api for a social media clone u can manage users and write posts , and it was impleneted using an approach that focuses on performance 
and i ended up liking this approach alot more than the Entity framework approach .

# Description

this Api has 2 Versions :

 Version 1 => has endpoints that talk to the Db using Dapper theat takes in a Sql string and passes it to the Db
              and this approach will work fine but its not secure against sql injections and this simple api 
              will need too many end points than it needs to be.
              
 Version 2 => in this Version we convert the database to a punch of stored procedures and we make use of dynamic parameters
              to avoid sql injections and it takes less end points to do the same job as version 1 in a more dynamic way.
              
 Version Neutral => we have an Auth Controller which is version neutral and we implement Auth from scratch in it not using .net identity .
 
--------------------
# Before Cloning 
if you want to clone it before running it make sure that u run the sql script in the DbScripts Folder
and change the connection string in appsetting.json to your localDb name 
before you build the Project 
