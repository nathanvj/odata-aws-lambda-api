Repository to reproduce issue when deploying OData API to AWS Lambda
See [https://github.com/aws/aws-lambda-dotnet/issues/1473](https://github.com/aws/aws-lambda-dotnet/issues/1473).

---
**Steps**
1. Clone
2. Run app locally  
3. Test that all OData expressions work
4. Deploy .NET API to AWS Lambda  
For example using command line:  
a. Navigate to solution  
b. Run `dotnet lambda deploy-function`  
c. Follow steps
5. Do step 3 again, but now using the deployed API url  
a. Notice that whenever you execute the API request `/api/v1/tickets` it works fine.  
b. Notice that whenever you add an OData expression (e.g. `/api/v1/tickets?$top=1`) it returns the following response:
```json
{
  "message": null
}
```
  
---
Some example OData requests, for people not familiar with OData:
- `/api/v1/tickets?$top=3`
- `/api/v1/tickets?$orderBy=createdAt`
- `/api/v1/tickets?$skip=1&$top=1`

