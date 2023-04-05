# ApiPro - No Code API Framework

ApiPro is a Web API framework developed in ASP.Net 4.7.1, which can be hosted in the IIS. It makes developing Web APIs so easy just writing JSON configuration files. 

Most of common features are already supported in the framework itself. Though it gives the capability to extend its features by writing C# code.

This framework can be used to create Web API where we have to fetch some data from the SQL Server using Stored Procedures or Parameterized SQL Quries.

This Framework has following components:

| SN| Component| Description|
| ------| ------| ------ |
|1|api.framework.net|Presentation Layer|
|2|api.framework.net.lib | Logical Business Layer|
|3|api.framework.net.business | External Business Layer|
|4|api.logging | Define Logging Framework |
|5|api.framework.net.exception |Define Exception Types|

## Unit Test Projects

The Framework solution has following unit test projects.

| SN| Component| Description|
| ------| ------| ------ |
|1|api.framework.net.test| Unit Tests for Presentation Layer|
|2|api.framework.net.lib.test | Unit Tests for Logical Business Layer|

Unit tests are written with MSTest framework, so can be run within the Visual Studio, in Test window.

# Code setup 

Setting up the framework can be done by following simple steps:

- As a First step get the local copy, and open the solution file in Visual Studio 2017 or later.
- Build the solution to restore all the nuget dependencies.
- Run the Web application (make sure Web Project is set as startup project in solution)

## Inbuilt Health Monitor URL

Framework provide a health monitor URL, which gives you current version of the API, with health status, as shown in below snapshot. It can also be used  as health probe in Load Balanced Environments.

It is available at the application root URL:
http://localhost:23450

![Output](https://github.com/contact2abhi/ApiPro/blob/main/public/output.PNG)

## Inbuilt Swagger documentation

Framework provide the read only, dynamic swagger documentation for all configured endpoints. That can be accessed by browsing the following local URL:
http://localhost:23450/swagger

![Output](https://github.com/contact2abhi/ApiPro/blob/main/public/swagger.PNG)

### Note: 
The Employee API you see on running it first time, is just a dummy endpoint for DEMO purpose. To clean up this endpoint, you have to remove the following files:

\src\api.framework.net\Configs\schema\endpoints\get_employee_details.json
\src\api.framework.net\Configs\schema\definitions\employee.json

# Authentication

For Authentication of each API request, Framework expects OAuth Bearer token with RSA (SHA-256) Encryption with configurable key size. To configure the token validation using same algorithm following configurations need to be updated in Presentation Layer (Web Project of solution) Web.Config file:

```
    <add key="myCompany" value="myCompany" />
    <add key="KeySize" value="2048" />
    <add key="PublicAuthKey" value="Configs\\public_key.xml" />
```
**myCompany**: holds the Issuer of token in token validation.

**KeySize**: holds the RSA key size.

**PublicAuthKey**: holds the local path of RSA public key  in XML format.

### Customize Authentication
 
Framework gives you flexibility to write your own token validation. Write your own code in following file:

/api.framework.net.business/BusinessAuthorzation/ApiBusinessAuthorization.cs

The above code is already injected in Authentication module using Dependency Injection.

# Features

- No Code API framework provide the inbuilt swagger documentation.
- Provide very detail level transaction based logs, to troubleshoot the issue quickly.
- Provide the mocking feature, which is helpful when the backend procedure is not ready, but contract of output is finalized.

# How It Works

It reads all the endpoints and schemas defined in configuration section of presentation layer of framework, and register all the routes under the common action method using MVC architecture.

# High Level Design

![Logical structure](https://github.com/contact2abhi/ApiPro/blob/main/public/high_level_design.png)


## Define Endpoints

To Add an endpoint we need to create a JSON file at below path

\src\api.framework.net\Configs\schema\endpoints\

Sample endpoint:

```
{
      "tag": "employees",
      "version": "v1",
      "summary": "get employee's detail for given employee id",
      "scope": "employees:read",
      "verb": "GET",
      "name": "",
      "route": "/api/{version}/{tag}/{EmployeeId}",
      "path": "/api/v1/employees/{EmployeeId}",
      "inputs": [
        {
	  "id": 1,
          "name": "traceId",
          "type": "string",
          "source": "header",
          "description": "unique request id."
        },
        {
          "id": 2,
          "name": "EmployeeId",
          "type": "string",
          "source": "path",
          "description": "employee unique id",          
          "validations": [
            {
              "type": "regex",
              "regex": "^[0-9]*$",
              "error": "Employee Id {0} is not valid. Please provide valid Employee ID for further processing.",
              "errorCode": "InvalidEmployeeId"
            }
          ]
        }
      ],
      "operations": [
        {
          "order": 1,
          "db": "ApplicationDb",
          "type": "sp",
          "value": "sp_GetEmployeeDetails",
          "inputs": [
		      {
			  "source": "client",
			  "mapId": "2",
			  "name": "Id",
			  "type": "BIGINT"
                    }
		  ],
          "output": {
            "Data": [ "Name", "Designation", "EmailId", "JoiningDate", "ProjectCode", "ManagerId" ],            
            "Errors": [ "ErrorCode", "ErrorMessage" ]
          }
        }
      ],
  "responses": {
    "200": {
      "description": "OK",
      "schema": { "$ref": "#/definitions/Employee" }
    },,
    "400": {
      "description": "Bad Request",
      "schema": { "$ref": "#/definitions/error" }
    },
    "404": {
      "description": "Not Found",
      "schema": { "$ref": "#/definitions/error" }
    },
    "401": {
      "description": "Unauthorized",
      "schema": { "$ref": "#/definitions/error" }
    }
  }
}
  
```
### Notes:

- File can be saved by any name, just make sure it has a file extension of JSON type (*.json).

- One file can have only one endpoint defined in it.

## Define Schemas

Schema's used by endpoint can be defined at below location:

\src\api.framework.net\Configs\schema\definations\

Sample Schema:

```
{
  "Employee": {
    "type": "object",
    "properties": {
      "Name": {
        "type": "string",
        "example": "Abhishek Goel"
      },
      "Designation": {
        "type": "string",
        "example": "Software Engineer"
      },
      "EmailId": {
        "type": "string",
        "example": "abhishek-goel@hcl.com"
      },
      "JoiningDate": {
        "type": "string",
        "example": "13-05-2022"
      },
      "ProjectCode": {
        "type": "string",
        "example": "ERS"
      },
      "ManagerId": {
        "type": "number",
        "example": "123456"
      }
    }
  }
}
```


### Notes:

- File can be saved by any name, just make sure it has a file extension of JSON type(*.json).

- One file can any number of schemas in it.

- Currently schemas are only used in Swagger Documentation. There is no schema validation done by the framework.

## Further Help

To get more help on defining the endpoints in the framework. [Click Here](https://github.com/contact2abhi/ApiPro/blob/main/DEVELOPER_GUIDE.md)

## License

MIT

## Maintainers

This is currently maintained by following team members:

Primary: abhishek-goel@hcl.com

Secondary: amitkumar.vahash@hcl.com

## Third Party dependencies

The Framework is using following NuGet Packages

- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle)
- [NLog](https://nlog-project.org/)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Unity](https://github.com/unitycontainer/unity)
- [WebActivatorEx](https://github.com/davidebbo/WebActivator)
- [Owin](https://github.com/owin-contrib/owin-hosting/)
- [Moq](https://github.com/moq/moq4)
- [OpenCover](https://github.com/opencover/opencover)
- [MSTest.TestFramework](https://github.com/microsoft/testfx)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)


