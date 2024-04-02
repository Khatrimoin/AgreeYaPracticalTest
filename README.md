# AgreeYaPracticalTest

This repository contains an N-tier architecture pattern for a web application. Below are the details and instructions for configuring and using the different components of this architecture.

Components:

1. CustomerAPI Project:
   - AppSettings Configuration:
     - Update the appsettings.json file with your own database connection string:
       "DefaultConnection": "Server=*;Database=AgreeYaPracticalDatabase;User=*****;Password=******;"

2. CustomerMVC Project:
   - AppSettings Configuration:
     - Update the appsettings.json file with your own SMTP settings for sending emails:
       "SmtpSettings": {
           "Server": "smtp.gmail.com",
           "Port": 587,
           "Username": "Example@gmail.com",
           "Password": "your Password",
           "SenderEmail": "example@gmail.com"
       }

3. Default User:
   - A default user named gk is provided with Admin role access.
   - Credentials:
     - Username/Email: gk@gmail.com
     - Password: gk@12345

4. Architecture Details:
   - The architecture utilizes:
     - .NET Core Restful API
     - .NET Core MVC
     - 3 Class Library projects with Repository pattern
     - XML comments in API for documentation
     - Logger implementation
     - Exception handling concept
