# Define Endpoint
API no-code framework allows the developer to write the API endpoints in form of JSON configuration. For example, below JSON snippet define the endpoint schema for the no-code API framework:

```
  "tag": "",
  "version": "",
  "summary": "",
  "scope": "",
  "verb": "GET | POST | PUT | DELETE",
  "name": "",
  "route": "",
  "path": "",
  "inputs": [
  ],
  "operations": [
  ],
  "responses": {
  }
```

## Define simple endpoint 

Following is the example for define a simple endpoint like below:

**api/v1/employees/get**

```
{
  "tag": "employees",
  "version": "v1",
  "verb": "GET",
  "name": "get",
  "route": "/api/{version}/{tag}/{name}",
  "path": "/api/v1/employees/get"
}
```

## Define endpoint having path parameter

Following is the example for define an endpoint which has 2 path parameter org_id adn user_id and look as below:

/ap/v1/employees/{employee_id}/get

```
{
  "tag": "employees",
  "version": "v1",
  "verb": "GET",
  "name": "get",
  "route": "/api/{version}/{tag}/{name}/{employee_id}",
  "path": "/api/v1/employees/get/{employee_id}/"
}
```
## Define Inputs

We can add as many inputs as possible to the endpoint, by just adding them into the configuration. 

| Property Name | Type | Description |
|-----|-----|-----|
| name |string|This property defines the name of the input.|
|type| string |This property defines the type of the input as string. For example, if you need to define a property as integer its value will be “int” |
|id|integer|This property defines the unique id for the input. This should be unique and used to map the input with backend operation’s inputs.|
|defaultValue|string|This property defines the default value for the input, for example if an input is not mandatory this property can be used.|
|conditionalDefaults|List<ConditionalDefault>|This property defines the list of expressions along with condition and value. It's evaluated when the input is not provided a value, also defaultValue is not configured. It evaluates each expression and takes the value of first condition which evaluate as true. Examples with use case given below. |
|enum|Dictionary<string, int>|This property defines the collection of all possible values, for example if an input can have only some specific values, they this property is useful.|
|pattern|string|This property defines the input pattern for the input types those support the formatting. Currently this support for DateTime or Date type inputs only. |
|source|string|This property specifies the source for the input. It only support “header”, “query”, “path”, "body".|
|validations|List<Validation>|This property defines the validations required for the input. All validations are of type validation.|

## Define different “source” for input

### header

```
{
  "name": "traceId",
  "type": "string",
  "source": "header"
}
```

### query

```
{
  "name": "id",
  "type": "string",
  "source": "query"
}
```

### path

```
{
  "name": "id",
  "type": "string",
  "source": "path"
}
```

### body

```
{
  "name": "data",
  "type": "string",
  "source": "body"
}
```

If you have path parameters, in the endpoint, we have to define the same in the route property of endpoint. Which will tell the framework about the position of the input in URL. For example, if we have “id” input to be read from path like below:

“/api/v1/employees/get/{id}”

it has to be defined in endpoint like below:

```
{
  "tag": "employees",
  "version": "v1",
  "verb": "GET",
  "name": "get",
  "route": "/{id}"
}
```

## Define “conditionalDefaults” for input

If you have two inputs for an API named input1 and input2. And default value for input2 should be 0 if value of input1 is 1 and 1 if greater than 1.

```
{
    "name": "input2",
    "type": "int",
    "source": "query",
    "conditionalDefaults": [
      {
        "expressions": [
          {
            "inputName": "input1",
            "condition": "=",
            "value": "1"
          }
        ],
        "value": "0"
       },
       {
        "expressions": [
          {
            "inputName": "input1",
            "condition": ">",
            "value": "1"
          }
        ],
        "value": "1"
       }
     ]
  }
```
Here value is used for defining static values. But if you have to assign default value as the value of some other input, that can be done by using the fieldValue instead of value and give the value as name of that input. 

For example, in above example if we have to give the default value of input1 then, we could have defined like below:


```
"conditionalDefaults": [
      {
        "expressions": [
          {
            "inputName": "input1",
            "condition": "=",
            "value": "1"
          }
        ],
        "fieldValue": "input1"
       }
]
```

Also, you could assign value to input from the bearer token’s claim, by specifying the name of the claim like below:

```
"conditionalDefaults": [
      {
        "expressions": [
          {
            "inputName": "input1",
            "condition": "=",
            "value": "1"
          }
        ],
        "claimValue": "[name of the claim here]"
       }
]
```

## Define “enum” for the input 

If we need to specify the input can have only specific values, any other value should be treated as invalid. Then we can define them under enum like below:

```
{
      "name": "shape",
      "type": "string",
      "source": "query",
      "enum": {
        "Round": 0,
        "Triangle": 1,
        "Rectangle": 2
      },
      "validations": [
        {
          "type": "type",
          "error": "Invalid shape value entered.",
          "errorCode": "InvalidShape"
        }
      ]
 }
```

## Example for defining different “type” of inputs

There are following supported types we can give to an input.

| type | Description |
|-----| ------|
| long | for long type of input.|
| int | for int type of input. |
| double | for double type of input. |
| string | for string type of input. |
| date | for date type of input. |
| datetime | for datetime type of input. |
| delimited | for delimited type of input. For example, API needs to receive multiple values like emails separated by a configured character in a single field. separator |
|bool | for bool type of input.|

## Define Validations for input

We can add any of the following type of validations. With each validation you can define the error code and error message, if not defined, framework will generate the error details.

| Type | Description |
| --- | --- |
| type | This is mandatory validation; you cannot turn this validation off. In this validation input value is validated against the type of the input specified. For example, if an input is of type integer and user provide an alphabet, this validation will get triggered. |
| mandatory | This validation checks that, the input is provided a value. If not provided this validation will be triggered. |
| regex | This validation checks the input value against the given regex. If no match found, then this validation will be triggered. |
| compare | This validation checks the input value against the given **value**, or any other field mentioned with **fieldName ** for the given **condition**. Supported condition values are (>, >=, = or ==, !=, <, <=). The expression is created on the runtime with input value, condition and value (or fieldName). For example, value is given 10, and condition is given >, then if input is given value less than 10 it will get triggered. |
| range | This validation checks the input value against the given range with rangeStart and rangeEnd. For example, value is given 10, and **rangeStart** is 0 and **rangeEnd** is 9, then this validation will get triggered, as the given value does not fall between the range. This validation works with numeric types and date type of inputs. |
| length | This validation checks the input value against the **minLength** and **maxLength**. For example, value is given testvaluevalidation, and minLength is 0 and maxLength is 9, then this validation will get triggered as the length of the string is 19. This validation works with **string** type and **delimited** type of inputs. |
| multiExpression | This validation allow user to execute multiple expressions and evaluate condition. Based on the output of condition this validation get triggered. |
| custom | For this validation, user allowed to write a new function, which returns bool based on which this validation get triggered. |

### Mandatory validation
This check makes sure that user provide a value for that input. Below example is showing mandatory validation configuration for PageNo field. 

```
{
    "type": "mandatory",
    "error": "Page number does not exist. Please provide valid page number.",
    "errorCode": "PageNoMissing"
}
```

### Type validation

This check makes sure that the value provided for the input is of proper type of input. For example, if input type is a date but provided value is not a valid date.

```
{
    "type": "type",
    "error": "Invalid date. Please enter a valid date.",
    "errorCode": "DateValidation"
}
```

### Regular Expression validation
This check validates the input value against the regex provided in this validation. For Example, if we want to validate the provided date is in 'dd-MMM-yyyy' format.

```
{
     "type": "regex",
     "regex": "^[0-9]{2}-[a-zA-Z]{3}-[0-9]{4}$",
     "error": "Date format is Invalid. Please enter date in format (dd-MMM-yyyy).",
     "errorCode": "DateFormatValidation"
}
```

### Compare validation

This check validates the input value against any static value, dynamic value or any other input’s value. For Example, if we want to validate that start date should be smaller than end date.

```
{
     "type": "compare",
     "condition": ">=",
     "fieldName": "StartDate",
     "error": "Incorrect end date provided. End date cannot be less than start date.",
     "errorCode": "EndDateLessValidation"
}
```

or if we want to check that end date should be less than today’s date.

```
{
     "type": "compare",
     "condition": "<",
     "value": "today",
     "error": "Incorrect end date provided. End date cannot be greater than or equal to today's date.",
     "errorCode": "EndDateGreaterValidation"
}
```
or if we want to check if page no should be greater than 0.

```
{
     "type": "compare",
     "condition": ">",
     "value": "0",
     "error": "Page number is less than required. Minimum page number should not be less than 1 page.",
     "errorCode": "MinPageNoValidation"
}
```

### Range validation
This check validates the input value falls between the specified range.  Both range start and range end values are excluded in comparison, for example we have a bit flag for which only 0 or 1 is expected, we can say the range is 0 < {value} > 2.

```
{
    "type": "range",
    "rangeStart": "0",
    "rangeEnd": "2",
    "error": "Invalid Isshift parameter entered. Supported flags for IsShift are either 1 or 0.",
    "errorCode": "InvalidIsShift"
}
```

### Length validation
This check validates the length of input value falls between the specified range.  Both range start and range end values are excluded in comparison. This is valid for string and delimited type of inputs.

For example, if user want to only accept 50 coma separated email ids.

 ```
{
      "type": "length",
      "maxLength": 50,
      "error": "Requested Email ID exceed permitted limit. Maximum number of Email ID's should not be more than 50 records.",
      "errorCode": "MaxEmailValidation"
}
```

If user want to restrict length of string input between 1 to 440.

```
{
     "type": "length",
     "minLength": 1,
     "maxLength": 440,
     "error": "Unique ID {0} not found. Please provide valid Unique ID for further processing.",
     "errorCode": "InvalidNode"
}
```

### Dynamic Expression validation
This validation allow user to execute multiple expressions and evaluate condition. Based on the output of condition this validation get triggered. For example, you want to allow only TrendType “purpose” with IsOnlineOffline flag value “1”, otherwise any value of TrendType.

```
{
          "type": "multiExpression",
          "condition": "({0} AND {1}) OR {2}",
          "error": "Incorrect parameters combination. IsOnline flag works with 'Purpose' trend type only.",
          "errorCode": "IsOnlineOfflineValidation",
          "expressions": [
            {
              "inputName": "TrendType",
              "condition": "!=",
              "value": "Purpose"
            },
            {
              "inputName": "IsOnlineOffline",
              "condition": "!=",
              "value": "1"
            },
            {
              "inputName": "TrendType",
              "condition": "=",
              "value": "Purpose"
            }
          ]
}
```

### Custom validation

For this validation, user allowed to write a new class, implementing the interface "IBusinessValidation". For example, you have some custom logic to be checked and you write a class named MyValidation in api.framework.net.business under BusinessValidations folder and implement the interface IBusinessValidation, which has a single method Validate. Write your logic in the Validate method.

```
{
     "type": "custom",
     "name": "MyValidation",
     "error": "Incorrect value provided.",
     "errorCode": "InvalidValue"
}
```

Here in the Validate method you will get all inputs with values as JObject (JSON Type).

## Define Operations

We can add the backend operations in this section. This configuration is of array type, so we can define multiple operations for an API endpoint.

| Property Name | Type | Description |
| ---- | ---- | ----|
| order | short | **Required**. This property defines the order for the operation, in which they will be executed.|
| type | string | **Required**. This property defines the type of the operation as string. Supported values for type are sp or business. Here sp is SQL stored procedure, business stands for business operation. |
| value | string | **Optional**. This property defines the stored procedure name for sp type, business class name for business type.|
| db | string | **Optional**. This property defines the name of the database key, which is used to read the SQL connection string from the AppSettings of Web.Config file. |
| inputs | List<DbInput> | **Optional**. This property is required for only sp type operation. As the name says, it defines the inputs for the stored procedure. |
| output | object | **Optional**. This property defines the output schema of the operation. If we do not specify this, it will send the complete output as it is, but using this property we can filter the schema. For example, if a sp return 10 fields, and we need only 5 we can write only those five as JSON of string array. |


### Define Stored Procedure Operation

If we have to call a stored procedure as the backend of an API endpoint, we can use the sp operation, defined in the API endpoint configuration file as below:

```
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
```

### Define Inputs

As explained in above example for the sp type operation we need to define the inputs. An input has the following properties.

| Property Name | Type | Description |
| ---- | ---- | ----|
| source | string | **Required**. This property defines the source for the value of the input. Possible values are client/data/authClaim/none.|
|name | string | **Required**. This property defines the name of the SQL parameter. |
| type | string | **Required**. This property defines the SQL type of input. |
| mapId | string | **Required**. This property defines from where the value for the input is taken. If source is client, then the mapId will be matched with the API input id. If source is data, then the value will be mapped from response of previous operation, and mapId will be operation{order}.{property_name}.|
| optional | bool | **Optional**. This property defines that if this input is optional for the stored procedure. Default value is false. |
| allowMultiple | bool | **Optional**. This property specifies, that this input can have multiple values. If value is true then, if the source of value map to multiple values, then they will be joined as coma (,) separated values. If false, then operation will be executed for each value and result will be merged. Default value is false. |
| defaultValue | string | **Optional**. This property required only for input of source none. That means, we do not have to map the value from any source and read it from this property.|
| direction | string | **Optional**. This property defines the direction of the input, it supports the values Input / Output. Default value is Input |

### Define Output

Output is an object type property, which define the structure/schema of the output for any operation. Here we will talk about the stored procedure type operations. Stored procedure mostly returns the record set(s), we can say dataset in dotnet terminology, that could have one or multiple tables in it. So, output property will have one or more properties having array of column names returned by stored procedure. Here following points are important while defining this property:

- Column names are case sensitive, so we need to be cautious while defining these, as we will get null values in the value if incorrectly defined.

- We can choose the column names, if we want to restrict some data to be published as API output.

- In API output the column names will be camel cased automatically with first letter in small case.

### Define Business Operation

For defining a business operation in API framework, first we have to write a class in “APIFramework.Net.Business” project, under “BusinessLogic” folder. The class should implement IBusinessGenericTransform interface, which has only one method as shown below:

JObject Transform(JObject response, ref JObject inputs);

## Define Mocks for stored procedures

As we can independently work on defining API configuration, and writing stored procedures, for a new API development. So, there may be chances that actual stored procedure development took longer than defining the frontend configuration. To tackle this scenario partially, we have the mocking feature in the framework, where we can mock the stored procedure output, with scenario based. But limitation for this mocking is that we should be aware of the procedure output schema, so if we have the output schema for the API endpoint from the database team, we can follow the following steps to mock the endpoint.

![Mockup](https://gitlab.nippon-hcl.com/nippon_poc/opensource.dotnet.api.framework/-/raw/master/public/mock_setup_example.png)`

## Calling Mocked Endpoints

If you want to call the mocked endpoint with the mock data, you have to add the following 2 request headers.

- **mock**: used as flag to indicate that you want to execute the mock.

- **mock_scenario**: name of the scenario, you want to run and already setup.





