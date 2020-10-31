# Outlook Clone

## WUT is that?
 The Outlook Clone is a pseudo Outlook webmail application.
It should be accessible and fully operative via the html website.

## Usage
- Visit the [website](potential_link)
- Create an account
- Mail your friends

## Roles
- User role
- Admin role
- Group owner role

## Tech
Our stack:
 
- ASP.NET Core 3.1 MVC  
- ASP.NET Core 3.1 API  
- Entity Framework  
- Azure SQL DB
- Azure AD B2C
- Azure Service
- Azure SendGrid

We are using git and git-flow, Azure DevOps and we are agile.

## Architecture

- User open website which is stored on Azure Web App
- List of the job offers is loaded asynchronously via API using AJAX
- Data are stored in SQL database
- Communication between Web App and SQL done by Entity Framework
- Files are stored in Azure Blob storage
 - User Authentication is done with Azure AD B2C
 - Periodical notification is done via Azure SendGrid 

### Development
 In progress...

### Why is it even being created?
It is a part of .NET Web Development course at Warsaw University of Technology.
